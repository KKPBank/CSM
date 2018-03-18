using System;
using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Business
{
    public interface ISlaFacade : IDisposable
    {
        bool ValidateSla(int? slaId, int productId, int? campaignServiceId, int areaId, int? subAreaId,
            int typeId, int channelId, int srStatusId);
        void SaveSLA(SlaItemEntity slaItemEntity);

        IEnumerable<SlaItemEntity> GetSlaList(SlaSearchFilter searchFilter);

        SlaItemEntity GetSlaById(int? slaId);

        bool DeleteSla(int slaId);
    }
}
