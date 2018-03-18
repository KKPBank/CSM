using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CSM.Common.Resources;
using CSM.Common.Securities;
using CSM.Common.Utilities;
using CSM.Data.DataAccess;
using CSM.Entity;
using CSM.Service.Messages.Common;
using log4net;
using System.Globalization;

///<summary>
/// Class Name : CommonFacade
/// Purpose    : -
/// Author     : Neda Peyrone
///</summary>
///<remarks>
/// Change History:
/// Date         Author           Description
/// ----         ------           -----------
///</remarks>
namespace CSM.Business
{
    public class CommonFacade : ICommonFacade
    {
        private readonly CSMContext _context;
        private ICommonDataAccess _commonDataAccess;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CommonFacade));

        public CommonFacade()
        {
            _context = new CSMContext();
        }

        public IDictionary<string, string> GetStatusSelectList()
        {
            return this.GetStatusSelectList(null);
        }

        public IDictionary<string, string> GetStatusSelectList(string textName, int? textValue = null)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(textName))
            {
                dic.Add(textValue.ConvertToString(), textName);
            }

            dic.Add(Constants.ApplicationStatus.Active.ToString(CultureInfo.InvariantCulture), Resource.Ddl_Status_Active);
            dic.Add(Constants.ApplicationStatus.Inactive.ToString(CultureInfo.InvariantCulture), Resource.Ddl_Status_Inactive);
            return dic;
        }

        public List<DocumentTypeEntity> GetActiveDocumentTypes(int documentCategory)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetActiveDocumentTypes(documentCategory);
        }

        public List<DocumentTypeEntity> GetDocumentTypeList(List<AttachmentTypeEntity> attachTypes, int documentCategory)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetDocumentTypeList(attachTypes, documentCategory);
        }

        public IEnumerable<BranchEntity> GetBranchList(BranchSearchFilter searchFilter)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetBranchList(searchFilter);
        }

        public List<BranchEntity> GetBranchesByName(string searchTerm, int pageSize, int pageNum)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetBranchesByName(searchTerm, pageSize, pageNum);
        }

        public int GetBranchCountByName(string searchTerm, int pageSize, int pageNum)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetBranchCountByName(searchTerm, pageSize, pageNum);
        }

        public List<BranchEntity> GetBranchesByName(string searchTerm, int pageSize, int pageNum, int? userId)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetBranchesByName(searchTerm, pageSize, pageNum, userId);
        }

        public int GetBranchCountByName(string searchTerm, int pageSize, int pageNum, int? userId)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetBranchCountByName(searchTerm, pageSize, pageNum, userId);
        }

        public ParameterEntity GetCacheParamByName(string paramName)
        {
            List<ParameterEntity> parameters = this.GetCacheParameters();
            return parameters.FirstOrDefault(x => x.ParamName.ToUpper(CultureInfo.InvariantCulture).Equals(paramName.ToUpper(CultureInfo.InvariantCulture)));
        }

        public ParameterEntity GetParamByName(string paramName)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetParamByName(paramName);
        }

        public string GetValueParamByName(string paramName)
        {
            var parameter = GetCacheParamByName(paramName);
            return parameter != null ? parameter.ParamValue : string.Empty;
        }

        public string GetDescParamByName(string paramName)
        {
            var parameter = GetCacheParamByName(paramName);
            return parameter != null ? parameter.ParamDesc : string.Empty;
        }

        public IDictionary<string, string> GetCustomerTypeSelectList()
        {
            return this.GetCustomerTypeSelectList(null);
        }

        public IDictionary<string, string> GetCustomerTypeSelectList(string textName, int? textValue = null)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(textName))
            {
                dic.Add(textValue.ConvertToString(), textName);
            }

            dic.Add(Constants.CustomerType.Customer.ToString(CultureInfo.InvariantCulture), Resource.Ddl_CustomerType_Customer);
            dic.Add(Constants.CustomerType.Prospect.ToString(CultureInfo.InvariantCulture), Resource.Ddl_CustomerType_Prospect);
            dic.Add(Constants.CustomerType.Employee.ToString(CultureInfo.InvariantCulture), Resource.Ddl_CustomerType_Employee);
            return dic;
        }

        public IDictionary<string, string> GetTitleThaiSelectList()
        {
            _commonDataAccess = new CommonDataAccess(_context);
            var lst = _commonDataAccess.GetActiveTitle();
            return (from x in lst
                    where x.Language.ToUpper(CultureInfo.InvariantCulture) == Constants.TitleLanguage.TitleTh
                    select new
                    {
                        key = x.TitleId.ToString(),
                        value = x.TitleName
                    }).ToDictionary(t => t.key, t => t.value);
        }

        public IDictionary<string, string> GetTitleEnglishSelectList()
        {
            _commonDataAccess = new CommonDataAccess(_context);
            var lst = _commonDataAccess.GetActiveTitle();
            return (from x in lst
                    where x.Language.ToUpper(CultureInfo.InvariantCulture) == Constants.TitleLanguage.TitleEn
                    select new
                    {
                        key = x.TitleId.ToString(),
                        value = x.TitleName
                    }).ToDictionary(t => t.key, t => t.value);
        }

        public IDictionary<string, string> GetSubscriptTypeSelectList()
        {
            _commonDataAccess = new CommonDataAccess(_context);
            var lst = _commonDataAccess.GetActiveSubscriptType();
            return (from x in lst
                    select new
                    {
                        key = x.SubscriptTypeId.ToString(),
                        value = x.SubscriptTypeName
                    }).ToDictionary(t => t.key, t => t.value);
        }

        public IDictionary<string, string> GetPhoneTypeSelectList()
        {
            _commonDataAccess = new CommonDataAccess(_context);
            var lst = _commonDataAccess.GetActivePhoneType();
            return (from x in lst
                    select new
                    {
                        key = x.PhoneTypeId.ToString(),
                        value = x.PhoneTypeName
                    }).ToDictionary(t => t.key, t => t.value);
        }

        public SubscriptTypeEntity GetSubscriptTypeByCode(string subscriptTypeCode)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetSubscriptTypeByCode(subscriptTypeCode);
        }

        public PhoneTypeEntity GetPhoneTypeByCode(string phoneTypeCode)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetPhoneTypeByCode(phoneTypeCode);
        }

        public List<MenuEntity> GetCacheMainMenu(string selectedMenu, int roleValue)
        {
            // Check the cache
            List<MenuEntity> mainMenu = this.GetCacheMainMenu();

            List<MenuEntity> cloned = (mainMenu.Where(x => IsMenuVisible(x.MenuId.Value, roleValue))
                .Select(x => new MenuEntity
                {
                    MenuId = x.MenuId,
                    MenuCode = x.MenuCode,
                    MenuName = x.MenuName,
                    ActionName = x.ActionName,
                    ControllerName = x.ControllerName,
                    CssClass = x.CssClass,
                    AccessRoles = x.AccessRoles,
                    IsSelected = !string.IsNullOrWhiteSpace(selectedMenu) && x.MenuCode.ToUpper(CultureInfo.InvariantCulture).Equals(selectedMenu.ToUpper(CultureInfo.InvariantCulture)),
                    ConfigUrlList = x.ConfigUrlList != null ?
                        (from cf in x.ConfigUrlList
                         where (cf.Roles.Sum(r => r.RoleValue) & roleValue) != 0
                         select new ConfigureUrlEntity
                         {
                             ConfigureUrlId = cf.ConfigureUrlId,
                             SystemName = cf.SystemName,
                             Url = cf.Url,
                             ImageFile = cf.ImageFile,
                             Menu = cf.Menu,
                             FontName = cf.FontName
                         }).ToList() : null
                })).ToList();

            return cloned;
        }

        public List<MenuEntity> GetCacheCustomerTab(string selectedTab, int? customerId = null)
        {
            // Check the cache
            List<MenuEntity> customerTab = CacheLayer.Get<List<MenuEntity>>(Constants.CacheKey.CustomerTab);

            if (customerTab == null)
            {
                List<MenuEntity> menuItems = GetCustomerTabs();

                // Then add it to the cache so we 
                // can retrieve it from there next time
                CacheLayer.Add(menuItems, Constants.CacheKey.CustomerTab);
                customerTab = CacheLayer.Get<List<MenuEntity>>(Constants.CacheKey.CustomerTab);
            }

            string encryptedstring = StringCipher.Encrypt(customerId.ConvertToString(), Constants.PassPhrase);
            List<MenuEntity> cloned = (customerTab
                .Select(x => new MenuEntity
                {
                    MenuCode = x.MenuCode,
                    MenuName = x.MenuName,
                    ActionName = x.ActionName,
                    ControllerName = x.ControllerName,
                    CssClass = x.CssClass,
                    TabContent = x.TabContent,
                    CustomerEncrypted = encryptedstring,
                    IsSelected = !string.IsNullOrWhiteSpace(selectedTab) && x.MenuCode.ToUpper(CultureInfo.InvariantCulture).Equals(selectedTab.ToUpper(CultureInfo.InvariantCulture))
                })).ToList();

            return cloned;
        }

        public int GetRoleValueByScreenCode(string screenCode)
        {
            int roleValue = 0;
            List<ScreenEntity> screenRoles = this.GetCacheScreenRoles();

            if (!string.IsNullOrWhiteSpace(screenCode) && screenRoles != null)
            {
                ScreenEntity screen = screenRoles.FirstOrDefault(x => x.ScreenCode.ToUpper(CultureInfo.InvariantCulture).Equals(screenCode.ToUpper(CultureInfo.InvariantCulture)));
                if (screen != null)
                {
                    roleValue = screen.Roles.Sum(x => x.RoleValue);
                }
            }

            return roleValue;
        }

        public List<MenuEntity> GetReportList(int roleValue)
        {
            List<MenuEntity> menuItems = this.GetCacheMainMenu();
            var reportMenu = menuItems.FirstOrDefault(x => x.MenuCode == MenuCode.Report);

            if (reportMenu != null)
            {
                List<ScreenEntity> screenRoles = this.GetCacheScreenRoles();

                var reports = from sc in screenRoles
                              where sc.MenuId == reportMenu.MenuId
                                    && sc.ScreenCode != ScreenCode.MainReport
                                    && !string.IsNullOrEmpty(sc.ActionName) && !string.IsNullOrEmpty(sc.ControllerName)
                                    && ((roleValue & GetRoleValueByScreenCode(sc.ScreenCode)) != 0)
                              orderby sc.ScreenName
                              select new MenuEntity
                              {
                                  MenuCode = sc.ScreenCode,
                                  MenuName = sc.ScreenName,
                                  ActionName = sc.ActionName,
                                  ControllerName = sc.ControllerName
                              };

                return reports.Any() ? reports.ToList() : null;
            }

            return null;
        }

        public List<MenuEntity> GetMasterList(int roleValue)
        {
            List<MenuEntity> menuItems = this.GetCacheMainMenu();
            var masterMenu = menuItems.FirstOrDefault(x => x.MenuCode == MenuCode.Master);

            if (masterMenu != null)
            {
                List<ScreenEntity> screenRoles = this.GetCacheScreenRoles();

                var masters = from sc in screenRoles
                              where sc.MenuId == masterMenu.MenuId
                                    && !string.IsNullOrEmpty(sc.ActionName) && !string.IsNullOrEmpty(sc.ControllerName)
                                    && ((roleValue & GetRoleValueByScreenCode(sc.ScreenCode)) != 0)
                              orderby sc.ScreenName
                              select new MenuEntity
                              {
                                  MenuCode = sc.ScreenCode,
                                  MenuName = sc.ScreenName,
                                  ActionName = sc.ActionName,
                                  ControllerName = sc.ControllerName
                              };

                return masters.Any() ? masters.ToList() : null;
            }

            return null;
        }

        public List<UserEntity> GetActionByName(string searchTerm, int pageSize, int pageNum, int? branchId)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetActionByName(searchTerm, pageSize, pageNum, branchId);
        }

        public int GetActionCountByName(string searchTerm, int pageSize, int pageNum, int? branchId)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetActionCountByName(searchTerm, pageSize, pageNum, branchId);
        }

        public IDictionary<string, string> GetRelationshipSelectList()
        {
            _commonDataAccess = new CommonDataAccess(_context);
            var lst = _commonDataAccess.GetActiveRelationship();
            return (from x in lst
                    select new
                    {
                        key = x.RelationshipId.ToString(CultureInfo.InvariantCulture),
                        value = x.RelationshipName
                    }).ToDictionary(t => t.key, t => t.value);
        }

        public bool SaveShowhidePanel(int expand, int userId, string currentPage)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.SaveShowhidePanel(expand, userId, currentPage);
        }

        public DefaultSearchEntity GetShowhidePanelByUserId(UserEntity user, string currentPage)
        {
            if (user != null)
            {
                _commonDataAccess = new CommonDataAccess(_context);
                return _commonDataAccess.GetShowhidePanelByUserId(user.UserId, currentPage);
            }

            return null;
        }

        public string GetCSMDocumentFolder()
        {
            var paramEntity = this.GetCacheParamByName(Constants.ParameterName.AttachmentPathCustomer);
            return (paramEntity != null) ? paramEntity.ParamValue : string.Empty;
        }

        public string GetNewsDocumentFolder()
        {
            var paramEntity = this.GetCacheParamByName(Constants.ParameterName.AttachmentPathNews);
            return (paramEntity != null) ? paramEntity.ParamValue : string.Empty;
        }

        public string GetJobDocumentFolder()
        {
            var paramEntity = this.GetCacheParamByName(Constants.ParameterName.AttachmentPathJob);
            return (paramEntity != null) ? paramEntity.ParamValue : string.Empty;
        }

        public string GetSrDocumentFolder()
        {
            var paramEntity = this.GetCacheParamByName(Constants.ParameterName.AttachmentPathSr);
            return (paramEntity != null) ? paramEntity.ParamValue : string.Empty;
        }

        public int GetMaxMinuteBatchCreateSRActivityFromReplyEmail()
        {
            var paramEntity = this.GetCacheParamByName(Constants.ParameterName.MaxMinuteBatchCreateSRActivityFromReplyEmail);
            return (paramEntity != null) ? paramEntity.ParamValue.ToNullable<int>().Value : 60;
        }

        public int GetMaxMinuteBatchReSubmitActivityToCARSystem()
        {
            var paramEntity = this.GetCacheParamByName(Constants.ParameterName.MaxMinuteBatchReSubmitActivityToCARSystem);
            return (paramEntity != null) ? paramEntity.ParamValue.ToNullable<int>().Value : 60;
        }

        public int GetMaxMinuteBatchReSubmitActivityToCBSHPSystem()
        {
            var paramEntity = this.GetCacheParamByName(Constants.ParameterName.MaxMinuteBatchReSubmitActivityToCBSHPSystem);
            return (paramEntity != null) ? paramEntity.ParamValue.ToNullable<int>().Value : 60;
        }

        public string GetSLMUrlNewLead()
        {
            var paramEntity = this.GetCacheParamByName(Constants.ParameterName.SLMUrlNewLead);
            return (paramEntity != null) ? paramEntity.ParamValue : string.Empty;
        }

        public string GetSLMUrlViewLead()
        {
            var paramEntity = this.GetCacheParamByName(Constants.ParameterName.SLMUrlViewLead);
            return (paramEntity != null) ? paramEntity.ParamValue : string.Empty;
        }

        public string GetSLMEncryptPassword()
        {
            var paramEntity = this.GetCacheParamByName(WebConfig.GetSLMEncryptPassword());
            return (paramEntity != null) ? paramEntity.ParamValue : string.Empty;
        }

        public string GetOfficePhoneNo()
        {
            var paramEntity = this.GetCacheParamByName(Constants.ParameterName.OfficePhoneNo);
            return (paramEntity != null) ? paramEntity.ParamValue : string.Empty;
        }

        public string GetOfficeHour()
        {
            var paramEntity = this.GetCacheParamByName(Constants.ParameterName.OfficeHour);
            return (paramEntity != null) ? paramEntity.ParamValue : string.Empty;
        }

        public string GetProductGroupSubmitCBSHP()
        {
            var paramEntity = this.GetCacheParamByName(Constants.ParameterName.ProductGroupSubmitCBSHP);
            return (paramEntity != null) ? paramEntity.ParamValue : string.Empty;
        }

        public string GetTextDummyAccountNo()
        {
            var paramEntity = this.GetCacheParamByName(Constants.ParameterName.TextDummyAccountNo);
            return (paramEntity != null) ? paramEntity.ParamValue : string.Empty;
        }

        public int GetPageSizeStart()
        {
            ParameterEntity paramEntity = this.GetCacheParamByName(Constants.ParameterName.PageSizeStart);
            return (paramEntity != null) ? paramEntity.ParamValue.ToNullable<int>().Value : 15;
        }

        public int GetNextAttachmentSeq()
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetNextAttachmentSeq();
        }

        public IDictionary<string, int> GetPageSizeList()
        {
            // Check the cache
            IDictionary<string, int> pageSizeList = CacheLayer.Get<IDictionary<string, int>>(Constants.CacheKey.PageSizeList);

            if (pageSizeList == null)
            {
                int pageSize = this.GetPageSizeStart();
                IDictionary<string, int> dic = new Dictionary<string, int>();
                dic.Add(pageSize.ConvertToString(), pageSize);

                for (int i = 0; i < 2; i++)
                {
                    pageSize = pageSize * 2;
                    dic.Add(pageSize.ConvertToString(), pageSize);
                }

                // Then add it to the cache so we 
                // can retrieve it from there next time
                CacheLayer.Add(dic, Constants.CacheKey.PageSizeList);
                pageSizeList = CacheLayer.Get<IDictionary<string, int>>(Constants.CacheKey.PageSizeList);
            }

            return pageSizeList;
        }

        public List<string> GetExceptionErrorCodes(string errorSystem, string errorService)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetExceptionErrorCodes(errorSystem, errorService);
        }

        public BranchEntity GetBranchById(int branchId)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetBranchById(branchId);
        }

        public int GetNumMonthsActivity()
        {
            var paramEntity = this.GetCacheParamByName(Constants.ParameterName.NumMonthsActivity);
            return (paramEntity != null) ? paramEntity.ParamValue.ToNullable<int>().Value : 3;
        }

        #region "Interface Configuration"

        public dynamic GetHeaderByServiceName<T>(string serviceName)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Request Header").Add("ServiceName", serviceName).ToInputLogString());

            var today = DateTime.Now;
            var appDataPath = GetProfileXml();
            XDocument doc = XDocument.Load(appDataPath);

            dynamic result = null;
            var query = doc.Descendants("service").Where(x => x.Attribute("name").Value.ToUpper(CultureInfo.InvariantCulture) == serviceName.ToUpper(CultureInfo.InvariantCulture));

            if (typeof(T) == typeof(Header))
            {
                result = query.Select(x => new Header
                {
                    user_name = x.Element("username").Value,
                    password = x.Element("password").Value,
                    reference_no = ApplicationHelpers.GenerateRefNo(),
                    system_code = x.Element("system_code").Value,
                    service_name = x.Element("service_name").Value,
                    transaction_date = today.FormatDateTime("yyyy-MM-dd"),
                    channel_id = x.Element("channel_id").Value,
                    command = x.Element("command").Value
                }).FirstOrDefault();
            }

            return result;
        }

        public bool VerifyServiceRequest<T>(dynamic header)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Verify Request Header").ToInputLogString());

            bool noError = true;
            var appDataPath = GetProfileXml();
            IEnumerable<XElement> query = null;
            XDocument doc = XDocument.Load(appDataPath);

            if (typeof(T) == typeof(Header))
            {
                try
                {
                    query = from x in doc.Descendants("service")
                            where x.Element("service_name").Value.ToUpperInvariant() == header.service_name.ToUpperInvariant() &&
                                //x.Element("reference_no").Value == referenceNo &&
                                x.Element("system_code").Value.ToUpperInvariant() == header.system_code.ToUpperInvariant() &&
                                x.Element("username").Value.ToUpperInvariant() == header.user_name.ToUpperInvariant() &&
                                x.Element("password").Value == header.password
                            select x;

                    if (!query.Any())
                    {
                        noError = false;
                    }
                }
                catch (Exception ex)
                {
                    noError = false;
                    Logger.Error("Exception occur:\n", ex);
                }
            }

            Logger.Info(noError ? _logMsg.Clear().SetPrefixMsg("Verify Request Header").ToSuccessLogString() 
                : _logMsg.Clear().SetPrefixMsg("Verify Request Header").ToFailLogString());

            return noError;
        }

        private static string GetProfileXml()
        {
            try
            {
                var appDataPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Config");

                if (!Directory.Exists(appDataPath))
                    Directory.CreateDirectory(appDataPath);

                return string.Format(CultureInfo.InvariantCulture, "{0}/Profile.xml", appDataPath);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw;
            }
        }

        #endregion

        #region "Functions"

        private bool IsMenuVisible(int menuId, int userRole)
        {
            List<ScreenEntity> screenRoles = this.GetCacheScreenRoles();
            return screenRoles.Any(x => x.MenuId == menuId && (x.Roles.Sum(r => r.RoleValue) & userRole) != 0);
        }

        private List<ParameterEntity> GetCacheParameters()
        {
            // Check the cache
            List<ParameterEntity> parameters = CacheLayer.Get<List<ParameterEntity>>(Constants.CacheKey.AllParameters);

            if (parameters == null)
            {
                _commonDataAccess = new CommonDataAccess(_context);
                List<ParameterEntity> lstParameter = _commonDataAccess.GetAllParameters();

                // Then add it to the cache so we 
                // can retrieve it from there next time
                CacheLayer.Add(lstParameter, Constants.CacheKey.AllParameters);
                parameters = CacheLayer.Get<List<ParameterEntity>>(Constants.CacheKey.AllParameters);
            }

            return parameters;
        }

        private List<ScreenEntity> GetCacheScreenRoles()
        {
            // Check the cache
            List<ScreenEntity> screenRoles = CacheLayer.Get<List<ScreenEntity>>(Constants.CacheKey.ScreenRoles);

            if (screenRoles == null)
            {
                _commonDataAccess = new CommonDataAccess(_context);
                List<ScreenEntity> screenList = _commonDataAccess.GetScreenList();

                // Then add it to the cache so we 
                // can retrieve it from there next time
                CacheLayer.Add(screenList, Constants.CacheKey.ScreenRoles);
                screenRoles = CacheLayer.Get<List<ScreenEntity>>(Constants.CacheKey.ScreenRoles);
            }

            return screenRoles;
        }

        private List<MenuEntity> GetCacheMainMenu()
        {
            // Check the cache
            List<MenuEntity> mainMenu = CacheLayer.Get<List<MenuEntity>>(Constants.CacheKey.MainMenu);

            if (mainMenu == null)
            {
                _commonDataAccess = new CommonDataAccess(_context);
                List<MenuEntity> menuItems = _commonDataAccess.GetMenuList();

                // Then add it to the cache so we 
                // can retrieve it from there next time
                CacheLayer.Add(menuItems, Constants.CacheKey.MainMenu);
                mainMenu = CacheLayer.Get<List<MenuEntity>>(Constants.CacheKey.MainMenu);
            }

            return mainMenu;
        }

        public int[] GetCheckMailToDelegateUsers() { return (new CommonDataAccess(_context)).GetCheckMailToDelegateUsers(); }

        public int[] GetUnCheckMailToDelegateUsers() { return (new CommonDataAccess(_context)).GetUnCheckMailToDelegateUsers(); }

        private static List<MenuEntity> GetCustomerTabs()
        {
            var appDataPath = GetMenuXml();
            XDocument doc = XDocument.Load(appDataPath);

            var query = doc.Descendants("menu")
                .Select(x => new MenuEntity
            {
                MenuCode = x.Element("menu_code").Value,
                MenuName = x.Element("menu_name").Value,
                ActionName = x.Element("action_name").Value,
                ControllerName = x.Element("controller_name").Value,
                CssClass = x.Element("css_class").Value,
                TabContent = x.Element("tab_content").Value
            });

            return query.Any() ? query.ToList() : null;
        }

        private static string GetMenuXml()
        {
            try
            {
                var appDataPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Config");

                if (!Directory.Exists(appDataPath))
                    Directory.CreateDirectory(appDataPath);

                return string.Format(CultureInfo.InvariantCulture, "{0}/CustomerTabs.xml", appDataPath);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw;
            }
        }

        #endregion

        #region "IDisposable"

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (_context != null) { _context.Dispose(); }
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
