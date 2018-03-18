using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CSM.Common.Resources;
using CSM.Common.Securities;
using CSM.Common.Utilities;
using CSM.Data.DataAccess;
using CSM.Entity;
using CSM.Service.Messages.Common;
using CSM.Service.Messages.SchedTask;
using log4net;
using LumiSoft.Net.Mime;
using LumiSoft.Net.POP3.Client;
using System.Text.RegularExpressions;
using System.Globalization;

namespace CSM.Business
{
    public sealed class CommPoolFacade : ICommPoolFacade
    {
        private object sync = new Object();
        private ICommonFacade _commonFacade;
        private ValidationContext vc = null;
        private readonly CSMContext _context;
        private ICommPoolDataAccess _commPoolDataAccess;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CommPoolFacade));

        public CommPoolFacade()
        {
            _context = new CSMContext();
        }

        public JobTaskResult AddMailContent(string hostname, int port, bool useSsl, PoolEntity pool)
        {
            JobTaskResult taskResult = new JobTaskResult();
            taskResult.StatusResponse = new StatusResponse();
            taskResult.Username = pool.Email;

            try
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Add Mail Content").Add("Username", pool.Email).ToInputLogString());

                if (string.IsNullOrWhiteSpace(pool.Password))
                {
                    throw new CustomException("Password is required");
                }

                int refNumOfSR = 0;
                int refNumOfFax = 0;
                int refNumOfWeb = 0;
                int refNumOfEmail = 0;
                List<string> mailMsgIds;

                _commPoolDataAccess = new CommPoolDataAccess(_context);
                List<CommunicationPoolEntity> mailList = this.FetchAllMessages(hostname, port, useSsl, pool, out mailMsgIds);

                taskResult.TotalEmailRead = mailList.Count;
                bool success = _commPoolDataAccess.AddMailContent(mailList, ref refNumOfSR, ref refNumOfFax, ref refNumOfWeb, ref refNumOfEmail);
                taskResult.NumOfSR = refNumOfSR;
                taskResult.NumOfFax = refNumOfFax;
                taskResult.NumOfKKWebSite = refNumOfWeb;
                taskResult.NumOfEmail = refNumOfEmail;

                if (success)
                {
                    taskResult.StatusResponse.Status = Constants.StatusResponse.Success;
                    taskResult.StatusResponse.Description = string.Empty;

                    #region "Delete mail list from user's mailboxes"

                    this.DeleteMessageOnServer(hostname, port, useSsl, pool, mailList, mailMsgIds, taskResult);

                    #endregion
                }
                else
                {
                    taskResult.StatusResponse.Status = Constants.StatusResponse.Failed;
                    taskResult.StatusResponse.Description = "Failed to save data";
                }

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Add Mail Content").Add("MailList Size", mailList.Count).ToSuccessLogString());
                return taskResult;
            }
            catch (CustomException cex)
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Add Mail Content").Add("Error Message", cex.Message).ToFailLogString());
                Logger.Error("Exception occur:\n", cex);
                taskResult.StatusResponse.Status = Constants.StatusResponse.Failed;
                taskResult.StatusResponse.Description = cex.Message;
            }
            catch (Exception ex)
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Add Mail Content").Add("Error Message", ex.Message).ToFailLogString());
                Logger.Error("Exception occur:\n", ex);
                taskResult.StatusResponse.Status = Constants.StatusResponse.Failed;
                taskResult.StatusResponse.Description = ex.Message;
            }

            return taskResult;
        }

        public IEnumerable<PoolEntity> GetPoolList(PoolSearchFilter searchFilter)
        {
            _commPoolDataAccess = new CommPoolDataAccess(_context);
            return _commPoolDataAccess.GetPoolList(searchFilter);
        }

        public List<PoolBranchEntity> GetPoolBranchList(int poolId)
        {
            _commPoolDataAccess = new CommPoolDataAccess(_context);
            return _commPoolDataAccess.GetPoolBranchList(poolId);
        }

        public List<PoolBranchEntity> GetPoolBranchList(List<PoolBranchEntity> poolBranches)
        {
            _commPoolDataAccess = new CommPoolDataAccess(_context);
            return _commPoolDataAccess.GetPoolBranchList(poolBranches);
        }

        public bool IsDuplicateCommPool(PoolEntity poolEntity)
        {
            _commPoolDataAccess = new CommPoolDataAccess(_context);
            return _commPoolDataAccess.IsDuplicateCommPool(poolEntity);
        }

        public bool SaveCommPool(PoolEntity poolEntity, List<PoolBranchEntity> poolBranches)
        {
            _commPoolDataAccess = new CommPoolDataAccess(_context);
            return _commPoolDataAccess.SaveCommPool(poolEntity, poolBranches);
        }

        public PoolEntity GetPoolByID(int commPoolId)
        {
            _commPoolDataAccess = new CommPoolDataAccess(_context);
            return _commPoolDataAccess.GetPoolByID(commPoolId);
        }

        public IDictionary<string, string> GetJobStatusSelectList()
        {
            return this.GetJobStatusSelectList(null);
        }

        public IDictionary<string, string> GetJobStatusSelectList(string textName, int? textValue = null)
        {
            _commPoolDataAccess = new CommPoolDataAccess(_context);
            var list = _commPoolDataAccess.GetAllJobStatuses();

            if (!string.IsNullOrWhiteSpace(textName))
            {
                list.Insert(0, new StatusEntity { StatusValue = textValue, StatusName = textName });
            }

            return (from x in list
                    select new
                    {
                        key = x.StatusValue.ToString(),
                        value = x.StatusName
                    }).ToDictionary(t => t.key, t => t.value);
        }

        public IDictionary<string, string> GetSRStatusSelectList()
        {
            return this.GetSRStatusSelectList(null);
        }

        public IDictionary<string, string> GetSRStatusSelectList(string textName, int? textValue = null)
        {
            _commPoolDataAccess = new CommPoolDataAccess(_context);
            var list = _commPoolDataAccess.GetAllSRStatuses();

            if (!string.IsNullOrWhiteSpace(textName))
            {
                list.Insert(0, new StatusEntity { StatusValue = textValue, StatusName = textName });
            }

            return (from x in list
                    select new
                    {
                        key = x.StatusValue.ToString(),
                        value = x.StatusName
                    }).ToDictionary(t => t.key, t => t.value);
        }

        public IDictionary<string, string> GetChannelSelectList()
        {
            return this.GetChannelSelectList(null);
        }

        public IDictionary<string, string> GetChannelSelectList(string textName, int textValue = 0)
        {
            _commPoolDataAccess = new CommPoolDataAccess(_context);
            var list = _commPoolDataAccess.GetActiveChannels();

            if (!string.IsNullOrWhiteSpace(textName))
            {
                list.Insert(0, new ChannelEntity { ChannelId = textValue, Name = textName });
            }

            return (from x in list
                    select new
                    {
                        key = x.ChannelId.ToString(CultureInfo.InvariantCulture),
                        value = x.Name
                    }).ToDictionary(t => t.key, t => t.value);
        }

        public IDictionary<string, string> GetChannelWithEmailSelectList(string textName, int textValue = 0)
        {
            _commPoolDataAccess = new CommPoolDataAccess(_context);
            var list = _commPoolDataAccess.GetActiveChannels();
            list = list.Where(x => !string.IsNullOrEmpty(x.Email)).ToList(); // Email not null

            if (!string.IsNullOrWhiteSpace(textName))
            {
                list.Insert(0, new ChannelEntity { ChannelId = textValue, Name = textName });
            }

            return (from x in list
                    select new
                    {
                        key = x.ChannelId.ToString(CultureInfo.InvariantCulture),
                        value = x.Name
                    }).ToDictionary(t => t.key, t => t.value);
        }

        public IEnumerable<CommunicationPoolEntity> SearchJobs(CommPoolSearchFilter searchFilter)
        {
            _commPoolDataAccess = new CommPoolDataAccess(_context);
            return _commPoolDataAccess.SearchJobs(searchFilter);
        }

        public CommunicationPoolEntity GetJob(int jobId)
        {
            _commPoolDataAccess = new CommPoolDataAccess(_context);
            return _commPoolDataAccess.GetJob(jobId);
        }

        public List<PoolEntity> GetActivePoolList()
        {
            _commPoolDataAccess = new CommPoolDataAccess(_context);
            return _commPoolDataAccess.GetActivePoolList();
        }

        public bool UpdateJob(int? jobId, int userId, int? status, string remark)
        {
            _commPoolDataAccess = new CommPoolDataAccess(_context);
            return _commPoolDataAccess.UpdateJob(jobId, userId, status, remark);
        }

        public bool SaveNewSR(int jobId, int userId, ref int srId)
        {
            _commPoolDataAccess = new CommPoolDataAccess(_context);
            return _commPoolDataAccess.SaveNewSR(jobId, userId, ref srId);
        }

        public AttachmentEntity GetAttachmentsById(int attachmentId)
        {
            _commPoolDataAccess = new CommPoolDataAccess(_context);
            return _commPoolDataAccess.GetAttachmentsById(attachmentId);
        }

        private List<CommunicationPoolEntity> FetchAllMessages(string hostname, int port, bool useSsl, PoolEntity pool, out List<string> mailMsgIds)
        {
            var allMessages = new List<CommunicationPoolEntity>();

            _commPoolDataAccess = new CommPoolDataAccess(_context);
            List<ChannelEntity> channels = _commPoolDataAccess.GetActiveChannels();

            // Authenticate ourselves towards the server
            string decryptedstring = StringCipher.Decrypt(pool.Password, Constants.PassPhrase);
            List<Mime> mailMessages = this.GetEmails(hostname, port, useSsl, pool.Email, decryptedstring, out mailMsgIds);

            //// Get the number of messages in the inbox
            //int messageCount = mailMessages != null ? mailMessages.Count : 0;
            //int maxRetriveMail = this.GetMaxRetrieveMail();
            //int totalEmails = messageCount > maxRetriveMail ? maxRetriveMail : messageCount;
            int totalEmails = mailMessages != null ? mailMessages.Count : 0;

            if (totalEmails <= 0)
            {
                goto Outer;
            }

            //Messages are numbered in the interval: [1, messageCount]
            //Ergo: message numbers are 1-based.
            //Most servers give the latest message the highest number
            Task.Factory.StartNew(() => Parallel.For(0, totalEmails, new ParallelOptions { MaxDegreeOfParallelism = WebConfig.GetTotalCountToProcess() },
                k =>
                {
                    lock (sync)
                    {
                        Mime message = mailMessages[k];
                        CommunicationPoolEntity mail = GetMailContent(message, k);
                        mail.PoolEntity = pool;

                        vc = new ValidationContext(mail, null, null);
                        var validationResults = new List<ValidationResult>();
                        bool valid = Validator.TryValidateObject(mail, vc, validationResults, true);

                        if (!valid)
                        {
                            string errorMsg = validationResults.Select(x => x.ErrorMessage).Aggregate((i, j) => i + Environment.NewLine + j);
                            Logger.DebugFormat("I:--START--:--Validate Email Subject--:Subject/{0}:Sender:/{1}:Error Message/{2}", mail.Subject, mail.SenderAddress, errorMsg);
                        }
                        else
                        {
                            string mailSubject = mail.Subject.Replace(" ", String.Empty);
                            Match match = Regex.Match(mailSubject, @";\s*[0-9]{1,}\s*;", RegexOptions.IgnoreCase);
                            var emailChannel = channels.FirstOrDefault(x => Constants.ChannelCode.Email.Equals(x.Code));

                            if (emailChannel == null)
                            {
                                Logger.ErrorFormat("O:--FAILED--:--Do not configure email channel--:EmailChannelCode/{0}", Constants.ChannelCode.Email);
                                throw new CustomException("ERROR: Do not configure email channel");
                            }

                            if (!match.Success)
                            {
                                mail.ChannelEntity = channels.FirstOrDefault(x => x.Email == mail.SenderAddress) ?? emailChannel;

                                #region "Get Contact Name from KKWebsite"

                                if (Constants.ChannelCode.KKWebSite.Equals(mail.ChannelEntity.Code))
                                {
                                    string s = mail.PlainText;

                                    if (!string.IsNullOrWhiteSpace(s))
                                    {
                                        Logger.Debug("I:--START--:--Get Contact Name from KKWebsite--");

                                        try
                                        {
                                            IList<object> lines = StringHelpers.ConvertStringToList(s, '\n');
                                            mail.ContactName = (lines.FirstOrDefault(x => x.ToString().Contains(Resource.Lbl_CommFirstname)) as string).ExtractDataField(Resource.Lbl_CommFirstname);
                                            mail.ContactSurname = (lines.FirstOrDefault(x => x.ToString().Contains(Resource.Lbl_CommSurname)) as string).ExtractDataField(Resource.Lbl_CommSurname);
                                            Logger.DebugFormat("I:--SUCCESS--:--Get Contact Name from KKWebsite--:Contact Name/{0}:Contact Surname/{1}", mail.ContactName, mail.ContactSurname);
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.DebugFormat("O:--FAILED--:--Get Contact Name from KKWebsite--:MainContent/{0}:Error Message/{1}", s, ex.Message);
                                            Logger.Error("Exception occur:\n", ex);
                                        }
                                    }

                                    // Clear value
                                    mail.PlainText = null;
                                }

                                #endregion
                            }
                            else
                            {
                                mail.SRNo = mailSubject.ExtractSRNo();
                                mail.SRStatusCode = mailSubject.ExtractSRStatus();
                                mail.ChannelEntity = emailChannel;
                            }

                            allMessages.Add(mail);
                        }
                    }
                })).Wait();

            // Now return the fetched messages
        Outer:
            var orderedList = allMessages.OrderBy(x => x.SendDateTime).ToList();
            return orderedList;
        }

        private CommunicationPoolEntity GetMailContent(Mime message, int messageNumber)
        {
            CommunicationPoolEntity mail = null;

            if (message != null)
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Mail Content").Add("Subject", message.MainEntity.Subject).Add("From", message.MainEntity.From)
                    .Add("MessageId", message.MainEntity.MessageID).Add("MessageNumber", messageNumber).Add("SendDateTime", message.MainEntity.Date).ToInputLogString());

                mail = new CommunicationPoolEntity();
                mail.SenderAddress = message.MainEntity.From.Mailboxes[0] != null ? message.MainEntity.From.Mailboxes[0].EmailAddress.NullSafeTrim() : "-";
                mail.Subject = message.MainEntity.Subject.RemoveExtraSpaces();
                mail.Content = ApplicationHelpers.StripHtmlTags(GetMailMessage(message));
                mail.PlainText = ApplicationHelpers.RemoveHtmlTags(FindPlainTextInMessage(message));
                mail.MessageNumber = messageNumber;
                mail.SendDateTime = message.MainEntity.Date;

                MimeEntity[] list = message.Attachments;
                foreach (MimeEntity part in list)
                {
                    byte[] bytes;
                    using (var memory = new MemoryStream())
                    {
                        part.DataToStream(memory);
                        bytes = StreamDataHelpers.ReadDataToBytes(memory);
                    }

                    IList<object> result = StringHelpers.ConvertStringToList(part.ContentTypeString, ';');
                    var attachment = new AttachmentEntity();
                    attachment.ContentType = (string)result[0];
                    attachment.ByteArray = bytes;

                    if (!string.IsNullOrWhiteSpace(part.ContentDisposition_FileName))
                    {
                        attachment.Filename = part.ContentDisposition_FileName;
                    }
                    else
                    {
                        attachment.Filename = part.ContentType_Name;
                    }

                    mail.Attachments.Add(attachment);
                }

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Mail Body").ToSuccessLogString());
            }
            else
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Mail Content").Add("Error Message", "Message is null").ToFailLogString());
            }

            return mail;
        }

        private static string GetMailMessage(Mime message)
        {
            string html = FindHtmlInMessage(message);
            string plainText = FindPlainTextInMessage(message);
            return !string.IsNullOrWhiteSpace(html) ? html : plainText;
        }

        private static string FindPlainTextInMessage(Mime message)
        {
            if (!string.IsNullOrWhiteSpace(message.BodyText))
            {
                return message.BodyText;
            }

            return string.Empty;
        }

        private static string FindHtmlInMessage(Mime message)
        {
            if (!string.IsNullOrWhiteSpace(message.BodyHtml))
            {
                return message.BodyHtml;
            }

            return string.Empty;
        }

        private void DeleteMessageOnServer(string hostname, int port, bool useSsl, PoolEntity pool, List<CommunicationPoolEntity> mailList,
            List<string> mailMsgIds, JobTaskResult taskResult)
        {
            if (mailList != null && mailList.Count > 0)
            {
                Task.Factory.StartNew(() => Parallel.ForEach(mailList,
                    new ParallelOptions { MaxDegreeOfParallelism = WebConfig.GetTotalCountToProcess() },
                    x =>
                    {
                        lock (sync)
                        {
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Mail On Server").Add("Subject", x.Subject)
                                .Add("SenderAddress", x.SenderAddress).Add("MessageNumber", x.MessageNumber).ToInputLogString());

                            try
                            {
                                string decryptedstring = StringCipher.Decrypt(pool.Password, Constants.PassPhrase);
                                this.DeleteMail(hostname, port, useSsl, pool.Email, decryptedstring, mailMsgIds);
                                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Mail On Server").ToSuccessLogString());
                            }
                            catch (Exception ex)
                            {
                                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Mail On Server").Add("Error Message", ex.Message).ToFailLogString());
                                Logger.Error("Exception occur:\n", ex);

                                _commPoolDataAccess = new CommPoolDataAccess(_context);
                                if (_commPoolDataAccess.DeleteMailContent(x, taskResult))
                                {
                                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Communication Pool").Add("JobId", x.JobId).Add("SRId", x.SRId).ToSuccessLogString());
                                }
                                else
                                {
                                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Communication Pool").Add("JobId", x.JobId).Add("SRId", x.SRId).ToFailLogString());
                                }
                            }
                        }
                    })).Wait();

                // Override status response
                if (taskResult.NumFailedDelete > 0)
                {
                    taskResult.StatusResponse.Status = Constants.StatusResponse.Failed;
                    taskResult.StatusResponse.Description = "Unable to delete emails";
                }
            }
        }

        private int GetMaxRetrieveMail()
        {
            _commonFacade = new CommonFacade();
            ParameterEntity param = _commonFacade.GetCacheParamByName(Constants.ParameterName.MaxRetrieveMail);
            return param != null ? param.ParamValue.ToNullable<int>().Value : 200;
        }

        #region "LumiSoft.Net"

        private List<Mime> GetEmails(string pop3Server, int pop3Port, bool pop3UseSsl, string username, string password, out List<string> mailMsgIds)
        {
            mailMsgIds = new List<string>();
            List<Mime> result = new List<Mime>();
            List<string> gotEmailIds = new List<string>();

            using (POP3_Client pop3 = new POP3_Client())
            {
                pop3.Connect(pop3Server, pop3Port, pop3UseSsl);
                pop3.Authenticate(username, password, false);
                POP3_ClientMessageCollection infos = pop3.Messages;
                int maxRetriveMail = this.GetMaxRetrieveMail();
                int messageCount = infos != null ? infos.Count : 0;
                int totalEmails = messageCount > maxRetriveMail ? maxRetriveMail : messageCount;

                //foreach (POP3_ClientMessage info in infos)
                for (int i = totalEmails; i > 0; i--)
                {
                    var info = infos[i - 1];
                    if (gotEmailIds.Contains(info.UID))
                        continue;
                    byte[] bytes = info.MessageToByte();
                    gotEmailIds.Add(info.UID);
                    Mime mime = Mime.Parse(bytes);
                    result.Add(mime);
                }

                mailMsgIds.AddRange(gotEmailIds);
            }

            return result;
        }

        private void DeleteMail(string pop3Server, int pop3Port, bool pop3UseSsl, string username, string password, List<string> mailMsgIds)
        {
            using (POP3_Client c = new POP3_Client())
            {
                c.Connect(pop3Server, pop3Port);
                c.Authenticate(username, password, pop3UseSsl);

                if (c.Messages.Count > 0)
                {
                    foreach (POP3_ClientMessage mail in c.Messages)
                    {
                        if (mailMsgIds.Contains(mail.UID))
                        {
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Mark For Deletion").Add("UID", mail.UID));
                            mail.MarkForDeletion();
                        }
                    }
                }
            }
        }

        #endregion

        #region "IDisposable"

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (_context != null) { _context.Dispose(); }
                    if (_commonFacade != null) { _commonFacade.Dispose(); }
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}