using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisProductGroupEntity
    {
        [Display(Name = "PRODUCT_CODE")]
        [LocalizedStringLength(Constants.MaxLength.product_code, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProductCode { get; set; }

        [Display(Name = "PRODUCT_TYPE")]
        [LocalizedStringLength(Constants.MaxLength.product_type, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProductType { get; set; }

        [Display(Name = "PRODUCT_DESC")]
        [LocalizedStringLength(Constants.MaxLength.product_desc, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProductDesc { get; set; }

        [Display(Name = "SYSTEM")]
        [LocalizedStringLength(Constants.MaxLength.system, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SYSTEM { get; set; }

        [Display(Name = "PRODUCT_FLAG")]
        [LocalizedStringLength(Constants.MaxLength.product_flag, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProductFlag { get; set; }

        [Display(Name = "ENITY_CODE")]
        [LocalizedStringLength(Constants.MaxLength.enity_code, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string EntityCode { get; set; }

        [Display(Name = "SUBSCR_CODE")]
        [LocalizedStringLength(Constants.MaxLength.subscr_code, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubscrCode { get; set; }

        [Display(Name = "SUBSCR_DESC")]
        [LocalizedStringLength(Constants.MaxLength.subscr_desc, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubscrDesc { get; set; }        

    }
}
