using System;
using System.Globalization;

namespace CSM.Entity
{
    [Serializable]
    public class InquiryAccountAddressEntity
    {
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public string AccountTypeDescription { get; set; }
        public int Seq { get; set; }
        public string PhoneNo { get; set; }
        public string EMail { get; set; }
        public string AddressFormat { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string AddressLine5 { get; set; }
        public string AddressLine6 { get; set; }
        public string AddressLine7 { get; set; }
        public string PostalCode { get; set; }
        public string StateCode { get; set; }
        public string AddressDisplay {
            get
            {
                string strAddress = "";
                strAddress += !string.IsNullOrEmpty(AddressLine1) ? AddressLine1 : string.Empty;
                strAddress += !string.IsNullOrEmpty(AddressLine2) ? " " + AddressLine2 : string.Empty;
                strAddress += !string.IsNullOrEmpty(AddressLine3) ? " " + AddressLine3 : string.Empty;
                strAddress += !string.IsNullOrEmpty(AddressLine4) ? " " + AddressLine4 : string.Empty;
                strAddress += !string.IsNullOrEmpty(AddressLine5) ? " " + AddressLine5 : string.Empty;
                strAddress += !string.IsNullOrEmpty(AddressLine6) ? " " + AddressLine6 : string.Empty;
                strAddress += !string.IsNullOrEmpty(AddressLine7) ? " " + AddressLine7 : string.Empty;
                strAddress += !string.IsNullOrEmpty(PostalCode) ? string.Format(CultureInfo.InvariantCulture, " รหัสไปรษณีย์ {0}", PostalCode) : string.Empty;
                strAddress += !string.IsNullOrEmpty(StateCode) ? string.Format(CultureInfo.InvariantCulture, " ประเทศ {0} ", StateCode) : string.Empty;

                return strAddress;
            }
        }
    }
}
