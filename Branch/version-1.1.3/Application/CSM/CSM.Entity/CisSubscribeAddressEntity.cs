using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CisSubscribeAddressEntity
    {
        [Display(Name = "KKCIS_ID")]
        [LocalizedStringLength(Constants.CisMaxLength.KKCisId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string KKCisId { get; set; }
        public string CustId { get; set; }

        [Display(Name = "CARD_ID")]
        [LocalizedStringLength(Constants.CisMaxLength.CardId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardId { get; set; }

        [Display(Name = "CARD_TYPE_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.CardTypeCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardTypeCode { get; set; }        
        public string CustTypeGroup { get; set; }

        [Display(Name = "PROD_GROUP")]
        [LocalizedStringLength(Constants.CisMaxLength.ProdGroup, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProdGroup { get; set; }

        [Display(Name = "PROD_TYPE")]
        [LocalizedStringLength(Constants.CisMaxLength.ProdType, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProdType { get; set; }

        [Display(Name = "SUBSCR_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.SubscrCode, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubscrCode { get; set; }

        [Display(Name = "ADDRESS_TYPE_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.AddressTypeCode, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AddressTypeCode { get; set; }

        [Display(Name = "ADDRESS_NUMBER")]
        [LocalizedStringLength(Constants.CisMaxLength.AddressNumber, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AddressNumber { get; set; }

        [Display(Name = "VILLAGE")]
        [LocalizedStringLength(Constants.CisMaxLength.Village, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Village { get; set; }

        [Display(Name = "BUILDING")]
        [LocalizedStringLength(Constants.CisMaxLength.Building, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Building { get; set; }

        [Display(Name = "FLOOR_NO")]
        [LocalizedStringLength(Constants.CisMaxLength.FloorNo, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FloorNo { get; set; }

        [Display(Name = "ROOM_NO")]
        [LocalizedStringLength(Constants.CisMaxLength.RoomNo, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string RoomNo { get; set; }

        [Display(Name = "MOO")]
        [LocalizedStringLength(Constants.CisMaxLength.Moo, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Moo { get; set; }

        [Display(Name = "STREET")]
        [LocalizedStringLength(Constants.CisMaxLength.Street, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Street { get; set; }

        [Display(Name = "SOI")]
        [LocalizedStringLength(Constants.CisMaxLength.Soi, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Soi { get; set; }

        [Display(Name = "SUB_DISTRICT_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.SubdistrictCode, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubDistrictCode { get; set; }

        [Display(Name = "DISTRICT_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.DistrictCode, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DistrictCode { get; set; }

        [Display(Name = "PROVICE_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.ProvinceCode, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProvinceCode { get; set; }

        [Display(Name = "COUNTRY_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.CountryCode, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CountryCode { get; set; }

        [Display(Name = "POSTAL_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.PostCode, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PostalCode { get; set; }

        [Display(Name = "SUB_DISTRICT_VALUE")]
        [LocalizedStringLength(Constants.CisMaxLength.SubDistrictValue, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubDistrictValue { get; set; }

        [Display(Name = "DISTRICT_VALUE")]
        [LocalizedStringLength(Constants.CisMaxLength.DistrictValue, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DistrictValue { get; set; }

        [Display(Name = "PROVINCE_VALUE")]
        [LocalizedStringLength(Constants.CisMaxLength.ProvinceValue, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProvinceValue { get; set; }

        [Display(Name = "POSTAL_VALUE")]
        [LocalizedStringLength(Constants.CisMaxLength.PostalValue, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PostalValue { get; set; }

        [Display(Name = "CREATE_DATE")]
        [LocalizedStringLength(Constants.CisMaxLength.CreatedDate, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedDate { get; set; }

        [Display(Name = "CREATE_BY")]
        [LocalizedStringLength(Constants.CisMaxLength.CreatedBy, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedBy { get; set; }

        [Display(Name = "UPDATE_DATE")]
        [LocalizedStringLength(Constants.CisMaxLength.UpdatedDate, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedDate { get; set; }

        [Display(Name = "UPDATE_BY")]
        [LocalizedStringLength(Constants.CisMaxLength.UpdatedBy, ErrorMessageResourceName = "ValErr_StringLength",
        ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedBy { get; set; }

        [Display(Name = "SYSCUSTSUBSCR_ID")]
        [LocalizedStringLength(Constants.CisMaxLength.SysCustSubscrId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SysCustSubscrId { get; set; }

        public string Error { get; set; }
    }
}
