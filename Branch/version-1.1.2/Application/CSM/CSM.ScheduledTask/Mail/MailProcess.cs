using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSM.Common.Utilities;
using CSM.ScheduledTask.CSMMailService;
using System;
using CSM.ScheduledTask.Utilities;
using log4net;

///<summary>
/// Class Name : MailProcess
/// Purpose    : -
/// Author     : Neda Peyrone
///</summary>
///<remarks>
/// Change History:
/// Date         Author           Description
/// ----         ------           -----------
///</remarks>
namespace CSM.ScheduledTask.Mail
{
    public class MailProcess
    {
        private static TaskMailSender _mailSender;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MailProcess));

        public static void GetMailboxJobAsync()
        {
            try
            {
                Logger.Info("I:--START--");

                Task<JobTaskResponse> task;
                using (var client = new CSMMailServiceClient())
                {
                    task = client.GetMailboxAsync(WebConfig.GetTaskUsername(), WebConfig.GetTaskPassword());
                }

                while (!task.IsCompleted)
                {
                    Thread.Sleep(1000);
                }

                if (task.Exception != null)
                {
                    Logger.InfoFormat("O:--FAILED--:Exception/{0}", task.Exception);
                    Logger.Error("Exception occur:\n", task.Exception);

                    // Send mail to system administrator
                    _mailSender = TaskMailSender.GetTaskMailSender();
                    _mailSender.NotifySyncEmailFailed(WebConfig.GetTaskEmailToAddress(), DateTime.Now, task.Exception);
                    Thread.Sleep(5000);
                }
                //else
                //{
                //    var result = task.GetAwaiter().GetResult();
                //    Logger.InfoFormat("O:--SUCCESS--:Schedule DateTime/{0}:", result.SchedDateTime);

                //    // Send mail to system administrator
                //    SendMailTask(result);
                //}
            }
            catch (Exception ex)
            {
                Logger.InfoFormat("O:--FAILED--:Exception/{0}", ex.Message);
                Logger.Error("Exception occur:\n", ex);

                // Send mail to system administrator
                _mailSender = TaskMailSender.GetTaskMailSender();
                _mailSender.NotifySyncEmailFailed(WebConfig.GetTaskEmailToAddress(), DateTime.Now, new AggregateException(ex));
                Thread.Sleep(5000);
            }
        }

        private static void SendMailTask(JobTaskResponse result)
        {
            _mailSender = TaskMailSender.GetTaskMailSender();

            if (result.JobTaskResults != null && result.JobTaskResults.Length > 0)
            {
                foreach (var task in result.JobTaskResults.Where(x => x.StatusResponse.Status == Constants.StatusResponse.Failed))
                {
                    _mailSender.NotifySyncEmailFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime, task.StatusResponse.Description);
                }
            }
            else
            {
                Logger.InfoFormat("O:--Unable to Send Email Notification--:Total Results/{0}", 0);
            }

            Thread.Sleep(5000);
        }
    }
}
