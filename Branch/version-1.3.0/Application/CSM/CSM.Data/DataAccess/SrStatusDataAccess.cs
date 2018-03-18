using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CSM.Entity;
using CSM.Common.Utilities;

namespace CSM.Data.DataAccess
{
    public class SrStatusDataAccess : ISrStatusDataAccess
    {
        private readonly CSMContext _context;

        public SrStatusDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        #region "SrStatus"
        public List<SRStatusEntity> GetSrStatusList()
        {
            List<SRStatusEntity> list =
                _context.TB_C_SR_STATUS.OrderBy(l => l.SR_STATUS_ID).Select(item => new SRStatusEntity
                {
                    SRStatusId = item.SR_STATUS_ID,
                    SRStatusCode = item.SR_STATUS_CODE,
                    SRStatusName = item.SR_STATUS_NAME
                }).ToList();

            return list;
        }
        #endregion

        public List<SRStatusEntity> AutoCompleteSearchSrStatus()
        {
            var query = _context.TB_C_SR_STATUS.AsQueryable();
            query = query.OrderBy(q => q.SR_STATUS_ID);

            return query.Select(item => new SRStatusEntity
            {
                SRStatusId = item.SR_STATUS_ID,
                SRStatusName = item.SR_STATUS_NAME
            }).ToList();
        }

        public SRStatusEntity GetSrStatus(string code)
        {
            var obj = _context.TB_C_SR_STATUS.FirstOrDefault(s => s.SR_STATUS_CODE.ToUpper() == code.ToUpper());
            if (obj == null)
                return null;

            return new SRStatusEntity
            {
                SRStatusId = obj.SR_STATUS_ID,
                SRStatusCode = obj.SR_STATUS_CODE,
                SRStatusName = obj.SR_STATUS_NAME
            };
        }

        #region "Persistence"

        private int Save()
        {
            return _context.SaveChanges();
        }
        
        private void SetEntryCurrentValues(object entityTo, object entityFrom)
        {
            _context.Entry(entityTo).CurrentValues.SetValues(entityFrom);
            // Set state to Modified
            _context.Entry(entityTo).State = EntityState.Modified;
        }

        private void SetEntryStateModified(object entity)
        {
            if (_context.Configuration.AutoDetectChangesEnabled == false)
            {
                // Set state to Modified
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion
    }
}
