using System;
using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Business
{
    public interface IQuestionFacade : IDisposable
    {
        IEnumerable<QuestionItemEntity> GetQuestionList(QuestionSearchFilter searchFilter); 
        bool SaveQuestion(QuestionItemEntity questionItemEntity);
        bool CheckQuestionName(QuestionItemEntity questionItemEntity);
        QuestionItemEntity GetQuestionById(int questionId);
    }
}
