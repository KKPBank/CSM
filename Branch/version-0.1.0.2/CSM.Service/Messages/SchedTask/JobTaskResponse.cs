using System;
using System.Collections.Generic;
using System.Text;
using CSM.Service.Messages.Common;

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
        public StatusResponse StatusResponse { get; set; }

        public int NumFailedDelete
        {
            get { return m_NumFailedDelete; }
            set { m_NumFailedDelete = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append(string.Format("Username = {0}\n", Username));
            sb.Append(string.Format("Total emailread = {0}\n", TotalEmailRead));
            sb.Append(string.Format("Found SR = {0}\n", NumOfSR));
            sb.Append(string.Format("Job type fax = {0}\n", NumOfFax));
            sb.Append(string.Format("Job type KK web site = {0}\n", NumOfKKWebSite));
            sb.Append(string.Format("Job type email = {0}\n", NumOfEmail));
            sb.Append(string.Format("Cannot delete mail messages = {0}\n", NumFailedDelete));
            return sb.ToString();
        }
    }
}
