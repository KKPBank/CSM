using CSM.Common.Utilities;
using CSM.Entity.Common;
using System;

namespace CSM.Entity
{
    public class AccountEmailEntity
    {
        public int? CustomerId { get; set; }
        public int? EmailId { get; set; }
        public string ProductGroup { get; set; }
        public string ProductType { get; set; }
        public string SubscriptionCode { get; set; }
        public string EmailTypeCode { get; set; }
        public string EmailTypeName { get; set; }
        public string EmailAccount { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
