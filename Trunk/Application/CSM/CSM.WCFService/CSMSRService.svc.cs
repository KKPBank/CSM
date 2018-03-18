using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Service.Messages.Common;
using CSM.Service.Messages.SchedTask;
using CSM.Service.Messages.Sr;
using log4net;
using System;
using System.Text;
using CSM.Business;
using System.Collections.Generic;
using System.Globalization;
using CSM.Common.Resources;
using System.IO;
using System.Linq;

namespace CSM.WCFService
{
    public class CSMSRService : ICSMSRService
    {
        private readonly ILog _logger;
        private AuditLogEntity _auditLog;
        private CSMMailSender _mailSender;
        private IServiceRequestFacade _srFacade;

        private ICommonFacade _commonFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CSMSRService));

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

        private bool CheckTimeToExport(out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                //if (_reportDataAccess == null) { _reportDataAccess = new ReportDataAccess(_context); }

                if (_commonFacade == null)
                {
                    _commonFacade = new CommonFacade();
                }
                string reportTimeStart = _commonFacade.GetCacheParamByName(Constants.ParameterName.ReportTimeStart).ParamValue;
                string reportTimeEnd = _commonFacade.GetCacheParamByName(Constants.ParameterName.ReportTimeEnd).ParamValue;

                IList<object> start = StringHelpers.ConvertStringToList(reportTimeStart, ':');
                IList<object> end = StringHelpers.ConvertStringToList(reportTimeEnd, ':');

                string hour = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.ShortTime);
                IList<object> hours = StringHelpers.ConvertStringToList(hour, ':');

                TimeSpan tsStart = new TimeSpan(Convert.ToInt32(start[0], CultureInfo.InvariantCulture), Convert.ToInt32(start[1], CultureInfo.InvariantCulture), 0);
                TimeSpan tsEnd = new TimeSpan(Convert.ToInt32(end[0], CultureInfo.InvariantCulture), Convert.ToInt32(end[1], CultureInfo.InvariantCulture), 0);
                TimeSpan tsHour = new TimeSpan(Convert.ToInt32(hours[0], CultureInfo.InvariantCulture), Convert.ToInt32(hours[1], CultureInfo.InvariantCulture), 0);

                if (tsHour >= tsStart && tsHour <= tsEnd)
                {
                    //return Json(new
                    //{
                    //    Valid = false,
                    //    Error = string.Format(CultureInfo.InvariantCulture, Resource.ValErr_ReportTime, reportTimeStart, reportTimeEnd)
                    //});

                    errorMessage = string.Format(CultureInfo.InvariantCulture, Resource.ValErr_ReportTime, reportTimeStart, reportTimeEnd);
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMessage = Resource.ValErr_ReportTimeConfiguration;
                Logger.Error("Exception occur:\n", ex);
                //return Json(new
                //{
                //    Valid = false,
                //    Error = Resource.ValErr_ReportTimeConfiguration
                //});
            }
            return true;
        }

        public ExportSRTaskResponse DailySRReport()
        {
            long _elapsedTime = 0;
            DateTime _schedDateTime = DateTime.Now;
            //int batchStatus= Constants.BatchProcessStatus.Processing;

            ExportSRTaskResponse taskResponse = new ExportSRTaskResponse()
            {
                SchedDateTime = _schedDateTime,
                TaskAction = Constants.AuditAction.ExportSRDaily,
                StatusResponse = new StatusResponse()
                {
                    Status = Constants.StatusResponse.Failed,
                    ErrorCode = string.Empty,
                    Description = ""
                }
            };

            using (var _reportFacade = new ReportFacade())
            {

                System.Diagnostics.Stopwatch _stopwatch = System.Diagnostics.Stopwatch.StartNew();
                _logger.Debug("-- Start Cron Job --:--Get ExportDailySRReport--");

                #region "BatchProcess Start"

                if (AppLog.BatchProcessStart(Constants.BatchProcessCode.ExportDailySRReport, _schedDateTime) == false)
                {
                    _logger.Info("I:--NOT PROCESS--:--ExportDailySRReport--");

                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;
                    _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                    taskResponse.ElapsedTime = _elapsedTime;
                    taskResponse.StatusResponse.Status = Constants.StatusResponse.NotProcess;
                    return taskResponse;
                }

                #endregion

                _commonFacade = new CommonFacade();

                CultureInfo ci = new CultureInfo("en");

                ExportSRSearchFilter searchFilter = new ExportSRSearchFilter
                {
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = string.Empty,
                    SortOrder = "DESC",
                    //SRDateFrom = DateTime.Now.Date.AddDays(-1).ToString(Constants.DateTimeFormat.DefaultShortDate),
                    //SRDateTo = DateTime.Now.Date.ToString(Constants.DateTimeFormat.DefaultShortDate),
                    SRDateFrom = DateTime.Now.AddDays(-1).ToString(Constants.DateTimeFormat.DefaultShortDate, ci),
                    SRDateTo = DateTime.Now.AddDays(-1).ToString(Constants.DateTimeFormat.DefaultShortDate, ci),
                    SRStatus = "-1",
                    SRStatusName = "ทั้งหมด"
                };

                try
                {
                    #region "Check Time to Export"

                    string errorMessage;
                    bool isValid = CheckTimeToExport(out errorMessage);

                    #endregion

                    string dateStr = DateTime.Now.AddDays(-1).FormatDateTime(Constants.DateTimeFormat.ExportAfsDateTime);
                    string fileDownloadName = string.Format(CultureInfo.InvariantCulture, "{0}_{1}.{2}", Resource.Report_SR_Daily_FileName, dateStr, Resource.Report_FileExtention);
                    string fileNameChkDup = $"{Resource.Report_SR_Daily_FileName}_{DateTime.Now.FormatDateTime(Constants.DateTimeFormat.ExportAfsDateTime)}*.{Resource.Report_FileExtention}";
                    string reportPath = _reportFacade.GetReportPath();
                    string[] fileDups = Directory.GetFiles(reportPath, fileNameChkDup);

                    IList<ExportSREntity> srList = null;

                    //ถ้ามีไฟล์ของวันที่วันนี้อยู่แล้ว ไม่ Gen อีก
                    if (fileDups.Length == 0)
                    { srList = _reportFacade.GetExportSR(searchFilter); }
                    else
                    { srList = new List<ExportSREntity>(); }

                    if (fileDups.Length > 0 || (srList != null /*&& srList.Count > 0*/))
                    {
                        //Skip Gen File
                        if (fileDups.Length == 0)
                        {
                            var bytes = _reportFacade.CreateReportSR(srList, searchFilter);
                            File.WriteAllBytes(reportPath + @"\" + fileDownloadName, bytes);
                        }
                        //Transfer file ทั้งหมดใน Folder
                        string[] files = Directory.GetFiles(reportPath, $"{Resource.Report_SR_Daily_FileName}_*.{Resource.Report_FileExtention}");
                        bool transfer = false;
                        try
                        {
                            if (files.Length > 0)
                            {
                                transfer = true;
                                foreach (var f in files)
                                {
                                    var cfg = new SRReportSFtpConfig("Daily") { UploadFile = f };
                                    if (_reportFacade.UploadFileSRReport(cfg) == false)
                                    {
                                        transfer = false;
                                        break;
                                    }
                                    else { StreamDataHelpers.TryToDelete(f); }
                                }
                            }
                            else { taskResponse.StatusResponse.Description = "No file to transfer."; }
                        }
                        catch (Exception ex)
                        {
                            transfer = false;
                            taskResponse.StatusResponse.Description = ex.Message;
                        }

                        _stopwatch.Stop();
                        _elapsedTime = _stopwatch.ElapsedMilliseconds;

                        taskResponse.ElapsedTime = _elapsedTime;
                        taskResponse.NumOfActivity = srList == null ? 0 : srList.Count;
                        taskResponse.StatusResponse.Status = Constants.StatusResponse.Success;
                        taskResponse.TransferStatus = transfer;
                        _reportFacade.SaveAuditLogSRReport(taskResponse);
                        return taskResponse;
                    }

                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;

                    taskResponse.ElapsedTime = _elapsedTime;
                    taskResponse.NumOfActivity = srList == null ? 0 : srList.Count;
                    taskResponse.StatusResponse.Description = "No data to export.";
                    _reportFacade.SaveAuditLogSRReport(taskResponse);
                    return taskResponse;
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception occur:\n", ex);
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Daily SR Report").Add("Error Message", ex.Message).ToFailLogString());
                    //batchStatus = Constants.BatchProcessStatus.Fail;
                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;

                    taskResponse.ElapsedTime = _elapsedTime;
                    taskResponse.StatusResponse.Description = ex.Message;
                    _reportFacade.SaveAuditLogSRReport(taskResponse);
                }
                finally
                {
                    #region "BatchProcess End"

                    int batchStatus = (taskResponse.StatusResponse.Status == Constants.StatusResponse.Success)
                            ? Constants.BatchProcessStatus.Success
                            : Constants.BatchProcessStatus.Fail;

                    DateTime endTime = DateTime.Now;
                    TimeSpan processTime = endTime.Subtract(_schedDateTime);

                    string processDetail = taskResponse.ToString();

                    AppLog.BatchProcessEnd(Constants.BatchProcessCode.ExportDailySRReport, batchStatus, endTime, processTime, processDetail);

                    #endregion

                    if (batchStatus == Constants.BatchProcessStatus.Fail || taskResponse.TransferStatus == false)
                    {
                        _mailSender = new CSMMailSender();
                        _mailSender.NotifyExportSRReportFailed(taskResponse);
                    }
                }
            }
            return taskResponse;
        }

        public ExportSRTaskResponse SRReport()
        {
            long _elapsedTime = 0;
            DateTime _schedDateTime = DateTime.Now;
            //int batchStatus = Constants.BatchProcessStatus.Processing;

            ExportSRTaskResponse taskResponse = new ExportSRTaskResponse
            {
                SchedDateTime = _schedDateTime,
                TaskAction = Constants.AuditAction.ExportSRAccum,
                StatusResponse = new StatusResponse
                {
                    Status = Constants.StatusResponse.Failed,
                    ErrorCode = string.Empty,
                    Description = ""
                }
            };

            using (var _reportFacade = new ReportFacade())
            {

                System.Diagnostics.Stopwatch _stopwatch = System.Diagnostics.Stopwatch.StartNew();
                _logger.Debug("-- Start Cron Job --:--Get ExportMonthlySRReport--");

                #region "BatchProcess Start"

                if (AppLog.BatchProcessStart(Constants.BatchProcessCode.ExportMonthlySRReport, _schedDateTime) == false)
                {
                    _logger.Info("I:--NOT PROCESS--:--ExportMonthlySRReport--");

                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;
                    _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                    taskResponse.ElapsedTime = _elapsedTime;
                    taskResponse.StatusResponse.Status = Constants.StatusResponse.NotProcess;
                    taskResponse.StatusResponse.Description = "NOT PROCESS";
                    return taskResponse;
                }

                #endregion

                var triggerDay = _reportFacade.GetTriggerDays();
                string[] triggerDays = triggerDay.Split(',');
                int[] intTriggerDays = Array.ConvertAll(triggerDays, s => int.Parse(s));

                if (intTriggerDays.Contains(DateTime.Now.Day))
                {
                    _commonFacade = new CommonFacade();

                    var numDaysSRReport = _reportFacade.GetNumOfDaySRReport();

                    ExportSRSearchFilter searchFilter = new ExportSRSearchFilter
                    {
                        PageNo = 1,
                        PageSize = _commonFacade.GetPageSizeStart(),
                        SortField = string.Empty,
                        SortOrder = "DESC",
                        SRDateFrom = DateTime.Now.Date.AddDays(int.Parse(numDaysSRReport)).ToString(),
                        SRDateTo = DateTime.Now.Date.ToString(),
                        SRStatus = "-1",
                        SRStatusName = "ทั้งหมด"
                    };

                    try
                    {
                        #region "Check Time to Export"

                        string errorMessage;
                        bool isValid = CheckTimeToExport(out errorMessage);

                        #endregion

                        string dateStr = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.ExportDateTime);
                        string fileDownloadName = string.Format(CultureInfo.InvariantCulture, "{0}_{1}.{2}", Resource.Report_SR_Monthly_FileName, dateStr, Resource.Report_FileExtention);
                        string fileNameChkDup = $"{Resource.Report_SR_Monthly_FileName}_{DateTime.Now.FormatDateTime(Constants.DateTimeFormat.ExportAfsDateTime)}*.{Resource.Report_FileExtention}";
                        string reportPath = _reportFacade.GetReportPath();
                        string[] fileDups = Directory.GetFiles(reportPath, fileNameChkDup);

                        IList<ExportSREntity> srList = null;

                        if (fileDups.Length == 0)
                        { srList = _reportFacade.GetExportSRMonthly(searchFilter); }
                        else
                        { srList = new List<ExportSREntity>(); }

                        if (fileDups.Length > 0 || (srList != null /*&& srList.Count > 0*/))
                        {
                            if (fileDups.Length == 0)
                            {
                                //var bytes = ExcelHelpers.WriteToExcelSR(HostingEnvironment.MapPath("~/Templates/Reports/rpt_sr.xlsx"), DateTime.Now, ds);
                                //// Create the file.
                                //using (FileStream fs = File.Create(@"D:\CsmPath\"+fileDownloadName))
                                //{
                                //    Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                                //    // Add some information to the file.
                                //    fs.Write(info, 0, info.Length);
                                //}
                                var bytes = _reportFacade.CreateReportSR(srList, searchFilter); //ExcelHelpers.WriteToCSV(DateTime.Now, ds);

                                File.WriteAllBytes(reportPath + @"\" + fileDownloadName, bytes);
                                //File(bytes, "application/octet-stream", fileDownloadName);
                            }

                            string[] files = Directory.GetFiles(reportPath, $"{Resource.Report_SR_Monthly_FileName}_*.{Resource.Report_FileExtention}");
                            bool transfer = false;
                            try
                            {
                                if (files.Length > 0)
                                {
                                    transfer = true;
                                    foreach (var f in files)
                                    {
                                        var cfg = new SRReportSFtpConfig("Accum") { UploadFile = f };
                                        if (_reportFacade.UploadFileSRReport(cfg) == false)
                                        {
                                            transfer = false;
                                            break;
                                        }
                                        else { StreamDataHelpers.TryToDelete(f); }
                                    }
                                }
                                else { taskResponse.StatusResponse.Description = "No file to transfer."; }
                            }
                            catch (Exception ex)
                            {
                                transfer = false;
                                taskResponse.StatusResponse.Description = ex.Message;
                            }

                            _stopwatch.Stop();
                            _elapsedTime = _stopwatch.ElapsedMilliseconds;

                            taskResponse.ElapsedTime = _elapsedTime;
                            taskResponse.NumOfActivity = srList == null ? 0 : srList.Count;
                            taskResponse.StatusResponse.Status = Constants.StatusResponse.Success;
                            taskResponse.TransferStatus = transfer;
                            _reportFacade.SaveAuditLogSRReport(taskResponse);
                            return taskResponse;
                        }

                        _stopwatch.Stop();
                        _elapsedTime = _stopwatch.ElapsedMilliseconds;

                        taskResponse.ElapsedTime = _elapsedTime;
                        taskResponse.NumOfActivity = srList == null ? 0 : srList.Count;
                        taskResponse.StatusResponse.Description = "No data to export.";
                        _reportFacade.SaveAuditLogSRReport(taskResponse);
                        return taskResponse;

                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Exception occur:\n", ex);
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Monthly SR Report").Add("Error Message", ex.Message).ToFailLogString());

                        _stopwatch.Stop();
                        _elapsedTime = _stopwatch.ElapsedMilliseconds;

                        taskResponse.ElapsedTime = _elapsedTime;
                        taskResponse.StatusResponse.Description = ex.Message;
                        _reportFacade.SaveAuditLogSRReport(taskResponse);
                    }
                    finally
                    {
                        #region "BatchProcess End"

                        int batchStatus = (taskResponse.StatusResponse.Status == Constants.StatusResponse.Success)
                            ? Constants.BatchProcessStatus.Success
                            : Constants.BatchProcessStatus.Fail;

                        DateTime endTime = DateTime.Now;
                        TimeSpan processTime = endTime.Subtract(_schedDateTime);

                        string processDetail = taskResponse.ToString();

                        AppLog.BatchProcessEnd(Constants.BatchProcessCode.ExportMonthlySRReport, batchStatus, endTime, processTime, processDetail);

                        #endregion

                        if (batchStatus == Constants.BatchProcessStatus.Fail || taskResponse.TransferStatus == false)
                        {
                            _mailSender = new CSMMailSender();
                            _mailSender.NotifyExportSRReportFailed(taskResponse);
                        }
                    }
                    return taskResponse;
                }
                else
                {
                    #region "BatchProcess End"

                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;

                    taskResponse = new ExportSRTaskResponse
                    {
                        SchedDateTime = _schedDateTime,
                        ElapsedTime = _elapsedTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = Constants.StatusResponse.Failed,
                            ErrorCode = string.Empty,
                            Description = $"รันเฉพาะวันที่ {intTriggerDays.Aggregate(string.Empty, (accum, val) => accum + (accum == string.Empty ? "" : ", ") + val.ToString())} เท่านั้น"
                        },
                    };

                    int batchStatus = (taskResponse.StatusResponse.Status == Constants.StatusResponse.Success)
                            ? Constants.BatchProcessStatus.Success
                            : Constants.BatchProcessStatus.Fail;

                    DateTime endTime = DateTime.Now;
                    TimeSpan processTime = endTime.Subtract(_schedDateTime);

                    string processDetail = !string.IsNullOrEmpty(taskResponse.StatusResponse.Description)
                           ? taskResponse.StatusResponse.Description
                           : taskResponse.ToString();

                    AppLog.BatchProcessEnd(Constants.BatchProcessCode.ExportMonthlySRReport, batchStatus, endTime, processTime, processDetail);

                    #endregion

                    return taskResponse;
                }
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
                    if (_commonFacade != null) { _commonFacade.Dispose(); }
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
