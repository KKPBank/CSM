using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisPhoneTypeEntity
    {
        [Display(Name = "PHONE_TYPE_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.PhoneTypeCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneTypecode { get; set; }

        [Display(Name = "PHONE_TYPE_DESC")]
        [LocalizedStringLength(Constants.CisMaxLength.PhoneTypeDesc, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneTypeDesc { get; set; }
        [Display(Name = "STATUS")]
        [LocalizedStringLength(Constants.CisMaxLength.Status, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Status { get; set; }
    }
}
