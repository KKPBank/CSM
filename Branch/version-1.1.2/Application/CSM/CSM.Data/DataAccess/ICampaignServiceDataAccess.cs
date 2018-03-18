using CSM.Entity;
using System.Collections.Generic;

namespace CSM.Data.DataAccess
{
    public interface ICampaignServiceDataAccess
    {
        List<CampaignServiceEntity> AutoCompleteSearchCampaignService(string keyword, int? productGroupId, int? productId, int limit, bool? isAllStatus);
        List<CampaignServiceEntity> AutoCompleteSearchCampaignServiceOnMapping(string keyword, int? areaId, int? subAreaId, int? typeId);
    }
}
