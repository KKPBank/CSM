using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Data.DataAccess
{
    public interface IProductGroupDataAccess
    {
        List<ProductGroupEntity> AutoCompleteSearchProductGroup(string keyword, int limit, int? productId, bool? isAllStatus);
    }
}
