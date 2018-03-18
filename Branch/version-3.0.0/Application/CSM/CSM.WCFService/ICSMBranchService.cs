using System;
using System.ServiceModel;
using CSM.Entity;
using CSM.Common.Utilities;
using CSM.Service.Messages.Branch;
using CSM.Service.Messages.Calendar;

namespace CSM.WCFService
{
    [ServiceContract(Namespace = Constants.ServicesNamespace.BranchService)]
    public interface ICSMBranchService : IDisposable
    {
        [OperationContract]
        CreateBranchResponse InsertOrUpdateBranch(InsertOrUpdateBranchRequest request);

        [OperationContract]
        UpdateCalendarResponse UpdateBranchCalendar(UpdateBranchCalendarRequest request);
    }
}
