using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CSM.Business;
using CSM.Common.Resources;
using CSM.Common.Securities;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;
using Newtonsoft.Json;
using System.Globalization;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class CommPoolController : BaseController
    {
        private ICommonFacade _commonFacade;
        private ICommPoolFacade _commPoolFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CommPoolController));

        [CheckUserRole(ScreenCode.SearchConfigCommPool)]
        public ActionResult Search()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Pool").ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();

                PoolViewModel poolVM = new PoolViewModel();
                poolVM.SearchFilter = new PoolSearchFilter
                {
                    PoolName = string.Empty,
                    PoolDesc = string.Empty,
                    Email = string.Empty,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "PoolId",
                    SortOrder = "DESC"
                };

                ViewBag.PageSize = poolVM.SearchFilter.PageSize;
                ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.Message = string.Empty;

                return View(poolVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Pool").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.SearchConfigCommPool)]
        public ActionResult PoolList(PoolSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Pool").Add("PoolName", searchFilter.PoolName)
                .Add("PoolDesc", searchFilter.PoolDesc).Add("Email", searchFilter.Email).ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _commonFacade = new CommonFacade();
                    _commPoolFacade = new CommPoolFacade();
                    PoolViewModel poolVM = new PoolViewModel();
                    poolVM.SearchFilter = searchFilter;

                    poolVM.PoolList = _commPoolFacade.GetPoolList(poolVM.SearchFilter);
                    ViewBag.PageSize = poolVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Pool").ToSuccessLogString());
                    return PartialView("~/Views/CommPool/_PoolList.cshtml", poolVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Pool").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ManageConfigCommPool)]
        public ActionResult InitEdit(int? commPoolId = null)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitEdit CommunicationPool").Add("CommPoolId", commPoolId).ToInputLogString());

            try
            {
                PoolViewModel poolVM = null;

                if (TempData["PoolVM"] != null)
                {
                    poolVM = (PoolViewModel)TempData["PoolVM"];
                }
                else
                {
                    poolVM = new PoolViewModel { PoolId = commPoolId };
                }

                _commonFacade = new CommonFacade();
                _commPoolFacade = new CommPoolFacade();

                var statusList = _commonFacade.GetStatusSelectList();
                poolVM.StatusList = new SelectList((IEnumerable)statusList, "Key", "Value", string.Empty);

                if (poolVM.PoolId != null)
                {
                    PoolEntity poolEntity = _commPoolFacade.GetPoolByID(poolVM.PoolId.Value);
                    poolVM.PoolName = poolEntity.PoolName;
                    poolVM.PoolDesc = poolEntity.PoolDesc;
                    poolVM.Email = poolEntity.Email;
                    poolVM.Status = poolEntity.Status;

                    poolVM.CreateUser = poolEntity.CreateUser != null ? poolEntity.CreateUser.FullName : string.Empty;
                    poolVM.UpdateUser = poolEntity.UpdateUser != null ? poolEntity.UpdateUser.FullName : string.Empty;
                    poolVM.CreatedDate =
                        poolEntity.CreatedDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                    poolVM.UpdateDate =
                        poolEntity.Updatedate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);

                    var poolBranches = _commPoolFacade.GetPoolBranchList(poolVM.PoolId.Value);
                    poolVM.PoolBranchList = poolBranches;
                    poolVM.JsonBranch = JsonConvert.SerializeObject(poolBranches);
                }
                else
                {
                    poolVM.PoolBranchList = _commPoolFacade.GetPoolBranchList(poolVM.SelectedBranch)
                        .Where(x => x.IsDelete == false).ToList();

                    // default UserLogin
                    if (this.UserInfo != null)
                    {
                        var today = DateTime.Now;
                        poolVM.CreateUser = this.UserInfo.FullName;
                        poolVM.CreatedDate = today.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                        poolVM.UpdateDate = today.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                        poolVM.UpdateUser = this.UserInfo.FullName;
                    }
                }

                return View("~/Views/CommPool/Edit.cshtml", poolVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitEdit CommunicationPool").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [CheckUserRole(ScreenCode.ManageConfigCommPool)]
        public ActionResult Edit(PoolViewModel poolVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Save CommunicationPool").Add("CommPoolId", poolVM.PoolId)
                .Add("PoolName", poolVM.PoolName).Add("PoolDesc", poolVM.PoolDesc).ToInputLogString());

            try
            {
                bool skipValidate = false;

                if (poolVM.PoolId != null && string.IsNullOrWhiteSpace(poolVM.Password)
                        && string.IsNullOrWhiteSpace(poolVM.ConfirmPasswd))
                {
                    ModelState.Remove("Password");
                    ModelState.Remove("ConfirmPasswd");
                    skipValidate = true;
                }

                if (!string.IsNullOrWhiteSpace(poolVM.Password) && !string.IsNullOrWhiteSpace(poolVM.ConfirmPasswd)
                        && !poolVM.ConfirmPasswd.Equals(poolVM.Password))
                {
                    ModelState.AddModelError("ConfirmPasswd", Resource.ValError_InvalidConfirmPasswd);
                }

                if (ModelState.IsValid)
                {
                    List<PoolBranchEntity> selectedBranch = poolVM.SelectedBranch;

                    // Validate select at least one branch
                    if (!poolVM.SelectedBranch.Any(x => x.IsDelete == false))
                    {
                        ViewBag.ErrorMessage = string.Format(CultureInfo.InvariantCulture, Resource.ValErr_AtLeastOneItemWithField, Resource.Lbl_Branch);
                        goto Outer;
                    }

                    // Save CommPool
                    PoolEntity poolEntity = new PoolEntity
                    {
                        PoolId = poolVM.PoolId,
                        PoolName = poolVM.PoolName,
                        PoolDesc = poolVM.PoolDesc,
                        Email = poolVM.Email,
                        Status = poolVM.Status,
                        CreateUser = UserInfo, //When save the program will select to save this parameter
                        UpdateUser = UserInfo
                    };

                    if (!skipValidate)
                    {
                        string encryptedstring = StringCipher.Encrypt(poolVM.Password, Constants.PassPhrase);
                        poolEntity.Password = encryptedstring;
                    }

                    _commPoolFacade = new CommPoolFacade();

                    #region "Check Duplicate"
                    // Check Duplicate                    
                    if (_commPoolFacade.IsDuplicateCommPool(poolEntity) == true)
                    {
                        ViewBag.ErrorMessage = Resource.Error_SaveFailed_CommPoolDuplicate;
                        goto Outer;
                    }
                    #endregion
                    
                    bool success = _commPoolFacade.SaveCommPool(poolEntity, selectedBranch);

                    if (success)
                    {
                        return RedirectToAction("Search", "CommPool");
                    }

                    ViewBag.ErrorMessage = Resource.Error_SaveFailed;
                }

            Outer:
                TempData["PoolVM"] = poolVM;
                return InitEdit();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save CommunicationPool").ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [CheckUserRole(ScreenCode.ManageConfigCommPool)]
        public ActionResult PoolBranchList(PoolViewModel poolVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get PoolBranch List").Add("PoolName", poolVM.JsonBranch).ToInputLogString());

            try
            {
                _commPoolFacade = new CommPoolFacade();
                poolVM.PoolBranchList = _commPoolFacade.GetPoolBranchList(poolVM.SelectedBranch)
                    .Where(x => x.IsDelete == false).ToList();

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get PoolBranch List").ToSuccessLogString());
                return PartialView("~/Views/CommPool/_PoolBranchList.cshtml", poolVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get PoolBranch List").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [CheckUserRole(ScreenCode.ManageConfigCommPool)]
        public JsonResult DeleteBranch(PoolViewModel poolVM, int branchId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete PoolBranch").Add("CommPoolId", poolVM.PoolId).Add("BranchId", branchId).ToInputLogString());

            try
            {
                var deletedBranch = poolVM.SelectedBranch.FirstOrDefault(x => x.BranchId == branchId);

                if (deletedBranch.PoolId == null)
                {
                    poolVM.SelectedBranch.Remove(deletedBranch);
                }
                else
                {
                    deletedBranch.IsDelete = true;
                }

                return Json(new
                {
                    Valid = true,
                    Data = JsonConvert.SerializeObject(poolVM.SelectedBranch)
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete PoolBranch").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = Resource.Error_System,
                    Errors = string.Empty
                });
            }
        }
    }
}
