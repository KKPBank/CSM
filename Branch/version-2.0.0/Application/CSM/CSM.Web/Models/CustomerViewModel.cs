using System;
using System.Collections.Generic;
using CSM.Entity;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Web.Models
{
    [Serializable]
    public class CustomerViewModel
    {
        public SelectList CustomerTypeList { get; set; }
        public SelectList StatusList { get; set; }
        public CustomerSearchFilter SearchFilter { get; set; }
        public IEnumerable<CustomerEntity> CustomerList { get; set; }
        public CustomerInfoViewModel CustomerInfo { get; set; }
        public NoteSearchFilter NoteSearchFilter { get; set; }
        public IEnumerable<NoteEntity> NoteList { get; set; }
        public string IsSubmit { get; set; } // 1 = true, 0 = false
        public string IsConfirm { get; set; } // 1 = true, 0 = false
        public SelectList SubscriptTypeList { get; set; }
        public SelectList TitleThaiList { get; set; }
        public SelectList TitleEnglishList { get; set; }
        public SelectList PhoneTypeList { get; set; }
        public int? CustomerId { get; set; }
        public string SubscriptType { get; set; }
        public string TitleThai { get; set; }

        [Display(Name = "Lbl_FirstNameThai", ResourceType = typeof(CSM.Common.Resources.Resource))]
        //[LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        [LocalizedStringLengthAttribute(Constants.MaxLength.FirstName, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex(@"([\-ก-๙0-9()., ]+)", "ValErr_NoSpecialCharacterThai")]
        public string FirstNameThai { get; set; }

        [Display(Name = "Lbl_LastNameThai", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedStringLengthAttribute(Constants.MaxLength.LastName, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex(@"([\-ก-๙0-9()., ]+)", "ValErr_NoSpecialCharacterThai")]
        public string LastNameThai { get; set; }

        public string TitleEnglish { get; set; }

        [Display(Name = "Lbl_FirstNameEnglish", ResourceType = typeof(CSM.Common.Resources.Resource))]        
        [LocalizedStringLengthAttribute(Constants.MaxLength.FirstName, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex(@"([\-a-zA-Z0-9()., ]+)", "ValErr_NoSpecialCharacterEnglish")]
        public string FirstNameEnglish { get; set; }

        [Display(Name = "Lbl_LastNameEnglish", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedStringLengthAttribute(Constants.MaxLength.LastName, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex(@"([\-a-zA-Z0-9()., ]+)", "ValErr_NoSpecialCharacterEnglish")]
        public string LastNameEnglish { get; set; }

        [Display(Name = "Lbl_CardNo", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedStringLengthAttribute(Constants.MaxLength.CardNo, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardNo { get; set; }

        public string BirthDate { get; set; }
        public DateTime? BirthDateValue { get { return BirthDate.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }

        [Display(Name = "Lbl_Email", ResourceType = typeof(CSM.Common.Resources.Resource))]
        //[DataType(DataType.EmailAddress)]
        //[LocalizedRegex("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", "ValErr_InvalidEmail")]
        [LocalizedStringLengthAttribute(Constants.MaxLength.Email, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Email { get; set; }

        [Display(Name = "Lb_PhoneNo_1", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public string PhoneType1 { get; set; }
        public string PhoneType2 { get; set; }
        public string PhoneType3 { get; set; }

        [Display(Name = "Lb_PhoneNo_1", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("([0-9#]+)", "ValErr_NumericAndExtOnly")]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        [LocalizedStringLength(Constants.MaxLength.PhoneNo, ErrorMessageResourceName = "ValErr_StringLength",
           ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneNo1 { get; set; }

        [Display(Name = "Lb_PhoneNo_2", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("([0-9#]+)", "ValErr_NumericAndExtOnly")]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        [LocalizedStringLength(Constants.MaxLength.PhoneNo, ErrorMessageResourceName = "ValErr_StringLength",
           ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneNo2 { get; set; }

        [Display(Name = "Lb_PhoneNo_3", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("([0-9#]+)", "ValErr_NumericAndExtOnly")]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        [LocalizedStringLength(Constants.MaxLength.PhoneNo, ErrorMessageResourceName = "ValErr_StringLength",
           ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneNo3 { get; set; }

        [Display(Name = "Lbl_Fax", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]
        [LocalizedStringLength(Constants.MaxLength.PhoneNo, ErrorMessageResourceName = "ValErr_StringLength",
           ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Fax { get; set; }

        public string IsSelected { get; set; }
        public bool NotValidatePhone1 { get; set; }
    }

    public class LookupCustomerViewModel
    {
        public CustomerSearchFilter CustomerSearchFilter { get; set; }
        public IEnumerable<ServiceRequestCustomerSearchResult> CustomerTableList { get; set; }
    }
    public class CriteriaCustomerPhoneDuplicateList
    {
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string PhoneNo3 { get; set; }
        public string FirstNameThai { get; set; }
    }
    public class LookupCustomerDuplicateViewModel
    {
        //public CustomerSearchFilter CustomerSearchFilter { get; set; } = new CustomerSearchFilter();

        private CustomerSearchFilter _CustomerSearchFilter;
        public LookupCustomerDuplicateViewModel()
        {
            _CustomerSearchFilter = new CustomerSearchFilter();
        }
        public CustomerSearchFilter CustomerSearchFilter
        {
            get { return _CustomerSearchFilter; }
            set { _CustomerSearchFilter = value; }
        }
        public IEnumerable<ServiceRequestCustomerSearchResult> CustomerTableList { get; set; }
    }
    public class LookupContractViewModel
    {
        public ContractSearchFilter ContactSearchFilter { get; set; }
        public IEnumerable<ServiceRequestAccountSearchResult> ContractTableList { get; set; }
    }

    public class LookupCustomerContactViewModel
    {
        public CustomerContactSearchFilter CustomerContactSearchFilter { get; set; }
        public IEnumerable<ServiceRequestContactSearchResult> ContactTableList { get; set; }

    }

    public class LookupAccountAddressViewModel
    {
        public AccountAddressSearchFilter ContactAddressFilter { get; set; }
        public IEnumerable<AccountAddressSearchResult> AddressAddressTableList { get; set; }

        public int AddressTypeId { get; set; }
        public List<SelectListItem> AddressTypeList { get; set; }
    }
}