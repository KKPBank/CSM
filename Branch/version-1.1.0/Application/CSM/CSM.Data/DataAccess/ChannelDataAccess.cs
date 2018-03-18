using CSM.Common.Utilities;
using CSM.Entity;
using log4net;
using System.Collections.Generic;
using System.Linq;

namespace CSM.Data.DataAccess
{
    public class ChannelDataAccess : IChannelDataAccess
    {
        private readonly CSMContext _context;
        //private static readonly ILog Logger = LogManager.GetLogger(typeof(ChannelDataAccess));

        public ChannelDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        public List<ChannelEntity> AutoCompleteSearchChannel()
        {
            var query = _context.TB_R_CHANNEL.AsQueryable();

            query = query.Where(q => (q.STATUS ?? 1) == 1);

            query = query.OrderBy(q => q.CHANNEL_NAME);

            return query.Select(item => new ChannelEntity
            {
                ChannelId = item.CHANNEL_ID,
                Name = item.CHANNEL_NAME,
            }).ToList();
        }

        public int? GetChannelIdByChannelCode(string channelCode)
        {
            var item = _context.TB_R_CHANNEL.Where(x => x.CHANNEL_CODE.Trim() == channelCode.Trim()).Select(x => new { x.CHANNEL_ID, x.CHANNEL_CODE }).FirstOrDefault();
            if (item == null)
                return null;
            else
                return item.CHANNEL_ID;
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
            _context.Entry(entityTo).State = System.Data.Entity.EntityState.Modified;
        }

        private void SetEntryStateModified(object entity)
        {
            if (_context.Configuration.AutoDetectChangesEnabled == false)
            {
                // Set state to Modified
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
        }

        #endregion
    }
}
