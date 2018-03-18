using System;
using CSM.Service.Messages.Common;

namespace CSM.Service.Messages.Activity
{
    [Serializable]
    public class CreateActivityLogResponse
    {
        public StatusResponse StatusResponse { get; set; }
    }
}
