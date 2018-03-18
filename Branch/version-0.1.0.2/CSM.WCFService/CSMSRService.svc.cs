using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Service.Messages.SchedTask;
using log4net;
using System;
using System.Text;
using CSM.Business;

namespace CSM.WCFService
{
    public class CSMSRService : ICSMSRService
    {
        private readonly ILog _logger;
        private AuditLogEntity _auditLog;
        private IServiceRequestFacade _srFacade;
        
        #region "Constructor"
        public CSMSRService()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();

                // Set logfile name and application name variables
                GlobalContext.Properties["ApplicationCode"] = "CSMWS";
                GlobalContext.Properties["ServerName"] = Environment.MachineName;
                _logger = LogManager.GetLogger(typeof(CSMSRService));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
            }
        }
        #endregion

        public CreateSRResponse CreateSR(CreateSRRequest request)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

            _logger.Debug("Start Web Service: CreateSR");

            _srFacade = new ServiceRequestFacade();
            var result = _srFacade.CreateSRWebService(request);

            _logger.Debug("End Web Service: Create SR");

            if (result != null)
            {
                _logger.DebugFormat("Result: IsSuccess = {0}", result.IsSuccess);
                _logger.DebugFormat("Result: ErrorCode = {0}", result.ErrorCode);
                _logger.DebugFormat("Result: ErrorMessage = {0}", result.ErrorMessage);
                _logger.DebugFormat("Result: SRNo = {0}", result.SRNo);
            }

            return result;
        }

        public UpdateSRResponse UpdateSR(UpdateSRRequest request)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

            _srFacade = new ServiceRequestFacade();
            
            _logger.Debug("Start Web Service: UpdateSR");

            var result = _srFacade.UpdateSRWebService(request);

            _logger.Debug("End Web Service: UpdateSR");

            if (result != null)
            {
                _logger.DebugFormat("Result: IsSuccess = {0}", result.IsSuccess);
                _logger.DebugFormat("Result: ErrorCode = {0}", result.ErrorCode);
                _logger.DebugFormat("Result: ErrorMessage = {0}", result.ErrorMessage);
                _logger.DebugFormat("Result: SRNo = {0}", result.SRNo);
            }

            return result;
        }

        public SearchSRResponse SearchSR(SearchSRRequest request)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

            _srFacade = new ServiceRequestFacade();
            return _srFacade.SearchSRWebService(request);
        }

        public GetSRResponse GetSR(string srNo)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

            _srFacade = new ServiceRequestFacade();
            return _srFacade.GetSRWebService(srNo);
        }

        /// <summary>
        /// Batch Process
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public CreateSrFromReplyEmailTaskResponse CreateSRActivityFromReplyEmail(string username, string password)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();
            ThreadContext.Properties["UserID"] = "CSM_SCHEDTASK";

            long elapsedTime = 0;
            DateTime schedDateTime = DateTime.Now;

            _auditLog = new AuditLogEntity();
            _auditLog.Module = Constants.Module.Batch;
            _auditLog.Action = Constants.AuditAction.SyncSRStatusFromReplyEmail;
            _auditLog.IpAddress = ApplicationHelpers.GetClientIP();

            System.Diagnostics.Stopwatch stopwatch =  System.Diagnostics.Stopwatch.StartNew();
            _logger.Debug("-- Start Cron Job --:--Get CreateSRActivityFromReplyEmail--");

            if (!ApplicationHelpers.Authenticate(username, password))
            {
                _logger.DebugFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");

                AppLog.AuditLog(_auditLog, LogStatus.Fail, "Username and/or Password Invalid.");
            }
            try
            {
                _logger.Info("I:--START--:--CreateSRActivityFromReplyEmail--");

                //int NumOfTotal = 0;

                #region "Process Batch"

                var countSuccess = 0;
                var countError = 0;

                new ServiceRequestFacade().CreateSRActivityFromReplyEmail(ref countSuccess, ref countError);

                #endregion

                _logger.Info("I:--SUCCESS--:--CreateSRActivityFromReplyEmail--");

                stopwatch.Stop();
                elapsedTime = stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + elapsedTime);

                StringBuilder taskResult = new StringBuilder("");
                taskResult.Append(string.Format("Username = {0}\n", username));
                taskResult.Append(string.Format("SchedDateTime = {0}\n", schedDateTime));
                taskResult.Append(string.Format("ElapsedTime = {0}\n", elapsedTime));
                taskResult.Append(string.Format("Count Success = {0}\n", countSuccess));
                taskResult.Append(string.Format("Count Error = {0}\n", countError));
                taskResult.Append(string.Format("Total = {0}\n", countSuccess + countError));

                AppLog.AuditLog(_auditLog, LogStatus.Success, taskResult.ToString());

                return new CreateSrFromReplyEmailTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = elapsedTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = Constants.StatusResponse.Success
                    },
                    NumOfComplete = countSuccess,
                    NumOfError = countError,
                    NumOfTotal = countSuccess + countError,
                };
            }
            catch (Exception ex)
            {
                _logger.InfoFormat("O:--FAILED--:--CreateSRActivityFromReplyEmail--:--Error Message/{0}", ex.Message);
                _logger.Error("Exception occur:\n", ex);
                
                stopwatch.Stop();
                elapsedTime = stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + elapsedTime);

                AppLog.AuditLog(_auditLog, LogStatus.Fail, ex.Message);

                return new CreateSrFromReplyEmailTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = elapsedTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = "Fail to CreateSRActivityFromReplyEmail"
                    },
                };
            }
        }

        /// <summary>
        /// Batch Process
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ReSubmitActivityToCARSystemTaskResponse ReSubmitActivityToCARSystem(string username, string password)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();
            ThreadContext.Properties["UserID"] = "CSM_SCHEDTASK";

            long elapsedTime = 0;
            DateTime schedDateTime = DateTime.Now;

            _auditLog = new AuditLogEntity();
            _auditLog.Module = Constants.Module.Batch;
            _auditLog.Action = Constants.AuditAction.ReSubmitActivityToCARSystem;
            _auditLog.IpAddress = ApplicationHelpers.GetClientIP();

            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Debug("-- Start Cron Job --:--Get ReSubmitActivityToCARSystem--");

            if (!ApplicationHelpers.Authenticate(username, password))
            {
                _logger.DebugFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");

                AppLog.AuditLog(_auditLog, LogStatus.Fail, "Username and/or Password Invalid.");
            }
            try
            {
                _logger.Info("I:--START--:--ReSubmitActivityToCARSystem--");

                #region "Process Batch"

                var countSuccess = 0;
                var countError = 0;

                _srFacade = new ServiceRequestFacade();
                _srFacade.ReSubmitActivityToCARSystem(ref countSuccess, ref countError);

                #endregion

                _logger.Info("I:--SUCCESS--:--ReSubmitActivityToCARSystem--");

                stopwatch.Stop();
                elapsedTime = stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + elapsedTime);

                StringBuilder taskResult = new StringBuilder("");
                taskResult.Append(string.Format("Username = {0}\n", username));
                taskResult.Append(string.Format("SchedDateTime = {0}\n", schedDateTime));
                taskResult.Append(string.Format("ElapsedTime = {0}\n", elapsedTime));
                taskResult.Append(string.Format("Count Success = {0}\n", countSuccess));
                taskResult.Append(string.Format("Count Error = {0}\n", countError));
                taskResult.Append(string.Format("Total = {0}\n", countSuccess + countError));

                AppLog.AuditLog(_auditLog, LogStatus.Success, taskResult.ToString());

                return new ReSubmitActivityToCARSystemTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = elapsedTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = Constants.StatusResponse.Success
                    },
                    NumOfComplete = countSuccess,
                    NumOfError = countError,
                    NumOfTotal = countSuccess + countError,
                };
            }
            catch (Exception ex)
            {
                _logger.InfoFormat("O:--FAILED--:--ReSubmitActivityToCARSystem--:--Error Message/{0}", ex.Message);
                _logger.Error("Exception occur:\n", ex);

                stopwatch.Stop();
                elapsedTime = stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + elapsedTime);

                AppLog.AuditLog(_auditLog, LogStatus.Fail, ex.Message);

                return new ReSubmitActivityToCARSystemTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = elapsedTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = "Fail to ReSubmitActivityToCARSystem"
                    },
                };
            }
        }

        /// <summary>
        /// Batch Process
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ReSubmitActivityToCBSHPSystemTaskResponse ReSubmitActivityToCBSHPSystem(string username, string password)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();
            ThreadContext.Properties["UserID"] = "CSM_SCHEDTASK";

            long elapsedTime = 0;
            DateTime schedDateTime = DateTime.Now;

            _auditLog = new AuditLogEntity();
            _auditLog.Module = Constants.Module.Batch;
            _auditLog.Action = Constants.AuditAction.ReSubmitActivityToCBSHPSystem;
            _auditLog.IpAddress = ApplicationHelpers.GetClientIP();

            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Debug("-- Start Cron Job --:--Get ReSubmitActivityToCBSHPSystem--");

            if (!ApplicationHelpers.Authenticate(username, password))
            {
                _logger.DebugFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");

                AppLog.AuditLog(_auditLog, LogStatus.Fail, "Username and/or Password Invalid.");
            }
            try
            {
                _logger.Info("I:--START--:--ReSubmitActivityToCBSHPSystem--");

                #region "Process Batch"

                var countSuccess = 0;
                var countError = 0;

                _srFacade = new ServiceRequestFacade();
                _srFacade.ReSubmitActivityToCBSHPSystem(ref countSuccess, ref countError);

                #endregion

                _logger.Info("I:--SUCCESS--:--ReSubmitActivityToCBSHPSystem--");

                stopwatch.Stop();
                elapsedTime = stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + elapsedTime);

                StringBuilder taskResult = new StringBuilder("");
                taskResult.Append(string.Format("Username = {0}\n", username));
                taskResult.Append(string.Format("SchedDateTime = {0}\n", schedDateTime));
                taskResult.Append(string.Format("ElapsedTime = {0}\n", elapsedTime));
                taskResult.Append(string.Format("Count Success = {0}\n", countSuccess));
                taskResult.Append(string.Format("Count Error = {0}\n", countError));
                taskResult.Append(string.Format("Total = {0}\n", countSuccess + countError));

                AppLog.AuditLog(_auditLog, LogStatus.Success, taskResult.ToString());

                return new ReSubmitActivityToCBSHPSystemTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = elapsedTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = Constants.StatusResponse.Success
                    },
                    NumOfComplete = countSuccess,
                    NumOfError = countError,
                    NumOfTotal = countSuccess + countError,
                };
            }
            catch (Exception ex)
            {
                _logger.InfoFormat("O:--FAILED--:--ReSubmitActivityToCBSHPSystem--:--Error Message/{0}", ex.Message);
                _logger.Error("Exception occur:\n", ex);

                stopwatch.Stop();
                elapsedTime = stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + elapsedTime);

                AppLog.AuditLog(_auditLog, LogStatus.Fail, ex.Message);

                return new ReSubmitActivityToCBSHPSystemTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = elapsedTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = "Fail to ReSubmitActivityToCBSHPSystem"
                    },
                };
            }
        }

        #region "IDisposable"

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (_srFacade != null) { _srFacade.Dispose(); }
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
