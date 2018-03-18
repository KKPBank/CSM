using System;
using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Business
{
    public interface ISrStatusFacade : IDisposable
    {
        List<SRStatusEntity> GetSrStatusList();
    }
}
