using System;

namespace CSM.Entity
{
    [Serializable]
    public class RoleEntity
    {
        #region "Local Declaration"
        private bool m_IsDelete = false;
        #endregion

        public int? RoleId { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public int RoleValue { get; set; }

        public bool IsDelete
        {
            get { return m_IsDelete; }
            set { m_IsDelete = value; }
        }
    }
}
