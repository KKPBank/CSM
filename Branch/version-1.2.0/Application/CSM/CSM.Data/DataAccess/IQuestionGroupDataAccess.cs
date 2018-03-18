using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Data.DataAccess
{
    public interface IQuestionGroupDataAccess
    {
        IEnumerable<QuestionGroupItemEntity> GetQuestionGroupList(QuestionGroupSearchFilter searchFilter);
        bool SaveQuestionGroup(QuestionGroupItemEntity questionGroupItemEntity);
        QuestionGroupItemEntity GetQuestionGroupById(int questionGroupId);
        List<QuestionGroupDuplicateEntity> GetQuestionGroupDuplicates(List<int> productIds, string questionGroupName, int? questionGroupId);
        IEnumerable<QuestionGroupQuestionItemEntity> GetQuestionListById(QuestionGroupInQuestionSearchFilter searchFilter);
        List<QuestionItemEntity> SearchQuestionGroupQuestion(int offset, int limit, string questionName ,string questionIdList, ref int totalCount);
        bool SaveQuestionGroup(QuestionGroupItemEntity questionGroupItemEntity, string idQuestions);
        List<QuestionGroupProductEntity> GetProductList(string prefix, ref int totalCount);

        List<QuestionItemEntity> GetQuestionList(QuestionSelectSearchFilter searchFilter);

        List<QuestionItemEntity> GetQuestionList(int questionGroupId);
    }
}
