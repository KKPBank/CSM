using System.IO;
using System.Text.RegularExpressions;
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
                    else if (lstLengthNotMatch.Count > 0)
                    {
                        Logger.DebugFormat("File:{0} Invalid format {1} records", fiActivity,
                            lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
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

                    var rx = new Regex(@".*SR;.*;\s*Status\s*:.*;\s*Contact\s*Mobile\s*#", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    List<HpActivityEntity> hpActivity = (from source in results
                                                         where !rx.IsMatch(source[5].NullSafeTrim())
                                                         select new HpActivityEntity
                                                         {
                                                             Channel = source[0].NullSafeTrim(),
                                                             Type = source[1].NullSafeTrim(),
                                                             Area = source[2].NullSafeTrim(),
                                                             Status = source[3].NullSafeTrim(),
                                                             Description = source[4].NullSafeTrim(),
                                                             Comment = source[5].NullSafeTrim(),
                                                             AssetInfo = source[6].NullSafeTrim(),
                                                             ContactInfo = source[7].NullSafeTrim(),
                                                             Ano = source[8].NullSafeTrim(),
                                                             CallId = source[9].NullSafeTrim(),
                                                             ContactName = source[10].NullSafeTrim(),
                                                             ContactLastName = source[11].NullSafeTrim(),
                                                             ContactPhone = source[12].NullSafeTrim(),
                                                             DoneFlg = source[13].NullSafeTrim(),
                                                             CreateDate = source[14].NullSafeTrim(),
                                                             CreateBy = source[15].NullSafeTrim(),
                                                             StartDate = source[16].NullSafeTrim(),
                                                             EndDate = source[17].NullSafeTrim(),
                                                             OwnerLogin = source[18].NullSafeTrim(),
                                                             OwnerPerId = source[19].NullSafeTrim(),
                                                             UpdateDate = source[20].NullSafeTrim(),
                                                             UpdateBy = source[21].NullSafeTrim(),
                                                             SrNo = source[22].NullSafeTrim(),
                                                             CallFlg = source[23].NullSafeTrim(),
                                                             EnqFlg = source[24].NullSafeTrim(),
                                                             LocEnqFlg = source[25].NullSafeTrim(),
                                                             DocReqFlg = source[26].NullSafeTrim(),
                                                             PriIssuedFlg = source[27].NullSafeTrim(),
                                                             AssetInspectFlg = source[28].NullSafeTrim(),
                                                             PlanstartDate = source[29].NullSafeTrim(),
                                                             ContactFax = source[30].NullSafeTrim(),
                                                             ContactEmail = source[31].NullSafeTrim()
                                                         }).ToList();
                    
                    #region "Validate MaxLength"

                    ValidationContext vc = null;
                    int inxErr = 1;
                    var lstErrMaxLength = new List<string>();
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

                        //Logger.DebugFormat("File:{0} Invalid MaxLength", fiActivity);
                        isValidFile = false; // ref value
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid maxLength.", fiActivity);
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

                if (!string.IsNullOrEmpty(fiActivity))
                {
                    string hpPathSource = this.GetParameter(Constants.ParameterName.HpPathSource);
                    StreamDataHelpers.TryToCopy(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiActivity), string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", hpPathSource, fiActivity));
                    StreamDataHelpers.TryToDelete(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiActivity));
                }

                #endregion
            }
        }

        public HpStatusEntity GetHpStatusById(int? id)
        {
            return (new HpDataAccess(_context)).GetHpStatus(id).FirstOrDefault();
        }

        public List<HpStatusEntity> GetHpStatus()
        {
            return (new HpDataAccess(_context)).GetHpStatus().OrderBy(o => o.HpLangIndeCode).ToList();
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
                var auditLog = new AuditLogEntity();
                auditLog.Module = Constants.Module.Batch;
                auditLog.Action = Constants.AuditAction.SubmitActivityToCARSystem;
                auditLog.IpAddress = ApplicationHelpers.GetClientIP();

                _srFacade = new ServiceRequestFacade();
                foreach (var sr in lstSrActivity)
                {
                    // TODO :: Add new parameter: AuditLogEntity for Call CAR Web Service
                    _srFacade.CreateServiceRequestActivity(auditLog, sr, true);
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
                dt.Columns.Add("ERROR", typeof(string));

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
                                x.ContactEmail,
                                x.Error
                             }, false);
                
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fileName);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    foreach (DataRow row in result)
                    {
                        IEnumerable<string> fields =
                            row.ItemArray.Select(field => "\"" + field.ToString().Replace("\"", "\"\"") + "\"");
                        sw.WriteLine(string.Join(",", fields));
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

        public void SaveLogSuccessOrFail(ImportHpTaskResponse taskResponse)
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
                _auditLog.Status = (taskResponse.StatusResponse.Status == Constants.StatusResponse.Failed) ? LogStatus.Fail : LogStatus.Success;
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
