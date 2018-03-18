using System;
using CSM.Entity.Common;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class PoolEntity
    {
        #region "Local Declaration"
        private int m_NumOfJob = 0;
        #endregion

        public int? PoolId { get; set; }
        public string PoolName { get; set; }
        public string PoolDesc { get; set; }
        public string Email { get; set; }
        public short? Status { get; set; }
        public string Password { get; set; }

        public int NumOfJob
        {
            get { return m_NumOfJob; }
            set { m_NumOfJob = value; }
        }

        public UserEntity CreateUser { get; set; }
        public UserEntity UpdateUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? Updatedate { get; set; }
    }

    [Serializable]
    public class PoolBranchEntity : BranchEntity
    {
        public int? PoolId { get; set; }
    }

    [Serializable]
    public class PoolSearchFilter : Pager
    {
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PoolName { get; set; }
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PoolDesc { get; set; }
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Email { get; set; }
        public int? BranchId { get; set; }
    }
}
