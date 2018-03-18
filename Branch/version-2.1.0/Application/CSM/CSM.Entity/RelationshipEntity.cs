using System;
using System.Linq;
using CSM.Entity.Common;
using CSM.Common.Resources;
using CSM.Common.Utilities;
using System.Globalization;

namespace CSM.Entity
{
    [Serializable]

    public class RelationshipEntity
    {
        public int RelationshipId { get; set; }
        public string RelationshipName { get; set; }
        public string RelationshipDesc { get; set; }
        public short? Status { get; set; }
        public UserEntity CreateUser { get; set; }

        public string CreateUserDisplay
        {
            get
            {
                if (this.CreateUser != null)
                {
                    string[] names = new string[2] { CreateUser.Firstname.NullSafeTrim(), CreateUser.Lastname.NullSafeTrim() };

                    if (names.Any(x => !string.IsNullOrEmpty(x)))
                    {
                        string positionCode = CreateUser.PositionCode.NullSafeTrim();
                        string fullName = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                        return !string.IsNullOrEmpty(positionCode) ? string.Format(CultureInfo.InvariantCulture, "{0} - {1}", positionCode, fullName) : fullName;
                    }
                }

                return string.Empty;
            }
        }

        public string UpdateUserDisplay
        {
            get
            {
                if (this.UpdateUser != null)
                {
                    string[] names = new string[2] { UpdateUser.Firstname.NullSafeTrim(), UpdateUser.Lastname.NullSafeTrim() };

                    if (names.Any(x => !string.IsNullOrEmpty(x)))
                    {
                        string positionCode = UpdateUser.PositionCode.NullSafeTrim();
                        string fullName = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                        return !string.IsNullOrEmpty(positionCode) ? string.Format(CultureInfo.InvariantCulture, "{0} - {1}", positionCode, fullName) : fullName;
                    }
                }

                return string.Empty;
            }
        }

        public UserEntity UpdateUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? Updatedate { get; set; }

        public string UpdatedateDisplay
        {
            get { return Updatedate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime); }
        }

        public string StatusDisplay
        {
            get { return Constants.ApplicationStatus.GetMessage(this.Status); }
        }
    }

    public class RelationshipSearchFilter : Pager
    {
        public string RelationshipName { get; set; }
        public string RelationshipDesc { get; set; }
        public short? Status { get; set; }
    }
}
