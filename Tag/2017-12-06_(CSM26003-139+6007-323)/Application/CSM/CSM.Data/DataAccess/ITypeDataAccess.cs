using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Data.DataAccess
{
    public interface ITypeDataAccess
    {
        IEnumerable<TypeItemEntity> GetTypeList(TypeSearchFilter searchFilter); 
        bool SaveType(TypeItemEntity typeItemEntity);
        TypeItemEntity GetTypeById(int typeId);
        bool CheckTypeName(TypeItemEntity typeItemEntity);
        decimal? GetNextTypeCode();
    }
}
