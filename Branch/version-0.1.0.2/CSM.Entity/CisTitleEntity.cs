using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisTitleEntity
    {
        [Display(Name = "TITLE_ID")]
        [LocalizedStringLength(Constants.MaxLength.TitleId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TitleID { get; set; }

        [Display(Name = "TITLE_NAME_TH")]
        [LocalizedStringLength(Constants.MaxLength.TitleNameTH, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TitleNameTH { get; set; }

        [Display(Name = "TITLE_NAME_EN")]
        [LocalizedStringLength(Constants.MaxLength.TitleNameEN, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TitleNameEN { get; set; }

        [Display(Name = "TITLE_TYPE_GROUP")]
        [LocalizedStringLength(Constants.MaxLength.TitleTypeGroup, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TitleTypeGroup { get; set; }

        [Display(Name = "GENDER_CODE")]
        [LocalizedStringLength(Constants.MaxLength.GenderCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string GenderCode { get; set; }
    }
}
