using System;
using CSM.Common.Utilities;
using CSM.Entity.Common;
using System.ComponentModel;

namespace CSM.Entity
{
    [Serializable]
    public class ExportNcbEntity
    {
        [DisplayName("No")]
        public string No { get; set; }

        [DisplayName("จำนวนครั้งที่เกิน SLA ทั้งหมด")]
        public string Sla { get; set; }

        [DisplayName("ชื่อลูกค้า")]
        public string CustomerFistname { get; set; }

        [DisplayName("นามสกุลลูกค้า")]
        public string CustomerLastname { get; set; }

        [DisplayName("Subscription ID")]
        public string CardNo { get; set; }
        //public int? CustomerTypeId { get; set; }

        //public string CustomerType
        //{
        //    get { return Constants.CustomerType.GetMessage(this.CustomerTypeId); }
        //}

        [DisplayName("วันเกิด/วันจดทะเบียน")]
        public DateTime? CustomerBirthDate { get; set; }

        //public string CustomerBirthDateDisplay
        //{
        //    get { return CustomerBirthDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        //}

        [DisplayName("NCB Check Status")]
        public string NcbCheckStatus { get; set; }

        [DisplayName("SR ID")]
        public string SRId { get; set; }

        [DisplayName("SR Status")]
        public string SRStatus { get; set; }

        [DisplayName("Product Group")]
        public string ProductGroupName { get; set; }

        [DisplayName("Product")]
        public string ProductName { get; set; }

        [DisplayName("Campaign")]
        public string CampaignName { get; set; }

        [DisplayName("Type")]
        public string TypeName { get; set; }

        [DisplayName("Area")]
        public string AreaName { get; set; }

        [DisplayName("Sub-Area")]
        public string SubAreaName { get; set; }

        [DisplayName("SR Creator")]
        public string SRCreator { get; set; }

        [DisplayName("SR Created Date Time")]
        public DateTime? SRCreateDate { get; set; }

        //public string SRCreateDateDisplay
        //{
        //    get { return SRCreateDate.FormatDateTime(Constants.DateTimeFormat.ReportDateTime); }
        //}

        [DisplayName("SR Owner")]
        public string SROwner { get; set; }

        [DisplayName("Owner Updated Date Time")]
        public DateTime? OwnerUpdate { get; set; }

        //public string OwnerUpdateDisplay
        //{
        //    get { return OwnerUpdate.FormatDateTime(Constants.DateTimeFormat.ReportDateTime); }
        //}

        [DisplayName("SR Delegate")]
        public string SRDelegate { get; set; }

        [DisplayName("Delegate Updated Date Time")]
        public DateTime? SRDelegateUpdate { get; set; }
        //public string SRDelegateUpdateDisplay
        //{
        //    get { return SRDelegateUpdate.FormatDateTime(Constants.DateTimeFormat.ReportDateTime); }
        //}

        [DisplayName("Marketing Upper Branch1")]
        public string MKTUpperBranch1 { get; set; }

        [DisplayName("Marketing Upper Branch2")]
        public string MKTUpperBranch2 { get; set; }

        [DisplayName("MKT Employee Branch")]
        public string MKTEmployeeBranch { get; set; }

        [DisplayName("MKT Employee Name")]
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
         
    }
}
