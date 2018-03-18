using System;
using System.Linq;
using CSM.Common.Utilities;
using System.Globalization;

namespace CSM.Entity
{
    [Serializable]
    public class UserEntity
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public short? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? Modifiedate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public int? BranchId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Email { get; set; }
        public string EmployeeCode { get; set; }
        public string RoleCode { get; set; }
        public string PositionCode { get; set; }
        public int RoleValue { get; set; }
        public bool IsGroup { get; set; }
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }

        public string FullName
        {
            get
            {
                string[] names = new string[2] { this.Firstname.NullSafeTrim(), this.Lastname.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    string positionCode = this.PositionCode.NullSafeTrim();
                    string fullName = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                    return !string.IsNullOrEmpty(positionCode) ? string.Format(CultureInfo.InvariantCulture, "{0} - {1}", positionCode, fullName) : fullName;
                }

                return string.Empty;
            }
        }

        public int? JobOnHand { get; set; }
    }
}
