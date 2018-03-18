using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisCountryEntity
    {
        [Display(Name = "COUNTRY_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.CountryCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CountryCode { get; set; }

        [Display(Name = "COUNTRY_NAME_TH")]
        [LocalizedStringLength(Constants.CisMaxLength.CountryNameTH, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CountryNameTH { get; set; }

        [Display(Name = "COUNTRY_NAME_EN")]
        [LocalizedStringLength(Constants.CisMaxLength.CountryNameEN, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CountryNameEN { get; set; }
        [Display(Name = "STATUS")]
        [LocalizedStringLength(Constants.CisMaxLength.Status, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Status { get; set; }
    }
}
