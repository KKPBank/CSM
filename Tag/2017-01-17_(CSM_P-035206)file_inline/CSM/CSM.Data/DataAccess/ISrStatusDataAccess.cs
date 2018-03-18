using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Data.DataAccess
{
    public interface ISrStatusDataAccess
    {
        List<SRStatusEntity> GetSrStatusList();
    }
}
