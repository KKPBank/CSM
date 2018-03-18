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
        public string SRStateName { get; set; }
        public string SRStatusName { get; set; }

        public string CloseUserName { get; set; }
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

        public string UpdaterName { get; set; }
        public DateTime? UpdateDate { get; set; }

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

        public string ReportType { get; set; } = "SR";
        public string SearchFilterDisplay { get; set; }

        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public string TypeName { get; set; }
        public string SRStatusName { get; set; }
        public string AreaName { get; set; }
        public string CampaignName { get; set; }
        public string SubAreaName { get; set; }
        public string SlaName { get; set; }
        public string SRChannelName { get; set; }
        public string OwnerSRName { get; set; }
        public string OwnerBranchName { get; set; }
        public string CreatorSRName { get; set; }
        public string CreatorBranchName { get; set; }
    }

    [Serializable]
    public class ExportSRComplaintEntity
    {
        [Export("ลำดับ")]
        public string No { get; set; }

        [Export("Total Alert")]
        public int? TotalSla { get; set; }

        public int? TotalWorkHour { get; set; }
        [Export("Total Working Hours")]
        public string TotalWorkHourDisplay { get { return TotalWorkHour.ToDayHourMinute(); } }

        [Export("This Alert")]
        public int? CurrentAlert { get; set; }

        public int? CurrentWorkHour { get; set; }
        [Export("Working Hours")]
        public string CurrentWorkHourDisplay { get { return CurrentWorkHour.ToDayHourMinute(); } }

        [Export("ชื่อลูกค้า")]
        public string CustomerFistname { get; set; }
        [Export("นามสกุล")]
        public string CustomerLastname { get; set; }
        [Export("รหัสบัตรประชาชน", IsNumberAsString = true)]
        public string CardNo { get; set; }
        [Export("เลขที่สัญญา / เลขที่บัญชี", IsNumberAsString = true)]
        public string AccountNo { get; set; }
        [Export("ทะเบียนรถ")]
        public string CarRegisNo { get; set; }
        [Export("SR ID", IsNumberAsString = true)]
        public string SRNo { get; set; }
        [Export("SR Creator Branch")]
        public string CreatorBranch { get; set; }
        [Export("Channel")]
        public string ChannelName { get; set; }
        [Export("Call ID", IsNumberAsString = true)]
        public string CallId { get; set; }
        [Export("Tell (A-Number)", IsNumberAsString = true)]
        public string ANo { get; set; }
        [Export("Product Group")]
        public string ProductGroupName { get; set; }
        [Export("Product")]
        public string ProductName { get; set; }
        [Export("Campiagn/Service")]
        public string CampaignServiceName { get; set; }
        [Export("Type")]
        public string TypeName { get; set; }
        [Export("Area")]
        public string AreaName { get; set; }
        [Export("Sub-Area")]
        public string SubAreaName { get; set; }
        [Export("SR Update by")]
        public string SRUpdateBy { get; set; }
        public DateTime? SRUpdateDate { get; set; }
        [Export("SR Update Date")]
        public string SRUpdateDateDisplay
        {
            get
            {
                return SRUpdateDate.HasValue ? SRUpdateDate.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime) : "";
            }
        }
        public DateTime? CloseDate { get; set; }
        [Export("Closed Date Time")]
        public string CloseDateDisplay
        {
            get
            {
                return CloseDate.HasValue ? CloseDate.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime) : "";
            }
        }
        [Export("Closed By")]
        public string ClosedBy { get; set; }

        public string SRIsverifyPass { get; set; }
        [Export("Verify Result")]
        public string SRIsverifyPassDisplay
        {
            get
            {
                return Constants.ReportSRStatus.GetMessage(SRIsverifyPass);
            }
        }

        public string VerifyOTPResult { get; set; }
        [Export("Verify Result OTP")]
        public string VerifyOTPResultDisplay
        {
            get
            {
                return Constants.ReportSRStatus.GetMessage(VerifyOTPResult);
            }
        }

        [Export("SR Creator")]
        public string CreatorName { get; set; }
        public DateTime? CreateDate { get; set; }
        [Export("SR Created Date Time")]
        public string CreateDateDisplay
        {
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime) : "";
            }
        }

        [Export("SR Owner")]
        public string OwnerName { get; set; }
        public DateTime? UpdateDateOwner { get; set; }
        [Export("Owner Updated Date Time")]
        public string UpdateDateOwnerDisplay
        {
            get
            {
                return UpdateDateOwner.HasValue ? UpdateDateOwner.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime) : "";
            }
        }

        [Export("SR Delegate")]
        public string DelegatorName { get; set; }
        public DateTime? UpdateDelegate { get; set; }
        [Export("Delegate Updated Date Time")]
        public string UpdateDelegateDisplay
        {
            get
            {
                return UpdateDelegate.HasValue ? UpdateDelegate.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime) : "";
            }
        }

        public string SRSubject { get; set; }
        [Export("Subject")]
        public string SRSubjectDisplay
        {
            get { return ApplicationHelpers.RemoveAllHtmlTags(SRSubject); }
        }
        public string SRRemark { get; set; }
        [Export("Remark")]
        public string SRRemarkDisplay
        {
            get { return ApplicationHelpers.RemoveAllHtmlTags(SRRemark); }
        }

        [Export("ชื่อผู้ติดต่อ")]
        public string ContactName { get; set; }
        [Export("นามสกุลผู้ติดต่อ")]
        public string ContactSurname { get; set; }
        [Export("ความสัมพันธ์")]
        public string Relationship { get; set; }
        [Export("เบอร์ติดต่อ", IsNumberAsString = true)]
        public string ContactNo { get; set; }
        [Export("Media Source")]
        public string MediaSourceName { get; set; }
        [Export("Attach File")]
        public string AttachFile { get; set; }
        [Export("Job Type")]
        public string JobType { get; set; }
        [Export("Product Group by Complaint")]
        public string CPNProductGroupName { get; set; }
        [Export("Product  by Complaint")]
        public string CPNProductName { get; set; }
        [Export("Campaign  by Complaint")]
        public string CPNCampaignServiceName { get; set; }
        [Export("หัวข้อ")]
        public string CPNSubjectName { get; set; }
        [Export("ประเภทการร้องเรียน")]
        public string CPNTypeName { get; set; }
        [Export("สาเหตุการร้องเรียน")]
        public string CPNRootCauseName { get; set; }
        [Export("ประเด็นร้องเรียน")]
        public string CPNIssuesName { get; set; }
        [Export("ความลับ")]
        public string CPNIsSecret { get; set; }
        [Export("กลุ่มธรกิจ")]
        public string CPNBUGroupName { get; set; }
        public string CPNTeamName { get; set; }
        public string CPNDeptName { get; set; }
        public string CPNSectName { get; set; }
        public string CPNBranchName { get; set; }
        [Export("Summary")]
        public string CPNIsSummary { get; set; }
        [Export("Root Cause Customer")]
        public string CPNCauseCustomer { get; set; }
        [Export("หากพบสาเหตุเป็น Customer")]
        public string CPNCauseCustomerDetail { get; set; }
        [Export("Root Cause System")]
        public string CPNCauseSystem { get; set; }
        [Export("หากพบสาเหตุเป็น System")]
        public string CPNCauseSystemDetail { get; set; }
        [Export("Root Cause Process")]
        public string CPNCauseProcess { get; set; }
        [Export("หากพบสาเหตุเป็น Process")]
        public string CPNCauseProcessDetail { get; set; }
        [Export("Root Cause Staff")]
        public string CPNCauseStaff { get; set; }
        [Export("หากพบสาเหตุเป็น Staff")]
        public string CPNCauseStaffDetail { get; set; }

        [Export("สรุปสาเหตุตามความเสี่ยง")]
        public string CPNCauseSummaryName { get; set; }
        [Export("สรุปข้อร้องเรียนตามความเสี่ยง")]
        public string CPNSummaryName { get; set; }
        [Export("สรุปการแก้ไขปัญหา")]
        public string CPNFixedDetail { get; set; }

        [Export("SR State Old")]
        public string SRStateNameOld { get; set; }
        [Export("SR Status Old")]
        public string SRStatusNameOld { get; set; }
        [Export("SR State New")]
        public string SRStateNameNew { get; set; }
        [Export("SR Status New")]
        public string SRStatusNameNew { get; set; }

        [Export("SR Owner Old")]
        public string OwnerNameOld { get; set; }
        [Export("SR Owner New")]
        public string OwnerNameNew { get; set; }
        [Export("SR Delegate Old")]
        public string DelegatorNameOld { get; set; }
        [Export("SR Delegate New")]
        public string DelegatorNameNew { get; set; }

        [Export("Activity Type")]
        public string ActivityTypeName { get; set; }
        [Export("รายละเอียดการติดต่อ")]
        public string ActivityDescription { get; set; }
        [Export("Email TO")]
        public string SendMailTo { get; set; }
        [Export("Email CC")]
        public string SendMailCc { get; set; }
        [Export("Email BCC")]
        public string SendMailBcc { get; set; }
        [Export("Email Subject")]
        public string SendMailSubject { get; set; }
        [Export("Email Body")]
        public string SendMailBody { get; set; }
        [Export("Activity Create by")]
        public string ActivityCreateBy { get; set; }
        public DateTime? ActivityCreateDate { get; set; }
        [Export("Activity Create Date")]
        public string ActivityCreateDateDisplay
        {
            get
            {
                return ActivityCreateDate.HasValue ? ActivityCreateDate.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime) : "";
            }
        }
    }

    public class SRReportSFtpConfig : SFtpConfig
    {
        public string UploadFile { get; set; }
        public string DailyRemoteDir { get; set; }
        public string AccumRemoteDir { get; set; }
        public string ReportType { get; set; }

        public SRReportSFtpConfig(string reportType)
        {
            SFtp_Host = WebConfig.GetReportSRSshHost();
            SFtp_Port = WebConfig.GetReportSRSshPort();
            SFtp_UserName = WebConfig.GetReportSRSshUser();
            SFtp_Password = WebConfig.GetReportSRSshPassword();
            DailyRemoteDir = WebConfig.GetReportSRSshDailyDir();
            AccumRemoteDir = WebConfig.GetReportSRSshAccumDir();
            RemoteDir = (reportType.ToUpper() == "DAILY" ? DailyRemoteDir : AccumRemoteDir);
        }
    }
}
