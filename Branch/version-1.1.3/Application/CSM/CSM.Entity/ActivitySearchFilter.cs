using System;
using CSM.Common.Utilities;
using CSM.Entity.Common;

namespace CSM.Entity
{
    [Serializable]
    public class ActivitySearchFilter : Pager
    {
        public ActivitySearchFilter()
        {
            SrOnly = false;
        }

        #region "Local Declaration"

        private int m_IsConnect = 1;

        #endregion

        public int IsConnect
        {
            get { return m_IsConnect; }
            set { m_IsConnect = value; }
        }
        public string JsonActivities { get; set; }
        public string CardNo { get; set; }
        public string SubsTypeCode { get; set; }
        public string ActivityStartDateTime { get; set; }

        public DateTime ActivityStartDateTimeValue
        {
            get { return ActivityStartDateTime.ParseDateTime(Constants.DateTimeFormat.DefaultFullDateTime).Value; }
        }

        public string ActivityEndDateTime { get; set; }

        public DateTime ActivityEndDateTimeValue
        {
            get { return ActivityEndDateTime.ParseDateTime(Constants.DateTimeFormat.DefaultFullDateTime).Value; }
        }

        public int? CustomerId { get; set; }
        public string SrNo { get; set; }
        public bool SrOnly { get; set; }
    }
}
