using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Service.Messages.SchedTask;
using CSM.Service.Messages.Sr;
using CSM.Business;
using CSM.Data.DataAccess;
using CSM.Common.Resources;
using log4net;
using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Web.Hosting;
using System.IO;
using System.Linq;
using CSM.Service.Messages.Common;

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
                _context = new CSMContext();
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

        private bool CheckTimeToExport(out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                if (_reportDataAccess == null) { _reportDataAccess = new ReportDataAccess(_context); }

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

        private readonly CSMContext _context;
        private IReportDataAccess _reportDataAccess;
        private ICommonFacade _commonFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CSMSRService));

        public ExportSRTaskResponse SRReport()
        {
            ExportSRTaskResponse taskResponse = null;

            long _elapsedTime = 0;
            DateTime _schedDateTime = DateTime.Now;
            //int batchStatus = Constants.BatchProcessStatus.Processing;

            System.Diagnostics.Stopwatch _stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Debug("-- Start Cron Job --:--Get ExportMonthlySRReport--");

            #region "BatchProcess Start"

            if (AppLog.BatchProcessStart(Constants.BatchProcessCode.ExportMonthlySRReport, _schedDateTime) == false)
            {
                _logger.Info("I:--NOT PROCESS--:--ExportMonthlySRReport--");

                _stopwatch.Stop();
                _elapsedTime = _stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                taskResponse = new ExportSRTaskResponse
                {
                    SchedDateTime = _schedDateTime,
                    ElapsedTime = _elapsedTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.NotProcess,
                        ErrorCode = string.Empty,
                        Description = ""
                    }
                };

                //batchStatus = Constants.BatchProcessStatus.Fail;
                return taskResponse;
            }

            #endregion

            var triggerDay = _context.TB_C_PARAMETER.FirstOrDefault(x => x.PARAMETER_NAME == Constants.ParameterName.TriggerDays).PARAMETER_VALUE;
            string[] triggerDays = triggerDay.Split(',');
            int[] intTriggerDays = Array.ConvertAll(triggerDays, s => int.Parse(s));

            if (intTriggerDays.Contains(DateTime.Now.Day))
            {
                _commonFacade = new CommonFacade();

                var numDaysSRReport = _context.TB_C_PARAMETER.FirstOrDefault(x => x.PARAMETER_NAME == Constants.ParameterName.NumDaysSRReport).PARAMETER_VALUE;

                ExportSRSearchFilter searchFilter = new ExportSRSearchFilter
                {
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = string.Empty,
                    SortOrder = "DESC",
                    SRDateFrom = DateTime.Now.Date.AddDays(int.Parse(numDaysSRReport)).ToString(),
                    SRDateTo = DateTime.Now.Date.ToString(),
                    SRStatus = "-1"
                };

                try
                {
                    #region "Check Time to Export"

                    string errorMessage;
                    bool isValid = CheckTimeToExport(out errorMessage);

                    #endregion

                    if (_reportDataAccess == null) { _reportDataAccess = new ReportDataAccess(_context); }
                    using (var ds = new DataSet("Item"))
                    {
                        ds.Locale = CultureInfo.CurrentCulture;
                        _reportDataAccess = new ReportDataAccess(_context);
                        IList<ExportSREntity> exportSR = _reportDataAccess.GetExportSRMonthly(searchFilter);
                        DataTable dt = DataTableHelpers.ConvertTo(exportSR);
                        ds.Tables.Add(dt);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            //var bytes = ExcelHelpers.WriteToExcelSR(HostingEnvironment.MapPath("~/Templates/Reports/rpt_sr.xlsx"), DateTime.Now, ds);
                            var bytes = ExcelHelpers.WriteToCSV(DateTime.Now, ds);

                            string dateStr = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.ExportDateTime);
                            string fileDownloadName = string.Format(CultureInfo.InvariantCulture, "{0}_{1}.{2}", Resource.Report_SR_Monthly_FileName, dateStr, Resource.Report_FileExtention);

                            //// Create the file.
                            //using (FileStream fs = File.Create(@"D:\CsmPath\"+fileDownloadName))
                            //{
                            //    Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                            //    // Add some information to the file.
                            //    fs.Write(info, 0, info.Length);
                            //}
                            var reportPath = _context.TB_C_PARAMETER.FirstOrDefault(x => x.PARAMETER_NAME == Constants.ParameterName.ReportPath).PARAMETER_VALUE;
                            File.WriteAllBytes(reportPath + @"\" + fileDownloadName, bytes);
                            //File(bytes, "application/octet-stream", fileDownloadName);

                            _stopwatch.Stop();
                            _elapsedTime = _stopwatch.ElapsedMilliseconds;

                            taskResponse = new ExportSRTaskResponse
                            {
                                SchedDateTime = _schedDateTime,
                                ElapsedTime = _elapsedTime,
                                StatusResponse = new StatusResponse
                                {
                                    Status = Constants.StatusResponse.Success,
                                    ErrorCode = string.Empty
                                },
                                NumOfActivity = dt.Rows.Count
                            };

                            //_afsFacade.SaveLogExportSuccess(taskResponse);
                            return taskResponse;
                        }

                        _stopwatch.Stop();
                        _elapsedTime = _stopwatch.ElapsedMilliseconds;

                        taskResponse = new ExportSRTaskResponse
                        {
                            SchedDateTime = _schedDateTime,
                            ElapsedTime = _elapsedTime,
                            StatusResponse = new StatusResponse
                            {
                                Status = Constants.StatusResponse.Failed,
                                ErrorCode = string.Empty
                            },
                            NumOfActivity = dt.Rows.Count
                        };
                        return taskResponse;
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error("Exception occur:\n", ex);
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Monthly SR Report").Add("Error Message", ex.Message).ToFailLogString());

                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;

                    taskResponse = new ExportSRTaskResponse
                    {
                        SchedDateTime = _schedDateTime,
                        ElapsedTime = _elapsedTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = Constants.StatusResponse.Failed,
                            ErrorCode = string.Empty
                        },
                    };
                }
                finally
                {
                    #region "BatchProcess End"

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
                        ErrorCode = string.Empty
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

        public ExportSRTaskResponse DailySRReport()
        {
            ExportSRTaskResponse taskResponse = null;

            long _elapsedTime = 0;
            DateTime _schedDateTime = DateTime.Now;
            //int batchStatus= Constants.BatchProcessStatus.Processing;

            System.Diagnostics.Stopwatch _stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Debug("-- Start Cron Job --:--Get ExportDailySRReport--");

            #region "BatchProcess Start"

            if (AppLog.BatchProcessStart(Constants.BatchProcessCode.ExportDailySRReport, _schedDateTime) == false)
            {
                _logger.Info("I:--NOT PROCESS--:--ExportDailySRReport--");

                _stopwatch.Stop();
                _elapsedTime = _stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                taskResponse = new ExportSRTaskResponse
                {
                    SchedDateTime = _schedDateTime,
                    ElapsedTime = _elapsedTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.NotProcess,
                        ErrorCode = string.Empty,
                        Description = ""
                    }
                };

                //batchStatus = Constants.BatchProcessStatus.Fail;
                return taskResponse;
            }

            #endregion

            _commonFacade = new CommonFacade();

            ExportSRSearchFilter searchFilter = new ExportSRSearchFilter
            {
                PageNo = 1,
                PageSize = _commonFacade.GetPageSizeStart(),
                SortField = string.Empty,
                SortOrder = "DESC",
                SRDateFrom = DateTime.Now.Date.AddDays(-1).ToString(),
                SRDateTo = DateTime.Now.Date.ToString(),
                SRStatus = "-1"
            };

            try
            {
                #region "Check Time to Export"

                string errorMessage;
                bool isValid = CheckTimeToExport(out errorMessage);

                #endregion

                if (_reportDataAccess == null) { _reportDataAccess = new ReportDataAccess(_context); }
                using (var ds = new DataSet("Item"))
                {
                    ds.Locale = CultureInfo.CurrentCulture;
                    _reportDataAccess = new ReportDataAccess(_context);
                    IList<ExportSREntity> exportSR = _reportDataAccess.GetExportSR(searchFilter);
                    DataTable dt = DataTableHelpers.ConvertTo(exportSR);
                    ds.Tables.Add(dt);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        //var bytes = ExcelHelpers.WriteToExcelSR(HostingEnvironment.MapPath("~/Templates/Reports/rpt_sr.xlsx"), DateTime.Now, ds);
                        var bytes = ExcelHelpers.WriteToCSV(DateTime.Now, ds);

                        string dateStr = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.ExportDateTime);
                        string fileDownloadName = string.Format(CultureInfo.InvariantCulture, "{0}_{1}.{2}", Resource.Report_SR_Daily_FileName, dateStr, Resource.Report_FileExtention);

                        //// Create the file.
                        //using (FileStream fs = File.Create(@"D:\CsmPath\"+fileDownloadName))
                        //{
                        //    Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                        //    // Add some information to the file.
                        //    fs.Write(info, 0, info.Length);
                        //}
                        var reportPath = _context.TB_C_PARAMETER.FirstOrDefault(x => x.PARAMETER_NAME == Constants.ParameterName.ReportPath).PARAMETER_VALUE;
                        File.WriteAllBytes(reportPath + @"\" + fileDownloadName, bytes);
                        //File(bytes, "application/octet-stream", fileDownloadName);

                        _stopwatch.Stop();
                        _elapsedTime = _stopwatch.ElapsedMilliseconds;

                        taskResponse = new ExportSRTaskResponse
                        {
                            SchedDateTime = _schedDateTime,
                            ElapsedTime = _elapsedTime,
                            StatusResponse = new StatusResponse
                            {
                                Status = Constants.StatusResponse.Success,
                                ErrorCode = string.Empty
                            },
                            NumOfActivity = dt.Rows.Count
                        };

                        //_afsFacade.SaveLogExportSuccess(taskResponse);
                        return taskResponse;
                        //batchStatus = Constants.BatchProcessStatus.Success;
                        //return true;
                    }

                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;

                    taskResponse = new ExportSRTaskResponse
                    {
                        SchedDateTime = _schedDateTime,
                        ElapsedTime = _elapsedTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = Constants.StatusResponse.Failed,
                            ErrorCode = string.Empty
                        },
                        NumOfActivity = dt.Rows.Count
                    };
                    return taskResponse;
                }
                
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Daily SR Report").Add("Error Message", ex.Message).ToFailLogString());
                //batchStatus = Constants.BatchProcessStatus.Fail;
                _stopwatch.Stop();
                _elapsedTime = _stopwatch.ElapsedMilliseconds;

                taskResponse = new ExportSRTaskResponse
                {
                    SchedDateTime = _schedDateTime,
                    ElapsedTime = _elapsedTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty
                    },
                };
            }
            finally
            {
                #region "BatchProcess End"

                int batchStatus = (taskResponse.StatusResponse.Status == Constants.StatusResponse.Success)
                        ? Constants.BatchProcessStatus.Success
                        : Constants.BatchProcessStatus.Fail;

                DateTime endTime = DateTime.Now;
                TimeSpan processTime = endTime.Subtract(_schedDateTime);

                string processDetail = !string.IsNullOrEmpty(taskResponse.StatusResponse.Description)
                       ? taskResponse.StatusResponse.Description
                       : taskResponse.ToString();

                AppLog.BatchProcessEnd(Constants.BatchProcessCode.ExportDailySRReport, batchStatus, endTime, processTime, processDetail);

                #endregion
            }

            return taskResponse;
        }

        public bool FirstMonthlySRReport()
        {
            if (DateTime.Now.Day == 1)
            {
                _commonFacade = new CommonFacade();

                ExportSRSearchFilter searchFilter = new ExportSRSearchFilter
                {
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = string.Empty,
                    SortOrder = "DESC",
                    SRDateFrom = DateTime.Now.Date.AddDays(-90).ToString(),
                    SRDateTo = DateTime.Now.Date.ToString(),
                    SRStatus = "-1"
                };

                try
                {
                    #region "Check Time to Export"

                    string errorMessage;
                    bool isValid = CheckTimeToExport(out errorMessage);

                    #endregion

                    if (_reportDataAccess == null) { _reportDataAccess = new ReportDataAccess(_context); }
                    using (var ds = new DataSet("Item"))
                    {
                        ds.Locale = CultureInfo.CurrentCulture;
                        _reportDataAccess = new ReportDataAccess(_context);
                        IList<ExportSREntity> exportSR = _reportDataAccess.GetExportSRMonthly(searchFilter);
                        DataTable dt = DataTableHelpers.ConvertTo(exportSR);
                        ds.Tables.Add(dt);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            //var bytes = ExcelHelpers.WriteToExcelSR(HostingEnvironment.MapPath("~/Templates/Reports/rpt_sr.xlsx"), DateTime.Now, ds);
                            var bytes = ExcelHelpers.WriteToCSV(DateTime.Now, ds);

                            string dateStr = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.ExportDateTime);
                            string fileDownloadName = string.Format(CultureInfo.InvariantCulture, "{0}_{1}.{2}", Resource.Report_SR_FileName, dateStr, Resource.Report_FileExtention);

                            File.WriteAllBytes(@"D:\CsmPath\" + fileDownloadName, bytes);
                            //File(bytes, "application/octet-stream", fileDownloadName);

                            return true;
                        }

                        return false;
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error("Exception occur:\n", ex);
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Daily SR Report").Add("Error Message", ex.Message).ToFailLogString());
                }

                return true;
            }
            else return false;
        }

        public bool MidMonthlySRReport()
        {
            if (DateTime.Now.Day == 16)
            {
                _commonFacade = new CommonFacade();

                ExportSRSearchFilter searchFilter = new ExportSRSearchFilter
                {
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = string.Empty,
                    SortOrder = "DESC",
                    SRDateFrom = DateTime.Now.Date.AddDays(-90).ToString(),
                    SRDateTo = DateTime.Now.Date.ToString(),
                    SRStatus = "-1"
                };

                try
                {
                    #region "Check Time to Export"

                    string errorMessage;
                    bool isValid = CheckTimeToExport(out errorMessage);

                    #endregion

                    if (_reportDataAccess == null) { _reportDataAccess = new ReportDataAccess(_context); }
                    using (var ds = new DataSet("Item"))
                    {
                        ds.Locale = CultureInfo.CurrentCulture;
                        _reportDataAccess = new ReportDataAccess(_context);
                        IList<ExportSREntity> exportSR = _reportDataAccess.GetExportSRMonthly(searchFilter);
                        DataTable dt = DataTableHelpers.ConvertTo(exportSR);
                        ds.Tables.Add(dt);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            //var bytes = ExcelHelpers.WriteToExcelSR(HostingEnvironment.MapPath("~/Templates/Reports/rpt_sr.xlsx"), DateTime.Now, ds);
                            var bytes = ExcelHelpers.WriteToCSV(DateTime.Now, ds);

                            string dateStr = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.ExportDateTime);
                            string fileDownloadName = string.Format(CultureInfo.InvariantCulture, "{0}_{1}.{2}", Resource.Report_SR_FileName, dateStr, Resource.Report_FileExtention);

                            File.WriteAllBytes(@"D:\CsmPath\" + fileDownloadName, bytes);
                            //File(bytes, "application/octet-stream", fileDownloadName);

                            return true;
                        }

                        return false;
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error("Exception occur:\n", ex);
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Daily SR Report").Add("Error Message", ex.Message).ToFailLogString());
                }

                return true;
            }
            else return false;
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