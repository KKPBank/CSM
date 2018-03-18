using System;
using System.Collections.Generic;
using System.Text;
using CSM.Common.Utilities;
using CSM.Service.Messages.Common;

namespace CSM.Service.Messages.SchedTask
{
    public class ImportAFSTaskResponse
    {
        public DateTime SchedDateTime { get; set; }
        public long ElapsedTime { get; set; }
        public List<object> FileList { get; set; }
        public int NumOfProp { get; set; }
        public int NumOfSaleZones { get; set; }
        public int NumOfComplete { get; set; }
        public int NumOfErrProp { get; set; }
        public int NumOfErrSaleZone { get; set; }
        public StatusResponse StatusResponse { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append(string.Format("Reading files = {0}\n", StringHelpers.ConvertListToString(FileList, "/")));
            sb.Append(string.Format("Total property = {0} records\n", NumOfProp));
            sb.Append(string.Format("Total sale zone = {0} records\n", NumOfSaleZones));
            sb.Append(string.Format("Total mapping records = {0} records\n", NumOfComplete));
            sb.Append(string.Format("Total property error records = {0} records\n", NumOfErrProp));
            sb.Append(string.Format("Total sale zone error records = {0} records\n", NumOfErrSaleZone));
            return sb.ToString();
        }
    }
}
