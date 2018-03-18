using CSM.Common.Resources;
using CSM.Common.Utilities;
using System;
using System.Collections.Generic;

namespace CSM.Entity
{
    [Serializable]
    public class CustomerContactEntity
    {
        public int? CustomerContactId { get; set; }
        public int? CustomerId { get; set; }
        public decimal? CustomerNumber { get; set; }
        public int? ContactId { get; set; }              
        public string AccountNo { get; set; }      
        public string AccountType { get; set; }  
        public AccountEntity Account { get; set; }
        public string Product { get; set; }
        public int RelationshipId { get; set; }
        public string RelationshipName { get; set; }
        public UserEntity CreateUser { get; set; }
        public UserEntity UpdateUser { get; set; }
        public bool? IsEdit { get; set; }
        public string CustomerFullNameTh { get; set; }
        public string CustomerFullNameEn { get; set; }

        public string CustomerFullName
        {
            get
            {
                return CustomerFullNameTh;
            }
        }

        public string CustomerFullNameThaiEng { get; set; }
        
        public string ContactFromSystem { get; set; }

        public string RelationshipNameDisplay
        {
            get
            {
                if (this.ContactFromSystem != null && Constants.SystemName.BDW.Equals(this.ContactFromSystem))
                    return string.Format("{0} ({1})", Resource.Lbl_Guarantor, this.RelationshipName);

                return this.RelationshipName;
            }
        }

        public string Fax { get; set; }
        public string Email { get; set; }
        public List<PhoneEntity> PhoneList { get; set; }
    }
}
