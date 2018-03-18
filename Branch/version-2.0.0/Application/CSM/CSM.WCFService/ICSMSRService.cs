using System;
using System.ServiceModel;
using CSM.Service.Messages.SchedTask;
using CSM.Common.Utilities;
using CSM.Service.Messages.Sr;

namespace CSM.WCFService
{
    [ServiceContract(Namespace = Constants.ServicesNamespace.SRService)]
    public interface ICSMSRService : IDisposable
    {
        [OperationContract]
        CreateSRResponse CreateSR(CreateSRRequest request);

        [OperationContract]
        UpdateSRResponse UpdateSR(UpdateSRRequest request);

        [OperationContract]
        SearchSRResponse SearchSR(SearchSRRequest request);

        [OperationContract]
        GetSRResponse GetSR(GetSRRequest request);

        [OperationContract]
        CreateSrFromReplyEmailTaskResponse CreateSRActivityFromReplyEmail(string username, string password);

        [OperationContract]
        ReSubmitActivityToCARSystemTaskResponse ReSubmitActivityToCARSystem(string username, string password);

        [OperationContract]
        ReSubmitActivityToCBSHPSystemTaskResponse ReSubmitActivityToCBSHPSystem(string username, string password);

        [OperationContract]
        ExportSRTaskResponse DailySRReport();

        [OperationContract]
        ExportSRTaskResponse SRReport();
    }
}
