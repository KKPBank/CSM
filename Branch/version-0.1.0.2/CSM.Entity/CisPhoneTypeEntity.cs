using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisPhoneTypeEntity
    {
        [Display(Name = "PHONE_TYPE_CODE")]
        [LocalizedStringLength(Constants.MaxLength.phone_type_code, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneTypecode { get; set; }

        [Display(Name = "PHONE_TYPE_DESC")]
        [LocalizedStringLength(Constants.MaxLength.phone_type_desc, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneTypeDesc { get; set; }
    }
}
