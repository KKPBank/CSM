using System;
using System.Collections.Generic;
using CSM.Entity;
using CSM.Service.Messages.SchedTask;

namespace CSM.Business
{
    public interface IAFSFacade : IDisposable
    {
        bool SaveAFSProperties(List<PropertyEntity> properties);
        bool SaveSaleZones(List<SaleZoneEntity> saleZones, string fiSaleZone);
        bool SaveCompleteProperties(ref int numOfComplete);
        bool ExportErrorProperties(string filePath, string fileName, ref int numOfErrProp);
        bool ExportErrorSaleZones(string filePath, string fileName, ref int numOfErrSaleZone);
        string GetParameter(string paramName);
        List<PropertyEntity> ReadFileProperty(string filepath, string fiPrefix, ref int numOfProp, ref string fiProp, ref bool isValidFile, ref string msgValidateFileError, bool isValidFile_SaleZone);
        List<SaleZoneEntity> ReadFileSaleZone(string filepath, string fiPrefix, ref int numOfSaleZones, ref string fiSaleZone, ref bool isValidFile, ref string msgValidateFileError);
        void SaveLogSuccess(ImportAFSTaskResponse taskResponse);
        void SaveLogError(ImportAFSTaskResponse taskResponse);
        bool ExportActivityAFS(string filePath, string fileName, ref int numOfActivity);
        void SaveLogExportSuccess(ExportAFSTaskResponse taskResponse);
        void SaveLogExportError(ExportAFSTaskResponse taskResponse);
        bool ExportEmployeeNCBNew(string filePath, string fileName, ref int numOfNew, ref string newEmplFile);
        bool ExportEmplyeeNCBUpdate(string filePath, string fileName, ref int numOfUpdate, ref string updateEmplFile);
        void SaveLogExportMarketingSuccess(ExportNCBTaskResponse taskResponse);
        void SaveLogExportMarketingError(ExportNCBTaskResponse taskResponse);
        bool UploadFilesViaFTP(string newEmplFile, string updateEmplFile);
    }
}
