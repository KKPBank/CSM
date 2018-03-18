using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using CSM.Business;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Service.Messages.Common;
using CSM.Service.Messages.SchedTask;
using log4net;
using System.Globalization;

///<summary>
/// Class Name : CSMMailService
/// Purpose    : -
/// Author     : Neda Peyrone
///</summary>
///<remarks>
/// Change History:
/// Date         Author           Description
/// ----         ------           -----------
///</remarks>
namespace CSM.WCFService
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CSMMailService : ICSMMailService
    {
        private long _elapsedTime;
        private readonly ILog _logger;
        private AuditLogEntity _auditLog;
        private CSMMailSender _mailSender;
        private object sync = new Object();
        private ICommPoolFacade _commPoolFacade;
        private List<JobTaskResult> _taskResults;
        private System.Diagnostics.Stopwatch _stopwatch;

        #region "Constructor"

        public CSMMailService()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();

                // Set logfile name and application name variables
                GlobalContext.Properties["ApplicationCode"] = "CSMWS";
                GlobalContext.Properties["ServerName"] = Environment.MachineName;
                _logger = LogManager.GetLogger(typeof(CSMMailService));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
            }
        }

        #endregion

        public JobTaskResponse GetMailbox(string username, string password)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

            if (!string.IsNullOrWhiteSpace(username))
            {
                ThreadContext.Properties["UserID"] = username.ToUpperInvariant();
            }

            _stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Debug("-- Start Cron Job --:--Get Mailbox--");

            string errorMessage = string.Empty;
            JobTaskResponse taskResponse = null;
            DateTime schedDateTime = DateTime.Now;

            if (!ApplicationHelpers.Authenticate(username, password))
            {
                errorMessage = "Username and/or Password Invalid.";
                taskResponse = new JobTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = errorMessage
                    }
                };
                _logger.InfoFormat("O:--LOGIN--:Error Message/{0}", errorMessage);
            }

            _auditLog = new AuditLogEntity();
            _auditLog.Module = Constants.Module.Batch;
            _auditLog.Action = Constants.AuditAction.CreateCommPool;
            _auditLog.IpAddress = ApplicationHelpers.GetClientIP();

            try
            {
                #region "BatchProcess Start"

                if (AppLog.BatchProcessStart(Constants.BatchProcessCode.CreateCommPool, schedDateTime) == false)
                {
                    _logger.Info("I:--NOT PROCESS--:--Get Mailbox--");

                    taskResponse = new JobTaskResponse
                    {
                        SchedDateTime = schedDateTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = Constants.StatusResponse.NotProcess,
                            ErrorCode = string.Empty,
                            Description = string.Empty
                        }
                    };

                    return taskResponse;
                }

                #endregion

                _logger.Info("I:--START--:--Get Mailbox--");

                _commPoolFacade = new CommPoolFacade();
                _taskResults = new List<JobTaskResult>();

                #region "Retrieve Mail Settings"

                string hostname = WebConfig.GetPOP3EmailServer();
                int port = WebConfig.GetPOP3Port();
                bool useSsl = WebConfig.GetMailUseSsl();
                List<PoolEntity> poolList = _commPoolFacade.GetActivePoolList();

                #endregion

                if (poolList == null || poolList.Count == 0)
                {
                    errorMessage = "Pool list cannot be null or empty";
                    taskResponse = new JobTaskResponse
                    {
                        SchedDateTime = schedDateTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = Constants.StatusResponse.Failed,
                            ErrorCode = string.Empty,
                            Description = errorMessage
                        }
                    };

                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", errorMessage);
                    _logger.ErrorFormat("Exception occur:\n{0}", errorMessage);
                    AppLog.AuditLog(_auditLog, LogStatus.Fail, errorMessage);
                    return taskResponse;
                }

                Task.Factory.StartNew(() => Parallel.ForEach(poolList,
                         new ParallelOptions { MaxDegreeOfParallelism = WebConfig.GetTotalCountToProcess() },
                         x =>
                         {
                             lock (sync)
                             {
                                 var stopwatch = new System.Diagnostics.Stopwatch();
                                 stopwatch.Start();

                                 var taskDateTime = DateTime.Now;
                                 var taskResult = _commPoolFacade.AddMailContent(hostname, port, useSsl, x);
                                 taskResult.SchedDateTime = taskDateTime;

                                 if (taskResult.StatusResponse.Status == Constants.StatusResponse.Success)
                                 {
                                     AppLog.AuditLog(_auditLog, LogStatus.Success, taskResult.ToString());
                                 }
                                 if (taskResult.StatusResponse.Status == Constants.StatusResponse.Failed)
                                 {
                                     if (taskResult.NumFailedDelete == 0)
                                     {
                                         AppLog.AuditLog(_auditLog, LogStatus.Fail, string.Format(CultureInfo.InvariantCulture, "Username:{0}\n Error:{1}", taskResult.Username, taskResult.StatusResponse.Description));
                                     }
                                     if (taskResult.NumFailedDelete > 0)
                                     {
                                         AppLog.AuditLog(_auditLog, LogStatus.Fail, taskResult.ToString());
                                     }
                                 }

                                 stopwatch.Stop();
                                 taskResult.ElapsedTime = stopwatch.ElapsedMilliseconds;
                                 _taskResults.Add(taskResult);
                             }
                         })).Wait();

                _logger.Info("O:--SUCCESS--:--Get Mailbox--");

                taskResponse = new JobTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.Success
                    },
                    JobTaskResults = _taskResults
                };

                return taskResponse;
            }
            catch (Exception ex)
            {
                taskResponse = new JobTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = ex.Message
                    }
                };

                _logger.InfoFormat("O:--FAILED--:Error Message/{0}", ex.Message);
                _logger.Error("Exception occur:\n", ex);
                AppLog.AuditLog(_auditLog, LogStatus.Fail, ex.Message);
                return taskResponse;
            }
            finally
            {
                _stopwatch.Stop();
                _elapsedTime = _stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                #region "BatchProcess End"

                if (taskResponse != null && taskResponse.StatusResponse != null &&
                    taskResponse.StatusResponse.Status != Constants.StatusResponse.NotProcess)
                {
                    var batchStatus = Constants.BatchProcessStatus.Fail;
                    var batchDetails = string.Empty;
                    if (taskResponse.StatusResponse.Status == Constants.StatusResponse.Success)
                    {
                        batchStatus =
                            taskResponse.JobTaskResults.Any(
                                x => x.StatusResponse.Status != Constants.StatusResponse.Success)
                                ? Constants.BatchProcessStatus.Fail
                                : Constants.BatchProcessStatus.Success;

                        //batchDetails = StringHelpers.ConvertListToString(taskResponse.JobTaskResults.Select(x => x.ToString()).ToList<object>(), "\n");

                        if (taskResponse.JobTaskResults != null && taskResponse.JobTaskResults.Count > 0)
                        {
                            foreach (var taskResult in taskResponse.JobTaskResults)
                            {
                                if (taskResult.StatusResponse.Status == Constants.StatusResponse.Success)
                                {
                                    batchDetails += taskResult.ToString() + "\n\n";
                                }

                                if (taskResult.StatusResponse.Status == Constants.StatusResponse.Failed)
                                {
                                    if (taskResult.NumFailedDelete == 0)
                                    {
                                        batchDetails += string.Format(CultureInfo.InvariantCulture, "Username:{0}\n Error:{1} \n\n", taskResult.Username, taskResult.StatusResponse.Description);
                                    }
                                    else
                                    {
                                        batchDetails += taskResult.ToString() + "\n\n";
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        batchDetails = taskResponse.StatusResponse.Description;
                    }

                    DateTime endTime = taskResponse.SchedDateTime.AddMilliseconds(_elapsedTime);
                    TimeSpan processTime = endTime.Subtract(taskResponse.SchedDateTime);

                    AppLog.BatchProcessEnd(Constants.BatchProcessCode.CreateCommPool, batchStatus, endTime, processTime, batchDetails);
                }

                #endregion

                // Send mail to system administrator
                ImportJobSendMail(taskResponse);
            }
        }

        #region "Functions"

        private void ImportJobSendMail(JobTaskResponse result)
        {
            try
            {
                _mailSender = CSMMailSender.GetCSMMailSender();

                if (result.JobTaskResults != null && result.JobTaskResults.Count > 0)
                {
                    foreach (var task in result.JobTaskResults.Where(x => x.StatusResponse.Status == Constants.StatusResponse.Failed))
                    {
                        if (task.NumFailedDelete == 0)
                        {
                            _mailSender.NotifySyncEmailFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime, task.StatusResponse.Description);
                        }
                        else
                        {
                            _mailSender.NotifySyncEmailSuccess(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime, task);
                        }
                    }
                }
                else
                {
                    _logger.InfoFormat("O:--Unable to Send Email Notification--:Total Results/{0}", 0);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
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
                    if (_commPoolFacade != null) { _commPoolFacade.Dispose(); }
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
