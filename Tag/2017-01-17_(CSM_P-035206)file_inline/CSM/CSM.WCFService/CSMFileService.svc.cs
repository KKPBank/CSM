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

namespace CSM.WCFService
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CSMFileService : ICSMFileService
    {
        private long _elapsedTime;
        private string _logDetail;
        private IAFSFacade _afsFacade;
        private readonly ILog _logger;
        private ICisFacade _cisFacade;
        private CSMMailSender _mailSender;
        private object sync = new Object();
        private List<SaleZoneEntity> _saleZones;
        private List<PropertyEntity> _properties;
        private System.Diagnostics.Stopwatch _stopwatch;

        private IBdwFacade _bdwFacade;
        private List<BdwContactEntity> _bdwContacts;

        private IHpFacade _hpFacade;
        private List<HpActivityEntity> _hpActivity;

        #region "Constructor"

        public CSMFileService()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();

                // Set logfile name and application name variables
                GlobalContext.Properties["ApplicationCode"] = "CSMWS";
                GlobalContext.Properties["ServerName"] = Environment.MachineName;
                _logger = LogManager.GetLogger(typeof(CSMFileService));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
            }
        }

        #endregion

        public ImportAFSTaskResponse GetFileAFS(string username, string password)
        {
            ImportAFSTaskResponse taskResponse = null;

            try
            {
                ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
                ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

                if (!string.IsNullOrWhiteSpace(username))
                {
                    ThreadContext.Properties["UserID"] = username.ToUpperInvariant();
                }

                string fiProp = string.Empty;
                string fiSaleZone = string.Empty;

                DateTime schedDateTime = DateTime.Now;
                _stopwatch = System.Diagnostics.Stopwatch.StartNew();
                _logger.Debug("-- Start Cron Job --:--Get GetFileAFS--");

                #region "BatchProcess Start"

                if (AppLog.BatchProcessStart(Constants.BatchProcessCode.ImportAFS, schedDateTime) == false)
                {
                    _logger.Info("I:--NOT PROCESS--:--GetFileAFS--");

                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;
                    _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                    taskResponse = new ImportAFSTaskResponse
                    {
                        SchedDateTime = schedDateTime,
                        ElapsedTime = _elapsedTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = Constants.StatusResponse.NotProcess,
                            ErrorCode = string.Empty,
                            Description = "",
                        },

                    };

                    return taskResponse;
                }

                #endregion

                if (!ApplicationHelpers.Authenticate(username, password))
                {
                    _logDetail = "Username and/or Password Invalid.";
                    _logger.InfoFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");
                    goto Outer;
                }

                try
                {
                    _logger.Info("I:--START--:--Get GetFileAFS--");

                    #region "AFS Settings"

                    int numOfProp = 0;
                    int numOfErrProp = 0;
                    int numOfComplete = 0;
                    int numOfSaleZones = 0;
                    int numOfErrSaleZone = 0;

                    _afsFacade = new AFSFacade();
                    string afsProperty = WebConfig.GetAFSProperty();
                    string afsSaleZone = WebConfig.GetAFSSaleZone();
                    string afsPath = _afsFacade.GetParameter(Constants.ParameterName.AFSPathImport);
                    string afsExportPath = _afsFacade.GetParameter(Constants.ParameterName.AFSPathError);

                    bool isValidFile_SaleZone = true;
                    string msgValidateFileError_SaleZone = "";

                    bool isValidFile_Property = true;
                    string msgValidateFileError_Property = "";

                    #endregion

                    _saleZones = _afsFacade.ReadFileSaleZone(afsPath, afsSaleZone, ref numOfSaleZones, ref fiSaleZone,
                        ref isValidFile_SaleZone, ref msgValidateFileError_SaleZone);
                    _properties = _afsFacade.ReadFileProperty(afsPath, afsProperty, ref numOfProp, ref fiProp,
                        ref isValidFile_Property, ref msgValidateFileError_Property, isValidFile_SaleZone);

                    if (string.IsNullOrWhiteSpace(fiSaleZone) && string.IsNullOrWhiteSpace(fiProp))
                    {
                        _logger.Info("O:--FAILED--:--File not found--");
                        _logDetail = "File not found";
                        goto Outer;
                    }

                    if ((_saleZones == null && isValidFile_SaleZone == false) ||
                        (_properties == null && isValidFile_Property == false))
                    {
                        _logDetail += (isValidFile_SaleZone == false) ? "[SaleZone]: " + msgValidateFileError_SaleZone : "";
                        _logDetail += (isValidFile_Property == false) ? " [Property]: " + msgValidateFileError_Property : "";

                        _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                        goto Outer;
                    }

                    Task.Factory.StartNew(() => Parallel.ForEach(_properties, i =>
                    {
                        lock (sync)
                        {
                            GetSaleZoneInfo(i);
                        }
                    })).Wait();

                    if (!_afsFacade.SaveSaleZones(_saleZones, fiSaleZone))
                    {
                        _logger.Info("O:--FAILED--:--Save Sale Zones--");
                        _logDetail = "Failed save Sale Zones";
                        goto Outer;
                    }

                    if (_afsFacade.SaveAFSProperties(_properties, fiProp))
                    {
                        if (_afsFacade.SaveCompleteProperties(ref numOfComplete))
                        {
                            string exportPropFile = this.GetAfsFileFormat(fiProp, afsProperty, schedDateTime);
                            bool exportProp = _afsFacade.ExportErrorProperties(afsExportPath, exportPropFile, ref numOfErrProp);
                            if (!exportProp)
                            {
                                _logDetail = "Failed to export AFS Property";
                                goto Outer;
                            }

                            string exportSaleZoneFile = this.GetAfsFileFormat(fiSaleZone, afsSaleZone, schedDateTime);
                            bool exportSaleZone = _afsFacade.ExportErrorSaleZones(afsExportPath, exportSaleZoneFile, ref numOfErrSaleZone);
                            if (!exportSaleZone)
                            {
                                _logDetail = "Failed to export Sale Zone";
                                goto Outer;
                            }

                            _logger.Info("O:--SUCCESS--:--Get GetFileAFS--");
                        }
                        else
                        {
                            _logger.Info("O:--FAILED--:--Cannot save AFS Asset--");
                            _logDetail = "Failed to save AFS Asset";
                            goto Outer;
                        }
                    }
                    else
                    {
                        _logger.Info("O:--FAILED--:--Save AFS Property--");
                        _logDetail = "Failed to save AFS Property";
                        goto Outer;
                    }

                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;
                    _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                    taskResponse = new ImportAFSTaskResponse
                    {
                        SchedDateTime = schedDateTime,
                        ElapsedTime = _elapsedTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = (numOfErrProp > 0) ? Constants.StatusResponse.Failed : Constants.StatusResponse.Success
                        },
                        FileList = new List<object> { fiProp, fiSaleZone },
                        NumOfProp = numOfProp,
                        NumOfSaleZones = numOfSaleZones,
                        NumOfComplete = numOfComplete,
                        NumOfErrProp = numOfErrProp,
                        NumOfErrSaleZone = numOfErrSaleZone
                    };

                    _afsFacade.SaveLogSuccessOrFail(taskResponse);
                    return taskResponse;
                }
                catch (Exception ex)
                {
                    _logDetail = ex.Message;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    _logger.Error("Exception occur:\n", ex);
                }

                Outer:
                _stopwatch.Stop();
                _elapsedTime = _stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                taskResponse = new ImportAFSTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = _elapsedTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = _logDetail
                    },
                    FileList = new List<object>() { fiProp, fiSaleZone }
                };

                _afsFacade.SaveLogError(taskResponse);
                return taskResponse;
            }
            finally
            {
                // Send mail to system administrator
                ImportAfsSendMail(taskResponse);

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

                    AppLog.BatchProcessEnd(Constants.BatchProcessCode.ImportAFS, batchStatus, endTime, processTime, processDetail);
                }

                #endregion
            }
        }

        public ExportAFSTaskResponse ExportFileAFS(string username, string password)
        {
            ExportAFSTaskResponse taskResponse = null;

            try
            {
                ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
                ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

                if (!string.IsNullOrWhiteSpace(username))
                {
                    ThreadContext.Properties["UserID"] = username.ToUpperInvariant();
                }

                DateTime schedDateTime = DateTime.Now;
                _stopwatch = System.Diagnostics.Stopwatch.StartNew();
                _logger.Debug("-- Start Cron Job --:--Get ExportFileAFS--");

                #region "BatchProcess Start"

                if (AppLog.BatchProcessStart(Constants.BatchProcessCode.ExportAFS, schedDateTime) == false)
                {
                    _logger.Info("I:--NOT START--:--Get ExportFileAFS--");

                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;
                    _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                    taskResponse = new ExportAFSTaskResponse
                    {
                        SchedDateTime = schedDateTime,
                        ElapsedTime = _elapsedTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = Constants.StatusResponse.NotProcess,
                            ErrorCode = string.Empty,
                            Description = ""
                        }
                    };

                    return taskResponse;
                }

                #endregion

                if (!ApplicationHelpers.Authenticate(username, password))
                {
                    _logDetail = "Username and/or Password Invalid.";
                    _logger.InfoFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");
                    goto Outer;
                }
                try
                {
                    _logger.Info("I:--START--:--Get ExportFileAFS--");

                    #region "AFS Settings"

                    int NumOfActivity = 0;

                    _afsFacade = new AFSFacade();
                    string afsactivities = WebConfig.GetActivityAFS();
                    string afsexportpath = _afsFacade.GetParameter(Constants.ParameterName.AFSPathExport);

                    #endregion

                    bool exportAc = _afsFacade.ExportActivityAFS(afsexportpath, afsactivities, ref NumOfActivity);
                    if (!exportAc)
                    {
                        _logDetail = "Fail to export activity";
                        goto Outer;
                    }

                    _logger.Info("O:--SUCCESS--:--Get ExportFileAFS--");

                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;
                    _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                    taskResponse = new ExportAFSTaskResponse
                    {
                        SchedDateTime = schedDateTime,
                        ElapsedTime = _elapsedTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = Constants.StatusResponse.Success,
                            ErrorCode = string.Empty
                        },
                        NumOfActivity = NumOfActivity
                    };

                    _afsFacade.SaveLogExportSuccess(taskResponse);
                    return taskResponse;
                }
                catch (Exception ex)
                {
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", ex.Message);
                    _logger.Error("Exception occur:\n", ex);
                    _logDetail = "Fail to export activity";
                }

                Outer:
                _stopwatch.Stop();
                _elapsedTime = _stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                taskResponse = new ExportAFSTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = _elapsedTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = _logDetail
                    }
                };

                _afsFacade.SaveLogExportError(taskResponse);
                return taskResponse;
            }
            finally
            {
                // Send mail to system administrator
                ExportAfsSendMail(taskResponse);

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

                    AppLog.BatchProcessEnd(Constants.BatchProcessCode.ExportAFS, batchStatus, endTime, processTime, processDetail);
                }

                #endregion
            }
        }

        #region Backup ExportFileNCB 20170308
        //public ExportNCBTaskResponse ExportFileNCB(string username, string password)
        //{
        //    ExportNCBTaskResponse taskResponse = null;

        //    try
        //    {
        //        ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
        //        ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

        //        if (!string.IsNullOrWhiteSpace(username))
        //        {
        //            ThreadContext.Properties["UserID"] = username.ToUpperInvariant();
        //        }

        //        DateTime schedDateTime = DateTime.Now;
        //        _stopwatch = System.Diagnostics.Stopwatch.StartNew();
        //        _logger.Debug("-- Start Cron Job --:--Get ExportFileNCB--");

        //        #region "BatchProcess Start"

        //        if (AppLog.BatchProcessStart(Constants.BatchProcessCode.ExportMarketing, schedDateTime) == false)
        //        {
        //            _logger.Info("I:--NOT PROCESS--:--ExportFileNCB--");

        //            _stopwatch.Stop();
        //            _elapsedTime = _stopwatch.ElapsedMilliseconds;
        //            _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

        //            taskResponse = new ExportNCBTaskResponse
        //            {
        //                SchedDateTime = schedDateTime,
        //                ElapsedTime = _elapsedTime,
        //                StatusResponse = new StatusResponse
        //                {
        //                    Status = Constants.StatusResponse.NotProcess,
        //                    ErrorCode = string.Empty,
        //                    Description = "",
        //                }
        //            };

        //            return taskResponse;
        //        }

        //        #endregion

        //        if (!ApplicationHelpers.Authenticate(username, password))
        //        {
        //            _logDetail = "Username and/or Password Invalid.";
        //            _logger.InfoFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");
        //            goto Outer;
        //        }
        //        try
        //        {
        //            _logger.Info("I:--START--:--Get ExportFileNCB--");

        //            #region "AFS Settings"

        //            int numOfNew = 0;
        //            int numOfUpdate = 0;
        //            string newEmplFile = string.Empty;
        //            string updateEmplFile = string.Empty;

        //            _afsFacade = new AFSFacade();
        //            string afsemployeesnew = WebConfig.GetNewEmployeeNCBAFS();
        //            string afsemployeesupdate = WebConfig.GetUpdateEmployeeNCBAFS();
        //            string afsexportpath = _afsFacade.GetParameter(Constants.ParameterName.CICPathExport);

        //            #endregion

        //            bool exportEmNew = _afsFacade.ExportEmployeeNCBNew(afsexportpath, afsemployeesnew, ref numOfNew, ref newEmplFile);
        //            bool exportEmUpdate = _afsFacade.ExportEmplyeeNCBUpdate(afsexportpath, afsemployeesupdate, ref numOfUpdate, ref updateEmplFile);

        //            if (!exportEmNew || !exportEmUpdate)
        //            {
        //                _logDetail = "Fail to export marketing";
        //                goto Outer;
        //            }

        //            if (!string.IsNullOrWhiteSpace(newEmplFile) || !string.IsNullOrWhiteSpace(updateEmplFile))
        //            {
        //                if (!_afsFacade.UploadFilesViaFTP(newEmplFile, updateEmplFile))
        //                {
        //                    _logDetail = "Fail to upload file via SFTP";
        //                    goto Outer;
        //                }
        //            }

        //            _logger.Info("I:--SUCCESS--:--Get ExportFileNCB--");

        //            _stopwatch.Stop();
        //            _elapsedTime = _stopwatch.ElapsedMilliseconds;
        //            _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

        //            taskResponse = new ExportNCBTaskResponse
        //            {
        //                SchedDateTime = schedDateTime,
        //                ElapsedTime = _elapsedTime,
        //                StatusResponse = new StatusResponse
        //                {
        //                    Status = Constants.StatusResponse.Success,
        //                    ErrorCode = string.Empty
        //                },
        //                NumOfNew = numOfNew,
        //                NumOfUpdate = numOfUpdate
        //            };

        //            _afsFacade.SaveLogExportMarketingSuccess(taskResponse);
        //            return taskResponse;
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.InfoFormat("O:--FAILED--:Error Message/{0}", ex.Message);
        //            _logger.Error("Exception occur:\n", ex);
        //            _logDetail = "Fail to export marketing";
        //        }

        //    Outer:
        //        _stopwatch.Stop();
        //        _elapsedTime = _stopwatch.ElapsedMilliseconds;
        //        _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

        //        taskResponse = new ExportNCBTaskResponse
        //        {
        //            SchedDateTime = schedDateTime,
        //            ElapsedTime = _elapsedTime,
        //            StatusResponse = new StatusResponse
        //            {
        //                Status = Constants.StatusResponse.Failed,
        //                ErrorCode = string.Empty,
        //                Description = _logDetail
        //            },
        //        };

        //        _afsFacade.SaveLogExportMarketingError(taskResponse);
        //        return taskResponse;
        //    }
        //    finally
        //    {
        //        // Send mail to system administrator
        //        ExportNcbSendMail(taskResponse);

        //        #region "BatchProcess End"

        //        if (taskResponse != null && taskResponse.StatusResponse != null &&
        //            taskResponse.StatusResponse.Status != Constants.StatusResponse.NotProcess)
        //        {

        //            int batchStatus = (taskResponse.StatusResponse.Status == Constants.StatusResponse.Success)
        //                ? Constants.BatchProcessStatus.Success : Constants.BatchProcessStatus.Fail;

        //            DateTime endTime = taskResponse.SchedDateTime.AddMilliseconds(taskResponse.ElapsedTime);
        //            TimeSpan processTime = endTime.Subtract(taskResponse.SchedDateTime);

        //            string processDetail = !string.IsNullOrEmpty(taskResponse.StatusResponse.Description)
        //               ? taskResponse.StatusResponse.Description
        //               : taskResponse.ToString();

        //            AppLog.BatchProcessEnd(Constants.BatchProcessCode.ExportMarketing, batchStatus, endTime, processTime,
        //                processDetail);
        //        }

        //        #endregion
        //    }
        //}
        #endregion

        public ExportNCBTaskResponse ExportFileNCB(string username, string password)
        {
            ExportNCBTaskResponse taskResponse = null;

            try
            {
                ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
                ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

                if (!string.IsNullOrWhiteSpace(username))
                {
                    ThreadContext.Properties["UserID"] = username.ToUpperInvariant();
                }

                DateTime schedDateTime = DateTime.Now;
                _stopwatch = System.Diagnostics.Stopwatch.StartNew();
                _logger.Debug("-- Start Cron Job --:--Get ExportFileNCB--");

                #region "BatchProcess Start"

                if (AppLog.BatchProcessStart(Constants.BatchProcessCode.ExportMarketing, schedDateTime) == false)
                {
                    _logger.Info("I:--NOT PROCESS--:--ExportFileNCB--");

                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;
                    _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                    taskResponse = new ExportNCBTaskResponse
                    {
                        SchedDateTime = schedDateTime,
                        ElapsedTime = _elapsedTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = Constants.StatusResponse.NotProcess,
                            ErrorCode = string.Empty,
                            Description = "",
                        }
                    };

                    return taskResponse;
                }

                #endregion

                if (!ApplicationHelpers.Authenticate(username, password))
                {
                    _logDetail = "Username and/or Password Invalid.";
                    _logger.InfoFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");
                    goto Outer;
                }
                try
                {
                    _logger.Info("I:--START--:--Get ExportFileNCB--");

                    #region "AFS Settings"

                    int numOfNew = 0;
                    int numOfUpdate = 0;
                    string newEmplFile = string.Empty;
                    string updateEmplFile = string.Empty;

                    _afsFacade = new AFSFacade();
                    string afsemployeesnew = WebConfig.GetNewEmployeeNCBAFS();
                    string afsemployeesupdate = WebConfig.GetUpdateEmployeeNCBAFS();
                    string afsexportpath = _afsFacade.GetParameter(Constants.ParameterName.CICPathExport);

                    #endregion

                    bool exportEmNew = false;
                    bool exportEmUpdate = false;

                    _afsFacade.ExportEmployeeNCB(afsexportpath, afsemployeesnew, afsemployeesupdate, ref numOfNew, ref numOfUpdate, ref newEmplFile, ref updateEmplFile, ref exportEmNew, ref exportEmUpdate);

                    if (!exportEmNew || !exportEmUpdate)
                    {
                        _logDetail = "Fail to export marketing";
                        goto Outer;
                    }

                    if (!string.IsNullOrWhiteSpace(newEmplFile) || !string.IsNullOrWhiteSpace(updateEmplFile))
                    {
                        if (!_afsFacade.UploadFilesViaFTP(newEmplFile, updateEmplFile))
                        {
                            _logDetail = "Fail to upload file via SFTP";
                            goto Outer;
                        }
                    }

                    _logger.Info("I:--SUCCESS--:--Get ExportFileNCB--");

                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;
                    _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                    taskResponse = new ExportNCBTaskResponse
                    {
                        SchedDateTime = schedDateTime,
                        ElapsedTime = _elapsedTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = Constants.StatusResponse.Success,
                            ErrorCode = string.Empty
                        },
                        NumOfNew = numOfNew,
                        NumOfUpdate = numOfUpdate
                    };

                    _afsFacade.SaveLogExportMarketingSuccess(taskResponse);
                    return taskResponse;
                }
                catch (Exception ex)
                {
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", ex.Message);
                    _logger.Error("Exception occur:\n", ex);
                    _logDetail = "Fail to export marketing";
                }

                Outer:
                _stopwatch.Stop();
                _elapsedTime = _stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                taskResponse = new ExportNCBTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = _elapsedTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = _logDetail
                    },
                };

                _afsFacade.SaveLogExportMarketingError(taskResponse);
                return taskResponse;
            }
            finally
            {
                // Send mail to system administrator
                ExportNcbSendMail(taskResponse);

                #region "BatchProcess End"

                if (taskResponse != null && taskResponse.StatusResponse != null &&
                    taskResponse.StatusResponse.Status != Constants.StatusResponse.NotProcess)
                {

                    int batchStatus = (taskResponse.StatusResponse.Status == Constants.StatusResponse.Success)
                        ? Constants.BatchProcessStatus.Success : Constants.BatchProcessStatus.Fail;

                    DateTime endTime = taskResponse.SchedDateTime.AddMilliseconds(taskResponse.ElapsedTime);
                    TimeSpan processTime = endTime.Subtract(taskResponse.SchedDateTime);

                    string processDetail = !string.IsNullOrEmpty(taskResponse.StatusResponse.Description)
                       ? taskResponse.StatusResponse.Description
                       : taskResponse.ToString();

                    AppLog.BatchProcessEnd(Constants.BatchProcessCode.ExportMarketing, batchStatus, endTime, processTime,
                        processDetail);
                }

                #endregion
            }
        }

        public ImportBDWTaskResponse GetFileBDW(string username, string password, bool skipSftp)
        {
            ImportBDWTaskResponse taskResponse = null;

            try
            {
                ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
                ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

                if (!string.IsNullOrWhiteSpace(username))
                {
                    ThreadContext.Properties["UserID"] = username.ToUpperInvariant();
                }

                string fiBdwContact = string.Empty;
                DateTime schedDateTime = DateTime.Now;
                _stopwatch = System.Diagnostics.Stopwatch.StartNew();

                _logger.Debug("-- Start Cron Job --:--Get GetFileBDW--");

                #region "BatchProcess Start"

                if (AppLog.BatchProcessStart(Constants.BatchProcessCode.ImportBDW, schedDateTime) == false)
                {
                    _logger.Info("I:--NOT PROCESS--:--GetFileBDW--");

                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;
                    _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                    taskResponse = new ImportBDWTaskResponse
                    {
                        SchedDateTime = schedDateTime,
                        ElapsedTime = _elapsedTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = Constants.StatusResponse.NotProcess,
                            ErrorCode = string.Empty,
                            Description = "",
                        },

                    };

                    return taskResponse;
                }

                #endregion

                if (!ApplicationHelpers.Authenticate(username, password))
                {
                    _logDetail = "Username and/or Password Invalid.";
                    _logger.InfoFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");
                    goto Outer;
                }

                int numOfBdwContact = 0;
                int numOfError = 0;
                int numOfComplete = 0;
                string messageError = string.Empty;

                try
                {
                    _logger.Info("I:--START--:--Get GetFileBDW--");

                    #region "BDW Contact Settings"

                    _bdwFacade = new BdwFacade();
                    string bdwContactFilePrefix = WebConfig.GetBDWContactPrefix();
                    string bdwPath = _bdwFacade.GetParameter(Constants.ParameterName.BDWPathImport);
                    string bdwExportPath = _bdwFacade.GetParameter(Constants.ParameterName.BDWPathError);

                    bool isValidFile = true;
                    string msgValidateFileError = "";

                    #endregion

                    if (!skipSftp)
                    {
                        if (!_bdwFacade.DownloadFilesViaFTP(bdwPath, bdwContactFilePrefix))
                        {
                            _logDetail = "Cannot download files from SFTP";
                            goto Outer;
                        }
                    }

                    _bdwContacts = _bdwFacade.ReadFileBdwContact(bdwPath, bdwContactFilePrefix, ref numOfBdwContact,
                        ref fiBdwContact, ref isValidFile, ref msgValidateFileError);

                    if (_bdwContacts == null && isValidFile == false)
                    {
                        _logDetail = msgValidateFileError;
                        _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                        goto Outer;
                    }

                    if (_bdwFacade.SaveBdwContact(_bdwContacts))
                    {
                        if (_bdwFacade.SaveCompleteBdwContact(ref numOfComplete, ref numOfError, ref messageError))
                        {
                            if (numOfError > 0)
                            {
                                bool exportError = _bdwFacade.ExportErrorBdwContact(bdwExportPath, fiBdwContact,
                                    ref numOfError);
                                if (!exportError)
                                {
                                    _logDetail = "Failed to export BDW Contact";
                                    goto Outer;
                                }
                            }

                            _logger.Info("I:--SUCCESS--:--Get GetFileBDW--");
                        }
                        else
                        {
                            _logger.Info("I:--FAILED--:--Cannot save BDW Contact --");
                            _logDetail = "Failed to save BDW Contact";
                            goto Outer;
                        }
                    }
                    else
                    {
                        _logger.Info("I:--FAILED--:--Save BDW Contact--");
                        _logDetail = "Failed to save BDW Contact";
                        goto Outer;
                    }

                    var isDeleteFilesViaFTP = true;
                    if (!skipSftp)
                    {
                        if (!_bdwFacade.DeleteFilesViaFTP(bdwContactFilePrefix))
                        {
                            isDeleteFilesViaFTP = false;
                        }
                    }

                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;
                    _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                    taskResponse = new ImportBDWTaskResponse
                    {
                        SchedDateTime = schedDateTime,
                        ElapsedTime = _elapsedTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = (numOfError > 0) ? Constants.StatusResponse.Failed : Constants.StatusResponse.Success
                        },
                        FileList = isDeleteFilesViaFTP ? (new List<object> { fiBdwContact }) : new List<object> { fiBdwContact, "Cannot delete files from SFTP" },
                        NumOfBdwContact = numOfBdwContact,
                        NumOfComplete = numOfComplete,
                        NumOfError = numOfError
                    };

                    _bdwFacade.SaveLogSuccessOrFail(taskResponse);
                    return taskResponse;
                }
                catch (Exception ex)
                {
                    _logDetail = ex.Message;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    _logger.Error("Exception occur:\n", ex);
                }

                Outer:
                _stopwatch.Stop();
                _elapsedTime = _stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                taskResponse = new ImportBDWTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = _elapsedTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = _logDetail
                    },
                    FileList = new List<object>() { fiBdwContact }
                };

                _bdwFacade.SaveLogError(taskResponse);
                return taskResponse;
            }
            finally
            {
                // Send mail to system administrator
                ImportBdwSendMail(taskResponse);

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

                    AppLog.BatchProcessEnd(Constants.BatchProcessCode.ImportBDW, batchStatus, endTime, processTime, processDetail);
                }

                #endregion
            }
        }

        public ImportHRTaskResponse GetFileHR(string username, string password)
        {
            ImportHRTaskResponse taskResponse = null;
            using (HRIFacade facade = new HRIFacade())
            {
                try
                {
                    ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
                    ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

                    if (!string.IsNullOrWhiteSpace(username))
                    {
                        ThreadContext.Properties["UserID"] = username.ToUpperInvariant();
                    }

                    string readFile = string.Empty;
                    DateTime schedDateTime = DateTime.Now;
                    _stopwatch = System.Diagnostics.Stopwatch.StartNew();

                    _logger.Debug("-- Start Task --:--Get GetFileHR--");

                    #region "BatchProcess Start"

                    if (AppLog.BatchProcessStart(Constants.BatchProcessCode.ImportHR, schedDateTime) == false)
                    {
                        _logger.Info("I:--NOT PROCESS--:--GetFileHR--");

                        _stopwatch.Stop();
                        _elapsedTime = _stopwatch.ElapsedMilliseconds;
                        _logger.Debug("-- Finish Task --:ElapsedMilliseconds/" + _elapsedTime);

                        taskResponse = new ImportHRTaskResponse
                        {
                            SchedDateTime = schedDateTime,
                            ElapsedTime = _elapsedTime,
                            StatusResponse = new StatusResponse
                            {
                                Status = Constants.StatusResponse.NotProcess,
                                ErrorCode = string.Empty,
                                Description = "",
                            },

                        };
                        return taskResponse;
                    }

                    #endregion

                    if (!ApplicationHelpers.Authenticate(username, password))
                    {
                        _logDetail = "Username and/or Password Invalid.";
                        _logger.InfoFormat("O:--LOGIN--:Error Message/{0}", _logDetail);
                        goto Outer;
                    }

                    int readRow = 0;
                    int readError = 0;
                    int empInsert = 0;
                    int empUpdate = 0;
                    int empMarkDelete = 0;
                    int buInsert = 0;
                    int buUpdate = 0;
                    int buMarkDelete = 0;
                    int brInsert = 0;
                    int brUpdate = 0;
                    int brMarkDelete = 0;

                    string messageError = string.Empty;
                    List<HRIEmployeeEntity> hriEmpls;

                    try
                    {
                        _logger.Info("I:--START--:--Get GetFileHR--");

                        string msg = "";

                        if (facade.SFTPDownload)
                        {
                            if (!facade.DownloadFiles())
                            {
                                _logDetail = "Cannot download files from SFTP";
                                goto Outer;
                            }
                        }

                        if (facade.ReadHRFile(out readFile, out msg, out hriEmpls))
                        {
                            if (facade.InsertHRTempTable(hriEmpls))
                            {
                                readRow = hriEmpls.Count;
                                readError = hriEmpls.Where(x => !string.IsNullOrWhiteSpace(x.Error)).Count();

                                if (facade.UpdateHREmployee(out empInsert, out empUpdate, out empMarkDelete
                                                            , out buInsert, out buUpdate, out buMarkDelete
                                                            , out brInsert, out brUpdate, out brMarkDelete, out msg))
                                {
                                    _logger.Info("I:--SUCCESS--:--Get GetFileHR--");
                                }
                                else
                                {
                                    _logger.Info($"I:--FAILED--:--Import from Interface Table--:{msg}");
                                    _logDetail = msg;
                                    goto Outer;
                                }
                            }
                            else
                            {
                                _logger.Info($"I:--FAILED--:--Insert Interface Table--:{msg}");
                                _logDetail = msg;
                                goto Outer;
                            }
                        }
                        else
                        {
                            _logDetail = msg;
                            _logger.Info($"O:--FAILED--:Read File--:{msg}");
                            goto Outer;
                        }

                        _stopwatch.Stop();
                        _elapsedTime = _stopwatch.ElapsedMilliseconds;
                        _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                        taskResponse = new ImportHRTaskResponse
                        {
                            SchedDateTime = schedDateTime,
                            ElapsedTime = _elapsedTime,
                            StatusResponse = new StatusResponse
                            {
                                Status = (readError > 0) ? Constants.StatusResponse.Failed : Constants.StatusResponse.Success
                            },
                            FileList = new List<object> { readFile },
                            ReadRow = readRow,
                            ReadErrorRow = readError,
                            EmpInsert = empInsert,
                            EmpUpdate = empUpdate,
                            EmpMarkDelete = empMarkDelete,
                            BUInsert = buInsert,
                            BUUpdate = buUpdate,
                            BUMarkDelete = buMarkDelete,
                            BRInsert = brInsert,
                            BRUpdate = brUpdate,
                            BRMarkDelete = brMarkDelete
                        };

                        facade.SaveLogSuccessOrFail(taskResponse);
                        return taskResponse;
                    }
                    catch (Exception ex)
                    {
                        _logDetail = ex.Message;
                        _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                        _logger.Error("Exception occur:\n", ex);
                    }

                    Outer:
                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;
                    _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                    taskResponse = new ImportHRTaskResponse
                    {
                        SchedDateTime = schedDateTime,
                        ElapsedTime = _elapsedTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = Constants.StatusResponse.Failed,
                            ErrorCode = string.Empty,
                            Description = _logDetail
                        },
                        FileList = new List<object>() { readFile }
                    };

                    facade.SaveLogError(taskResponse);
                    return taskResponse;
                }
                finally
                {
                    // Send mail to system administrator
                    //ImportBdwSendMail(taskResponse);

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

                        AppLog.BatchProcessEnd(Constants.BatchProcessCode.ImportHR, batchStatus, endTime, processTime, processDetail);
                    }

                    #endregion
                }
            }
        }

        public ImportCISTaskResponse GetFileCIS(string username, string password)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

            if (!string.IsNullOrWhiteSpace(username))
            {
                ThreadContext.Properties["UserID"] = username.ToUpperInvariant();
            }

            _cisFacade = new CisFacade();
            return _cisFacade.GetFileCIS(username, password, true, null);
        }

        public ImportHpTaskResponse GetFileHP(string username, string password)
        {
            ImportHpTaskResponse taskResponse = null;

            try
            {
                ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
                ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

                if (!string.IsNullOrWhiteSpace(username))
                {
                    ThreadContext.Properties["UserID"] = username.ToUpperInvariant();
                }

                string fiHpActivity = string.Empty;
                DateTime schedDateTime = DateTime.Now;
                _stopwatch = System.Diagnostics.Stopwatch.StartNew();
                _logger.Debug("-- Start Cron Job --:--Get GetFileHP--");

                #region "BatchProcess Start"

                if (AppLog.BatchProcessStart(Constants.BatchProcessCode.ImportHP, schedDateTime) == false)
                {
                    _logger.Info("I:--NOT PROCESS--:--GetFileHP--");

                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;
                    _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                    taskResponse = new ImportHpTaskResponse
                    {
                        SchedDateTime = schedDateTime,
                        ElapsedTime = _elapsedTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = Constants.StatusResponse.NotProcess,
                            ErrorCode = string.Empty,
                            Description = "",
                        },

                    };

                    return taskResponse;
                }

                #endregion

                if (!ApplicationHelpers.Authenticate(username, password))
                {
                    _logDetail = "Username and/or Password Invalid.";
                    _logger.InfoFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");
                    goto Outer;
                }

                int numOfTotal = 0;
                int numOfError = 0;
                int numOfComplete = 0;
                string messageError = "";

                try
                {
                    _logger.Info("I:--START--:--Get GetFileHP--");

                    #region "Hp Settings"

                    _hpFacade = new HpFacade();

                    string hpActivityFilePrefix = WebConfig.GetHpActivity();
                    string hpImportPath = _hpFacade.GetParameter(Constants.ParameterName.HPPathImport);
                    string hpExportPath = _hpFacade.GetParameter(Constants.ParameterName.HPPathError);

                    bool isValidFile = true;
                    string msgValidateFileError = "";

                    #endregion

                    _hpActivity = _hpFacade.ReadFileHpActivity(hpImportPath, hpActivityFilePrefix, ref numOfTotal,
                        ref fiHpActivity, ref isValidFile, ref msgValidateFileError);

                    if (_hpActivity == null && isValidFile == false)
                    {
                        _logDetail = msgValidateFileError;
                        _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                        goto Outer;
                    }
                    else if (_hpActivity != null && _hpActivity.Count == 0) // กรณีไม่มีข้อมูลในไฟล์
                    {
                        _stopwatch.Stop();
                        _elapsedTime = _stopwatch.ElapsedMilliseconds;
                        _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                        taskResponse = new ImportHpTaskResponse
                        {
                            SchedDateTime = schedDateTime,
                            ElapsedTime = _elapsedTime,
                            StatusResponse = new StatusResponse
                            {
                                Status = Constants.StatusResponse.Success
                            },
                            FileList = new List<object> { fiHpActivity },
                            NumOfTotal = 0,
                            NumOfComplete = 0,
                            NumOfError = 0
                        };

                        _hpFacade.SaveLogSuccessOrFail(taskResponse);
                        return taskResponse;
                    }

                    #region "pass basic validate "

                    if (_hpFacade.SaveHpActivity(_hpActivity, fiHpActivity))
                    {

                        if (_hpFacade.SaveHpActivityComplete(ref numOfComplete, ref numOfError, ref messageError))
                        {
                            _hpFacade.SaveServiceRequestActivity(); //  module Sr

                            if (numOfError > 0)
                            {
                                bool exportError = _hpFacade.ExportActivityHP(hpExportPath, fiHpActivity);
                                if (!exportError)
                                {
                                    _logDetail = "Failed to export HP Activity";
                                    goto Outer;
                                }
                            }

                            _logger.Info("I:--SUCCESS--:--Get GetFileHP--");
                        }
                        else
                        {
                            _logger.Info("I:--FAILED--:--Cannot save HP Activity --");
                            _logDetail = "Failed to save HP Activity";
                            goto Outer;
                        }
                    }
                    else
                    {
                        _logger.Info("I:--FAILED--:--Cannot save HP Activity --");
                        _logDetail = "Failed to save HP Activity";
                        goto Outer;
                    }

                    _stopwatch.Stop();
                    _elapsedTime = _stopwatch.ElapsedMilliseconds;
                    _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                    taskResponse = new ImportHpTaskResponse
                    {
                        SchedDateTime = schedDateTime,
                        ElapsedTime = _elapsedTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = (numOfError > 0) ? Constants.StatusResponse.Failed : Constants.StatusResponse.Success
                        },
                        FileList = new List<object> { fiHpActivity },
                        NumOfTotal = numOfTotal,
                        NumOfComplete = numOfComplete,
                        NumOfError = numOfError
                    };

                    _hpFacade.SaveLogSuccessOrFail(taskResponse);
                    return taskResponse;

                    #endregion

                }
                catch (Exception ex)
                {
                    _logDetail = ex.Message;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    _logger.Error("Exception occur:\n", ex);
                }

                Outer:
                _stopwatch.Stop();
                _elapsedTime = _stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                taskResponse = new ImportHpTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = _elapsedTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = _logDetail
                    },
                    FileList = new List<object>() { fiHpActivity }
                };

                _hpFacade.SaveLogError(taskResponse);
                return taskResponse;
            }
            finally
            {
                // Send mail to system administrator
                ImportHpSendMail(taskResponse);

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

                    AppLog.BatchProcessEnd(Constants.BatchProcessCode.ImportHP, batchStatus, endTime, processTime, processDetail);
                }

                #endregion
            }
        }

        #region "Functions"

        private string GetAfsFileFormat(string srcFile, string fiPrefix, DateTime dt)
        {
            string newFile = srcFile;

            if (string.IsNullOrWhiteSpace(newFile))
            {
                newFile = ApplicationHelpers.GetFileFormat(fiPrefix, dt, Constants.DateTimeFormat.ExportAfsDateTime);
            }

            return newFile;
        }

        private void GetSaleZoneInfo(PropertyEntity prop)
        {
            SaleZoneEntity saleZone = _saleZones.FirstOrDefault(x => x.District == prop.AssetLot && x.Province == prop.AssetPurch);
            if (saleZone != null)
            {
                prop.EmployeeId = saleZone.EmployeeId;
                prop.SaleName = saleZone.SaleName;
                prop.MobileNo = saleZone.MobileNo;
                prop.PhoneNo = saleZone.PhoneNo;
                prop.Email = saleZone.Email;
            }
        }

        private void ImportCisSendMail(ImportCISTaskResponse result)
        {
            try
            {
                _mailSender = CSMMailSender.GetCSMMailSender();

                if ((result.NumOfErrCor > 0) || (result.NumOfErrIndiv > 0) || (result.NumOfErrSub > 0) ||
                    (result.FileErrorList != null && result.FileErrorList.Count > 0))
                {
                    _mailSender.NotifyImportCISFailed(WebConfig.GetTaskEmailToAddress(), result);
                }
                if (Constants.StatusResponse.Failed.Equals(result.StatusResponse.Status))
                {
                    _mailSender.NotifyImportCISFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime,
                        result.StatusResponse.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
            }
        }

        private void ImportAfsSendMail(ImportAFSTaskResponse result)
        {
            try
            {
                _mailSender = CSMMailSender.GetCSMMailSender();

                if (result.NumOfErrProp > 0 || result.NumOfErrSaleZone > 0)
                {
                    _mailSender.NotifyImportAssetSuccess(WebConfig.GetTaskEmailToAddress(), result);
                }
                else if (Constants.StatusResponse.Failed.Equals(result.StatusResponse.Status))
                {
                    _mailSender.NotifyImportAssetFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime,
                        result.StatusResponse.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
            }
        }

        private void ExportAfsSendMail(ExportAFSTaskResponse result)
        {
            try
            {
                _mailSender = CSMMailSender.GetCSMMailSender();

                if (Constants.StatusResponse.Failed.Equals(result.StatusResponse.Status))
                {
                    _mailSender.NotifyExportActivityFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime, result.StatusResponse.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
            }
        }

        private void ExportNcbSendMail(ExportNCBTaskResponse result)
        {
            try
            {
                _mailSender = CSMMailSender.GetCSMMailSender();

                if (Constants.StatusResponse.Failed.Equals(result.StatusResponse.Status))
                {
                    _mailSender.NotifyFailExportActvity(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime, result.StatusResponse.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
            }
        }

        private void ImportBdwSendMail(ImportBDWTaskResponse result)
        {
            try
            {
                _mailSender = CSMMailSender.GetCSMMailSender();

                if (result.NumOfError > 0)
                {
                    _mailSender.NotifyImportContactSuccess(WebConfig.GetTaskEmailToAddress(), result);
                }
                else if (Constants.StatusResponse.Failed.Equals(result.StatusResponse.Status))
                {
                    _mailSender.NotifyImportContactFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime, result.StatusResponse.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
            }
        }

        private void ImportHpSendMail(ImportHpTaskResponse result)
        {
            try
            {
                _mailSender = CSMMailSender.GetCSMMailSender();

                if (result.NumOfError > 0)
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

                else if (Constants.StatusResponse.Failed.Equals(result.StatusResponse.Status))
                {
                    _mailSender.NotifyImportHPFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime, result.StatusResponse.Description);
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
                    if (_afsFacade != null) { _afsFacade.Dispose(); }
                    if (_hpFacade != null) { _hpFacade.Dispose(); }
                    if (_bdwFacade != null) { _bdwFacade.Dispose(); }
                    if (_cisFacade != null) { _cisFacade.Dispose(); }
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
