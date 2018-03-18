using System;
using System.Globalization;
using System.Linq;
using CSM.Common.Utilities;
using CSM.Entity;
using log4net;
using System.Collections.Generic;

namespace CSM.Data.DataAccess
{
    public class AuditLogDataAccess : IAuditLogDataAccess
    {
        private readonly CSMContext _context;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AuditLogDataAccess));
        private readonly string[] _ignoreModules = new string[] { Constants.Module.Authentication };
        private readonly string[] _ignoreActions = new string[] { Constants.AuditAction.Search, Constants.AuditAction.Login, Constants.AuditAction.Logout };

        public AuditLogDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        public void AddLog(AuditLogEntity auditlog)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                TB_L_AUDIT_LOG dbAuditLog = new TB_L_AUDIT_LOG
                {
                    MODULE = auditlog.Module,
                    ACTION = auditlog.Action,
                    IP_ADDRESS = auditlog.IpAddress,
                    STATUS = (short)auditlog.Status,
                    DETAIL = auditlog.Detail,
                    CREATE_DATE = DateTime.Now,
                    CREATE_USER = auditlog.CreateUserId
                };

                _context.TB_L_AUDIT_LOG.Add(dbAuditLog);
                Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public IEnumerable<AuditLogEntity> SearchAuditLogs(AuditLogSearchFilter searchFilter)
        {
            string selectAllValue = Constants.ApplicationStatus.All.ConvertToString();

            DateTime dateTo = DateTime.Now;
            if (searchFilter.DateToValue.HasValue)
            {
                dateTo = searchFilter.DateToValue.Value.AddDays(1);
            }

            var query = from au in _context.TB_L_AUDIT_LOG.AsNoTracking()
                        from us in _context.TB_R_USER.Where(o => o.USER_ID == au.CREATE_USER).DefaultIfEmpty()
                        where !string.IsNullOrEmpty(au.MODULE) && !_ignoreActions.Contains(au.MODULE)
                            && !string.IsNullOrEmpty(au.ACTION) && !_ignoreActions.Contains(au.ACTION)
                            && (string.IsNullOrEmpty(searchFilter.FirstName) || (us.FIRST_NAME.ToUpper().Contains(searchFilter.FirstName.ToUpper())))
                            && (string.IsNullOrEmpty(searchFilter.LastName) || (us.LAST_NAME.ToUpper().Contains(searchFilter.LastName.ToUpper())))
                            && (string.IsNullOrEmpty(searchFilter.Module) || selectAllValue == searchFilter.Module || au.MODULE.ToUpper().Contains(searchFilter.Module.ToUpper()))
                            && (string.IsNullOrEmpty(searchFilter.Action) || selectAllValue == searchFilter.Action || au.ACTION.ToUpper().Contains(searchFilter.Action.ToUpper()))
                            && (searchFilter.Status == null || searchFilter.Status == Constants.ApplicationStatus.All || au.STATUS == searchFilter.Status)
                            && (!searchFilter.DateFromValue.HasValue || au.CREATE_DATE >= searchFilter.DateFromValue.Value)
                            && (!searchFilter.DateToValue.HasValue || au.CREATE_DATE <= dateTo)
                        select new AuditLogEntity
                        {
                            AuditLogId = au.AUDIT_LOG_ID,
                            CreatedDate = au.CREATE_DATE,
                            IpAddress = au.IP_ADDRESS,
                            Action = au.ACTION,
                            Module = au.MODULE,
                            Detail = au.DETAIL,
                            Status = (LogStatus)au.STATUS,
                            User = (us != null ? new UserEntity
                            {
                                Firstname = us.FIRST_NAME,
                                Lastname = us.LAST_NAME,
                                PositionCode = us.POSITION_CODE
                            } : null)
                        };

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = query.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            query = SetAuditLogListSort(query, searchFilter);
            return query.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<AuditLogEntity>();
        }

        public IQueryable<MessageEntity> GetMessages(string culture)
        {
            var query = from x in _context.TB_C_ERROR.AsNoTracking()
                        select new MessageEntity
                        {
                            ErrorCode = x.ERROR_CODE,
                            ErrorService = x.ERROR_SERVICE,
                            ErrorSystem = x.ERROR_SYSTEM,
                            ErrorDesc = x.ERROR_DESC
                        };

            return query;
        }

        #region "Functions"

        public IQueryable<ModuleEntity> GetModule()
        {
            var query = from au in _context.TB_L_AUDIT_LOG
                        where !string.IsNullOrEmpty(au.MODULE) && !_ignoreModules.Contains(au.MODULE)
                        group au by au.MODULE into g
                        orderby g.Key ascending
                        select new ModuleEntity
                        {
                            ModuleId = g.Key,
                            ModuleName = g.Key
                        };

            return query;
        }

        public IQueryable<ActionEntity> GetAction()
        {
            var query = from au in _context.TB_L_AUDIT_LOG
                        where !string.IsNullOrEmpty(au.MODULE) && !_ignoreActions.Contains(au.MODULE)
                            && !string.IsNullOrEmpty(au.ACTION) && !_ignoreActions.Contains(au.ACTION)
                        group au by au.ACTION into g
                        orderby g.Key ascending
                        select new ActionEntity
                        {
                            ActionId = g.Key,
                            ActionName = g.Key
                        };

            return query;
        }

        public IQueryable<ActionEntity> GetActionByModule(string module)
        {
            if (module == "-1")
            {
                var query = from au in _context.TB_L_AUDIT_LOG
                            where !string.IsNullOrEmpty(au.MODULE) && !_ignoreActions.Contains(au.MODULE)
                                && !string.IsNullOrEmpty(au.ACTION) && !_ignoreActions.Contains(au.ACTION)
                            group au by au.ACTION into g
                            orderby g.Key ascending
                            select new ActionEntity
                            {
                                ActionId = g.Key,
                                ActionName = g.Key
                            };

                return query;
            }
            else
            {
                var query = from au in _context.TB_L_AUDIT_LOG
                            where !string.IsNullOrEmpty(au.MODULE) && !_ignoreActions.Contains(au.MODULE)
                                && au.MODULE.ToUpper().Equals(module.ToUpper())
                                && !string.IsNullOrEmpty(au.ACTION) && !_ignoreActions.Contains(au.ACTION)
                            group au by au.ACTION into g
                            orderby g.Key ascending
                            select new ActionEntity
                            {
                                ActionId = g.Key,
                                ActionName = g.Key
                            };

                return query;
            }
        }

        private static IQueryable<AuditLogEntity> SetAuditLogListSort(IQueryable<AuditLogEntity> auditlogList, AuditLogSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "CREATE DATE":
                        return auditlogList.OrderBy(ord => ord.CreatedDate);
                    case "CREATE USER":
                        return auditlogList.OrderBy(ord => ord.User.PositionCode).ThenBy(ord => ord.User.Firstname).ThenBy(ord => ord.User.Lastname);
                    case "IPADDRESS":
                        return auditlogList.OrderBy(ord => ord.IpAddress);
                    case "MODULE":
                        return auditlogList.OrderBy(ord => ord.Module);
                    case "ACTION":
                        return auditlogList.OrderBy(ord => ord.Action);
                    case "STATUS":
                        return auditlogList.OrderBy(ord => ord.Status);
                    case "DETAIL":
                        return auditlogList.OrderBy(ord => ord.Detail);
                    default:
                        return auditlogList.OrderBy(ord => ord.AuditLogId);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "CREATE DATE":
                        return auditlogList.OrderByDescending(ord => ord.CreatedDate);
                    case "CREATE USER":
                        return auditlogList.OrderByDescending(ord => ord.User.PositionCode).ThenByDescending(ord => ord.User.Firstname).ThenByDescending(ord => ord.User.Lastname);
                    case "IPADDRESS":
                        return auditlogList.OrderByDescending(ord => ord.IpAddress);
                    case "MODULE":
                        return auditlogList.OrderByDescending(ord => ord.Module);
                    case "ACTION":
                        return auditlogList.OrderByDescending(ord => ord.Action);
                    case "STATUS":
                        return auditlogList.OrderByDescending(ord => ord.Status);
                    case "DETAIL":
                        return auditlogList.OrderByDescending(ord => ord.Detail);
                    default:
                        return auditlogList.OrderByDescending(ord => ord.AuditLogId);
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
