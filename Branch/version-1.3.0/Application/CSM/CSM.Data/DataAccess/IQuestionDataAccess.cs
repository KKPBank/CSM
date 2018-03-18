using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Data.DataAccess
{
    public interface IQuestionDataAccess
    {
        IEnumerable<QuestionItemEntity> GetQuestionList(QuestionSearchFilter searchFilter);
        bool SaveQuestion(QuestionItemEntity questionItemEntity);
        QuestionItemEntity GetQuestionById(int questionId);

        bool CheckQuestionName(QuestionItemEntity questionItemEntity);
    }
}
