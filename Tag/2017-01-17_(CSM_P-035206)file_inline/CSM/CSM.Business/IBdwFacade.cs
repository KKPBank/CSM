using System;
using System.Collections.Generic;
using CSM.Entity;
using CSM.Service.Messages.SchedTask;

namespace CSM.Business
{
    public interface IBdwFacade : IDisposable
    {
        bool SaveBdwContact(List<BdwContactEntity> bdwContacts);
        bool SaveCompleteBdwContact(ref int numOfComplete, ref int numOfError, ref string messageError);
        void SaveLogSuccessOrFail(ImportBDWTaskResponse taskResponse);
        void SaveLogError(ImportBDWTaskResponse taskResponse);
        List<BdwContactEntity> ReadFileBdwContact(string filePath, string fiPrefix, ref int numOfBdw, ref string fiBdw, ref bool isValidFile, ref string msgValidateFileError);
        bool ExportErrorBdwContact(string filePath, string fileName, ref int numOfError);
        string GetParameter(string paramName);
        bool DownloadFilesViaFTP(string localPath, string fiPrefix);
        bool DeleteFilesViaFTP(string fiPrefix);
    }
}
