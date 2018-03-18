using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Service.Messages.SchedTask;
using CSM.Service.Messages.Sr;
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
        private CSMMailSender _mailSender;
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

        public GetSRResponse GetSR(GetSRRequest request)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

            _srFacade = new ServiceRequestFacade();
            var result = _srFacade.GetSRWebService(request);
            result.CreateDate = DateUtil.ToStringAsDateTime(result.CreateDateDt);
            result.CloseDate = DateUtil.ToStringAsDateTime(result.CloseDateDt);

            return result;
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
            //ThreadContext.Properties["UserID"] = "CSM_SCHEDTASK";
            if (!string.IsNullOrWhiteSpace(username))
            {
                ThreadContext.Properties["UserID"] = username.ToUpperInvariant();
            }

            long elapsedTime = 0;
            string errorMessage = string.Empty;
            DateTime schedDateTime = DateTime.Now;
            CreateSrFromReplyEmailTaskResponse taskResponse = null;

            _auditLog = new AuditLogEntity();
            _auditLog.Module = Constants.Module.Batch;
            _auditLog.Action = Constants.AuditAction.SyncSRStatusFromReplyEmail;
            _auditLog.IpAddress = ApplicationHelpers.GetClientIP();

            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Debug("-- Start Cron Job --:--Get CreateSRActivityFromReplyEmail--");

            #region "BatchProcess Start"

            if (AppLog.BatchProcessStart(Constants.BatchProcessCode.SyncSRStatusFromReplyEmail, schedDateTime) == false)
            {
                _logger.Info("I:--NOT PROCESS--:--CreateSRActivityFromReplyEmail--");

                stopwatch.Stop();
                elapsedTime = stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + elapsedTime);

                taskResponse = new CreateSrFromReplyEmailTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = elapsedTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = Constants.StatusResponse.NotProcess
                    }
                };

                return taskResponse;
            }

            #endregion

            //_logger.Debug("-- Start Cron Job --:--Get CreateSRActivityFromReplyEmail--");

            if (!ApplicationHelpers.Authenticate(username, password))
            {
                errorMessage = "Username and/or Password Invalid.";
                taskResponse = new CreateSrFromReplyEmailTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = errorMessage
                    }
                };

                _logger.InfoFormat("O:--LOGIN--:Error Message/{0}", errorMessage);
                //AppLog.AuditLog(_auditLog, LogStatus.Fail, "Username and/or Password Invalid.");
                return taskResponse;
            }
            try
            {
                _logger.Info("I:--START--:--CreateSRActivityFromReplyEmail--");

                //int NumOfTotal = 0;

                #region "Process Batch"

                var countSuccess = 0;
                var countError = 0;

                _srFacade = new ServiceRequestFacade();
                _srFacade.CreateSRActivityFromReplyEmail(ref countSuccess, ref countError);

                #endregion

                _logger.Info("O:--SUCCESS--:--CreateSRActivityFromReplyEmail--");

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

                AppLog.AuditLog(_auditLog, ((countError > 0) ? LogStatus.Fail : LogStatus.Success), taskResult.ToString());

                taskResponse = new CreateSrFromReplyEmailTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = elapsedTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = (countError > 0) ? Constants.StatusResponse.Failed : Constants.StatusResponse.Success
                    },
                    NumOfComplete = countSuccess,
                    NumOfError = countError,
                    NumOfTotal = countSuccess + countError
                };

                return taskResponse;
            }
            catch (Exception ex)
            {
                _logger.InfoFormat("O:--FAILED--:--CreateSRActivityFromReplyEmail--:--Error Message/{0}", ex.Message);
                _logger.Error("Exception occur:\n", ex);

                stopwatch.Stop();
                elapsedTime = stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + elapsedTime);

                AppLog.AuditLog(_auditLog, LogStatus.Fail, ex.Message);

                taskResponse = new CreateSrFromReplyEmailTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = elapsedTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = "Fail to CreateSRActivityFromReplyEmail"
                    }
                };

                return taskResponse;
            }
            finally
            {
                // Send mail to system administrator
                CreateSRActivitySendMail(taskResponse);

                #region "BatchProcess End"

                if (taskResponse != null && taskResponse.StatusResponse != null &&
                    taskResponse.StatusResponse.Status != Constants.StatusResponse.NotProcess)
                {

                    int batchStatus = (taskResponse.StatusResponse.Status == Constants.StatusResponse.Success)
                        ? Constants.BatchProcessStatus.Success
                        : Constants.BatchProcessStatus.Fail;

                    DateTime endTime = taskResponse.SchedDateTime.AddMilliseconds(taskResponse.ElapsedTime);
                    TimeSpan processTime = endTime.Subtract(taskResponse.SchedDateTime);

                    string processDetail = !string.IsNullOrEmpty(taskResponse.StatusResponse.Description)
                       ? taskResponse.StatusResponse.Description
                       : taskResponse.ToString();

                    AppLog.BatchProcessEnd(Constants.BatchProcessCode.SyncSRStatusFromReplyEmail, batchStatus, endTime,
                        processTime, processDetail);
                }

                #endregion
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
            //ThreadContext.Properties["UserID"] = "CSM_SCHEDTASK";
            if (!string.IsNullOrWhiteSpace(username))
            {
                ThreadContext.Properties["UserID"] = username.ToUpperInvariant();
            }

            long elapsedTime = 0;
            string errorMessage = string.Empty;
            DateTime schedDateTime = DateTime.Now;
            ReSubmitActivityToCARSystemTaskResponse taskResponse = null;

            _auditLog = new AuditLogEntity();
            _auditLog.Module = Constants.Module.Batch;
            _auditLog.Action = Constants.AuditAction.ReSubmitActivityToCARSystem;
            _auditLog.IpAddress = ApplicationHelpers.GetClientIP();

            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Debug("-- Start Cron Job --:--Get ReSubmitActivityToCARSystem--");

            #region "BatchProcess Start"

            if (AppLog.BatchProcessStart(Constants.BatchProcessCode.ReSubmitActivityToCARSystem, schedDateTime) == false)
            {
                _logger.Info("I:--NOT PROCESS--:--ReSubmitActivityToCARSystem--");

                stopwatch.Stop();
                elapsedTime = stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + elapsedTime);

                taskResponse = new ReSubmitActivityToCARSystemTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = elapsedTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = Constants.StatusResponse.NotProcess
                    }
                };

                return taskResponse;
            }

            #endregion

            //_logger.Debug("-- Start Cron Job --:--Get ReSubmitActivityToCARSystem--");

            if (!ApplicationHelpers.Authenticate(username, password))
            {
                errorMessage = "Username and/or Password Invalid.";
                taskResponse = new ReSubmitActivityToCARSystemTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = errorMessage
                    }
                };

                _logger.InfoFormat("O:--LOGIN--:Error Message/{0}", errorMessage);
                //AppLog.AuditLog(_auditLog, LogStatus.Fail, "Username and/or Password Invalid.");
                return taskResponse;
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

                _logger.Info("O:--SUCCESS--:--ReSubmitActivityToCARSystem--");

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

                AppLog.AuditLog(_auditLog,((countError > 0) ? LogStatus.Fail : LogStatus.Success), taskResult.ToString());

                taskResponse = new ReSubmitActivityToCARSystemTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = elapsedTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = (countError > 0) ? Constants.StatusResponse.Failed : Constants.StatusResponse.Success
                    },
                    NumOfComplete = countSuccess,
                    NumOfError = countError,
                    NumOfTotal = countSuccess + countError
                };

                return taskResponse;
            }
            catch (Exception ex)
            {
                _logger.InfoFormat("O:--FAILED--:--ReSubmitActivityToCARSystem--:--Error Message/{0}", ex.Message);
                _logger.Error("Exception occur:\n", ex);

                stopwatch.Stop();
                elapsedTime = stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + elapsedTime);

                AppLog.AuditLog(_auditLog, LogStatus.Fail, ex.Message);

                taskResponse = new ReSubmitActivityToCARSystemTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = elapsedTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = "Failed to ReSubmitActivityToCARSystem"
                    }
                };

                return taskResponse;
            }
            finally
            {
                // Send mail to system administrator
                ReSubmitActivityToCARSendMail(taskResponse);

                #region "BatchProcess End"

                if (taskResponse != null && taskResponse.StatusResponse != null &&
                    taskResponse.StatusResponse.Status != Constants.StatusResponse.NotProcess)
                {

                    int batchStatus = (taskResponse.StatusResponse.Status == Constants.StatusResponse.Success)
                        ? Constants.BatchProcessStatus.Success
                        : Constants.BatchProcessStatus.Fail;

                    DateTime endTime = taskResponse.SchedDateTime.AddMilliseconds(taskResponse.ElapsedTime);
                    TimeSpan processTime = endTime.Subtract(taskResponse.SchedDateTime);

                    string processDetail = !string.IsNullOrEmpty(taskResponse.StatusResponse.Description)
                       ? taskResponse.StatusResponse.Description
                       : taskResponse.ToString();

                    AppLog.BatchProcessEnd(Constants.BatchProcessCode.ReSubmitActivityToCARSystem, batchStatus, endTime, processTime, processDetail);
                }

                #endregion
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
            //ThreadContext.Properties["UserID"] = "CSM_SCHEDTASK";
            if (!string.IsNullOrWhiteSpace(username))
            {
                ThreadContext.Properties["UserID"] = username.ToUpperInvariant();
            }

            long elapsedTime = 0;
            string errorMessage = string.Empty;
            DateTime schedDateTime = DateTime.Now;
            ReSubmitActivityToCBSHPSystemTaskResponse taskResponse = null;

            _auditLog = new AuditLogEntity();
            _auditLog.Module = Constants.Module.Batch;
            _auditLog.Action = Constants.AuditAction.ReSubmitActivityToCBSHPSystem;
            _auditLog.IpAddress = ApplicationHelpers.GetClientIP();

            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Debug("-- Start Cron Job --:--Get ReSubmitActivityToCBSHPSystem--");

            #region "BatchProcess Start"

            if (AppLog.BatchProcessStart(Constants.BatchProcessCode.ReSubmitActivityToCBSHPSystem, schedDateTime) == false)
            {
                _logger.Info("I:--NOT PROCESS--:--ReSubmitActivityToCBSHPSystem--");

                stopwatch.Stop();
                elapsedTime = stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + elapsedTime);

                taskResponse = new ReSubmitActivityToCBSHPSystemTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = elapsedTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = Constants.StatusResponse.NotProcess
                    }
                };

                return taskResponse;
            }

            #endregion

            //_logger.Debug("-- Start Cron Job --:--Get ReSubmitActivityToCBSHPSystem--");

            if (!ApplicationHelpers.Authenticate(username, password))
            {
                errorMessage = "Username and/or Password Invalid.";
                taskResponse = new ReSubmitActivityToCBSHPSystemTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = errorMessage
                    }
                };

                _logger.InfoFormat("O:--LOGIN--:Error Message/{0}", errorMessage);
                //AppLog.AuditLog(_auditLog, LogStatus.Fail, "Username and/or Password Invalid.");
                return taskResponse;
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

                _logger.Info("O:--SUCCESS--:--ReSubmitActivityToCBSHPSystem--");

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

                AppLog.AuditLog(_auditLog, ((countError > 0) ? LogStatus.Fail : LogStatus.Success), taskResult.ToString());

                taskResponse = new ReSubmitActivityToCBSHPSystemTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = elapsedTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = (countError > 0) ? Constants.StatusResponse.Failed : Constants.StatusResponse.Success
                    },
                    NumOfComplete = countSuccess,
                    NumOfError = countError,
                    NumOfTotal = countSuccess + countError
                };

                return taskResponse;
            }
            catch (Exception ex)
            {
                _logger.InfoFormat("O:--FAILED--:--ReSubmitActivityToCBSHPSystem--:--Error Message/{0}", ex.Message);
                _logger.Error("Exception occur:\n", ex);

                stopwatch.Stop();
                elapsedTime = stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + elapsedTime);

                AppLog.AuditLog(_auditLog, LogStatus.Fail, ex.Message);

                taskResponse = new ReSubmitActivityToCBSHPSystemTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = elapsedTime,
                    StatusResponse = new CSM.Service.Messages.Common.StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = "Failed to ReSubmitActivityToCBSHPSystem"
                    }
                };

                return taskResponse;
            }
            finally
            {
                // Send mail to system administrator
                ReSubmitActivityToCBSHPSendMail(taskResponse);

                #region "BatchProcess End"

                if (taskResponse != null && taskResponse.StatusResponse != null &&
                    taskResponse.StatusResponse.Status != Constants.StatusResponse.NotProcess)
                {

                    int batchStatus = (taskResponse.StatusResponse.Status == Constants.StatusResponse.Success)
                        ? Constants.BatchProcessStatus.Success
                        : Constants.BatchProcessStatus.Fail;

                    DateTime endTime = taskResponse.SchedDateTime.AddMilliseconds(taskResponse.ElapsedTime);
                    TimeSpan processTime = endTime.Subtract(taskResponse.SchedDateTime);

                    string processDetail = !string.IsNullOrEmpty(taskResponse.StatusResponse.Description)
                       ? taskResponse.StatusResponse.Description
                       : taskResponse.ToString();

                    AppLog.BatchProcessEnd(Constants.BatchProcessCode.ReSubmitActivityToCBSHPSystem, batchStatus, endTime, processTime, processDetail);
                }

                #endregion
            }
        }

        #region "Functions"

        private void CreateSRActivitySendMail(CreateSrFromReplyEmailTaskResponse result)
        {
            try
            {
                _mailSender = CSMMailSender.GetCSMMailSender();

                if (result.NumOfError > 0)
                {
                    _mailSender.NotifyCreateSrFromReplyEmailSuccess(WebConfig.GetTaskEmailToAddress(), result);
                }
                else if (Constants.StatusResponse.Failed.Equals(result.StatusResponse.Status))
                {
                    _mailSender.NotifyCreateSrFromReplyEmailFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime, result.StatusResponse.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
            }
        }

        private void ReSubmitActivityToCARSendMail(ReSubmitActivityToCARSystemTaskResponse result)
        {
            try
            {
                _mailSender = CSMMailSender.GetCSMMailSender();

                if (result.NumOfError > 0)
                {
                    _mailSender.NotifyReSubmitActivityToCARSystemSuccess(WebConfig.GetTaskEmailToAddress(), result);
                }
                else if (Constants.StatusResponse.Failed.Equals(result.StatusResponse.Status))
                {
                    _mailSender.NotifyReSubmitActivityToCARSystemFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime, result.StatusResponse.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
            }
        }

        private void ReSubmitActivityToCBSHPSendMail(ReSubmitActivityToCBSHPSystemTaskResponse result)
        {
            try
            {
                _mailSender = CSMMailSender.GetCSMMailSender();

                if (result.NumOfError > 0)
                {
                    _mailSender.NotifyReSubmitActivityToCBSHPSystemSuccess(WebConfig.GetTaskEmailToAddress(), result);
                }
                else if (Constants.StatusResponse.Failed.Equals(result.StatusResponse.Status))
                {
                    _mailSender.NotifyReSubmitActivityToCBSHPSystemFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime, result.StatusResponse.ErrorCode);
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
