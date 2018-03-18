using System;
using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Business
{
    public interface ISrPageFacade : IDisposable
    {
        List<SrPageItemEntity> GetSrPageList();
    }
}
