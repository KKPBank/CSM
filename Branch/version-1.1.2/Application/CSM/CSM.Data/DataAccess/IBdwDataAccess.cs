using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Data.DataAccess
{
    public interface IBdwDataAccess
    {
        bool SaveBdwContact(List<BdwContactEntity> bdwContacts);
        bool SaveCompleteBdwContact(ref int numOfComplete, ref int numOfError, ref string messageError);
        List<BdwContactEntity> GetErrorBdwContact(int pageIndex, int pageSize);
        BdwContactEntity GetErrorHeaderBdwContact();
        int GetCountErrorBdwContact();
    }
}
