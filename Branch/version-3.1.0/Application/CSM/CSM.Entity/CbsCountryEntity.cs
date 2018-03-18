using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CbsCountryEntity
    { 
        public string CountryCode { get; set; }
        public string CountryNameTH { get; set; }
        public string CountryNameEN { get; set; }
    }
}
