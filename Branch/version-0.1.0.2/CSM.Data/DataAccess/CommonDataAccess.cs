using System;
using System.Collections.Generic;
using System.Linq;
using CSM.Common.Utilities;
using CSM.Entity;
using log4net;
using System.Globalization;
using System.Data;

namespace CSM.Data.DataAccess
{
    public class CommonDataAccess : ICommonDataAccess
    {
        private readonly CSMContext _context;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CommonDataAccess));

        public CommonDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        public List<FontEntity> GetFont()
        {
            var fontList = (from ft in _context.TB_C_FONT.AsNoTracking()
                           select new FontEntity
                           {
                               FontId = ft.FONT_ID,
                               FontName = ft.FONT_NAME
                           });

            return fontList.ToList<FontEntity>();
        }

        public IEnumerable<ConfigureUrlEntity> GetConfigureUrl(ConfigureUrlSearchFilter searchFilter)
        {
            var urlList = (from cf in _context.TB_M_CONFIG_URL.AsNoTracking()
                           join mu in _context.TB_C_MENU on cf.MENU_ID equals mu.MENU_ID
                           where (string.IsNullOrEmpty(searchFilter.SystemName) || cf.CONFIG_NAME.ToUpper().Contains(searchFilter.SystemName.ToUpper()))
                           && (string.IsNullOrEmpty(searchFilter.Url) || cf.CONFIG_URL.ToUpper().Contains(searchFilter.Url.ToUpper()))
                           && (searchFilter.Status == null || searchFilter.Status == Constants.ApplicationStatus.All || cf.STATUS == searchFilter.Status)
                           select new ConfigureUrlEntity
                           {
                               ConfigureUrlId = cf.CONFIG_URL_ID,
                               SystemName = cf.CONFIG_NAME,
                               Url = cf.CONFIG_URL,
                               ImageFile = cf.IMAGE,
                               Status = cf.STATUS,
                               Menu = mu != null ? new MenuEntity
                               {
                                   MenuId = mu.MENU_ID,
                                   MenuName = mu.MENU_NAME
                               } : null,
                               Roles = (from r in _context.TB_M_CONFIG_ROLE
                                        where r.CONFIG_URL_ID == cf.CONFIG_URL_ID
                                        select new RoleEntity
                                        {
                                            RoleName = r.TB_C_ROLE.ROLE_NAME,
                                            RoleId = r.TB_C_ROLE.ROLE_ID
                                        }).ToList()
                           });

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = urlList.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }
            urlList = SetConfigureUrlSort(urlList, searchFilter);
            return urlList.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<ConfigureUrlEntity>();
        }

        public ConfigureUrlEntity GetConfigureUrlById(int configUrlId)
        {
            var query = from cf in _context.TB_M_CONFIG_URL
                        join sc in _context.TB_C_MENU on cf.MENU_ID equals sc.MENU_ID
                        from cs in _context.TB_R_USER.Where(o => o.USER_ID == cf.CREATE_USER).DefaultIfEmpty()
                        from us in _context.TB_R_USER.Where(o => o.USER_ID == cf.UPDATE_USER).DefaultIfEmpty()
                        where cf.CONFIG_URL_ID == configUrlId
                        select new ConfigureUrlEntity
                        {
                            ConfigureUrlId = cf.CONFIG_URL_ID,
                            SystemName = cf.CONFIG_NAME,
                            Url = cf.CONFIG_URL,
                            ImageFile = cf.IMAGE,
                            Status = cf.STATUS,
                            FontName = cf.FONT_NAME,
                            Menu = sc != null ? new MenuEntity
                            {
                                MenuId = sc.MENU_ID,
                                MenuName = sc.MENU_NAME
                            } : null,
                            Roles = (from r in _context.TB_M_CONFIG_ROLE
                                     where r.CONFIG_URL_ID == cf.CONFIG_URL_ID
                                     select new RoleEntity
                                     {
                                         RoleName = r.TB_C_ROLE.ROLE_NAME,
                                         RoleId = r.TB_C_ROLE.ROLE_ID
                                     }).ToList(),
                            CreatedDate = cf.CREATE_DATE,
                            Updatedate = cf.UPDATE_DATE,
                            CreateUser = cs != null ? new UserEntity
                            {
                                Firstname = cs.FIRST_NAME,
                                Lastname = cs.LAST_NAME,
                                PositionCode = cs.POSITION_CODE
                            } : null,
                            UpdateUser = us != null ? new UserEntity
                            {
                                Firstname = us.FIRST_NAME,
                                Lastname = us.LAST_NAME,
                                PositionCode = us.POSITION_CODE
                            } : null
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        public bool IsDuplicateConfigureUrl(ConfigureUrlEntity configUrlEntity)
        {

            var cnt = _context.TB_M_CONFIG_URL.Where(
                            x => x.CONFIG_NAME.ToUpper() == configUrlEntity.SystemName.ToUpper()
                                 && x.CONFIG_URL.ToUpper() == configUrlEntity.Url.ToUpper()
                                 && x.MENU_ID == configUrlEntity.Menu.MenuId
                                 && x.CONFIG_URL_ID != configUrlEntity.ConfigureUrlId
                                 ).Count();

            return cnt > 0;

        }

        public bool SaveConfigureUrl(ConfigureUrlEntity configUrlEntity)
        {
            var today = DateTime.Now;

            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        TB_M_CONFIG_URL dbConfigUrl = null;

                        if (configUrlEntity.ConfigureUrlId == 0)
                        {
                            dbConfigUrl = new TB_M_CONFIG_URL();
                            dbConfigUrl.CONFIG_URL_ID = configUrlEntity.ConfigureUrlId;
                            dbConfigUrl.CONFIG_NAME = configUrlEntity.SystemName;
                            dbConfigUrl.CONFIG_URL = configUrlEntity.Url;
                            dbConfigUrl.IMAGE = configUrlEntity.ImageFile; //path
                            dbConfigUrl.STATUS = configUrlEntity.Status;
                            dbConfigUrl.CREATE_USER = configUrlEntity.CreateUser.UserId;
                            dbConfigUrl.UPDATE_USER = configUrlEntity.UpdateUser.UserId;
                            dbConfigUrl.CREATE_DATE = today;
                            dbConfigUrl.UPDATE_DATE = today;
                            dbConfigUrl.MENU_ID = configUrlEntity.Menu.MenuId;
                            dbConfigUrl.FONT_NAME = configUrlEntity.FontName;

                            _context.TB_M_CONFIG_URL.Add(dbConfigUrl);
                            this.Save();

                            // Save Config Role
                            if (configUrlEntity.Roles != null && configUrlEntity.Roles.Count > 0)
                            {
                                this.SaveConfigurationRole(dbConfigUrl.CONFIG_URL_ID, configUrlEntity);
                            }
                        }
                        else
                        {
                            //Edit
                            dbConfigUrl =
                                _context.TB_M_CONFIG_URL.FirstOrDefault(
                                    x => x.CONFIG_URL_ID == configUrlEntity.ConfigureUrlId);
                            if (dbConfigUrl != null)
                            {

                                if (!string.IsNullOrWhiteSpace(configUrlEntity.ImageFile))
                                {
                                    var prevFile =
                                        StreamDataHelpers.GetApplicationPath(string.Format(CultureInfo.InvariantCulture,"{0}{1}",
                                            Constants.ConfigUrlPath,
                                            dbConfigUrl.IMAGE));

                                    if (StreamDataHelpers.TryToDelete(prevFile))
                                    {
                                        // Save new file
                                        dbConfigUrl.IMAGE = configUrlEntity.ImageFile;
                                    }
                                    else
                                    {
                                        var newFile =
                                            StreamDataHelpers.GetApplicationPath(string.Format(CultureInfo.InvariantCulture,"{0}{1}",
                                                Constants.ConfigUrlPath, configUrlEntity.ImageFile));
                                        StreamDataHelpers.TryToDelete(newFile);
                                        throw new CustomException(string.Format(CultureInfo.InvariantCulture,"Could not delete existing file : {0}",
                                            prevFile));
                                    }
                                }

                                dbConfigUrl.CONFIG_NAME = configUrlEntity.SystemName;
                                dbConfigUrl.CONFIG_URL = configUrlEntity.Url;
                                dbConfigUrl.STATUS = configUrlEntity.Status;
                                dbConfigUrl.UPDATE_USER = configUrlEntity.UpdateUser.UserId;
                                dbConfigUrl.UPDATE_DATE = today;
                                dbConfigUrl.MENU_ID = configUrlEntity.Menu.MenuId;
                                dbConfigUrl.FONT_NAME = configUrlEntity.FontName;

                                SetEntryStateModified(dbConfigUrl);
                                this.Save();

                                if (configUrlEntity.Roles != null && configUrlEntity.Roles.Count > 0)
                                {
                                    this.SaveConfigurationRole(dbConfigUrl.CONFIG_URL_ID, configUrlEntity);
                                }
                            }
                            else
                            {
                                Logger.ErrorFormat("CONFIG_URL_ID: {0} does not exist",
                                    configUrlEntity.ConfigureUrlId);
                            }
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

        private void SaveConfigurationRole(int configUrlId, ConfigureUrlEntity configUrlEntity)
        {
            var today = DateTime.Now;
            foreach (RoleEntity role in configUrlEntity.Roles)
            {
                TB_M_CONFIG_ROLE configRole = _context.TB_M_CONFIG_ROLE.FirstOrDefault(x => x.CONFIG_URL_ID == configUrlId
                        && x.ROLE_ID == role.RoleId);

                if (configRole == null && role.IsDelete == false)
                {
                    configRole = new TB_M_CONFIG_ROLE();
                    configRole.CONFIG_URL_ID = configUrlId;
                    configRole.ROLE_ID = role.RoleId;
                    configRole.CREATE_USER = configUrlEntity.CreateUser.UserId;
                    configRole.CREATE_DATE = today;
                    _context.TB_M_CONFIG_ROLE.Add(configRole);
                }

                if (configRole != null && role.IsDelete == true)
                {
                    _context.TB_M_CONFIG_ROLE.Remove(configRole);
                }
            }

            this.Save();
        }

        public IQueryable<ScreenEntity> GetAllScreen()
        {
            var query = from sc in _context.TB_C_SCREEN
                        select new ScreenEntity()
                        {
                            ScreenId = sc.SCREEN_ID,
                            ScreenName = sc.SCREEN_NAME
                        };

            return query;
        }

        public IQueryable<MenuEntity> GetAllMenu()
        {
            var query = from mu in _context.TB_C_MENU
                        select new MenuEntity
                        {
                            MenuId = mu.MENU_ID,
                            MenuName = mu.MENU_NAME
                        };

            return query;
        }

        public List<RoleEntity> GetAllRole()
        {
            var query = (from rl in _context.TB_C_ROLE
                         select new RoleEntity
                         {
                             RoleId = rl.ROLE_ID,
                             RoleName = rl.ROLE_NAME
                         });

            return query.Any() ? query.ToList() : null;
        }

        public IEnumerable<BranchEntity> GetBranchList(BranchSearchFilter searchFilter)
        {
            var ignoreList = searchFilter.Branches.Where(x => x.IsDelete == false).Select(x => x.BranchId);
            var brList = (from b in _context.TB_R_BRANCH
                          where (string.IsNullOrEmpty(searchFilter.BranchName) || b.BRANCH_NAME.ToUpper().Contains(searchFilter.BranchName.ToUpper()))
                            && !ignoreList.Contains(b.BRANCH_ID)
                          select new BranchEntity
                          {
                              BranchId = b.BRANCH_ID,
                              BranchName = b.BRANCH_NAME
                          }).AsQueryable<BranchEntity>();

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = brList.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            brList = SetBranchListSort(brList, searchFilter);
            return brList.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<BranchEntity>();
        }

        public List<BranchEntity> GetBranchesByName(string searchTerm, int pageSize, int pageNum)
        {
            return GetBranchQueryByName(searchTerm).OrderBy(x => x.BranchName)
                .Skip(pageSize * (pageNum - 1))
                .Take(pageSize)
                .ToList();
        }

        public int GetBranchCountByName(string searchTerm, int pageSize, int pageNum)
        {
            return GetBranchQueryByName(searchTerm).Count();
        }

        public List<BranchEntity> GetBranchesByName(string searchTerm, int pageSize, int pageNum, int? userId)
        {
            return GetBranchQueryByName(searchTerm, userId).OrderBy(x => x.BranchName)
                .Skip(pageSize * (pageNum - 1))
                .Take(pageSize)
                .ToList();
        }

        // And the total count of records
        public int GetBranchCountByName(string searchTerm, int pageSize, int pageNum, int? userId)
        {
            return GetBranchQueryByName(searchTerm, userId).Count();
        }

        public List<UserEntity> GetActionByName(string searchTerm, int pageSize, int pageNum, int? branchId)
        {
            var ci = ApplicationHelpers.GetCultureInfo(Constants.KnownCulture.Thai);
            var query = GetActionQueryByName(searchTerm, branchId).OrderBy(x => x.UserId).Skip(pageSize * (pageNum - 1)).Take(pageSize).ToList();
            return query.OrderBy(x => x.FullName, StringComparer.Create(ci, true)).ToList();
        }

        public int GetActionCountByName(string searchTerm, int pageSize, int pageNum, int? branchId)
        {
            return GetActionQueryByName(searchTerm, branchId).Count();
        }

        public List<DocumentTypeEntity> GetActiveDocumentTypes(int documentCategory)
        {
            var query = from dc in _context.TB_M_DOCUMENT_TYPE
                        where dc.STATUS == Constants.ApplicationStatus.Active
                        && dc.CATEGORY == documentCategory
                        select new DocumentTypeEntity
                        {
                            DocTypeId = dc.DOCUMENT_TYPE_ID,
                            Code = dc.DOCUMENT_TYPE_CODE,
                            Name = dc.DOCUMENT_TYPE_NAME,
                            Status = dc.STATUS
                        };

            return query.Any() ? query.ToList() : null;
        }

        public List<DocumentTypeEntity> GetDocumentTypeList(List<AttachmentTypeEntity> attachTypes, int documentCategory)
        {
            var query = from dc in _context.TB_M_DOCUMENT_TYPE.ToArray()
                        from tp in attachTypes.Where(x => x.DocTypeId == dc.DOCUMENT_TYPE_ID).DefaultIfEmpty()
                        where dc.STATUS == Constants.ApplicationStatus.Active
                        && dc.CATEGORY == documentCategory
                        select new DocumentTypeEntity
                        {
                            DocTypeId = dc.DOCUMENT_TYPE_ID,
                            Code = dc.DOCUMENT_TYPE_CODE,
                            Name = dc.DOCUMENT_TYPE_NAME,
                            Status = dc.STATUS,
                            IsChecked = (tp != null && tp.IsDelete != true) ? true : false
                        };

            return query.Any() ? query.ToList() : null;
        }

        public List<ParameterEntity> GetAllParameters()
        {
            var query = from x in _context.TB_C_PARAMETER
                        select new ParameterEntity
                        {
                            ParamId = x.PARAMETER_ID,
                            ParamName = x.PARAMETER_NAME,
                            ParamDesc = x.PARAMETER_DESC,
                            ParamValue = x.PARAMETER_VALUE
                        };

            return query.ToList();
        }

        public List<TitleEntity> GetActiveTitle()
        {
            var query = from t in _context.TB_M_TITLE
                        //where t.STATUS == Constants.ApplicationStatus.Active
                        orderby t.TITLE_NAME ascending
                        select new TitleEntity
                        {
                            TitleId = t.TITLE_ID,
                            TitleName = t.TITLE_NAME,
                            Language = t.LANGUAGE
                        };

            return query.Any() ? query.ToList() : null;
        }

        public List<SubscriptTypeEntity> GetActiveSubscriptType()
        {
            var query = from t in _context.TB_M_SUBSCRIPT_TYPE
                        //where t.STATUS == Constants.ApplicationStatus.Active
                        orderby t.SUBSCRIPT_TYPE_NAME ascending
                        select new SubscriptTypeEntity
                        {
                            SubscriptTypeId = t.SUBSCRIPT_TYPE_ID,
                            SubscriptTypeName = t.SUBSCRIPT_TYPE_NAME
                        };

            return query.Any() ? query.ToList() : null;
        }

        public List<PhoneTypeEntity> GetActivePhoneType()
        {
            var query = from t in _context.TB_M_PHONE_TYPE
                        where t.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax
                        //&& t.STATUS == Constants.ApplicationStatus.Active                        
                        select new PhoneTypeEntity
                        {
                            PhoneTypeId = t.PHONE_TYPE_ID,
                            PhoneTypeName = t.PHONE_TYPE_NAME
                        };

            return query.Any() ? query.ToList() : null;
        }

        public SubscriptTypeEntity GetSubscriptTypeByCode(string subscriptTypeCode)
        {
            var query = from t in _context.TB_M_SUBSCRIPT_TYPE
                        where t.SUBSCRIPT_TYPE_CODE == subscriptTypeCode
                        select new SubscriptTypeEntity
                        {
                            SubscriptTypeId = t.SUBSCRIPT_TYPE_ID,
                            SubscriptTypeName = t.SUBSCRIPT_TYPE_NAME,
                            SubscriptTypeCode = t.SUBSCRIPT_TYPE_CODE
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        public PhoneTypeEntity GetPhoneTypeByCode(string phoneTypeCode)
        {
            var query = from t in _context.TB_M_PHONE_TYPE
                        where t.PHONE_TYPE_CODE == phoneTypeCode
                        select new PhoneTypeEntity
                        {
                            PhoneTypeId = t.PHONE_TYPE_ID,
                            PhoneTypeName = t.PHONE_TYPE_NAME,
                            PhoneTypeCode = t.PHONE_TYPE_CODE
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        public List<MenuEntity> GetMenuList()
        {
            var menuItems = (from mn in _context.TB_C_MENU.ToArray()
                             select new MenuEntity
                             {
                                 MenuId = mn.MENU_ID,
                                 MenuCode = mn.MENU_CODE,
                                 MenuName = mn.MENU_NAME,
                                 ControllerName = mn.CONTROLLER_NAME,
                                 ActionName = mn.ACTION_NAME,
                                 CssClass = mn.CSS_CLASS,
                                 ConfigUrlList = this.GetActiveConfigUrlListByMenuCode(mn.MENU_CODE)
                             }).ToList();

            return menuItems;
        }

        public List<ScreenEntity> GetScreenList()
        {
            try
            {
                var query = from sc in _context.TB_C_SCREEN
                    select new ScreenEntity
                    {
                        MenuId = sc.MENU_ID,
                        ScreenId = sc.SCREEN_ID,
                        ScreenCode = sc.SCREEN_CODE,
                        ScreenName = sc.SCREEN_NAME,
                        ActionName = sc.ACTION_NAME,
                        ControllerName = sc.CONTROLLER_NAME,
                        Roles = (from rs in _context.TB_C_ROLE_SCREEN
                            join rl in _context.TB_C_ROLE on rs.ROLE_ID equals rl.ROLE_ID
                            where rs.SCREEN_ID == sc.SCREEN_ID
                            select new RoleEntity
                            {
                                RoleId = rl.ROLE_ID,
                                RoleName = rl.ROLE_NAME,
                                RoleValue = rl.ROLE_VALUE ?? 0
                            }).ToList()
                    };

                return query.Any() ? query.ToList() : null;
            }
            catch
            {
                return null;
            }
        }

        public List<RelationshipEntity> GetActiveRelationship()
        {
            var query = from t in _context.TB_M_RELATIONSHIP
                        where t.STATUS == Constants.ApplicationStatus.Active
                        orderby t.RELATIONSHIP_NAME ascending
                        select new RelationshipEntity
                        {
                            RelationshipId = t.RELATIONSHIP_ID,
                            RelationshipName = t.RELATIONSHIP_NAME
                        };

            return query.Any() ? query.ToList() : null;
        }

        public bool SaveShowhidePanel(int expand, int userId, string currentPage)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                bool isSelected;
                if (expand == 1) { isSelected = true; }
                else { isSelected = false; }

                var defaultSearch = _context.TB_T_DEFAULT_SEARCH.FirstOrDefault(x => x.USER_ID == userId && x.SEARCH_PAGE == currentPage);
                if (defaultSearch != null)
                {
                    defaultSearch.IS_SELECTED = !defaultSearch.IS_SELECTED;

                    SetEntryStateModified(defaultSearch);
                    this.Save();
                }
                else
                {
                    TB_T_DEFAULT_SEARCH defSearch = new TB_T_DEFAULT_SEARCH();

                    defSearch.SEARCH_PAGE = currentPage; //Constants.Page.CommunicationPage;
                    defSearch.IS_SELECTED = isSelected;
                    defSearch.USER_ID = userId;

                    _context.TB_T_DEFAULT_SEARCH.Add(defSearch);
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

        public DefaultSearchEntity GetShowhidePanelByUserId(int userId, string currentPage)
        {
            var query = from ds in _context.TB_T_DEFAULT_SEARCH
                        where ds.USER_ID == userId
                        && ds.SEARCH_PAGE == currentPage
                        select new DefaultSearchEntity
                        {
                            IsSelectd = ds.IS_SELECTED,
                            SearchPage = ds.SEARCH_PAGE,
                            UserId = ds.USER_ID
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        public int GetNextAttachmentSeq()
        {
            int nextSeq = 0;

            System.Data.Entity.Core.Objects.ObjectParameter param = new System.Data.Entity.Core.Objects.ObjectParameter("NextSeq", typeof(int));
            _context.SP_GET_NEXT_ATTACHMENT_SEQ(param);
            nextSeq = (int)param.Value;

            return nextSeq;
        }

        public int GetNextServiceRequestSeq()
        {
            int nextSeq = 0;

            System.Data.Entity.Core.Objects.ObjectParameter param = new System.Data.Entity.Core.Objects.ObjectParameter("NextSeq", typeof(int));
            _context.SP_GET_NEXT_SR_SEQ(param);
            nextSeq = (int)param.Value;

            return nextSeq;
        }

        public List<string> GetExceptionErrorCodes(string errorSystem, string errorService)
        {
            var query = _context.TB_C_ERROR.Where(x => x.ERROR_SYSTEM.ToUpper() == errorSystem.ToUpper()
                && x.ERROR_SERVICE.ToUpper() == errorService.ToUpper() && x.IS_INQUIRY == true).Select(x => x.ERROR_CODE);
            return query.Any() ? query.ToList() : null;
        }

        public BranchEntity GetBranchById(int branchId)
        {
            var query = from br in _context.TB_R_BRANCH
                        where br.BRANCH_ID == branchId
                        select new BranchEntity
                        {
                            BranchId = br.BRANCH_ID,
                            BranchCode = br.BRANCH_CODE,
                            BranchName = br.BRANCH_NAME
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        #region "Functions"

        private List<ConfigureUrlEntity> GetActiveConfigUrlListByMenuCode(string menuCode)
        {
            var query = from cf in _context.TB_M_CONFIG_URL
                        join mn in _context.TB_C_MENU on cf.MENU_ID equals mn.MENU_ID
                        where mn.MENU_CODE.ToUpper().Equals(menuCode.ToUpper()) && cf.STATUS == Constants.ApplicationStatus.Active
                        select new ConfigureUrlEntity
                        {
                            ConfigureUrlId = cf.CONFIG_URL_ID,
                            SystemName = cf.CONFIG_NAME,
                            Url = cf.CONFIG_URL,
                            ImageFile = cf.IMAGE,
                            Status = cf.STATUS,
                            FontName = cf.FONT_NAME,
                            Menu = mn != null ? new MenuEntity
                            {
                                MenuId = mn.MENU_ID,
                                MenuName = mn.MENU_NAME
                            } : null,
                            Roles = (from r in _context.TB_M_CONFIG_ROLE
                                     where r.CONFIG_URL_ID == cf.CONFIG_URL_ID
                                     select new RoleEntity
                                     {
                                         RoleName = r.TB_C_ROLE.ROLE_NAME,
                                         RoleId = r.TB_C_ROLE.ROLE_ID,
                                         RoleValue = r.TB_C_ROLE.ROLE_VALUE ?? 0
                                     }).ToList()
                        };

            return query.Any() ? query.ToList() : null;
        }

        private static IQueryable<ConfigureUrlEntity> SetConfigureUrlSort(IQueryable<ConfigureUrlEntity> urlList, ConfigureUrlSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "SYSTEMNAME":
                        return urlList.OrderBy(ord => ord.SystemName);
                    case "URL":
                        return urlList.OrderBy(ord => ord.Url);
                    case "STATUS":
                        return urlList.OrderBy(ord => (ord.Status == 1) ? "A" : "I");
                    default:
                        return urlList.OrderBy(ord => ord.ConfigureUrlId);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "SYSTEMNAME":
                        return urlList.OrderByDescending(ord => ord.SystemName);
                    case "URL":
                        return urlList.OrderByDescending(ord => ord.Url);
                    case "STATUS":
                        return urlList.OrderByDescending(ord => (ord.Status == 1) ? "A" : "I");
                    default:
                        return urlList.OrderByDescending(ord => ord.ConfigureUrlId);
                }
            }
        }

        private static IQueryable<BranchEntity> SetBranchListSort(IQueryable<BranchEntity> brList, BranchSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "BRANCHNAME":
                        return brList.OrderBy(ord => ord.BranchName);
                    default:
                        return brList.OrderBy(ord => ord.BranchId);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "BRANCHNAME":
                        return brList.OrderByDescending(ord => ord.BranchName);
                    default:
                        return brList.OrderByDescending(ord => ord.BranchId);
                }
            }
        }

        private IQueryable<BranchEntity> GetBranchQueryByName(string searchTerm)
        {
            var query = from br in _context.TB_R_BRANCH.AsNoTracking()
                        where br.STATUS == Constants.ApplicationStatus.Active
                              && (string.IsNullOrEmpty(searchTerm) || br.BRANCH_NAME.Contains(searchTerm))
                        select new BranchEntity
                        {
                            BranchId = br.BRANCH_ID,
                            BranchName = br.BRANCH_NAME
                        };
            return query;
        }

        private IQueryable<BranchEntity> GetBranchQueryByName(string searchTerm, int? userId)
        {
            var query = from br in _context.TB_R_BRANCH.AsNoTracking()
                        from ur in _context.TB_R_USER.Where(x => x.BRANCH_ID == br.BRANCH_ID).DefaultIfEmpty()
                        where br.STATUS == Constants.ApplicationStatus.Active
                              && (string.IsNullOrEmpty(searchTerm) || br.BRANCH_NAME.Contains(searchTerm))
                              && (userId == null || ur.USER_ID == userId)
                        group br.BRANCH_ID by br into g
                        select new BranchEntity
                        {
                            BranchId = g.Key.BRANCH_ID,
                            BranchName = g.Key.BRANCH_NAME
                        };
            return query;
        }

        private IQueryable<UserEntity> GetActionQueryByName(string searchTerm, int? branchId)
        {
            var userList = from ur in _context.TB_R_USER.AsNoTracking()
                           from br in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == ur.BRANCH_ID).DefaultIfEmpty()
                           where (branchId == null || br.BRANCH_ID == branchId)
                           select new
                           {
                               UserId = ur.USER_ID,
                               Firstname = ur.FIRST_NAME,
                               Lastname = ur.LAST_NAME,
                               PositionCode = ur.POSITION_CODE,
                               FullName = ur.POSITION_CODE + " - " + ur.FIRST_NAME + " " + ur.LAST_NAME
                           };

            var query = from ur in userList
                        where (string.IsNullOrEmpty(searchTerm) || ur.FullName.Contains(searchTerm))
                        group ur.UserId by ur into g
                        select new UserEntity
                        {
                            UserId = g.Key.UserId,
                            Firstname = g.Key.Firstname,
                            Lastname = g.Key.Lastname,
                            PositionCode = g.Key.PositionCode
                        };

            return query;
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
