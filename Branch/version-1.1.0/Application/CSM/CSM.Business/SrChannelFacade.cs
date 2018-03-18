using System;
using System.Collections.Generic;
using CSM.Data.DataAccess;
using CSM.Entity;

namespace CSM.Business
{
    public class SrChannelFacade : ISrChannelFacade
    {
        private readonly CSMContext _context;
        private ISrChannelDataAccess _srChannelDataAccess;

        public SrChannelFacade()
        {
            _context = new CSMContext();
        }

        #region "SrChannel"

        public List<SrChannelEntity> GetSrChannelList()
        {
            _srChannelDataAccess = new SrChannelDataAccess(_context);
            return _srChannelDataAccess.GetSrChannelList();
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
