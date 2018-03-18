using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using LumiSoft.Net.Mail;
using LumiSoft.Net.POP3.Client;
using System.Text.RegularExpressions;
using System.Globalization;
using LumiSoft.Net.MIME;
using System.Collections.Concurrent;

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
                if (string.IsNullOrWhiteSpace(pool.Password))
                {
                    throw new CustomException("Password is required");
                }

                int refNumOfSR = 0;
                int refNumOfFax = 0;
                int refNumOfWeb = 0;
                int refNumOfEmail = 0;
                List<string> mailMsgIds = null;

                _commPoolDataAccess = new CommPoolDataAccess(_context);
                List<CommunicationPoolEntity> mailList = this.FetchAllMessages(hostname, port, useSsl, pool);

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Add Mail Content").Add("Username", pool.Email).Add("MailList Size", mailList.Count).ToInputLogString());

                taskResult.TotalEmailRead = mailList.Count;
                bool success = _commPoolDataAccess.AddMailContent(mailList, ref refNumOfSR, ref refNumOfFax, ref refNumOfWeb, ref refNumOfEmail);
                taskResult.NumOfSR = refNumOfSR;
                taskResult.NumOfFax = refNumOfFax;
                taskResult.NumOfKKWebSite = refNumOfWeb;
                taskResult.NumOfEmail = refNumOfEmail;
                taskResult.NumFailedDelete = mailList.Count(x => x.IsDeleted == false);

                if (success)
                {
                    if (mailList.Any(x => x.IsDeleted))
                    {
                        mailMsgIds = mailList.Where(x => x.IsDeleted).Select(x => x.UID).ToList();

                        #region "Delete mail list from user's mailboxes"

                        this.DeleteMessageOnServer(hostname, port, useSsl, pool, mailList, mailMsgIds, taskResult);

                        #endregion
                    }

                    if (taskResult.NumFailedDelete == 0)
                    {
                        taskResult.StatusResponse.Status = Constants.StatusResponse.Success;
                        taskResult.StatusResponse.Description = string.Empty;
                    }
                    else
                    {
                        taskResult.StatusResponse.Status = Constants.StatusResponse.Failed;
                        taskResult.StatusResponse.Description = "Unable to delete emails";
                    }

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Add Mail Content").ToSuccessLogString());
                }
                else
                {
                    taskResult.StatusResponse.Status = Constants.StatusResponse.Failed;
                    taskResult.StatusResponse.Description = "Failed to save data";
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Add Mail Content").Add("Error Message", taskResult.StatusResponse.Description).ToFailLogString());
                }

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

        private List<CommunicationPoolEntity> FetchAllMessages(string hostname, int port, bool useSsl, PoolEntity pool)
        {
            var allMessages = new List<CommunicationPoolEntity>();

            _commPoolDataAccess = new CommPoolDataAccess(_context);
            List<ChannelEntity> channels = _commPoolDataAccess.GetActiveChannels();

            // Authenticate ourselves towards the server
            string decryptedstring = StringCipher.Decrypt(pool.Password, Constants.PassPhrase);
            ConcurrentDictionary<string, Mail_Message> mailMessages = this.GetEmails(hostname, port, useSsl, pool.Email, decryptedstring);

            // Get the number of messages in the inbox
            int k = 0;
            int totalEmails = mailMessages != null ? mailMessages.Count : 0;

            if (totalEmails <= 0)
            {
                goto Outer;
            }

            //Messages are numbered in the interval: [1, messageCount]
            //Ergo: message numbers are 1-based.
            //Most servers give the latest message the highest number
            Task.Factory.StartNew(() => Parallel.ForEach(mailMessages, new ParallelOptions { MaxDegreeOfParallelism = WebConfig.GetTotalCountToProcess() },
                kvp =>
                {
                    lock (sync)
                    {
                        try
                        {
                            string msgKey = kvp.Key;
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Prepared CommuPool Object").Add("KeyPairValue (UID)", (!string.IsNullOrWhiteSpace(msgKey) ? msgKey : "NULL")).ToInputLogString());

                            Mail_Message message;
                            mailMessages.TryGetValue(msgKey, out message);

                            CommunicationPoolEntity mail = GetMailContent(message, k);
                            mail.PoolEntity = pool;
                            mail.UID = msgKey;

                            string mailSubject = !string.IsNullOrWhiteSpace(mail.Subject) ? mail.Subject.Replace(" ", string.Empty) : string.Empty;
                            Match match = Regex.Match(mailSubject, @";\s*[0-9]{1,}\s*;", RegexOptions.IgnoreCase);
                            var emailChannel = channels.FirstOrDefault(x => Constants.ChannelCode.Email.Equals(x.Code));

                            if (emailChannel == null)
                            {
                                Logger.ErrorFormat("O:--FAILED--:--Do not configure email channel--:EmailChannelCode/{0}", Constants.ChannelCode.Email);
                                throw new CustomException("ERROR: Do not configure email channel");
                            }

                            if (match.Success && ApplicationHelpers.IsValidEmailDomain(mail.SenderAddress))
                            {
                                mail.SRNo = mailSubject.ExtractSRNo();
                                mail.SRStatusCode = mailSubject.ExtractSRStatus();
                                mail.ChannelEntity = emailChannel;
                            }
                            else
                            {
                                mail.ChannelEntity = channels.FirstOrDefault(x => x.Email != null && x.Email.ToUpperInvariant() == mail.SenderAddress.ToUpperInvariant()) ?? emailChannel;

                                #region "Get Contact Name from KKWebsite"

                                if (mail.ChannelEntity != null && Constants.ChannelCode.KKWebSite.Equals(mail.ChannelEntity.Code))
                                {
                                    string s = mail.PlainText;

                                    if (!string.IsNullOrWhiteSpace(s))
                                    {
                                        Logger.DebugFormat("I:--START--:--Get Contact Name from KKWebsite--:PlainText/{0}", s);

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

                            allMessages.Add(mail);
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Prepared CommuPool Object").ToSuccessLogString());
                        }
                        catch (Exception ex)
                        {
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Prepared CommuPool Object").Add("Error Message", ex.Message).ToFailLogString());
                            Logger.Error("Exception occur:\n", ex);
                        }

                        k++;
                    }
                })).Wait();

        // Now return the fetched messages
        Outer:
            var orderedList = allMessages.OrderBy(x => x.RecvDateTime).ToList();
            return orderedList;
        }

        private CommunicationPoolEntity GetMailContent(Mail_Message message, int messageNumber)
        {
            CommunicationPoolEntity mail = null;

            if (message != null)
            {
                string displayName = message.From != null ? message.From[0].DisplayName.NullSafeTrim() : "-";
                string senderAddress = message.From != null ? message.From[0].Address.NullSafeTrim() : "-";
                string senderName = !string.IsNullOrWhiteSpace(displayName) ? displayName : ApplicationHelpers.GetSenderAddress(senderAddress);
                string recvDate = message.Date == DateTime.MinValue ? "date not specified" : message.Date.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Mail Content").Add("Subject", message.Subject).Add("From", senderAddress)
                    .Add("SenderName", senderName).Add("MessageId", message.MessageID).Add("MessageNumber", messageNumber)
                    .Add("RecvDateTime", recvDate).ToInputLogString());

                mail = new CommunicationPoolEntity();
                mail.SenderAddress = senderAddress;
                mail.SenderName = senderName;
                mail.Subject = ApplicationHelpers.StringLimit(message.Subject.RemoveExtraSpaces(), Constants.MaxLength.MailSubject);
                mail.Content = ApplicationHelpers.StripHtmlTags(GetMailMessage(message));
                mail.PlainText = ApplicationHelpers.RemoveHtmlTags(GetMailMessage(message).ReplaceBreaks());
                mail.MessageNumber = messageNumber;
                mail.RecvDateTime = message.Date;

                MIME_Entity[] list = message.GetAttachments(true, true);

                if (list != null && list.Length > 0)
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Mail Attachments").Add("ListSize", list.Length).ToOutputLogString());

                    foreach (MIME_Entity entity in list)
                    {
                        try
                        {
                            if (entity != null)
                            {
                                string fileName;
                                string dispoType;
                                string contentId = entity.ContentID.ExtractContentID();
                                string contentType = entity.ContentType.TypeWithSubtype.ToLowerInvariant();

                                if (entity.ContentDisposition != null)
                                {
                                    fileName = entity.ContentDisposition.Param_FileName;
                                    dispoType = entity.ContentDisposition.DispositionType;

                                    // Retrieve file name from contentType instead.
                                    if (string.IsNullOrWhiteSpace(fileName))
                                    {
                                        fileName = entity.ContentType.Param_Name;
                                    }

                                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Mail Attachment").Add("ContentDisposition", "NOT NULL").Add("ContentId", contentId)
                                        .Add("FileName", fileName).Add("ContentType", contentType).Add("DispositionType", dispoType).ToInputLogString());

                                    if (entity.Body != null && entity.Body is MIME_b_SinglepartBase)
                                    {
                                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Download Attachment").ToInputLogString());
                                        byte[] bytes = ((MIME_b_SinglepartBase)entity.Body).Data;

                                        if (bytes != null && bytes.Length > 0)
                                        {
                                            if ((string.IsNullOrWhiteSpace(contentId) || MIME_DispositionTypes.Attachment.Equals(dispoType)) && !string.IsNullOrWhiteSpace(fileName))
                                            {
                                                var attachment = new AttachmentEntity();
                                                attachment.ContentType = contentType;
                                                attachment.ByteArray = bytes;
                                                attachment.Filename = fileName;
                                                mail.Attachments.Add(attachment);
                                            }
                                            if (!string.IsNullOrWhiteSpace(contentId) && MIME_DispositionTypes.Inline.Equals(dispoType) && contentType.IsImage())
                                            {
                                                // data:image/png;base64,{0}
                                                string base64String = ApplicationHelpers.GetBase64Image(bytes);
                                                string newImg = string.Format(CultureInfo.InvariantCulture, "data:{0};base64,{1}", contentType, base64String);
                                                string srcImg = string.Format(CultureInfo.InvariantCulture, "cid:{0}", contentId);
                                                mail.Content = mail.Content.Replace(srcImg, newImg);
                                                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Attachment").Add("NewImg", newImg).Add("SrcImg", srcImg).ToOutputLogString());
                                            }
                                        }

                                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Download Attachment").ToSuccessLogString());
                                    }
                                }
                                else
                                {
                                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Mail Attachment").Add("ContentDisposition", "NULL").Add("ContentId", contentId)
                                        .Add("ContentType", contentType).ToInputLogString());

                                    if (entity.Body != null && entity.Body is MIME_b_SinglepartBase)
                                    {
                                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Download Attachment").ToInputLogString());
                                        byte[] bytes = ((MIME_b_SinglepartBase)entity.Body).Data;

                                        if (bytes != null && bytes.Length > 0)
                                        {
                                            if (!string.IsNullOrWhiteSpace(contentId) && contentType.IsImage())
                                            {
                                                // data:image/png;base64,{0}
                                                string base64String = ApplicationHelpers.GetBase64Image(bytes);
                                                string newImg = string.Format(CultureInfo.InvariantCulture, "data:{0};base64,{1}", contentType, base64String);
                                                string srcImg = string.Format(CultureInfo.InvariantCulture, "cid:{0}", contentId);
                                                mail.Content = mail.Content.Replace(srcImg, newImg);
                                                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Attachment").Add("NewImg", newImg).Add("SrcImg", srcImg).ToOutputLogString());
                                            }
                                        }
                                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Download Attachment").ToSuccessLogString());
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            mail.IsDeleted = false;
                            Logger.Error("Exception occur:\n", ex);
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Mail Content").Add("Error Message", ex.Message).ToFailLogString());
                        }
                    }
                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Mail Attachments").Add("ListSize", "0").ToOutputLogString());
                }

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Mail Content").Add("Content", mail.Content).ToSuccessLogString());
            }
            else
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Mail Content").Add("Error Message", "Mail Content is null").ToFailLogString());
            }

            return mail;
        }

        private static string GetMailMessage(Mail_Message message)
        {
            string html = FindHtmlInMessage(message);
            string plainText = FindPlainTextInMessage(message);
            return !string.IsNullOrWhiteSpace(html) ? html : plainText;
        }

        private static string FindPlainTextInMessage(Mail_Message message)
        {
            if (!string.IsNullOrWhiteSpace(message.BodyText))
            {
                return message.BodyText;
            }

            return string.Empty;
        }

        private static string FindHtmlInMessage(Mail_Message message)
        {
            if (!string.IsNullOrWhiteSpace(message.BodyHtmlText))
            {
                return message.BodyHtmlText;
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
                            catch (CustomException cex)
                            {
                                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Mail On Server").Add("Error Message", cex.Message).ToFailLogString());

                                _commPoolDataAccess = new CommPoolDataAccess(_context);
                                if (_commPoolDataAccess.DeleteMailContent(x, taskResult))
                                {
                                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Communication Pool").Add("JobId", x.JobId).Add("SRId", x.SRId).ToSuccessLogString());
                                }
                                else
                                {
                                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Communication Pool").Add("JobId", x.JobId).Add("SRId", x.SRId).ToFailLogString());
                                }

                                Logger.Error("CustomException occur:\n", cex);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("Exception occur:\n", ex);
                            }
                        }
                    })).Wait();
            }
        }

        private int GetMaxRetrieveMail()
        {
            _commonFacade = new CommonFacade();
            ParameterEntity param = _commonFacade.GetParamByName(Constants.ParameterName.MaxRetrieveMail);
            return param != null ? param.ParamValue.ToNullable<int>().Value : 200;
        }

        #region "LumiSoft.Net"

        private ConcurrentDictionary<string, Mail_Message> GetEmails(string pop3Server, int pop3Port, bool pop3UseSsl, string username, string password)
        {
            List<string> gotEmailIds = new List<string>();
            ConcurrentDictionary<string, Mail_Message> result = new ConcurrentDictionary<string, Mail_Message>();

            using (POP3_Client pop3 = new POP3_Client())
            {
                pop3.Connect(pop3Server, pop3Port, pop3UseSsl);
                pop3.Login(username, password);
                POP3_ClientMessageCollection infos = pop3.Messages;
                int maxRetriveMail = this.GetMaxRetrieveMail();
                int messageCount = infos != null ? infos.Count : 0;
                int totalEmails = (maxRetriveMail > 0 && messageCount > maxRetriveMail) ? maxRetriveMail : messageCount;
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Emails from Lotus Notes").Add("MaxRetriveMail", totalEmails).ToInputLogString());

                if (infos != null)
                {
                    for (int i = totalEmails; i > 0; i--)
                    {
                        var info = infos[i - 1];
                        if (gotEmailIds.Contains(info.UID))
                            continue;
                        gotEmailIds.Add(info.UID);
                        byte[] messageBytes = info.MessageToByte();

                        if (messageBytes != null && messageBytes.Length > 0)
                        {
                            Mail_Message mimeMessage = Mail_Message.ParseFromByte(messageBytes, new System.Text.UTF8Encoding(false));
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Mail Body").Add("SequenceNumber", i).Add("UID", info.UID).ToOutputLogString());
                            result.TryAdd(info.UID, mimeMessage);
                        }
                        else
                        {
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Mail Body").Add("SequenceNumber", i).Add("Error Message", "Mail Body is null").ToOutputLogString());
                        }
                    }
                }
            }

            return result;
        }

        private void DeleteMail(string pop3Server, int pop3Port, bool pop3UseSsl, string username, string password, List<string> mailMsgIds)
        {
            using (POP3_Client c = new POP3_Client())
            {
                c.Connect(pop3Server, pop3Port, pop3UseSsl);
                c.Login(username, password);

                var totalEmail = c.Messages.Count;
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Mail List").Add("MailList Size", totalEmail).ToOutputLogString());

                if (totalEmail > 0)
                {
                    foreach (POP3_ClientMessage mail in c.Messages)
                    {
                        if (mailMsgIds.Contains(mail.UID))
                        {
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Mail by UID").Add("UID", mail.UID)
                                .Add("SequenceNumber", mail.SequenceNumber).Add("Mail Size", mail.Size).ToInputLogString());

                            try
                            {
                                mail.MarkForDeletion();
                            }
                            catch (POP3_ClientException pex)
                            {
                                Logger.Error("POP3_ClientException occur:\n", pex);
                            }

                            if (mail.IsMarkedForDeletion)
                            {
                                Logger.Info(_logMsg.Clear().SetPrefixMsg("Mark For Deletion").ToSuccessLogString());
                            }
                            else
                            {
                                Logger.Info(_logMsg.Clear().SetPrefixMsg("Mark For Deletion").Add("Error Message", "Failed to delete an email").ToFailLogString());
                                throw new CustomException(string.Format(CultureInfo.InvariantCulture, "Failed to delete an email {0}", mail.UID));
                            }
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