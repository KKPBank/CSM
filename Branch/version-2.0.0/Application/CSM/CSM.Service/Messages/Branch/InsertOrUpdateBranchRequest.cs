using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CSM.Service.Messages.Branch
{
    [DataContract]
    public class InsertOrUpdateBranchRequest
    {
        [DataMember(IsRequired = true, Order = 1)]
        public WebServiceHeader Header { get; set; }

        [DataMember(Order = 2)]
        public string BranchCode { get; set; }

        [DataMember(Order = 3)]
        public string BranchName { get; set; }

        [DataMember(Order = 4)]
        public string ChannelCode { get; set; }

        [DataMember(Order = 5)]
        public string UpperBranchCode { get; set; }

        [DataMember(Order = 6)]
        public int StartTimeHour { get; set; }

        [DataMember(Order = 7)]
        public int StartTimeMinute { get; set; }

        [DataMember(Order = 8)]
        public int EndTimeHour { get; set; }

        [DataMember(Order = 9)]
        public int EndTimeMinute { get; set; }

        [DataMember(Order = 10)]
        public string HomeNo { get; set; }

        [DataMember(Order = 11)]
        public string Moo { get; set; }

        [DataMember(Order = 12)]
        public string Building { get; set; }

        [DataMember(Order = 13)]
        public string Floor { get; set; }

        [DataMember(Order = 14)]
        public string Soi { get; set; }

        [DataMember(Order = 15)]
        public string Street { get; set; }

        [DataMember(Order = 16)]
        public string Province { get; set; }

        [DataMember(Order = 17)]
        public string Amphur { get; set; }

        [DataMember(Order = 18)]
        public string Tambol { get; set; }

        [DataMember(Order = 19)]
        public string Zipcode { get; set; }

        [DataMember(Order = 20)]
        public short Status { get; set; }

        [DataMember(Order = 21)]
        public string ActionUsername { get; set; }

        [DataMember(Order = 22)]
        public bool Command { get; set; } //true = edit mode, false = add mode
    }

    [DataContract]
    public class WebServiceHeader
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

    [DataContract]
    public class InsertOrUpdateBranchResponse
    {
        [DataMember(Order = 1)]
        public bool IsSuccess { get; set; }

        [DataMember(Order = 2)]
        public bool IsNewBranch { get; set; }

        [DataMember(Order = 3)]
        public string ErrorCode { get; set; }

        [DataMember(Order = 4)]
        public string ErrorMessage { get; set; }
    }

    [DataContract]
    public class UpdateBranchCalendarRequest
    {
        [DataMember(Order = 1)]
        public WebServiceHeader Header { get; set; }

        [DataMember(Order = 2)]
        public DateTime HolidayDate { get; set; }

        [DataMember(Order = 3)]
        public string HolidayDesc { get; set; }

        [DataMember(Order = 4)]
        public List<string> BranchCodeList { get; set; }

        [DataMember(Order = 5)]
        public int UpdateMode { get; set; }

        [DataMember(Order = 6)]
        public string ActionUsername { get; set; }
    }

    [DataContract]
    public class UpdateBranchCalendarResponse
    {
        [DataMember(Order = 1)]
        public bool IsSuccess { get; set; }

        [DataMember(Order = 2)]
        public string ErrorCode { get; set; }

        [DataMember(Order = 3)]
        public string ErrorMessage { get; set; }
    }
}
