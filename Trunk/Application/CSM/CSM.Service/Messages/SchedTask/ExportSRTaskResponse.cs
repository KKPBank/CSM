using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSM.Service.Messages.Common;
using CSM.Common.Utilities;

namespace CSM.Service.Messages.SchedTask
{
    public class ExportSRTaskResponse
    {
        public DateTime SchedDateTime { get; set; }
        public long ElapsedTime { get; set; }
        public int NumOfActivity { get; set; }
        public StatusResponse StatusResponse { get; set; }
        public string TaskAction { get; set; }
        public bool TransferStatus { get; set; }
        public string ReportType { get { return (TaskAction == Constants.AuditAction.ExportSRAccum ? "Accumulate" : "Daily"); } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            if (StatusResponse.Status == Constants.StatusResponse.Success)
            {
                sb.Append($"Success to export SR Report {ReportType}\n");
                sb.Append(string.Format("วันที่ {0} \n", SchedDateTime));
                sb.Append(string.Format("ElapsedTime  = {0} (ms)\n", ElapsedTime));
                sb.Append(string.Format("Total export data = {0} records.\n", NumOfActivity));
                if (!TransferStatus)
                {
                    sb.AppendLine();
                    sb.AppendLine($"Transfer Fail: {StatusResponse.Description}");
                }
                else
                {
                    sb.AppendLine();
                    sb.AppendLine("Transfer Success.");
                }
            }
            else
            {
                sb.AppendFormat("Error Message = {0}\n", StatusResponse.Description);
            }
            return sb.ToString();
        }

        public string ToHtml()
        {
            StringBuilder sb = new StringBuilder("");
            if (StatusResponse.Status == Constants.StatusResponse.Success)
            {
                sb.AppendLine($"<span class='text-info'>Success to export SR Report {ReportType}<br />");
                sb.AppendLine(string.Format("วันที่ {0} <br />", SchedDateTime));
                sb.AppendLine(string.Format("ElapsedTime  = {0} (ms)<br />", ElapsedTime));
                sb.AppendLine(string.Format("Total export data = {0} records</span><br />", NumOfActivity));
                if (!TransferStatus)
                {
                    sb.AppendLine("<br />");
                    sb.AppendLine($"<span class='text-danger'>Transfer Fail: {StatusResponse.Description}</span>");
                }
                else
                {
                    sb.AppendLine("<br />");
                    sb.AppendLine("<span class='text-info'>Transfer Success.</span>");
                }
            }
            else
            {
                sb.AppendFormat("Error Message = {0}<br />", StatusResponse.Description);
            }
            return sb.ToString();
        }
    }
}
