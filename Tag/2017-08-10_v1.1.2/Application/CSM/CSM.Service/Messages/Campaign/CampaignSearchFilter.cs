using System;

namespace CSM.Service.Messages.Campaign
{
    [Serializable]
    public class CampaignSearchFilter
    {
        public string CampaignId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HasOffered { get; set; }
        public string IsInterested { get; set; }
        public string UpdatedBy { get; set; }
        public string Comments { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string CardNo { get; set; }
        public string ChannelName { get; set; }
        public string AvailableTime { get; set; }
        public string ContractNoRefer { get; set; }
        public string OwnerBranchCode { get; set; }
        public string OwnerLeadCode { get; set; }
    }
}
