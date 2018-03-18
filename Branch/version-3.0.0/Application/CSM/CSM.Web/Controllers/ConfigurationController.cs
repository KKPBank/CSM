using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CSM.Business;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Entity.Common;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;
using CSM.Common.Resources;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class ConfigurationController : BaseController
    {
        private ICommonFacade _commonFacade;
        private IConfigurationFacade _configFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ConfigurationController));

        [CheckUserRole(ScreenCode.SearchConfigUrl)]
        public ActionResult Search()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Configuration").ToInputLogString());
            try
            {
                _commonFacade = new CommonFacade();
                ConfigurationViewModel configurationVM = new ConfigurationViewModel();

                configurationVM.SearchFilter = new ConfigureUrlSearchFilter
                {
                    Status = null,
                    SystemName = string.Empty,
                    Url = string.Empty,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "ConfigureUrlId",
                    SortOrder = "DESC"
                };

                var statusList = _commonFacade.GetStatusSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                configurationVM.StatusList = new SelectList((IEnumerable)statusList, "Key", "Value", string.Empty);

                ViewBag.PageSize = configurationVM.SearchFilter.PageSize;
                ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.Message = string.Empty;
                return View(configurationVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Configuration").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [CheckUserRole(ScreenCode.SearchConfigUrl)]
        public ActionResult ConfigurationList(ConfigureUrlSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Configuration").Add("SystemName", searchFilter.SystemName)
                .Add("Url", searchFilter.Url).Add("Status", searchFilter.Status));
            try
            {
                if (ModelState.IsValid)
                {
                    _commonFacade = new CommonFacade();
                    _configFacade = new ConfigurationFacade();
                    ConfigurationViewModel configVM = new ConfigurationViewModel();
                    configVM.SearchFilter = searchFilter;

                    configVM.ConfigureUrlList = _configFacade.GetConfigureURL(searchFilter);
                    ViewBag.PageSize = configVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                    return PartialView("~/Views/Configuration/_ConfigurationList.cshtml", configVM);
                }

                return Json(new
                {
                    Valid = false,
                    Error = string.Empty,
                    Errors = GetModelValidationErrors()
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Configuration").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [CheckUserRole(ScreenCode.ManageConfigUrl)]
        public ActionResult InitEdit(int? ConfigureUrlId = null)
        {
            ConfigurationViewModel configVM = null;

            if (TempData["configurationVM"] != null)
            {
                configVM = (ConfigurationViewModel)TempData["configurationVM"];
            }
            else
            {
                configVM = new ConfigurationViewModel { ConfigureUrlId = ConfigureUrlId };
            }

            _commonFacade = new CommonFacade();
            var statusList = _commonFacade.GetStatusSelectList();
            configVM.StatusList = new SelectList((IEnumerable)statusList, "Key", "Value", string.Empty);

            #region "For show in hint"

            //ParameterEntity param = _commonFacade.GetCacheParamByName(Constants.ParameterName.RegexConfigIcon);
            //ParameterEntity paramSingleFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.SingleFileSize);

            //int? limitSingleFileSize = paramSingleFileSize.ParamValue.ToNullable<int>();
            //var singleLimitSize = limitSingleFileSize.HasValue ? (limitSingleFileSize / 1048576) : 0;
            //ViewBag.UploadLimitType = string.Format(param.ParamDesc, singleLimitSize);

            #endregion

            _configFacade = new ConfigurationFacade();
            var menuList = _configFacade.GetAllMenu();
            configVM.MenuList = new SelectList((IEnumerable)menuList, "Key", "Value", string.Empty);

            List<RoleEntity> roles = _configFacade.GetAllRole();
            if (roles != null && roles.Count > 0)
            {
                configVM.RoleCheckBoxes = roles.Select(x => new CheckBoxes
                {
                    Value = x.RoleId.ToString(),
                    Text = x.RoleName,
                    Checked = false
                }).ToList();
            }

            if (configVM.ConfigureUrlId != null)
            {
                ConfigureUrlEntity configUrlEntity = _configFacade.GetConfigureURLById(configVM.ConfigureUrlId.Value);
                configVM.ConfigureUrlId = configUrlEntity.ConfigureUrlId;
                configVM.SystemName = configUrlEntity.SystemName;
                configVM.Url = configUrlEntity.Url;
                configVM.Status = configUrlEntity.Status;
                configVM.MenuId = configUrlEntity.Menu.MenuId;
                configVM.FileUrl = configUrlEntity.ImageUrl;
                configVM.FontName = configUrlEntity.FontName;

                configVM.CreateUser = configUrlEntity.CreateUser != null ? configUrlEntity.CreateUser.FullName : "";
                configVM.CreatedDate = configUrlEntity.CreatedDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                configVM.UpdateDate = configUrlEntity.Updatedate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                configVM.UpdateUser = configUrlEntity.UpdateUser != null ? configUrlEntity.UpdateUser.FullName : "";

                if (configVM.RoleCheckBoxes != null && configUrlEntity.Roles != null)
                {
                    configVM.JsonRole = JsonConvert.SerializeObject(configUrlEntity.Roles);
                    configVM.RoleCheckBoxes.Where(x => configUrlEntity.Roles.Select(r => r.RoleId.ToString()).Contains(x.Value))
                        .ToList().ForEach(x => x.Checked = true);
                }
            }
            else
            {
                // default UserLogin
                if (this.UserInfo != null)
                {
                    var today = DateTime.Now;
                    configVM.CreateUser = this.UserInfo.FullName;
                    configVM.CreatedDate = today.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                    configVM.UpdateDate = today.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                    configVM.UpdateUser = this.UserInfo.FullName;
                }
               
            }

            // List Font
            int refPageIndex = 0;
            configVM.FontList = GetFont(null, null, ref refPageIndex).FontList;
            configVM.PageIndexOfFont = refPageIndex;

            return View("~/Views/Configuration/Edit.cshtml", configVM);
        }

        [HttpPost]
        [CheckUserRole(ScreenCode.ManageConfigUrl)]
        public ActionResult Edit(ConfigurationViewModel configVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Save Configuration").Add("SystemName", configVM.SystemName)
                .Add("URL", configVM.Url).ToInputLogString());
            try
            {
                //if (configVM.ConfigureUrlId != null)
                //{
                //    ModelState.Remove("File");
                //}
                
                if (ModelState.IsValid)
                {
                    var selectedRole = configVM.RoleCheckBoxes.Where(x => x.Checked == true)
                               .Select(x => new RoleEntity
                               {
                                   RoleId = x.Value.ToNullable<int>()
                               }).ToList();

                    if (!selectedRole.Any(x => x.IsDelete == false))
                    {
                        ViewBag.ErrorMessage = Resource.ValErr_AtLeastOneItem;
                        goto Outer;
                    }

                    //var file = configVM.File;
                    ConfigureUrlEntity configUrlEntity = new ConfigureUrlEntity();
                    configUrlEntity.ConfigureUrlId = configVM.ConfigureUrlId ?? 0;
                    configUrlEntity.SystemName = configVM.SystemName;
                    configUrlEntity.Url = configVM.Url;
                    configUrlEntity.Status = configVM.Status;
                    configUrlEntity.CreateUser = UserInfo;
                    configUrlEntity.UpdateUser = UserInfo;
                    configUrlEntity.Menu = new MenuEntity { MenuId = configVM.MenuId ?? 0 };
                    configUrlEntity.FontName = configVM.FontName;

                    #region "Check Duplicate"
                    // Check Duplicate
                    _configFacade = new ConfigurationFacade();
                    if(_configFacade.IsDuplicateConfigureUrl(configUrlEntity) == true)
                    {
                        ViewBag.ErrorMessage = Resource.Error_SaveUrl;
                        goto Outer;
                    }
                    #endregion


                    #region "comment out"
                    //if (file != null && file.ContentLength > 0)
                    //{
                    //    _commonFacade = new CommonFacade();
                    //    ParameterEntity paramSingleFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.SingleFileSize);
                    //    int? limitSingleFileSize = paramSingleFileSize.ParamValue.ToNullable<int>();

                    //    if (file.ContentLength > limitSingleFileSize)//2MB = 2097152 , 1MB = 1048576
                    //    {
                    //        ViewBag.ErrorMessage = Resource.ValError_FileSizeUploadMaxLimit;
                    //        goto Outer;
                    //    }

                    //    // extract only the filename
                    //    var fiWithoutExt = Path.GetFileNameWithoutExtension(file.FileName);
                    //    var fiExt = Path.GetExtension(file.FileName);
                    //    var fileName = string.Format("{0}_{1}{2}", fiWithoutExt, DateTime.Now.FormatDateTime("yyyyMMddHHmmssfff"), fiExt);

                    //    //const string regexPattern = @"^.*\.(jpg|jpeg|doc|docx|xls|xlsx|ppt|txt)$";
                    //    ParameterEntity param = _commonFacade.GetCacheParamByName(Constants.ParameterName.RegexConfigIcon);
                    //    Match match = Regex.Match(fileName, param.ParamValue, RegexOptions.IgnoreCase);

                    //    if (!match.Success)
                    //    {
                    //        ViewBag.ErrorMessage = Resource.ValError_FileExtensionConfigUrl;
                    //        goto Outer;
                    //    }

                    //    string imgPath = StreamDataHelpers.GetApplicationPath(string.Format("{0}{1}", Constants.ConfigUrlPath, fileName));
                    //    file.SaveAs(imgPath);

                    //    configUrlEntity.ImageFile = fileName;
                    //}

                    #endregion

                    #region "Get Roles"

                    if (configVM.RoleList != null && configVM.RoleList.Count > 0)
                    {
                        var prevRoles = (from rl in configVM.RoleList
                            select new RoleEntity
                            {
                                RoleId = rl.RoleId,
                                RoleName = rl.RoleName,
                                IsDelete = !selectedRole.Select(x => x.RoleId).Contains(rl.RoleId)
                            }).ToList();

                        var dupeRoles = new List<RoleEntity>(selectedRole);
                        dupeRoles.AddRange(prevRoles);

                        var duplicates = dupeRoles.GroupBy(x => new {x.RoleId})
                            .Where(g => g.Count() > 1)
                            .Select(g => (object) g.Key.RoleId);

                        if (duplicates.Any())
                        {
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Duplicate ID in list")
                                .Add("IDs", StringHelpers.ConvertListToString(duplicates.ToList(), ","))
                                .ToInputLogString());
                            prevRoles.RemoveAll(x => duplicates.Contains(x.RoleId));
                        }

                        selectedRole.AddRange(prevRoles);
                    }

                    configUrlEntity.Roles = selectedRole;

                    #endregion
                    
                    bool success = _configFacade.SaveConfigurationUrl(configUrlEntity);
                    if (success)
                    {
                        CacheLayer.Clear(Constants.CacheKey.MainMenu);
                        return RedirectToAction("Search", "Configuration");
                    }

                    ViewBag.ErrorMessage = Resource.Error_SaveFailed;
                }

            Outer:
                TempData["configurationVM"] = configVM;
                return InitEdit();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save Configuration").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        public ActionResult Index()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List Configuration").ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();
                ViewBag.MasterList = _commonFacade.GetMasterList(this.UserInfo.RoleValue);
                return View();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(
                    _logMsg.Clear().SetPrefixMsg("List Configuration").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [CheckUserRole(ScreenCode.ManageConfigUrl)]
        public ActionResult IconList(int? currentPage, string isNext)
        {
            try
            {
                int refPageIndex = 0;
                var configVM = this.GetFont(currentPage, isNext, ref refPageIndex);
                configVM.PageIndexOfFont = refPageIndex;

                return PartialView("~/Views/Configuration/_IconList.cshtml", configVM);

            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Configuration").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        private ConfigurationViewModel GetFont(int? currentPage, string isNext, ref int refPageIndex)
        {
            _configFacade = new ConfigurationFacade();
            var lstAllFont = _configFacade.GetFont();

            ConfigurationViewModel configVM = new ConfigurationViewModel();
            configVM.FontList = new List<FontViewModel>();

            int pageIndex = 0;
            if (currentPage.HasValue == false || string.IsNullOrEmpty(isNext))
            {
                pageIndex = 0;
            }
            else
            {
                pageIndex = isNext.Equals("1") ? currentPage.Value + 1 : currentPage.Value - 1;
                pageIndex = pageIndex <= 0 ? 0 : pageIndex;
            }

            int itemPerPage = 16;            
            int startPageIndex = pageIndex == 0 ? 0 : (pageIndex * itemPerPage);
            int totalRecords = lstAllFont.Count();
            if (startPageIndex >= totalRecords)
            {
                startPageIndex = 0;
                pageIndex = 1;
            }
            var lstFont = lstAllFont.Skip(startPageIndex).Take(itemPerPage).ToList<FontEntity>();
            var cntOfIndex = lstFont.Count();

            int index = 0;
            for (; ; )
            {
                configVM.FontList.Add
                    (new FontViewModel
                    {
                        Font_1 = (index < cntOfIndex) ? lstFont[index].FontName : string.Empty,
                        Font_2 = (index + 1 < cntOfIndex) ? lstFont[index + 1].FontName : string.Empty,
                        Font_3 = (index + 2 < cntOfIndex) ? lstFont[index + 2].FontName : string.Empty,
                        Font_4 = (index + 3 < cntOfIndex) ? lstFont[index + 3].FontName : string.Empty,
                    });

                index = index + 4;

                if (index >= cntOfIndex)
                    break;
            }

            refPageIndex = pageIndex;

            return configVM;
        }
    }
}
