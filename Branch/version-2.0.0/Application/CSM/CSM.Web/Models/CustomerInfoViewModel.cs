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
    }
}