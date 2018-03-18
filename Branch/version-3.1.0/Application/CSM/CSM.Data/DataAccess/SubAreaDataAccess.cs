using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using CSM.Entity;
using log4net;
using System.Globalization;
using CSM.Common.Utilities;

namespace CSM.Data.DataAccess
{
    public class SubAreaDataAccess : ISubAreaDataAccess
    {
        private readonly CSMContext _context;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SubAreaDataAccess));

        public SubAreaDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        #region "SubArea"

        public IEnumerable<SubAreaItemEntity> GetSelectSubAreaList(SelectSubAreaSearchFilter searchFilter)
        {
            var resultQuery = (from subArea in _context.TB_M_SUBAREA
                               from createUser in _context.TB_R_USER.Where(x => x.USER_ID == subArea.CREATE_USER).DefaultIfEmpty()
                               from updateUser in _context.TB_R_USER.Where(x => x.USER_ID == subArea.UPDATE_USER).DefaultIfEmpty()
                               where ((searchFilter.SubAreaName == null || subArea.SUBAREA_NAME.Contains(searchFilter.SubAreaName)) && (searchFilter.SubAreaCode == null || SqlFunctions.StringConvert(subArea.SUBAREA_CODE).Contains(searchFilter.SubAreaCode)))
                               select new SubAreaItemEntity
                               {
                                   SubAreaId = subArea.SUBAREA_ID,
                                   SubAreaName = subArea.SUBAREA_NAME,
                                   SubAreaCode = SqlFunctions.StringConvert(subArea.SUBAREA_CODE),
                                   IsActive = subArea.SUBAREA_IS_ACTIVE,
                                   UpdateUser = (updateUser != null
                                       ? new UserEntity
                                       {
                                           PositionCode = updateUser.POSITION_CODE,
                                           Firstname = updateUser.FIRST_NAME,
                                           Lastname = updateUser.LAST_NAME
                                       }
                                       : null),
                                   CreateUser = (createUser != null
                                       ? new UserEntity
                                       {
                                           PositionCode = createUser.POSITION_CODE,
                                           Firstname = createUser.FIRST_NAME,
                                           Lastname = createUser.LAST_NAME
                                       }
                                       : null),
                                   UpdateDateTime = subArea.UPDATE_DATE.HasValue ? subArea.UPDATE_DATE : subArea.CREATE_DATE
                               });

            if (!string.IsNullOrEmpty(searchFilter.SubAreaIdList))
            {
                var subAreaIdArray = searchFilter.SubAreaIdList.Split(',').Select(s => Convert.ToInt32(s)).ToList();
                resultQuery = resultQuery.Where(q => !subAreaIdArray.Contains(q.SubAreaId.Value));
            }

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = resultQuery.Count();

            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            resultQuery = SetSelectSubAreaListSort(resultQuery, searchFilter);
            return resultQuery.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();
        }

        public bool ValidateSubArea(string subAreaName, int? subAreaId)
        {
            if (!subAreaId.HasValue)
            {
                //validate add
                return _context.TB_M_SUBAREA.Count(q => q.SUBAREA_NAME == subAreaName) == 0;
            }
            else
            {
                //validate edit
                return _context.TB_M_SUBAREA.Count(q => q.SUBAREA_ID != subAreaId.Value && q.SUBAREA_NAME == subAreaName) == 0;
            }
        }

        public SubAreaItemEntity SaveSubArea(SubAreaItemEntity subAreaItemEntity)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                var isEdit = subAreaItemEntity.SubAreaId.HasValue;

                TB_M_SUBAREA subArea;

                if (!isEdit)
                {
                    subArea = new TB_M_SUBAREA();
                    subArea.SUBAREA_CODE = GetNextSubAreaCode();
                }
                else
                {
                    subArea = _context.TB_M_SUBAREA.SingleOrDefault(s => s.SUBAREA_ID == subAreaItemEntity.SubAreaId.Value);
                    if (subArea == null)
                    {
                        Logger.ErrorFormat("SUBAREA ID: {0} does not exist", subAreaItemEntity.SubAreaId);
                        return null;
                    }
                }

                subArea.SUBAREA_NAME = subAreaItemEntity.SubAreaName;
                //subArea.SUBAREA_CODE = Convert.ToDecimal(subAreaItemEntity.SubAreaCode);
                subArea.SUBAREA_IS_ACTIVE = subAreaItemEntity.IsActive;
                subArea.UPDATE_USER = subAreaItemEntity.UserId;
                subArea.UPDATE_DATE = DateTime.Now;

                if (!isEdit)
                {
                    subArea.CREATE_USER = subAreaItemEntity.UserId;
                    subArea.CREATE_DATE = DateTime.Now;
                    _context.TB_M_SUBAREA.Add(subArea);
                }
                else
                {
                    SetEntryStateModified(subArea);
                }

                Save();

                SubAreaItemEntity result = new SubAreaItemEntity();
                result.SubAreaId = subArea.SUBAREA_ID;
                result.SubAreaName = subArea.SUBAREA_NAME;
                result.SubAreaCode = Convert.ToString(subArea.SUBAREA_CODE);
                result.IsActive = subArea.SUBAREA_IS_ACTIVE;
                result.CreateDateTime = subArea.CREATE_DATE;
                result.UpdateDateTime = subArea.UPDATE_DATE;
                result.UpdateUser = new UserDataAccess(_context).GetUserById(subArea.UPDATE_USER??0);
                result.IsEdit = isEdit;
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                return null;
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public SubAreaItemEntity GetSubAreaItem(int id)
        {
            var subAreaItemEntity = new SubAreaItemEntity();
            var query = _context.TB_M_SUBAREA.SingleOrDefault(q => q.SUBAREA_ID == id);

            if (query != null)
            {
                subAreaItemEntity.SubAreaId = query.SUBAREA_ID;
                subAreaItemEntity.SubAreaName = query.SUBAREA_NAME;
                subAreaItemEntity.SubAreaCode = Convert.ToString(query.SUBAREA_CODE);
                subAreaItemEntity.IsActive = query.SUBAREA_IS_ACTIVE;
                return subAreaItemEntity;
            }

            return null;
        }

        public IEnumerable<SubAreaItemEntity> GetSubAreaListById(SelectSubAreaSearchFilter searchFilter)
        {
            var resultQuery = (from areaSubArea in _context.TB_M_AREA_SUBAREA
                               from createUser in _context.TB_R_USER.Where(x => x.USER_ID == areaSubArea.CREATE_USER).DefaultIfEmpty()
                               from subArea in _context.TB_M_SUBAREA.Where(x => x.SUBAREA_ID == areaSubArea.SUBAREA_ID).DefaultIfEmpty()
                               where (areaSubArea.AREA_ID == searchFilter.AreaId)
                               select new SubAreaItemEntity
                               {
                                   SubAreaId = areaSubArea.SUBAREA_ID,
                                   SubAreaName = subArea.SUBAREA_NAME,
                                   SubAreaCode = SqlFunctions.StringConvert(subArea.SUBAREA_CODE),
                                   IsActive = subArea.SUBAREA_IS_ACTIVE,
                                   CreateUser = (createUser != null
                                       ? new UserEntity
                                       {
                                           PositionCode = createUser.POSITION_CODE,
                                           Firstname = createUser.FIRST_NAME,
                                           Lastname = createUser.LAST_NAME
                                       }
                                       : null),
                                   UpdateDateTime = areaSubArea.CREATE_DATE
                               });

            if (!string.IsNullOrEmpty(searchFilter.SubAreaIdList))
            {
                var subAreaIdArray = searchFilter.SubAreaIdList.Split(',').Select(s => Convert.ToInt32(s)).ToList();
                resultQuery = resultQuery.Where(q => !subAreaIdArray.Contains(q.SubAreaId.Value));
            }

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = resultQuery.Count();

            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            resultQuery = SetSelectSubAreaListSort(resultQuery, searchFilter);

            return resultQuery.ToList();
            //            return resultQuery.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();
        }

        #endregion

        public List<SubAreaItemEntity> AutoCompleteSearchSubArea(string keyword, int? areaId, int limit, bool? isAllStatus)
        {
            var query = _context.TB_M_SUBAREA.AsQueryable();

            var isAll = (isAllStatus ?? false);

            query = query.Where(q => isAll || (!isAll && q.SUBAREA_IS_ACTIVE));

            if (areaId.HasValue)
            {
                query = query.Where(q => q.TB_M_AREA_SUBAREA.Any(s => s.AREA_ID == areaId.Value));
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q => q.SUBAREA_NAME.Contains(keyword));
            }

            return query.Select(item => new SubAreaItemEntity
            {
                SubAreaId = item.SUBAREA_ID,
                SubAreaName = item.SUBAREA_NAME
            }).OrderBy(q => q.SubAreaName).Take(limit).ToList();
        }

        public List<SubAreaItemEntity> AutoCompleteSearchSubAreaOnMapping(string keyword, int? campaignServiceId, int? areaId, int? typeId, int limit)
        {
            return (from q in _context.TB_M_MAP_PRODUCT
                    where
                        q.MAP_PRODUCT_IS_ACTIVE
                        && q.TB_M_SUBAREA.SUBAREA_IS_ACTIVE
                        && (string.IsNullOrEmpty(keyword) || q.TB_M_SUBAREA.SUBAREA_NAME.Contains(keyword))
                        &&
                        (
                            !campaignServiceId.HasValue
                            ||
                            (
                                campaignServiceId.HasValue
                                && q.CAMPAIGNSERVICE_ID == campaignServiceId.Value
                            )
                            ||
                            (
                                campaignServiceId.HasValue
                                && !q.CAMPAIGNSERVICE_ID.HasValue
                                && q.TB_R_PRODUCT.TB_R_CAMPAIGNSERVICE.Any(c => c.CAMPAIGNSERVICE_ID == campaignServiceId.Value)
                            )
                        )
                        && (!areaId.HasValue || (areaId.HasValue && q.AREA_ID == areaId.Value))
                        && (!typeId.HasValue || (typeId.HasValue && q.TYPE_ID == typeId.Value))
                    orderby q.TB_M_SUBAREA.SUBAREA_NAME
                    select new SubAreaItemEntity
                    {
                        SubAreaId = q.SUBAREA_ID,
                        SubAreaName = q.TB_M_SUBAREA.SUBAREA_NAME
                    }).Distinct().OrderBy(q => q.SubAreaName).Take(limit).ToList();
        }

        public decimal? GetNextSubAreaCode()
        {
            decimal? next = (from a in _context.TB_M_SUBAREA.AsNoTracking()
                             orderby a.SUBAREA_CODE descending
                             select a.SUBAREA_CODE).Take(1).FirstOrDefault();
            return next == null ? 1 : ++next;
        }

        #region "Functions"

        private static IQueryable<SubAreaItemEntity> SetSelectSubAreaListSort(IQueryable<SubAreaItemEntity> subAreaList, SelectSubAreaSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    default:
                        return subAreaList.OrderBy(a => a.SubAreaName);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    default:
                        return subAreaList.OrderByDescending(a => a.SubAreaName);
                }
            }
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
