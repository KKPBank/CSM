using CSM.Service.Messages.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Service.Messages.SchedTask
{
    public class ExportCISTaskResponse
    {
        public DateTime SchedDateTime { get; set; }
        public long ElapsedTime { get; set; }
        public int NumOfCor { get; set; }
        public int NumOfIndiv { get; set; }
        public StatusResponse StatusResponse { get; set; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append(string.Format("วันที่ {0} \n", SchedDateTime));
            sb.Append(string.Format("ElapsedTime  = {0} (ms)\n", ElapsedTime));
            sb.Append(string.Format("Total export Corporate = {0} records\n", NumOfCor));
            sb.Append(string.Format("Total export Individual = {0} records\n", NumOfIndiv));
            return sb.ToString();
        }
    }
}
