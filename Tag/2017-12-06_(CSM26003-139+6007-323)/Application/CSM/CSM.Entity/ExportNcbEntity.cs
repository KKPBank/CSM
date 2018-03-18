using System;
using CSM.Common.Utilities;
using CSM.Entity.Common;

namespace CSM.Entity
{
    [Serializable]
    public class ExportNcbEntity
    {
        public string No { get; set; }
        public string Sla { get; set; }
        public string CustomerFistname { get; set; }
        public string CustomerLastname { get; set; }
        public string CardNo { get; set; } 
        public int? CustomerTypeId { get; set; }

        public string CustomerType
        {
            get { return Constants.CustomerType.GetMessage(this.CustomerTypeId); }
        }

        public DateTime? CustomerBirthDate { get; set; }

        public string CustomerBirthDateDisplay
        {
            get { return CustomerBirthDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }
        public string NcbCheckStatus { get; set; }
        public string SRId { get; set; }
        public string SRStatus { get; set; }
        public string SRState { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public string CampaignName { get; set; }
        public string TypeName { get; set; }
        public string AreaName { get; set; }
        public string SubAreaName { get; set; }
        public string SRCreator { get; set; }
        public DateTime? SRCreateDate { get; set; }
        public string SRCreateDateDisplay
        {
            get { return SRCreateDate.FormatDateTime(Constants.DateTimeFormat.ReportDateTime); }
        }
        public string SROwner { get; set; }
        public DateTime? OwnerUpdate { get; set; }
        public string OwnerUpdateDisplay
        {
            get { return OwnerUpdate.FormatDateTime(Constants.DateTimeFormat.ReportDateTime); }
        }
        public string SRDelegate { get; set; }
        public DateTime? SRDelegateUpdate { get; set; }
        public string SRDelegateUpdateDisplay
        {
            get { return SRDelegateUpdate.FormatDateTime(Constants.DateTimeFormat.ReportDateTime); }
        }
        public string MKTUpperBranch1 { get; set; }
        public string MKTUpperBranch2 { get; set; }
        public string MKTEmployeeBranch { get; set; }
        public string MKTEmployeeName { get; set; }
    }

    public class ExportNcbSearchFilter : Pager
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
        public string BirthDate { get; set; }
        public DateTime? BirthDateValue { get { return BirthDate.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        public string ProductGroup { get; set; }
        public string Product { get; set; }
        public string Campaign { get; set; }
        public string Type { get; set; }
        public string Area { get; set; }
        public string SubArea { get; set; }
        public string OwnerSR { get; set; }
        public string OwnerBranch { get; set; }
        public string CustomerType { get; set; }
        public string Sla { get; set; }
        public string UpperBranch { get; set; }
        public string CreatorBranch { get; set; }
        public string CreatorSR { get; set; }
        public string DelegateBranch { get; set; }
        public string DelegateSR { get; set; }
        public string SRStatus { get; set; }

        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SRId { get; set; }
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
        public string SlaName { get; set; }
        public string CampaignName { get; set; }
        public string SRStatusName { get; set; }
        public string TypeName { get; set; }
        public string AreaName { get; set; }
        public string SubAreaName { get; set; }
        public string UpperBranchName { get; set; }
        public string OwnerSRName { get; set; }
        public string OwnerBranchName { get; set; }
        public string CreatorBranchName { get; set; }
        public string CreatorSRName { get; set; }
        public string DelegateBranchName { get; set; }
        public string DelegateSRName { get; set; }
    }
}
