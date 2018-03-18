using System;
using System.Collections.Generic;
using System.Text;
using CSM.Common.Utilities;
using CSM.Service.Messages.Common;

namespace CSM.Service.Messages.SchedTask
{
    public class ExportAFSTaskResponse
    {
        public DateTime SchedDateTime { get; set; }
        public long ElapsedTime { get; set; } 
        public int NumOfActivity { get; set; } 
        public StatusResponse StatusResponse { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append(string.Format("วันที่ {0} \n", SchedDateTime));
            sb.Append(string.Format("ElapsedTime  = {0} (ms)\n", ElapsedTime));
            sb.Append(string.Format("Total export data = {0} records\n", NumOfActivity));
            return sb.ToString();
        }
    }
}
