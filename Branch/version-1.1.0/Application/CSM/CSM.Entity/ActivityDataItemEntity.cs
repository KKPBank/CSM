using System;

namespace CSM.Entity
{

    [Serializable]
    public class ActivityDataItemEntity
    {
        public int SeqNo { get; set; }
        public string DataLabel { get; set; }
        public string DataValue { get; set; }
    }
}
