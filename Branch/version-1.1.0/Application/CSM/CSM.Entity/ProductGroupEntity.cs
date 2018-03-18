using System;

namespace CSM.Entity
{
    [Serializable]
    public class ProductGroupEntity
    {
        public int? ProductGroupId { get; set; }
        public string ProductGroupCode { get; set; }
        public string ProductGroupName { get; set; }
        public bool IsActive { get; set; }
        public int? IsDelete { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
