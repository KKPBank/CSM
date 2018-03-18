using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
using Renci.SshNet.Sftp;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CSM.Business
{
    public class BdwFacade : IBdwFacade
    {
        private AuditLogEntity _auditLog;
        private ICommonFacade _commonFacade;
        private readonly CSMContext _context;
        private IBdwDataAccess _bdwDataAccess;
        static Object _lockObject = new Object();
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BdwFacade));

        public BdwFacade()
        {
            _context = new CSMContext();
        }

        public bool SaveBdwContact(List<BdwContactEntity> bdwContacts)
        {
            _bdwDataAccess = new BdwDataAccess(_context);
            return _bdwDataAccess.SaveBdwContact(bdwContacts);
        }

        public bool SaveCompleteBdwContact(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _bdwDataAccess = new BdwDataAccess(_context);
            return _bdwDataAccess.SaveCompleteBdwContact(ref numOfComplete, ref numOfError, ref messageError);
        }

        public void SaveLogSuccessOrFail(ImportBDWTaskResponse taskResponse)
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
                _auditLog.Action = Constants.AuditAction.ImportBDW;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.Status = (taskResponse.StatusResponse.Status == Constants.StatusResponse.Failed) ? LogStatus.Fail : LogStatus.Success;
                _auditLog.Detail = sb.ToString();
                AppLog.AuditLog(_auditLog);
            }
        }

        public void SaveLogError(ImportBDWTaskResponse taskResponse)
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
                _auditLog.Action = Constants.AuditAction.ImportBDW;
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

        public List<BdwContactEntity> ReadFileBdwContact(string filePath, string fiPrefix, ref int numOfBdw, ref string fiBdw, ref bool isValidFile, ref string msgValidateFileError)
        {
            bool hasFile = true;
            try
            {

                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiBdw = Path.GetFileName(files.First());

                    string endFile = fiBdw.Replace(".txt", ".end").Replace(".TXT", ".end");

                    if (File.Exists(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, endFile)) == false)
                    {
                        hasFile = false;
                        isValidFile = false; // ref value
                        msgValidateFileError = " File not found.";
                        goto Outer;
                    }

                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"

                    bool isValidFormat = false;

                    // Validate Header
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportBDWContact.LengthOfHeader)
                    {
                        if (header[0] == Constants.ImportBDWContact.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File:{0} dataType of header mismatch", fiBdw);
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiBdw);
                        }
                    }
                    else
                    {
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File:{0} length of header mismatch", fiBdw);
                        Logger.DebugFormat("File:{0} length of header mismatch", fiBdw);
                    }

                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportBDWContact.LengthOfHeader - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File:{0} TotalRecord mismatch", fiBdw);
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiBdw);
                        }
                    }

                    if (isValidFormat)
                    {
                        int inx = 2;
                        var lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportBDWContact.LengthOfDetail || source[0].NullSafeTrim() != Constants.ImportBDWContact.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiBdw, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiBdw,
                                lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);

                        if (isValidFormat == false)
                        {
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiBdw);
                        }
                    }

                    if (isValidFormat == false)
                    {
                        // Move File
                        string bdwExportPath = this.GetParameter(Constants.ParameterName.BDWPathError);
                        StreamDataHelpers.TryToCopy(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiBdw), string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", bdwExportPath, fiBdw));

                        isValidFile = false; // ref value
                        goto Outer;
                    }

                    #endregion

                    BdwContactEntity headerContact = (from source in results
                                                      where source[0].NullSafeTrim() == Constants.ImportBDWContact.DataTypeHeader
                                                      select new BdwContactEntity
                                                      {
                                                          DataType = source[0].NullSafeTrim(),
                                                          DataDate = source[1].NullSafeTrim(),
                                                          DataSource = source[2].NullSafeTrim()
                                                      }).FirstOrDefault();

                    List<BdwContactEntity> deatilContacts = (from source in results.Skip(1)
                                                             select new BdwContactEntity
                                                             {
                                                                 DataType = source[0].NullSafeTrim(),
                                                                 DataDate = source[1].NullSafeTrim(),
                                                                 DataSource = source[2].NullSafeTrim(),
                                                                 CardTypeCode = source[3].NullSafeTrim(),
                                                                 CardNo = source[4].NullSafeTrim(),
                                                                 TitileTh = source[5].NullSafeTrim(),
                                                                 NameTh = source[6].NullSafeTrim(),
                                                                 SurnameTh = source[7].NullSafeTrim(),
                                                                 TitileEn = source[8].NullSafeTrim(),
                                                                 NameEn = source[9].NullSafeTrim(),
                                                                 SurnameEn = source[10].NullSafeTrim(),
                                                                 AccountNo = source[11].NullSafeTrim(),
                                                                 LoanMain = source[12].NullSafeTrim(),
                                                                 ProductGroup = source[13].NullSafeTrim(),
                                                                 Product = source[14].NullSafeTrim(),
                                                                 Campaign = source[15].NullSafeTrim(),
                                                                 AccountStatus = source[16].NullSafeTrim(),
                                                                 Relationship = source[17].NullSafeTrim(),
                                                                 Phone = source[18].NullSafeTrim()
                                                             }).ToList();

                    #region "Validate MaxLength"

                    ValidationContext vc = null;
                    int inxErr = 2;
                    var lstErrMaxLength = new List<string>();
                    foreach (BdwContactEntity bdwContact in deatilContacts)
                    {
                        vc = new ValidationContext(bdwContact, null, null);
                        var validationResults = new List<ValidationResult>();
                        bool valid = Validator.TryValidateObject(bdwContact, vc, validationResults, true);
                        if (!valid)
                        {
                            bdwContact.Error =
                                validationResults.Select(x => x.ErrorMessage)
                                    .Aggregate((i, j) => i + ", " + j);

                            lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), bdwContact.Error));
                        }

                        inxErr++;
                    }

                    if (lstErrMaxLength.Count > 0)
                    {
                        Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiBdw, string.Join("\n", lstErrMaxLength.ToArray()));
                    }

                    if (deatilContacts.Count(x => !string.IsNullOrWhiteSpace(x.Error)) > 0)
                    {
                        // Move File
                        string bdwExportPath = this.GetParameter(Constants.ParameterName.BDWPathError);
                        StreamDataHelpers.TryToCopy(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiBdw), string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", bdwExportPath, fiBdw));

                        //Logger.DebugFormat("File:{0} Invalid MaxLength", fiBdw);
                        isValidFile = false; // ref value
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid maxLength.", fiBdw);
                        goto Outer;
                    }

                    #endregion

                    var bdwContacts = new List<BdwContactEntity>();
                    bdwContacts.Add(headerContact);
                    bdwContacts.AddRange(deatilContacts);

                    numOfBdw = deatilContacts.Count;
                    return bdwContacts;
                }
                else
                {
                    isValidFile = false; // ref value
                    msgValidateFileError = " File not found.";
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

                if (!string.IsNullOrEmpty(fiBdw) && hasFile)
                {
                    string bdwExportSource = this.GetParameter(Constants.ParameterName.BdwPathSource);
                    StreamDataHelpers.TryToCopy(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiBdw), string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", bdwExportSource, fiBdw));
                    StreamDataHelpers.TryToDelete(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fiBdw));
                    string endFile = fiBdw.Replace(".txt", ".end").Replace(".TXT", ".end");
                    StreamDataHelpers.TryToDelete(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, endFile));
                }

                #endregion
            }
        }

        public bool ExportErrorBdwContact(string filePath, string fileName, ref int numOfError)
        {
            _bdwDataAccess = new BdwDataAccess(_context);
            var header = _bdwDataAccess.GetErrorHeaderBdwContact();
            int totalRecords = _bdwDataAccess.GetCountErrorBdwContact();

            if (ExportBdwContactHeader(filePath, fileName, header, totalRecords))
            {
                int count = 0;
                int pageSize = 5000;
                int totalPage = (totalRecords + pageSize - 1) / pageSize;

                Task.Factory.StartNew(() => Parallel.For(0, totalPage, new ParallelOptions { MaxDegreeOfParallelism = WebConfig.GetTotalCountToProcess() },
                    k =>
                    {
                        lock (_lockObject)
                        {
                            List<BdwContactEntity> contactList = _bdwDataAccess.GetErrorBdwContact(k, pageSize);
                            count += contactList.Count;

                            if (!ExportBdwContactDetail(filePath, fileName, contactList))
                            {
                                return;
                            }
                        }
                    })).Wait();

                numOfError = count;
                return (totalRecords == numOfError) ? true : false;
            }

            return false;
        }

        private static bool ExportBdwContactHeader(string filePath, string fileName, BdwContactEntity header, int totalRecord)
        {
            try
            {
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fileName);
                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    // Header
                    sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}", header.DataType, header.DataDate, totalRecord.ToString(CultureInfo.InvariantCulture)));
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

        private bool ExportBdwContactDetail(string filePath, string fileName, List<BdwContactEntity> bdwContactList)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add("DATA_TYPE", typeof(string));
                dt.Columns.Add("DATA_DATE", typeof(string));
                dt.Columns.Add("DATA_SOURCE", typeof(string));
                dt.Columns.Add("CARD_TYPE_CODE", typeof(string));
                dt.Columns.Add("CARD_NO", typeof(string));
                dt.Columns.Add("TITILE_TH", typeof(string));
                dt.Columns.Add("NAME_TH", typeof(string));
                dt.Columns.Add("SURNAME_TH", typeof(string));
                dt.Columns.Add("TITILE_EN", typeof(string));
                dt.Columns.Add("NAME_EN", typeof(string));
                dt.Columns.Add("SURNAME_EN", typeof(string));
                dt.Columns.Add("ACCOUNT_NO", typeof(string));
                dt.Columns.Add("LOAN_MAIN", typeof(string));
                dt.Columns.Add("PRODUCT_GROUP", typeof(string));
                dt.Columns.Add("PRODUCT", typeof(string));
                dt.Columns.Add("CAMPAIGN", typeof(string));
                dt.Columns.Add("ACCOUNT_STATUS", typeof(string));
                dt.Columns.Add("RELATIONSHIP", typeof(string));
                dt.Columns.Add("PHONE", typeof(string));
                dt.Columns.Add("ERROR", typeof(string));

                var result = from x in bdwContactList
                             select dt.LoadDataRow(new object[]
                    {
                        x.DataType,
                        x.DataDate,
                        x.DataSource,
                        x.CardTypeCode,
                        x.CardNo,
                        x.TitileTh,
                        x.NameTh,
                        x.SurnameTh,
                        x.TitileEn,
                        x.NameEn,
                        x.SurnameEn,
                        x.AccountNo,
                        x.LoanMain,
                        x.ProductGroup,
                        x.Product,
                        x.Campaign,
                        x.AccountStatus,
                        x.Relationship,
                        x.Phone,
                        x.Error
                    }, false);

                _lock.EnterWriteLock();
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fileName);

                using (var sw = new StreamWriter(targetFile, true, Encoding.UTF8))
                {
                    // Details
                    foreach (DataRow row in result)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sw.WriteLine(string.Join("|", fields));
                    }

                    //sw.Close();
                }
                bdwContactList = null;
                return true;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        #region "Functions"

        List<SftpFile> _downloadList = new List<SftpFile>();

        /// <summary>
        /// This will list the contents of the current directory.
        /// </summary>
        public bool DownloadFilesViaFTP(string localPath, string fiPrefix)
        {
            try
            {
                // Delete exist files
                IEnumerable<string> localFiles = Directory.EnumerateFiles(localPath, string.Format(CultureInfo.InvariantCulture, "{0}*.*", fiPrefix)); // lazy file system lookup

                foreach (var localFile in localFiles)
                {
                    if (StreamDataHelpers.TryToDelete(localFile))
                    {
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete exist local file").Add("FileName", localFile).ToSuccessLogString());
                    }
                    else
                    {
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete exist local file").Add("FileName", localFile).ToFailLogString());
                    }
                }

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Download Files Via FTP").Add("LocalPath", localPath).ToInputLogString());

                bool isFileFound;
                string host = WebConfig.GetBDWSshServer();
                int port = WebConfig.GetBDWSshPort();
                string username = WebConfig.GetBDWSshUsername();
                string password = WebConfig.GetBDWSshPassword();
                string remoteDirectory = WebConfig.GetBDWSshRemoteDir(); // . always refers to the current directory.

                using (var sftp = new SftpClient(host, port, username, password))
                {
                    sftp.Connect();
                    var files = sftp.ListDirectory(remoteDirectory).Where(x => x.Name.ToUpperInvariant().Contains(fiPrefix.ToUpperInvariant()));

                    //เช็คไฟล์ที่ write เสร็จแล้ว จะมีไฟล์ชื่อเดียวกันที่มีนามสกุล .txt และ .end
                    var txt = files.Where(f => Path.GetExtension(f.Name).ToUpperInvariant() == ".TXT");
                    var end = files.Where(f => Path.GetExtension(f.Name).ToUpperInvariant() == ".END");

                    _downloadList.Clear();

                    foreach (var t in txt)
                    {
                        var y = end.Where(e => Path.GetFileNameWithoutExtension(e.Name).ToUpperInvariant() == Path.GetFileNameWithoutExtension(t.Name).ToUpperInvariant())
                                .FirstOrDefault();
                        if (y != null)
                        {
                            _downloadList.Add(t);
                            _downloadList.Add(y);
                        }
                    }

                    isFileFound = _downloadList.Count > 0;

                    if (isFileFound)
                    {
                        // Download file to local via SFTP
                        foreach (var file in _downloadList)
                        {
                            DownloadFile(sftp, file, localPath);
                        }

                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Download Files Via FTP").ToSuccessLogString());
                    }
                    else
                    {
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Download Files Via FTP").Add("Error Message", "File Not Found").ToFailLogString());
                    }

                    sftp.Disconnect();
                }
                return isFileFound;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Download Files Via FTP").Add("Error Message", ex.Message).ToInputLogString());
            }

            return false;
        }

        private void DownloadFile(SftpClient client, SftpFile file, string directory)
        {
            try
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Download File").Add("FileName", file.FullName).ToInputLogString());

                using (var fileStream = File.OpenWrite(Path.Combine(directory, file.Name)))
                {
                    client.DownloadFile(file.FullName, fileStream);
                    // fileStream.Close();
                }

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Download File").ToSuccessLogString());
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Download File").Add("Error Message", ex.Message).ToInputLogString());
                throw;
            }
        }

        public bool DeleteFilesViaFTP(string fiPrefix)
        {
            try
            {
                // Delete exist files
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Files Via FTP").ToInputLogString());

                bool isFileFound;
                string host = WebConfig.GetBDWSshServer();
                int port = WebConfig.GetBDWSshPort();
                string username = WebConfig.GetBDWSshUsername();
                string password = WebConfig.GetBDWSshPassword();
                string remoteDirectory = WebConfig.GetBDWSshRemoteDir(); // . always refers to the current directory.

                using (var sftp = new SftpClient(host, port, username, password))
                {
                    sftp.Connect();
                    //var files = sftp.ListDirectory(remoteDirectory).Where(x => x.Name.ToUpperInvariant().Contains(fiPrefix.ToUpperInvariant()));
                    //isFileFound = files.Any();
                    isFileFound = _downloadList.Count > 0;

                    if (isFileFound)
                    {
                        // Download file to local via SFTP
                        foreach (var file in _downloadList)
                        {
                            DeleteFile(sftp, file.FullName);
                        }

                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Files Via FTP").ToSuccessLogString());
                    }
                    else
                    {
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Files Via FTP").Add("Error Message", "File Not Found").ToFailLogString());
                    }

                    sftp.Disconnect();
                }
                return isFileFound;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Files Via FTP").Add("Error Message", ex.Message).ToInputLogString());
            }

            return false;
        }

        private void DeleteFile(SftpClient client, string remoteFile)
        {
            try
            {
                client.DeleteFile(remoteFile);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Remote File").Add("FileName", remoteFile).ToSuccessLogString());
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Remote File").Add("Error Message", ex.Message).ToInputLogString());
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
                    if (_lock != null) { _lock.Dispose(); }
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
