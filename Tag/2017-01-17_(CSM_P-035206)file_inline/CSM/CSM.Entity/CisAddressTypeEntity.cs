using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisAddressTypeEntity
    {
        [Display(Name = "ADDRESS_TYPE_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.AddressTypeCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AddressTypeCode { get; set; }

        [Display(Name = "ADDRESS_TYPE_NAME")]
        [LocalizedStringLength(Constants.CisMaxLength.AddressTypeName, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AddressTypeDesc { get; set; }
        [Display(Name = "STATUS")]
        [LocalizedStringLength(Constants.CisMaxLength.Status, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Status { get; set; }
    }
}
