using System;
using System.ServiceModel;
using CSM.Common.Utilities;
using CSM.Service.Messages.SchedTask;

namespace CSM.WCFService
{   
    [ServiceContract(Namespace = Constants.ServicesNamespace.FileService)]
    public interface ICSMFileService : IDisposable
    {
        [OperationContract]
        ImportAFSTaskResponse GetFileAFS(string username, string password);

        [OperationContract]
        ExportAFSTaskResponse ExportFileAFS(string username, string password);

        [OperationContract]
        ExportNCBTaskResponse ExportFileNCB(string username, string password);

        [OperationContract]
        ImportBDWTaskResponse GetFileBDW(string username, string password, bool skipSftp);

        [OperationContract]
        ImportCISTaskResponse GetFileCIS(string username, string password);

        [OperationContract]
        ImportHpTaskResponse GetFileHP(string username, string password);

        [OperationContract]
        ImportHRTaskResponse GetFileHR(string username, string password);
    }
}
