using System.ServiceModel;
using CSM.Entity;
using CSM.Common.Utilities;

namespace CSM.WCFService
{
    [ServiceContract(Namespace = Constants.ServicesNamespace.BranchService)]
    public interface ICSMBranchService
    {
        [OperationContract]
        InsertOrUpdateBranchResponse InsertOrUpdateBranch(InsertOrUpdateBranchRequest request);

        [OperationContract]
        UpdateBranchCalendarResponse UpdateBranchCalendar(UpdateBranchCalendarRequest request);
    }
}
