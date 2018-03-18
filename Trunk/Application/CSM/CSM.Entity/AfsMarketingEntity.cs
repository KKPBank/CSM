using CSM.Common.Utilities;
using System;

namespace CSM.Entity
{
    public class AfsMarketingEntity
    {
        public string EmpNum { get; set; }
        public string FstName { get; set; }
        public string LastName { get; set; } 
        public string PhoneNo { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatedDateDisplay
        {
            get { return CreateDate.HasValue ? CreateDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate) : string.Empty; }
        }
        public DateTime? UpdateDate { get; set; }

        public string UpdateDateDisplay
        {
            get { return UpdateDate.HasValue ? UpdateDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate) : string.Empty; }
        }
        public short? PhoneOrder { get; set; }

        public short? Status { get; set; }
        public string EmpStatus
        {
            get {  return Constants.EmployeeStatus.GetMessage(Status); }
        }

        public bool IsNew { get; set; }
        public int UserID { get; set; }
    }
}
