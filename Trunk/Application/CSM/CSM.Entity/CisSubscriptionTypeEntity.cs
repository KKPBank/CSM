using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    public class CisSubscriptionTypeEntity
    {
        public int? SubscriptionTypeId { get; set; }

        [Display(Name = "CUST_TYPE_GROUP")]
        [LocalizedStringLength(Constants.CisMaxLength.CustTypeGroup, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))] 
        public string CustTypeGroup { get; set; }

        [Display(Name = "CUST_TYPE_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.CustTypeCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))] 
        public string CustTypeCode { get; set; }

        [Display(Name = "CUST_TYPE_TH")]
        [LocalizedStringLength(Constants.CisMaxLength.CustTypeTH, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))] 
        public string CustTypeTh { get; set; }

        [Display(Name = "CUST_TYPE_EN")]
        [LocalizedStringLength(Constants.CisMaxLength.CustTypeEN, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))] 
        public string CustTypeEn { get; set; }

        [Display(Name = "CARD_TYPE_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.CardTypeCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))] 
        public string CardTypeCode { get; set; }

        [Display(Name = "CARD_TYPE_EN")]
        [LocalizedStringLength(Constants.CisMaxLength.CardTypeEN, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))] 
        public string CardTypeEn { get; set; }

        [Display(Name = "CARD_TYPE_TH")]
        [LocalizedStringLength(Constants.CisMaxLength.CardTypeTH, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))] 
        public string CardTypeTh { get; set; }

        [Display(Name = "STATUS")]
        [LocalizedStringLength(Constants.CisMaxLength.Status, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Status { get; set; }

        public string Error { get; set; }
        public int? SubscriptTypeId { get; set; }
    }
}
