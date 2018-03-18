using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisEmailTypeEntity
    {
        [Display(Name = "EMAIL_TYPE_CODE")]
        [LocalizedStringLength(Constants.MaxLength.email_type_code, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string MailTypecode { get; set; }

        [Display(Name = "EMAIL_TYPE_DESC")]
        [LocalizedStringLength(Constants.MaxLength.email_type_desc, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string MailTypeDesc { get; set; }
    }
}
