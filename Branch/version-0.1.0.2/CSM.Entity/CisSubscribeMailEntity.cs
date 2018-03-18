using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisSubscribeMailEntity
    {
        [Display(Name = "KKCIS_ID")]
        [LocalizedStringLength(Constants.MaxLength.KKCisId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string KKCisId { get; set; }

        [Display(Name = "CUST_ID")]
        [LocalizedStringLength(Constants.MaxLength.CustId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CustId { get; set; }

        [Display(Name = "CARD_ID")]
        [LocalizedStringLength(Constants.MaxLength.Card_Id, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardId { get; set; }

        [Display(Name = "CARD_TYPE_CODE")]
        [LocalizedStringLength(Constants.MaxLength.CardtypeCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardTypeCode { get; set; }

        [Display(Name = "PROD_GROUP")]
        [LocalizedStringLength(Constants.MaxLength.Prod_Group, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProdGroup { get; set; }

        [Display(Name = "PROD_TYPE")]
        [LocalizedStringLength(Constants.MaxLength.Prod_Type, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProdType { get; set; }

        [Display(Name = "SUBSCR_CODE")]
        [LocalizedStringLength(Constants.MaxLength.SubscrCode, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubscrCode { get; set; }

        [Display(Name = "EMAIL_TYPE_CODE")]
        [LocalizedStringLength(Constants.MaxLength.MailTypeCode, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string MailTypeCode { get; set; }

        [Display(Name = "MAILACCOUNT")]
        [LocalizedStringLength(Constants.MaxLength.MailAccount, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string MailAccount { get; set; }

        [Display(Name = "CREATE_DATE")]
        [LocalizedStringLength(Constants.MaxLength.CreatedDate, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedDate { get; set; }

        [Display(Name = "CREATE_BY")]
        [LocalizedStringLength(Constants.MaxLength.CreatedBy, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedBy { get; set; }

        [Display(Name = "UPDATE_DATE")]
        [LocalizedStringLength(Constants.MaxLength.UpdatedDate, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedDate { get; set; }

        [Display(Name = "UPDATE_BY")]
        [LocalizedStringLength(Constants.MaxLength.UpdatedBy, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedBy { get; set; }
    }
}
