using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Entity
{
    public class InsertOrUpdateUserRequest
    {
        public int ActionType { get; set; }
        public string WindowsUsername { get; set; }
        public string EmployeeCodeNew { get; set; }
        public string EmployeeCodeOld { get; set; }
        public string MarketingCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }
        public string Email { get; set; }
        public string PositionCode { get; set; }
        public string RoleSale { get; set; }
        public string MarketingTeam { get; set; }
        public string BranchCode { get; set; }
        public string SupervisorEmployeeCode { get; set; }
        public string Line { get; set; }
        public string Rank { get; set; }
        public string EmployeeType { get; set; }
        public string CompanyName { get; set; }
        public string TelesaleTeam { get; set; }
        public string RoleCode { get; set; }
        public bool IsGroup { get; set; }
        public int Status { get; set; }
        public string ActionUsername { get; set; }
        public bool MarketingFlag { get; set; }
    }

    public class InsertOrUpdateUserResponse
    {
        public bool IsSuccess { get; set; }
        public bool IsNewUser { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
