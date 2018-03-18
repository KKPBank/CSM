using System;
using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Business
{
    public interface ISrChannelFacade : IDisposable
    {
        List<SrChannelEntity> GetSrChannelList();
    }
}
