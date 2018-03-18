using CSM.Common.Utilities;
using CSM.Entity.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CSM.Entity
{
    public class InstructionEntity
    {
        public decimal CustomerNumber { get; set; }
        public int CustomerId { get; set; }
        public int ContactId { get; set; }
        public BankEntity Bank { get; set; }
        public string AccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string InstructionDescription { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string EffectiveDateDisplay
        {
            get
            {
                return EffectiveDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate);
            }
        }
        public string SubscriptionStatus { get; set; }
    }

    public class InstructionSearchFilter : Pager
    {
        public int? CustomerId { get; set; }
        public decimal? CustomerNumber { get; set; }
    }
}
