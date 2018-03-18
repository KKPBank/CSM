using System;
using System.Collections.Generic;
using CSM.Common.Utilities;
using CSM.Service.Messages.Common;

namespace CSM.Service.Messages.Campaign
{
    public class RecommendCampaignResponse
    {
        public string CitizenId { get; set; }
        public StatusResponse StatusResponse { get; set; }
        public List<CampaignDetail> RecommendCampaignDetails { get; set; }

        public RecommendCampaignResponse()
        {
            this.StatusResponse = new StatusResponse();
        }
    }

    public class CampaignDetail
    {
        public string CampaignCriteria { get; set; }
        public string CampaignDesc { get; set; }
        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string CampaignOffer { get; set; }
        public string CampaignScore { get; set; }
        public string Channel { get; set; }
        public string CitizenIds { get; set; }
        public string DescCust { get; set; }
        public string IsInterested { get; set; }
        public string StrExpireDate { get; set; }
        public string ContractNoRefer { get; set; }
        public string ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }

        public string InterestedDisplay
        {
            get { return Constants.CMTParamConfig.GetInterestedMessage(IsInterested); }
        }

        public DateTime? ExpireDate
        {
            get { return this.StrExpireDate.ParseDateTime("yyyyMMdd"); }
        }
    }
}
