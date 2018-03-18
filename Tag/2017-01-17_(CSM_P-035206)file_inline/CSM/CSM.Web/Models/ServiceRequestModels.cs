using CSM.Common.Resources;
using CSM.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CSM.Common.Utilities;
using CSM.Entity.Common;
using Newtonsoft.Json;

namespace CSM.Web.Models
{
    public class SearchServiceRequestViewModel
    {
        public bool IsShowAdvanceSearch { get; set; }
        public ServiceRequestSearchFilter SearchFilter { get; set; }
        public IEnumerable<ServiceRequestEntity> ServiceRequestList { get; set; }
        public string ServiceRequestStatusList { get; set; }
    }

    public class EditServiceRequestViewModel
    {
        public ServiceRequestNoDetailEntity Entity { get; set; }
        public bool CanEdit { get; set; }

        #region "SR Tab"

        //edit SR Activity tab
        public ActivityTabSearchFilter ActivitySearchFilter { get; set; }
        public IEnumerable<ServiceRequestActivityResult> ActivityList { get; set; }

        //edit SR Existing tab
        public ExistingSearchFilter ExistingSearchFilter { get; set; }
        public IEnumerable<ServiceRequestEntity> ExistingList { get; set; }

        //edit SR Document tab
        public DocumentSearchFilter DocumentSearchFilter { get; set; }
        public IEnumerable<ServiceDocumentEntity> DocumentList { get; set; }

        //edit SR Logging tab
        public LoggingSearchFilter LoggingSearchFilter { get; set; }
        public IEnumerable<ServiceRequestLoggingResult> LoggingResultList { get; set; }

        #endregion
    }


    public class ValidateResult
    {
        public ValidateResult() { }
        public ValidateResult(bool isValid, string errorMessage)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }

        public bool IsValid { get; set; }

        public string ErrorMessage { get; set; }

    }

    public class CreateServiceRequestViewModel
    {
        public CreateServiceRequestViewModel()
        {
            CanEdit = true;
        }

        public bool IsEdit { get; set; }

        public string PhoneNo { get; set; }
        public string CallId { get; set; }

        public int? CreatorBranchId { get; set; }
        public string CreatorBranchCode { get; set; }
        public string CreatorBranchName { get; set; }
        public int CreatorUserId { get; set; }
        public string CreatorUserFullName { get; set; }

        [Display(Name = "รหัสสินทรัพย์รอขาย")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? AfsAssetId { get; set; }

        public string AfsAssetNo { get; set; }
        public string AfsAssetDesc { get; set; }


        [Display(Name = "วันเกิด/วันที่จดทะเบียน")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string NCBBirthDate { get; set; }


        #region == Step 1 ==

        // Section 1: Customer Profile Information
        [Display(Name = "ลูกค้า")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? CustomerId { get; set; }

        // New Customer modal
        public string SubscriptType { get; set; }
        public SelectList SubscriptTypeList { get; set; }
        public string CardNo { get; set; }
        public string ModalCustomerBirthDate { get; set; }
        public string TitleThai { get; set; }
        public SelectList TitleThaiList { get; set; }

        [Display(Name = "Lbl_FirstNameThai", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex(@"([\-ก-๙0-9(). ]+)", "ValErr_NoSpecialCharacterThai")]
        public string FirstNameThai { get; set; }

        [LocalizedRegex(@"([\-ก-๙0-9(). ]+)", "ValErr_NoSpecialCharacterThai")]
        public string LastNameThai { get; set; }
        public string TitleEnglish { get; set; }
        public SelectList TitleEnglishList { get; set; }

        [Display(Name = "Lbl_FirstNameEnglish", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_Required")]
        [LocalizedRegex(@"([\-a-zA-Z0-9(). ]+)", "ValErr_NoSpecialCharacterEnglish")]
        public string FirstNameEnglish { get; set; }

        [LocalizedRegex(@"([\-a-zA-Z0-9(). ]+)", "ValErr_NoSpecialCharacterEnglish")]
        public string LastNameEnglish { get; set; }

        [Display(Name = "Lbl_PhoneNo", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_Required")]
        public string PhoneType1 { get; set; }
        public SelectList PhoneTypeList { get; set; }

        [Display(Name = "Lbl_PhoneNo", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        [LocalizedStringLength(Constants.MaxLength.PhoneNo, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneNo1 { get; set; }
        public string PhoneType2 { get; set; }

        [Display(Name = "Lbl_PhoneNo", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]
        [LocalizedRequired(ErrorMessage = "ValErr_Required")]
        [LocalizedStringLength(Constants.MaxLength.PhoneNo, ErrorMessageResourceName = "ValErr_StringLength",
           ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneNo2 { get; set; }
        public string PhoneType3 { get; set; }

        [Display(Name = "Lbl_PhoneNo", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]
        [LocalizedRequired(ErrorMessage = "ValErr_Required")]
        [LocalizedStringLength(Constants.MaxLength.PhoneNo, ErrorMessageResourceName = "ValErr_StringLength",
           ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneNo3 { get; set; }

        [Display(Name = "Lbl_Fax", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]
        [LocalizedStringLength(Constants.MaxLength.PhoneNo, ErrorMessageResourceName = "ValErr_StringLength",
           ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Fax { get; set; }

        [Display(Name = "Lbl_Email", ResourceType = typeof(CSM.Common.Resources.Resource))]
        //[DataType(DataType.EmailAddress)]
        //[LocalizedRegex("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", "ValErr_InvalidEmail")]
        public string Email { get; set; }

        //end modal customer

        //Modal New Account

        public string AccoutSubscriptType { get; set; }
        public SelectList AccountSubscriptTypeList { get; set; }
        public string AccountCardNo { get; set; }
        public string AccountBirthDate { get; set; }
        public string AccountTitleThai { get; set; }
        public SelectList AccountTitleThaiList { get; set; }

        [Display(Name = "Lbl_FirstNameThai", ResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AccountFirstNameThai { get; set; }
        public string AccountLastNameThai { get; set; }
        public string AccountTitleEnglish { get; set; }
        public SelectList AccountTitleEnglishList { get; set; }

        [Display(Name = "Lbl_FirstNameEnglish", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_Required")]
        public string AccountFirstNameEnglish { get; set; }
        public string AccountLastNameEnglish { get; set; }

        [Display(Name = "Lbl_PhoneNo", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_Required")]
        public string AccountPhoneType1 { get; set; }
        public SelectList AccountPhoneTypeList { get; set; }

        [Display(Name = "Lbl_PhoneNo", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        [LocalizedStringLength(Constants.MaxLength.PhoneNo, ErrorMessageResourceName = "ValErr_StringLength",
           ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AccountPhoneNo1 { get; set; }
        public string AccountPhoneType2 { get; set; }

        [Display(Name = "Lbl_PhoneNo", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]
        [LocalizedRequired(ErrorMessage = "ValErr_Required")]
        [LocalizedStringLength(Constants.MaxLength.PhoneNo, ErrorMessageResourceName = "ValErr_StringLength",
           ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AccountPhoneNo2 { get; set; }
        public string AccountPhoneType3 { get; set; }

        [Display(Name = "Lbl_PhoneNo", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]
        [LocalizedRequired(ErrorMessage = "ValErr_Required")]
        [LocalizedStringLength(Constants.MaxLength.PhoneNo, ErrorMessageResourceName = "ValErr_StringLength",
           ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AccountPhoneNo3 { get; set; }

        [Display(Name = "Lbl_Fax", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]
        [LocalizedStringLength(Constants.MaxLength.PhoneNo, ErrorMessageResourceName = "ValErr_StringLength",
           ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AccountFax { get; set; }

        [Display(Name = "Lbl_Email", ResourceType = typeof(CSM.Common.Resources.Resource))]
        //[DataType(DataType.EmailAddress)]
        //[LocalizedRegex("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", "ValErr_InvalidEmail")]
        public string AccountEmail { get; set; }

        [Display(Name = "Lbl_AccountNo", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public int? AccountAccountId { get; set; }
        public SelectList AccountList { get; set; }
        public string AccountProduct { get; set; }

        [Display(Name = "Lbl_Relationship", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        [LocalizedRequired(ErrorMessage = "ValErr_Required")]
        public int? RelationshipId { get; set; }

        public SelectList RelationshipList { get; set; }

        //end modal account


        // Section 2: Contact Profile Information
        [Display(Name = "บัญชีลูกค้า")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? AccountId { get; set; }

        [Display(Name = "ผู้ติดต่อ")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? ContactId { get; set; }


        // Section 3: Service Request Information

        [Display(Name = "Campaign/Service")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? CampaignServiceId { get; set; }

        public string CampaignServiceName { get; set; }
        
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public int? ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }

        [Display(Name = "Area")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? AreaId { get; set; }
        public string AreaName { get; set; }

        [Display(Name = "Sub Area")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? SubAreaId { get; set; }
        public string SubAreaName { get; set; }

        [Display(Name = "Type")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? TypeId { get; set; }
        public string TypeName { get; set; }

        [Display(Name = "Media Source")]
        public int? MediaSourceId { get; set; }
        public string MediaSourceName { get; set; }

        [Display(Name = "SR Channel")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? ChannelId { get; set; }
        public string ChannelName { get; set; }


        public int? SrEmailTemplateId { get; set; }
        public string SrEmailTemplateName { get; set; }

        [Display(Name = "Activity Type")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? ActivityTypeId { get; set; }
        public string ActivityTypeName { get; set; }

        public List<SelectListItem> CampaignServices { get; set; }
        public List<SelectListItem> Channels { get; set; }
        public List<SelectListItem> MediaSources { get; set; }
        public List<SelectListItem> SrEmailTemplates { get; set; }
        public List<SelectListItem> ActivityTypes { get; set; }


        [Display(Name = "Subject")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        [MaxLength(Constants.MaxLength.MailSubject, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "ValErr_StringLength")]
        public string Subject { get; set; }

        //[MaxLength(1000, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "ValErr_StringLength")]
        [AllowHtml]
        public string Remark { get; set; }
        [AllowHtml]
        public string RemarkOriginal { get; set; }

        //AdHoc
        [AllowHtml]
        public string Remark3 { get; set; }
        [AllowHtml]
        public string Remark3Original { get; set; }

        public string CommandButton { get; set; }

        #endregion

        #region == Step 2 ==

        public int? MappingProductId { get; set; }

        public bool IsVerify { get; set; }

        [Display(Name = "Verify")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredVerify")]
        public string IsVerifyPass { get; set; }

        public string VerifyAnswerJson { get; set; }


        /// <summary>
        /// For View Only
        /// </summary>
        public List<SrVerifyGroupEntity> VerifyGroups { get; set; }

        public int? SrPageId { get; set; }
        public string SrPageCode { get; set; }

        public MappingProductEntity MappingProduct { get; set; }
        public string JsonQuestionGroups
        {
            get
            {
                if (MappingProduct == null)
                    return string.Empty;
                if (MappingProduct.MappingProductQuestionGroups.Count == 0)
                    return string.Empty;

                var isFirstRow = true;

                var str = new StringBuilder();
                str.Append("[");

                foreach (var grp in MappingProduct.MappingProductQuestionGroups)
                {
                    if (!isFirstRow)
                        str.Append(",");

                    str.Append("{");
                    str.AppendFormat("\\\"QuestionGroupId\\\":{0},\\\"RequireAmountPass\\\":{1}", grp.QuestionGroupId, grp.RequireAmountPass);
                    str.Append("}");

                    isFirstRow = false;
                }
                str.Append("]");

                return str.ToString();
            }
        }

        public string OTPRefNo { get; set; }
        public int[] CheckMailToDelegateUsers { get; set; }
        public int[] UnCheckMailToDelegateUsers { get; set; }

        #endregion


        #region == Step 3 ==

        public int? AddressSendDocId { get; set; }
        public string AddressSendDocText { get; set; }



        [Display(Name = "Owner Branch")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? OwnerBranchId { get; set; }

        public string OwnerBranchName { get; set; }


        [Display(Name = "Owner SR")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? OwnerUserId { get; set; }

        public string OwnerUserFullName { get; set; }
        public int? DelegateBranchId { get; set; }
        public string DelegateBranchName { get; set; }
        public int? DelegateUserId { get; set; }
        public string DelegateUserFullName { get; set; }

        public int? DefaultOwnerUserId { get; set; }
        public string DefaultOwnerUserFullName { get; set; }
        public int? DefaultOwnerBranchId { get; set; }
        public string DefaultOwnerBranchName { get; set; }

        public bool IsEmailDelegate { get; set; }
        public bool IsClose { get; set; }

        #region Step 3 Complaint Part

        public ProductGroupEntity CPN_ProductGroup { get; set; } = new ProductGroupEntity();
        public ProductEntity CPN_Product { get; set; } = new ProductEntity();
        public CampaignServiceEntity CPN_Campaign { get; set; } = new CampaignServiceEntity();
        public ComplaintSubjectEntity CPN_Subject { get; set; } = new ComplaintSubjectEntity();
        public ComplaintTypeEntity CPN_Type { get; set; } = new ComplaintTypeEntity();
        public RootCauseEntity CPN_RootCause { get; set; } = new RootCauseEntity();
        public ComplaintIssuesEntity CPN_Issues { get; set; } = new ComplaintIssuesEntity();
        public ComplaintMappingEntity CPN_Mapping { get; set; } = new ComplaintMappingEntity();
        public bool CPN_IsSecret { get; set; }
        public bool CPN_IsCar { get; set; }
        public bool CPN_IsHPLog { get; set; }
        public ComplaintBUGroupEntity CPN_BUGroup { get; set; } = new ComplaintBUGroupEntity();

        public bool? CPN_IsSummary { get; set; }
        public bool CPN_Cause_Customer { get; set; }
        public bool CPN_Cause_Staff { get; set; }
        public bool CPN_Cause_System { get; set; }
        public bool CPN_Cause_Process { get; set; }
        public string CPN_Cause_Customer_Detail { get; set; }
        public string CPN_Cause_Staff_Detail { get; set; }
        public string CPN_Cause_System_Detail { get; set; }
        public string CPN_Cause_Process_Detail { get; set; }

        public ComplaintCauseSummaryEntity CPN_CauseSummary { get; set; } = new ComplaintCauseSummaryEntity();
        public ComplaintSummaryEntity CPN_Summary { get; set; } = new ComplaintSummaryEntity();

        public string CPN_Fixed_Detail { get; set; }

        public ComplaintBUEntity CPN_BU1 { get; set; }
        public ComplaintBUEntity CPN_BU2 { get; set; }
        public ComplaintBUEntity CPN_BU3 { get; set; }
        public MSHBranchEntity CPN_MSHBranch { get; set; }

        #endregion

        [Display(Name = "รายละเอียดการติดต่อ")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string ActivityDescription { get; set; }

        [Display(Name = "Sender E-Mail")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string SendMailSender { get; set; }

        [Display(Name = "To E-Mail")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string SendMailTo { get; set; }

        public string SendMailCc { get; set; }
        public string SendMailBcc { get; set; }

        [Display(Name = "Subject E-Mail")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string SendMailSubject { get; set; }

        [AllowHtml]
        [Display(Name = "Body E-Mail")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string SendMailBody { get; set; }

        //SR Attach table
        public SrAttachmentSearchFilter SearchFilter { get; set; }
        public IEnumerable<SrDocumentTypeEntity> SrAttachmentList { get; set; }

        public List<SrDocumentTypeEntity> SrAttachmentForRenderList { get; set; }

        //Attach Modal
        [Display(Name = "Lbl_FileAttach", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public HttpPostedFileBase FileAttach { get; set; }

        [Display(Name = "ชื่อเอกสาร")]
        [MaxLength(200)]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string DocName { get; set; }

        public string DocDesc { get; set; }
        public string ExpiryDate { get; set; }
        public List<CheckBoxes> DocTypeCheckBoxes { get; set; }
        public bool AttachToEmail { get; set; }
        private List<AttachmentTypeEntity> m_AttachType;
        public SelectList StatusList { get; set; }

        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public short? Status { get; set; }

        //edit modal
        public int? SrAttachmentId { get; set; }
        public string DocNameEdit { get; set; }
        public string DocDescEdit { get; set; }
        public string ExpireDateEdit { get; set; }
        public List<CheckBoxes> DocTypeCheckBoxesEdit { get; set; }

        public string AttachmentJson { get; set; }

        public int? TotalAttachmentFileSize { get; set; }

        public string SrDocumentFolder { get; set; }
        public string AllowFileExtensionsRegex { get; set; }
        public string AllowFileExtensionsDesc { get; set; }


        #endregion

        #region CustomerInfo

        public string CustomerSubscriptionTypeCode { get; set; }
        public string CustomerSubscriptionName { get; set; }
        public string CustomerCardNo { get; set; }
        public string CustomerBirthDate { get; set; }
        public string CustomerTitleTh { get; set; }
        public string CustomerFirstNameTh { get; set; }
        public string CustomerLastNameTh { get; set; }
        public string CustomerTitleEn { get; set; }
        public string CustomerFirstNameEn { get; set; }
        public string CustomerLastNameEn { get; set; }
        public string CustomerPhoneTypeName1 { get; set; }
        public string CustomerPhoneTypeName2 { get; set; }
        public string CustomerPhoneTypeName3 { get; set; }
        public string CustomerPhoneNo1 { get; set; }
        public string CustomerPhoneNo2 { get; set; }
        public string CustomerPhoneNo3 { get; set; }
        public string CustomerFax { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerEmployeeCode { get; set; }

        #endregion

        #region AccountInfo
        public string AccountNo { get; set; }
        public string AccountStatus { get; set; }
        public string AccountCarNo { get; set; }
        public string AccountProductGroupName { get; set; }
        public string AccountProductName { get; set; }
        public string AccountBranchName { get; set; }

        #endregion

        #region ContactInfo
        public string ContactSubscriptionName { get; set; }
        public string ContactCardNo { get; set; }
        public string ContactBirthDate { get; set; }
        public string ContactTitleTh { get; set; }
        public string ContactFirstNameTh { get; set; }
        public string ContactLastNameTh { get; set; }
        public string ContactTitleEn { get; set; }
        public string ContactFirstNameEn { get; set; }
        public string ContactLastNameEn { get; set; }
        public string ContactPhoneTypeName1 { get; set; }
        public string ContactPhoneTypeName2 { get; set; }
        public string ContactPhoneTypeName3 { get; set; }
        public string ContactPhoneNo1 { get; set; }
        public string ContactPhoneNo2 { get; set; }
        public string ContactPhoneNo3 { get; set; }
        public string ContactFax { get; set; }
        public string ContactEmail { get; set; }

        public string ContactAccountNo { get; set; }

        public int? ContactRelationshipId { get; set; }
        public string ContactRelationshipName { get; set; }

        #endregion



        [Display(Name = "NCB Check Status")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string NCBCheckStatus { get; set; }
        public List<SelectListItem> NCBCheckStatuses { get; set; }

        public int? NCBMarketingUserId { get; set; }
        public string NCBMarketingName { get; set; }
        public int? NCBMarketingBranchId { get; set; }
        public string NCBMarketingBranchName { get; set; }
        public int? NCBMarketingBranchUpper1Id { get; set; }
        public string NCBMarketingBranchUpper1Name { get; set; }
        public int? NCBMarketingBranchUpper2Id { get; set; }
        public string NCBMarketingBranchUpper2Name { get; set; }

        public ServiceRequestEntity ServiceRequest { get; set; }
        public ServiceRequestForDisplayEntity ServiceRequestForDisplayEntity { get; set; }
        public int? SrId { get; set; }
        public string SrNo { get; set; }


        // *** Use for Page Parameter only
        public int? CustomerContactId { get; set; }


        [Display(Name = "SR Status")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int SRStatusId { get; set; }

        public string SRStatusName { get; set; }
        public string SRStateName { get; set; }

        public DateTime? CloseDate { get; set; }
        public string CloseDateDisplay { get { return DateUtil.ToStringAsDateTime(CloseDate); } }

        public bool CanEdit { get; set; }

        public string OfficePhoneNo { get; set; }
        public string OfficeHour { get; set; }

        #region "SR Tab"

        //edit SR Activity tab
        public ActivityTabSearchFilter ActivitySearchFilter { get; set; }
        public IEnumerable<ServiceRequestActivityResult> ActivityList { get; set; } 

        //edit SR Existing tab
        public ExistingSearchFilter ExistingSearchFilter { get; set; }
        public IEnumerable<ServiceRequestEntity> ExistingList { get; set; } 

        //edit SR Document tab
        public DocumentSearchFilter DocumentSearchFilter { get; set; }
        public IEnumerable<ServiceDocumentEntity> DocumentList { get; set; }

        //edit SR Logging tab
        public LoggingSearchFilter LoggingSearchFilter { get; set; }
        public IEnumerable<ServiceRequestLoggingResult> LoggingResultList { get; set; }

        #endregion

        public string DocTypeIds { get; set; }
        public string DocumentLevel { get; set; }

        public List<AttachmentTypeEntity> AttachTypeList
        {
            get { return m_AttachType; }
            set { m_AttachType = value; }
        }
    }

    public class SrActivityViewModel
    {
        public int? SrId { get; set; }
        public string SrNo { get; set; }
        public string CreateDateForEmailTemplate { get; set; }

        public string PhoneNo { get; set; }
        public string CallId { get; set; }

        [Display(Name = "Owner Branch")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? OwnerBranchId { get; set; }
        public string OwnerBranchName { get; set; }


        [Display(Name = "Owner SR")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? OwnerUserId { get; set; }
        public string OwnerUserFullName { get; set; }

        public int? DelegateBranchId { get; set; }
        public string DelegateBranchName { get; set; }

        public int? DelegateUserId { get; set; }
        public string DelegateUserFullName { get; set; }

        public bool IsEmailDelegate { get; set; }


        [Display(Name = "SR Status")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? SrStatusId { get; set; }
        public string SrStatusName { get; set; }
        public List<SelectListItem> SrStatuses { get; set; }
        public int? SrStateId { get; set; }
        public string SrStateName { get; set; }

        public string RuleAssignFlag { get; set; }

        public bool IsRuleAssigned
        {
            get
            {
                if (string.IsNullOrEmpty(RuleAssignFlag) || RuleAssignFlag == "0")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsStatusClosedOrCancelled
        {
            get
            {
                return SrStatusId == Constants.SRStatusId.Closed || SrStatusId == Constants.SRStatusId.Cancelled;
            }
        }


        public int? SrEmailTemplateId { get; set; }
        public string SrEmailTemplateName { get; set; }
        public List<SelectListItem> SrEmailTemplates { get; set; }


        [Display(Name = "รายละเอียดการติดต่อ")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string ActivityDescription { get; set; }


        [Display(Name = "Sender E-Mail")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string SendMailSender { get; set; }

        [Display(Name = "Sender E-Mail")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string SendMailTo { get; set; }
        public string SendMailCc { get; set; }
        public string SendMailBcc { get; set; }

        [Display(Name = "Subject E-Mail")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string SendMailSubject { get; set; }

        [AllowHtml]
        [Display(Name = "Body E-Mail")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string SendMailBody { get; set; }


        [Display(Name = "Activity Type")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? ActivityTypeId { get; set; }
        public string ActivityTypeName { get; set; }
        public List<SelectListItem> ActivityTypes { get; set; }


        //Attach Modal
        [Display(Name = "Lbl_FileAttach", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public HttpPostedFileBase FileAttach { get; set; }

        [Display(Name = "ชื่อเอกสาร")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string DocName { get; set; }

        public string DocDesc { get; set; }
        public string ExpiryDate { get; set; }
        public List<CheckBoxes> DocTypeCheckBoxes { get; set; }
        public bool AttachToEmail { get; set; }
        private List<AttachmentTypeEntity> m_AttachType;

        public string AttachmentJson { get; set; }

        public DocumentSearchFilter DocumentSearchFilter { get; set; }
        public IEnumerable<ServiceDocumentEntity> DocumentList { get; set; }

        // For Email Template
        public string CustomerFirstNameTh { get; set; }
        public string CustomerLastNameTh { get; set; }
        public string CustomerPhoneNo1 { get; set; }
        public string AccountNo { get; set; }
        public string ContactFirstNameTh { get; set; }
        public string ContactLastNameTh { get; set; }
        public string ContactPhoneNo1 { get; set; }
        public string CreatorBranchCode { get; set; }
        public string CreatorBranchName { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public string CampaignServiceName { get; set; }
        public string TypeName { get; set; }
        public string AreaName { get; set; }
        public string SubAreaName { get; set; }
        public string ChannelName { get; set; }
        public string Remark { get; set; }
        public string RemarkOriginal { get; set; }

        public string CustomerCardNo { get; set; }
        public string RemarkSubject { get; set; }
        public string CPN_ProductGroupName { get; set; }
        public string CPN_ProductName { get; set; }
        public string CPN_CampaignName { get; set; }
        public string CPN_SubjectName { get; set; }
        public string CPN_TypeName { get; set; }
        public string CPN_RootCauseName { get; set; }
        public string CPN_IssuesName { get; set; }
        public bool? CPN_IsSecret { get; set; }

        // public string OwnerUserFullName { get; set; } // Duplicated with old property
        public string NCBMarketingName { get; set; }
        public string CreatorUserFullName { get; set; }
        public string OfficePhoneNo { get; set; }
        public string OfficeHour { get; set; }

        public string AllowFileExtensionsRegex { get; set; }
        public string AllowFileExtensionsDesc { get; set; }
        public string SrDocumentFolder { get; set; }
        public int? SRPageId { get; set; }
    }

    public class ServiceRequestQuestionGroupViewModel
    {
        public int QuestionGroupId { get; set; }
        public string QuestionGroupName { get; set; }
        public int MinimumPassAnswer { get; set; }

        public List<ServiceRequestQuestionViewModel> Questions { get; set; }
    }

    public class ServiceRequestQuestionViewModel
    {
        public int QuestionGroupQuestionId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionName { get; set; }
    }



    public class SearchCustomerViewModel
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public string CardNo { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string AccountNo { get; set; }
        public string PhoneNo { get; set; }
        public string CarNo { get; set; }
        public string AccountStatus { get; set; }
        public string ProductName { get; set; }
        public string BranchName { get; set; }
        public string CustomerType { get; set; }
        public string SubscriptionTypeName { get; set; }
    }

    public class SearchGroupAssignViewModel
    {
        public GroupAssignSearchFilter SearchFilter { get; set; }
        public IEnumerable<GroupAssignEntity> ResultList { get; set; }
    }

    public class SearchUserAssignViewModel
    {
        public UserAssignSearchFilter SearchFilter { get; set; }
        public IEnumerable<UserAssignEntity> ResultList { get; set; }
    }

    public class UserMonitoringSrViewModel
    {
        public UserMonitoringSrSearchFilter SearchFilter { get; set; }
        public IEnumerable<ServiceRequestEntity> ResultList { get; set; }
    }

    public class FileNameViewModel
    {
        public string Name { get; set; }
    }

    public class RenderEmailTemplateViewModel : SrEmailTemplateEntity
    {
        public string CustomerFirstNameTh { get; set; }
        public string CustomerLastNameTh { get; set; }
        public string CustomerPhoneNo1 { get; set; }
        public string AccountNo { get; set; }

        public string ContactFirstNameTh { get; set; }
        public string ContactLastNameTh { get; set; }
        public string ContactPhoneNo1 { get; set; }

        public string CreatorBranchCode { get; set; }
        public string CreatorBranchName { get; set; }

        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public string CampaignServiceName { get; set; }
        public string TypeName { get; set; }
        public string AreaName { get; set; }
        public string SubAreaName { get; set; }
        public string ChannelName { get; set; }

        public string RemarkSubject { get; set; }
        public string Remark { get; set; }

        public string OwnerUserFullName { get; set; }
        public string CreatorUserFullName { get; set; }

        public string OfficePhoneNo { get; set; }
        public string OfficeHour { get; set; }
        public string CustomerCardNo { get; set; }

        public string CPN_ProductGroupName { get; set; }
        public string CPN_ProductName { get; set; }
        public string CPN_CampaignName { get; set; }
        public string CPN_SubjectName { get; set; }
        public string CPN_TypeName { get; set; }
        public string CPN_RootCauseName { get; set; }
        public string CPN_IssueName { get; set; }
        public string IsCreateActivity { get; set; }
    }

    public class SendOTPHistoryVM
    {
        public List<SendOTPEntity> List { get; set; }
    }
}