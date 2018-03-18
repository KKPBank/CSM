using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CbsDistrictEntity
    {
        public string ProvinceCode { get; set; }
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
    }
}
