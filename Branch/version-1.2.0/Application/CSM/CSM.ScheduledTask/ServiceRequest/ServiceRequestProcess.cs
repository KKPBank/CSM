using System.Threading;
using System.Threading.Tasks;
using CSM.Common.Utilities;
using System;
using CSM.ScheduledTask.Utilities;
using log4net;
using CSM.ScheduledTask.CSMSRService;

namespace CSM.ScheduledTask.ServiceRequest
{
    public class ServiceRequestProcess
    {
        private static TaskMailSender _mailSender;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ServiceRequestProcess));

        #region == Batch Create SRActivity from Reply Email ==

        public static void CreateSRActivityFromReplyEmail()
        {
            try
            {
                Logger.Info("I:--START--");

                Task<CreateSrFromReplyEmailTaskResponse> task;
                using (var client = new CSMSRServiceClient())
                {
                    task = client.CreateSRActivityFromReplyEmailAsync(WebConfig.GetTaskUsername(),
                        WebConfig.GetTaskPassword());
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
                    _mailSender.NotifyCreateSrFromReplyEmailFailed(WebConfig.GetTaskEmailToAddress(), DateTime.Now, task.Exception);
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
                _mailSender.NotifyCreateSrFromReplyEmailFailed(WebConfig.GetTaskEmailToAddress(), DateTime.Now, new AggregateException(ex));
                Thread.Sleep(5000);
            }
        }

        private static void SendMailTask(CreateSrFromReplyEmailTaskResponse result)
        {
            _mailSender = TaskMailSender.GetTaskMailSender();
            if (Constants.StatusResponse.Success.Equals(result.StatusResponse.Status))
            {
                _mailSender.NotifyCreateSrFromReplyEmailSuccess(WebConfig.GetTaskEmailToAddress(), result);
            }
            if (Constants.StatusResponse.Failed.Equals(result.StatusResponse.Status))
            {
                _mailSender.NotifyCreateSrFromReplyEmailFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime, result.StatusResponse.ErrorCode);
            }

            Thread.Sleep(5000);
        }

        #endregion

        #region == ReSubmit Activity to CAR System ==

        public static void ReSubmitActivityToCARSystem()
        {
            try
            {
                Logger.Info("I:--START--");

                Task<ReSubmitActivityToCARSystemTaskResponse> task;
                using (var client = new CSMSRServiceClient())
                {
                    task = client.ReSubmitActivityToCARSystemAsync(WebConfig.GetTaskUsername(),
                        WebConfig.GetTaskPassword());
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
                    _mailSender.NotifyReSubmitActivityToCARSystemFailed(WebConfig.GetTaskEmailToAddress(), DateTime.Now, task.Exception);
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
                _mailSender.NotifyReSubmitActivityToCARSystemFailed(WebConfig.GetTaskEmailToAddress(), DateTime.Now, new AggregateException(ex));
                Thread.Sleep(5000);
            }
        }

        private static void SendMailTask(ReSubmitActivityToCARSystemTaskResponse result)
        {
            _mailSender = TaskMailSender.GetTaskMailSender();
            if (Constants.StatusResponse.Success.Equals(result.StatusResponse.Status))
            {
                _mailSender.NotifyReSubmitActivityToCARSystemSuccess(WebConfig.GetTaskEmailToAddress(), result);
            }
            if (Constants.StatusResponse.Failed.Equals(result.StatusResponse.Status))
            {
                _mailSender.NotifyReSubmitActivityToCARSystemFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime, result.StatusResponse.ErrorCode);
            }

            Thread.Sleep(5000);
        }

        #endregion

        #region == ReSubmit Activity to CBS-HP (Log100) System ==

        public static void ReSubmitActivityToCBSHPSystem()
        {
            try
            {
                Logger.Info("I:--START--");

                Task<ReSubmitActivityToCBSHPSystemTaskResponse> task;
                using (var client = new CSMSRServiceClient())
                {
                    task = client.ReSubmitActivityToCBSHPSystemAsync(WebConfig.GetTaskUsername(), WebConfig.GetTaskPassword());
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
                    _mailSender.NotifyReSubmitActivityToCBSHPSystemFailed(WebConfig.GetTaskEmailToAddress(), DateTime.Now, task.Exception);
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
                _mailSender.NotifyReSubmitActivityToCBSHPSystemFailed(WebConfig.GetTaskEmailToAddress(), DateTime.Now, new AggregateException(ex));
                Thread.Sleep(5000);
            }
        }

        private static void SendMailTask(ReSubmitActivityToCBSHPSystemTaskResponse result)
        {
            _mailSender = TaskMailSender.GetTaskMailSender();
            if (Constants.StatusResponse.Success.Equals(result.StatusResponse.Status))
            {
                _mailSender.NotifyReSubmitActivityToCBSHPSystemSuccess(WebConfig.GetTaskEmailToAddress(), result);
            }
            if (Constants.StatusResponse.Failed.Equals(result.StatusResponse.Status))
            {
                _mailSender.NotifyReSubmitActivityToCBSHPSystemFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime, result.StatusResponse.ErrorCode);
            }

            Thread.Sleep(5000);
        }

        #endregion
    }
}
