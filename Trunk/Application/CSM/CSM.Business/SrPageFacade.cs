using System;
using System.Collections.Generic;
using CSM.Data.DataAccess;
using CSM.Entity;
using System.Linq;

namespace CSM.Business
{
    public class SrPageFacade : ISrPageFacade
    {
        private readonly CSMContext _context;
        private ISrPageDataAccess _srPageDataAccess;

        public SrPageFacade()
        {
            _context = new CSMContext();
        }

        public List<SrPageItemEntity> GetSrPageList()
        {
            _srPageDataAccess = new SrPageDataAccess(_context);
            return _srPageDataAccess.GetSrPageList();
        }

        public List<SrPageItemEntity> GetSrPages(int? roleId = null, int? srStatusId = null)
        {
            return (new SrPageDataAccess(_context)).GetSrPages(roleId, srStatusId).ToList();
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
