using CSM.Common.Utilities;
using CSM.Common.Resources;
using CSM.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSM.Entity
{
    [Serializable]
    public class ContactEntity
    {
        public int? CustomerContactId { get; set; }
        public int? ContactId { get; set; }
        public string FirstNameThai { get; set; }
        public string LastNameThai { get; set; }     
        public string FirstNameEnglish { get; set; }
        public string LastNameEnglish { get; set; }
        public string CardNo { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; } 
        public string FirstName { get { return FirstNameThai; } }
        public string LastName { get { return LastNameThai; } }
        public string FirstNameThaiEng { get; set; } // for display & sorting
        public string LastNameThaiEng { get; set; } // for display & sorting
        public string BirthDateDisplay
        {
            get { return BirthDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }

        public bool? IsEdit { get; set; }

        public RelationshipEntity Relationship { get; set; }
        public AccountEntity Account { get; set; }
        public UserEntity CreateUser { get; set; }
        public UserEntity UpdateUser { get; set; }

        public SubscriptTypeEntity SubscriptType { get; set; }
        public TitleEntity TitleThai { get; set; }
        public TitleEntity TitleEnglish { get; set; }
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
        public List<PhoneEntity> PhoneList { get; set; }
        public string Fax { get; set; }

        public int? CustomerId { get; set; }
        public int? AccountId { get; set; }
        public int? RelationshipId { get; set; }

        public bool? IsDefault { get; set; }
        public string StrPhoneNo { get; set; }
        public string System { get; set; }

        public string RelationshipNameDisplay
        {
            get
            {
                string relationshipName = this.Relationship != null ? this.Relationship.RelationshipName : string.Empty;

                //if (this.UpdateUser != null && Constants.SystemName.BDW.Equals(this.UpdateUser.Username))
                //    return string.Format("{0} ({1})", Resource.Lbl_Guarantor, relationshipName);

                if (this.System != null && Constants.SystemName.BDW.Equals(this.System))
                    return string.Format("{0} ({1})", Resource.Lbl_Guarantor, relationshipName);

                return relationshipName;
            }
        }
    }

    [Serializable]
    public class ContactSearchFilter : Pager
    {
        public int? CustomerId { get; set; }
    }
}
