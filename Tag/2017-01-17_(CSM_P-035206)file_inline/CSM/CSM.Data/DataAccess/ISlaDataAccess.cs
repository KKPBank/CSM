using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Data.DataAccess
{
    public interface ISlaDataAccess
    {
        bool ValidateSla(int? slaId, int productId, int? campaignServiceId, int areaId, int? subAreaId, int typeId,
            int channelId, int srStatusId);
        void SaveSla(SlaItemEntity slaItemEntity);

        IEnumerable<SlaItemEntity> GetSlaList(SlaSearchFilter searchFilter);

        SlaItemEntity GetSlaById(int? slaId);

        bool DeleteSla(int slaId);
    }
}
