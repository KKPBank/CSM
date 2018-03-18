using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
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
    public class SubAreaController : BaseController
    {
        private ISubAreaFacade _subAreaFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ContactController));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchSelectSubAreaList(SelectSubAreaSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search SelectSubArea").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _subAreaFacade = new SubAreaFacade();
                    var model = new AreaViewModel();
                    model.SelectSearchFilter = searchFilter;

                    model.SelectSubAreaList = _subAreaFacade.GetSelectSubAreaList(model.SelectSearchFilter);
                    ViewBag.PageSize = model.SelectSearchFilter.PageSize;

                    return PartialView("~/Views/Area/_SelectSubAreaList.cshtml", model);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search SelectSubArea").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Save(SubAreaViewModel subAreaVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Sub Area Save").ToInputLogString());
            try
            {
                if (ModelState.IsValid)
                {
                    _subAreaFacade = new SubAreaFacade();

                    //validate duplicate sub area name
                    var isValidate = _subAreaFacade.ValidateSubArea(subAreaVM.SubAreaName, subAreaVM.SubAreaId);

                    if (!isValidate)
                        return Json(new { is_success = false, message = Resource.Error_SaveFailedDuplicate });

                    SubAreaItemEntity subAreaItemEntity = new SubAreaItemEntity
                    {
                        SubAreaId = subAreaVM.SubAreaId,
                        SubAreaName = subAreaVM.SubAreaName,
                        SubAreaCode = subAreaVM.SubAreaCode,
                        IsActive = subAreaVM.Status,
                        UserId = UserInfo.UserId
                    };
                    //save sub area
                    var result = _subAreaFacade.SaveSubArea(subAreaItemEntity);

                    return result != null
                        ? Json(new
                        {
                            is_success = true,
                            subAreaId = result.SubAreaId,
                            subAreaName = result.SubAreaName,
                            subAreaCode = result.SubAreaCode,
                            status = result.IsActive,
                            updateUser = result.UpdateUser != null ? result.UpdateUser.FullName : "",
                            updateDate = result.UpdateDateTime.HasValue ? DateUtil.ToStringAsDateTime(result.UpdateDateTime.Value) : DateUtil.ToStringAsDateTime(result.CreateDateTime.Value),
                            is_edit = result.IsEdit
                        })
                        : Json(new {is_success = false, message = Resource.Error_SaveFailed});
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Sub Area Save").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new { is_success = false, message = string.Format(CultureInfo.InvariantCulture, "Error : {0}", ex.Message) });
            }
        }

        [HttpPost]
        public JsonResult GetSubArea(int id)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get SubArea").Add("Id", id).ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _subAreaFacade = new SubAreaFacade();

                    SubAreaItemEntity subAreaItemEntity = _subAreaFacade.GetSubAreaItem(id);

                    return subAreaItemEntity != null
                        ? Json(new
                        {
                            is_success = true,
                            subAreaId = subAreaItemEntity.SubAreaId,
                            subAreaName = subAreaItemEntity.SubAreaName,
                            subAreaCode = subAreaItemEntity.SubAreaCode,
                            subAreaStatus = subAreaItemEntity.IsActive
                        })
                        : Json(new
                        {
                            is_success = false,
                            message = "ไม่พบข้อมูล"
                        });
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get SubArea").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    is_success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult GetNextSubAreaCode()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get SubArea").ToInputLogString());
            try
            {
                string nextCode;
                using (var facade = new SubAreaFacade())
                {
                    nextCode = facade.GetNextSubAreaCode().ToString();
                }
                return Json(new
                {
                    is_success = true,
                    NextSubAreaCode = nextCode
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get SubArea").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    is_success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RenderSubAreaList(string subAreaList, string selectSubAreaList, int? deleteId, int? subAreaId, string subAreaName, string subAreaCode, string status, string updateUser, string updateDate)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Render SubArea").ToInputLogString());

            if (!string.IsNullOrEmpty(status))
            {
                if (status.ToLower() == "true")
                    status = "Active";
                else if (status.ToLower() == "false")
                    status = "Inactive";
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var subAreaTableModel = new SubAreaViewModel();
                    var areaList = new JavaScriptSerializer().Deserialize<SubAreaTableModel[]>(subAreaList);
                    subAreaTableModel.SubAreaTableList = new List<SubAreaTableModel>();

                    foreach (var item in areaList)
                    {
                        if (!deleteId.HasValue || item.id != deleteId.Value)
                        {
                            var model = new SubAreaTableModel()
                            {
                                id = item.id,
                                area_name = item.area_name,
                                area_code = item.area_code,
                                status = item.status,
                                update_name = item.update_name,
                                update_date = item.update_date
                            };

                            subAreaTableModel.SubAreaTableList.Add(model);
                        }
                    }

                    if (selectSubAreaList != "null")
                    {
                        var selectSubList = new JavaScriptSerializer().Deserialize<SubAreaTableModel[]>(selectSubAreaList);
                        foreach (var item in selectSubList)
                        {
                            if (!deleteId.HasValue || item.id != deleteId.Value)
                            {
                                var model = new SubAreaTableModel()
                                {
                                    id = item.id,
                                    area_name = item.area_name,
                                    area_code = item.area_code,
                                    status = item.status,
                                    update_name = item.update_name,
                                    update_date = item.update_date
                                };

                                subAreaTableModel.SubAreaTableList.Add(model);
                            }
                        } 
                    }

                    if (subAreaId.HasValue)
                    {
                        var model = new SubAreaTableModel()
                        {
                            id = subAreaId.Value,
                            area_name = subAreaName,
                            area_code = subAreaCode,
                            status = status,
                            update_name = updateUser,
                            update_date = updateDate
                        };

                        subAreaTableModel.SubAreaTableList.Add(model);
                    }

                    subAreaTableModel.SubAreaTableList = subAreaTableModel.SubAreaTableList.OrderBy(q => q.area_name).ToList();
                    return PartialView("~/Views/Area/_RenderSubAreaList.cshtml", subAreaTableModel);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Render SubArea").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
    }
}