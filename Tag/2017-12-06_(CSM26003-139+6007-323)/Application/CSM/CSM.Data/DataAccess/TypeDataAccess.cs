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
    public class TypeDataAccess : ITypeDataAccess
    {
        private readonly CSMContext _context;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TypeDataAccess));

        public TypeDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        #region "Type"

        public IEnumerable<TypeItemEntity> GetTypeList(TypeSearchFilter searchFilter)
        {
            var typeStatus = searchFilter.Status == "all" ? null : searchFilter.Status.ToNullable<bool>();
            var resultQuery = (from type in _context.TB_M_TYPE
                               from createUser in _context.TB_R_USER.Where(x => x.USER_ID == type.CREATE_USER).DefaultIfEmpty()
                               from updateUser in _context.TB_R_USER.Where(x => x.USER_ID == type.UPDATE_USER).DefaultIfEmpty()
                               where ((searchFilter.TypeName == null || type.TYPE_NAME.Contains(searchFilter.TypeName)) &&
                                      (!typeStatus.HasValue || type.TYPE_IS_ACTIVE == typeStatus)
                                      && (searchFilter.TypeCode == null || SqlFunctions.StringConvert(type.TYPE_CODE).Contains(searchFilter.TypeCode)))
                               select new TypeItemEntity
                               {
                                   TypeId = type.TYPE_ID,
                                   TypeName = type.TYPE_NAME,
                                   TypeCode = SqlFunctions.StringConvert(type.TYPE_CODE),
                                   Status = type.TYPE_IS_ACTIVE,
                                   UpdateUserName = (updateUser != null ? new UserEntity
                                   {
                                       PositionCode = updateUser.POSITION_CODE,
                                       Firstname = updateUser.FIRST_NAME,
                                       Lastname = updateUser.LAST_NAME
                                   } : null),
                                   CreateUserName = (createUser != null ? new UserEntity
                                   {
                                       PositionCode = createUser.POSITION_CODE,
                                       Firstname = createUser.FIRST_NAME,
                                       Lastname = createUser.LAST_NAME
                                   } : null),
                                   UpdateDate = type.UPDATE_DATE.HasValue ? type.UPDATE_DATE : type.CREATE_DATE,
                               });
            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = resultQuery.Count();

            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            resultQuery = SetTypeListSort(resultQuery, searchFilter);
            return resultQuery.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();
        }

        public bool SaveType(TypeItemEntity typeItemEntity)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            var now = DateTime.Now;

            try
            {
                TB_M_TYPE type;
                if (!typeItemEntity.TypeId.HasValue)
                {
                    //save
                    type = new TB_M_TYPE();
                    type.TYPE_NAME = typeItemEntity.TypeName;
                    //type.TYPE_CODE = Convert.ToDecimal(typeItemEntity.TypeCode);
                    type.TYPE_CODE = GetNextTypeCode();
                    type.TYPE_IS_ACTIVE = typeItemEntity.Status;
                    type.CREATE_USER = typeItemEntity.UserId;
                    type.CREATE_DATE = now;
                    type.UPDATE_USER = typeItemEntity.UserId;
                    type.UPDATE_DATE = now;

                    _context.TB_M_TYPE.Add(type);
                }
                else
                {
                    //save
                    type = _context.TB_M_TYPE.SingleOrDefault(t => t.TYPE_ID == typeItemEntity.TypeId.Value);
                    type.TYPE_NAME = typeItemEntity.TypeName;
                    //type.TYPE_CODE = Convert.ToDecimal(typeItemEntity.TypeCode);
                    type.TYPE_IS_ACTIVE = typeItemEntity.Status;
                    type.UPDATE_USER = typeItemEntity.UserId;
                    type.UPDATE_DATE = now;

                    SetEntryStateModified(type);
                }
                Save();

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

        #endregion

        public bool CheckTypeName(TypeItemEntity typeItemEntity)
        {
            var typeName = typeItemEntity.TypeName;


            if (!typeItemEntity.TypeId.HasValue)
            {
                var query = _context.TB_M_TYPE.Where(x => x.TYPE_NAME.ToUpper().Equals(typeName.ToUpper()));
                var count = query.Count();
                if (count > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                var query = _context.TB_M_TYPE.Where(x => x.TYPE_NAME.ToUpper().Equals(typeName.ToUpper()) && x.TYPE_ID != typeItemEntity.TypeId);
                var count = query.Count();
                if (count > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }                
            }
        }
        
        public TypeItemEntity GetTypeById(int typeId)
        {
            var query = from tp in _context.TB_M_TYPE
                        from createUser in _context.TB_R_USER.Where(x => x.USER_ID == tp.CREATE_USER).DefaultIfEmpty()
                        from updateUser in _context.TB_R_USER.Where(x => x.USER_ID == tp.UPDATE_USER).DefaultIfEmpty()
                        where tp.TYPE_ID == typeId
                        select new TypeItemEntity
                        {
                            TypeId = tp.TYPE_ID,
                            Status = tp.TYPE_IS_ACTIVE,
                            TypeName = tp.TYPE_NAME,
                            TypeCode = tp.TYPE_CODE.ToString(),
                            CreateDate = tp.CREATE_DATE,
                            UpdateDate = tp.UPDATE_DATE,
                            CreateUserName = (createUser != null ? new UserEntity
                            {
                                PositionCode = createUser.POSITION_CODE,
                                Firstname = createUser.FIRST_NAME,
                                Lastname = createUser.LAST_NAME
                            } : null),
                            UpdateUserName = (updateUser != null ? new UserEntity
                            {
                                PositionCode = updateUser.POSITION_CODE,
                                Firstname = updateUser.FIRST_NAME,
                                Lastname = updateUser.LAST_NAME
                            } : null)
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        public List<TypeItemEntity> AutoCompleteSearchType(string keyword, int limit, bool? isAllStatus)
        {
            var query = _context.TB_M_TYPE.AsQueryable();

            var isAll = (isAllStatus ?? false);

            query = query.Where(q => isAll || (!isAll && q.TYPE_IS_ACTIVE));

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q => q.TYPE_NAME.Contains(keyword));
            }

            return query.Select(item => new TypeItemEntity
            {
                TypeId = item.TYPE_ID,
                TypeName = item.TYPE_NAME,
            }).OrderBy(q => q.TypeName).Take(limit).ToList();
        }

        public List<TypeItemEntity> AutoCompleteSearchTypeOnMapping(string keyword, int? campaignServiceId, int? areaId, int? subAreaId, int limit)
        {
            return (from q in _context.TB_M_MAP_PRODUCT
                    where
                        q.MAP_PRODUCT_IS_ACTIVE
                        && q.TB_M_TYPE.TYPE_IS_ACTIVE
                        && (string.IsNullOrEmpty(keyword) || q.TB_M_TYPE.TYPE_NAME.Contains(keyword))
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
                        && (!subAreaId.HasValue || (subAreaId.HasValue && q.SUBAREA_ID == subAreaId.Value))
                    select new TypeItemEntity
                    {
                        TypeId = q.TYPE_ID,
                        TypeName = q.TB_M_TYPE.TYPE_NAME
                    }).Distinct().OrderBy(x => x.TypeName).Take(limit).ToList();
        }

        public decimal? GetNextTypeCode()
        {
            decimal? nextCode = (from a in _context.TB_M_TYPE.AsNoTracking()
                                 orderby a.TYPE_CODE descending
                                 select a.TYPE_CODE).Take(1).FirstOrDefault();
            return nextCode == null ? 1 : ++nextCode;
        }

        #region "Functions"

        private static IQueryable<TypeItemEntity> SetTypeListSort(IQueryable<TypeItemEntity> areaList, TypeSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "TypeName":
                        return areaList.OrderBy(a => a.TypeName);
                    default:
                        return areaList.OrderBy(a => a.TypeName);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "TypeName":
                        return areaList.OrderByDescending(a => a.TypeName);
                    default:
                        return areaList.OrderByDescending(a => a.TypeName);
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
