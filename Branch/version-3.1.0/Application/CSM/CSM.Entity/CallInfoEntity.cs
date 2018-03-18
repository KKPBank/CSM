using System;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class CallInfoEntity
    {
        public string CallId { get; set; }
        public string PhoneNo { get; set; }
        public string CardNo { get; set; }
        public string CallType { get; set; }
        public string IVRLang { get; set; }

        public string CardNoDisplay
        {
            get { return Constants.CallType.NCB.Equals(this.CallType) ? this.CardNo : string.Empty; }
        }
    }
}
