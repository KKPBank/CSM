using System;
using System.Collections.Generic;
using System.Text;
using CSM.Common.Utilities;
using CSM.Service.Messages.Common;

namespace CSM.Service.Messages.SchedTask
{
    public class ImportHpTaskResponse
    {
        public DateTime SchedDateTime { get; set; }
        public long ElapsedTime { get; set; }
        public List<object> FileList { get; set; }
        public int NumOfTotal { get; set; }
        public int NumOfComplete { get; set; }
        public int NumOfError { get; set; }
        public StatusResponse StatusResponse { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append(string.Format("Reading files = {0}\n", StringHelpers.ConvertListToString(FileList, "/")));
            sb.Append(string.Format("Total Records = {0} records\n", NumOfTotal));
            sb.Append(string.Format("Total complete records = {0} records\n", NumOfComplete));
            sb.Append(string.Format("Total error records = {0} records\n", NumOfError));
            return sb.ToString();
        }
    }
}
