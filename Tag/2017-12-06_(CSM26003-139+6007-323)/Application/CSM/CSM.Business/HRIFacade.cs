using CSM.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet.Sftp;
using System.IO;
using CSM.Common.Utilities;
using System.Globalization;
using Renci.SshNet;
using CSM.Entity;
using System.ComponentModel.DataAnnotations;
using CSM.Data.DataAccess;
using CSM.Service.Messages.SchedTask;

namespace CSM.Business
{
    public class HRIFacade : BaseFacade2<HRIFacade>, ISFTP
    {
        List<SftpFile> sftpFileList = new List<SftpFile>();

        string filePrefix = string.Empty;
        string host = string.Empty;
        int port = 22;
        string username = string.Empty;
        string password = string.Empty;
        string localPath = string.Empty;
        string errorPath = string.Empty;
        string sourcePath = string.Empty;
        string remoteDirectory = string.Empty;

        bool _download { get; set; }
        public bool SFTPDownload { get { return _download; } }

        int totalRows { get; set; }
        public int TotalRows { get { return totalRows; } }

        int errorRows { get; set; }
        public int ErrorRows { get { return errorRows; } }

        string errorMsg { get; set; }
        public string ErrorMessage { get { return errorMsg; } }

        public HRIFacade() : base()
        {
            using (var commFacade = new CommonFacade())
            {
                filePrefix = commFacade.GetValueParamByName(Constants.ParameterName.HRFilePrefix);
                host = commFacade.GetValueParamByName(Constants.ParameterName.HRSFTPHost);
                port = commFacade.GetValueParamByName(Constants.ParameterName.HRSFTPPort).ToNullable<int>() ?? 22;
                username = WebConfig.GetHRIUser();
                password = WebConfig.GetHRIPassword();
                localPath = commFacade.GetValueParamByName(Constants.ParameterName.HRPathImport);
                errorPath = commFacade.GetValueParamByName(Constants.ParameterName.HRPathError);
                sourcePath = commFacade.GetValueParamByName(Constants.ParameterName.HRPathSource);
                remoteDirectory = commFacade.GetValueParamByName(Constants.ParameterName.HRSFTPRemoteDir);
                _download = commFacade.GetValueParamByName(Constants.ParameterName.HRSFTPDownload)
                                .ToNullable<bool>() ?? false;
            }
        }

        public bool DeleteDownloadedFiles()
        {
            try
            {
                // Delete exist files
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Files Via FTP").ToInputLogString());

                bool isFileFound;

                using (var sftp = new SftpClient(host, port, username, password))
                {
                    sftp.Connect();
                    sftp.ChangeDirectory(remoteDirectory);
                    isFileFound = sftpFileList.Count > 0;
                    if (isFileFound)
                    {
                        foreach (var file in sftpFileList)
                        {
                            sftp.DeleteFile(file.FullName);
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

        public bool DownloadFiles()
        {
            try
            {
                // Delete exist files
                IEnumerable<string> localFiles = Directory.EnumerateFiles(localPath, $"{filePrefix}*.txt"); // lazy file system lookup

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

                using (var sftp = new SftpClient(host, port, username, password))
                {
                    sftp.Connect();
                    sftp.ChangeDirectory(remoteDirectory);

                    var files = sftp.ListDirectory(remoteDirectory)
                                .Where(x => x.Name.ToUpperInvariant().StartsWith(filePrefix.ToUpperInvariant()));

                    sftpFileList.Clear();
                    sftpFileList.AddRange(files);

                    isFileFound = sftpFileList.Count > 0;

                    if (isFileFound)
                    {
                        foreach (var file in sftpFileList)
                        {
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Download File").Add("FileName", file.FullName).ToInputLogString());
                            using (var fileStream = File.OpenWrite(Path.Combine(localPath, file.Name)))
                            {
                                sftp.DownloadFile(file.FullName, fileStream);
                            }
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Download File").ToSuccessLogString());
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

        public bool ReadHRFile(out string readFile, out string msgValidateFileError, out List<HRIEmployeeEntity> hriEmpls)
        {
            readFile = string.Empty;
            msgValidateFileError = string.Empty;
            hriEmpls = null;
            try
            {
                IEnumerable<string> files = Directory.EnumerateFiles(localPath, $"{filePrefix}*.txt"); // lazy file system lookup
                if (files.Any())
                {
                    FileInfo fi = new FileInfo(files.First());

                    readFile = fi.Name;

                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(fi.FullName);

                    #region "Validate file format"

                    bool isValidFormat = false;

                    // Validate Header
                    string[] header = results.FirstOrDefault();
                    if (header.Length != Constants.ImportHRI.ColumnCount)
                    {
                        msgValidateFileError = $"File {fi.Name} : columns length of header mismatch.";
                        Logger.Debug(msgValidateFileError);
                    }
                    else
                    {
                        isValidFormat = true;
                        int lineNo = 1;
                        foreach (var line in results.Skip(1))
                        {
                            lineNo++;
                            if (line.Length != Constants.ImportHRI.ColumnCount)
                            {
                                isValidFormat = false;
                                msgValidateFileError = $"File {fi.Name} : columns length of data row {lineNo} mismatch.";
                                Logger.Debug(msgValidateFileError);
                            }
                        }
                    }

                    if (isValidFormat == false)
                    {
                        // Move File to Error
                        if (StreamDataHelpers.TryToCopy(fi.FullName, string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", errorPath, fi.Name)))
                        {
                            StreamDataHelpers.TryToDelete(fi.FullName);
                        }
                        goto Outer;
                    }

                    #endregion

                    var empls = (from src in results.Skip(1)
                                 select new HRIEmployeeEntity
                                 {
                                     Branch = src[0].NullSafeTrim(),
                                     BranchDesc = src[1].NullSafeTrim(),
                                     EmployeeId = src[2].NullSafeTrim(),
                                     TitleId = src[3].NullSafeTrim(),
                                     Title = src[4].NullSafeTrim(),
                                     FName = src[5].NullSafeTrim(),
                                     LName = src[6].NullSafeTrim(),
                                     Nickname = src[7].NullSafeTrim(),
                                     FullNameEng = src[8].NullSafeTrim(),
                                     ETitle = src[9].NullSafeTrim(),
                                     EFName = src[10].NullSafeTrim(),
                                     ELName = src[11].NullSafeTrim(),
                                     Sex = src[12].NullSafeTrim(),
                                     BirthDay = src[13].NullSafeTrim(),
                                     EmpType = src[14].NullSafeTrim(),
                                     EmpTypeDesc = src[15].NullSafeTrim(),
                                     Position = src[16].NullSafeTrim(),
                                     PositionDesc = src[17].NullSafeTrim(),
                                     BU1 = src[18].NullSafeTrim(),
                                     BU1Desc = src[19].NullSafeTrim(),
                                     BU2 = src[20].NullSafeTrim(),
                                     BU2Desc = src[21].NullSafeTrim(),
                                     BU3 = src[22].NullSafeTrim(),
                                     BU3Desc = src[23].NullSafeTrim(),
                                     BU4 = src[24].NullSafeTrim(),
                                     BU4Desc = src[25].NullSafeTrim(),
                                     Job = src[26].NullSafeTrim(),
                                     JobPosition = src[27].NullSafeTrim(),
                                     StartDate = src[28].NullSafeTrim(),
                                     FirstHireDate = src[29].NullSafeTrim(),
                                     ResignDate = src[30].NullSafeTrim(),
                                     Status = src[31].NullSafeTrim(),
                                     EmpStatus = src[32].NullSafeTrim(),
                                     Email = src[33].NullSafeTrim(),
                                     NotesAddress = src[34].NullSafeTrim(),
                                     WorkArea = src[35].NullSafeTrim(),
                                     WorkAreaDesc = src[36].NullSafeTrim(),
                                     CostCenter = src[37].NullSafeTrim(),
                                     CostCenterDesc = src[38].NullSafeTrim(),
                                     TelExt = src[39].NullSafeTrim(),
                                     Boss = src[40].NullSafeTrim(),
                                     BossName = src[41].NullSafeTrim(),
                                     Assessor1 = src[42].NullSafeTrim(),
                                     Assessor1Name = src[43].NullSafeTrim(),
                                     Assessor2 = src[44].NullSafeTrim(),
                                     Assessor2Name = src[45].NullSafeTrim(),
                                     Assessor3 = src[46].NullSafeTrim(),
                                     Assessor3Name = src[47].NullSafeTrim(),
                                     TelNo = src[48].NullSafeTrim(),
                                     MobileNo = src[49].NullSafeTrim(),
                                     ADUser = src[50].NullSafeTrim(),
                                     OfficerId = src[51].NullSafeTrim(),
                                     OfficerDesc = src[52].NullSafeTrim(),
                                     AdditionJob = src[53].NullSafeTrim(),
                                     UnitBoss = src[54].NullSafeTrim(),
                                     UnitBossName = src[55].NullSafeTrim(),
                                     IDNO = src[56].NullSafeTrim()
                                 });

                    #region "Validate MaxLength"

                    ValidationContext vc = null;
                    int inxErr = 2;
                    var lstErrMaxLength = new List<string>();
                    foreach (HRIEmployeeEntity empl in empls)
                    {
                        vc = new ValidationContext(empl, null, null);
                        var validationResults = new List<ValidationResult>();
                        bool valid = Validator.TryValidateObject(empl, vc, validationResults, true);
                        if (!valid)
                        {
                            empl.Error = validationResults.Select(x => x.ErrorMessage)
                                            .Aggregate((result, item) => result + ", " + item);
                            lstErrMaxLength.Add($"@Line {inxErr}: {empl.Error}");
                        }
                        inxErr++;
                    }
                    if (lstErrMaxLength.Count > 0)
                    {
                        Logger.Debug($"File:{fi.Name} Invalid MaxLength \n{string.Join("\n", lstErrMaxLength.ToArray())}");
                    }
                    if (empls.Count(x => !string.IsNullOrWhiteSpace(x.Error)) > 0)
                    {
                        // Move File to Error
                        if (StreamDataHelpers.TryToCopy(fi.FullName, string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", errorPath, fi.Name)))
                        {
                            StreamDataHelpers.TryToDelete(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", localPath, fi.Name));
                        }
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid maxLength.", fi.Name);
                        goto Outer;
                    }
                    #endregion

                    hriEmpls = empls.ToList();

                    #region "Move file to PathSource"
                    if (fi.Exists)
                    {
                        StreamDataHelpers.TryToCopy(fi.FullName, string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", sourcePath, fi.Name));
                        StreamDataHelpers.TryToDelete(fi.FullName);
                    }
                    #endregion

                    return true;
                }
                else
                {
                    msgValidateFileError = " File not found.";
                }
                Outer:
                return false;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("file {0}: {1}", readFile, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw;
            }
            finally
            { }
        }

        public bool InsertHRTempTable(List<HRIEmployeeEntity> hriEmpls)
        {
            return (new HRIDataAccess(_context)).InsertHRTempTable(hriEmpls);
        }

        public bool UpdateHREmployee(out int empInsert, out int empUpdate, out int empMarkDelete, out int buInsert, out int buUpdate, out int buMarkDelete, out int brInsert, out int brUpdate, out int brMarkDelete, out string msg)
        {
            return (new HRIDataAccess(_context)).UpdateHREmployee(out empInsert, out empUpdate, out empMarkDelete, out buInsert, out buUpdate, out buMarkDelete, out brInsert, out brUpdate, out brMarkDelete, out msg);
        }

        public void SaveLogSuccessOrFail(ImportHRTaskResponse taskResponse)
        {
            if (taskResponse != null)
            {
                StringBuilder sb = new StringBuilder("");
                sb.AppendFormat("วัน เวลาที่ run task scheduler = {0}\n",
                    taskResponse.SchedDateTime.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime));
                sb.AppendFormat("ElapsedTime = {0} (ms)\n", taskResponse.ElapsedTime);
                sb.Append(taskResponse.ToString());

                var _auditLog = new AuditLogEntity();
                _auditLog.Module = Constants.Module.Batch;
                _auditLog.Action = Constants.AuditAction.ImportHRIS;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.Status = (taskResponse.StatusResponse.Status == Constants.StatusResponse.Failed) ? LogStatus.Fail : LogStatus.Success;
                _auditLog.Detail = sb.ToString();
                AppLog.AuditLog(_auditLog);
            }
        }

        public void SaveLogError(ImportHRTaskResponse taskResponse)
        {
            if (taskResponse != null)
            {
                StringBuilder sb = new StringBuilder("");
                sb.AppendFormat("วัน เวลาที่ run task scheduler = {0}\n",
                    taskResponse.SchedDateTime.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime));
                sb.AppendFormat("ElapsedTime = {0} (ms)\n", taskResponse.ElapsedTime);
                sb.AppendFormat("Error Message = {0}\n", taskResponse.StatusResponse.Description);

                var _auditLog = new AuditLogEntity();
                _auditLog.Module = Constants.Module.Batch;
                _auditLog.Action = Constants.AuditAction.ImportHRIS;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.Status = LogStatus.Fail;
                _auditLog.Detail = sb.ToString();
                _auditLog.CreateUserId = null;
                AppLog.AuditLog(_auditLog);
            }
        }
    }
}
