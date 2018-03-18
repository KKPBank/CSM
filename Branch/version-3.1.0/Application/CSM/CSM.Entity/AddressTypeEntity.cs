using System;

namespace CSM.Entity
{
    [Serializable]
    public class AddressTypeEntity
    {
        public int AddressTypeId { get; set; }
        public string AddressTypeCode { get; set; }
        public string ShortDescEng { get; set; }
        public string LongDescEng { get; set; }
        public string LongDescTha { get; set; }
        public string AllowMultiple { get; set; }
        public string CategoryCode { get; set; }
        public string MainAddress { get; set; }
        public string CisCode { get; set; }
    }
}
