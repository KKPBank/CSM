﻿using System.Runtime.Serialization;
using CSM.Service.Messages.Common;

namespace CSM.Service.Messages.Sr
{
    [DataContract]
    public class HeaderSrResponse
    {
        [DataMember(Order = 1)]
        public Header Header { get; set; }

        [DataMember(Order = 2)]
        public StatusResponse StatusResponse { get; set; }
    }
}
