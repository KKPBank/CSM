using System;
using System.Collections.Generic;
using CSM.Data.DataAccess;
using CSM.Entity;

namespace CSM.Business
{
    public class QuestionFacade : IQuestionFacade
    {
        private readonly CSMContext _context;
        private IQuestionDataAccess _questionDataAccess;

        public QuestionFacade()
        {
            _context = new CSMContext();
        }

        public IEnumerable<QuestionItemEntity> GetQuestionList(QuestionSearchFilter searchFilter)
        {
            _questionDataAccess = new QuestionDataAccess(_context);
            return _questionDataAccess.GetQuestionList(searchFilter);
        }

        public bool SaveQuestion(QuestionItemEntity questionItemEntity)
        {
            _questionDataAccess = new QuestionDataAccess(_context);
            return _questionDataAccess.SaveQuestion(questionItemEntity);
        }

        public bool CheckQuestionName(QuestionItemEntity questionItemEntity)
        {
            _questionDataAccess = new QuestionDataAccess(_context);
            return _questionDataAccess.CheckQuestionName(questionItemEntity);
        }

        public QuestionItemEntity GetQuestionById(int questionId)
        {
            _questionDataAccess = new QuestionDataAccess(_context);
            return _questionDataAccess.GetQuestionById(questionId);
        }

        #region "IDisposable"

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (_context != null) { _context.Dispose(); }
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
