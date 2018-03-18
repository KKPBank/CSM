using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisIndividualEntity
    {
        [Display(Name = "KKCIS_ID")]
        [LocalizedStringLength(Constants.MaxLength.KKCisId, ErrorMessageResourceName = "ValErr_StringLength",
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
        public string CardtypeCode { get; set; }

        [Display(Name = "CUST_TYPE_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CusttypeCode { get; set; }

        [Display(Name = "CUST_TYPE_GROUP")]
        [LocalizedStringLength(2, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CusttypeGroup { get; set; }

        [Display(Name = "TITLE_ID")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TitleId { get; set; }

        [Display(Name = "TITLE_NAME_CUSTOM")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TitlenameCustom { get; set; }

        [Display(Name = "FIRST_NAME_TH")]
        [LocalizedStringLength(255, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FirstnameTh { get; set; }

        [Display(Name = "MID_NAME_TH")]
        [LocalizedStringLength(255, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string MidnameTh { get; set; }

        [Display(Name = "LAST_NAME_TH")]
        [LocalizedStringLength(255, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string LastnameTh { get; set; }

        [Display(Name = "FIRST_NAME_EN")]
        [LocalizedStringLength(255, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FirstnameEn { get; set; }

        [Display(Name = "MID_NAME_EN")]
        [LocalizedStringLength(255, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string MidnameEn { get; set; }

        [Display(Name = "LAST_NAME_EN")]
        [LocalizedStringLength(255, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string LastnameEn { get; set; }

        [Display(Name = "BIRTH_DATE")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BirthDate { get; set; }

        [Display(Name = "GENDER_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string GenderCode { get; set; }

        [Display(Name = "MARITAL_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string MaritalCode { get; set; }

        [Display(Name = "NATIONALITY1_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Nationality1Code { get; set; }

        [Display(Name = "NATIONALITY2_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Nationality2Code { get; set; }

        [Display(Name = "NATIONALITY3_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Nationality3Code { get; set; }

        [Display(Name = "RELIGION_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ReligionCode { get; set; }

        [Display(Name = "EDUCATION_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string EducationCode { get; set; }

        [Display(Name = "POSITION")]
        [LocalizedStringLength(255, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Position { get; set; }

        [Display(Name = "BUSINESS_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BusinessCode { get; set; }

        [Display(Name = "COMPANY_NAME")]
        [LocalizedStringLength(255, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CompanyName { get; set; }

        [Display(Name = "ISIC_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string IsicCode { get; set; }

        [Display(Name = "ANNUAL_INCOME")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AnnualIncome { get; set; }

        [Display(Name = "SOURCE_INCOME")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SourceIncome { get; set; }

        [Display(Name = "TOTAL_WEALTH_PERIOD")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TotalwealthPeriod { get; set; }

        [Display(Name = "FLG_MST_APP")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FlgmstApp { get; set; }

        [Display(Name = "CHANNEL_HOME")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ChannelHome { get; set; }

        [Display(Name = "FIRST_BRANCH")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FirstBranch { get; set; }

        [Display(Name = "SHARE_INFO_FLAG")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ShareinfoFlag { get; set; }

        [Display(Name = "PLACE_CUST_UPDATED")]
        [LocalizedStringLength(20, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PlacecustUpdated { get; set; }

        [Display(Name = "DATE_CUST_UPDATED")]
        [LocalizedStringLength(20, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DatecustUpdated { get; set; }

        [Display(Name = "ANNUAL_INCOME_PERIOD")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AnnualincomePeriod { get; set; }

        [Display(Name = "MARKETING_ID")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string MarketingId { get; set; }

        [Display(Name = "NUMBER_OF_EMPLOYEE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string NumberofEmployee { get; set; }

        [Display(Name = "FIXED_ASSET")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FixedAsset { get; set; }

        [Display(Name = "FIXED_ASSET_EXCLUDE_LAND")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FixedassetExcludeland { get; set; }

        [Display(Name = "OCCUPATION_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string OccupationCode { get; set; }

        [Display(Name = "OCCUPATION_SUBTYPE_CODE")]
        [LocalizedStringLength(10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string OccupationsubtypeCode { get; set; }

        [Display(Name = "COUNTRY_INCOME")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CountryIncome { get; set; }

        [Display(Name = "TOTAL_WEALTH")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TotalwealTh { get; set; }

        [Display(Name = "SOURCE_INCOME_REM")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SourceIncomerem { get; set; }

        [Display(Name = "CREATED_DATE")]
        [LocalizedStringLength(20, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedDate { get; set; }

        [Display(Name = "CREATED_BY")]
        [LocalizedStringLength(100, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedBy { get; set; }

        [Display(Name = "UPDATE_DATE")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdateDate { get; set; }

        [Display(Name = "UPDATED_BY")]
        [LocalizedStringLength(100, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedBy { get; set; }
        public string Error { get; set; }
        
    }
}
