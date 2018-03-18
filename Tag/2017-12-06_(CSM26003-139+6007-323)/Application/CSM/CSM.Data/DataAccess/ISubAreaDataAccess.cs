using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Data.DataAccess
{
    public interface ISubAreaDataAccess
    {
        IEnumerable<SubAreaItemEntity> GetSelectSubAreaList(SelectSubAreaSearchFilter searchFilter);
        bool ValidateSubArea(string subAreaName, int? subAreaId);
        SubAreaItemEntity SaveSubArea(SubAreaItemEntity subAreaItemEntity);
//        SubAreaItemEntity GetLastedSubArea();
        SubAreaItemEntity GetSubAreaItem(int id);
        IEnumerable<SubAreaItemEntity> GetSubAreaListById(SelectSubAreaSearchFilter searchFilter);
        decimal? GetNextSubAreaCode();
    }
}
