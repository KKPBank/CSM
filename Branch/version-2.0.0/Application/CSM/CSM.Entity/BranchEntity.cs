using System;
using System.Collections.Generic;
using CSM.Common.Utilities;
using CSM.Entity.Common;
using Newtonsoft.Json;

namespace CSM.Entity
{
    [Serializable]
    public class BranchEntity
    {
        public int? BranchId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }

        private bool m_IsDelete = false;
        public bool IsDelete
        {
            get { return m_IsDelete; }
            set { m_IsDelete = value; }
        }
    }
    
    [Serializable]
    public class BranchSearchFilter : Pager
    {
        [LocalizedRequired(ErrorMessage = "ValErr_Required")]
        [LocalizedMinLength(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BranchName { get; set; }

        private string m_JsonBranch;
        public string JsonBranch
        {
            get { return m_JsonBranch; }
            set
            {
                m_JsonBranch = value;
                try
                {
                    m_Branches = JsonConvert.DeserializeObject<List<PoolBranchEntity>>(m_JsonBranch);
                }
                catch (Exception)
                {
                    m_Branches = new List<PoolBranchEntity>();
                }
            }
        }

        private List<PoolBranchEntity> m_Branches;
        public List<PoolBranchEntity> Branches
        {
            get { return m_Branches; }
            set { m_Branches = value; }
        }
    }
}
