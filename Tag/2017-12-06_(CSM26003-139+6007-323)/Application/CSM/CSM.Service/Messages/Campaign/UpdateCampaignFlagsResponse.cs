using CSM.Service.Messages.Common;

namespace CSM.Service.Messages.Campaign
{
    public class UpdateCampaignFlagsResponse
    {
        public StatusResponse StatusResponse { get; set; }
        public string CitizenId { get; set; }
        public string UpdateStatus { get; set; }

        public UpdateCampaignFlagsResponse() 
        {
            this.StatusResponse = new StatusResponse();
        }
    }
}
