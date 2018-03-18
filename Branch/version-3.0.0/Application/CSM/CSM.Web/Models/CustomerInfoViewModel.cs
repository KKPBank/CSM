using CSM.Common.Utilities;
using CSM.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSM.Web.Models
{
    [Serializable]
    public class CustomerInfoViewModel
    {
        public int? CustomerId { get; set; }
        public decimal? CustomerNumber { get; set; }
        public string CustomerNumberDisplay {
            get {
                string ret = "";
                if (CustomerNumber.HasValue && CustomerNumber > 0) {
                    ret = CustomerNumber.Value.ToString("#");
                }
                return ret;
            }
        }
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
            get { return StringHelpers.ConvertListToString(PhoneList.Select(x => x.PhoneNo).ToList<object>(), ","); }
        }

        public List<PhoneEntity> PhoneList { get; set; }
        public string FirstName { get { return FirstNameThai; } }

        public string LastName { get { return LastNameThai; } }
        public string FirstNameThaiEng { get; set; } // for display & sorting
        public string LastNameThaiEng { get; set; } // for display & sorting

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

        public string BirthDateDisplay
        {
            get { return BirthDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }

        public AccountEntity Account { get; set; }
        public int? CustomerType { get; set; }
        public string Registration { get; set; }

        public string CustomerTypeDisplay
        {
            get { return Constants.CustomerType.GetMessage(this.CustomerType); }
        }

        public UserEntity CreateUser { get; set; }
        public UserEntity UpdateUser { get; set; }
        public string CountryOfCitizenship { get; set; }

        public string BankNumber { get; set; }
        public int? AccountNumber { get; set; }
        public string Address { get; set; }
        public string AccountName { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public string ApprovalDateDisplay
        {
            get { return ApprovalDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }

        public int? DrawingLimit { get; set; }
        public string PaymentDayOfMonth { get; set; }
        public DateTime? FirstPaymentDate { get; set; }
        public string FirstPaymentDateDisplay
        {
            get { return FirstPaymentDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }

        public double? LoanInterestRate { get; set; }
        public int? Term { get; set; }
        public int? PaymentAmount { get; set; }
        public DateTime?  MaturityDate { get; set; }
        public string MaturityDateDateDisplay
        {
            get { return MaturityDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }

        public string StatusAccountCode { get; set; }
        public string StatusDescription { get; set; }
        public string FullStatus
        {
            get
            {
                string[] names = new string[2] { this.StatusAccountCode.NullSafeTrim(), this.StatusDescription.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    return names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                }

                return string.Empty;
            }
        }

        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string AFTLoan { get; set; }
        public string InsuranceFlag { get; set; }

        public string LocalDescription { get; set; }
        public decimal? AvailableBalance { get; set; }
        public string Officer { get; set; }
        public string Status { get; set; }
        public string PassbookFlag { get; set; }
        public string SignatureConditionCode { get; set; }
        public string SignatureConditionName { get; set; }
        public string FullSignature
        {
            get
            {
                string[] names = new string[2] { this.SignatureConditionCode.NullSafeTrim(), this.SignatureConditionName.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    return names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                }

                return string.Empty;
            }
        }
        public int? BranchNumber { get; set; }




    }


}