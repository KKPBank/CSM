using System;
using CSM.Entity.Common;
using CSM.Common.Resources;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class NewsEntity
    {
        public int? NewsId { get; set; }
        public string Topic { get; set; }
        public string Content { get; set; }
        public DateTime? AnnounceDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public short? Status { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? CreateUserId { get; set; }
        public int? UpdateUserId { get; set; }        
        public string AnnounceDateDisplay
        {
            get
            {
                return AnnounceDate.HasValue ? AnnounceDate.Value.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate) : "";
            }
        }

        public string ExpiryDateDisplay
        {
            get
            {
                return ExpiryDate.HasValue ? ExpiryDate.Value.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate) : "";
            }
        }
               
        public string StatusDisplay
        {
            get
            {
                string sts = "";
                if(this.Status.HasValue)
                {
                    if (this.Status.Value.Equals(1)) sts = Resource.Ddl_Status_Active;
                    else if (this.Status.Value.Equals(0)) sts = Resource.Ddl_Status_Inactive;
                }

                return sts;
            }
        }

        public UserEntity CreateUser { get; set; }
        public UserEntity UpdateUser { get; set; }

        public string DocumentFolder { get; set; }
    }

    public class NewsBranchEntity : BranchEntity
    {
        public int? NewsId { get; set; }
    }

    [Serializable]
    public class NewsSearchFilter : Pager
    {
         [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Topic { get; set; }        
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public DateTime? AnnounceDate { get { return DateFrom.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        public DateTime? ExpiryDate { get { return DateTo.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        public string Status { get; set; }

        public int? UserId { get; set; }
    }
}
