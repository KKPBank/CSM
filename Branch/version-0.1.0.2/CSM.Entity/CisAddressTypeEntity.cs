using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisAddressTypeEntity
    {
        [Display(Name = "ADDRESS_TYPE_CODE")]
        [LocalizedStringLength(Constants.MaxLength.AddresstypeCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AddressTypeCode { get; set; }

        [Display(Name = "ADDRESS_TYPE_NAME")]
        [LocalizedStringLength(Constants.MaxLength.AddresstypeName, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AddressTypeDesc { get; set; }
    }
}
