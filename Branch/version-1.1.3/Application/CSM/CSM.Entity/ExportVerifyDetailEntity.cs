using System;
using CSM.Common.Utilities;
using CSM.Entity.Common;

namespace CSM.Entity
{
    [Serializable]
    public class ExportVerifyDetailEntity
    {
        public string No { get; set; }
        public string SRId { get; set; }
        public string AccountNo { get; set; }
        public string CustomerFistname { get; set; }
        public string CustomerLastname { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public string CampaignServiceName { get; set; }
        public string TypeName { get; set; }
        public string AreaName { get; set; }
        public string SubAreaName { get; set; }
        public string SROwnerName { get; set; }
        public string IsVerifyPass { get; set; }
        public string IsVerifyPassDisplay
        {
            get
            {
                return Constants.ReportSRStatus.GetMessage(IsVerifyPass);
            }
        }
        public string QuestionGroupName { get; set; }
        public string QuestionName { get; set; }
        public string VerifyResult { get; set; }
        public string VerifyResultDisplay 
        {
            get
            {
                return Constants.ReportVerify.GetMessage(VerifyResult);
            }
        }
    }

    public class ExportVerifyDetailSearchFilter : Pager
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
