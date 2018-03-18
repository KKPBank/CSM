using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using CSM.Common.Utilities;
using CSM.Entity;
using log4net;
using System.Globalization;

namespace CSM.Data.DataAccess
{
    public class AreaDataAccess : IAreaDataAccess
    {
        private readonly CSMContext _context;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AreaDataAccess));
        public AreaDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        #region "Area"

        public IEnumerable<AreaItemEntity> GetAreaList(AreaSearchFilter searchFilter)
        {
            var areaStatus = searchFilter.Status == "all" ? null : searchFilter.Status.ToNullable<bool>();
            var resultQuery = (from area in _context.TB_M_AREA
                from createUser in _context.TB_R_USER.Where(x => x.USER_ID == area.CREATE_USER).DefaultIfEmpty()
                from updateUser in _context.TB_R_USER.Where(x => x.USER_ID == area.UPDATE_USER).DefaultIfEmpty()
                where ((searchFilter.AreaName == null || area.AREA_NAME.Contains(searchFilter.AreaName)) &&
                       (!areaStatus.HasValue || area.AREA_IS_ACTIVE == areaStatus)
                       && (searchFilter.AreaCode == null || SqlFunctions.StringConvert(area.AREA_CODE).Contains(searchFilter.AreaCode)))
                select new AreaItemEntity
                {
                    AreaId = area.AREA_ID,
                    AreaName = area.AREA_NAME,
                    AreaCode = SqlFunctions.StringConvert(area.AREA_CODE),
                    IsActive = area.AREA_IS_ACTIVE ? "Active" : "Inactive",
                    UpdateUser = (updateUser != null ? new UserEntity()
                    {
                        PositionCode = updateUser.POSITION_CODE,
                        Firstname = updateUser.FIRST_NAME,
                        Lastname = updateUser.LAST_NAME
                    } : null),
                    CreateUser = (createUser != null ? new UserEntity()
                    {
                        PositionCode = createUser.POSITION_CODE,
                        Firstname = createUser.FIRST_NAME,
                        Lastname = createUser.LAST_NAME
                    } : null),
                    UpdateDate = area.UPDATE_DATE.HasValue ? area.UPDATE_DATE : area.CREATE_DATE,
                });

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = resultQuery.Count();

            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            resultQuery = SetAreaListSort(resultQuery, searchFilter);

            return resultQuery.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();
        }

        public List<AreaSubAreaItemEntity> GetSubAreaListById(int offset, int limit, int areaId, ref int totalCount)
        {
            var query = _context.TB_M_AREA_SUBAREA.AsQueryable();
            var areaSubAreaEntity = new AreaSubAreaEntity();

            query = query.Where(q => q.AREA_ID == areaId).OrderBy(q => q.TB_M_SUBAREA.SUBAREA_NAME);

            totalCount = query.Count();

            areaSubAreaEntity.AreaSubAreaList = query.Skip(offset).Take(limit).Select(item => new AreaSubAreaItemEntity
            {
                AreaSubAreaId = item.AREA_SUBAREA_ID,
                SubAreaId = item.SUBAREA_ID,
                SubAreaName = item.TB_M_SUBAREA.SUBAREA_NAME,
                IsActive = item.TB_M_SUBAREA.SUBAREA_IS_ACTIVE,
                CreateUserFirstName = item.TB_R_USER.FIRST_NAME,
                CreateUserLastName = item.TB_R_USER.LAST_NAME,
                CreateDate = item.CREATE_DATE
            }).ToList();

            return areaSubAreaEntity.AreaSubAreaList;
        }

        public AreaItemEntity GetArea(int areaId)
        {
            var areaItemEntity = (from area in _context.TB_M_AREA
                                  from createUser in _context.TB_R_USER.Where(x => x.USER_ID == area.CREATE_USER).DefaultIfEmpty()
                                  from updateUser in _context.TB_R_USER.Where(x => x.USER_ID == area.UPDATE_USER).DefaultIfEmpty()
                where (area.AREA_ID == areaId)
                select new AreaItemEntity()
                {
                    AreaId = area.AREA_ID,
                    AreaName = area.AREA_NAME,
                    AreaCode = area.AREA_CODE.ToString(),
                    Status = area.AREA_IS_ACTIVE,
                    UpdateUser = (updateUser != null ? new UserEntity()
                    {
                        PositionCode = updateUser.POSITION_CODE,
                        Firstname = updateUser.FIRST_NAME,
                        Lastname = updateUser.LAST_NAME
                    } : null),
                    CreateUser = (createUser != null ? new UserEntity()
                    {
                        PositionCode = createUser.POSITION_CODE,
                        Firstname = createUser.FIRST_NAME,
                        Lastname = createUser.LAST_NAME
                    } : null),
                    CreateDate = area.CREATE_DATE,
                    UpdateDate = area.UPDATE_DATE
                }).SingleOrDefault();

            return areaItemEntity;
        }

        public bool SaveArea(AreaItemEntity areaItemEntity, string idSubAreas)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var isEdit = areaItemEntity.AreaId.HasValue;
                TB_M_AREA area;

                if (!isEdit)
                {
                    //add
                    area = new TB_M_AREA();
                    area.AREA_CODE = GetNextAreaCode();
                }
                else
                {
                    area = _context.TB_M_AREA.SingleOrDefault(a => a.AREA_ID == areaItemEntity.AreaId.Value);
                    if (area == null)
                    {
                        Logger.ErrorFormat("SUBAREA ID: {0} does not exist", areaItemEntity.AreaId);
                        return false;
                    }
                }

                area.AREA_NAME = areaItemEntity.AreaName;
                //area.AREA_CODE = Convert.ToDecimal(areaItemEntity.AreaCode);
                area.AREA_IS_ACTIVE = areaItemEntity.Status;
                area.UPDATE_USER = areaItemEntity.UserId;
                area.UPDATE_DATE = DateTime.Now;

                if (!isEdit)
                {
                    area.CREATE_USER = areaItemEntity.UserId;
                    area.CREATE_DATE = DateTime.Now;
                    _context.TB_M_AREA.Add(area);
                    this.Save();

                    //save area_subarea
                    if (!string.IsNullOrEmpty(idSubAreas))
                    {
                        string[] idSubAreasArray = idSubAreas.Split(',');

                        foreach (var idSubArea in idSubAreasArray)
                        {
                            var areaSubArea = new TB_M_AREA_SUBAREA();
                            areaSubArea.AREA_ID = area.AREA_ID;
                            areaSubArea.SUBAREA_ID = Convert.ToInt32(idSubArea, CultureInfo.InvariantCulture);
                            areaSubArea.CREATE_USER = areaItemEntity.UserId;
                            areaSubArea.CREATE_DATE = DateTime.Now;
                            _context.TB_M_AREA_SUBAREA.Add(areaSubArea);
                            Save();
                        }
                    }
                }
                else
                {
                    SetEntryStateModified(area);

                    //delete area_subarea
                    var areaSubAreaList = _context.TB_M_AREA_SUBAREA.Where(a => a.AREA_ID == areaItemEntity.AreaId);
                    foreach (var areaSubAreaItem in areaSubAreaList)
                    {
                        _context.TB_M_AREA_SUBAREA.Remove(areaSubAreaItem);
                    }
                    this.Save();

                    //update area_subarea
                    if (!string.IsNullOrEmpty(idSubAreas))
                    {
                        string[] idSubAreasArray = idSubAreas.Split(',');

                        foreach (var idSubArea in idSubAreasArray)
                        {
                            var areaSubArea = new TB_M_AREA_SUBAREA();
                            areaSubArea.AREA_ID = area.AREA_ID;
                            areaSubArea.SUBAREA_ID = Convert.ToInt32(idSubArea, CultureInfo.InvariantCulture);
                            areaSubArea.CREATE_USER = areaItemEntity.UserId;
                            areaSubArea.CREATE_DATE = DateTime.Now;
                            _context.TB_M_AREA_SUBAREA.Add(areaSubArea);
                            this.Save();
                        }
                    }
                }

                this.Save();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                return false;
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public bool ValidateAreaName(int? areaId, string areaName)
        {
            if (!areaId.HasValue)
            {
                //validate add
                return _context.TB_M_AREA.Count(q => q.AREA_NAME == areaName) == 0;
            }
            else
            {
                //validate edit
                return _context.TB_M_AREA.Count(q => q.AREA_ID != areaId.Value && q.AREA_NAME == areaName) == 0;
            }
        }

        public List<AreaItemEntity> AutoCompleteSearchArea(string keyword, int? subAreaId, int limit, bool? isAllStatus)
        {
            var query = _context.TB_M_AREA.AsQueryable();
            
                var isAll = (isAllStatus ?? false);

            query = query.Where(q => isAll || (!isAll && q.AREA_IS_ACTIVE));

            if (subAreaId.HasValue)
            {
                query = query.Where(q => q.TB_M_AREA_SUBAREA.Any(s => s.SUBAREA_ID == subAreaId.Value));
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q => q.AREA_NAME.Contains(keyword));
            }

            query = query.OrderBy(q => q.AREA_NAME);

            return query.Take(limit).Select(item => new AreaItemEntity
            {
                AreaId = item.AREA_ID,
                AreaName = item.AREA_NAME,
            }).ToList();
        }

        public List<AreaItemEntity> AutoCompleteSearchAreaOnMapping(string keyword, int? campaignServiceId, int? subAreaId, int? typeId, int limit)
        {
            return (from q in _context.TB_M_MAP_PRODUCT
                    where
                        q.MAP_PRODUCT_IS_ACTIVE
                        && q.TB_M_AREA.AREA_IS_ACTIVE
                        && (string.IsNullOrEmpty(keyword) || q.TB_M_AREA.AREA_NAME.Contains(keyword))
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
                        && (!subAreaId.HasValue || (subAreaId.HasValue && q.SUBAREA_ID == subAreaId.Value))
                        && (!typeId.HasValue || (typeId.HasValue && q.TYPE_ID == typeId.Value))
                    select new AreaItemEntity
                    {
                        AreaId = q.AREA_ID,
                        AreaName = q.TB_M_AREA.AREA_NAME,
                    }).Distinct().OrderBy(x => x.AreaName).Take(10).ToList();
        }

        public decimal? GetNextAreaCode()
        {
            decimal? nextCode = (from a in _context.TB_M_AREA.AsNoTracking()
                                 orderby a.AREA_CODE descending
                                 select a.AREA_CODE).Take(1).FirstOrDefault();
            return nextCode == null ? 1 : ++nextCode;
        }

        #endregion

        #region "Functions"

        private static IQueryable<AreaItemEntity> SetAreaListSort(IQueryable<AreaItemEntity> areaList, AreaSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "AreaName":
                        return areaList.OrderBy(a => a.AreaName);
                    default:
                        return areaList.OrderBy(a => a.AreaName);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "AreaName":
                        return areaList.OrderByDescending(a => a.AreaName);
                    default:
                        return areaList.OrderByDescending(a => a.AreaName);
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
