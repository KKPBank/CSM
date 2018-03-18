using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class BdwContactEntity
    {
        public int? BdwContactId { get; set; }

        public string DataType { get; set; }
        public string DataDate { get; set; }
        public string DataSource { get; set; }

        [Display(Name = "CARD_NO")]
        [LocalizedStringLength(Constants.MaxLength.BdwCardNo, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardNo { get; set; }
        [Display(Name = "TITILE_TH")]
        [LocalizedStringLength(Constants.MaxLength.BdwTitleTH, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TitileTh { get; set; }
        [Display(Name = "NAME_TH")]
        [LocalizedStringLength(Constants.MaxLength.BdwNameTH, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string NameTh { get; set; }
        [Display(Name = "SURNAME_TH")]
        [LocalizedStringLength(Constants.MaxLength.BdwSurnameTH, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SurnameTh { get; set; }
        [Display(Name = "TITILE_EN")]
        [LocalizedStringLength(Constants.MaxLength.BdwTitleEN, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TitileEn { get; set; }
        [Display(Name = "NAME_EN")]
        [LocalizedStringLength(Constants.MaxLength.BdwNameEN, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string NameEn { get; set; }
        [Display(Name = "SURNAME_EN")]
        [LocalizedStringLength(Constants.MaxLength.BdwSurnameEN, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SurnameEn { get; set; }
        [Display(Name = "ACCOUNT_NO")]
        [LocalizedStringLength(Constants.MaxLength.BdwAccountNo, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AccountNo { get; set; }

        [Display(Name = "LOAN_MAIN")]
        [LocalizedStringLength(Constants.MaxLength.BdwLoanMain, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string LoanMain { get; set; }
        [Display(Name = "PRODUCT_GROUP")]
        [LocalizedStringLength(Constants.MaxLength.BdwProductGroup, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProductGroup { get; set; }
        [Display(Name = "PRODUCT")]
        [LocalizedStringLength(Constants.MaxLength.BdwProduct, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Product { get; set; }
        [Display(Name = "RELATIONSHIP")]
        [LocalizedStringLength(Constants.MaxLength.BdwRelationship, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Relationship { get; set; }
        [Display(Name = "PHONE")]
        [LocalizedStringLength(Constants.MaxLength.BdwPhone, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Phone { get; set; }
        [Display(Name = "ACCOUNT_STATUS")]
        [LocalizedStringLength(Constants.MaxLength.BdwAccountStatus, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AccountStatus { get; set; }
        [Display(Name = "CAMPAIGN")]
        [LocalizedStringLength(Constants.MaxLength.BdwCampaign, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Campaign { get; set; }
        [Display(Name = "CARD_TYPE_CODE")]
        [LocalizedStringLength(Constants.MaxLength.BdwCardTypeCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardTypeCode { get; set; }
        public string Error { get; set; }
        public string RelationName { get; set; }
        public int? ContactId { get; set; }
        public int? CustomerContactId { get; set; }
        public int? RelationId { get; set; }

    }
}
