using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Data.DataAccess
{
    public interface IAreaDataAccess
    {
        IEnumerable<AreaItemEntity> GetAreaList(AreaSearchFilter searchFilter);
        List<AreaSubAreaItemEntity> GetSubAreaListById(int offset, int limit, int areaId, ref int totalCount);
        AreaItemEntity GetArea(int areaId);
        bool SaveArea(AreaItemEntity areaItemEntity, string idSubAreas);
        bool ValidateAreaName(int? areaId, string areaName);
        decimal? GetNextAreaCode();
    }
}
