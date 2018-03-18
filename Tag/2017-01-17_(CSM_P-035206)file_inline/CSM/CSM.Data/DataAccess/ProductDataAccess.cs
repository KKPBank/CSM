using System;
using System.Linq;
using CSM.Common.Utilities;
using CSM.Entity;
using log4net;
using System.Collections.Generic;
using System.Globalization;
using System.Data;

namespace CSM.Data.DataAccess
{
    public class ProductDataAccess : IProductDataAccess
    {
        private readonly CSMContext _context;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ProductDataAccess));

        public ProductDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        public IEnumerable<ProductSREntity> SearchProducts(ProductSearchFilter searchFilter)
        {
            var query = (from cs in _context.TB_C_SR_STATUS_CHANGE.AsNoTracking()
                         join fs in _context.TB_C_SR_STATUS on cs.FROM_SR_STATUS_ID equals fs.SR_STATUS_ID
                         join pg in _context.TB_R_PRODUCTGROUP on cs.PRODUCTGROUP_ID equals pg.PRODUCTGROUP_ID
                         join pr in _context.TB_R_PRODUCT on cs.PRODUCT_ID equals pr.PRODUCT_ID
                         join ty in _context.TB_M_TYPE on cs.TYPE_ID equals ty.TYPE_ID
                         join ar in _context.TB_M_AREA on cs.AREA_ID equals ar.AREA_ID
                         from cp in _context.TB_R_CAMPAIGNSERVICE.Where(x => x.CAMPAIGNSERVICE_ID == cs.CAMPAIGNSERVICE_ID).DefaultIfEmpty()
                         from sa in _context.TB_M_SUBAREA.Where(x => x.SUBAREA_ID == cs.SUBAREA_ID).DefaultIfEmpty()
                         from ts in _context.TB_C_SR_STATUS.Where(x => x.SR_STATE_ID == cs.TO_SR_STATUS_ID).DefaultIfEmpty()
                         from fst in _context.TB_C_SR_STATE.Where(x => x.SR_STATE_ID == fs.SR_STATE_ID).DefaultIfEmpty()
                         where (searchFilter.ProductGroupId == null || pg.PRODUCTGROUP_ID == searchFilter.ProductGroupId)
                         && (searchFilter.ProductId == null || pr.PRODUCT_ID == searchFilter.ProductId)
                         && (searchFilter.CampaignId == null || cp.CAMPAIGNSERVICE_ID == searchFilter.CampaignId)
                         && (searchFilter.TypeId == null || ty.TYPE_ID == searchFilter.TypeId)
                         && (searchFilter.AreaId == null || ar.AREA_ID == searchFilter.AreaId)
                         && (searchFilter.SubAreaId == null || sa.SUBAREA_ID == searchFilter.SubAreaId)
                         && (searchFilter.FromSRState == null || searchFilter.FromSRState == Constants.ApplicationStatus.All || fs.SR_STATE_ID == searchFilter.FromSRState)
                         && (searchFilter.ToSRState == null || searchFilter.ToSRState == Constants.ApplicationStatus.All || ts.SR_STATE_ID == searchFilter.ToSRState)
                         && (searchFilter.FromSRStatus == null || searchFilter.FromSRStatus == Constants.ApplicationStatus.All || fs.SR_STATUS_ID == searchFilter.FromSRStatus)
                         && (searchFilter.ToSRStatus == null || searchFilter.ToSRStatus == Constants.ApplicationStatus.All || cs.TO_SR_STATUS_ID == searchFilter.ToSRStatus)
                         group new { fs.SR_STATUS_ID, pg.PRODUCTGROUP_ID, pr.PRODUCT_ID, cp.CAMPAIGNSERVICE_ID, ty.TYPE_ID, ar.AREA_ID, sa.SUBAREA_ID } by
                            new { fs, pg, pr, cp, ty, ar, sa, fst } into g
                         select new ProductSREntity
                         {
                             ProductGroupId = g.Key.pg.PRODUCTGROUP_ID,
                             ProductGroupName = g.Key.pg.PRODUCTGROUP_NAME,
                             ProductId = g.Key.pr.PRODUCT_ID,
                             ProductName = g.Key.pr.PRODUCT_NAME,
                             CampaignId = g.Key.cp != null ? g.Key.cp.CAMPAIGNSERVICE_ID : new Nullable<int>(),
                             CampaignName = g.Key.cp != null ? g.Key.cp.CAMPAIGNSERVICE_NAME : null,
                             TypeId = g.Key.ty.TYPE_ID,
                             TypeName = g.Key.ty.TYPE_NAME,
                             AreaId = g.Key.ar.AREA_ID,
                             AreaName = g.Key.ar.AREA_NAME,
                             SubAreaId = g.Key.sa != null ? g.Key.sa.SUBAREA_ID : new Nullable<int>(),
                             SubAreaName = g.Key.sa != null ? g.Key.sa.SUBAREA_NAME : null,
                             FromSRStatusId = g.Key.fs.SR_STATUS_ID,
                             FromSRStatusName = g.Key.fs.SR_STATUS_NAME,
                             FromSRStateId = g.Key.fst.SR_STATE_ID,
                             FromSRStateName = g.Key.fst.SR_STATE_NAME
                         });

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = query.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            query = SetProductListSort(query, searchFilter);
            var productList = query.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<ProductSREntity>();
            productList.ForEach(x => GetToSRStatusList(x));
            return productList;
        }

        public ProductSREntity GetProduct(ProductSearchFilter searchFilter)
        {
            var query = (from cs in _context.TB_C_SR_STATUS_CHANGE
                         join fs in _context.TB_C_SR_STATUS on cs.FROM_SR_STATUS_ID equals fs.SR_STATUS_ID
                         join pg in _context.TB_R_PRODUCTGROUP on cs.PRODUCTGROUP_ID equals pg.PRODUCTGROUP_ID
                         join pr in _context.TB_R_PRODUCT on cs.PRODUCT_ID equals pr.PRODUCT_ID
                         join ty in _context.TB_M_TYPE on cs.TYPE_ID equals ty.TYPE_ID
                         join ar in _context.TB_M_AREA on cs.AREA_ID equals ar.AREA_ID
                         from cp in _context.TB_R_CAMPAIGNSERVICE.Where(x => x.CAMPAIGNSERVICE_ID == cs.CAMPAIGNSERVICE_ID).DefaultIfEmpty()
                         from sa in _context.TB_M_SUBAREA.Where(x => x.SUBAREA_ID == cs.SUBAREA_ID).DefaultIfEmpty()
                         from fst in _context.TB_C_SR_STATE.Where(x => x.SR_STATE_ID == fs.SR_STATE_ID).DefaultIfEmpty()
                         where pg.PRODUCTGROUP_ID == searchFilter.ProductGroupId
                              && pr.PRODUCT_ID == searchFilter.ProductId
                              && cp.CAMPAIGNSERVICE_ID == searchFilter.CampaignId
                              && ty.TYPE_ID == searchFilter.TypeId
                              && ar.AREA_ID == searchFilter.AreaId
                              && sa.SUBAREA_ID == searchFilter.SubAreaId
                              && fs.SR_STATUS_ID == searchFilter.FromSRStatus
                         group new { fs.SR_STATUS_ID, pg.PRODUCTGROUP_ID, pr.PRODUCT_ID, cp.CAMPAIGNSERVICE_ID, ty.TYPE_ID, ar.AREA_ID, sa.SUBAREA_ID } by
                            new { fs, pg, pr, cp, ty, ar, sa, fst } into g
                         select new ProductSREntity
                         {
                             ProductGroupId = g.Key.pg.PRODUCTGROUP_ID,
                             ProductGroupName = g.Key.pg.PRODUCTGROUP_NAME,
                             ProductId = g.Key.pr.PRODUCT_ID,
                             ProductName = g.Key.pr.PRODUCT_NAME,
                             CampaignId = g.Key.cp != null ? g.Key.cp.CAMPAIGNSERVICE_ID : new Nullable<int>(),
                             CampaignName = g.Key.cp != null ? g.Key.cp.CAMPAIGNSERVICE_NAME : null,
                             TypeId = g.Key.ty.TYPE_ID,
                             TypeName = g.Key.ty.TYPE_NAME,
                             AreaId = g.Key.ar.AREA_ID,
                             AreaName = g.Key.ar.AREA_NAME,
                             SubAreaId = g.Key.sa != null ? g.Key.sa.SUBAREA_ID : new Nullable<int>(),
                             SubAreaName = g.Key.sa != null ? g.Key.sa.SUBAREA_NAME : null,
                             FromSRStatusId = g.Key.fs.SR_STATUS_ID,
                             FromSRStatusName = g.Key.fs.SR_STATUS_NAME,
                             FromSRStateId = g.Key.fst.SR_STATE_ID,
                             FromSRStateName = g.Key.fst.SR_STATE_NAME
                         });

            var product = query.Any() ? query.FirstOrDefault() : null;
            this.GetToSRStatusList(product);
            return product;
        }

        public bool IsDuplicateSRStatus(ProductSREntity product)
        {
            bool result = false;
            foreach (int statusId in product.ToSRStatusIds)
            {
                var cntStatus = _context.TB_C_SR_STATUS_CHANGE.Where(
                            x => x.PRODUCTGROUP_ID == product.ProductGroupId && x.PRODUCT_ID == product.ProductId
                                 && x.CAMPAIGNSERVICE_ID == product.CampaignId && x.TYPE_ID == product.TypeId
                                 && x.AREA_ID == product.AreaId && x.SUBAREA_ID == product.SubAreaId
                                 && x.FROM_SR_STATUS_ID == product.FromSRStatusId
                                 && x.TO_SR_STATUS_ID == statusId).Count();

                result = cntStatus > 0;
                if (result == true) break;
            }

            return result;
        }

        public bool SaveSRStatus(ProductSREntity product)
        {
            try
            {
                var today = DateTime.Now;

                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        if (product.IsEdit == true)
                        {
                            var statusChanges = _context.TB_C_SR_STATUS_CHANGE.Where(
                                x => x.PRODUCTGROUP_ID == product.ProductGroupId && x.PRODUCT_ID == product.ProductId
                                     && x.CAMPAIGNSERVICE_ID == product.CampaignId && x.TYPE_ID == product.TypeId
                                     && x.AREA_ID == product.AreaId && x.SUBAREA_ID == product.SubAreaId
                                     && x.FROM_SR_STATUS_ID == product.FromSRStatusId);

                            if (statusChanges.Any())
                            {
                                _context.TB_C_SR_STATUS_CHANGE.RemoveRange(statusChanges);
                                this.Save();
                            }
                        }

                        if (product.ToSRStatusIds != null && product.ToSRStatusIds.Count > 0)
                        {
                            foreach (int statusId in product.ToSRStatusIds)
                            {
                                var statusChange = new TB_C_SR_STATUS_CHANGE();
                                statusChange.PRODUCTGROUP_ID = product.ProductGroupId;
                                statusChange.PRODUCT_ID = product.ProductId;
                                statusChange.CAMPAIGNSERVICE_ID = product.CampaignId;
                                statusChange.TYPE_ID = product.TypeId;
                                statusChange.AREA_ID = product.AreaId;
                                statusChange.SUBAREA_ID = product.SubAreaId;
                                statusChange.FROM_SR_STATUS_ID = product.FromSRStatusId;
                                statusChange.TO_SR_STATUS_ID = statusId;
                                statusChange.CREATE_USER = product.CreateUser.UserId;
                                statusChange.UPDATE_USER = product.UpdateUser.UserId;
                                statusChange.CREATE_DATE = today;
                                statusChange.UPDATE_DATE = today;

                                _context.TB_C_SR_STATUS_CHANGE.Add(statusChange);
                            }
                            this.Save();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        public bool DeleteSRStatus(ProductSearchFilter searchFilter)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var statusChanges = _context.TB_C_SR_STATUS_CHANGE.Where(
                    x => x.PRODUCTGROUP_ID == searchFilter.ProductGroupId && x.PRODUCT_ID == searchFilter.ProductId
                        && x.CAMPAIGNSERVICE_ID == searchFilter.CampaignId && x.TYPE_ID == searchFilter.TypeId
                        && x.AREA_ID == searchFilter.AreaId && x.SUBAREA_ID == searchFilter.SubAreaId
                        && x.FROM_SR_STATUS_ID == searchFilter.FromSRStatus);

                if (statusChanges.Any())
                {
                    _context.TB_C_SR_STATUS_CHANGE.RemoveRange(statusChanges);
                    this.Save();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }

            return false;
        }

        private void GetToSRStatusList(ProductSREntity product)
        {
            if (product != null)
            {
                var query = from cs in _context.TB_C_SR_STATUS_CHANGE
                            join ts in _context.TB_C_SR_STATUS on cs.TO_SR_STATUS_ID equals ts.SR_STATUS_ID
                            where cs.PRODUCTGROUP_ID == product.ProductGroupId
                                  && cs.PRODUCT_ID == product.ProductId
                                  && cs.CAMPAIGNSERVICE_ID == product.CampaignId
                                  && cs.TYPE_ID == product.TypeId
                                  && cs.AREA_ID == product.AreaId
                                  && cs.SUBAREA_ID == product.SubAreaId
                                  && cs.FROM_SR_STATUS_ID == product.FromSRStatusId
                            select new SRStatusEntity
                            {
                                SRStatusId = ts.SR_STATUS_ID,
                                SRStatusCode = ts.SR_STATUS_CODE,
                                SRStatusName = ts.SR_STATUS_NAME
                            };

                product.ToSRStatusList = query.Any() ? query.ToList() : null;

                #region "Get UserCreate"
                // UserInfo
                var queryUserCreate = from cs in _context.TB_C_SR_STATUS_CHANGE
                    join ts in _context.TB_C_SR_STATUS on cs.TO_SR_STATUS_ID equals ts.SR_STATUS_ID
                    from ucs in _context.TB_R_USER.Where(o => o.USER_ID == cs.CREATE_USER).DefaultIfEmpty()
                    from us in _context.TB_R_USER.Where(o => o.USER_ID == cs.UPDATE_USER).DefaultIfEmpty()
                    where cs.PRODUCTGROUP_ID == product.ProductGroupId
                          && cs.PRODUCT_ID == product.ProductId
                          && cs.CAMPAIGNSERVICE_ID == product.CampaignId
                          && cs.TYPE_ID == product.TypeId
                          && cs.AREA_ID == product.AreaId
                          && cs.SUBAREA_ID == product.SubAreaId
                          && cs.FROM_SR_STATUS_ID == product.FromSRStatusId
                    select new ProductSREntity
                    {
                        CreatedDate = cs.CREATE_DATE,
                        Updatedate = cs.UPDATE_DATE,
                        CreateUser = ucs != null
                            ? new UserEntity
                            {
                                Firstname = ucs.FIRST_NAME,
                                Lastname = ucs.LAST_NAME,
                                PositionCode = ucs.POSITION_CODE
                            }
                            : null,
                        UpdateUser = us != null
                            ? new UserEntity
                            {
                                Firstname = us.FIRST_NAME,
                                Lastname = us.LAST_NAME,
                                PositionCode = us.POSITION_CODE
                            }
                            : null
                    };

                var userObj = queryUserCreate.Any() ? queryUserCreate.FirstOrDefault() : null;
                if (userObj != null)
                {
                    product.CreatedDate = userObj.CreatedDate;
                    product.Updatedate = userObj.Updatedate;
                    product.CreateUser = userObj.CreateUser;
                    product.UpdateUser = userObj.UpdateUser;
                }

                #endregion
            }
        }

        private static IQueryable<ProductSREntity> SetProductListSort(IQueryable<ProductSREntity> productList, ProductSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "PRODUCTGROUP":
                        return productList.OrderBy(ord => ord.ProductGroupName);
                    case "PRODUCT":
                        return productList.OrderBy(ord => ord.ProductName);
                    case "CAMPAIGN":
                        return productList.OrderBy(ord => ord.CampaignName);
                    case "TYPE":
                        return productList.OrderBy(ord => ord.TypeName);
                    case "AREA":
                        return productList.OrderBy(ord => ord.AreaName);
                    case "SUBAREA":
                        return productList.OrderBy(ord => ord.SubAreaName);
                    case "FROMSRSTATUS":
                        return productList.OrderBy(ord => ord.FromSRStatusName);
                    default:
                        return productList.OrderBy(ord => ord.ProductGroupName);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "PRODUCTGROUP":
                        return productList.OrderByDescending(ord => ord.ProductGroupName);
                    case "PRODUCT":
                        return productList.OrderByDescending(ord => ord.ProductName);
                    case "CAMPAIGN":
                        return productList.OrderByDescending(ord => ord.CampaignName);
                    case "TYPE":
                        return productList.OrderByDescending(ord => ord.TypeName);
                    case "AREA":
                        return productList.OrderByDescending(ord => ord.AreaName);
                    case "SUBAREA":
                        return productList.OrderByDescending(ord => ord.SubAreaName);
                    case "FROMSRSTATUS":
                        return productList.OrderByDescending(ord => ord.FromSRStatusName);
                    default:
                        return productList.OrderByDescending(ord => ord.ProductGroupName);
                }
            }
        }

        #region "Product Group"

        public List<ProductGroupEntity> GetProductGroupByName(string searchTerm, int pageSize, int pageNum, int? productId, int? campaignId)
        {
            return GetProductGroupByName(searchTerm, productId, campaignId).OrderBy(x => x.ProductGroupName)
               .Skip(pageSize * (pageNum - 1))
               .Take(pageSize)
               .ToList();
        }

        public int GetProductGroupCountByName(string searchTerm, int pageSize, int pageNum, int? productId, int? campaignId)
        {
            return GetProductGroupByName(searchTerm, productId, campaignId).Count();
        }

        private IQueryable<ProductGroupEntity> GetProductGroupByName(string searchTerm, int? productId, int? campaignId)
        {
            var query = from pg in _context.TB_R_PRODUCTGROUP.AsNoTracking()
                        from pd in _context.TB_R_PRODUCT.Where(x => x.PRODUCTGROUP_ID == pg.PRODUCTGROUP_ID).DefaultIfEmpty()
                        from cm in _context.TB_R_CAMPAIGNSERVICE.Where(x => x.PRODUCT_ID == pd.PRODUCT_ID).DefaultIfEmpty()
                        where (string.IsNullOrEmpty(searchTerm) || pg.PRODUCTGROUP_NAME.Contains(searchTerm))
                              && (productId == null || pd.PRODUCT_ID == productId)
                              && (campaignId == null || cm.CAMPAIGNSERVICE_ID == campaignId)
                        group pg.PRODUCTGROUP_ID by pg into g
                        select new ProductGroupEntity
                        {
                            ProductGroupId = g.Key.PRODUCTGROUP_ID,
                            ProductGroupName = g.Key.PRODUCTGROUP_NAME
                        };
            return query;
        }

        #endregion

        #region "Product"

        public List<ProductEntity> GetProductByName(string searchTerm, int pageSize, int pageNum, int? productGroupId, int? campaignId)
        {
            return GetProductByName(searchTerm, productGroupId, campaignId).OrderBy(x => x.ProductName)
               .Skip(pageSize * (pageNum - 1))
               .Take(pageSize)
               .ToList();
        }

        public int GetProductCountByName(string searchTerm, int pageSize, int pageNum, int? productGroupId, int? campaignId)
        {
            return GetProductByName(searchTerm, productGroupId, campaignId).Count();
        }

        private IQueryable<ProductEntity> GetProductByName(string searchTerm, int? productGroupId, int? campaignId)
        {
            var query = from pd in _context.TB_R_PRODUCT.AsNoTracking()
                        from pg in _context.TB_R_PRODUCTGROUP.Where(x => x.PRODUCTGROUP_ID == pd.PRODUCTGROUP_ID).DefaultIfEmpty()
                        from cm in _context.TB_R_CAMPAIGNSERVICE.Where(x => x.PRODUCT_ID == pd.PRODUCT_ID).DefaultIfEmpty()
                        where (string.IsNullOrEmpty(searchTerm) || pd.PRODUCT_NAME.Contains(searchTerm))
                              && (productGroupId == null || pd.PRODUCTGROUP_ID == productGroupId)
                              && (campaignId == null || cm.CAMPAIGNSERVICE_ID == campaignId)
                        group pd.PRODUCT_ID by pd into g
                        select new ProductEntity
                        {
                            ProductId = g.Key.PRODUCT_ID,
                            ProductName = g.Key.PRODUCT_NAME
                        };

            return query;
        }

        #endregion

        #region "Campaign Service"

        public List<CampaignServiceEntity> GetCampaignServiceByName(string searchTerm, int pageSize, int pageNum, int? productGroupId, int? campaignId)
        {
            return GetCampaignServiceByName(searchTerm, productGroupId, campaignId).OrderBy(x => x.CampaignServiceName)
               .Skip(pageSize * (pageNum - 1))
               .Take(pageSize)
               .ToList();
        }

        public int GetCampaignServiceCountByName(string searchTerm, int pageSize, int pageNum, int? productGroupId, int? productId)
        {
            return GetCampaignServiceByName(searchTerm, productGroupId, productId).Count();
        }

        private IQueryable<CampaignServiceEntity> GetCampaignServiceByName(string searchTerm, int? productGroupId, int? productId)
        {
            var query = from cm in _context.TB_R_CAMPAIGNSERVICE
                        from pd in _context.TB_R_PRODUCT.Where(x => x.PRODUCT_ID == cm.PRODUCT_ID).DefaultIfEmpty()
                        from pg in _context.TB_R_PRODUCTGROUP.Where(x => x.PRODUCTGROUP_ID == pd.PRODUCTGROUP_ID).DefaultIfEmpty()
                        where (string.IsNullOrEmpty(searchTerm) || cm.CAMPAIGNSERVICE_NAME.Contains(searchTerm))
                              && (productGroupId == null || pd.PRODUCTGROUP_ID == productGroupId)
                              && (productId == null || cm.PRODUCT_ID == productId)
                        group cm.CAMPAIGNSERVICE_ID by cm into g
                        select new CampaignServiceEntity
                        {
                            CampaignServiceId = g.Key.CAMPAIGNSERVICE_ID,
                            CampaignServiceName = g.Key.CAMPAIGNSERVICE_NAME
                        };

            return query;
        }

        #endregion

        #region "Type"

        public List<TypeEntity> GetTypeByName(string searchTerm, int pageSize, int pageNum)
        {
            return GetTypeByName(searchTerm).OrderBy(x => x.TypeName)
               .Skip(pageSize * (pageNum - 1))
               .Take(pageSize)
               .ToList();
        }

        public int GetTypeCountByName(string searchTerm, int pageSize, int pageNum)
        {
            return GetTypeByName(searchTerm).Count();
        }

        private IQueryable<TypeEntity> GetTypeByName(string searchTerm)
        {
            return _context.TB_M_TYPE.Where(x => x.TYPE_NAME.Contains(searchTerm))
                .Select(x => new TypeEntity
                {
                    TypeId = x.TYPE_ID,
                    TypeName = x.TYPE_NAME
                });
        }

        #endregion

        #region "Area"

        public List<AreaEntity> GetAreaByName(string searchTerm, int pageSize, int pageNum, int? subAreaId)
        {
            return GetAreaByName(searchTerm, subAreaId).OrderBy(x => x.AreaName)
               .Skip(pageSize * (pageNum - 1))
               .Take(pageSize)
               .ToList();
        }

        public int GetAreaCountByName(string searchTerm, int pageSize, int pageNum, int? subAreaId)
        {
            return GetAreaByName(searchTerm, subAreaId).Count();
        }

        private IQueryable<AreaEntity> GetAreaByName(string searchTerm, int? subAreaId)
        {
            var query = from ar in _context.TB_M_AREA.AsNoTracking()
                        from ab in _context.TB_M_AREA_SUBAREA.Where(x => x.AREA_ID == ar.AREA_ID).DefaultIfEmpty()
                        where (string.IsNullOrEmpty(searchTerm) || ar.AREA_NAME.Contains(searchTerm))
                              && (subAreaId == null || ab.SUBAREA_ID == subAreaId)
                        group ar.AREA_ID by ar into g
                        select new AreaEntity
                        {
                            AreaId = g.Key.AREA_ID,
                            AreaName = g.Key.AREA_NAME
                        };

            return query;
        }

        #endregion

        #region "SubArea"

        public List<SubAreaEntity> GetSubAreaByName(string searchTerm, int pageSize, int pageNum, int? areaId)
        {
            return GetSubAreaByName(searchTerm, areaId).OrderBy(x => x.SubareaName)
               .Skip(pageSize * (pageNum - 1))
               .Take(pageSize)
               .ToList();
        }

        public int GetSubAreaCountByName(string searchTerm, int pageSize, int pageNum, int? areaId)
        {
            return GetSubAreaByName(searchTerm, areaId).Count();
        }

        private IQueryable<SubAreaEntity> GetSubAreaByName(string searchTerm, int? areaId)
        {
            var query = from sb in _context.TB_M_SUBAREA.AsNoTracking()
                        from ab in _context.TB_M_AREA_SUBAREA.Where(x => x.SUBAREA_ID == sb.SUBAREA_ID).DefaultIfEmpty()
                        where (string.IsNullOrEmpty(searchTerm) || sb.SUBAREA_NAME.Contains(searchTerm))
                         && (areaId == null || ab.AREA_ID == areaId)
                        group sb.SUBAREA_ID by sb into g
                        select new SubAreaEntity
                        {
                            SubareaId = g.Key.SUBAREA_ID,
                            SubareaName = g.Key.SUBAREA_NAME
                        };

            return query;
        }

        #endregion

        public bool SaveProductMaster(ProductGroupEntity productGroup, ProductEntity product, CampaignServiceEntity campaign)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        TB_R_PRODUCT dbProduct = null;
                        TB_R_CAMPAIGNSERVICE dbCampaign = null;
                        TB_R_PRODUCTGROUP dbProductGroup = null;

                        #region "Product Group"

                        if (productGroup != null)
                        {
                            dbProductGroup = _context.TB_R_PRODUCTGROUP.FirstOrDefault(x => x.PRODUCTGROUP_CODE == productGroup.ProductGroupCode);
                            if (dbProductGroup == null)
                            {
                                dbProductGroup = new TB_R_PRODUCTGROUP
                                {
                                    PRODUCTGROUP_CODE = productGroup.ProductGroupCode,
                                    PRODUCTGROUP_NAME = productGroup.ProductGroupName,
                                    PRODUCTGROUP_IS_ACTIVE = productGroup.IsActive,
                                    CREATE_USER = productGroup.CreateUser,
                                    CREATE_DATE = productGroup.CreateDate,
                                    UPDATE_USER = productGroup.UpdateUser,
                                    UPDATE_DATE = productGroup.UpdateDate
                                };

                                _context.TB_R_PRODUCTGROUP.Add(dbProductGroup);
                            }
                            else
                            {
                                dbProductGroup.PRODUCTGROUP_CODE = productGroup.ProductGroupCode;
                                dbProductGroup.PRODUCTGROUP_NAME = productGroup.ProductGroupName;
                                dbProductGroup.PRODUCTGROUP_IS_ACTIVE = productGroup.IsActive;
                                dbProductGroup.UPDATE_USER = productGroup.UpdateUser;
                                dbProductGroup.UPDATE_DATE = productGroup.UpdateDate;
                                SetEntryStateModified(dbProductGroup);
                            }

                            this.Save();
                        }

                        #endregion

                        #region "Product"

                        if (dbProductGroup != null && product != null)
                        {
                            dbProduct = _context.TB_R_PRODUCT.FirstOrDefault(x => x.PRODUCT_CODE == product.ProductCode);
                            if (dbProduct == null)
                            {
                                dbProduct = new TB_R_PRODUCT
                                {
                                    PRODUCT_CODE = product.ProductCode,
                                    PRODUCT_NAME = product.ProductName,
                                    PRODUCT_TYPE = product.ProductType,
                                    PRODUCT_IS_ACTIVE = product.IsActive,
                                    PRODUCTGROUP_ID = dbProductGroup.PRODUCTGROUP_ID,
                                    CREATE_USER = product.CreateUser,
                                    CREATE_DATE = product.CreateDate,
                                    UPDATE_USER = product.UpdateUser,
                                    UPDATE_DATE = product.UpdateDate
                                };

                                _context.TB_R_PRODUCT.Add(dbProduct);
                            }
                            else
                            {
                                dbProduct.PRODUCT_CODE = product.ProductCode;
                                dbProduct.PRODUCT_NAME = product.ProductName;
                                dbProduct.PRODUCT_TYPE = product.ProductType;
                                dbProduct.PRODUCT_IS_ACTIVE = product.IsActive;
                                dbProduct.PRODUCTGROUP_ID = dbProductGroup.PRODUCTGROUP_ID;
                                dbProduct.UPDATE_USER = product.UpdateUser;
                                dbProduct.UPDATE_DATE = product.UpdateDate;
                                SetEntryStateModified(dbProduct);
                            }

                            this.Save();
                        }

                        #endregion

                        #region "Campaign"

                        if (dbProduct != null && campaign != null)
                        {
                            dbCampaign = _context.TB_R_CAMPAIGNSERVICE.FirstOrDefault(x => x.CAMPAIGNSERVICE_CODE == campaign.CampaignServiceCode);
                            if (dbCampaign == null)
                            {
                                dbCampaign = new TB_R_CAMPAIGNSERVICE
                                {
                                    CAMPAIGNSERVICE_CODE = campaign.CampaignServiceCode,
                                    CAMPAIGNSERVICE_NAME = campaign.CampaignServiceName,
                                    CAMPAIGNSERVICE_IS_ACTIVE = campaign.IsActive,
                                    PRODUCT_ID = dbProduct.PRODUCT_ID,
                                    CREATE_USER = campaign.CreateUser,
                                    CREATE_DATE = campaign.CreateDate,
                                    UPDATE_USER = campaign.UpdateUser,
                                    UPDATE_DATE = campaign.UpdateDate
                                };

                                _context.TB_R_CAMPAIGNSERVICE.Add(dbCampaign);
                            }
                            else
                            {
                                dbCampaign.CAMPAIGNSERVICE_CODE = campaign.CampaignServiceCode;
                                dbCampaign.CAMPAIGNSERVICE_NAME = campaign.CampaignServiceName;
                                dbCampaign.CAMPAIGNSERVICE_IS_ACTIVE = campaign.IsActive;
                                dbCampaign.PRODUCT_ID = dbProduct.PRODUCT_ID;
                                dbCampaign.UPDATE_USER = campaign.UpdateUser;
                                dbCampaign.UPDATE_DATE = campaign.UpdateDate;
                                SetEntryStateModified(dbCampaign);
                            }

                            this.Save();
                        }

                        #endregion

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        public List<ProductEntity> AutoCompleteSearchProduct(string keyword, int? productGroupId, int limit, int? campaignServiceId, bool? isAllStatus)
        {
            var query = _context.TB_R_PRODUCT.AsQueryable();

            var isAll = (isAllStatus ?? false);

            query = query.Where(q => isAll || (!isAll && q.PRODUCT_IS_ACTIVE));

            if (productGroupId.HasValue)
            {
                query = query.Where(q => q.PRODUCTGROUP_ID == productGroupId.Value);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q => q.PRODUCT_NAME.Contains(keyword));
            }

            if (campaignServiceId.HasValue)
            {
                var queryCampaignService = _context.TB_R_CAMPAIGNSERVICE.Where(q => q.CAMPAIGNSERVICE_ID == campaignServiceId && q.TB_R_PRODUCT.PRODUCT_NAME.Contains(keyword));
                return queryCampaignService.Take(limit).Select(item => new ProductEntity()
                {
                    ProductId = item.PRODUCT_ID,
                    ProductName = item.TB_R_PRODUCT.PRODUCT_NAME,
                    ProductGroupId = item.TB_R_PRODUCT.PRODUCTGROUP_ID,
                    ProductGroupName = item.TB_R_PRODUCT.TB_R_PRODUCTGROUP.PRODUCTGROUP_NAME,
                }).Distinct().ToList();
            }

            query = query.OrderBy(q => q.PRODUCT_NAME);

            return query.Take(limit).Select(item => new ProductEntity
            {
                ProductId = item.PRODUCT_ID,
                ProductName = item.PRODUCT_NAME,
                ProductGroupId = item.PRODUCTGROUP_ID,
                ProductGroupName = item.TB_R_PRODUCTGROUP.PRODUCTGROUP_NAME,
            }).Distinct().ToList();
        }

        public List<ProductEntity> AutoCompleteSearchProductForQuestionGroup(string keyword, int? productGroupId, int limit, int? campaignServiceId)
        {
            var query = _context.TB_R_PRODUCT.AsQueryable();

            if (productGroupId.HasValue)
            {
                query = query.Where(q => q.PRODUCTGROUP_ID == productGroupId.Value);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q => q.PRODUCT_NAME.Contains(keyword));
            }

            if (campaignServiceId.HasValue)
            {
                var queryCampaignService = _context.TB_R_CAMPAIGNSERVICE.Where(q => q.CAMPAIGNSERVICE_ID == campaignServiceId && q.TB_R_PRODUCT.PRODUCT_NAME.Contains(keyword));
                return queryCampaignService.Take(limit).Select(item => new ProductEntity()
                {
                    ProductId = item.PRODUCT_ID,
                    ProductName = item.TB_R_PRODUCT.PRODUCT_NAME,
                    ProductGroupId = item.TB_R_PRODUCT.PRODUCTGROUP_ID,
                    ProductGroupName = item.TB_R_PRODUCT.TB_R_PRODUCTGROUP.PRODUCTGROUP_NAME,
                }).Distinct().ToList();
            }

            query = query.OrderBy(q => q.PRODUCT_NAME);

            return query.Take(limit).Select(item => new ProductEntity
            {
                ProductId = item.PRODUCT_ID,
                ProductName = item.PRODUCT_NAME,
                ProductGroupId = item.PRODUCTGROUP_ID,
                ProductGroupName = item.TB_R_PRODUCTGROUP.PRODUCTGROUP_NAME,
            }).Distinct().ToList();
        }

        public List<ProductEntity> GetProductList(int? productGroupId = null)
        {
            return (from p in _context.TB_R_PRODUCT
                    where (productGroupId == null || p.PRODUCTGROUP_ID == productGroupId)
                    orderby p.PRODUCT_NAME
                    select new ProductEntity()
                    {
                        ProductId = p.PRODUCT_ID,
                        ProductName = p.PRODUCT_NAME,
                    }).ToList();
        }

        public IEnumerable<ProductEntity> GetProduct(int? id = null)
        {
            return (from p in _context.TB_R_PRODUCT
                    where (id == null || p.PRODUCT_ID == id)
                    orderby p.PRODUCT_NAME
                    select new ProductEntity()
                    {
                        ProductId = p.PRODUCT_ID,
                        ProductName = p.PRODUCT_NAME,
                    });
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
