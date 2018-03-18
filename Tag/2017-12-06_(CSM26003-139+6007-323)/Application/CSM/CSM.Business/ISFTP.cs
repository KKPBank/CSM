using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Business
{
    public interface ISFTP
    {
        bool DownloadFiles();
        bool DeleteDownloadedFiles();
    }
}
