using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisSubDistrictEntity
    {
        [Display(Name = "DISTRICT_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.DistrictCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DistrictCode { get; set; }

        [Display(Name = "SUBDISTRICT_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.SubdistrictCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubDistrictCode { get; set; }

        [Display(Name = "SUBDISTRICT_NAME_TH")]
        [LocalizedStringLength(Constants.CisMaxLength.SubdistrictNameTH, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubDistrictNameTH { get; set; }

        [Display(Name = "SUBDISTRICT_NAME_EN")]
        [LocalizedStringLength(Constants.CisMaxLength.SubdistrictNameEN, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubDistrictNameEN { get; set; }

        [Display(Name = "POSTCODE")]
        [LocalizedStringLength(Constants.CisMaxLength.PostCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PostCode { get; set; }
        [Display(Name = "STATUS")]
        [LocalizedStringLength(Constants.CisMaxLength.Status, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Status { get; set; }
    }
}
