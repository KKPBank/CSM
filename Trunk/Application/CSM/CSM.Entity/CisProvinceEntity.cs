using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisProvinceEntity
    {
        [Display(Name = "PROVINCE_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.ProvinceCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProvinceCode { get; set; }

        [Display(Name = "PROVINCE_NAME_TH")]
        [LocalizedStringLength(Constants.CisMaxLength.ProvinceNameTH, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProvinceNameTH { get; set; }

        [Display(Name = "PROVINCE_NAME_EN")]
        [LocalizedStringLength(Constants.CisMaxLength.ProvinceNameEN, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProvinceNameEN { get; set; }
        [Display(Name = "STATUS")]
        [LocalizedStringLength(Constants.CisMaxLength.Status, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Status { get; set; }
    }
}
