using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisTitleEntity
    {
        [Display(Name = "TITLE_ID")]
        [LocalizedStringLength(Constants.CisMaxLength.TitleId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TitleID { get; set; }

        [Display(Name = "TITLE_NAME_TH")]
        [LocalizedStringLength(Constants.CisMaxLength.TitleNameTH, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TitleNameTH { get; set; }

        [Display(Name = "TITLE_NAME_EN")]
        [LocalizedStringLength(Constants.CisMaxLength.TitleNameEN, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TitleNameEN { get; set; }

        [Display(Name = "TITLE_TYPE_GROUP")]
        [LocalizedStringLength(Constants.CisMaxLength.TitleTypeGroup, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TitleTypeGroup { get; set; }

        [Display(Name = "GENDER_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.GenderCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string GenderCode { get; set; }
        [Display(Name = "STATUS")]
        [LocalizedStringLength(Constants.CisMaxLength.Status, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Status { get; set; }
    }
}
