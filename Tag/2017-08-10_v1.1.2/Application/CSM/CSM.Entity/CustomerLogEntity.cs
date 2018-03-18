using System;
using CSM.Common.Utilities;
using CSM.Entity.Common;

namespace CSM.Entity
{
    [Serializable]
    public class CustomerLogEntity
    {
        public int? LogId { get; set; }
        public int? CustomerId { get; set; }
        public string Detail { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedDateDisplay
        {
            get { return CreatedDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime); }
        }
        public UserEntity User { get; set; }
    }

    [Serializable]
    public class CustomerLogSearchFilter : Pager
    {
        public int? CustomerId { get; set; }
    }
}
