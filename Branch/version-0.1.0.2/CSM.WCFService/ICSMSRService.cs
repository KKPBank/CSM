using System;
using CSM.Entity;
using System.ServiceModel;
using CSM.Service.Messages.SchedTask;
using CSM.Common.Utilities;

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
        GetSRResponse GetSR(string srNo);

        [OperationContract]
        CreateSrFromReplyEmailTaskResponse CreateSRActivityFromReplyEmail(string username, string password);

        [OperationContract]
        ReSubmitActivityToCARSystemTaskResponse ReSubmitActivityToCARSystem(string username, string password);

        [OperationContract]
        ReSubmitActivityToCBSHPSystemTaskResponse ReSubmitActivityToCBSHPSystem(string username, string password);
    }
}
