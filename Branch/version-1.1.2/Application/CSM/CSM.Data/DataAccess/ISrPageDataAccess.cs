using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Data.DataAccess
{
    public interface ISrPageDataAccess
    {
        List<SrPageItemEntity> GetSrPageList();
    }
}
