using System;
using System.Collections.Generic;
using CSM.Data.DataAccess;
using CSM.Entity;

namespace CSM.Business
{
    public class QuestionGroupFacade : IQuestionGroupFacade
    {
        private readonly CSMContext _context;
        private IQuestionGroupDataAccess _questionGroupDataAccess;

        public QuestionGroupFacade()
        {
            _context = new CSMContext();
        }

        public IEnumerable<QuestionGroupItemEntity> GetQuestionGroupList(QuestionGroupSearchFilter searchFilter)
        {
            _questionGroupDataAccess = new QuestionGroupDataAccess(_context);
            return _questionGroupDataAccess.GetQuestionGroupList(searchFilter);
        }

        public bool SaveQuestionGroup(QuestionGroupItemEntity questionGroupItemEntity)
        {
            _questionGroupDataAccess = new QuestionGroupDataAccess(_context);
            return _questionGroupDataAccess.SaveQuestionGroup(questionGroupItemEntity);
        }

        public List<QuestionGroupDuplicateEntity> GetQuestionGroupDuplicates(List<int> productIds, string questionGroupName, int? questionGroupId)
        {
            _questionGroupDataAccess = new QuestionGroupDataAccess(_context);
            return _questionGroupDataAccess.GetQuestionGroupDuplicates(productIds, questionGroupName, questionGroupId);
        }

        public QuestionGroupItemEntity GetQuestionGroupById(int questionGroupId)
        {
            _questionGroupDataAccess = new QuestionGroupDataAccess(_context);
            return _questionGroupDataAccess.GetQuestionGroupById(questionGroupId);
        }

        public IEnumerable<QuestionGroupQuestionItemEntity> GetQuestionListById(QuestionGroupInQuestionSearchFilter searchFilter)
        {
            _questionGroupDataAccess = new QuestionGroupDataAccess(_context);
            return _questionGroupDataAccess.GetQuestionListById(searchFilter);
        }
        public List<QuestionItemEntity> SearchQuestionGroupQuestion(int offset, int limit, string questionName, string questionIdList, ref int totalCount)
        {
            _questionGroupDataAccess = new QuestionGroupDataAccess(_context);
            return _questionGroupDataAccess.SearchQuestionGroupQuestion(offset, limit, questionName, questionIdList, ref totalCount);
        }

        public bool SaveQuestionGroup(QuestionGroupItemEntity questionGroupItemEntity, string idQuestions)
        {
            _questionGroupDataAccess = new QuestionGroupDataAccess(_context);
            return _questionGroupDataAccess.SaveQuestionGroup(questionGroupItemEntity, idQuestions);            
        }

        public List<QuestionGroupProductEntity> GetProductList(string prefix, ref int totalCount)
        {
            _questionGroupDataAccess = new QuestionGroupDataAccess(_context);
            return _questionGroupDataAccess.GetProductList(prefix, ref totalCount);              
        }

        public List<QuestionItemEntity> GetQuestionList(QuestionSelectSearchFilter searchFilter)
        {
            _questionGroupDataAccess = new QuestionGroupDataAccess(_context);
            return _questionGroupDataAccess.GetQuestionList(searchFilter);
        }

        public List<QuestionItemEntity> GetQuestionList(int questionGroupId)
        {
            _questionGroupDataAccess = new QuestionGroupDataAccess(_context);
            return _questionGroupDataAccess.GetQuestionList(questionGroupId);
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
