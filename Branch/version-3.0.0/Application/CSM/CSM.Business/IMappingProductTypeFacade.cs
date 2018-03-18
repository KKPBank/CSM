using System;
using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Business
{
    public interface IMappingProductTypeFacade : IDisposable
    {
        IEnumerable<QuestionGroupTableItemEntity> GetQuestionGroupList(QuestionSelectSearchFilter searchFilter);
        bool SaveMapProduct(MappingProductTypeItemEntity mappingItemEntity, List<ProductQuestionGroupItemEntity> productQuestionEntityList);
        IEnumerable<MappingProductTypeItemEntity> GetMappingList(MappingProductSearchFilter searchFilter);
        MappingProductTypeItemEntity GetMappingById(int mapProductId);

        IEnumerable<QuestionGroupEditTableItemEntity> GetQuestionGroupById(QuestionGroupEditSearchFilter searchFilter);
		
		IEnumerable<QuestionGroupEditTableItemEntity> GetLoadQuestionGroupById(int mapProductId);

        bool CheckDuplicateMappProduct(MappingProductTypeItemEntity mappingItemEntity);
    }
}
