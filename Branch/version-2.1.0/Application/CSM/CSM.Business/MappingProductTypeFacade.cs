using System;
using System.Collections.Generic;
using CSM.Data.DataAccess;
using CSM.Entity;
using System.Linq;

namespace CSM.Business
{
    public class MappingProductTypeFacade : IMappingProductTypeFacade
    {
        private readonly CSMContext _context;
        private IMappingProductTypeDataAccess _mappingProductTypeDataAccess;

        public MappingProductTypeFacade()
        {
            _context = new CSMContext();
        }

        #region "MappingProduct"

        public IEnumerable<QuestionGroupTableItemEntity> GetQuestionGroupList(QuestionSelectSearchFilter searchFilter)
        {
            _mappingProductTypeDataAccess = new MappingProductTypeDataAccess(_context);
            return _mappingProductTypeDataAccess.GetQuestionGroupList(searchFilter);
        }

        public bool SaveMapProduct(MappingProductTypeItemEntity mappingItemEntity, List<ProductQuestionGroupItemEntity> productQuestionEntityList)
        {
            _mappingProductTypeDataAccess = new MappingProductTypeDataAccess(_context);
            return _mappingProductTypeDataAccess.SaveMapProduct(mappingItemEntity, productQuestionEntityList);
        }

        public IEnumerable<MappingProductTypeItemEntity> GetMappingList(MappingProductSearchFilter searchFilter)
        {
            _mappingProductTypeDataAccess = new MappingProductTypeDataAccess(_context);
            return _mappingProductTypeDataAccess.GetMappingList(searchFilter);
        }

        public MappingProductTypeItemEntity GetMappingById(int mapProductId)
        {
            _mappingProductTypeDataAccess = new MappingProductTypeDataAccess(_context);
            return _mappingProductTypeDataAccess.GetMappingById(mapProductId);
        }

        public IEnumerable<QuestionGroupEditTableItemEntity> GetQuestionGroupById(QuestionGroupEditSearchFilter searchFilter)
        {
            _mappingProductTypeDataAccess = new MappingProductTypeDataAccess(_context);
            return _mappingProductTypeDataAccess.GetQuestionGroupById(searchFilter);
        }

		public IEnumerable<QuestionGroupEditTableItemEntity> GetLoadQuestionGroupById(int mapProductId)
        {
            _mappingProductTypeDataAccess = new MappingProductTypeDataAccess(_context);
            return _mappingProductTypeDataAccess.GetLoadQuestionGroupById(mapProductId);
        }

        public bool CheckDuplicateMappProduct(MappingProductTypeItemEntity mappingItemEntity)
        {
            _mappingProductTypeDataAccess = new MappingProductTypeDataAccess(_context);
            return _mappingProductTypeDataAccess.CheckDuplicateMappProduct(mappingItemEntity);
        }

        public List<OTPTemplateEntity> GetOTPTemplate(int? id = null, short? status = null)
        {
            return (new MappingProductTypeDataAccess(_context)).GetOTPTemplate(id, status).ToList();
        }

        #endregion

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
