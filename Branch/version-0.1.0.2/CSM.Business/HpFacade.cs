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
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CSM.Business
{
    public class HpFacade : IHpFacade
    {
        private AuditLogEntity _auditLog;
        private ICommonFacade _commonFacade;
        private IHpDataAccess _hpDataAccess;
        private readonly CSMContext _context;
        private IServiceRequestFacade _srFacade;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(HpFacade));

        public HpFacade()
        {
            _context = new CSMContext();
        }
        public string GetParameter(string paramName)
        {
            _commonFacade = new CommonFacade();
            ParameterEntity param = _commonFacade.GetCacheParamByName(paramName);
            return param != null ? param.ParamValue : string.Empty;
        }

        public List<HpActivityEntity> ReadFileHpActivity(string filePath, string fiPrefix, ref int numOfActivity, ref string fiActivity, ref bool isValidFile, ref string msgValidateFileError)
        {
            try
            {
                _hpDataAccess = new HpDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiActivity = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadCsv(files.First());

                    #region "Validate file format"

                    bool isValidFormat = false;

                    int inx = 1;
                    List<string> lstLengthNotMatch = new List<string>();
                    foreach (var source in results)
                    {
                        if (source.Length != Constants.ImportHp.LengthOfDetail)
                        {
                            lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                        }

                        inx++;
                    }

                    if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                    {
                        Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiActivity, string.Join(",", lstLengthNotMatch.ToArray()));
                    }

                    isValidFormat = (lstLengthNotMatch.Count == 0);

                    if (isValidFormat == false)
                    {
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiActivity);
                    }


                    if (isValidFormat == false)
                    {
                        // Move File
                        string pathError = this.GetParameter(Constants.ParameterName.HPPathError);
                        StreamDataHelpers.TryToCopy(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiActivity), string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", pathError, fiActivity));

                        isValidFile = false; // ref value
                        goto Outer;
                    }

                    #endregion

                    List<HpActivityEntity> hpActivity = (from source in results
                                                         select new HpActivityEntity
                                                         {
                                                             Channel = source[0].ToString(),
                                                             Type = source[1].ToString(),
                                                             Area = source[2].ToString(),
                                                             Status = source[3].ToString(),
                                                             Description = source[4].ToString(),
                                                             Comment = source[5].ToString(),
                                                             AssetInfo = source[6].ToString(),
                                                             ContactInfo = source[7].ToString(),
                                                             Ano = source[8].ToString(),
                                                             CallId = source[9].ToString(),
                                                             ContactName = source[10].ToString(),
                                                             ContactLastName = source[11].ToString(),
                                                             ContactPhone = source[12].ToString(),
                                                             DoneFlg = source[13].ToString(),
                                                             CreateDate = source[14].ToString(),
                                                             CreateBy = source[15].ToString(),
                                                             StartDate = source[16].ToString(),
                                                             EndDate = source[17].ToString(),
                                                             OwnerLogin = source[18].ToString(),
                                                             OwnerPerId = source[19].ToString(),
                                                             UpdateDate = source[20].ToString(),
                                                             UpdateBy = source[21].ToString(),
                                                             SrNo = source[22].ToString(),
                                                             CallFlg = source[23].ToString(),
                                                             EnqFlg = source[24].ToString(),
                                                             LocEnqFlg = source[25].ToString(),
                                                             DocReqFlg = source[26].ToString(),
                                                             PriIssuedFlg = source[27].ToString(),
                                                             AssetInspectFlg = source[28].ToString(),
                                                             PlanstartDate = source[29].ToString(),
                                                             ContactFax = source[30].ToString(),
                                                             ContactEmail = source[31].ToString()
                                                         }).ToList();


                    #region "Validate MaxLength"

                    ValidationContext vc = null;
                    int inxErr = 1;
                    List<string> lstErrMaxLength = new List<string>();
                    foreach (HpActivityEntity hp in hpActivity)
                    {
                        vc = new ValidationContext(hp, null, null);
                        var validationResults = new List<ValidationResult>();
                        bool valid = Validator.TryValidateObject(hp, vc, validationResults, true);
                        if (!valid)
                        {
                            hp.Error =
                                validationResults.Select(x => x.ErrorMessage)
                                    .Aggregate((i, j) => i + Environment.NewLine + j);

                            lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                        }

                        inxErr++;
                    }

                    if (lstErrMaxLength.Count > 0)
                    {
                        Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiActivity, string.Join(",", lstErrMaxLength.ToArray()));
                    }

                    if (hpActivity.Count(x => !string.IsNullOrWhiteSpace(x.Error)) > 0)
                    {
                        // Move File
                        string pathError = this.GetParameter(Constants.ParameterName.HPPathError);
                        StreamDataHelpers.TryToCopy(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiActivity), string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", pathError, fiActivity));

                        Logger.DebugFormat("File:{0} Invalid MaxLength", fiActivity);
                        isValidFile = false; // ref value
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiActivity);
                        goto Outer;
                    }

                    #endregion

                    numOfActivity = hpActivity.Count;
                    return hpActivity;
                }
                else
                {
                    isValidFile = false;
                    msgValidateFileError = "File not found.";
                }

            Outer:

                return null;
            }
            finally
            {
                #region "Move file to PathSource"

                if (!string.IsNullOrEmpty(fiActivity))
                {
                    string hpPathSource = this.GetParameter(Constants.ParameterName.HpPathSource);
                    StreamDataHelpers.TryToCopy(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiActivity), string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", hpPathSource, fiActivity));
                    StreamDataHelpers.TryToDelete(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiActivity));
                }

                #endregion
            }
        }

        public bool SaveHpActivity(List<HpActivityEntity> hpActivity, string fiActivity)
        {
            if (!string.IsNullOrWhiteSpace(fiActivity))
            {
                _hpDataAccess = new HpDataAccess(_context);
                return _hpDataAccess.SaveHpActivity(hpActivity);
            }
            return true;
        }
        public bool SaveHpActivityComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _hpDataAccess = new HpDataAccess(_context);
            return _hpDataAccess.SaveHpActivityComplete(ref numOfComplete, ref numOfError, ref messageError);
        }
        public bool ExportActivityHP(string filePath, string fileName)
        {
            _hpDataAccess = new HpDataAccess(_context);
            var activityList = _hpDataAccess.GetHpActivityExport();
            return ExportActivityHP(filePath, fileName, activityList);
        }

        public bool SaveServiceRequestActivity()
        {
            _hpDataAccess = new HpDataAccess(_context);
            var lstSrActivity = _hpDataAccess.GetSrWithHpActivity();
            if (lstSrActivity != null && lstSrActivity.Count > 0)
            {
                _srFacade = new ServiceRequestFacade();
                foreach (var sr in lstSrActivity)
                {
                    // TODO :: Add new parameter: AuditLogEntity for Call CAR Web Service
                    _srFacade.CreateServiceRequestActivity(null, sr, true);
                    //if (result.IsSuccess == false)
                    //{
                    //    Logger.Debug(string.Format("CreateServiceRequestActivity SrId:{0} ErrorMessage:{1} WarningMessages:{2} "
                    //        , sr.SrId.Value.ToString(), result.ErrorMessage, result.WarningMessages));
                    //}
                }
            }

            return true;
        }

        private static bool ExportActivityHP(string filePath, string fileName, List<HpActivityEntity> activityexport)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add("CHANNEL", typeof(string));
                dt.Columns.Add("TYPE", typeof(string));
                dt.Columns.Add("AREA", typeof(string));
                dt.Columns.Add("STATUS", typeof(string));
                dt.Columns.Add("DESCRIPTION", typeof(string));
                dt.Columns.Add("COMMENT", typeof(string));
                dt.Columns.Add("ASSET_INFO", typeof(string));
                dt.Columns.Add("CONTACT_INFO", typeof(string));
                dt.Columns.Add("A_NO", typeof(string));
                dt.Columns.Add("CALL_ID", typeof(string));
                dt.Columns.Add("CONTACT_NAME", typeof(string));
                dt.Columns.Add("CONTACT_LAST_NAME", typeof(string));
                dt.Columns.Add("CONTACT_PHONE", typeof(string));
                dt.Columns.Add("DONE_FLG", typeof(string));
                dt.Columns.Add("CREATE_DATE", typeof(string));
                dt.Columns.Add("CREATE_BY", typeof(string));
                dt.Columns.Add("START_DATE", typeof(string));
                dt.Columns.Add("END_DATE", typeof(string));
                dt.Columns.Add("OWNER_LOGIN", typeof(string));
                dt.Columns.Add("OWNER_PER_ID", typeof(string));
                dt.Columns.Add("UPDATE_DATE", typeof(string));
                dt.Columns.Add("UPDATE_BY", typeof(string));
                dt.Columns.Add("SR_ID", typeof(string));
                dt.Columns.Add("CALL_FLG", typeof(string));
                dt.Columns.Add("ENQ_FLG", typeof(string));
                dt.Columns.Add("LOC_ENQ_FLG", typeof(string));
                dt.Columns.Add("DOC_REQ_FLG", typeof(string));
                dt.Columns.Add("PRI_ISSUED_FLG", typeof(string));
                dt.Columns.Add("ASSET_INSPECT_FLG", typeof(string));
                dt.Columns.Add("PLAN_START_DATE", typeof(string));
                dt.Columns.Add("CONTACT_FAX", typeof(string));
                dt.Columns.Add("CONTACT_EMAIL", typeof(string));

                var result = from x in activityexport
                             select dt.LoadDataRow(new object[]
                             {
                                x.Channel,
                                x.Type,
                                x.Area,
                                x.Status,
                                x.Description,
                                x.Comment,
                                x.AssetInfo,
                                x.ContactInfo,
                                x.Ano,
                                x.CallId,
                                x.ContactName,
                                x.ContactLastName,
                                x.ContactPhone,
                                x.DoneFlg,
                                x.CreateDate,
                                x.CreateBy,
                                x.StartDate,
                                x.EndDate,
                                x.OwnerLogin,
                                x.OwnerPerId,
                                x.UpdateDate,
                                x.UpdateBy,
                                x.SrNo,
                                x.CallFlg,
                                x.EnqFlg,
                                x.LocEnqFlg,
                                x.DocReqFlg,
                                x.PriIssuedFlg,
                                x.AssetInspectFlg,                     
                                x.PlanstartDate,
                                x.ContactFax,
                                x.ContactEmail
                             }, false);

                //IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                //const string dateStr = "20160613";
                //string dateStr = DateTime.Now.ToString("yyyyMMdd");
                //string targetFile = string.Format("{0}\\{1}_{2}.txt", filePath, fileName, dateStr);
                //StreamWriter sw = new StreamWriter(targetFile, false, Encoding.UTF8);
                //sw.WriteLine(string.Join("|", columnNames));

                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fileName);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    foreach (DataRow row in result)
                    {
                        IEnumerable<string> fields =
                            row.ItemArray.Select(field => "\"" + field.ToString().Replace("\"", "\"\"") + "\"");
                        sw.WriteLine(string.Join(",", fields));
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

        public void SaveLogSuccess(ImportHpTaskResponse taskResponse)
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
                _auditLog.Action = Constants.AuditAction.ImportHP;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.Status = LogStatus.Success;
                _auditLog.Detail = sb.ToString();
                AppLog.AuditLog(_auditLog);
            }
        }
        public void SaveLogError(ImportHpTaskResponse taskResponse)
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
                _auditLog.Action = Constants.AuditAction.ImportHP;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.Status = LogStatus.Fail;
                _auditLog.Detail = sb.ToString();
                _auditLog.CreateUserId = null;
                AppLog.AuditLog(_auditLog);
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
                    if (_context != null) { _context.Dispose(); }
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
