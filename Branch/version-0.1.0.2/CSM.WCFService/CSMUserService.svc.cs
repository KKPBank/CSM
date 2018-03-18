using CSM.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CSM.Business;

namespace CSM.WCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CSMUserService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CSMUserService.svc or CSMUserService.svc.cs at the Solution Explorer and start debugging.
    public class CSMUserService : ICSMUserService
    {
        public InsertOrUpdateUserResponse InsertOrUpdateUser(InsertOrUpdateUserRequest request)
        {
            IUserFacade facade = new UserFacade();
            return facade.InsertOrUpdateUser(request);
        }
    }
}
