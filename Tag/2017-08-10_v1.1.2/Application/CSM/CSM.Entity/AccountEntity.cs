using System.Linq;
using CSM.Common.Resources;
using CSM.Common.Utilities;
using CSM.Entity.Common;
using System;
using System.Globalization;

namespace CSM.Entity
{
    [Serializable]
    public class AccountEntity
    {
        public int? AccountId { get; set; }
        public int? CustomerId { get; set; }
        public string ProductGroup { get; set; }
        public string Product { get; set; }
        public string CarNo { get; set; }
        public string AccountNo { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }

        public string BranchDisplay
        {
            get
            {
                string[] names = new string[2] { this.BranchCode.NullSafeTrim(), this.BranchName.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    string display = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + "-" + j);
                    return display;
                }

                return string.Empty;
            }
        }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; }
        public DateTime? UpdateDate { get; set; }

        public string EffectiveDateDisplay
        {
            get { return EffectiveDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }
        public string ExpiryDateDisplay
        {
            get { return ExpiryDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }

        public string StatusDisplay
        {
            get 
            {
                string strStatus = Resource.Ddl_Status_Inactive;
                if (!string.IsNullOrEmpty(Status))
                {
                    if (Status.ToUpper(CultureInfo.InvariantCulture).Equals("A"))
                    {
                        strStatus = Resource.Ddl_Status_Active;
                    }
                }
               
                return strStatus;
            }
        }

        public string Registration { get; set; } // ทะเบียนรถยนต์
        public string Grade { get; set; } // เกรด/สถานะ
        public string CountOfPayment { get; set; } // ผ่อนชำระมาแล้วกี่งวด      

        public string SubscriptionCode { get; set; }

        public string AccountDesc { get; set; } 

        public string AccountDescDisplay
        {           
            get
            {
                string result = "";
                if (!string.IsNullOrEmpty(AccountNo))
                {
                    result = AccountNo;
                }

                if (!string.IsNullOrEmpty(AccountDesc))
                {
                    if (!string.IsNullOrEmpty(result))
                    {
                        result += "/" + AccountDesc;
                    }
                    else
                    {
                        result = AccountDesc;
                    }
                }
                return result;
            }
        }

        public string ProductAndAccountNoDisplay
        {
            get
            {
                return (!string.IsNullOrEmpty(Product) ? (Product + " - ") : "") + AccountDescDisplay;
            }
        }
    }

    public class AccountSearchFilter : Pager
    {
        public int? CustomerId { get; set; }
    }
}
