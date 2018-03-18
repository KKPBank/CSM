using System;
using System.Collections.Generic;
using System.Text;
using CSM.Service.Messages.Common;
using CSM.Common.Utilities;

namespace CSM.Service.Messages.SchedTask
{
    public class JobTaskResponse
    {
        public DateTime SchedDateTime { get; set; }
        public long ElapsedTime { get; set; }
        public StatusResponse StatusResponse { get; set; }
        public List<JobTaskResult> JobTaskResults { get; set; }
    }

    public class JobTaskResult
    {
        #region "Local Declaration"

        private int m_NumFailedDelete = 0;

        #endregion

        public string Username { get; set; }
        public int TotalEmailRead { get; set; }
        public int NumOfSR { get; set; }
        public int NumOfFax { get; set; }
        public int NumOfKKWebSite { get; set; }
        public int NumOfEmail { get; set; }
        public long ElapsedTime { get; set; }
        public DateTime SchedDateTime { get; set; }
        public StatusResponse StatusResponse { get; set; }

        public int NumFailedDelete
        {
            get { return m_NumFailedDelete; }
            set { m_NumFailedDelete = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.AppendFormat("วันที่ {0} \n", SchedDateTime);
            sb.Append(string.Format("ElapsedTime = {0} (ms)\n", ElapsedTime));
            sb.Append(string.Format("Username = {0}\n", Username));
            if(StatusResponse != null && Constants.StatusResponse.Failed.Equals(StatusResponse.Status))
            {
                sb.Append(string.Format("Error = {0}\n", StatusResponse.Description));
            }
            sb.Append(string.Format("Total email read = {0} records\n", TotalEmailRead));
            sb.Append(string.Format("Found SR = {0} records\n", NumOfSR));
            sb.Append(string.Format("Job type fax = {0} records\n", NumOfFax));
            sb.Append(string.Format("Job type KK web site = {0} records\n", NumOfKKWebSite));
            sb.Append(string.Format("Job type email = {0} records\n", NumOfEmail));
            sb.Append(string.Format("Cannot delete mail messages = {0} records\n", NumFailedDelete));
            return sb.ToString();
        }
    }
}
