using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisSubscribeAddressEntity
    {
        [Display(Name = "KKCIS_ID")]
        [LocalizedStringLength(Constants.MaxLength.KKCisId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string KKCisId { get; set; }
        public int? CustId { get; set; }

        [Display(Name = "CARD_ID")]
        [LocalizedStringLength(Constants.MaxLength.Card_Id, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardId { get; set; }

        [Display(Name = "CARD_TYPE_CODE")]
        [LocalizedStringLength(Constants.MaxLength.address_card_type_code, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardTypeCode { get; set; }        
        public string CustTypeGroup { get; set; }

        [Display(Name = "PROD_GROUP")]
        [LocalizedStringLength(Constants.MaxLength.Prod_Group, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProdGroup { get; set; }

        [Display(Name = "PROD_TYPE")]
        [LocalizedStringLength(Constants.MaxLength.Prod_Type, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProdType { get; set; }

        [Display(Name = "SUBSCR_CODE")]
        [LocalizedStringLength(Constants.MaxLength.SubscrCode, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubscrCode { get; set; }

        [Display(Name = "ADDRESS_TYPE_CODE")]
        [LocalizedStringLength(Constants.MaxLength.address_type_code, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AddressTypeCode { get; set; }

        [Display(Name = "ADDRESS_NUMBER")]
        [LocalizedStringLength(Constants.MaxLength.address_number, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AddressNumber { get; set; }

        [Display(Name = "VILLAGE")]
        [LocalizedStringLength(Constants.MaxLength.village, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Village { get; set; }

        [Display(Name = "BUILDING")]
        [LocalizedStringLength(Constants.MaxLength.building, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Building { get; set; }

        [Display(Name = "FLOOR_NO")]
        [LocalizedStringLength(Constants.MaxLength.floor_no, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FloorNo { get; set; }

        [Display(Name = "ROOM_NO")]
        [LocalizedStringLength(Constants.MaxLength.room_no, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string RoomNo { get; set; }

        [Display(Name = "MOO")]
        [LocalizedStringLength(Constants.MaxLength.moo, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Moo { get; set; }

        [Display(Name = "STREET")]
        [LocalizedStringLength(Constants.MaxLength.street, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Street { get; set; }

        [Display(Name = "SOI")]
        [LocalizedStringLength(Constants.MaxLength.soi, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Soi { get; set; }

        [Display(Name = "SUB_DISTRICT_CODE")]
        [LocalizedStringLength(Constants.MaxLength.sub_district_code, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubDistrictCode { get; set; }

        [Display(Name = "DISTRICT_CODE")]
        [LocalizedStringLength(Constants.MaxLength.add_district_code, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DistrictCode { get; set; }

        [Display(Name = "PROVICE_CODE")]
        [LocalizedStringLength(Constants.MaxLength.provice_code, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProvinceCode { get; set; }

        [Display(Name = "COUNTRY_CODE")]
        [LocalizedStringLength(Constants.MaxLength.country_code, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CountryCode { get; set; }

        [Display(Name = "POSTAL_CODE")]
        [LocalizedStringLength(Constants.MaxLength.postal_code, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PostalCode { get; set; }

        [Display(Name = "SUB_DISTRICT_VALUE")]
        [LocalizedStringLength(Constants.MaxLength.sub_district_value, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubDistrictValue { get; set; }

        [Display(Name = "DISTRICT_VALUE")]
        [LocalizedStringLength(Constants.MaxLength.district_value, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DistrictValue { get; set; }

        [Display(Name = "PROVINCE_VALUE")]
        [LocalizedStringLength(Constants.MaxLength.province_value, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProvinceValue { get; set; }

        [Display(Name = "POSTAL_VALUE")]
        [LocalizedStringLength(Constants.MaxLength.postal_value, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PostalValue { get; set; }

        [Display(Name = "CREATE_DATE")]
        [LocalizedStringLength(Constants.MaxLength.CreatedDate, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedDate { get; set; }

        [Display(Name = "CREATE_BY")]
        [LocalizedStringLength(Constants.MaxLength.address_created_by, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedBy { get; set; }

        [Display(Name = "UPDATE_DATE")]
        [LocalizedStringLength(Constants.MaxLength.UpdatedDate, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedDate { get; set; }

        [Display(Name = "UPDATE_BY")]
        [LocalizedStringLength(Constants.MaxLength.address_updated_by, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedBy { get; set; }
    }
}
