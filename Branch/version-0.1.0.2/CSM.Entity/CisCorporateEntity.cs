using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisCorporateEntity
    {
        [Display(Name = "KKCIS_ID")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        //[Required(ErrorMessage = "KK Cis Id is not null")]
        public string KKCisId { get; set; }

        [Display(Name = "CUST_ID")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CustId { get; set; }

        [Display(Name = "CARD_ID")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardId { get; set; }

        [Display(Name = "CARD_TYPE_CODE")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardTypeCode { get; set; }

        [Display(Name = "CUST_TYPE_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CustTypeCode { get; set; }

        [Display(Name = "CUST_TYPE_GROUP")]
        [LocalizedStringLength(2, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CustTypeGroup { get; set; }

        [Display(Name = "TITLE_ID")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TitleId { get; set; }

        [Display(Name = "NAME_TH")]
        [LocalizedStringLength(255, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string NameTh { get; set; }

        [Display(Name = "NAME_EN")]
        [LocalizedStringLength(255, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string NameEn { get; set; }

        [Display(Name = "ISIC_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string IsicCode { get; set; }

        [Display(Name = "TAX_ID")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TaxId { get; set; }

        [Display(Name = "HOST_BUSINESS_COUNTRY_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string HostBusinessCountryCode { get; set; }

        [Display(Name = "VALUE_PER_SHARE")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ValuePerShare { get; set; }

        [Display(Name = "AUTHORIZED_SHARE_CAPITAL")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AuthorizedShareCapital { get; set; }

        [Display(Name = "REGISTER_DATE")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string RegisterDate { get; set; }

        [Display(Name = "BUSINESS_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BusinessCode { get; set; }

        [Display(Name = "FIXED_ASSET")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FixedAsset { get; set; }

        [Display(Name = "FIXED_ASSET_EXCLUDE_LAND")]
        [LocalizedStringLength(Constants.MaxLength.TaxId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FixedAssetexcludeLand { get; set; }

        [Display(Name = "NUMBER_OF_EMPLOYEE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string NumberOfEmployee { get; set; }

        [Display(Name = "SHARE_INFO_FLAG")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ShareInfoFlag { get; set; }

        [Display(Name = "FLG_MST_APP")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FlgmstApp { get; set; }

        [Display(Name = "FIRST_BRANCH")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FirstBranch { get; set; }

        [Display(Name = "PLACE_CUST_UPDATED")]
        [LocalizedStringLength(20, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PlaceCustUpdated { get; set; }

        [Display(Name = "DATE_CUST_UPDATED")]
        [LocalizedStringLength(20, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DateCustUpdated { get; set; }

        [Display(Name = "ID_COUNTRY_ISSUE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string IdCountryIssue { get; set; }

        [Display(Name = "BUSINESS_CAT_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BusinessCatCode { get; set; }

        [Display(Name = "MARKETING_ID")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string MarketingId { get; set; }

        [Display(Name = "STOCK")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Stock { get; set; }

        [Display(Name = "CREATED_DATE")]
        [LocalizedStringLength(20, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedDate { get; set; }

        [Display(Name = "CREATED_BY")]
        [LocalizedStringLength(100, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedBy { get; set; }

        [Display(Name = "UPDATED_DATE")]
        [LocalizedStringLength(20, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedDate { get; set; }

        [Display(Name = "UPDATED_DATE")]
        [LocalizedStringLength(100, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedBy { get; set; }
        public string Error { get; set; }
        
    }
}
