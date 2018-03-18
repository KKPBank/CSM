using System.ServiceModel;
using CSM.Common.Utilities;
using System;
using CSM.Service.Messages.Customer;

namespace CSM.WCFService
{
    [ServiceContract(Namespace = Constants.ServicesNamespace.CustomerService)]
    public interface ICSMCustomerService : IDisposable
    {
        [OperationContract]
        ImportSubscriptionResponse ImportSubscription(ImportSubscriptionRequest request);
    }
}
