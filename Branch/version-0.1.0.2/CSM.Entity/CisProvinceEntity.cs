using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisProvinceEntity
    {
        [Display(Name = "PROVINCE_CODE")]
        [LocalizedStringLength(Constants.MaxLength.province_code, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProvinceCode { get; set; }

        [Display(Name = "PROVINCE_NAME_TH")]
        [LocalizedStringLength(Constants.MaxLength.province_name_th, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProvinceNameTH { get; set; }

        [Display(Name = "PROVINCE_NAME_EN")]
        [LocalizedStringLength(Constants.MaxLength.province_name_en, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProvinceNameEN { get; set; }
    }
}
