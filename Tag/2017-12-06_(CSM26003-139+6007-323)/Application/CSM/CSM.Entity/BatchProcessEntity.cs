using System.Globalization;
using System.Web.Mvc;
using CSM.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Entity
{
    [Serializable]
    public class BatchProcessEntity
    {
        public int? ProcessId { get; set; }
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }
        public int? Status { get; set; }
        public DateTime?  StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan? ProcessTime { get; set; }
        public bool? IsRerunable { get; set; }
        public string Detail { get; set; }

        [AllowHtml]
        public string DetailDisplay
        {
            get
            {
                string content = "";

                if (!string.IsNullOrWhiteSpace(this.Detail))
                {
                    content = this.Detail.TextToHtml();
                }

                return content;
            }
        }

        public string StartTimeDisplay 
        {
            get { return StartTime.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime); }
        }

        public string EndTimeDisplay
        {
            get { return EndTime.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime); }
        }

        public string ProcessTimeDisplay
        {
            get
            {
                if (ProcessTime.HasValue)
                {
                    return ProcessTime.Value.TotalMinutes.ToString("##0.0000", CultureInfo.InvariantCulture);
                }

                return "";

            }
        }

        public string StatusDisplay
        {
            get
            {
                string strResult = "";

                if (Status.HasValue)
                {
                    if (Status.Value == Constants.BatchProcessStatus.Fail)
                    {
                        strResult = "Fail";
                    }
                    else if (Status.Value == Constants.BatchProcessStatus.Success)
                    {
                        strResult = "Success";
                    }
                    else if (Status.Value == Constants.BatchProcessStatus.Processing)
                    {
                        strResult = "Processing";
                    }
                }

                return strResult;

            }
        }


    }
}
