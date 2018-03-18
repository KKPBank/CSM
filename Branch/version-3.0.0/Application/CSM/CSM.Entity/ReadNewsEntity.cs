using System;
using CSM.Entity.Common;
using CSM.Common.Resources;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class ReadNewsEntity
    {
        public int? NewsId { get; set; }
        public int? CreateUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
