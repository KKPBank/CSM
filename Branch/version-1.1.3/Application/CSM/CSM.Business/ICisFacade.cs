using System;
using CSM.Service.Messages.SchedTask;

namespace CSM.Business
{
    public interface ICisFacade : IDisposable
    {
        ImportCISTaskResponse GetFileCIS(string username, string password, bool skipSftp, string strDate);
    }
}
