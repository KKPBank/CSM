using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisCorporateEntity
    {
        [Display(Name = "KKCIS_ID")]
        [LocalizedStringLength(Constants.CisMaxLength.KKCisId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        //[Required(ErrorMessage = "KK Cis Id is not null")]
        public string KKCisId { get; set; }

        [Display(Name = "CUST_ID")]
        [LocalizedStringLength(Constants.CisMaxLength.CustId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CustId { get; set; }

        [Display(Name = "CARD_ID")]
        [LocalizedStringLength(Constants.CisMaxLength.CardId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardId { get; set; }

        [Display(Name = "CARD_TYPE_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.CardTypeCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardTypeCode { get; set; }

        [Display(Name = "CUST_TYPE_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.CustTypeCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CustTypeCode { get; set; }

        [Display(Name = "CUST_TYPE_GROUP")]
        [LocalizedStringLength(Constants.CisMaxLength.CustTypeGroup, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CustTypeGroup { get; set; }

        [Display(Name = "TITLE_ID")]
        [LocalizedStringLength(Constants.CisMaxLength.TitleId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TitleId { get; set; }

        [Display(Name = "NAME_TH")]
        [LocalizedStringLength(Constants.CisMaxLength.NameTH, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string NameTh { get; set; }

        [Display(Name = "NAME_EN")]
        [LocalizedStringLength(Constants.CisMaxLength.NameEN, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string NameEn { get; set; }

        [Display(Name = "ISIC_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.IsicCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string IsicCode { get; set; }

        [Display(Name = "TAX_ID")]
        [LocalizedStringLength(Constants.CisMaxLength.TaxId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TaxId { get; set; }

        [Display(Name = "HOST_BUSINESS_COUNTRY_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.HostBusinessCountryCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string HostBusinessCountryCode { get; set; }

        [Display(Name = "VALUE_PER_SHARE")]
        [LocalizedStringLength(Constants.CisMaxLength.ValuePerShare, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ValuePerShare { get; set; }

        [Display(Name = "AUTHORIZED_SHARE_CAPITAL")]
        [LocalizedStringLength(Constants.CisMaxLength.AuthorizedShareCapital, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AuthorizedShareCapital { get; set; }

        [Display(Name = "REGISTER_DATE")]
        [LocalizedStringLength(Constants.CisMaxLength.RegisterDate, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string RegisterDate { get; set; }

        [Display(Name = "BUSINESS_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.BusinessCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BusinessCode { get; set; }

        [Display(Name = "FIXED_ASSET")]
        [LocalizedStringLength(Constants.CisMaxLength.FixedAsset, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FixedAsset { get; set; }

        [Display(Name = "FIXED_ASSET_EXCLUDE_LAND")]
        [LocalizedStringLength(Constants.CisMaxLength.FixedAssetExcludeLand, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FixedAssetexcludeLand { get; set; }

        [Display(Name = "NUMBER_OF_EMPLOYEE")]
        [LocalizedStringLength(Constants.CisMaxLength.NumberOfEmployee, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string NumberOfEmployee { get; set; }

        [Display(Name = "SHARE_INFO_FLAG")]
        [LocalizedStringLength(Constants.CisMaxLength.ShareInfoFlag, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ShareInfoFlag { get; set; }

        [Display(Name = "FLG_MST_APP")]
        [LocalizedStringLength(Constants.CisMaxLength.FlgMstApp, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FlgmstApp { get; set; }

        [Display(Name = "FIRST_BRANCH")]
        [LocalizedStringLength(Constants.CisMaxLength.FirstBranch, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FirstBranch { get; set; }

        [Display(Name = "PLACE_CUST_UPDATED")]
        [LocalizedStringLength(Constants.CisMaxLength.PlaceCustUpdated, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PlaceCustUpdated { get; set; }

        [Display(Name = "DATE_CUST_UPDATED")]
        [LocalizedStringLength(Constants.CisMaxLength.DateCustUpdated, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DateCustUpdated { get; set; }

        [Display(Name = "ID_COUNTRY_ISSUE")]
        [LocalizedStringLength(Constants.CisMaxLength.IdCountryIssue, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string IdCountryIssue { get; set; }

        [Display(Name = "BUSINESS_CAT_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.BusinessCatCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BusinessCatCode { get; set; }

        [Display(Name = "MARKETING_ID")]
        [LocalizedStringLength(Constants.CisMaxLength.MarketingId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string MarketingId { get; set; }

        [Display(Name = "STOCK")]
        [LocalizedStringLength(Constants.CisMaxLength.Stock, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Stock { get; set; }

        [Display(Name = "CREATED_DATE")]
        [LocalizedStringLength(Constants.CisMaxLength.CreatedDate, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedDate { get; set; }

        [Display(Name = "CREATED_BY")]
        [LocalizedStringLength(Constants.CisMaxLength.CreatedBy, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedBy { get; set; }

        [Display(Name = "UPDATED_DATE")]
        [LocalizedStringLength(Constants.CisMaxLength.UpdatedDate, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedDate { get; set; }

        [Display(Name = "UPDATED_DATE")]
        [LocalizedStringLength(Constants.CisMaxLength.UpdatedBy, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedBy { get; set; }
        public string Error { get; set; }
    }
}
