using CSM.Common.Utilities;
using CSM.Entity.Common;
using System;

namespace CSM.Entity
{
    public class AccountPhoneEntity
    {
        public int? AccountPhoneId { get; set; }
        public int? CustomerId { get; set; }
        public string PhoneNo { get; set; }
        public string PhoneExt { get; set; }
        public string PhoneTypeCode { get; set; }
        public string PhoneTypeName { get; set; }
        public string ProductGroup { get; set; }
        public string ProductType { get; set; }
        public string SubscriptionCode { get; set; }
        public string SubscriptionName { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
