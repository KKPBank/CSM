using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Entity.Common
{
    public class SFtpConfig
    {
        public string SFtp_Host { get; set; }
        public string SFtp_UserName { get; set; }
        public string SFtp_Password { get; set; }
        public int SFtp_Port { get; set; }
        public string RemoteDir { get; set; }
    }
}
