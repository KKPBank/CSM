using System.Runtime.Serialization;

namespace CSM.Service.Messages.Common
{
    [DataContract]
    public class Header
    {
        [DataMember(IsRequired = true, Order = 1)]
        public string password { get; set; }

        [DataMember(IsRequired = true, Order = 2)]
        public string reference_no { get; set; }

        [DataMember(IsRequired = true, Order = 3)]
        public string service_name { get; set; }

        [DataMember(IsRequired = true, Order = 4)]
        public string system_code { get; set; }

        [DataMember(IsRequired = true, Order = 5)]
        public string transaction_date { get; set; }

        [DataMember(IsRequired = true, Order = 6)]
        public string user_name { get; set; }

        [DataMember(Order = 7)]
        public string channel_id { get; set; }

        [DataMember(Order = 8)]
        public string command { get; set; }
    }
}
