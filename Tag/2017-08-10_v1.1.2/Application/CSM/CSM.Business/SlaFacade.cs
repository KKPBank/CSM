using System;
using System.Collections.Generic;
using CSM.Data.DataAccess;
using CSM.Entity;

namespace CSM.Business
{
    public class SlaFacade : ISlaFacade
    {
        private readonly CSMContext _context;
        private ISlaDataAccess _slaDataAccess;

        public SlaFacade()
        {
            _context = new CSMContext();
        }

        #region "SLA"

        public bool ValidateSla(int? slaId, int productId, int? campaignServiceId, int areaId, int? subAreaId, int typeId,
            int channelId, int srStatusId)
        {
            _slaDataAccess = new SlaDataAccess(_context);
            return _slaDataAccess.ValidateSla(slaId, productId, campaignServiceId, areaId, subAreaId, typeId, channelId,
                srStatusId);
        }

        public void SaveSLA(SlaItemEntity slaItemEntity)
        {
            _slaDataAccess = new SlaDataAccess(_context);
            _slaDataAccess.SaveSla(slaItemEntity);
        }

        public IEnumerable<SlaItemEntity> GetSlaList(SlaSearchFilter searchFilter)
        {
            _slaDataAccess = new SlaDataAccess(_context);
            return _slaDataAccess.GetSlaList(searchFilter);
        }

        public SlaItemEntity GetSlaById(int? slaId)
        {
            _slaDataAccess = new SlaDataAccess(_context);
            return _slaDataAccess.GetSlaById(slaId);            
        }

        public bool DeleteSla(int slaId)
        {
            _slaDataAccess = new SlaDataAccess(_context);
            return _slaDataAccess.DeleteSla(slaId);
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
