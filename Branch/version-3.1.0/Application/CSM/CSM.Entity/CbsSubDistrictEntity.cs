using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CbsSubDistrictEntity
    {
        public string DistrictCode { get; set; }
        public string SubDistrictCode { get; set; }
        public string SubDistrictName { get; set; }
    }
}
