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
        public decimal? CustomerNumber { get; set; }
        public CbsCardTypeEntity CbsCardType { get; set; }
        public TitleEntity TitleThai { get; set; }
        public string FirstNameThai { get; set; }
        public string LastNameThai { get; set; }
        public string[] CustomerNameThai {
            set
            {
                if (value.Length == 1)
                {
                    FirstNameThai = value[0];
                }
                else if (value.Length == 2)
                {
                    FirstNameThai = value[0].Trim();
                    LastNameThai = value[1].Trim();
                }
                else if (value.Length > 2)
                {
                    FirstNameThai = value[0].Trim();
                    for (int i = 1; i < value.Length; i++)
                    {
                        LastNameThai += value[i] + " ";
                    }
                }
            }   
        }
        public string[] CustomerNameEng {
            set {
                if (value.Length == 1)
                {
                    FirstNameEnglish = value[0];
                }
                else if (value.Length == 2)
                {
                    FirstNameEnglish = value[0].Trim();
                    LastNameEnglish = value[1].Trim();
                }
                else if (value.Length > 2)
                {
                    FirstNameEnglish = value[0].Trim();
                    for (int i = 1; i < value.Length; i++)
                    {
                        LastNameEnglish += value[i] + " ";
                    }
                }
            }
        }
        public string CustomerNameThaiEng
        {
            set {
                FirstNameThaiEng = !string.IsNullOrEmpty(FirstNameThai) ? FirstNameThai : FirstNameEnglish;
                LastNameThaiEng = !string.IsNullOrEmpty(LastNameThai) ? LastNameThai : LastNameEnglish;
            }
        }

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
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string FirstName { get { return FirstNameThai; } }

        public string LastName { get { return LastNameThai; } }

        public string FullNameThai
        {
            get
            {
                string[] names = new string[2] { this.FirstNameThai.NullSafeTrim(), this.LastNameThai.NullSafeTrim() };

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
        //New Column v.3
        public decimal? CIF_ID { get; set; } //KKCIS_ID in SQl
        public string CustomerCategory { get; set; }
        public string IDTypeCode { get; set; }
        public string CountryOfCitizenship { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerWebserviceType { get; set; }
    }

    

    [Serializable]
    public class CustomerSearchFilter : Pager
    {
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FirstName { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string LastName { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardNo { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AccountNo { get; set; }

        //[LocalizedRegex("([0-9#]+)", "ValErr_NumericAndExtOnly")]
        //[LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
        //    ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        //[LocalizedStringLength(Constants.MaxLength.PhoneNo, ErrorMessageResourceName = "ValErr_StringLength",
        //   ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PhoneNo { get; set; }
        public int? CustomerType { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Registration { get; set; }
        public string Product { get; set; }
        public string Grade { get; set; }
        public string BranchName { get; set; }
        public string Status { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CustomerDeptFirstName { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CustomerDeptLastName { get; set; }

        public bool IsAdminNoteSearch { get; set; }

        public bool ExactFirstName { get; set; }
        public bool ExactLastName { get; set; }
        public bool ExactPhoneNo { get; set; }
    }

    public class ContractSearchFilter : Pager
    {
        public int CustomerId { get; set; }
        public decimal? CustomerNo { get; set; }

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

        public string Product { get; set; }
        public int? CustomerType { get; set; }
        public string Registration { get; set; }

        [LocalizedMinLength(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
          ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CustomerDeptFirstName { get; set; }

        [LocalizedMinLength(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CustomerDeptLastName { get; set; }
    }

    public class CustomerContactSearchFilter : Pager
    {
        public int CustomerId { get; set; }
        public decimal? CustomerNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CardNo { get; set; }
        public string AccountNo { get; set; }
        public string PhoneNo { get; set; }
    }

    public class CustomerComparer : IEqualityComparer<CustomerEntity>
    {
        public bool Equals(CustomerEntity x, CustomerEntity y)
        {
            bool ret = x.CIF_ID == y.CIF_ID && x.FullNameThai == y.FullNameThai && x.FullNameEnglish == y.FullNameEnglish
                && x.CardNo == y.CardNo && x.CbsCardType?.CardTypeID == y.CbsCardType?.CardTypeID
                && x.CustomerType == y.CustomerType;
            return ret;
        }

        public int GetHashCode(CustomerEntity x)
        {
            return (x.CIF_ID ?? x.CustomerId).GetHashCode();
        }
    }
}
