using System.IO;
using CSM.Common.Utilities;
using CSM.Data.DataAccess;
using CSM.Entity;
using CSM.Service.Messages.SchedTask;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Renci.SshNet;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CSM.Business
{
    public class AFSFacade : IAFSFacade
    {
        private AuditLogEntity _auditLog;
        private ICommonFacade _commonFacade;
        private readonly CSMContext _context;
        private IAFSDataAccess _afsDataAccess;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AFSFacade));

        public AFSFacade()
        {
            _context = new CSMContext();
        }

        public bool SaveAFSProperties(List<PropertyEntity> properties, string fiProp)
        {
            if (!string.IsNullOrWhiteSpace(fiProp))
            {
                _afsDataAccess = new AFSDataAccess(_context);
                return _afsDataAccess.SaveAFSProperty(properties);
            }

            return true;
        }

        public bool SaveSaleZones(List<SaleZoneEntity> saleZones, string fiSaleZone)
        {
            if (!string.IsNullOrWhiteSpace(fiSaleZone))
            {
                _afsDataAccess = new AFSDataAccess(_context);
                return _afsDataAccess.SaveSaleZone(saleZones);
            }

            return true;
        }

        public bool SaveCompleteProperties(ref int numOfComplete)
        {
            _afsDataAccess = new AFSDataAccess(_context);
            List<AfsAssetEntity> assetList = _afsDataAccess.GetCompleteProperties();
            return _afsDataAccess.SaveAFSAsset(assetList, ref numOfComplete);
        }

        public bool ExportErrorProperties(string filePath, string fileName, ref int numOfErrProp)
        {
            _afsDataAccess = new AFSDataAccess(_context);
            var assetList = _afsDataAccess.GetErrorProperties();
            if (assetList != null && assetList.Count > 0)
            {
                numOfErrProp = assetList.Count;
                return ExportProperties(filePath, fileName, assetList);
            }

            return true;
        }

        public bool ExportErrorSaleZones(string filePath, string fileName, ref int numOfErrSaleZone)
        {
            _afsDataAccess = new AFSDataAccess(_context);
            var saleZones = _afsDataAccess.GetErrorSaleZones();
            if (saleZones != null && saleZones.Count > 0)
            {
                numOfErrSaleZone = saleZones.Count;
                return ExportSaleZones(filePath, fileName, saleZones);
            }

            return true;
        }

        public void SaveLogSuccessOrFail(ImportAFSTaskResponse taskResponse)
        {
            if (taskResponse != null)
            {
                StringBuilder sb = new StringBuilder("");
                sb.AppendFormat("วัน เวลาที่ run task scheduler = {0}\n",
                    taskResponse.SchedDateTime.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime));
                sb.AppendFormat("ElapsedTime = {0} (ms)\n", taskResponse.ElapsedTime);
                sb.Append(taskResponse.ToString());

                _auditLog = new AuditLogEntity();
                _auditLog.Module = Constants.Module.Batch;
                _auditLog.Action = Constants.AuditAction.ImportAFS;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.Status = (taskResponse.StatusResponse.Status == Constants.StatusResponse.Failed) ? LogStatus.Fail : LogStatus.Success;
                _auditLog.Detail = sb.ToString();
                AppLog.AuditLog(_auditLog);
            }
        }

        public void SaveLogError(ImportAFSTaskResponse taskResponse)
        {
            if (taskResponse != null)
            {
                StringBuilder sb = new StringBuilder("");
                sb.AppendFormat("วัน เวลาที่ run task scheduler = {0}\n",
                    taskResponse.SchedDateTime.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime));
                sb.AppendFormat("ElapsedTime = {0} (ms)\n", taskResponse.ElapsedTime);
                sb.AppendFormat("Error Message = {0}\n", taskResponse.StatusResponse.Description);

                _auditLog = new AuditLogEntity();
                _auditLog.Module = Constants.Module.Batch;
                _auditLog.Action = Constants.AuditAction.ImportAFS;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.Status = LogStatus.Fail;
                _auditLog.Detail = sb.ToString();
                _auditLog.CreateUserId = null;
                AppLog.AuditLog(_auditLog);
            }
        }

        public string GetParameter(string paramName)
        {
            _commonFacade = new CommonFacade();
            ParameterEntity param = _commonFacade.GetCacheParamByName(paramName);
            return param != null ? param.ParamValue : string.Empty;
        }

        public List<PropertyEntity> ReadFileProperty(string filePath, string fiPrefix, ref int numOfProp, ref string fiProp, ref bool isValidFile, ref string msgValidateFileError, bool isValidFileSaleZone)
        {
            try
            {
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiProp = Path.GetFileName(files.First());

                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"

                    bool isValidFormat = false;

                    int inx = 1;
                    var lstLengthNotMatch = new List<string>();
                    foreach (var source in results.Skip(1))
                    {
                        if (source.Length != Constants.ImportAfs.LengthOfProperty)
                        {
                            lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                        }

                        inx++;
                    }

                    if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                    {
                        Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiProp, string.Join(",", lstLengthNotMatch.ToArray()));
                    }
                    else if (lstLengthNotMatch.Count > 0)
                    {
                        Logger.DebugFormat("File:{0} Invalid format {1} records", fiProp,
                            lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                    }

                    isValidFormat = (lstLengthNotMatch.Count == 0);

                    if (isValidFormat == false)
                    {
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiProp);
                    }


                    if (isValidFormat == false)
                    {
                        // Move File
                        string pathError = this.GetParameter(Constants.ParameterName.AFSPathError);
                        StreamDataHelpers.TryToCopy(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiProp), string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", pathError, fiProp));

                        isValidFile = false; // ref value
                        goto Outer;
                    }

                    #endregion

                    List<PropertyEntity> properties = (from source in results.Skip(1)
                                                       select new PropertyEntity
                                                       {
                                                           RowId = Convert.ToInt32(source[0], CultureInfo.InvariantCulture),
                                                           IfRowStat = source[1].NullSafeTrim(),
                                                           IfRowBatchNum = source[2].NullSafeTrim(),
                                                           AssetNum = source[3].NullSafeTrim(),
                                                           AssetType = source[4].NullSafeTrim(),
                                                           AssetTradeInType = source[5].NullSafeTrim(),
                                                           AssetStatus = source[6].NullSafeTrim(),
                                                           AssetDesc = source[7].NullSafeTrim(),
                                                           AssetName = source[8].NullSafeTrim(),
                                                           AssetComments = source[9].NullSafeTrim(),
                                                           AssetRefNo1 = source[10].NullSafeTrim(),
                                                           AssetLot = source[11].NullSafeTrim(),
                                                           AssetPurch = source[12].NullSafeTrim()
                                                       }).ToList();


                    #region "Validate MaxLength"

                    ValidationContext vc = null;
                    int inxErr = 2;
                    var lstErrMaxLength = new List<string>();
                    foreach (PropertyEntity obj in properties)
                    {
                        vc = new ValidationContext(obj, null, null);
                        var validationResults = new List<ValidationResult>();
                        bool valid = Validator.TryValidateObject(obj, vc, validationResults, true);
                        if (!valid)
                        {
                            obj.Error =
                                validationResults.Select(x => x.ErrorMessage)
                                    .Aggregate((i, j) => i + Environment.NewLine + j);

                            lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}({1})", inxErr.ToString(CultureInfo.InvariantCulture), obj.Error));
                        }

                        inxErr++;
                    }

                    if (lstErrMaxLength.Count > 0)
                    {
                        Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiProp, string.Join(",", lstErrMaxLength.ToArray()));
                    }

                    if (properties.Count(x => !string.IsNullOrWhiteSpace(x.Error)) > 0)
                    {
                        // Move File
                        string pathError = this.GetParameter(Constants.ParameterName.AFSPathError);
                        StreamDataHelpers.TryToCopy(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiProp), string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", pathError, fiProp));

                        //Logger.DebugFormat("File:{0} Invalid MaxLength", fiProp);
                        isValidFile = false; // ref value
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid maxLength.", fiProp);
                        goto Outer;
                    }

                    #endregion

                    if (isValidFileSaleZone == false && isValidFile == true)
                    {
                        #region "Move File to pathError"

                        // Move File
                        string pathError = this.GetParameter(Constants.ParameterName.AFSPathError);
                        StreamDataHelpers.TryToCopy(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiProp), string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", pathError, fiProp));
                        goto Outer;

                        #endregion
                    }

                    numOfProp = properties.Count;
                    return properties;
                }
                //else
                //{
                //    isValidFile = false;
                //    msgValidateFileError = "File not found.";
                //}

                fiProp = string.Empty;
                return _afsDataAccess.GetProperties();

            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw;
            }
            finally
            {
                #region "Move file to PathSource"

                if (!string.IsNullOrEmpty(fiProp))
                {
                    string afsPathSource = this.GetParameter(Constants.ParameterName.AfsPathSource);
                    StreamDataHelpers.TryToCopy(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiProp), string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", afsPathSource, fiProp));
                    StreamDataHelpers.TryToDelete(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiProp));
                }

                #endregion
            }
        }

        public List<SaleZoneEntity> ReadFileSaleZone(string filePath, string fiPrefix, ref int numOfSaleZones, ref string fiSaleZone, ref bool isValidFile, ref string msgValidateFileError)
        {
            try
            {
                _afsDataAccess = new AFSDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiSaleZone = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"

                    bool isValidFormat = false;

                    int inx = 1;
                    var lstLengthNotMatch = new List<string>();
                    foreach (var source in results.Skip(1))
                    {
                        if (source.Length != Constants.ImportAfs.LengthOfSaleZone)
                        {
                            lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                        }

                        inx++;
                    }

                    if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                    {
                        Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiSaleZone, string.Join(",", lstLengthNotMatch.ToArray()));
                    }
                    else if (lstLengthNotMatch.Count > 0)
                    {
                        Logger.DebugFormat("File:{0} Invalid format {1} records", fiSaleZone,
                            lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                    }

                    isValidFormat = (lstLengthNotMatch.Count == 0);

                    if (isValidFormat == false)
                    {
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiSaleZone);
                    }


                    if (isValidFormat == false)
                    {
                        // Move File
                        string pathError = this.GetParameter(Constants.ParameterName.AFSPathError);
                        StreamDataHelpers.TryToCopy(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiSaleZone), string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", pathError, fiSaleZone));

                        isValidFile = false; // ref value
                        goto Outer;
                    }

                    #endregion


                    List<SaleZoneEntity> saleZones = (from source in results
                                                      select new SaleZoneEntity
                                                      {
                                                          District = source[0].NullSafeTrim(),
                                                          Province = source[1].NullSafeTrim(),
                                                          SaleName = source[2].NullSafeTrim(),
                                                          PhoneNo = source[3].NullSafeTrim(),
                                                          EmployeeNo = source[4].NullSafeTrim(),
                                                          MobileNo = source[5].NullSafeTrim(),
                                                          Email = source[6].NullSafeTrim(),
                                                          EmployeeId = _afsDataAccess.GetUserIdByEmployeeCode(source[4].NullSafeTrim())
                                                      }).ToList();

                    #region "Validate MaxLength"

                    ValidationContext vc = null;
                    int inxErr = 2;
                    var lstErrMaxLength = new List<string>();
                    foreach (SaleZoneEntity obj in saleZones)
                    {
                        vc = new ValidationContext(obj, null, null);
                        var validationResults = new List<ValidationResult>();
                        bool valid = Validator.TryValidateObject(obj, vc, validationResults, true);
                        if (!valid)
                        {
                            obj.Error =
                                validationResults.Select(x => x.ErrorMessage)
                                    .Aggregate((i, j) => i + Environment.NewLine + j);

                            lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                        }

                        inxErr++;
                    }

                    if (lstErrMaxLength.Count > 0)
                    {
                        Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiSaleZone, string.Join(",", lstErrMaxLength.ToArray()));
                    }

                    if (saleZones.Count(x => !string.IsNullOrWhiteSpace(x.Error)) > 0)
                    {
                        // Move File
                        string pathError = this.GetParameter(Constants.ParameterName.AFSPathError);
                        StreamDataHelpers.TryToCopy(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiSaleZone), string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", pathError, fiSaleZone));

                        //Logger.DebugFormat("File:{0} Invalid MaxLength", fiSaleZone);
                        isValidFile = false; // ref value
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid maxLength.", fiSaleZone);
                        goto Outer;
                    }

                    #endregion

                    numOfSaleZones = saleZones.Count;
                    return saleZones;
                }

                fiSaleZone = string.Empty;
                return _afsDataAccess.GetSaleZones();

            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw;
            }
            finally
            {
                #region "Move file to PathSource"

                if (!string.IsNullOrEmpty(fiSaleZone))
                {
                    string afsPathSource = this.GetParameter(Constants.ParameterName.AfsPathSource);
                    StreamDataHelpers.TryToCopy(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiSaleZone), string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", afsPathSource, fiSaleZone));
                    StreamDataHelpers.TryToDelete(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiSaleZone));
                }

                #endregion
            }
        }

        private static bool ExportSaleZones(string filePath, string fileName, List<SaleZoneEntity> saleZones)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add("AMPHUR", typeof(string));
                dt.Columns.Add("PROVINCE", typeof(string));
                dt.Columns.Add("SALE_NAME", typeof(string));
                dt.Columns.Add("PHONE_NO", typeof(string));
                dt.Columns.Add("EMPLOYEE_NO", typeof(string));
                dt.Columns.Add("MOBILE_NO", typeof(string));
                dt.Columns.Add("EMAIL", typeof(string));
                dt.Columns.Add("ERROR", typeof(string));

                var result = from x in saleZones
                             select dt.LoadDataRow(new object[]
                             {
                                 x.District, 
                                 x.Province, 
                                 x.SaleName, 
                                 x.PhoneNo, 
                                 x.EmployeeNo,
                                 x.MobileNo, 
                                 x.Email,
                                 x.Error
                             }, false);

                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fileName);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    foreach (DataRow row in result)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sw.WriteLine(string.Join("|", fields));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        private static bool ExportProperties(string filePath, string fileName, List<PropertyEntity> properties)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add("ROW_ID", typeof(string));
                dt.Columns.Add("IF_ROW_STAT", typeof(string));
                dt.Columns.Add("IF_ROW_BATCH_NUM", typeof(string));
                dt.Columns.Add("AST_ASSET_NUM", typeof(string));
                dt.Columns.Add("AST_TYPE_CD", typeof(string));
                dt.Columns.Add("AST_TRADEINTYPE_CD", typeof(string));
                dt.Columns.Add("AST_STATUS_CD", typeof(string));
                dt.Columns.Add("AST_DESC_TEXT", typeof(string));
                dt.Columns.Add("AST_NAME", typeof(string));
                dt.Columns.Add("AST_COMMENTS", typeof(string));
                dt.Columns.Add("AST_REF_NUMBER_1", typeof(string));
                dt.Columns.Add("AST_LOT_NUM", typeof(string));
                dt.Columns.Add("AST_PURCH_LOC_DESC", typeof(string));
                dt.Columns.Add("ERROR", typeof(string));

                var result = from x in properties
                             select dt.LoadDataRow(new object[] { 
                                    x.RowId, 
                                    x.IfRowStat, 
                                    x.IfRowBatchNum, 
                                    x.AssetNum, 
                                    x.AssetType, 
                                    x.AssetTradeInType, 
                                    x.AssetStatus, 
                                    x.AssetDesc, 
                                    x.AssetName, 
                                    x.AssetComments, 
                                    x.AssetRefNo1, 
                                    x.AssetLot, 
                                    x.AssetPurch,
                                    x.Error 
                                }, false);

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fileName);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames));

                    foreach (DataRow row in result)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sw.WriteLine(string.Join("|", fields));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        #region "Export Activity AFS"

        public bool ExportActivityAFS(string filePath, string fileName, ref int numOfActivity)
        {
            _afsDataAccess = new AFSDataAccess(_context);
            var activityList = _afsDataAccess.GetAFSExport();
            numOfActivity = activityList.Count();
            if (numOfActivity > 0)
            {
                if (ExportActivityAFS(filePath, fileName, activityList))
                {
                    // Update ExportDate
                    //return _afsDataAccess.UpdateAFSExportWithExportDate();

                    var sr_id_list = activityList.Select(x => x.SR_ID).ToList();
                    return _afsDataAccess.UpdateAFSExportWithExportDate(sr_id_list);
                }

                return false;
            }

            return true;
        }

        private static bool ExportActivityAFS(string filePath, string fileName, List<AfsexportEntity> afsexport)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add(string.Empty, typeof(string));
                dt.Columns.Add(string.Empty, typeof(string));
                dt.Columns.Add(string.Empty, typeof(string));
                dt.Columns.Add(string.Empty, typeof(string));
                dt.Columns.Add(string.Empty, typeof(string));
                dt.Columns.Add(string.Empty, typeof(string));
                dt.Columns.Add(string.Empty, typeof(string));
                dt.Columns.Add(string.Empty, typeof(string));
                dt.Columns.Add(string.Empty, typeof(string));
                dt.Columns.Add(string.Empty, typeof(string));
                dt.Columns.Add(string.Empty, typeof(string));
                dt.Columns.Add(string.Empty, typeof(string));
                dt.Columns.Add(string.Empty, typeof(string));
                dt.Columns.Add(string.Empty, typeof(string));
                dt.Columns.Add(string.Empty, typeof(string));
                dt.Columns.Add(string.Empty, typeof(string));
                dt.Columns.Add(string.Empty, typeof(string));

                var result = from x in afsexport
                             select dt.LoadDataRow(new object[]
                             {
                                x.CreatedDateDisplay,
                                x.CreatedBy,
                                x.Type,
                                x.Question,
                                PrepareDescription(x.Description),
                                x.PhoneNo,
                                x.ContactFirstName,
                                x.ContactLastName,
                                x.Property,
                                x.AssetInspection,
                                x.CallBackRequest, 
                                x.DocumentRequest, 
                                x.LocationEnquiry,
                                x.PriceEnquiry,
                                x.PriceIssuedCallBack,
                                x.CallBackRequired,
                                x.MediaSource
                             }, false);

                string dateStr = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.ExportAfsDateTime);
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}{2}.txt", filePath, fileName, dateStr);

                //StreamDataHelpers.TryToCopy(HostingEnvironment.MapPath(@"~/Templates/ExportFile/ActivityAFS.txt"), targetFile);
                var enCoding = Encoding.GetEncoding("windows-874");
                using (var sw = new StreamWriter(targetFile, false, enCoding))
                {
                    foreach (DataRow row in result)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sw.WriteLine(string.Join("|", fields));
                    }

                    //sw.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        private static string PrepareDescription(string description)
        {
            if (!string.IsNullOrWhiteSpace(description))
            {
                description = description.Replace(Environment.NewLine, "");
                description = ApplicationHelpers.RemoveAllHtmlTags(description);
            }

            return description;
        }

        public void SaveLogExportSuccess(ExportAFSTaskResponse taskResponse)
        {
            if (taskResponse != null)
            {
                StringBuilder sb = new StringBuilder("");
                sb.Append("Success to export activity\n");
                sb.Append(taskResponse.ToString());

                _auditLog = new AuditLogEntity();
                _auditLog.Module = Constants.Module.Batch;
                _auditLog.Action = Constants.AuditAction.Export;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.Status = LogStatus.Success;
                _auditLog.Detail = sb.ToString();
                _auditLog.CreateUserId = null;
                _auditLog.CreatedDate = taskResponse.SchedDateTime;
                AppLog.AuditLog(_auditLog);
            }
        }

        public void SaveLogExportError(ExportAFSTaskResponse taskResponse)
        {
            if (taskResponse != null)
            {
                StringBuilder sb = new StringBuilder("");
                sb.AppendFormat("Error Message = {0}\n", taskResponse.StatusResponse.Description);

                _auditLog = new AuditLogEntity();
                _auditLog.Module = Constants.Module.Batch;
                _auditLog.Action = Constants.AuditAction.Export;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.Status = LogStatus.Fail;
                _auditLog.Detail = sb.ToString();
                _auditLog.CreateUserId = null;
                _auditLog.CreatedDate = taskResponse.SchedDateTime;
                AppLog.AuditLog(_auditLog);
            }
        }

        #endregion

        #region "Export Marketing AFS"

        public void ExportEmployeeNCB(string filePath, string fileNameNew, string fileNameUpdate, ref int numOfNew, ref int numOfUpdate, ref string newEmplFile, ref string updateEmplFile, ref bool exportEmNew, ref bool exportEmUpdate)
        {
            _afsDataAccess = new AFSDataAccess(_context);
            var marketingList = _afsDataAccess.GetAFSMarketing();

            var new_marketingList = marketingList.Where(p => p.IsNew == true).OrderBy(p => p.EmpNum).ThenBy(p => p.PhoneOrder).ToList();
            var update_marketingList = marketingList.Where(p => p.IsNew == false).OrderBy(p => p.EmpNum).ThenBy(p => p.PhoneOrder).ToList();

            numOfNew = new_marketingList.Count;
            numOfUpdate = update_marketingList.Count;

            //New
            if (numOfNew > 0)
            {
                if (ExportMarketingAFS(filePath, fileNameNew, new_marketingList, ref newEmplFile))
                {
                    var userIds = new_marketingList.Select(p => p.UserID).Distinct().ToList();
                    exportEmNew = _afsDataAccess.UpdateExportDate(userIds);
                }
                else
                {
                    exportEmNew = false;
                }
            }
            else
            {
                exportEmNew = true;
            }

            //Update
            if (numOfUpdate > 0)
            {
                if (ExportMarketingAFS(filePath, fileNameUpdate, update_marketingList, ref updateEmplFile))
                {
                    var userIds = update_marketingList.Select(p => p.UserID).Distinct().ToList();
                    exportEmUpdate = _afsDataAccess.UpdateExportDate(userIds);
                }
                else
                {
                    exportEmUpdate = false;
                }
            }
            else
            {
                exportEmUpdate = true;
            }
        }

        public bool ExportEmployeeNCBNew(string filePath, string fileName, ref int numOfNew, ref string newEmplFile)
        {
            _afsDataAccess = new AFSDataAccess(_context);
            var marketingList = _afsDataAccess.GetAFSMarketingNew();
            numOfNew = marketingList.Count();

            if (numOfNew > 0)
            {
                if (ExportMarketingAFS(filePath, fileName, marketingList, ref newEmplFile))
                {
                    bool isUpdate = false;
                    return _afsDataAccess.SaveExportDate(isUpdate);
                }

                return false;
            }

            return true;
        }

        public bool ExportEmplyeeNCBUpdate(string filePath, string fileName, ref int numOfUpdate, ref string updateEmplFile)
        {
            _afsDataAccess = new AFSDataAccess(_context);
            var marketingList = _afsDataAccess.GetAFSMarketingUpdate();
            numOfUpdate = marketingList.Count();

            if (numOfUpdate > 0)
            {
                if (ExportMarketingAFS(filePath, fileName, marketingList, ref updateEmplFile))
                {
                    bool isUpdate = true;
                    return _afsDataAccess.SaveExportDate(isUpdate);
                }

                return false;
            }

            return true;
        }

        private static bool ExportMarketingAFS(string filePath, string fileName, List<AfsMarketingEntity> afsexport, ref string emplFile)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add("EMP_NUM", typeof(string));
                dt.Columns.Add("FST_NAME", typeof(string));
                dt.Columns.Add("LAST_NAME", typeof(string));
                dt.Columns.Add("PHONE_NO", typeof(string));
                dt.Columns.Add("CREATE_DATE", typeof(string));
                dt.Columns.Add("UPDATE_DATE", typeof(string));
                dt.Columns.Add("PHONE_ORDER", typeof(string));
                dt.Columns.Add("EMP_STATUS", typeof(string));

                var result = from x in afsexport
                             select dt.LoadDataRow(new object[]
                             {
                                x.EmpNum,
                                x.FstName,
                                x.LastName,
                                x.PhoneNo,
                                x.CreatedDateDisplay,
                                x.UpdateDateDisplay,
                                x.PhoneOrder,
                                x.EmpStatus                               
                             }, false);

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                //const string dateStr = "20151126";
                string dateStr = DateTime.Now.FormatDateTime("yyyyMMdd");
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}_{2}.txt", filePath, fileName, dateStr);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames));

                    foreach (DataRow row in result)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sw.WriteLine(string.Join("|", fields));
                    }

                    //sw.Close();
                }

                emplFile = targetFile;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        public void SaveLogExportMarketingSuccess(ExportNCBTaskResponse taskResponse)
        {
            if (taskResponse != null)
            {
                StringBuilder sb = new StringBuilder("");
                sb.Append("Success to export marketing\n");
                sb.Append(taskResponse.ToString());

                _auditLog = new AuditLogEntity();
                _auditLog.Module = Constants.Module.Batch;
                _auditLog.Action = Constants.AuditAction.ExportMarketing;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.Status = LogStatus.Success;
                _auditLog.Detail = sb.ToString();
                _auditLog.CreateUserId = null;
                _auditLog.CreatedDate = taskResponse.SchedDateTime;
                AppLog.AuditLog(_auditLog);
            }
        }

        public void SaveLogExportMarketingError(ExportNCBTaskResponse taskResponse)
        {
            if (taskResponse != null)
            {
                StringBuilder sb = new StringBuilder("");
                sb.AppendFormat("Error Message = {0}\n", taskResponse.StatusResponse.Description);

                _auditLog = new AuditLogEntity();
                _auditLog.Module = Constants.Module.Batch;
                _auditLog.Action = Constants.AuditAction.ExportMarketing;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.Status = LogStatus.Fail;
                _auditLog.Detail = sb.ToString();
                _auditLog.CreateUserId = null;
                _auditLog.CreatedDate = taskResponse.SchedDateTime;
                AppLog.AuditLog(_auditLog);
            }
        }

        #endregion

        public bool UploadFilesViaFTP(string newEmplFile, string updateEmplFile)
        {
            try
            {
                string host = WebConfig.GetIVRSshServer();
                int port = WebConfig.GetIVRSshPort();
                string username = WebConfig.GetIVRSshUsername();
                string password = WebConfig.GetIVRSshPassword();
                string insertRemoteDir = WebConfig.GetIVRSshInsertRemoteDir(); // . always refers to the current directory.
                string updateRemoteDir = WebConfig.GetIVRSshUpdateRemoteDir(); // . always refers to the current directory.

                using (var sftp = new SftpClient(host, port, username, password))
                {
                    sftp.Connect();

                    #region "New Employee NCB"

                    if (!string.IsNullOrWhiteSpace(newEmplFile))
                    {
                        if (File.Exists(newEmplFile))
                        {
                            // Upload file to local via SFTP
                            UploadFile(sftp, insertRemoteDir, newEmplFile);
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Upload Files Via FTP").Add("FilePath", newEmplFile).ToSuccessLogString());
                        }
                        else
                        {
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Upload Files Via FTP").Add("FilePath", newEmplFile)
                                .Add("Error Message", "File Not Found").ToFailLogString());
                            return false;
                        }
                    }

                    #endregion

                    #region "Update Employee NCB"

                    if (!string.IsNullOrWhiteSpace(updateEmplFile))
                    {
                        if (File.Exists(updateEmplFile))
                        {
                            // Upload file to local via SFTP
                            UploadFile(sftp, updateRemoteDir, updateEmplFile);
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Upload Files Via FTP").Add("FilePath", updateEmplFile).ToSuccessLogString());
                        }
                        else
                        {
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Upload Files Via FTP").Add("FilePath", updateEmplFile)
                                .Add("Error Message", "File Not Found").ToFailLogString());
                            return false;
                        }
                    }

                    #endregion

                    sftp.Disconnect();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Upload Files Via FTP").Add("Error Message", ex.Message).ToInputLogString());
            }
            /*finally
            {
                string cicPathSource = this.GetParameter(Constants.ParameterName.CICPathSource);
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    StreamDataHelpers.TryToCopy(file, string.Format("{0}\\{1}", cicPathSource, fileName));
                    StreamDataHelpers.TryToDelete(file);
                }
            }*/

            return false;
        }

        #region "Functions"

        private void UploadFile(SftpClient client, string remoteDirectory, string uploadFile)
        {
            try
            {
                string fileName;

                using (var fs = new FileStream(uploadFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fileName = Path.GetFileName(fs.Name);
                    string uploadPath = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", remoteDirectory, fileName);
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Upload File").Add("FileName", fileName).ToInputLogString());
                    client.BufferSize = 4 * 1024;
                    client.UploadFile(fs, uploadPath, null);
                    //fs.Close();
                }

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Upload File").Add("FileName", fileName).ToSuccessLogString());
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Upload File").Add("Error Message", ex.Message).ToFailLogString());
                throw;
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
                    if (_context != null) { _context.Dispose(); }
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
