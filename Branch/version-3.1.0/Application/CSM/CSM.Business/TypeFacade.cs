using System;
using System.Collections.Generic;
using CSM.Data.DataAccess;
using CSM.Entity;

namespace CSM.Business
{
    public class TypeFacade : ITypeFacade
    {
        private readonly CSMContext _context;
        private ITypeDataAccess _typeDataAccess;

        public TypeFacade()
        {
            _context = new CSMContext();
        }

        public bool SaveType(TypeItemEntity typeItemEntity)
        {
            _typeDataAccess = new TypeDataAccess(_context);
            return _typeDataAccess.SaveType(typeItemEntity);
        }

        public bool CheckTypeName(TypeItemEntity typeItemEntity)
        {
            _typeDataAccess = new TypeDataAccess(_context);
            return _typeDataAccess.CheckTypeName(typeItemEntity);
        }

        public TypeItemEntity GetTypeById(int typeId)
        {
            _typeDataAccess = new TypeDataAccess(_context);
            return _typeDataAccess.GetTypeById(typeId);
        }

        public IEnumerable<TypeItemEntity> GetTypeList(TypeSearchFilter searchFilter)
        {
            _typeDataAccess = new TypeDataAccess(_context);
            return _typeDataAccess.GetTypeList(searchFilter);
        }

        public decimal? GetNextTypeCode()
        {
            _typeDataAccess = new TypeDataAccess(_context);
            return _typeDataAccess.GetNextTypeCode();
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
