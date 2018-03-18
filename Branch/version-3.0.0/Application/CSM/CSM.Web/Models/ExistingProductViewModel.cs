using System;
using CSM.Common.Utilities;
using CSM.Entity;
using System.Collections.Generic;
using System.Linq;

namespace CSM.Web.Models
{
    [Serializable]
    public class ExistingProductViewModel
    {
        public CustomerInfoViewModel CustomerInfo { get; set; }
        public AccountSearchFilter SearchFilter { get; set; }
        //public NewAccountSearchFilter TDSearchFilter { get; set; }
        public IEnumerable<AccountEntity> AccountList { get; set; }
        public ExistingProductEntity DetailProduct { get; set; }
    }

    [Serializable]
    public class ProductServiceViewModel
    {
        
        public CustomerInfoViewModel CustomerInfo { get; set; }
        public AccountSearchFilter SearchFilter { get; set; }
        public IEnumerable<AccountEntity> AccountList { get; set; }
        public ExistingProductEntity DetailProduct { get; set; }
    }

    [Serializable]
    public class TDAccountDetailViewModel
    {
        public string CIF_ID { get; set; }
        public string CustomerType { get; set; }
        public string BirthDate { get; set; }
        public string IDNumber { get; set; }
        public string CountryOfCitizenship { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public InquiryAccountAddressEntity AccountAddress { get; set; }

        public IEnumerable<TDAccountEntity> TDAccountList { get; set; }

        public ViewProductDetailSearchFilter SearchFilter { get; set; }

    }


    [Serializable]
    public class LoanAccountDetailViewModel
    {
        public string CustomerNumber { get; set; }
        public string BirthDateDisplay  { get;set; }
        public string IDNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string BankNumber { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string ApprovalDateDisplay
        {
            get { return ApprovalDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }
        public decimal? DrawingLimit { get; set; }
        public long? PaymentDayOfMonth { get; set; }
        public DateTime? FirstPaymentDate { get; set; }
        public string FirstPaymentDateDisplay
        {
            get { return FirstPaymentDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }
        public string LoanInterestRate { get; set; }
        public string Term { get; set; }
        public string PaymentAmount { get; set; }
        public DateTime? MaturityDate { get; set; }
        public string MaturityDateDisplay
        {
            get { return MaturityDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }
        public string StatusAccountCode { get; set; }
        public string StatusDescription { get; set; }
        public string StatusAccountDisplay {
            get {

                string[] StatusAccount = new string[2] { StatusAccountCode.NullSafeTrim(), StatusDescription.NullSafeTrim() };

                if (StatusAccount.Any(x => !string.IsNullOrEmpty(x)))
                {
                    return StatusAccount.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                }

                return string.Empty;
            }
        }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string AFTLoan { get; set; }
        public string InsuranceFlag { get; set; }
    }

    [Serializable]
    public class CASAAccountDetailViewModel
    {
        public string CIF_ID { get; set; }
        public string CustomerType { get; set; }
        public string BirthDate { get; set; }
        public string IDNumber { get; set; }
        public string CountryOfCitizenship { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public InquiryAccountAddressEntity AccountAddress { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string LocalDescription { get; set; }
        public string AvailableBalance { get; set; }
        public DateTime? DateOpened { get; set; }
        public string DateOpenedDisplay {
            get { return DateOpened.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }
        public string Officer { get; set; }
        public string AccountStatus { get; set; }
        public string PassbookFlag { get; set; }
        public string SignatureConditionCode { get; set; }
        public string SignatureConditionDescription { get; set; }

        public string SignatureConditionDisplay {
            get
            {
                string[] names = new string[2] { this.SignatureConditionCode.NullSafeTrim(), this.SignatureConditionDescription.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    return names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                }

                return string.Empty;
            }
        }
        public long? BranchNumber { get; set; }

        public IEnumerable<CASATransactionEntity> TransactionList { get; set; }


        public string Mobile
        {
            get { return StringHelpers.ConvertListToString(MobileList.Select(x => x.PhoneNo).ToList<object>(), ","); }
        }
        public List<PhoneEntity> MobileList { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }


        //public string SignatureConditionName { get; set; }
        //public string FullSignature
        //{
        //    get
        //    {
        //        string[] names = new string[2] { this.SignatureConditionCode.NullSafeTrim(), this.SignatureConditionName.NullSafeTrim() };

        //        if (names.Any(x => !string.IsNullOrEmpty(x)))
        //        {
        //            return names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
        //        }

        //        return string.Empty;
        //    }
        //}


        public ViewProductDetailSearchFilter SearchFilter { get; set; }
    }
}