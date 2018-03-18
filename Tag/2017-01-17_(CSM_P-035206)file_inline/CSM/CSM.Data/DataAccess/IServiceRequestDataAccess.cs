using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Data.DataAccess
{
    public interface IServiceRequestDataAccess
    {
        IEnumerable<ServiceRequestActivityResult> GetSRActivityList(ActivitySearchFilter searchFilter);
    }
}
