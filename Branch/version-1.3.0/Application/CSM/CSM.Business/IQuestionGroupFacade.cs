using System;
using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Business
{
    public interface IQuestionGroupFacade : IDisposable
    {
        IEnumerable<QuestionGroupItemEntity> GetQuestionGroupList(QuestionGroupSearchFilter searchFilter);
        bool SaveQuestionGroup(QuestionGroupItemEntity questionGroupItemEntity);

        List<QuestionGroupDuplicateEntity> GetQuestionGroupDuplicates(List<int> productId, string questionGroupName, int? questionGroupId);

        QuestionGroupItemEntity GetQuestionGroupById(int questionGroupId);
        IEnumerable<QuestionGroupQuestionItemEntity> GetQuestionListById(QuestionGroupInQuestionSearchFilter searchFilter);
        List<QuestionItemEntity> SearchQuestionGroupQuestion(int offset, int limit, string questionName, string questionIdList, ref int totalCount);

        bool SaveQuestionGroup(QuestionGroupItemEntity questionGroupItemEntity, string idQuestions);

        List<QuestionGroupProductEntity> GetProductList(string prefix, ref int totalCount);
        List<QuestionItemEntity> GetQuestionList(QuestionSelectSearchFilter searchFIlter);
        List<QuestionItemEntity> GetQuestionList(int questionId);
    }
}
