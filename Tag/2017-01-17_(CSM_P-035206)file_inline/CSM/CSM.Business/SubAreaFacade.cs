using System;
using System.Collections.Generic;
using CSM.Data.DataAccess;
using CSM.Entity;

namespace CSM.Business
{
    public class SubAreaFacade : ISubAreaFacade
    {
        private readonly CSMContext _context;
        private ISubAreaDataAccess _subAreaDataAccess;

        public SubAreaFacade()
        {
            _context = new CSMContext();
        }
        public IEnumerable<SubAreaItemEntity> GetSelectSubAreaList(SelectSubAreaSearchFilter searchFilter)
        {
            _subAreaDataAccess = new SubAreaDataAccess(_context);
            return _subAreaDataAccess.GetSelectSubAreaList(searchFilter);
        }

        public bool ValidateSubArea(string subAreaName, int? subAreaId)
        {
            _subAreaDataAccess = new SubAreaDataAccess(_context);
            return _subAreaDataAccess.ValidateSubArea(subAreaName, subAreaId);
        }

        public SubAreaItemEntity SaveSubArea(SubAreaItemEntity subAreaItemEntity)
        {
            _subAreaDataAccess = new SubAreaDataAccess(_context);
            return _subAreaDataAccess.SaveSubArea(subAreaItemEntity);
        }

        public SubAreaItemEntity GetSubAreaItem(int id)
        {
            _subAreaDataAccess = new SubAreaDataAccess(_context);
            return _subAreaDataAccess.GetSubAreaItem(id);
        }

        public IEnumerable<SubAreaItemEntity> GetSubAreaListById(SelectSubAreaSearchFilter searchFilter)
        {
            _subAreaDataAccess = new SubAreaDataAccess(_context);
            return _subAreaDataAccess.GetSubAreaListById(searchFilter);
        }

        public decimal? GetNextSubAreaCode()
        {
            _subAreaDataAccess = new SubAreaDataAccess(_context);
            return _subAreaDataAccess.GetNextSubAreaCode();
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
