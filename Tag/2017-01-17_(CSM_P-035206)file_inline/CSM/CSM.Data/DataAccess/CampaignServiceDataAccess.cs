using log4net;
using CSM.Entity;
using System.Collections.Generic;
using System.Linq;
using CSM.Common.Utilities;

namespace CSM.Data.DataAccess
{
    public class CampaignServiceDataAccess : ICampaignServiceDataAccess
    {
        private readonly CSMContext _context;
        //private static readonly ILog Logger = LogManager.GetLogger(typeof(CampaignServiceDataAccess));
        public CampaignServiceDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        public List<CampaignServiceEntity> AutoCompleteSearchCampaignService(string keyword, int? productGroupId, int? productId, int limit, bool? isAllStatus)
        {
            var query = _context.TB_R_CAMPAIGNSERVICE.AsQueryable();

            var isAll = (isAllStatus ?? false);
            query = query.Where(q => isAll || (!isAll && q.CAMPAIGNSERVICE_IS_ACTIVE));

            if (productGroupId.HasValue)
            {
                query = query.Where(q => q.TB_R_PRODUCT.PRODUCTGROUP_ID == productGroupId.Value);
            }

            if (productId.HasValue)
            {
                query = query.Where(q => q.PRODUCT_ID == productId.Value);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q => q.CAMPAIGNSERVICE_NAME.Contains(keyword));
            }

            query = query.OrderBy(q => q.CAMPAIGNSERVICE_NAME);

            return query.Take(limit).Select(item => new CampaignServiceEntity
            {
                CampaignServiceId = item.CAMPAIGNSERVICE_ID,
                CampaignServiceName = item.CAMPAIGNSERVICE_NAME,
                ProductId = item.PRODUCT_ID,
                ProductName = item.TB_R_PRODUCT.PRODUCT_NAME,
                ProductGroupId = item.TB_R_PRODUCT.PRODUCTGROUP_ID,
                ProductGroupName = item.TB_R_PRODUCT.TB_R_PRODUCTGROUP.PRODUCTGROUP_NAME,
            }).ToList();
        }

        public List<CampaignServiceEntity> AutoCompleteSearchCampaignServiceOnMapping(string keyword, int? areaId, int? subAreaId, int? typeId)
        {
            return (from q in _context.TB_M_MAP_PRODUCT
                where
                    q.MAP_PRODUCT_IS_ACTIVE
                    && q.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_IS_ACTIVE
                    && (string.IsNullOrEmpty(keyword) || q.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_NAME.Contains(keyword))
                    && (!areaId.HasValue || (areaId.HasValue && q.AREA_ID == areaId.Value))
                    && (!subAreaId.HasValue || (subAreaId.HasValue && q.SUBAREA_ID == subAreaId.Value))
                    && (!typeId.HasValue || (typeId.HasValue && q.TYPE_ID == typeId.Value))
                select new CampaignServiceEntity
                {
                    CampaignServiceId = q.CAMPAIGNSERVICE_ID,
                    CampaignServiceName = q.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_NAME
                }).Distinct().OrderBy(x => x.CampaignServiceName).Take(10).ToList();
        }

        public IEnumerable<CampaignServiceEntity> GetCampaign(int? productGroupId = null, int? productId = null, int? campaignId = null)
        {
            return from c in _context.TB_R_CAMPAIGNSERVICE
                   join p in _context.TB_R_PRODUCT on c.PRODUCT_ID equals p.PRODUCT_ID
                   join pg in _context.TB_R_PRODUCTGROUP on p.PRODUCTGROUP_ID equals pg.PRODUCTGROUP_ID
                   where (!campaignId.HasValue || c.CAMPAIGNSERVICE_ID == campaignId)
                    && (productGroupId == null || p.PRODUCTGROUP_ID == productGroupId)
                    && (productId == null || c.PRODUCT_ID == productId)
                   select new CampaignServiceEntity
                   {
                       CampaignServiceId = c.CAMPAIGNSERVICE_ID,
                       CampaignServiceCode = c.CAMPAIGNSERVICE_CODE,
                       CampaignServiceName = c.CAMPAIGNSERVICE_NAME,
                       IsActive = c.CAMPAIGNSERVICE_IS_ACTIVE,
                       ProductId = c.PRODUCT_ID,
                       ProductName = p.PRODUCT_NAME,
                       ProductGroupId = pg.PRODUCTGROUP_ID,
                       ProductGroupName = pg.PRODUCTGROUP_NAME
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