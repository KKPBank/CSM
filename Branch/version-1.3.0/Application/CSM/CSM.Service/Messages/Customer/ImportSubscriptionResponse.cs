using System.ServiceModel;
using CSM.Service.Messages.Common;

namespace CSM.Service.Messages.Customer
{
    [MessageContract]
    public class ImportSubscriptionResponse
    {
        [MessageHeader]
        public Header Header { get; set; }

        [MessageBodyMember]
        public StatusResponse StatusResponse { get; set; }
    }
}
