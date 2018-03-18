using CSM.Common.Utilities;
using CSM.Common.Resources;
using CSM.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSM.Entity
{
    [Serializable]
    public class CustomerEntity
    {
        public long? RowNum { get; set; }
        public int? CustomerId { get; set; }
        public SubscriptTypeEntity SubscriptType { get; set; }
        public TitleEntity TitleThai { get; set; }
        public string FirstNameThai { get; set; }
        public string LastNameThai { get; set; }
        public TitleEntity TitleEnglish { get; set; }
        public string FirstNameEnglish { get; set; }
        public string LastNameEnglish { get; set; }
        public string CardNo { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string AccountNo { get; set; }
        public string PhoneNo 
        { 
            get 
            {
                if (PhoneList != null)
                {
                   return StringHelpers.ConvertListToString(PhoneList.Select(x => x.PhoneNo).ToList<object>(), ","); 
                }
                else if (!string.IsNullOrEmpty(StrPhoneNo))
                {
                    return StrPhoneNo;
                }

                return "";
            } 
        }

        public string StrPhoneNo { get; set; }

        public List<PhoneEntity> PhoneList { get; set; }
        public int? EmployeeId { get; set; }
        public string FirstName { get { return FirstNameThai; } }

        public string LastName { get { return LastNameThai; } }

        public string FullNameThai
        {
            get
            {
                string[] names = new string[2] {this.FirstNameThai.NullSafeTrim(), this.LastNameThai.NullSafeTrim()};

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    return names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                }

                return string.Empty;
            }
        }

        public string FullNameEnglish
        {
            get
            {
                string[] names = new string[2] { this.FirstNameEnglish.NullSafeTrim(), this.LastNameEnglish.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    return names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                }

                return string.Empty;
            }
        }

        public string FirstNameThaiEng { get; set; } // for display & sorting

        public string LastNameThaiEng { get; set; } // for display & sorting

        public string BirthDateDisplay
        {
            get { return BirthDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }

        public AccountEntity Account { get; set; }
        public int? CustomerType { get; set; }
        public string Registration { get; set; }

        public string CustomerTypeDisplay
        {
            get
            {
                string typeName = string.Empty;
                if (this.CustomerType.HasValue)
                {
                    if (this.CustomerType.Value == Constants.CustomerType.Customer)
                    {
                        typeName = Resource.Ddl_CustomerType_Customer;
                    }
                    else if (this.CustomerType.Value == Constants.CustomerType.Prospect)
                    {
                        typeName = Resource.Ddl_CustomerType_Prospect;
                    }
                    else if (this.CustomerType.Value == Constants.CustomerType.Employee)
                    {
                        typeName = Resource.Ddl_CustomerType_Employee;
                    }
                }
                return typeName;
            }
        }

        public UserEntity CreateUser { get; set; }
        public UserEntity UpdateUser { get; set; }
        public int? DummyAccountId { get; set; }
        public int? DummyCustomerContactId { get; set; }
    }     

    [Serializable]
    public class CustomerSearchFilter : Pager
    {
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FirstName { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string LastName { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardNo { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AccountNo { get; set; }

        [LocalizedRegex("([0-9#*]+)", "ValErr_NumericExtAndStarOnly")]
        [LocalizedMinLength(Constants.MinLenght.PhoneNo, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedStringLength(Constants.MaxLength.PhoneNo, ErrorMessageResourceName = "ValErr_StringLength",
           ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneNo { get; set; }
        public string CustomerType { get; set; }
        public string Registration { get; set; }
        public string Product { get; set; }
        public string Grade { get; set; }
        public string BranchName { get; set; }
        public string Status { get; set; }

    }

    public class ContractSearchFilter : Pager
    {
        public int CustomerId { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AccountNo { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CarNo { get; set; }

        public string ProductGroupName { get; set; }
        public string BranchName { get; set; }
        public string ProductName { get; set; }
        public string Status { get; set; }

    }

    public class CustomerContactSearchFilter : Pager
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CardNo { get; set; }
        public string AccountNo { get; set; }
        public string PhoneNo { get; set; }
    }
}
