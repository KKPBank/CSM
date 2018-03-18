using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CSM.Entity;
using CSM.Common.Utilities;

namespace CSM.WCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICSMUserService" in both code and config file together.
    [ServiceContract(Namespace = Constants.ServicesNamespace.UserService)]
    public interface ICSMUserService
    {
        [OperationContract]
        InsertOrUpdateUserResponse InsertOrUpdateUser(InsertOrUpdateUserRequest request);
    }
}
