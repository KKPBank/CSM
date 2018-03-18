using System;

namespace CSM.Entity
{
    [Serializable]
    public class ProductEntity
    {
        public int? ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public int ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public int? IsDelete { get; set; }
        public bool IsActive { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
