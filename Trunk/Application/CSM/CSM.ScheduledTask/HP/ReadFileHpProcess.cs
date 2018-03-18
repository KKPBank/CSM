using System.Threading;
using System.Threading.Tasks;
using CSM.Common.Utilities;
using System;
using CSM.ScheduledTask.CSMFileService;
using CSM.ScheduledTask.Utilities;
using log4net;

namespace CSM.ScheduledTask.HP
{
    public class ReadFileHpProcess
    {
        private static TaskMailSender _mailSender;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReadFileHpProcess));
        public static void GetFileHpJobAsync()
        {
            try
            {
                Logger.Info("I:--START--");

                Task<ImportHpTaskResponse> task;
                using (var client = new CSMFileServiceClient())
                {
                    task = client.GetFileHPAsync(WebConfig.GetTaskUsername(), WebConfig.GetTaskPassword());
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
                    _mailSender.NotifyImportHPFailed(WebConfig.GetTaskEmailToAddress(), DateTime.Now, task.Exception);
                    Thread.Sleep(5000);
                }
                //else
                //{
                //    var result = task.GetAwaiter().GetResult();
                //    Logger.InfoFormat("O:--SUCCESS--:Task Result/{0}", result);

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
                _mailSender.NotifyImportHPFailed(WebConfig.GetTaskEmailToAddress(), DateTime.Now, new AggregateException(ex));
                Thread.Sleep(5000);
            }
        }

        private static void SendMailTask(ImportHpTaskResponse result)
        {
            _mailSender = TaskMailSender.GetTaskMailSender();
            if (Constants.StatusResponse.Success.Equals(result.StatusResponse.Status) && result.NumOfError > 0)
            {
                _mailSender.NotifyImportHPSuccess(WebConfig.GetTaskEmailToAddress(), result);
            }

            #region "กรณี File not found ไม่ส่งเมล์"
            //if (Constants.StatusResponse.Failed.Equals(result.StatusResponse.Status) && result.FileList.Length > 0)
            //{
            //    if (!string.IsNullOrEmpty(result.FileList[0].ToString()))
            //    {
            //        _mailSender.NotifyImportHPFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime, result.StatusResponse.Description);
            //    }
            //}
            #endregion

            if (Constants.StatusResponse.Failed.Equals(result.StatusResponse.Status))
            {
                _mailSender.NotifyImportHPFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime, result.StatusResponse.Description);                
            }

            Thread.Sleep(5000);
        }
    }
}
