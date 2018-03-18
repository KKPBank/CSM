using System;
using CSM.Common.Utilities;
using CSM.Entity.Common;
using System.ComponentModel;

namespace CSM.Entity
{
    [Serializable]
    public class ExportVerifyEntity
    {
        [DisplayName("No")]
        public string No { get; set; }

        [DisplayName("SR ID")]
        public string SRId { get; set; }
        [DisplayName("เลขที่สัญญา / เลขที่บัญชีชื่อลูกค้า")]
        public string AccountNo { get; set; }
        [DisplayName("ชื่อลูกค้า")]
        public string CustomerFistname { get; set; }
        [DisplayName("นามสกุลลูกค้า")]
        public string CustomerLastname { get; set; }
        [DisplayName("SR Owner")]
        public string SROwnerName { get; set; }
        [DisplayName("SR Created Date Time")]
        public DateTime? SRCreateDate { get; set; }
        //public string SRCreateDateDisplay
        //{
        //    get
        //    {
        //        return SRCreateDate.HasValue ? SRCreateDate.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime) : "";
        //    }
        //}
        [DisplayName("SR Creator Branch")]
        public string SRCreatorBranch { get; set; }
        [DisplayName("Product Group")]
        public string ProductGroupName { get; set; }
        [DisplayName("Product")]
        public string ProductName { get; set; }
        [DisplayName("Campaign/Service")]
        public string CampaignServiceName { get; set; }
        [DisplayName("Type")]
        public string TypeName { get; set; }
        [DisplayName("Area")]
        public string AreaName { get; set; }
        [DisplayName("Sub-Area")]
        public string SubAreaName { get; set; }
        [DisplayName("Subject")]
        public string SRSubject { get; set; }
        [DisplayName("SR Description")]
        public string SRDescription { get; set; }
        [DisplayName("SR Status")]
        public string SRStatus { get; set; }
        [DisplayName("Verify Result")]
        public string IsVerifyResult { get; set; }
        //public string IsVerifyResultDisplay
        //{
        //    get
        //    {
        //        return Constants.ReportSRStatus.GetMessage(IsVerifyResult);
        //    }
        //}
        [DisplayName("Total Question")]
        public int TotalQuestion { get; set; }
        [DisplayName("Total Passed")]
        public int TotalPass { get; set; }
        [DisplayName("Total Failed")]
        public int TotalFailed { get; set; }
        [DisplayName("Total disregarded")]
        public int TotalDisregard { get; set; }

        //public string SRDescDisplay
        //{
        //    get { return ApplicationHelpers.RemoveAllHtmlTags(this.SRDescription); }
        //}
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
    }
}
