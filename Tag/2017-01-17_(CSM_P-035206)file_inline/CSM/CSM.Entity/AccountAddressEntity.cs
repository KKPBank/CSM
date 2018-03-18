using CSM.Entity.Common;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CSM.Entity
{
    public class AccountAddressEntity
    {
        public int? AddressId { get; set; }
        public int? CustomerId { get; set; }
        public string AddressTypeCode { get; set; }
        public string AddressTypeName { get; set; }
        public string AddressNo { get; set; }
        public string Village { get; set; }
        public string Building { get; set; }
        public string FloorNo { get; set; }
        public string RoomNo { get; set; }
        public string Moo { get; set; }
        public string Street { get; set; }
        public string Soi { get; set; }
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string Postcode { get; set; }
        public decimal? KkcisId { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string ProductGroup { get; set; }
        public string ProductType { get; set; }
        public string SubscriptionCode { get; set; }

        public string AccountAddress { get; set; }

        public string AddressDiplay
        {
            get
            {
                string strAddress = "";
                strAddress += !string.IsNullOrEmpty(AddressNo) ? string.Format(CultureInfo.InvariantCulture, "เลขที่ {0} ", AddressNo) : string.Empty;
                strAddress += !string.IsNullOrEmpty(Village) ? string.Format(CultureInfo.InvariantCulture, " หมู่บ้าน {0} ", Village) : string.Empty;
                strAddress += !string.IsNullOrEmpty(Building) ? string.Format(CultureInfo.InvariantCulture, " อาคาร {0} ", Building) : string.Empty;
                strAddress += !string.IsNullOrEmpty(FloorNo) ? string.Format(CultureInfo.InvariantCulture, " ชั้น {0} ", FloorNo) : string.Empty;
                strAddress += !string.IsNullOrEmpty(RoomNo) ? string.Format(CultureInfo.InvariantCulture, " ห้อง {0} ", RoomNo) : string.Empty;
                strAddress += !string.IsNullOrEmpty(Moo) ? string.Format(CultureInfo.InvariantCulture, " หมู่ {0} ", Moo) : string.Empty;
                strAddress += !string.IsNullOrEmpty(Street) ? string.Format(CultureInfo.InvariantCulture, " ถนน {0} ", Street) : string.Empty;
                strAddress += !string.IsNullOrEmpty(Soi) ? string.Format(CultureInfo.InvariantCulture, " ซอย {0} ", Soi) : string.Empty;
                strAddress += !string.IsNullOrEmpty(SubDistrict) ? string.Format(CultureInfo.InvariantCulture, " แขวง {0} ", SubDistrict) : string.Empty;
                strAddress += !string.IsNullOrEmpty(District) ? string.Format(CultureInfo.InvariantCulture, " เขต {0} ", District) : string.Empty;
                strAddress += !string.IsNullOrEmpty(Province) ? string.Format(CultureInfo.InvariantCulture, " จังหวัด {0} ", Province) : string.Empty;
                strAddress += !string.IsNullOrEmpty(Postcode) ? string.Format(CultureInfo.InvariantCulture, " รหัสไปรษณีย์ {0}", Postcode) : string.Empty;
                strAddress += !string.IsNullOrEmpty(Country) ? string.Format(CultureInfo.InvariantCulture, " ประเทศ {0} ", Country) : string.Empty;

                return strAddress;
            }
        }

        public string AddressDiplayForSr
        {
            get
            {
                var token = new List<string>();

                if (!string.IsNullOrEmpty(AddressNo))
                    token.Add(AddressNo);

                if (!string.IsNullOrEmpty(Village))
                    token.Add(Village);

                if (!string.IsNullOrEmpty(Building))
                    token.Add(Building);

                if (!string.IsNullOrEmpty(FloorNo))
                    token.Add(FloorNo);

                if (!string.IsNullOrEmpty(RoomNo))
                    token.Add(RoomNo);

                if (!string.IsNullOrEmpty(Moo))
                    token.Add(Moo);

                if (!string.IsNullOrEmpty(Street))
                    token.Add(Street);

                if (!string.IsNullOrEmpty(SubDistrict))
                    token.Add(SubDistrict);

                if (!string.IsNullOrEmpty(District))
                    token.Add(District);

                if (!string.IsNullOrEmpty(Province))
                    token.Add(Province);

                if (!string.IsNullOrEmpty(Country))
                    token.Add(Country);

                if (!string.IsNullOrEmpty(Postcode))
                    token.Add(Postcode);

                return string.Join(" ", token);
            }
        }
    }

    public class AccountAddressSearchFilter : Pager
    {
        public string Type { get; set; }
        public string Address { get; set; }
        public int CustomerId { get; set; }
    }

    public class AccountAddressTypeEntity
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
    }
}
