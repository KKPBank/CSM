using System;
using System.Globalization;
using CSM.Entity.Common;

namespace CSM.Entity
{
    [Serializable]
    public class CustomerAddressEntity
    {
        public decimal CustomerNumber { get; set; }
        public string AddressFormatCode { get; set; }
        public string AddressFormatName { get; set; }
        public string AddressTypeCode { get; set; }
        public string AddressTypeName { get; set; }
        public long Seq { get; set; }
        public string HouseNumber { get; set; }
        public string MooLabel { get; set; }
        public string Moo { get; set; }
        public string FloorNumberLabel { get; set; }
        public string FloorNumber { get; set; }
        public string RoomNumberLabel { get; set; }
        public string RoomNumber { get; set; }
        public string Building { get; set; }
        public string SoiLabel { get; set; }
        public string Soi { get; set; }
        public string RoadLabel { get; set; }
        public string Road { get; set; }
        public CbsSubDistrictEntity SubDistrict { get; set; }
        public CbsDistrictEntity District { get; set; }
        public CbsProvinceEntity Province { get; set; }
        public string AddressAreaDetail { get; set; }
        public string PostalCode { get; set; }
        public string AddressDisplay
        {
            get
            {
                string strAddress = "";
                strAddress += !string.IsNullOrEmpty(HouseNumber) ? HouseNumber : string.Empty;
                strAddress += !string.IsNullOrEmpty(Moo) ? " " + MooLabel + " " + Moo : string.Empty;
                strAddress += !string.IsNullOrEmpty(FloorNumber) ? " " + FloorNumberLabel + " " + FloorNumber : string.Empty;
                strAddress += !string.IsNullOrEmpty(RoomNumber) ? " " + RoomNumberLabel + " " + RoomNumber : string.Empty;
                strAddress += !string.IsNullOrEmpty(Building) ? " " + Building : string.Empty;
                strAddress += !string.IsNullOrEmpty(Soi) ? " " + SoiLabel + " " + Soi : string.Empty;
                strAddress += !string.IsNullOrEmpty(Road) ? " " + RoadLabel + " " + Road : string.Empty;

                if (Province != null)
                {
                    if (Province.ProvinceCode == "10")
                    {
                        strAddress += SubDistrict != null ? " แขวง " + SubDistrict.SubDistrictName : string.Empty;
                        strAddress += District != null ? " เขต " + District.DistrictName : string.Empty;
                        strAddress += " " + Province.ProvinceName;
                    }
                    else
                    {
                        strAddress += SubDistrict != null ? " ตำบล " + SubDistrict.SubDistrictName : string.Empty;
                        strAddress += District != null ? " อำเภอ " + District.DistrictName : string.Empty;
                        strAddress += " จังหวัด " + Province.ProvinceName;
                    }
                }

                strAddress += !string.IsNullOrEmpty(PostalCode) ? string.Format(CultureInfo.InvariantCulture, " รหัสไปรษณีย์ {0}", PostalCode) : string.Empty;

                return strAddress;
            }
        }
    }
}
