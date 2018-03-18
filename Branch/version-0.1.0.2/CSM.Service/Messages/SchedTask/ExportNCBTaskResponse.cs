using CSM.Service.Messages.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Service.Messages.SchedTask
{
    public class ExportNCBTaskResponse
    {
        public DateTime SchedDateTime { get; set; }
        public long ElapsedTime { get; set; }
        public int NumOfNew { get; set; }
        public int NumOfUpdate { get; set; }
        public StatusResponse StatusResponse { get; set; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append(string.Format("วันที่ {0} \n", SchedDateTime));
            sb.Append(string.Format("ElapsedTime  = {0} (ms)\n", ElapsedTime));
            sb.Append(string.Format("Total export New Employee = {0} records\n", NumOfNew));
            sb.Append(string.Format("Total export Update Employee = {0} records\n", NumOfUpdate));
            return sb.ToString();
        }
    }
}
