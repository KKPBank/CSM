using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisDistrictEntity
    {
        [Display(Name = "PROVINCE_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.ProvinceCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProvinceCode { get; set; }

        [Display(Name = "DISTRIC_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.DistrictCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DistrictCode { get; set; }

        [Display(Name = "DISTRICT_NAME_TH")]
        [LocalizedStringLength(Constants.CisMaxLength.DistrictNameTH, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DistrictNameTH { get; set; }

        [Display(Name = "DISTRICT_NAME_EN")]
        [LocalizedStringLength(Constants.CisMaxLength.DistrictNameEN, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DistrictNameEN { get; set; }
        [Display(Name = "STATUS")]
        [LocalizedStringLength(Constants.CisMaxLength.Status, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Status { get; set; }
    }
}
