using System;

namespace CSM.Entity
{
    [Serializable]
    public class AddressFormatEntity
    {
        public int AddressFormatId { get; set; }
        public string AddressFormatCode { get; set; }
        public string ShortDescEng { get; set; }
        public string LongDescEng { get; set; }
        public string LongDescTha { get; set; }
    }
}
