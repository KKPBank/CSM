using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisEmailTypeEntity
    {
        [Display(Name = "EMAIL_TYPE_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.EmailTypeCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string MailTypecode { get; set; }

        [Display(Name = "EMAIL_TYPE_DESC")]
        [LocalizedStringLength(Constants.CisMaxLength.EmailTypeDesc, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string MailTypeDesc { get; set; }
        [Display(Name = "STATUS")]
        [LocalizedStringLength(Constants.CisMaxLength.Status, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Status { get; set; }
    }
}
