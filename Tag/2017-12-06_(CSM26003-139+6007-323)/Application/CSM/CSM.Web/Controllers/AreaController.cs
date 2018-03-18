using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CSM.Business;
using CSM.Common.Resources;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;
using System.Globalization;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class AreaController : BaseController
    {
        private IAreaFacade _areaFacade;
        private ISubAreaFacade _subAreaFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ContactController));

        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        public ActionResult Search()
        {
            try
            {
                var areaVM = new AreaViewModel();
                areaVM.SearchFilter = new AreaSearchFilter
                {
                    AreaName = string.Empty,
                    Status = string.Empty, 
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "AreaName",
                    SortOrder = "ASC"
                };

                ViewBag.PageSize = areaVM.SearchFilter.PageSize;
                ViewBag.Message = string.Empty;
                return View(areaVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Area").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
        [HttpPost]
        public ActionResult Create()
        {
            try
            {
                var model = new AreaViewModel();
                model.SubAreaSearchFilter = new SubAreaSearchFilter
                {
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "SubAreaName",
                    SortOrder = "ASC"
                };

                model.SelectSearchFilter = new SelectSubAreaSearchFilter
                {
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "SubAreaName",
                    SortOrder = "ASC"
                };
                using (var facade = new AreaFacade())
                {
                    model.AreaCode = facade.GetNextAreaCode().ToString();
                }

                ViewBag.PageSize = model.SubAreaSearchFilter.PageSize;
                ViewBag.Message = string.Empty;
                ViewBag.CreateUsername = UserInfo.FullName;
                ViewBag.UpdateUsername = UserInfo.FullName;
                ViewBag.CreateDate = DateTime.Now;
                ViewBag.UpdateDate = DateTime.Now;

                return View(model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List SubArea").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveArea(AreaSaveViewModel model, string idSubAreas)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Save Area").Add("IdSubAreas", idSubAreas).Add("AreaName", model.AreaName).ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _areaFacade = new AreaFacade();

                    //validate area name
                    var isValidate = _areaFacade.ValidateAreaName(model.AreaId, model.AreaName);

                    if (!isValidate)
                        return Json(new { is_success = false, message = Resource.Error_SaveFailedDuplicate });

                    AreaItemEntity areaEntity = new AreaItemEntity
                    {
                        AreaId = model.AreaId,
                        AreaName = model.AreaName,
                        AreaCode = model.AreaCode,
                        Status = model.Status,
                        UserId = UserInfo.UserId
                    };

                    var isSuccess = _areaFacade.SaveArea(areaEntity, idSubAreas);
                    return isSuccess ? Json(new {is_success = true, message = "บันทึก Area สำเร็จ"}) : Json(new {is_success = false , message = "บันทึก Area ไม่สำเร็จ"});
                }

                return Json(new
                {
                    is_success = false,
                    message = string.Empty
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save Area").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new { is_success = false, message = string.Format(CultureInfo.InvariantCulture, "Error : {0}", ex.Message) });
            }
        }

        [HttpPost]
        public ActionResult Edit(int? areaId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Area").Add("areaId", areaId).ToInputLogString());

            if (areaId.HasValue)
            {
                var areaVM = new AreaEditViewModel();
                _areaFacade = new AreaFacade();

                //get area section
                AreaItemEntity areaItemEntity = _areaFacade.GetArea(areaId.Value);
                areaVM.AreaId = areaItemEntity.AreaId;
                areaVM.txtAreaName = areaItemEntity.AreaName;
                areaVM.AreaCode = areaItemEntity.AreaCode;
                areaVM.txtCreateUser = areaItemEntity.CreateUser != null ? areaItemEntity.CreateUser.FullName : "";
                areaVM.txtUpdateUser = areaItemEntity.UpdateUser != null ? areaItemEntity.UpdateUser.FullName : "";
                areaVM.txtCreateDateTime = DateUtil.ToStringAsDateTime(areaItemEntity.CreateDate);
                areaVM.txtUpdateDateTime = DateUtil.ToStringAsDateTime(areaItemEntity.UpdateDate);
                areaVM.txtUpdateUser = areaItemEntity.UpdateDate.HasValue ? DateUtil.ToStringAsDateTime(areaItemEntity.UpdateDate) : "";

                areaVM.selectStatus = areaItemEntity.Status;
                areaVM.StatusList = new List<SelectListItem>();

                areaVM.StatusList.Add(new SelectListItem() { Text = "Active", Value = "true" });
                areaVM.StatusList.Add(new SelectListItem() { Text = "Inactive", Value = "false" });

                areaVM.SubAreaSearchFilter = new SelectSubAreaSearchFilter
                {
                    SubAreaName = string.Empty,
                    SubAreaIdList = string.Empty,
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "SubAreaName",
                    SortOrder = "ASC"
                };

                areaVM.SelectSearchFilter = new SelectSubAreaSearchFilter
                {
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "SubAreaName",
                    SortOrder = "ASC"
                };

                ViewBag.PageSize = areaVM.SubAreaSearchFilter.PageSize;
                ViewBag.Message = string.Empty;
                return View(areaVM);
            }

            return View("Create");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchSubArea(BootstrapParameters parameters, int areaId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search SubArea").Add("areaId", areaId).ToInputLogString());

            if (ModelState.IsValid)
            {
                _areaFacade = new AreaFacade();
                var totalCount = 0;
                List<AreaSubAreaItemEntity> list = _areaFacade.GetSubAreaListById(parameters.offset, parameters.limit, areaId, ref totalCount);

                return Json(new BootstrapTableResult()
                {
                    total = totalCount,
                    rows = list.Select(item => new
                    {
                        id = item.SubAreaId,
                        action = "<span style='cursor: pointer; color: red;' onclick='onDeleteSubAreaRow(" + item.SubAreaId + ")'><i class='fa fa-trash-o'></i></span>",
                        area_name = item.SubAreaName,
                        status = item.IsActive ? "Active" : "Inactive",
                        update_name = item.CreateUserFirstName + " " + item.CreateUserLastName,
                        update_date = item.CreateDate.HasValue ? DateUtil.ToStringAsDateTime(item.CreateDate) : ""
                    }).ToList()
                });
            }

            return Json(new
            {
                Valid = false,
                Error = string.Empty
            });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchAreaList(AreaSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Area").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _areaFacade = new AreaFacade();
                    var model = new AreaViewModel();
                    model.SearchFilter = searchFilter;
                    model.AreaList = _areaFacade.GetAreaList(model.SearchFilter);
                    ViewBag.PageSize = model.SearchFilter.PageSize;
                    return PartialView("~/Views/Area/_AreaList.cshtml", model);
                }

                return Json(new
                {
                    Valid = false,
                    Error = string.Empty
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Area").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchSubAreaListById(SelectSubAreaSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search SubArea").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _subAreaFacade = new SubAreaFacade();
                    var model = new AreaEditViewModel();
                    model.SubAreaSearchFilter = searchFilter;
                    model.SubAreaList = _subAreaFacade.GetSubAreaListById(model.SubAreaSearchFilter);
                    ViewBag.PageSize = model.SubAreaSearchFilter.PageSize;
                    return PartialView("~/Views/Area/_SubAreaEditList.cshtml", model);
                }

                return Json(new
                {
                    Valid = false,
                    Error = string.Empty
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search SubArea").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
    }
}