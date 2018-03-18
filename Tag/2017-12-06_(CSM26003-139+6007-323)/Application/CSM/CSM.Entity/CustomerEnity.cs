using CSM.Common.Utilities;
using CSM.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Entity
{
    public class CustomerEnity
    {
        public int CustomerId { get; set; }
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
                return PhoneList != null ? string.Join(",", PhoneList.Select(x => x.PhoneNo).ToList()) : "";
            } 
        }

        public List<PhoneEntity> PhoneList { get; set; }
        public string FirstName { get { return FirstNameThai; } }

        public string LastName { get { return LastNameThai; } }

        public string FullNameThai
        {
            get { return string.Format("{0} {1}", FirstNameThai, LastNameThai); }
        }

        public string FullNameEnglish
        {
            get { return string.Format("{0} {1}", FirstNameEnglish, LastNameEnglish); }
        }

        public string BirthDateDisplay
        {
            get
            {
                return BirthDate.HasValue ? BirthDate.Value.ToString(Constants.DateTimeFormat.DefaultShortDate) : "";
            }
        }

    }

    public class SubscriptTypeEntity
    {
        public int SubscriptTypeId { get; set; }
        public string SubscriptTypeName { get; set; }
    }

    public class TitleEntity
    {
        public int TitleId { get; set; }
        public string TitleNameTH { get; set; }
        public string TitleNameEN { get; set; }
    }

    public class CustomerSearchFilter : Pager
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CardNo { get; set; }
        public string AccountNo { get; set; }
        public string PhoneNo { get; set; }
    }
}
