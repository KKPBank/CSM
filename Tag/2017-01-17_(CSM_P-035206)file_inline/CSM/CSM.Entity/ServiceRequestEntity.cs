using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using CSM.Common.Utilities;
using CSM.Entity.Common;
using CSM.Common.Resources;
using System.Web.Mvc;

namespace CSM.Entity
{

    [Serializable]
    public class ServiceRequestEntity
    {
        #region == For Save Draft and Save Only ==

        public bool IsSaveSuccess { get; set; }

        public string ErrorMessage { get; set; }

        #endregion


        public int? SrId { get; set; }
        public string SrNo { get; set; }
        public int Status { get; set; }
        public string Subject { get; set; }
        public string Remark { get; set; }
        public string SRStateName { get; set; }
        public string StatusDisplay { get; set; }
        public string SrStatusCode { get; set; }
        public UserEntity UpdateUser { get; set; }
        public UserEntity CreateUser { get; set; }
        public UserEntity Owner { get; set; }
        public UserEntity Delegator { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? CreateDate { get; set; }

        public ProductSREntity ProductMapping { get; set; }
        public string UpdateDateDisplay
        {
            get
            {
                return UpdateDate.HasValue ? UpdateDate.Value.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime) : "";
            }

        }
        public string CreateDateDisplay
        {
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime) : "";
            }

        }

        public CustomerEntity Customer { get; set; }



        public bool IsAlertSla
        {
            get
            {
                return (ThisAlert ?? 0) > 0;
            }
        }

        // Motif Changed
        public int? ThisAlert { get; set; }
        public DateTime? NextSLA { get; set; }
        public string NextSLADisplay
        {
            get
            {
                return NextSLA.HasValue ? NextSLA.Value.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime) : "";
            }
        }
        public int? TotalWorkingHours { get; set; }
        public string TotalWorkingHoursDisplay { get { return ToDayHourMinute(TotalWorkingHours); } }
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
        public string CustomerCardNo { get; set; }
        public int? ChannelId { get; set; }
        public string ChannelName { get; set; }
        public int? MediaSourceId { get; set; }
        public string MediaSourceName { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public int? CampaignServiceId { get; set; }
        public string CampaignServiceName { get; set; }
        public int? AreaId { get; set; }
        public string AreaName { get; set; }
        public int? SubAreaId { get; set; }
        public string SubAreaName { get; set; }
        public string SrStatusName { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string ClosedDateDisplay
        {
            get
            {
                return ClosedDate.HasValue ? ClosedDate.Value.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime) : "";
            }
        }
        public int? OwnerUserId { get; set; }
        public string OwnerUserPosition { get; set; }
        public string OwnerUserFirstName { get; set; }
        public string OwnerUserLastName { get; set; }
        public string OwnerUserFullname
        {
            get
            {
                string[] names = new string[2] { this.OwnerUserFirstName.NullSafeTrim(), this.OwnerUserLastName.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    string positionCode = this.OwnerUserPosition.NullSafeTrim();
                    string fullName = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                    return !string.IsNullOrEmpty(OwnerUserPosition) ? string.Format(CultureInfo.InvariantCulture, "{0} - {1}", positionCode, fullName) : fullName;
                }

                return string.Empty;
            }
        }

        public int? DelegateUserId { get; set; }
        public string DelegateUserPosition { get; set; }
        public string DelegateUserFirstName { get; set; }
        public string DelegateUserLastName { get; set; }

        public string DelegateUserFullname
        {
            get
            {
                string[] names = new string[2] { this.DelegateUserFirstName.NullSafeTrim(), this.DelegateUserLastName.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    string positionCode = this.DelegateUserPosition.NullSafeTrim();
                    string fullName = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                    return !string.IsNullOrEmpty(DelegateUserPosition) ? string.Format(CultureInfo.InvariantCulture, "{0} - {1}", positionCode, fullName) : fullName;
                }

                return string.Empty;
            }
        }

        public string CallId { get; set; }
        public string ANo { get; set; }
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public string AccountNo { get; set; }
        public int ContactId { get; set; }
        public int ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }

        public bool IsVerify { get; set; }
        public string IsVerifyPass { get; set; }

        public int? SrPageId { get; set; }
        public string SrPageCode { get; set; }


        public int? AddressSendDocId { get; set; }
        public int? AFSAssetId { get; set; }
        public string NCBCustomerBirthDate { get; set; }
        public int? NCBMarketingUserId { get; set; }
        public string NCBCheckStatus { get; set; }

        public string TransferType { get; set; }
        public string TransferTypeDisplay
        {
            get
            {
                if (TransferType.ToUpper(CultureInfo.InvariantCulture) == "OWNER")
                    return "Owner SR";
                else if (TransferType.ToUpper(CultureInfo.InvariantCulture) == "DELEGATE")
                    return "Delegate SR";
                else
                    return "";
            }
        }

        public bool? CPN_IsSecret { get; set; }
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

    [Serializable]
    public class ServiceRequestForDisplayEntity
    {
        public int SrId { get; set; }
        public string SrNo { get; set; }
        public string CallId { get; set; }
        public string PhoneNo { get; set; }

        public CustomerEntity Customer { get; set; }
        public SubscriptTypeEntity CustomerSubscriptType { get; set; }
        public string CustomerEmployeeCode { get; set; }

        public AccountEntity Account { get; set; }
        public ContactEntity Contact { get; set; }
        public SubscriptTypeEntity ContactSubscriptType { get; set; }

        public RelationshipEntity Relationship { get; set; }
        public string ContactAccountNo { get; set; }
        
        public int? MappingProductId { get; set; }
        public ProductGroupEntity ProductGroup { get; set; }
        public ProductEntity Product { get; set; }
        public CampaignServiceEntity CampaignService { get; set; }
        public AreaEntity Area { get; set; }
        public SubAreaEntity SubArea { get; set; }
        public TypeEntity Type { get; set; }
        public MediaSourceEntity MediaSource { get; set; }
        public ChannelEntity Channel { get; set; }

        public string Subject { get; set; }
        public string Remark { get; set; }

        public int? SrPageId { get; set; }
        public string SrPageCode { get; set; }

        public UserEntity OwnerUser { get; set; }
        public BranchEntity OwnerUserBranch { get; set; }
        public UserEntity DelegateUser { get; set; }
        public BranchEntity DelegateUserBranch { get; set; }
        public UserEntity CreateUser { get; set; }
        public BranchEntity CreateBranch { get; set; }
        public UserEntity UpdateUser { get; set; }

        public SRStateEntity SRState { get; set; }
        public SRStatusEntity SRStatus { get; set; }
        public bool? IsVerify { get; set; }
        public string IsVerifyPass { get; set; }

        public int? DraftSrEmailTemplateId { get; set; }
        public string DraftActivityDescription { get; set; }
        public string DraftSendMailSender { get; set; }
        public string DraftSendMailTo { get; set; }
        public string DraftSendMailCc { get; set; }
        public string DraftSendMailSubject { get; set; }
        public string DraftSendMailBody { get; set; }
        public int? DraftActivityTypeId { get; set; }
        public bool DraftIsEmailDelegate { get; set; }
        public bool DraftIsClose { get; set; }

        public string DraftVerifyAnswerJson { get; set; }
        public string DraftAttachmentJson { get; set; }
        
        public int? DraftAccountAddressId { get; set; }
        public string DraftAccountAddressText { get; set; }


        public int? AddressSendDocId { get; set; }
        public AccountAddressEntity AccountAddress { get; set; }


        public int? AfsAssetId { get; set; }
        public string AfsAssetNo { get; set; }
        public string AfsAssetDesc { get; set; }

        public DateTime? NCBCustomerBirthDate { get; set; }
        public string NCBCustomerBirthDateDisplay
        {
            get
            {
                if (!NCBCustomerBirthDate.HasValue)
                    return "";

                return string.Format(new CultureInfo("th-TH"), "{0:dd/MM/yyyy}", NCBCustomerBirthDate);
            }
        }
        public string NCBCheckStatus { get; set; }
        public int? NCBMarketingUserId { get; set; }
        public string NCBMarketingFullName { get; set; }
        public int? NCBMarketingBranchId { get; set; }
        public string NCBMarketingBranchName { get; set; }
        public int? NCBMarketingBranchUpper1Id { get; set; }
        public string NCBMarketingBranchUpper1Name { get; set; }
        public int? NCBMarketingBranchUpper2Id { get; set; }
        public string NCBMarketingBranchUpper2Name { get; set; }

        public DateTime? CreateDate { get; set; }
        public DateTime? CloseDate { get; set; }

        #region Complaint

        public int? CPN_ProductGroupId { get; set; }
        public string CPN_ProductGroupName { get; set; }
        public int? CPN_ProductId { get; set; }
        public string CPN_ProductName { get; set; }
        public int? CPN_CampaignId { get; set; }
        public string CPN_CampaignName { get; set; }
        public int? CPN_SubjectId { get; set; }
        public string CPN_SubjectName { get; set; }
        public int? CPN_TypeId { get; set; }
        public string CPN_TypeName { get; set; }
        public int? CPN_RootCauseId { get; set; }
        public string CPN_RootCauseName { get; set; }
        public int? CPN_MappingId { get; set; }
        public int? CPN_IssuesId { get; set; }
        public string CPN_IssuesName { get; set; }
        public bool? CPN_IsSecret { get; set; }
        public bool? CPN_IsCAR { get; set; }
        public bool? CPN_IsHPLog { get; set; }
        public int? CPN_BUGroupId { get; set; }
        public string CPN_BUGroupName { get; set; }
        public bool? CPN_IsSummary { get; set; }
        public bool? CPN_Cause_Customer { get; set; }
        public bool? CPN_Cause_Staff { get; set; }
        public bool? CPN_Cause_System { get; set; }
        public bool? CPN_Cause_Process { get; set; }
        public string CPN_Cause_Customer_Detail { get; set; }
        public string CPN_Cause_Staff_Detail { get; set; }
        public string CPN_Cause_System_Detail { get; set; }
        public string CPN_Cause_Process_Detail { get; set; }
        public int? CPN_CauseSummaryId { get; set; }
        public string CPN_CauseSummaryName { get; set; }
        public int? CPN_SummaryId { get; set; }
        public string CPN_SummaryName { get; set; }
        public string CPN_Fixed_Detail { get; set; }
        public string CPN_BU1Code { get; set; }
        public string CPN_BU2Code { get; set; }
        public string CPN_BU3Code { get; set; }
        public int? CPN_MSHBranchId { get; set; }

        #endregion

        // For Create Activity
        public string RuleAssignFlag { get; set; }
    }

    public class ServiceRequestNoDetailEntity
    {
        public int SrId { get; set; }
        public string SrNo { get; set; }
        public int? CustomerId { get; set; }
        public int? AccountId { get; set; }
        public int? ContactId { get; set; }
        public string ContactAccountNo { get; set; }
        public int? ContactRelationshipId { get; set; }
        public int? ProductGroupId { get; set; }
        public int? ProductId { get; set; }
        public int? CampaignServiceId { get; set; }
        public int? AreaId { get; set; }
        public int? SubAreaId { get; set; }
        public int? TypeId { get; set; }
        public int? MapProductId { get; set; }
        public int? ChannelId { get; set; }
        public int? MediaSourceId { get; set; }
        public string Subject { get; set; }
        public string Remark { get; set; }

        public int? CreatorBranchId { get; set; }
        public int? CreatorUserId { get; set; }
        public int? OwnerBranchId { get; set; }
        public int? OwnerUserId { get; set; }
        public int? DelegateBranchId { get; set; }
        public int? DelegateUserId { get; set; }

        public int? CloseUserId { get; set; }
        public string CloseUserName { get; set; }

        public int? SrPageId { get; set; }
        public int? SrStatusId { get; set; }

        public bool IsEmailDelegate { get; set; }

        public bool? IsVerify { get; set; }
        public string IsVerifyPass { get; set; }

        public bool? CPN_IsSecret { get; set; }
        public bool? IsNotSendCar { get; set; }
        public DateTime? CreateDate { get; set; }

        public string SRStatusName { get; set; }
    }

    [Serializable]
    public class ServiceRequestActivityEntity
    {
        public int SrId { get; set; }
        public string SrNo { get; set; }
        public int SrActivityId { get; set; }

        public int OwnerBranchId { get; set; }
        public int OwnerUserId { get; set; }

        public int DelegateBranchId { get; set; }
        public int DelegateUserId { get; set; }
        public int SrStatusId { get; set; }

//        public int OldOwnerBranchId { get; set; }
//        public int OldOwnerUserId { get; set; }
//        public int OldDelegateBranchId { get; set; }
//        public int OldDelegateUserId { get; set; }
//        public int OldSrStatusId { get; set; }

        public bool IsSendDelegateEmail { get; set; }
        public string ActivityDescription { get; set; }
        
        public int? EmailTemplateId { get; set; }
        public string EmailSender { get; set; }
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public int ActivityTypeId { get; set; }
        
        public int? CreateBranchId { get; set; }
        public int CreateUserId { get; set; }

    }


    [Serializable]
    public class ServiceRequestForInsertLogEntity
    {
        public int SrId { get; set; }
        public int SrActivityId { get; set; }
        public string SrNo { get; set; }
        public string CallId { get; set; }
        public string ANo { get; set; }

        public int CustomerId { get; set; }
        public int CustomerSubscriptionTypeId { get; set; }
        public string CustomerSubscriptionTypeName { get; set; }
        public string CustomerSubscriptionTypeCode { get; set; }
        public string CustomerCardNo { get; set; }
        public DateTime? CustomerBirthDate { get; set; }
        public string CustomerBirthDateDisplay
        {
            get
            {
                if (CustomerBirthDate.HasValue)
                    return string.Format(new CultureInfo("en-US"), "{0:dd/MM/yyyy}", CustomerBirthDate.Value);
                else
                    return "";
            }
        }
        public string CustomerTitleTh { get; set; }
        public string CustomerFirstNameTh { get; set; }
        public string CustomerLastNameTh { get; set; }
        public string CustomerTitleEn { get; set; }
        public string CustomerFirstNameEn { get; set; }
        public string CustomerLastNameEn { get; set; }
        public string CustomerPhoneType1 { get; set; }
        public string CustomerPhoneNo1 { get; set; }
        public string CustomerPhoneType2 { get; set; }
        public string CustomerPhoneNo2 { get; set; }
        public string CustomerPhoneType3 { get; set; }
        public string CustomerPhoneNo3 { get; set; }
        public string CustomerFax { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerEmployeeCode { get; set; }
        public string KKCISID { get; set; }

        public int AccountId { get; set; }
        public string AccountNo { get; set; }
        public string AccountStatus { get; set; }
        public string AccountCarNo { get; set; }
        public string AccountProductGroup { get; set; }
        public string AccountProduct { get; set; }
        public string AccountBranchName { get; set; }

        public int ContactId { get; set; }
        public string ContactSubscriptionTypeName { get; set; }
        public string ContactCardNo { get; set; }
        public DateTime? ContactBirthDate { get; set; }
        public string ContactBirthDateDisplay
        {
            get
            {
                if (ContactBirthDate.HasValue)
                    return string.Format(new CultureInfo("en-US"), "{0:dd/MM/yyyy}", ContactBirthDate.Value);
                else
                    return "";
            }
        }
        public string ContactTitleTh { get; set; }
        public string ContactFirstNameTh { get; set; }
        public string ContactLastNameTh { get; set; }
        public string ContactTitleEn { get; set; }
        public string ContactFirstNameEn { get; set; }
        public string ContactLastNameEn { get; set; }
        public string ContactPhoneType1 { get; set; }
        public string ContactPhoneNo1 { get; set; }
        public string ContactPhoneType2 { get; set; }
        public string ContactPhoneNo2 { get; set; }
        public string ContactPhoneType3 { get; set; }
        public string ContactPhoneNo3 { get; set; }
        public string ContactFax { get; set; }
        public string ContactEmail { get; set; }
        public string ContactAccountNo { get; set; }
        public string ContactRelationshipName { get; set; }

        public string ProductGroupCode { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string CampaignServiceCode { get; set; }
        public string CampaignServiceName { get; set; }
        public int AreaId { get; set; }
        public decimal AreaCode { get; set; }
        public string AreaName { get; set; }
        public int SubAreaId { get; set; }
        public decimal SubAreaCode { get; set; }
        public string SubAreaName { get; set; }
        public int TypeId { get; set; }
        public decimal TypeCode { get; set; }
        public string TypeName { get; set; }
        public string ChannelCode { get; set; }
        public string ChannelName { get; set; }
        public string MediaSourceName { get; set; }
        public string Subject { get; set; }
        public string Remark { get; set; }
        public string Verify { get; set; }
        public string VerifyResult { get; set; }

        // Officer
        public string SRCreatorBranchName { get; set; }
        public string SRCreatorUserFirstName { get; set; }
        public string SRCreatorUserLastName { get; set; }
        public string SRCreatorUserPositionCode { get; set; }
        public string SRCreatorUserFullName
        {
            get
            {
                string[] names = new string[2] { this.SRCreatorUserFirstName.NullSafeTrim(), this.SRCreatorUserLastName.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    string positionCode = this.SRCreatorUserPositionCode.NullSafeTrim();
                    string fullName = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                    return !string.IsNullOrEmpty(positionCode) ? string.Format(CultureInfo.InvariantCulture, "{0} - {1}", positionCode, fullName) : fullName;
                }

                return string.Empty;
            }
        }
        
        public string OwnerBranchName { get; set; }
        public string OwnerUserFirstName { get; set; }
        public string OwnerUserLastName { get; set; }
        public string OwnerUserPositionCode { get; set; }
        public string OwnerUserFullName
        {
            get
            {
                string[] names = new string[2] { this.OwnerUserFirstName.NullSafeTrim(), this.OwnerUserLastName.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    string positionCode = this.OwnerUserPositionCode.NullSafeTrim();
                    string fullName = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                    return !string.IsNullOrEmpty(positionCode) ? string.Format(CultureInfo.InvariantCulture, "{0} - {1}", positionCode, fullName) : fullName;
                }

                return string.Empty;
            }
        }

        public string DelegateBranchName { get; set; }
        public string DelegateUserFirstName { get; set; }
        public string DelegateUserLastName { get; set; }
        public string DelegateUserPositionCode { get; set; }
        public string DelegateUserFullName
        {
            get
            {
                string[] names = new string[2] { this.DelegateUserFirstName.NullSafeTrim(), this.DelegateUserLastName.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    string positionCode = this.DelegateUserPositionCode.NullSafeTrim();
                    string fullName = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                    return !string.IsNullOrEmpty(positionCode) ? string.Format(CultureInfo.InvariantCulture, "{0} - {1}", positionCode, fullName) : fullName;
                }

                return string.Empty;
            }
        }

        public string SendEmail { get; set; }
        public string EmailSender { get; set; }
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
        public string EmailBcc { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string EmailAttachments { get; set; }

        public string ActivityDescription { get; set; }
        public int ActivityTypeId { get; set; }
        public string ActivityTypeName { get; set; }
        
        public string SRStateName { get; set; }
        public string SRStatusName { get; set; }

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
        public string NCBCheckStatus { get; set; }
        public string NCBMarketingFullName { get; set; }
        public string NCBMarketingBranchName { get; set; }
        public string NCBMarketingBranchUpper1Name { get; set; }
        public string NCBMarketingBranchUpper2Name { get; set; }

        public DateTime? ActivityCreateDate { get; set; }
        public string ActivityCreateDateDisplay
        {
            get
            {
                if (ActivityCreateDate.HasValue)
                    return string.Format(new CultureInfo("en-US"), "{0:dd/MM/yyyy}", ActivityCreateDate.Value);
                else
                    return "";
            }
        }
        public string ActivityCreateUserFirstName { get; set; }
        public string ActivityCreateUserLastName { get; set; }
        public string ActivityCreateUserPositionCode { get; set; }
        public string ActivityCreateUserEmployeeCode { get; set; }
        public string ActivityCreateUserFullName
        {
            get
            {
                string[] names = new string[2] { this.ActivityCreateUserFirstName.NullSafeTrim(), this.ActivityCreateUserLastName.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    string positionCode = this.ActivityCreateUserPositionCode.NullSafeTrim();
                    string fullName = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                    return !string.IsNullOrEmpty(positionCode) ? string.Format(CultureInfo.InvariantCulture, "{0} - {1}", positionCode, fullName) : fullName;
                }

                return string.Empty;
            }
        }

        public int MappingProductId { get; set; }
        public int SrPageId { get; set; }

        public string HPSubject { get; set; }
        public string HPLanguageIndependentCode { get; set; }

        public bool IsSendActivityToHP
        {
            get
            {
                return !string.IsNullOrEmpty(this.HPSubject) && !string.IsNullOrEmpty(this.HPLanguageIndependentCode);
            }
        }

        public string GetHpDescription(bool firstActivity)
        {
            //return string.Format(CultureInfo.InvariantCulture, "SR;{0}; Status : {1}; Contact Mobile# {2}; {3}; {4}; Customer Mobile# {5}; {6}; {7}; {8}; {9}; Subject : {10}; Remark : {11}; Activity Type : {12}; {13}",
            //        this.SrNo,                      // 0
            //        this.SRStatusName,              // 1
            //        this.ContactPhoneNo1,           // 2
            //        this.ContactFullName,           // 3
            //        this.ContactRelationshipName,   // 4
            //        this.CustomerPhoneNo1,          // 5
            //        this.CustomerFullName,          // 6
            //        this.TypeName,                  // 7
            //        this.AreaName,                  // 8
            //        this.SubAreaName,               // 9
            //        this.Subject,                   // 10
            //        ApplicationHelpers.RemoveAllHtmlTags(this.Remark),                    // 11
            //        this.ActivityTypeName,          // 12
            //        ApplicationHelpers.RemoveAllHtmlTags(this.ActivityDescription)        // 13
            //    );
            if (firstActivity)
            {
                return $"SR;{SrNo}; Status : {SRStatusName}; Contact Mobile# {ContactPhoneNo1}; "
                        + $"{ContactFullName}; {ContactRelationshipName}; {TypeName}; {AreaName}; {SubAreaName}; "
                        + $"Subject : {Subject}; Remark : {ApplicationHelpers.RemoveAllHtmlTags(Remark)}; "
                        + $"Activity Type : {ActivityTypeName}; {ApplicationHelpers.RemoveAllHtmlTags(ActivityDescription)}";
            }
            else
            {
                return $"SR;{SrNo}; Status : {SRStatusName}; Contact Mobile# {ContactPhoneNo1}; "
                        + $"{ContactFullName}; {ContactRelationshipName}; "
                        + $"Subject : {Subject}; Remark : {ApplicationHelpers.RemoveAllHtmlTags(Remark)}; "
                        + $"Activity Type : {ActivityTypeName}; {ApplicationHelpers.RemoveAllHtmlTags(ActivityDescription)}";
            }
        }

        public string CustomerFirstName { get { return CustomerFirstNameTh; } }

        public string CustomerLastName { get { return CustomerLastNameTh; } }

        public string CustomerFullName
        {
            get
            {
                string[] names = new string[2] { this.CustomerFirstNameTh.NullSafeTrim(), this.CustomerLastNameTh.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    return names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                }

                return string.Empty;
            }
        }

        public string ContactFirstName { get { return ContactFirstNameTh; } }

        public string ContactLastName { get { return ContactLastNameTh; } }

        public string ContactFullName
        {
            get
            {
                string[] names = new string[2] { this.ContactFirstNameTh.NullSafeTrim(), this.ContactLastNameTh.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    return names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                }

                return string.Empty;
            }
        }

        public string CPN_ProductGroupName { get; set; }
        public string CPN_ProductName { get; set; }
        public string CPN_CampaignName { get; set; }
        public string CPN_SubjectName { get; set; }
        public string CPN_TypeName { get; set; }
        public string CPN_RootCauseName { get; set; }
        public string CPN_IssueName { get; set; }
        public bool CPN_IsSecret { get; set; }
        public bool CPN_NotSend_CAR { get; set; }
        public bool CPN_NotSend_HPLog { get; set; }
        public string CPN_BUGroupName { get; set; }

        public string CPN_BU1Desc { get; set; }
        public string CPN_BU2Desc { get; set; }
        public string CPN_BU3Desc { get; set; }
        public string CPN_BUBranchName { get; set; }

        public bool? CPN_IsSummary { get; set; }
        public bool CPN_CauseCustomer { get; set; }
        public string CPN_CauseCustomerDetail { get; set; }
        public bool CPN_CauseStaff { get; set; }
        public string CPN_CauseStaffDetail { get; set; }
        public bool CPN_CauseSystem { get; set; }
        public string CPN_CauseSystemDetail { get; set; }
        public bool CPN_CauseProcess { get; set; }
        public string CPN_CauseProcessDetail { get; set; }
        public string CPN_CauseSummaryName { get; set; }
        public string CPN_SummaryName { get; set; }
        public string CPN_Fixed_Detail { get; set; }
    }


    [Serializable]
    public class ServiceRequestForSaveEntity
    {
        public int? SrId { get; set; }
        public string SrNo { get; set; }

        public string CallId { get; set; }
        public string PhoneNo { get; set; }

        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public int ContactId { get; set; }

        public string ContactAccountNo { get; set; }
        public int ContactRelationshipId { get; set; }

        public int ProductGroupId { get; set; }
        public int ProductId { get; set; }
        public int CampaignServiceId { get; set; }
        public int AreaId { get; set; }
        public int SubAreaId { get; set; }
        public int TypeId { get; set; }
        public int ChannelId { get; set; }
        public int? MediaSourceId { get; set; }
        public string Subject { get; set; }
        public string Remark { get; set; }

        public int MappingProductId { get; set; }
        public bool IsVerify { get; set; }
        public string IsVerifyPass { get; set; }
        public string VerifyAnswerJson { get; set; }
        public List<SrVerifyAnswerEntity> VerifyAnswers { get; set; }
        public List<SrAttachmentEntity> SrAttachments { get; set; }

        public int SrPageId { get; set; }

        public int? AddressSendDocId { get; set; }
        public string AddressSendDocText { get; set; }

        public int? AfsAssetId { get; set; }
        public string AfsAssetNo { get; set; }
        public string AfsAssetDesc { get; set; }

        public DateTime? NCBBirthDate { get; set; }
        public string NCBCheckStatus { get; set; }
        public int? NCBMarketingUserId { get; set; }
        public string NCBMarketingName { get; set; }
        public int? NCBMarketingBranchId { get; set; }
        public string NCBMarketingBranchName { get; set; }
        public int? NCBMarketingBranchUpper1Id { get; set; }
        public string NCBMarketingBranchUpper1Name { get; set; }
        public int? NCBMarketingBranchUpper2Id { get; set; }
        public string NCBMarketingBranchUpper2Name { get; set; }

        public int OwnerBranchId { get; set; }
        public int OwnerUserId { get; set; }
        public int? DelegateBranchId { get; set; }
        public int? DelegateUserId { get; set; }

        public int? SrEmailTemplateId { get; set; }
        public string ActivityDescription { get; set; }
        public string SendMailSender { get; set; }
        public string SendMailTo { get; set; }
        public string SendMailCc { get; set; }
        public string SendMailBcc { get; set; }
        public string SendMailSubject { get; set; }
        public string SendMailBody { get; set; }
        public int ActivityTypeId { get; set; }
        public bool IsEmailDelegate { get; set; }

        public bool IsClose { get; set; }
        public DateTime? CloseDate { get; set; }
        public int? CloseUserId { get; set; }
        public string CloseUserName { get; set; }

        public string AttachmentJson { get; set; }

        public int SrStatusId { get; set; }

        #region Complaint

        public int? CPN_ProductGroupId { get; set; }
        public int? CPN_ProductId { get; set; }
        public int? CPN_CampaignId { get; set; }
        public int? CPN_SubjectId { get; set; }
        public int? CPN_TypeId { get; set; }
        public int? CPN_RootCauseId { get; set; }
        public int? CPN_MappingId { get; set; }
        public int? CPN_IssuesId { get; set; }
        public bool? CPN_IsSecret { get; set; }
        public bool? CPN_IsCAR { get; set; }
        public bool? CPN_IsHPLog { get; set; }
        public int? CPN_BUGroupId { get; set; }
        public bool? CPN_IsSummary { get; set; }
        public bool? CPN_Cause_Customer { get; set; }
        public bool? CPN_Cause_Staff { get; set; }
        public bool? CPN_Cause_System { get; set; }
        public bool? CPN_Cause_Process { get; set; }
        public string CPN_Cause_Customer_Detail { get; set; }
        public string CPN_Cause_Staff_Detail { get; set; }
        public string CPN_Cause_System_Detail { get; set; }
        public string CPN_Cause_Process_Detail { get; set; }
        public int? CPN_CauseSummaryId { get; set; }
        public int? CPN_SummaryId { get; set; }
        public string CPN_Fixed_Detail { get; set; }
        public string CPN_BU1Code { get; set; }
        public string CPN_BU2Code { get; set; }
        public string CPN_BU3Code { get; set; }
        public int? CPN_MSHBranchId { get; set; }

        #endregion

        public int? CreateBranchId { get; set; }
        public int? CreateUserId { get; set; }
        public string CreateUsername { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class SrVerifyAnswerEntity
    {
        public int GroupId { get; set; }
        public int QuestionId { get; set; }
        public string Value { get; set; }
    }

    public class SrAttachmentEntity
    {
        public int? SrAttachId { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Filename { get; set; }
        public string DocDesc { get; set; }
        public string ContentType { get; set; }
        public string ExpiryDate { get; set; }
        public string AttachToEmail { get; set; }
        public string CreateDate { get; set; }
        public bool IsEditable { get; set; }
        public int? FileSize { get; set; }
        public List<AttachmentTypeEntity> DocumentType { get; set; }

        // For Batch Reply Email Only
        public DateTime? CreateDateAsDateTime { get; set; }
        public short? Status { get; set; }
        public string CreateUserId { get; set; }
        public string CreateUserName { get; set; }
    }

    public class SrVerifyGroupEntity
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int RequirePass { get; set; }
        public List<SrVerifyQuestionEntity> VerifyQuestions { get; set; }
    }

    public class SrVerifyQuestionEntity
    {
        public int QuestionId { get; set; }
        public string QuestionName { get; set; }
        public string Result { get; set; }
    }


    [Serializable]
    public class ServiceRequestSaveResult
    {
        public ServiceRequestSaveResult()
        {
            WarningMessages = new List<string>();
        }

        public ServiceRequestSaveResult(bool isSuccess, string errorMessage = "", string errorCode = "")
            : this()
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
        public bool IsSaveDraft { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string SrNo { get; set; }

        public short Status
        {
            get
            {
                return IsSuccess ? (short)1 : (short)0;
            }
        }

        public string ErrorMessageForDisplay
        {
            get
            {
                return ErrorMessage + (!string.IsNullOrEmpty(ErrorCode) ? string.Format(CultureInfo.InvariantCulture, " (ErrorCode={0})", ErrorCode) : "");
            }
        }

        public List<string> WarningMessages { get; set; }

    }

    [Serializable]
    public class ServiceRequestSaveSrResult : ServiceRequestSaveResult
    {
        public ServiceRequestSaveSrResult() { }

        public ServiceRequestSaveSrResult(bool isSuccess, string errorMessage = "", int? srId = null, string errorCode = "", string srNo = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            SrId = srId;
            SrNo = srNo;
        }

        public int? SrId { get; set; }
        public string SrNo { get; set; }
    }

    [Serializable]
    public class ServiceRequestSaveActivityResult : ServiceRequestSaveResult
    {
        public int SrActivityId { get; set; }

        public bool IsSendEmail { get; set; }
        public string EmailSender { get; set; }
        public string EmailReceivers { get; set; }
        public string EmailCcs { get; set; }
        public string EmailBccs { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string EmailAttachments { get; set; }
        public bool IsSensitive { get; set; } // email ที่มีข้อมูล Sensitive ไม่ให้ลูกค้า Reply email กลับมา
    }

    [Serializable]
    public class ServiceRequestSaveCarResult : ServiceRequestSaveResult
    {
        public ServiceRequestSaveCarResult() { }
    }

    [Serializable]
    public class ServiceRequestSaveCbsHpResult : ServiceRequestSaveResult
    {
        public ServiceRequestSaveCbsHpResult() { }
        public bool IsSendServiceRequestDataToLog100 { get; set; }
    }

    [Serializable]
    public class ServiceRequestCustomerContactEntity
    {
        public ServiceRequestCustomerAccount CustomerAccount { get; set; }
        public ContactEntity Contact { get; set; }
        public CustomerContactEntity CustomerContact { get; set; }
    }


    public class ServiceRequestSearchFilter : Pager
    {
        public int CurrentUserId { get; set; }
        public string CurrentUserRoleCode { get; set; }

        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerCardNo { get; set; }
        public string SrNo { get; set; }
        public string AccountNo { get; set; }
        public string PhoneNo { get; set; }
        public int? OwnerBranchId { get; set; }
        public int? OwnerUserId { get; set; }
        public int? DelegateBranchId { get; set; }
        public int? DelegateUserId { get; set; }
        public int? CreatorBranchId { get; set; }
        public int? CreatorUserId { get; set; }

        public string CreateDateFrom { get; set; }
		public DateTime? CreateDateFromValue { get { return CreateDateFrom.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        public string CreateDateTo { get; set; }
		public DateTime? CreateDateToValue { get { return CreateDateTo.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }

        public string CloseDateFrom { get; set; }
		public DateTime? CloseDateFromValue { get { return CloseDateFrom.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        public string CloseDateTo { get; set; }
		public DateTime? CloseDateToValue { get { return CloseDateTo.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }

        public string Subject { get; set; }
        public int? ChannelId { get; set; }
        public int? ProductGroupId { get; set; }
        public int? ProductId { get; set; }
        public int? CampaignServiceId { get; set; }
        public int? TypeId { get; set; }
        public int? AreaId { get; set; }
        public int? SubAreaId { get; set; }
        public string ContactCardNo { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string StatusCode { get; set; }
        public string SRStatusStringList { get; set; }

        public int? SRStateId { get; set; }
        public List<int> SRStatusIdList { get; set; }
        public List<string> SRStatusNameList { get; set; }
        public MultiSelectList SRStatusList { get; set; }

        public string CanViewSrPageIds { get; set; }
        public bool? CanViewAllUsers { get; set; }
        public string CanViewUserIds { get; set; }
    }


    public class GroupAssignSearchFilter : Pager
    {
        public string BranchIds { get; set; }
        public int? ProductId { get; set; }
        public int? CampaignServiceId { get; set; }
        public int? TypeId { get; set; }
        public int? AreaId { get; set; }
        public int? SubAreaId { get; set; }

        public string AssignDateFrom { get; set; }
        public DateTime? AssignDateFromValue { get { return AssignDateFrom.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        public string AssignDateTo { get; set; }
        public DateTime? AssignDateToValue { get { return AssignDateTo.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
    }

    public class UserAssignSearchFilter : Pager
    {
        public int CurrentUserId { get; set; }
        public string UserIds { get; set; }

        public int? BranchId { get; set; }
        public int? ProductId { get; set; }
        public int? CampaignServiceId { get; set; }
        public int? TypeId { get; set; }
        public int? AreaId { get; set; }
        public int? SubAreaId { get; set; }

        public string AssignDateFrom { get; set; }
        public DateTime? AssignDateFromValue { get { return AssignDateFrom.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        public string AssignDateTo { get; set; }
        public DateTime? AssignDateToValue { get { return AssignDateTo.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
    }

    public class UserMonitoringSrSearchFilter : Pager
    {
        /// <summary>
        /// Just have 2 values: {'branch','user'}
        /// </summary>
        public string ViewType { get; set; }
        public int? UserId { get; set; }
        public int? BranchId { get; set; }
        public int? StateId { get; set; }
        public string StatusCode { get; set; }

        public int? ProductId { get; set; }
        public int? CampaignServiceId { get; set; }
        public int? TypeId { get; set; }
        public int? AreaId { get; set; }
        public int? SubAreaId { get; set; }
        public string AssignDateFrom { get; set; }
        public DateTime? AssignDateFromValue { get { return AssignDateFrom.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        public string AssignDateTo { get; set; }
        public DateTime? AssignDateToValue { get { return AssignDateTo.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
    }

    public class ServiceRequestCustomerSearchFilter : SearchFilter
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CardNo { get; set; }
        public string CarNo { get; set; }
        public string AccountNo { get; set; }
        public string PhoneNo { get; set; }
    }

    public class ServiceRequestCustomerSearchResult
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public string CardNo { get; set; }
        public string CustomerFirstNameTh { get; set; }
        public string CustomerLastNameTh { get; set; }
        public string CustomerFirstNameEn { get; set; }
        public string CustomerLastNameEn { get; set; }
        public string CustomerFirstName
        {
            get { return string.IsNullOrEmpty(CustomerFirstNameTh) ? CustomerFirstNameEn : CustomerFirstNameTh; }
        }
        public string CustomerLastName
        {
            get { return string.IsNullOrEmpty(CustomerFirstNameTh) ? CustomerLastNameEn : CustomerLastNameTh; }
        }
        public string AccountNo { get; set; }
        public string PhoneNo { get; set; }
        public string CarNo { get; set; }
        public string AccountStatus { get; set; }
        public string ProductName { get; set; }
        public string BranchName { get; set; }
        public string CustomerType { get; set; }
        public string SubscriptionTypeName { get; set; }
        
        public List<ServiceRequestCustomerAccountPhone> PhoneList { get; set; }
    }

    public class ServiceRequestCustomerAccountPhone
    {
        public string PhoneTypeCode { get; set; }
        public string PhoneTypeName { get; set; }
        public string PhoneNo { get; set; }
        public string PhoneExt { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    public class ServiceRequestCustomerAccount
    {
        public ServiceRequestCustomerAccount()
        {
        }

        public CustomerEntity Customer { get; set; }
        public AccountEntity Account { get; set; }

    }


    public class ServiceRequestAccountSearchFilter : SearchFilter
    {
        public int CustomerId { get; set; }
        public string AccountNo { get; set; }
        public string CarNo { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public string BranchName { get; set; }
        public string Status { get; set; }
    }

    public class ServiceRequestAccountSearchResult : SearchFilter
    {
        public int AccountId { get; set; }
        public string AccountNo { get; set; }
        public string CarNo { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public string BranchName { get; set; }
        public string Status { get; set; }
        public bool? IsDefault { get; set; }
    }

    public class ServiceRequestContactSearchResult
    {
		public ServiceRequestContactSearchResult()
        {
            CustomerPhones = new List<ServiceRequestCustomerAccountPhone>();
        }

        public int? ContactId { get; set; }
        public string FirstNameTh { get; set; }
        public string LastNameTh { get; set; }
        public string CardNo { get; set; }
        public string AccountNo { get; set; }
        public int RelationshipId { get; set; }
        public string RelationName { get; set; }
        public string PhoneNo { get; set; }
        public string SubscriptionType { get; set; }
        public DateTime? BirthDay { get; set; }
        public string TitleTh { get; set; }
        public string TitleEn { get; set; }
        public string FirstNameEn { get; set; }
        public string LastNameEn { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public bool? IsDefault { get; set; }
		public List<ServiceRequestCustomerAccountPhone> CustomerPhones { get; set; }
        public string Product { get; set; }
        public UserEntity UpdateUser { get; set; }
        public string ContactFromSystem { get; set; }

        public string RelationNameDisplay
        {
            get
            {
                //if (this.UpdateUser != null && Constants.SystemName.BDW.Equals(this.UpdateUser.Username))
                //    return string.Format("{0} ({1})", Resource.Lbl_Guarantor, this.RelationName);

                if (this.ContactFromSystem != null && Constants.SystemName.BDW.Equals(this.ContactFromSystem))
                    return string.Format("{0} ({1})", Resource.Lbl_Guarantor, this.RelationName);

                return this.RelationName;
            }
        }
    }

    public class AccountAddressSearchResult
    {
        public int AccountAddressId { get; set; }
        public string AccountNo { get; set; }
        public string Product { get; set; }
        public string Type { get; set; }

        public string AddressNo { get; set; }
        public string Village { get; set; }
        public string Building { get; set; }
        public string FloorNo { get; set; }
        public string RoomNo { get; set; }
        public string Moo { get; set; }
        public string Street { get; set; }
        public string Soi { get; set; }
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string Postcode { get; set; }

        public string AddressDisplay
        {
            get
            {
                string strAddress = "";
                strAddress += !string.IsNullOrEmpty(AddressNo) ? string.Format(CultureInfo.InvariantCulture, "เลขที่ {0} ", AddressNo) : string.Empty;
                strAddress += !string.IsNullOrEmpty(Village) ? string.Format(CultureInfo.InvariantCulture, " หมู่บ้าน {0} ", Village) : string.Empty;
                strAddress += !string.IsNullOrEmpty(Building) ? string.Format(CultureInfo.InvariantCulture, " อาคาร {0} ", Building) : string.Empty;
                strAddress += !string.IsNullOrEmpty(FloorNo) ? string.Format(CultureInfo.InvariantCulture, " ชั้น {0} ", FloorNo) : string.Empty;
                strAddress += !string.IsNullOrEmpty(RoomNo) ? string.Format(CultureInfo.InvariantCulture, " ห้อง {0} ", RoomNo) : string.Empty;
                strAddress += !string.IsNullOrEmpty(Moo) ? string.Format(CultureInfo.InvariantCulture, " หมู่ {0} ", Moo) : string.Empty;
                strAddress += !string.IsNullOrEmpty(Street) ? string.Format(CultureInfo.InvariantCulture, " ถนน {0} ", Street) : string.Empty;
                strAddress += !string.IsNullOrEmpty(Soi) ? string.Format(CultureInfo.InvariantCulture, " ซอย {0} ", Soi) : string.Empty;
                strAddress += !string.IsNullOrEmpty(SubDistrict) ? string.Format(CultureInfo.InvariantCulture, " แขวง {0} ", SubDistrict) : string.Empty;
                strAddress += !string.IsNullOrEmpty(District) ? string.Format(CultureInfo.InvariantCulture, " เขต {0} ", District) : string.Empty;
                strAddress += !string.IsNullOrEmpty(Province) ? string.Format(CultureInfo.InvariantCulture, " จังหวัด {0} ", Province) : string.Empty;
                strAddress += !string.IsNullOrEmpty(Postcode) ? string.Format(CultureInfo.InvariantCulture, " รหัสไปรษณีย์ {0}", Postcode) : string.Empty;
                strAddress += !string.IsNullOrEmpty(Country) ? string.Format(CultureInfo.InvariantCulture, " ประเทศ {0} ", Country) : string.Empty;

                return strAddress;
            }
        }
    }

    public class SrAttachmentSearchFilter : Pager
    {
        public int? SrId { get; set; }
    }

    public class SrTransferEntity
    {
        public int SrId { get; set; }

        public int TransferToUserId { get; set; }

        public bool IsTransferOwner { get; set; }
    }

    public class TransferOwnerDelegateResult
    {
        public TransferOwnerDelegateResult()
        {
            ErrorMessages = new List<string>();
        }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessages { get; set; } 
    }

    public class GroupAssignEntity
    {
        public int BranchId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Email { get; set; }

        public List<ServiceRequestWithStatusEntity> OwnerSrList { get; set; }
        public List<ServiceRequestWithStatusEntity> DelegateSrList { get; set; }
        public List<ServiceRequestWithStatusEntity> CreateSrList { get; set; }

        public int CountByStatus(string status)
        {
            var sum = 0;

            if (status == Constants.SRStatusCode.Draft)
            {
                // DRAFT Status
                if (CreateSrList != null && CreateSrList.Count != 0)
                    sum += CreateSrList.Count(x => x.SrStatusCode == status);
            }
            else
            {
                // Other Status
                if (OwnerSrList != null && OwnerSrList.Count != 0)
                    sum += OwnerSrList.Count(x => x.SrStatusCode == status);

                if (DelegateSrList != null && DelegateSrList.Count != 0)
                    sum += DelegateSrList.Count(x => x.SrStatusCode == status);
            }

            return sum;
        }

        public int CountDraft { get { return CountByStatus(Constants.SRStatusCode.Draft); } }
        public int CountOpen { get { return CountByStatus(Constants.SRStatusCode.Open); } }
        public int CountInProgress { get { return CountByStatus(Constants.SRStatusCode.InProgress); } }
        public int CountWaitingCustomer { get { return CountByStatus(Constants.SRStatusCode.WaitingCustomer); } }
        public int CountRouteBack { get { return CountByStatus(Constants.SRStatusCode.RouteBack); } }

        public int CountSummary
        {
            get
            {
                //return CountDraft + CountOpen + CountWaitingCustomer + CountInProgress + CountRouteBack;
                return CountOpen + CountInProgress + CountWaitingCustomer + CountRouteBack;
            }
        }

        public int CountByState(int stateId)
        {
            var sum = 0;

            if (stateId == Constants.SRStateId.Draft)
            {
                // DRAFT Status
                if (CreateSrList != null && CreateSrList.Count != 0)
                    sum += CreateSrList.Count(x => x.SrStateId == stateId);
            }
            else
            {
                // Other Status
                if (OwnerSrList != null && OwnerSrList.Count != 0)
                    sum += OwnerSrList.Count(x => x.SrStateId == stateId);

                if (DelegateSrList != null && DelegateSrList.Count != 0)
                    sum += DelegateSrList.Count(x => x.SrStateId == stateId);
            }
            return sum;
        }
    }

    public class UserAssignEntity
    {
        public int UserId { get; set; }
        public bool IsCurrentUser { get; set; }
        public string RoleName { get; set; }
        public string Username { get; set; }
        public string UserPositionCode { get; set; }
        public string UserFirstname { get; set; }
        public string UserLastname { get; set; }
        public string UserFullname
        {
            get
            {
                string[] names = new string[2] { this.UserFirstname.NullSafeTrim(), this.UserLastname.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    string positionCode = this.UserPositionCode.NullSafeTrim();
                    string fullName = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                    return !string.IsNullOrEmpty(positionCode) ? string.Format(CultureInfo.InvariantCulture, "{0} - {1}", positionCode, fullName) : fullName;
                }

                return string.Empty;
            }
        }
        public string BranchName { get; set; }

        public List<ServiceRequestWithStatusEntity> OwnerSrList { get; set; }
        public List<ServiceRequestWithStatusEntity> DelegateSrList { get; set; }
        public List<ServiceRequestWithStatusEntity> CreateSrList { get; set; }

        public int CountByStatus(string status)
        {
            var sum = 0;

            if (status == Constants.SRStatusCode.Draft)
            {
                // DRAFT Status
                if (CreateSrList != null && CreateSrList.Count != 0)
                    sum += CreateSrList.Count(x => x.SrStatusCode == status);
            }
            else
            {
                // Other Status
                if (OwnerSrList != null && OwnerSrList.Count != 0)
                    sum += OwnerSrList.Count(x => x.SrStatusCode == status);

                if (DelegateSrList != null && DelegateSrList.Count != 0)
                    sum += DelegateSrList.Count(x => x.SrStatusCode == status);
            }

            return sum;
        }

        public int CountDraft { get { return CountByStatus(Constants.SRStatusCode.Draft); } }
        public int CountOpen { get { return CountByStatus(Constants.SRStatusCode.Open); } }
        public int CountInProgress { get { return CountByStatus(Constants.SRStatusCode.InProgress); } }
        public int CountWaitingCustomer { get { return CountByStatus(Constants.SRStatusCode.WaitingCustomer); } }
        public int CountRouteBack { get { return CountByStatus(Constants.SRStatusCode.RouteBack); } }

        public int CountSummary
        {
            get
            {
                //return CountDraft + CountOpen + CountWaitingCustomer + CountInProgress + CountRouteBack;
                return CountOpen + CountWaitingCustomer + CountInProgress + CountRouteBack;
            }
        }

        public int CountByState(int stateId)
        {
            var sum = 0;

            if (stateId == Constants.SRStateId.Draft)
            {
                // DRAFT Status
                if (CreateSrList != null && CreateSrList.Count != 0)
                    sum += CreateSrList.Count(x => x.SrStateId == stateId);
            }
            else
            {
                // Other Status
                if (OwnerSrList != null && OwnerSrList.Count != 0)
                    sum += OwnerSrList.Count(x => x.SrStateId == stateId);

                if (DelegateSrList != null && DelegateSrList.Count != 0)
                    sum += DelegateSrList.Count(x => x.SrStateId == stateId);
            }
            return sum;
        }
    }


    public class ServiceRequestWithStatusEntity
    {
        public int SrId { get; set; }
        public string SrStatusCode { get; set; }
        public int? SrStateId { get; set; }
    }

    public class SRReplyEmailEntity
    {
        public int SrReplyEmailId { get; set; }
        public int SrId { get; set; }
        public int NewSrStatusId { get; set; }
        public int OldSrStatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool IsProcess { get; set; }
        public int? JobId { get; set; }
        public bool IsChangeStatus { get; set; }
        public bool CanChangeStatus { get; set; }
        public int OwnerBranchId { get; set; }
        public int OwnerUserId { get; set; }
        public int? DelegateBranchId { get; set; }
        public int? DelegateUserId { get; set; }
        public int? CreateUserId { get; set; }
        public string CreateUsername { get; set; }
        public string EmailFrom { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public List<SrAttachmentEntity> SrAttachments { get; set; }

        public DateTime? ProcessDate { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ResubmitToCAREntity
    {
        public int SrActivityId { get; set; }
        public int NewSrStatusId { get; set; }
        public int OldSrStatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool IsProcess { get; set; }
        public int? JobId { get; set; }
        public bool IsChangeStatus { get; set; }
        public bool CanChangeStatus { get; set; }
        public int OwnerBranchId { get; set; }
        public int OwnerUserId { get; set; }
        public int? DelegateBranchId { get; set; }
        public int? DelegateUserId { get; set; }
        public int? CreateUserId { get; set; }
        public string CreateUsername { get; set; }
        public List<SrAttachmentEntity> SrAttachments { get; set; }

        public DateTime? ProcessDate { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class SingleMapProductEntity
    {
        public bool IsSingleQuery { get; set; }
        public int? CampaignServiceId { get; set; }
        public string CampaignServiceName { get; set; }
        public int? AreaId { get; set; }
        public string AreaName { get; set; }
        public int? SubAreaId { get; set; }
        public string SubAreaName { get; set; }
        public int? TypeId { get; set; }
        public string TypeName { get; set; }
        public int? ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
    }

    public class SendOTPEntity
    {
        public int? SendOTPId { get; set; }
        public string CSM_RefNo { get; set; }
        public string CallId { get; set; }
        public string MobileNo { get; set; }
        public string IVRLang { get; set; }
        public string TxnId { get; set; }
        public string CardNo { get; set; }
        public string CardType { get; set; }
        public string UserAction { get; set; }
        public string ClientIP { get; set; }
        public string Method { get; set; }
        public string TemplateId { get; set; }
        public string ProductDesc { get; set; }
        public string ReserveField1 { get; set; }
        public string RequestIVRRefNo { get; set; }
        public string RequestStatusCode { get; set; }
        public string RequestStatusCodeDisplay { get; set; }
        public string RequestErrorCode { get; set; }
        public string RequestErrorDesc { get; set; }
        public string RequestCaaErrorCode { get; set; }
        public string RequestCaaErrorDesc { get; set; }
        public string RequestOTPPrefix { get; set; }
        public string RequestOTPDetail { get; set; }
        public string RequestOTPSuffix { get; set; }
        public DateTime? RequestDate { get; set; }
        public string ResultIVRRefNo { get; set; }
        public string ResultStatusCode { get; set; }
        public string ResultStatusCodeDisplay { get; set; }
        public string ResultErrorCode { get; set; }
        public string ResultErrorDesc { get; set; }
        public string ResultCaaErrorCode { get; set; }
        public string ResultCaaErrorDesc { get; set; }
        public string ResultOTPPrefix { get; set; }
        public string ResultOTPDetail { get; set; }
        public string ResultOTPSuffix { get; set; }
        public DateTime? ResultDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateUser { get; set; }
    }
}
