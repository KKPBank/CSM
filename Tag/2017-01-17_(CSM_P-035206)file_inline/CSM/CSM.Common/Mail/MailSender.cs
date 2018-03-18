using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using CSM.Common.Utilities;
using log4net;

namespace CSM.Common.Mail
{
    public class MailSender
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MailSender));
        private readonly string _authenticationMethod = WebConfig.GetMailAuthenMethod(); // default or basic (for localhost use basic)
        private readonly string _basicAuthenPassword = WebConfig.GetMailAuthenPassword();
        private readonly string _basicAuthenUserName = WebConfig.GetMailAuthenUser();
        private readonly bool _enableMailSender = WebConfig.GetMailEnable();

        private void StartSendMail(object state)
        {
            MailMessage msgMail = null;
            SmtpClient mailClient = null;

            try
            {
                msgMail = (MailMessage)state;
                mailClient = new SmtpClient(WebConfig.GetEmailServer(), WebConfig.GetEmailServerPort());

                if (_authenticationMethod == "default")
                {
                    mailClient.UseDefaultCredentials = true;
                }
                else
                {
                    var basicAuthenticationInfo = new NetworkCredential(_basicAuthenUserName, _basicAuthenPassword);
                    mailClient.Credentials = basicAuthenticationInfo;
                    mailClient.EnableSsl = true;
                }

                mailClient.Send(msgMail);
            }
            catch (Exception e)
            {
                Logger.Error("Failed to Send Email:\n", e);
            }
            finally
            {
                if (mailClient != null) { mailClient.Dispose(); }
                if (msgMail != null) { msgMail.Dispose(); }
            }
        }

        protected bool SendMail(string receiverEmail, string receiverName, string senderEmail, string senderName,
            string subject, string message, string ccEmail, string ccName)
        {
            if (receiverEmail == null || string.IsNullOrEmpty(receiverEmail) || message == null || string.IsNullOrEmpty(message))
            {
                return false;
            }

            var receiver = new MailAddress(receiverEmail, receiverName);
            var sender = new MailAddress(senderEmail, senderName);
            var msgMail = new MailMessage(sender, receiver)
            {
                SubjectEncoding = Encoding.GetEncoding("TIS-620"),
                Subject = subject,
                Body = message,
                BodyEncoding = Encoding.GetEncoding("TIS-620"), //Encoding.UTF8;
                IsBodyHtml = true
            };
            try
            {
                if (!string.IsNullOrEmpty(ccEmail))
                {
                    msgMail.CC.Add(new MailAddress(ccEmail, !string.IsNullOrWhiteSpace(ccName) ? ccName : ccEmail));
                }

                if (_enableMailSender)
                {
                    ThreadPool.QueueUserWorkItem(StartSendMail, msgMail);
                    Logger.Info("sending mail to '" + receiverName + "' at '" + receiverEmail + "' with subject '" + subject + "'");
                }
            }
            finally
            { }
            return true;
        }

        protected bool SendMail(string senderEmail, string receiverEmails, string ccEmails, string subject, string message,
            List<byte[]> attachmentStreams, List<string> attachmentFilenames, bool isSensitive)
        {
            return SendMail(senderEmail, receiverEmails, ccEmails, null, subject, message,
                    attachmentStreams, attachmentFilenames, isSensitive);
        }

        protected bool SendMail(string senderEmail, string receiverEmails, string ccEmails, string bccEmails, string subject, string message,
            List<byte[]> attachmentStreams, List<string> attachmentFilenames, bool isSensitive)
        {
            MailMessage msgMail = new MailMessage()
            {
                From = new MailAddress(senderEmail),
                SubjectEncoding = Encoding.GetEncoding("TIS-620"),
                Subject = subject,
                Body = message,
                BodyEncoding = Encoding.GetEncoding("TIS-620"), //Encoding.UTF8;
                IsBodyHtml = true
            };

            try
            {
                if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(receiverEmails) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
                {
                    return false;
                }

                Logger.Info($"Start Send Mail Subject {subject} To {receiverEmails} CC {ccEmails} BCC {bccEmails}.");

                var receivers = receiverEmails.Split(',').Select(r => r.Trim()).ToList();
                foreach (var receiver in receivers)
                {
                    if (ApplicationHelpers.IsValidEmailAddress(receiver))
                        msgMail.To.Add(receiver);
                    else
                        Logger.Info($"Invalid TO mail address {receiver}.");
                }

                if (isSensitive)
                {
                    msgMail.Headers.Add("Sensitivity", "Private");
                }

                if (attachmentStreams != null && attachmentStreams.Count > 0 && attachmentFilenames != null
                    && attachmentFilenames.Count > 0 && attachmentStreams.Count == attachmentFilenames.Count)
                {
                    var countAttachment = attachmentStreams.Count;
                    for (int i = 0; i < countAttachment; i++)
                    {
                        //using (var ms = new MemoryStream(attachmentStreams[i]))
                        //{
                        //    msgMail.Attachments.Add(new Attachment(ms, attachmentFilenames[i]));
                        //}
                        msgMail.Attachments.Add(new Attachment(new MemoryStream(attachmentStreams[i]), attachmentFilenames[i]));
                    }
                }
                else
                {
                    Logger.WarnFormat("AttachmentStreams is not equals AttachmentFilenames");
                }

                if (!string.IsNullOrEmpty(ccEmails))
                {
                    var ccs = ccEmails.Split(',').Select(r => r.Trim()).ToList();
                    foreach (var cc in ccs)
                    {
                        if (ApplicationHelpers.IsValidEmailAddress(cc))
                            msgMail.CC.Add(new MailAddress(cc));
                        else
                            Logger.Info($"Invalid CC mail address {cc}.");
                    }

                    //if (string.IsNullOrEmpty(_fixDestination))
                    //{
                    //    var ccs = ccEmails.Split(',').Select(r => r.Trim()).ToList();
                    //    foreach (var cc in ccs)
                    //    {
                    //        msgMail.CC.Add(new MailAddress(cc));
                    //    }
                    //}
                    //else
                    //{
                    //    msgMail.CC.Add(new MailAddress(_fixDestination));
                    //}
                }

                if (!string.IsNullOrEmpty(bccEmails))
                {
                    var bccs = bccEmails.Split(',').Select(r => r.Trim()).ToList();
                    foreach (var bcc in bccs)
                    {
                        if (ApplicationHelpers.IsValidEmailAddress(bcc))
                            msgMail.Bcc.Add(new MailAddress(bcc));
                        else
                            Logger.Info($"Invalid BCC mail address {bcc}.");
                    }
                }

                if (_enableMailSender)
                {
                    ThreadPool.QueueUserWorkItem(StartSendMail, msgMail);
                    Logger.Info("sending mail to '" + receiverEmails + "' with subject '" + subject + "'");
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Failure sending mail:\n", ex);
                return false;
            }
            finally
            { }
        }

        protected bool SendMail(string receiverEmail, string receiverName, string senderEmail, string senderName, string subject, string message)
        {
            return SendMail(receiverEmail, receiverName, senderEmail, senderName, subject, message, string.Empty, string.Empty);
        }

        protected bool SendMail(EmailAddress sender, List<EmailAddress> receivers, List<EmailAddress> ccAddresses, string subject, string message, Byte[] byteArray, string attachmentName)
        {
            const string delimeter = ",";
            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(sender.Address, sender.Name),
                SubjectEncoding = Encoding.GetEncoding("TIS-620"),
                Subject = subject,
                Body = message,
                BodyEncoding = Encoding.GetEncoding("TIS-620"), //Encoding.UTF8;
                IsBodyHtml = true
            };

            try
            {
                if (receivers == null || receivers.Count == 0)
                {
                    Logger.Error("Failed to process message. The receiver cannot be null or empty.");
                    return false;
                }

                foreach (EmailAddress user in receivers.Where(user => !string.IsNullOrEmpty(user.Address)))
                {
                    mailMessage.To.Add(new MailAddress(user.Address, user.Name));
                }

                if (ccAddresses != null && ccAddresses.Count > 0)
                {
                    foreach (EmailAddress user in ccAddresses.Where(user => !string.IsNullOrEmpty(user.Address)))
                    {
                        mailMessage.CC.Add(new MailAddress(user.Address, user.Name));
                    }
                }

                if (byteArray != null && byteArray.Length > 0)
                {
                    //using (var ms = new MemoryStream(byteArray))
                    //{
                    //    Attachment attachment = new Attachment(ms, attachmentName);
                    //    mailMessage.Attachments.Add(attachment);
                    //}
                    Attachment attachment = new Attachment(new MemoryStream(byteArray), attachmentName);
                    mailMessage.Attachments.Add(attachment);
                }

                ThreadPool.QueueUserWorkItem(StartSendMail, mailMessage);
                Logger.Info("Sending mail to " + receivers.Select(t => t.Name).Aggregate((i, j) => i + delimeter + j) + " at " + receivers.Select(t => t.Address).Aggregate((i, j) => i + delimeter + j) + subject);

                if (ccAddresses != null && ccAddresses.Count > 0)
                {
                    Logger.Info("Sending mail cc to " + ccAddresses.Select(t => t.Name).Aggregate((i, j) => i + delimeter + j) + " at " + ccAddresses.Select(t => t.Address).Aggregate((i, j) => i + delimeter + j) + " with subject " + subject);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failure sending mail:\n", ex);
                return false;
            }
            finally
            { }
            return true;
        }

        #region "URL Generation"

        private static TextGenerator GetTextGenerator()
        {
            return new TextGenerator();
        }

        protected string GenText(string templateFilePath, Hashtable hData)
        {
            try
            {
                return GetTextGenerator().GenText(templateFilePath, hData);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Logger.Error(e.StackTrace);
                Logger.Error("Exception occur - [" + templateFilePath + "] :\n", e);
                return null;
            }
        }

        #endregion
    }
}