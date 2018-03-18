using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisSubDistrictEntity
    {
        [Display(Name = "DISTRICT_CODE")]
        [LocalizedStringLength(Constants.MaxLength.district_code, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DistrictCode { get; set; }

        [Display(Name = "SUBDISTRICT_CODE")]
        [LocalizedStringLength(Constants.MaxLength.subdistrict_code, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubDistrictCode { get; set; }

        [Display(Name = "SUBDISTRICT_NAME_TH")]
        [LocalizedStringLength(Constants.MaxLength.subdistrict_name_th, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubDistrictNameTH { get; set; }

        [Display(Name = "SUBDISTRICT_NAME_EN")]
        [LocalizedStringLength(Constants.MaxLength.subdistrict_name_en, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubDistrictNameEN { get; set; }

        [Display(Name = "POSTCODE")]
        [LocalizedStringLength(Constants.MaxLength.postcode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PostCode { get; set; }
    }
}
