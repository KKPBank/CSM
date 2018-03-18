using CSM.Common.Utilities;
using CSM.Entity.Common;
using System;

namespace CSM.Entity
{
    [Serializable]
    public class NoteEntity
    {
        public int? NoteId { get; set; }
        public int? CustomerId { get; set; }
        public decimal? CustomerNumber { get; set; }
        public string Detail { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public UserEntity CreateUser { get; set; }
        public UserEntity UpdateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public string EffectiveDateDisplay
        {
            get
            {
                return EffectiveDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate);
            }
        }

        public string ExpiryDateDisplay
        {
            get
            {
                return ExpiryDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate);
            }
        }
        public string CreateDateDisplay
        {
            get
            {
                return CreateDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
            }
        }

        public string UpdateDateDisplay
        {
            get
            {
                return UpdateDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
            }
        }

        public int? TDGroupNumber { get; set; }
        public int? TDPlacementNumber { get; set; }
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

        public int? ReceiptSerialNumber { get; set; }



 

 }


    public class NoteSearchFilter : Pager
    {
        public decimal? CustomerId { get; set; }
        public decimal? CustomerNumber { get; set; }
        public DateTime? EffectiveDate { get; set; }

    }
}
