using System;
using CSM.Common.Utilities;
using CSM.Entity.Common;

namespace CSM.Entity
{
    [Serializable]
    public class ExportVerifyEntity
    {
        public string No { get; set; }
        public string SRId { get; set; }
        public string AccountNo { get; set; }
        public string CustomerFistname { get; set; }
        public string CustomerLastname { get; set; }
        public string SROwnerName { get; set; }
        public DateTime? SRCreateDate { get; set; }
        public string SRCreateDateDisplay
        {
            get
            {
                return SRCreateDate.HasValue ? SRCreateDate.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime) : "";
            }
        }
        public string SRCreatorBranch { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public string CampaignServiceName { get; set; }
        public string TypeName { get; set; }
        public string AreaName { get; set; }
        public string SubAreaName { get; set; }
        public string SRSubject { get; set; }
        public string SRDescription { get; set; }
        public string SRState { get; set; }
        public string SRStatus { get; set; }        
        public string IsVerifyResult { get; set; }
        public string IsVerifyResultDisplay
        {
            get
            {
                return Constants.ReportSRStatus.GetMessage(IsVerifyResult);
            }
        }
        public int TotalQuestion { get; set; }
        public int TotalPass { get; set; }
        public int TotalFailed { get; set; }
        public int TotalDisregard { get; set; }

        public string SRDescDisplay
        {
            get { return ApplicationHelpers.RemoveAllHtmlTags(this.SRDescription); }
        }
    }

    public class ExportVerifySearchFilter : Pager
    {
        public string ProductGroup { get; set; }
        public string Product { get; set; }
        public string Campaign { get; set; }
        public string Type { get; set; }        
        public string Area { get; set; }
        public string SubArea { get; set; }
        public string OwnerSR { get; set; }
        public string OwnerBranch { get; set; }

        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SRId { get; set; }
        public string SRIsverify { get; set; }
        public string SRDateFrom { get; set; }
        public string SRDateTo { get; set; }
        public DateTime? SRDateFromValue { get { return SRDateFrom.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        public DateTime? SRDateToValue { get { return SRDateTo.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
         [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Description { get; set; }
         [LocalizedRegex("^([1-9]|[0-1][0-9]|[2][0-3]):([0-5][0-9])$", "ValErr_InvalidTimeFormat")]
         public string SRTimeFrom { get; set; }

         [LocalizedRegex("^([1-9]|[0-1][0-9]|[2][0-3]):([0-5][0-9])$", "ValErr_InvalidTimeFormat")]
         public string SRTimeTo { get; set; }
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

        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public string CampaignName { get; set; }
        public string TypeName { get; set; }
        public string AreaName { get; set; }
        public string SubAreaName { get; set; }
        public string OwnerSRName { get; set; }
        public string OwnerBranchName { get; set; }
        public string SRIsverifyName { get; set; }
    }
}
