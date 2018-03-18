using CSM.Common.Utilities;
using CSM.Entity;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CSM.Data.DataAccess
{
    public class ProductGroupDataAccess : IProductGroupDataAccess
    {
        private readonly CSMContext _context;

        public ProductGroupDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        public List<ProductGroupEntity> AutoCompleteSearchProductGroup(string keyword, int limit, int? productId, bool? isAllStatus)
        {
            var query = _context.TB_R_PRODUCTGROUP.AsQueryable();

            var isAll = (isAllStatus ?? false);

            query = query.Where(q => isAll || (!isAll && q.PRODUCTGROUP_IS_ACTIVE));

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q => q.PRODUCTGROUP_NAME.Contains(keyword));
            }

            if (productId.HasValue)
            {
                var queryProduct = _context.TB_R_PRODUCT.Where(q => q.PRODUCT_ID == productId && q.TB_R_PRODUCTGROUP.PRODUCTGROUP_NAME.Contains(keyword));

                return queryProduct.Take(limit).Select(item => new ProductGroupEntity
                {
                    ProductGroupId = item.PRODUCTGROUP_ID,
                    ProductGroupName = item.TB_R_PRODUCTGROUP.PRODUCTGROUP_NAME,
                }).ToList();
            }

            query = query.OrderBy(q => q.PRODUCTGROUP_NAME);

            return query.Take(limit).Select(item => new ProductGroupEntity
            {
                ProductGroupId = item.PRODUCTGROUP_ID,
                ProductGroupName = item.PRODUCTGROUP_NAME,
            }).Distinct().ToList();
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

        public IEnumerable<ProductGroupEntity> GetProductGroup(int? id)
        {
            return (from p in _context.TB_R_PRODUCTGROUP.AsNoTracking()
                    where (!id.HasValue || p.PRODUCTGROUP_ID == id)
                    select new ProductGroupEntity
                    {
                        ProductGroupId = p.PRODUCTGROUP_ID,
                        ProductGroupCode = p.PRODUCTGROUP_CODE,
                        ProductGroupName = p.PRODUCTGROUP_NAME
                    });
        }

        #endregion
    }
}
