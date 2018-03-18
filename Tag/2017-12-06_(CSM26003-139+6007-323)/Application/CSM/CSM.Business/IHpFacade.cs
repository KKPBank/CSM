using System;
using System.Collections.Generic;
using CSM.Entity;
using CSM.Service.Messages.SchedTask;

namespace CSM.Business
{
    public interface IHpFacade : IDisposable
    {
        string GetParameter(string paramName);
        List<HpActivityEntity> ReadFileHpActivity(string filePath, string fiPrefix, ref int numOfActivity, ref string fiActivity, ref bool isValidFile, ref string msgValidateFileError);
        bool SaveHpActivity(List<HpActivityEntity> hpActivity, string fiActivity);
        bool SaveHpActivityComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool ExportActivityHP(string filePath, string fileName);
        bool SaveServiceRequestActivity();
        void SaveLogSuccessOrFail(ImportHpTaskResponse taskResponse);
        void SaveLogError(ImportHpTaskResponse taskResponse);
    }
}
