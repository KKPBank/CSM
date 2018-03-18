using CSM.Common.Mail;
using System.Collections.Generic;

namespace CSM.Business
{
    public class CSMMailSender : MailSender
    {
        private static readonly CSMMailSender EmailSender = new CSMMailSender();

        public static CSMMailSender GetCSMMailSender()
        {
            return EmailSender;
        }

        public bool SendMail(string senderEmail, string receiverEmails, string ccEmails, string subject, string message, 
            List<byte[]> attachmentStreams, List<string> attachmentFilenames)
        {
            return base.SendMail(senderEmail, receiverEmails, ccEmails, subject, message, attachmentStreams, attachmentFilenames);
        }
    }
}
