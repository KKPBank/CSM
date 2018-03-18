using System;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CASATransactionEntity
    {
        public DateTime? TransactionDate { get; set; }
        public string TransactionDateDisplay
        {
            get { return TransactionDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime); }
        }
        public string PostingTimeStamp { get; set; }
        
        public string TransactionDescription { get; set; }
        public long? ChequeNumber { get; set; }
        public string DebitCreditCode { get; set; }
        public long? TransactionBranch { get; set; }
        public string TransactionBranchDescription { get; set; }
        public string TransactionAmount { get; set; }
    }
}
