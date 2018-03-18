using System;
using System.Collections.Generic;
using System.Text;
using CSM.Service.Messages.Common;
using CSM.Common.Utilities;

namespace CSM.Service.Messages.SchedTask
{
    public class ImportHRTaskResponse
    {
        public DateTime SchedDateTime { get; set; }
        public long ElapsedTime { get; set; }
        public List<object> FileList { get; set; }
        public int ReadRow { get; set; }
        public int ReadErrorRow { get; set; }

        public int EmpInsert { get; set; }
        public int EmpUpdate { get; set; }
        public int EmpMarkDelete { get; set; }

        public int BUInsert { get; set; }
        public int BUUpdate { get; set; }
        public int BUMarkDelete { get; set; }

        public int BRInsert { get; set; }
        public int BRUpdate { get; set; }
        public int BRMarkDelete { get; set; }

        public StatusResponse StatusResponse { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"วันที่ {SchedDateTime.ToString("dd/MM/yyyy HH:mm:ss")} ElapsedTime {ElapsedTime.ToString("f0")} (ms).\r\n");
            sb.Append($"Reading files = {StringHelpers.ConvertListToString(FileList, ",")}.\r\n");
            sb.Append($"Total Read {ReadRow} record\r\n");
            sb.Append($"Total Read error {ReadErrorRow} record\r\n");

            sb.Append($"HR Employee insert {EmpInsert} record\r\n");
            sb.Append($"HR Employee update {EmpUpdate} record\r\n");
            sb.Append($"HR Employee mark delete {EmpMarkDelete} record(s).\r\n");

            sb.Append($"HR BU insert {BUInsert} record\r\n");
            sb.Append($"HR BU update {BUUpdate} record\r\n");
            sb.Append($"HR BU mark delete {BUMarkDelete} record\r\n");

            sb.Append($"HR BRANCH insert {BRInsert} record\r\n");
            sb.Append($"HR BRANCH update {BRUpdate} record\r\n");
            sb.Append($"HR BRANCH mark delete {BRMarkDelete} record\r\n");

            return sb.ToString();
        }
    }
}