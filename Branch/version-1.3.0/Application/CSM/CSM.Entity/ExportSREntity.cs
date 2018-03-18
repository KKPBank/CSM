using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;
using CSM.Entity.Common;
using System.ComponentModel;

namespace CSM.Entity
{
    [Serializable]
    public class ExportSREntity
    {
        [DisplayName("No")]
        public string No { get; set; }

        [DisplayName("จำนวนครั้งที่เกิน SLA ทั้งหมด")]
        public int? TotalSla { get; set; }

        [DisplayName("แจ้งเตือนครั้งที่")]
        public int? CurrentAlert { get; set; }

        [DisplayName("ชื่อลูกค้า")]
        public string CustomerFistname { get; set; }

        [DisplayName("นามสกุล")]
        public string CustomerLastname { get; set; }

        [DisplayName("Subscription ID")]
        public string CardNo { get; set; }

        [DisplayName("เลขที่สัญญา / เลขที่บัญชี")]
        public string AccountNo { get; set; }

        [DisplayName("ทะเบียนรถ")]
        public string CarRegisNo { get; set; }

        [DisplayName("SR ID")]
        public string SRNo { get; set; }

        [DisplayName("SR Creator Branch")]
        public string CreatorBranch { get; set; }

        [DisplayName("Channel")]
        public string ChannelName { get; set; }

        [DisplayName("Call ID")]
        public string CallId { get; set; }

        [DisplayName("Tell (A-Number)")]
        public string ANo { get; set; }

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

        [DisplayName("SR Status")]
        public string SRStatusName { get; set; }

        [DisplayName("Closed Date Time")]
        public DateTime? CloseDate { get; set; }

        //public string CloseDateDisplay
        //{
        //    get
        //    {
        //        return CloseDate.HasValue ? CloseDate.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime) : "";
        //    }
        //}

        //public string UpdateDateOwnerDisplay
        //{
        //    get
        //    {
        //        return UpdateDateOwner.HasValue ? UpdateDateOwner.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime) : "";
        //    }
        //}

        //public string UpdateDelegateDisplay
        //{
        //    get
        //    {
        //        return UpdateDelegate.HasValue ? UpdateDelegate.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime) : "";
        //    }
        //}

        //public string CreateDateDisplay
        //{
        //    get
        //    {
        //        return CreateDate.HasValue ? CreateDate.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime) : "";
        //    }
        //}

        [DisplayName("Verify Result")]
        public string SRIsverifyPass { get; set; }

        //public string SRIsverifyPassDisplay
        //{
        //    get
        //    {
        //        return Constants.ReportSRStatus.GetMessage(SRIsverifyPass);
        //    }
        //}

        [DisplayName("SR Creator")]
        public string CreatorName { get; set; }

        [DisplayName("SR Created Date Time")]
        public DateTime? CreateDate { get; set; }

        [DisplayName("SR Owner")]
        public string OwnerName { get; set; }

        [DisplayName("Owner Updated Date Time")]
        public DateTime? UpdateDateOwner { get; set; }

        [DisplayName("SR Delegate")]
        public string DelegatorName { get; set; }

        [DisplayName("Delegate Updated Date Time")]
        public DateTime? UpdateDelegate { get; set; }

        [DisplayName("Subject")]
        public string SRSubject { get; set; }

        [DisplayName("SR Description")]
        public string SRRemark { get; set; }

        [DisplayName("ชื่อผู้ติดต่อ")]
        public string ContactName { get; set; }

        [DisplayName("นามสกุลผู้ติดต่อ")]
        public string ContactSurname { get; set; }

        [DisplayName("ความสัมพันธ์")]
        public string Relationship { get; set; }

        [DisplayName("เบอร์ติดต่อ")]
        public string ContactNo { get; set; }

        [DisplayName("media source")]
        public string MediaSourceName { get; set; }
        //public string JobType { get; set; }
        //public string AttachFile { get; set; }

        //public string SRRemarkDisplay
        //{
        //    get { return ApplicationHelpers.RemoveAllHtmlTags(this.SRRemark); }
        //}
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
