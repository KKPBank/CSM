using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CSM.Entity;
using CSM.Common.Utilities;

namespace CSM.Data.DataAccess
{
    public class SrChannelDataAccess : ISrChannelDataAccess
    {
        private readonly CSMContext _context;

        public SrChannelDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        #region "SrChannel"
        public List<SrChannelEntity> GetSrChannelList()
        {
            List<SrChannelEntity> list = _context.TB_R_CHANNEL.OrderBy(l => l.CHANNEL_NAME).Select(item => new SrChannelEntity
            {
                ChannelId = item.CHANNEL_ID,
                ChannelName = item.CHANNEL_NAME,
                Status = item.STATUS
            }).ToList();

            return list;
        }

        #endregion

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
