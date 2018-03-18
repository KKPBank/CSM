using System.Threading;
using System.Threading.Tasks;
using CSM.Common.Utilities;
using System;
using CSM.ScheduledTask.CSMFileService;
using CSM.ScheduledTask.Utilities;
using log4net;

namespace CSM.ScheduledTask.AFS
{
    public class ReadFileProcess
    {
        private static TaskMailSender _mailSender;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReadFileProcess));

        public static void GetFileAFSJobAsync()
        {
            try
            {
                Logger.Info("I:--START--");

                Task<ImportAFSTaskResponse> task;
                using (var client = new CSMFileServiceClient())
                {
                    task = client.GetFileAFSAsync(WebConfig.GetTaskUsername(), WebConfig.GetTaskPassword());
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
                    _mailSender.NotifyImportAssetFailed(WebConfig.GetTaskEmailToAddress(), DateTime.Now, task.Exception);
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
                _mailSender.NotifyImportAssetFailed(WebConfig.GetTaskEmailToAddress(), DateTime.Now, new AggregateException(ex));
                Thread.Sleep(5000);
            }
        }

        private static void SendMailTask(ImportAFSTaskResponse result)
        {
            _mailSender = TaskMailSender.GetTaskMailSender();
            if (result.NumOfErrProp > 0 || result.NumOfErrSaleZone > 0)
            {
                _mailSender.NotifyImportAssetSuccess(WebConfig.GetTaskEmailToAddress(), result);
            }
            if (Constants.StatusResponse.Failed.Equals(result.StatusResponse.Status))
            {
                _mailSender.NotifyImportAssetFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime, result.StatusResponse.Description);
            }

            Thread.Sleep(5000);
        }
    }
}
