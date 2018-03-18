using System;
using System.Collections.Generic;
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
            ThreadContext.Properties["UserID"] = "CSM_SCHEDTASK";

            _stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Debug("-- Start Cron Job --:--Get Mailbox--");

            if (!ApplicationHelpers.Authenticate(username, password))
            {
                _logger.DebugFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");
            }

            _auditLog = new AuditLogEntity();
            _auditLog.Module = Constants.Module.Batch;
            _auditLog.Action = Constants.AuditAction.CreateCommPool;
            _auditLog.IpAddress = ApplicationHelpers.GetClientIP();

            JobTaskResponse taskResponse;
            DateTime schedDateTime = DateTime.Now;

            try
            {
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
                    const string errorMessage = "Pool list cannot be null or empty";
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
                                 var taskResult = _commPoolFacade.AddMailContent(hostname, port, useSsl, x);
                                 _taskResults.Add(taskResult);

                                 if (taskResult.StatusResponse.Status == Constants.StatusResponse.Success)
                                 {
                                     AppLog.AuditLog(_auditLog, LogStatus.Success, taskResult.ToString());
                                 }
                                 else
                                 {
                                     AppLog.AuditLog(_auditLog, LogStatus.Fail, string.Format(CultureInfo.InvariantCulture, "Username:{0}\n Error:{1}", taskResult.Username, taskResult.StatusResponse.Description));
                                 }

                             }
                         })).Wait();

                _logger.Info("I:--SUCCESS--:--Get Mailbox--");

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
            }
            finally
            {
                _stopwatch.Stop();
                _elapsedTime = _stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);
            }

            return taskResponse;
        }

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
