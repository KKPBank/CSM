using System;
using System.Collections.Generic;
using CSM.Data.DataAccess;
using CSM.Entity;

namespace CSM.Business
{
    public class SrStatusFacade : ISrStatusFacade
    {
        private readonly CSMContext _context;
        private ISrStatusDataAccess _srStatusDataAccess;

        public SrStatusFacade()
        {
            _context = new CSMContext();
        }

        #region "SrStatus"
        
        public List<SRStatusEntity> GetSrStatusList()
        {
            _srStatusDataAccess = new SrStatusDataAccess(_context);
            return _srStatusDataAccess.GetSrStatusList();
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
