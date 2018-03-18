using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisDistrictEntity
    {
        [Display(Name = "PROVINCE_CODE")]
        [LocalizedStringLength(Constants.MaxLength.province_code, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProvinceCode { get; set; }

        [Display(Name = "DISTRIC_CODE")]
        [LocalizedStringLength(Constants.MaxLength.distric_code, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DistrictCode { get; set; }

        [Display(Name = "DISTRICT_NAME_TH")]
        [LocalizedStringLength(Constants.MaxLength.district_name_th, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DistrictNameTH { get; set; }

        [Display(Name = "DISTRICT_NAME_EN")]
        [LocalizedStringLength(Constants.MaxLength.district_name_en, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DistrictNameEN { get; set; }
    }
}
