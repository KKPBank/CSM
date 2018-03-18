using System;
using CSM.Common.Utilities;
using System.Globalization;

namespace CSM.Entity
{
    [Serializable]
    public class MessageEntity
    {
        public string ErrorSystem { get; set; }
        public string ErrorService { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDesc { get; set; }

        public string MessageKey
        {
            get { return ApplicationHelpers.GetMessageKey(this.ErrorSystem, this.ErrorService, this.ErrorCode); }
        }

        public string MessageValue
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ErrorCode, ErrorDesc); }
        }
    }
}
