using System;
using System.ServiceModel;
using CSM.Service.Messages.Master;
using CSM.Common.Utilities;

namespace CSM.WCFService
{
    [ServiceContract(Namespace = Constants.ServicesNamespace.MasterService)]
    public interface ICSMMasterService : IDisposable
    {
        [OperationContract]
        CreateProductMasterResponse CreateProductMaster(CreateProductMasterRequest request);
    }
}
