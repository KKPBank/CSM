using System;

namespace CSM.Entity
{
    [Serializable]
    public class DocumentTypeEntity
    {
        public int? DocTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public short? Status { get; set; }

        private bool m_IsChecked = false;
        public bool IsChecked
        {
            get { return m_IsChecked; }
            set { m_IsChecked = value; }
        }
        
        private bool m_IsDelete = false;
        public bool IsDelete
        {
            get { return m_IsDelete; }
            set { m_IsDelete = value; }
        }
    }
}
