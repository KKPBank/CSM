using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;
using CSM.Entity.Common;

namespace CSM.Entity
{
    [Serializable]
    public class ExportSREntity
    {
        public string No { get; set; }
        public int? TotalSla { get; set; }
        public int? CurrentAlert { get; set; }  
        public string CustomerFistname { get; set; }
        public string CustomerLastname { get; set; }
        public string CardNo { get; set; }
        public string AccountNo { get; set; }
        public string CarRegisNo { get; set; }
        public string SRNo { get; set; }
        public string CreatorBranch { get; set; }
        public string ChannelName { get; set; }
        public string CallId { get; set; }
        public string ANo { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public string CampaignServiceName { get; set; }
        public string SRStatusName { get; set; }
        public DateTime? CloseDate { get; set; }
        public string CloseDateDisplay
        {
            get
            {
                return CloseDate.HasValue ? CloseDate.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime) : "";
            }
        }

        public string UpdateDateOwnerDisplay
        {
            get
            {
                return UpdateDateOwner.HasValue ? UpdateDateOwner.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime) : "";
            }
        }

        public string UpdateDelegateDisplay
        {
            get
            {
                return UpdateDelegate.HasValue ? UpdateDelegate.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime) : "";
            }
        }

        public string CreateDateDisplay
        {
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime) : "";
            }
        }

        public string SRIsverifyPass { get; set; }
        public string SRIsverifyPassDisplay
        {
            get
            {
                return Constants.ReportSRStatus.GetMessage(SRIsverifyPass);
            }
        }


        public string CreatorName { get; set; }
        public DateTime? CreateDate { get; set; }
        public string OwnerName { get; set; }
        public DateTime? UpdateDateOwner { get; set; }
        public string DelegatorName { get; set; }
        public DateTime? UpdateDelegate { get; set; }
        public string SRSubject { get; set; }
        public string SRRemark { get; set; }
        public string ContactName { get; set; }
        public string ContactSurname { get; set; }
        public string Relationship { get; set; }
        public string ContactNo { get; set; }
        public string MediaSourceName { get; set; }
        public string JobType { get; set; }
        public string TypeName { get; set; }
        public string AreaName { get; set; }
        public string SubAreaName { get; set; }
        public string AttachFile { get; set; }

        public string SRRemarkDisplay
        {
            get { return ApplicationHelpers.RemoveAllHtmlTags(this.SRRemark); }
        }
    }

    public class ExportSRSearchFilter : Pager
    {
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FirstName { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string LastName { get; set; }

        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardId { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AccountNo { get; set; }

        public string ProductGroup { get; set; }
        public string Type { get; set; }
        public string SRStatus { get; set; }
        public string Product { get; set; }
        public string Area { get; set; }
        public string Campaign { get; set; }
        public string SubArea { get; set; }
        public string Sla { get; set; }
        public string SRChannel { get; set; }
        public string SRDateFrom { get; set; }
        public string SRDateTo { get; set; }
        public DateTime? SRDateFromValue { get { return SRDateFrom.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        public DateTime? SRDateToValue { get { return SRDateTo.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }

        [LocalizedRegex("^([1-9]|[0-1][0-9]|[2][0-3]):([0-5][0-9])$", "ValErr_InvalidTimeFormat")]
        public string SRTimeFrom { get; set; }

        [LocalizedRegex("^([1-9]|[0-1][0-9]|[2][0-3]):([0-5][0-9])$", "ValErr_InvalidTimeFormat")]
        public string SRTimeTo { get; set; }
        public string OwnerSR { get; set; }
        public string OwnerBranch { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Subject { get; set; }

        public string CreatorSR { get; set; }
        public string CreatorBranch { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Description { get; set; }

        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SRId { get; set; }

        public DateTime? SRDateTimeFromValue
        {
            get
            {
                string strTime = !string.IsNullOrEmpty(SRTimeFrom) ? (SRTimeFrom + ":00") : "00:00:00";
                return (SRDateFrom + ' ' + strTime).ParseDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
            }
        }

        public DateTime? SRDateTimeToValue
        {
            get
            {
                string strTime = !string.IsNullOrEmpty(SRTimeTo) ? (SRTimeTo + ":59") : "23:59:59";
                return (SRDateTo + ' ' + strTime).ParseDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
            }
        }
    }
}
