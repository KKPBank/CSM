using System;

namespace CSM.Entity
{
    [Serializable]
    public class SRValidateResult
    {
        public string ErrorCode { get; set; }
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public int ContactId { get; set; }
        public int ContactRelationshipId { get; set; }
        public int ProductGroupId { get; set; }
        public int ProductId { get; set; }
        public int CampaignServiceId { get; set; }
        public int AreaId { get; set; }
        public int SubAreaId { get; set; }
        public int TypeId { get; set; }
        public int ChannelId { get; set; }
        public int MediaSourceId { get; set; }
        public int MapProductId { get; set; }
        public bool IsVerify { get; set; }
        public int SrPageId { get; set; }
        public int AfsAssetId { get; set; }
        public string AfsAssetNo { get; set; }
        public string AfsAssetDesc { get; set; }
        public int NcbMarketingUserId { get; set; }
        public int OwnerBranchId { get; set; }
        public int OwnerUserId { get; set; }
        public int DelegateBranchId { get; set; }
        public int DelegateUserId { get; set; }
        public int CreatorBranchId { get; set; }
        public int CreatorUserId { get; set; }
    }
}
