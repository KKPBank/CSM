using System.Linq;
using CSM.Common.Resources;
using CSM.Common.Utilities;
using CSM.Entity.Common;
using System;
using System.Globalization;

namespace CSM.Entity
{
    [Serializable]
    public class TDAccountEntity
    {
        public string TDPlacementNumber { get; set; }
        public string CustomerNumber { get; set; }
        public string TDGroupNumber { get; set; }
        public string TDAccountTypeDescription { get; set; }
        public long RenewalCounter { get; set; }
        public long ProductTerm { get; set; }
        public string ProductTermCode { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string EffectiveDateDisplay
        {
            get { return EffectiveDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }
        public DateTime? MaturityDate { get; set; }
        public string MaturityDateDisplay
        {
            get { return MaturityDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }
        public decimal? InterestRatePercentage { get; set; }
        public string OriginalAmount { get; set; }
        public long? AccountStatus { get; set; }
        public string AccountStatusDescription { get; set; }
        public string TDAccountStatusDisplay {
            get {
                return AccountStatus.ConvertToString() + " " + AccountStatusDescription;
            }
        }
        public string BranchName { get; set; }
        public string ReceiptSerialNumber { get; set; }
    }

    public class TDAccountSearchFilter : Pager
    {
        public string AccountNo { get; set; }
    }
}
