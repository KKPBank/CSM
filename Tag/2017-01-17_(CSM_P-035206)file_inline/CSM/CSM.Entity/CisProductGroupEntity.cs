using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisProductGroupEntity
    {
        [Display(Name = "PRODUCT_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.ProductCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProductCode { get; set; }

        [Display(Name = "PRODUCT_TYPE")]
        [LocalizedStringLength(Constants.CisMaxLength.ProductType, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProductType { get; set; }

        [Display(Name = "PRODUCT_DESC")]
        [LocalizedStringLength(Constants.CisMaxLength.ProductDesc, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProductDesc { get; set; }

        [Display(Name = "SYSTEM")]
        [LocalizedStringLength(Constants.CisMaxLength.System, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SYSTEM { get; set; }

        [Display(Name = "PRODUCT_FLAG")]
        [LocalizedStringLength(Constants.CisMaxLength.ProductFlag, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProductFlag { get; set; }

        [Display(Name = "ENITY_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.EntityCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string EntityCode { get; set; }

        [Display(Name = "SUBSCR_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.SubscrCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubscrCode { get; set; }

        [Display(Name = "SUBSCR_DESC")]
        [LocalizedStringLength(Constants.CisMaxLength.SubscrDesc, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubscrDesc { get; set; }
        [Display(Name = "STATUS")]
        [LocalizedStringLength(Constants.CisMaxLength.Status, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Status { get; set; }

    }
}
