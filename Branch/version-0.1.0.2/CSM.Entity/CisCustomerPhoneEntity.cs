using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisCustomerPhoneEntity
    {
        [Display(Name = "KKCIS_ID")]
        [LocalizedStringLength(Constants.MaxLength.Cus_KKCisId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string KKCisId { get; set; }

        [Display(Name = "CUST_ID")]
        [LocalizedStringLength(Constants.MaxLength.Cus_CusId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CustId { get; set; }

        [Display(Name = "CARD_ID")]
        [LocalizedStringLength(Constants.MaxLength.Cus_CardId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardId { get; set; }

        [Display(Name = "CARD_TYPE_CODE")]
        [LocalizedStringLength(Constants.MaxLength.Cus_CardtypeCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardTypeCode { get; set; }

        [Display(Name = "CUST_TYPE_GROUP")]
        [LocalizedStringLength(Constants.MaxLength.Cus_CusttypeGroup, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CustTypeGroup { get; set; }

        [Display(Name = "PHONE_TYPE_CODE")]
        [LocalizedStringLength(Constants.MaxLength.Cus_PhonetypeCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneTypeCode { get; set; }

        [Display(Name = "PHONE_NUM")]
        [LocalizedStringLength(Constants.MaxLength.Cus_PhoneNum, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneNum { get; set; }

        [Display(Name = "PHONE_EXT")]
        [LocalizedStringLength(Constants.MaxLength.Cus_PhoneExt, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneExt { get; set; }

        [Display(Name = "CREATE_DATE")]
        [LocalizedStringLength(Constants.MaxLength.Cus_CreateDate, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreateDate { get; set; }

        [Display(Name = "CREATE_BY")]
        [LocalizedStringLength(Constants.MaxLength.Cus_CreateBy, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreateBy { get; set; }

        [Display(Name = "UPDATE_DATE")]
        [LocalizedStringLength(Constants.MaxLength.Cus_UpdateDate, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdateDate { get; set; }

        [Display(Name = "UPDATE_BY")]
        [LocalizedStringLength(Constants.MaxLength.Cus_UpdateBy, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdateBy { get; set; }
    }
}
