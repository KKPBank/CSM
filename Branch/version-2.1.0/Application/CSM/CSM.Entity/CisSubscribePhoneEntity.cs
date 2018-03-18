using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisSubscribePhoneEntity
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

        [Display(Name = "PROD_GROUP")]
        [LocalizedStringLength(Constants.CisMaxLength.ProdGroup, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProdGroup { get; set; }

        [Display(Name = "PROD_TYPE")]
        [LocalizedStringLength(Constants.CisMaxLength.ProdType, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProdType { get; set; }

        [Display(Name = "SUBSCR_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.SubscrCode, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubscrCode { get; set; }

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
        public string CreatedDate { get; set; }

        [Display(Name = "CREATE_BY")]
        [LocalizedStringLength(Constants.CisMaxLength.CreatedBy, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedBy { get; set; }

        [Display(Name = "UPDATE_DATE")]
        [LocalizedStringLength(Constants.CisMaxLength.UpdatedDate, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedDate { get; set; }

        [Display(Name = "UPDATE_BY")]
        [LocalizedStringLength(Constants.CisMaxLength.UpdatedBy, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedBy { get; set; }

        [Display(Name = "SYSCUSTSUBSCR_ID")]
        [LocalizedStringLength(Constants.CisMaxLength.SysCustSubscrId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SysCustSubscrId { get; set; }

        public string Error { get; set; }
    }
}
