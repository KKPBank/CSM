using System;
using System.Collections.Generic;
using CSM.Data.DataAccess;
using CSM.Entity;

namespace CSM.Business
{
    public class AreaFacade : IAreaFacade
    {
        private readonly CSMContext _context;
        private IAreaDataAccess _areaDataAccess;

        public AreaFacade() 
        {
            _context = new CSMContext();
        }

        public IEnumerable<AreaItemEntity> GetAreaList(AreaSearchFilter searchFilter)
        {
            _areaDataAccess = new AreaDataAccess(_context);
            return _areaDataAccess.GetAreaList(searchFilter);
        }

        public List<AreaSubAreaItemEntity> GetSubAreaListById(int offset, int limit, int areaId, ref int totalCount)
        {
            _areaDataAccess = new AreaDataAccess(_context);
            return _areaDataAccess.GetSubAreaListById(offset, limit, areaId, ref totalCount);
        }

        public AreaItemEntity GetArea(int areaId)
        {
            _areaDataAccess = new AreaDataAccess(_context);
            return _areaDataAccess.GetArea(areaId);
        }

        public bool SaveArea(AreaItemEntity areaItemEntity, string idSubAreas)
        {
            _areaDataAccess = new AreaDataAccess(_context);
            return _areaDataAccess.SaveArea(areaItemEntity, idSubAreas);
        }

        public bool ValidateAreaName(int? areaId, string areaName)
        {
            _areaDataAccess = new AreaDataAccess(_context);
            return _areaDataAccess.ValidateAreaName(areaId, areaName);
        }

        public decimal? GetNextAreaCode()
        {
            _areaDataAccess = new AreaDataAccess(_context);
            return _areaDataAccess.GetNextAreaCode();
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
