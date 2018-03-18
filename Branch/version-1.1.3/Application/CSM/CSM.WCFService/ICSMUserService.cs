using System;
using System.ServiceModel;
using CSM.Common.Utilities;
using CSM.Service.Messages.User;

namespace CSM.WCFService
{
    [ServiceContract(Namespace = Constants.ServicesNamespace.UserService)]
    public interface ICSMUserService : IDisposable
    {
        [OperationContract]
        InsertOrUpdateUserResponse InsertOrUpdateUser(InsertOrUpdateUserRequest request);
    }
}
