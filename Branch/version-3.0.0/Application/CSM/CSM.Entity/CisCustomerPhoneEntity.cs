using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisCustomerPhoneEntity
    {
        [Display(Name = "KKCIS_ID")]
        [LocalizedStringLength(Constants.CisMaxLength.KKCisId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
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

        [Display(Name = "CUST_TYPE_GROUP")]
        [LocalizedStringLength(Constants.CisMaxLength.CustTypeGroup, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CustTypeGroup { get; set; }

        [Display(Name = "PHONE_TYPE_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.PhoneTypeCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneTypeCode { get; set; }

        [Display(Name = "PHONE_NUM")]
        [LocalizedStringLength(Constants.CisMaxLength.PhoneNum, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneNum { get; set; }

        [Display(Name = "PHONE_EXT")]
        [LocalizedStringLength(Constants.CisMaxLength.PhoneExt, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneExt { get; set; }

        [Display(Name = "CREATE_DATE")]
        [LocalizedStringLength(Constants.CisMaxLength.CreatedDate, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreateDate { get; set; }

        [Display(Name = "CREATE_BY")]
        [LocalizedStringLength(Constants.CisMaxLength.CreatedBy, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreateBy { get; set; }

        [Display(Name = "UPDATE_DATE")]
        [LocalizedStringLength(Constants.CisMaxLength.UpdatedDate, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdateDate { get; set; }

        [Display(Name = "UPDATE_BY")]
        [LocalizedStringLength(Constants.CisMaxLength.UpdatedBy, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdateBy { get; set; }

        public string Error { get; set; }
    }
}
