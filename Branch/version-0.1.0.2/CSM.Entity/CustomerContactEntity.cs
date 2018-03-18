using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Entity
{
    [Serializable]
    public class CustomerContactEntity
    {
        public int? CustomerContactId { get; set; }
        public int? CustomerId { get; set; }
        public int? ContactId { get; set; }              
        public int? AccountId { get; set; }
        public string AccountNo { get; set; }      
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
    }
}
