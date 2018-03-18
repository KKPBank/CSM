using CSM.Business;
using CSM.Entity;

namespace CSM.WCFService
{
    public class CSMBranchService : ICSMBranchService
    {
        public InsertOrUpdateBranchResponse InsertOrUpdateBranch(InsertOrUpdateBranchRequest request)
        {
            IBranchFacade facade = new BranchFacade();
            return facade.InsertOrUpdateBranch(request);
        }

        public UpdateBranchCalendarResponse UpdateBranchCalendar(UpdateBranchCalendarRequest request)
        {
            IBranchFacade facade = new BranchFacade();
            return facade.UpdateBranchCalendar(request);
        }
    }
}
