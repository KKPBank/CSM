using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisCustomerEmailEntity
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

        [Display(Name = "MAIL_TYPE_CODE")]
        [LocalizedStringLength(Constants.MaxLength.Cus_MailtypeGroup, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string MailTypeCode { get; set; }

        [Display(Name = "MAILACCOUNT")]
        [LocalizedStringLength(Constants.MaxLength.Cus_MailAccount, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string MailAccount { get; set; }

        [Display(Name = "CREATED_DATE")]
        [LocalizedStringLength(Constants.MaxLength.Cus_CreateDate, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedDate { get; set; }

        [Display(Name = "CREATED_BY")]
        [LocalizedStringLength(Constants.MaxLength.Cus_CreateBy, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedBy { get; set; }

        [Display(Name = "UPDATED_DATE")]
        [LocalizedStringLength(Constants.MaxLength.Cus_UpdateDate, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedDate { get; set; }

        [Display(Name = "UPDATED_BY")]
        [LocalizedStringLength(Constants.MaxLength.Cus_UpdateBy, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedBy { get; set; }
    }
}
