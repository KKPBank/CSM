using System.Runtime.Serialization;
using CSM.Service.Messages.Branch;

namespace CSM.Service.Messages.User
{
    [DataContract]
    public class InsertOrUpdateUserRequest
    {
        [DataMember(Order = 1)]
        public WebServiceHeader Header { get; set; }

        [DataMember(Order = 2)]
        public int ActionType { get; set; }

        [DataMember(Order = 3)]
        public string WindowsUsername { get; set; }

        [DataMember(Order = 4)]
        public string EmployeeCodeNew { get; set; }

        [DataMember(Order = 5)]
        public string EmployeeCodeOld { get; set; }

        [DataMember(Order = 6)]
        public string MarketingCode { get; set; }

        [DataMember(Order = 7)]
        public string FirstName { get; set; }

        [DataMember(Order = 8)]
        public string LastName { get; set; }

        [DataMember(Order = 9)]
        public string Phone1 { get; set; }

        [DataMember(Order = 10)]
        public string Phone2 { get; set; }

        [DataMember(Order = 11)]
        public string Phone3 { get; set; }

        [DataMember(Order = 12)]
        public string Email { get; set; }

        [DataMember(Order = 13)]
        public string PositionCode { get; set; }

        [DataMember(Order = 14)]
        public string RoleSale { get; set; }

        [DataMember(Order = 15)]
        public string MarketingTeam { get; set; }

        [DataMember(Order = 16)]
        public string BranchCode { get; set; }

        [DataMember(Order = 17)]
        public string SupervisorEmployeeCode { get; set; }

        [DataMember(Order = 18)]
        public string Line { get; set; }

        [DataMember(Order = 19)]
        public string Rank { get; set; }

        [DataMember(Order = 20)]
        public string EmployeeType { get; set; }

        [DataMember(Order = 21)]
        public string CompanyName { get; set; }

        [DataMember(Order = 22)]
        public string TelesaleTeam { get; set; }

        [DataMember(Order = 23)]
        public string RoleCode { get; set; }

        [DataMember(Order = 24)]
        public bool IsGroup { get; set; }

        [DataMember(Order = 25)]
        public int Status { get; set; }

        [DataMember(Order = 26)]
        public string ActionUsername { get; set; }

        [DataMember(Order = 27)]
        public bool MarketingFlag { get; set; }
    }

    [DataContract]
    public class InsertOrUpdateUserResponse
    {
        [DataMember(Order = 1)]
        public bool IsSuccess { get; set; }

        [DataMember(Order = 2)]
        public bool IsNewUser { get; set; }

        [DataMember(Order = 3)]
        public string ErrorCode { get; set; }

        [DataMember(Order = 4)]
        public string ErrorMessage { get; set; }
    }
}
