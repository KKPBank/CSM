using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CbsProvinceEntity
    {
        public string ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
    }
}
