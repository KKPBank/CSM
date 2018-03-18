using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class SaleZoneEntity
    { 
        [Display(Name = "AMPHUR")]
        [LocalizedStringLength(Constants.MaxLength.Amphur, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string District { get; set; }

        [Display(Name = "PROVINCE")]
        [LocalizedStringLength(Constants.MaxLength.Province, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Province { get; set; }

        [Display(Name = "SALE_NAME")]
        [LocalizedStringLength(Constants.MaxLength.SaleName, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SaleName { get; set; }

        [Display(Name = "PHONE_NO")]
        [LocalizedStringLength(Constants.MaxLength.PhoneNo, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneNo { get; set; }

        [Display(Name = "EMPLOYEE_ID")]
        [LocalizedStringLength(Constants.MaxLength.EmployeeId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string EmployeeNo { get; set; }

        [Display(Name = "MOBILE_NO")]
        [LocalizedStringLength(Constants.MaxLength.PhoneNo, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string MobileNo { get; set; }

        [Display(Name = "EMAIL")]
        [LocalizedStringLength(Constants.MaxLength.Email, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", "ValErr_InvalidEmail")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "No matching employee")]       
        public int? EmployeeId { get; set; }
        public string Error { get; set; }
    }
}
