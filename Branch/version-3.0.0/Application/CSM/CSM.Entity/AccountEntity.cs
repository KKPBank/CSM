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
        public string CustomerName { get; set; }
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

        public string CustomerEmail { get; set; }
        public string CustomerMobileNo { get; set; }

        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        //public string Status { get; set; }
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
                if (!string.IsNullOrEmpty(AccountStatus))
                {
                    if (AccountStatus.ToUpper(CultureInfo.InvariantCulture).Equals("A"))
                    {
                        strStatus = Resource.Ddl_Status_Active;
                    }
                }
               
                return strStatus;
            }
        }
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
                else if (!string.IsNullOrEmpty(AccountDesc))
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
        public decimal? CustomerNumber { get; set; }
        public int? TDGroupNumber { get; set; }
        public int? TDPlacementNumber { get; set; }
        public string AccountType { get; set; }
        public string AccountTypeDescription { get; set; }
        public string RenewalCounter { get; set; }
        public string Term { get; set; }
        public string TermCode { get; set; }

        public string ProductTerm
        {
            get
            {
                string result = "";
                result = Term + ' ' + TermCode;
                return result;
            }
        }
        public string TDEffectiveDateDisplay
        {
            get { return EffectiveDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }
        public DateTime? MaturityDate { get; set; }
        public string TDMaturityDateDisplay
        {
            get { return MaturityDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }
        public double? InterestRatePercentage { get; set; }
        public double? OriginalAmount { get; set; }
        public string AccountStatus { get; set; }
        public string AccountStatusDescription { get; set; }
        public string TDStatusDisplay 
        {
            get
            {
                string reStatus = "";
                if (!string.IsNullOrEmpty(AccountStatus))
                {
                    reStatus = AccountStatus;
                }

                if (!string.IsNullOrEmpty(AccountStatusDescription))
                {
                    if (!string.IsNullOrEmpty(reStatus))
                    {
                        reStatus += " " + AccountStatusDescription;
                    }
                    else
                    {
                        reStatus = AccountStatusDescription;
                    }
                }
                return reStatus;
            }
        }
        public int? BranchNumber { get; set; }

        public int?  ReceiptSerialNumber { get; set; }
        public string BundleCode { get; set; }
        public string ServiceInquiryProduct {
            get {
                string ret = "";

                if (AccountType == Constants.AccountType.CurrentAccount)
                    ret = "CASA";
                else if (AccountType == Constants.AccountType.LoanAccount)
                    ret = "LOAN";
                else if (AccountType == Constants.AccountType.TermDepositAccount)
                    ret = "TD";
                else if (AccountType == Constants.AccountType.SavingsAccount)
                    ret = "CASA";
                else if (AccountType == Constants.AccountType.SpecialAccount)
                    ret = "SP";
                else if (AccountType == Constants.AccountType.ServiceWithBank)
                    ret = "SB";
                else if (AccountType == Constants.AccountType.HirePurchase)
                    ret = "HP";
                
                return ret;
            }
        }
        public string InquiryServiceName { get; set; }
    }

    public class AccountSearchFilter : Pager
    {
        public int? CustomerId { get; set; }
        public decimal? CustomerNumber { get; set; }
        public bool IsLookUpMode { get; set; }
    }

    public class ViewProductDetailSearchFilter : Pager {
        public string ProductGroup { get; set; }
        public string AccountNumber { get; set; }
        public decimal? CustomerNumber { get; set; }
        public int? CustomerId { get; set; }
        
    }
}
