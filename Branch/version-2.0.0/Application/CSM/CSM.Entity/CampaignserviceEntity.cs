using System;

namespace CSM.Entity
{
    [Serializable]
    public class CampaignServiceEntity
    {
        public int? CampaignServiceId { get; set; }
        public string CampaignServiceCode { get; set; }
        public string CampaignServiceName { get; set; }
        public int ProductId { get; set; }
        public bool IsActive { get; set; }
        public string ProductName { get; set; }
        public int ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
