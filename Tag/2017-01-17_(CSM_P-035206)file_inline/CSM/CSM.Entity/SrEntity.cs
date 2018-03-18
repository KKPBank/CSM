using System;
using System.Linq;
using CSM.Common.Utilities;
using CSM.Entity.Common;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;

namespace CSM.Entity
{
    [Serializable]
    public class SrEntity
    {
        //public int? RuleSlaFlag { get; set; }
        public int? SrId { get; set; }
        public int? RuleAlertNo { get; set; }
        public int? RuleTotalWorkingHours { get; set; }
        public DateTime? RuleNextSLA { get; set; }
        public string AccountNo { get; set; }
        public string SrNo { get; set; }
        public string ChannelName { get; set; }
        public string ProductName { get; set; }       
        public string AreaName { get; set; }
        public string SubareaName { get; set; }
        public string SrSubject { get; set; }
        public string SrStateName { get; set; }
        public string SrStatusName { get; set; }
        public string SrANo { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? OwnerId { get; set; }
        public UserEntity OwnerUser { get; set; }
        public UserEntity DelegateUser { get; set; }
        public string RuleNextSLADisplay
        {
            get { return RuleNextSLA.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime); }
        }
        public string CreateDateDisplay
        {
            get { return CreateDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime); }
        }
        public string UpdateDateDisplay
        {
            get { return UpdateDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime); }
        }

        public string TotalWorkingHoursDisplay { get { return ToDayHourMinute(RuleTotalWorkingHours); } }
        public static string ToDayHourMinute(int? minute)
        {
            if (!minute.HasValue)
                return string.Empty;

            var val = minute.Value;
            var dd = val / 1440;
            var hh = (val / 60) % 24;
            var mm = val % 60;

            return (dd > 0 ? dd + "D " : "") + (hh > 0 ? hh + "H " : "") + (mm > 0 ? mm + "MM" : "");
        }

        public bool? Is_Secret { get; set; }
        public int? SrPageId { get; set; }

        public int? OwnerUserId { get; set; }
        public int? DelegateUserId { get; set; }
        public int? OwnerSupUserId { get; set; }
        public int? DelegateSupUserId { get; set; }

        public IEnumerable<int> OwnerDelegateAndSupervisor
        {
            get
            {
                if (OwnerUserId.HasValue)
                { yield return OwnerUserId.Value; }
                if (DelegateUserId.HasValue)
                { yield return DelegateUserId.Value; }
                if (OwnerSupUserId.HasValue)
                { yield return OwnerSupUserId.Value; }
                if (DelegateSupUserId.HasValue)
                { yield return DelegateSupUserId.Value; }
            }
        }
    }

    public class SrSearchFilter : Pager
    {
        public int? CustomerId { get; set; }      
        public List<UserEntity> OwnerList { get; set; }
        public int? FilterType { get; set; }
        public int? CurrentUserId { get; set; }
        public bool? CanViewAllUsers { get; set; }
        public string CanViewUserIds { get; set; }
        public string CanViewSrPageIds { get; set; }
        public string CurrentUserRoleCode { get; set; }
    }

    public class ActivityTabSearchFilter : Pager
    {
        public int? SrId { get; set; }
        public string SrNo { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerCardNo { get; set; }
        public string CustomerSubscriptionTypeCode { get; set; }
        public bool SrOnly { get; set; }
        public string JsonActivities { get; set; }

        public bool IsOnline { get; set; }
    }

    public class ExistingSearchFilter : Pager
    {
        public int? CustomerId { get; set; }
    }

    public class DocumentSearchFilter : Pager
    {
        public int? SrId { get; set; }
        public int? CustomerId { get; set; }
        public bool SrOnly { get; set; }
    }

    public class LoggingSearchFilter : Pager
    {
        public int? SrId { get; set; }
    }

    public class ServiceDocumentEntity
    {
        public int SrAttachId { get; set; }
        public int? SrId { get; set; }
        public int? SrActivityId { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentDesc { get; set; }
        public string DocumentLevel { get; set; }
        public string SrReference { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public List<DocumentTypeEntity> DocumentTypes { get; set; }

        public string ContentType { get; set; }
        public string AttachmentFilename { get; set; }
        public string Url { get; set; }
        public string ExpiryDateDisplay
        {
            get
            {
                if (ExpireDate.HasValue)
                    return DateUtil.ToStringAsDate(ExpireDate.Value);
                else
                    return string.Empty;
            }
        }

        public string CreateDateDisplay
        {
            get { return CreateDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime); }
        }

        public int? FileSize { get; set; }
        public short? Status { get; set; }
        public int? CreateUserId { get; set; }
    }

    public class ServiceRequestLoggingResult
    {
        public int SrLoggingId { get; set; }
        public DateTime? CreateDate { get; set; }
        public string SystemAction { get; set; }
        public string Action { get; set; }
        public string StatusOld { get; set; }
        public string StatusNew { get; set; }
        public UserEntity CreateUser { get; set; }
        public string CreateUsername { get; set; }
        public UserEntity OwnerOldUser { get; set; }
        public UserEntity OwnerNewUser { get; set; }
        public UserEntity DelegateOldUser { get; set; }
        public UserEntity DelegateNewUser { get; set; }

        public int? OverSlaMinute { get; set; }
        public string OverSlaMinuteDisplay { get { return ToDayHourMinute(OverSlaMinute); } }

        public int? OverSlaTimes { get; set; }
        public string OverSlaTimesDisplay { get { return OverSlaTimes.HasValue ? OverSlaTimes.Value + " ครั้ง" : ""; } }

        public int? WorkingMinute { get; set; }
        //public string WorkingMinuteDisplay { get { return ToDayHourMinute(WorkingMinute); } }
        public string WorkingMinuteDisplay { get { return ToDayHourMinute(WorkingHour); } }
        public int? WorkingHour { get; set; }

        public static string ToDayHourMinute(int? minute)
        {
            if (!minute.HasValue)
                return string.Empty;

            var val = minute.Value;
            var dd = val / 1440;
            var hh = (val / 60) % 24;
            var mm = val % 60;

            return (dd > 0 ? dd + "D " : "") + (hh > 0 ? hh + "H " : "") + (mm > 0 ? mm + "MM" : "");
        }

        public bool? CPN_IsSecretOld { get; set; }
        public string CPN_IsSecretOldDisplay
        {
            get { return CPN_IsSecretOld.ToDisplay(); }
        }

        public bool? CPN_IsSecretNew { get; set; }
        public string CPN_IsSecretNewDisplay
        {
            get { return CPN_IsSecretNew.ToDisplay(); }
        }
    }

    public class ServiceRequestActivityResult
    {
        public string SystemCode { get; set; }
        public int SrActivityId { get; set; }
        public string CustomerCardNo { get; set; }
        public string AccountNo { get; set; }
        public string SrNo { get; set; }
        public string ProductName { get; set; }
        public string TypeName { get; set; }
        public string AreaName { get; set; }
        public string SubAreaName { get; set; }
        public string StatusDesc { get; set; }
        public DateTime? Date { get; set; }
        public string ContactUserFirstName { get; set; }
        public string ContactUserLastName { get; set; }
        public string ActivityDesc { get; set; }
        public string ChannelName { get; set; }

        private string _contactFullName;

        public string ContactFullName
        {
            get
            {
                if (!string.IsNullOrEmpty(_contactFullName))
                    return _contactFullName;

                string[] names = new string[2] { this.ContactUserFirstName.NullSafeTrim(), this.ContactUserLastName.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    return names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                }

                return string.Empty;
            }
            set { _contactFullName = value; }
        }

        public string OfficerUserFirstName { get; set; }
        public string OfficerUserLastName { get; set; }
        public string OfficerUserPositionCode { get; set; }

        private string _officerUserFullName;
        public string OfficerUserFullName
        {
            get
            {
                if (!string.IsNullOrEmpty(_officerUserFullName))
                    return _officerUserFullName;

                string[] names = new string[2] { this.OfficerUserFirstName.NullSafeTrim(), this.OfficerUserLastName.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    string positionCode = this.OfficerUserPositionCode.NullSafeTrim();
                    string fullName = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                    return !string.IsNullOrEmpty(positionCode) ? string.Format(CultureInfo.InvariantCulture, "{0} - {1}", positionCode, fullName) : fullName;
                }

                return string.Empty;
            }
            set
            {
                _officerUserFullName = value;
            }
        }


        public string CreatorBranchName { get; set; }
        public string CreatorUserFirstName { get; set; }
        public string CreatorUserLastName { get; set; }
        public string CreatorUserPositionCode { get; set; }

        private string _creatorUserFullName;
        public string CreatorUserFullName
        {
            get
            {
                if (!string.IsNullOrEmpty(_creatorUserFullName))
                    return _creatorUserFullName;

                string[] names = new string[2] { this.CreatorUserFirstName.NullSafeTrim(), this.CreatorUserLastName.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    string positionCode = this.CreatorUserPositionCode.NullSafeTrim();
                    string fullName = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                    return !string.IsNullOrEmpty(positionCode) ? string.Format(CultureInfo.InvariantCulture, "{0} - {1}", positionCode, fullName) : fullName;
                }

                return string.Empty;
            }
            set
            {
                _creatorUserFullName = value;
            }
        }
        public string OwnerBranchName { get; set; }
        public string OwnerUserFirstName { get; set; }
        public string OwnerUserLastName { get; set; }
        public string OwnerUserPositionCode { get; set; }

        private string _ownerUserFullName;
        public string OwnerUserFullName
        {
            get
            {
                if (!string.IsNullOrEmpty(_ownerUserFullName))
                    return _ownerUserFullName;

                string[] names = new string[2] { this.OwnerUserFirstName.NullSafeTrim(), this.OwnerUserLastName.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    string positionCode = this.OwnerUserPositionCode.NullSafeTrim();
                    string fullName = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                    return !string.IsNullOrEmpty(positionCode) ? string.Format(CultureInfo.InvariantCulture, "{0} - {1}", positionCode, fullName) : fullName;
                }

                return string.Empty;
            }
            set
            {
                _ownerUserFullName = value;
            }
        }

        public string DelegateBranchName { get; set; }
        public string DelegateUserFirstName { get; set; }
        public string DelegateUserLastName { get; set; }
        public string DelegateUserPositionCode { get; set; }

        private string _delegateUserFullName;
        public string DelegateUserFullName
        {
            get
            {
                if (!string.IsNullOrEmpty(_delegateUserFullName))
                    return _delegateUserFullName;

                string[] names = new string[2] { this.DelegateUserFirstName.NullSafeTrim(), this.DelegateUserLastName.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    string positionCode = this.DelegateUserPositionCode.NullSafeTrim();
                    string fullName = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                    return !string.IsNullOrEmpty(positionCode) ? string.Format(CultureInfo.InvariantCulture, "{0} - {1}", positionCode, fullName) : fullName;
                }

                return string.Empty;
            }
            set
            {
                _delegateUserFullName = value;
            }
        }
        public bool IsSendEmail { get; set; }
        public string EmailSender { get; set; }
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
        public string EmailBcc { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string EmailAttachments { get; set; }
        public string ActivityTypeName { get; set; }
        public string SRStateName { get; set; }
        public string SRStatusName { get; set; }

        public string AccountNoDisplay
        {
            get
            {
                string strResult = AccountNo;
                if (!string.IsNullOrEmpty(AccountNo))
                {
                    //string dummy = "DUMMY-";
                    //if (AccountNo.Count() > dummy.Count() && AccountNo.Substring(0, dummy.Count()) == dummy)
                    //{
                    //    strResult = "DUMMY";
                    //}
                    if (AccountNo.StartsWith("DUMMY-"))
                    {
                        strResult = "DUMMY";
                    }
                    else if (AccountNo.StartsWith("NA-"))
                    {
                        strResult = "NA";
                    }
                }
                return strResult;
            }
            
        }

        public string MediaSourceName { get; set; }
        public string Subject { get; set; }
        public string Remark { get; set; }
        public bool? Verify { get; set; }
        public string VerifyDisplay
        {
            get
            {
                if (Verify.HasValue)
                    return (Verify.Value) ? "Yes" : "No";
                else
                    return "";
            }
        }
        public string VerifyResult { get; set; }
        public int SrPageId { get; set; }

        #region "Address"

        public string AddressHouseNo { get; set; }
        public string AddressVillage { get; set; }
        public string AddressBuilding { get; set; }
        public string AddressFloorNo { get; set; }
        public string AddressRoomNo { get; set; }
        public string AddressMoo { get; set; }
        public string AddressSoi { get; set; }
        public string AddressStreet { get; set; }
        public string AddressTambol { get; set; }
        public string AddressAmphur { get; set; }
        public string AddressProvince { get; set; }
        public string AddressZipCode { get; set; }

        private static string ValueOrDash(string original)
        {
            if (string.IsNullOrEmpty(original))
                return "-";
            else
                return original;
        }

        #endregion
        public string AddressForDisplay
        {
            get
            {
                return "บ้านเลขที่ " + ValueOrDash(AddressHouseNo) +
                       " หมู่บ้าน " + ValueOrDash(AddressVillage) +
                       " อาคาร " + ValueOrDash(AddressBuilding) +
                       " ชั้น " + ValueOrDash(AddressFloorNo) +
                       " เลขที่ห้อง " + ValueOrDash(AddressRoomNo) +
                       " หมู่ " + ValueOrDash(AddressMoo) +
                       " ซอย " + ValueOrDash(AddressSoi) +
                       " ถนน " + ValueOrDash(AddressStreet) +
                       " ตำบล " + ValueOrDash(AddressTambol) +
                       " อำเภอ " + ValueOrDash(AddressAmphur) +
                       " จังหวัด " + ValueOrDash(AddressProvince) +
                       " รหัสไปรษณีย์ " + ValueOrDash(AddressZipCode);
            }
        }
        public string AFSAssetNo { get; set; }
        public string AFSAssetDesc { get; set; }
        public DateTime? NCBCustomerBirthDate { get; set; }
        public string NCBCustomerBirthDateDisplay
        {
            get
            {
                if (NCBCustomerBirthDate.HasValue)
                    return string.Format(new CultureInfo("th-TH"), "{0:dd/MM/yyyy}", NCBCustomerBirthDate.Value);
                else
                    return "";
            }
        }
        public string NCBMarketingBranchUpper1Name { get; set; }
        public string NCBMarketingBranchUpper2Name { get; set; }
        public string NCBMarketingBranchName { get; set; }
        public string NCBMarketingFullName { get; set; }
        public string NCBCheckStatus { get; set; }

        public List<ActivityDataItemEntity> CustomerInfoDataItems { get; set; }
        public List<ActivityDataItemEntity> ContractInfoDataItems { get; set; }
        public List<ActivityDataItemEntity> ActivityInfoDataItems { get; set; }
        public List<ActivityDataItemEntity> OfficerInfoDataItems { get; set; }
        public List<ActivityDataItemEntity> ProductInfoDataItems { get; set; }

        public bool? Is_Secret { get; set; }
        public bool? IsNotSendCAR { get; set; }
        public short? SendCARStatus { get; set; }
    }
}
