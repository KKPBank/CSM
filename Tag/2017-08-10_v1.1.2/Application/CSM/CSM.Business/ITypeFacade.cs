using System;
using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Business
{
    public interface ITypeFacade : IDisposable
    {
        bool SaveType(TypeItemEntity typeItemEntity);
        bool CheckTypeName(TypeItemEntity typeItemEntity);
        TypeItemEntity GetTypeById(int typeId);
        IEnumerable<TypeItemEntity> GetTypeList(TypeSearchFilter searchFilter);
    }
}
