using System;

namespace CSM.Entity
{
    [Serializable]
    public class AreaCodeEntity
    {
        public int AreaCodeId { get; set; }
        public string AreaCode { get; set; }
        public string ShortDescEng { get; set; }
        public string LongDescEng { get; set; }
        public string LongDescTha { get; set; }
    }
}
