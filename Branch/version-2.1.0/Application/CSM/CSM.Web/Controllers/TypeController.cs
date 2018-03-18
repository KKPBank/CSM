using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CSM.Business;
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
    public class TypeController : BaseController
    {
        private ITypeFacade _typeFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ContactController));

        public ActionResult Index()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List Type").ToInputLogString());

            try
            {
                var typeVM = new TypeViewModel();
                typeVM.SearchFilter = new TypeSearchFilter
                {
                    TypeName = string.Empty,
                    Status = string.Empty,
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "AreaName",
                    SortOrder = "ASC"
                };

                ViewBag.PageSize = typeVM.SearchFilter.PageSize;
                ViewBag.Message = string.Empty;

                return View(typeVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Type").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult Create()
        {
            var model = new TypeViewModel();
            model.TypeIsActiveList = new List<SelectListItem>();
            model.TypeIsActiveList.Add(new SelectListItem() { Text = "Active", Value = "true" });
            model.TypeIsActiveList.Add(new SelectListItem() { Text = "Inactive", Value = "false" });
            model.Status = true;
            using (var facade = new TypeFacade())
            {
                model.TypeCode = facade.GetNextTypeCode().ToString();
            }

            ViewBag.CreateUsername = UserInfo.FullName;
            ViewBag.UpdateUsername = UserInfo.FullName;
            ViewBag.CreateDate = DateTime.Now;
            ViewBag.UpdateDate = DateTime.Now;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveType(TypeSaveViewModel typeSaveVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Type Save").ToInputLogString());
            try
            {
                if (ModelState.IsValid)
                {
                    TypeItemEntity typeEntity = new TypeItemEntity
                    {
                        TypeId = typeSaveVM.TypeId,
                        TypeName = typeSaveVM.TypeName,
                        TypeCode = typeSaveVM.TypeCode,
                        Status = typeSaveVM.Status,
                        UserId = UserInfo.UserId,
                        CreateUser = typeSaveVM.CreateUser,
                        CreateDate = typeSaveVM.CreateDate
                    };

                    _typeFacade = new TypeFacade();

                    if (typeEntity.TypeName.Length <= 100 && typeEntity.TypeName.Length > 0)
                    {
                        var checkType = _typeFacade.CheckTypeName(typeEntity);

                        if (checkType == false)
                        {
                            return Json(new {is_success = false, message = "ชื่อ Type ซ้ำ"});
                        }
                        else
                        {
                            var isSuccess = _typeFacade.SaveType(typeEntity);
                            return isSuccess
                                ? Json(new {is_success = true, message = "บันทึก Type สำเร็จ"})
                                : Json(new {is_success = false, message = "บันทึก Type ไม่สำเร็จ"});
                        }
                    }
                    else
                    {
                        return Json(new { is_success = false, message = "ชื่อ Type ต้องมากกว่าหรือเท่ากับ 1 ตัวอักษร และไม่เกิน 100 ตัวอักษร" });
                    }
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Type Save").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new { is_success = false, message = string.Format(CultureInfo.InvariantCulture, "Error : {0}", ex.Message) });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int TypeId)
        {            

            var list = new TypeViewModel();
            list.TypeIsActiveList = new List<SelectListItem>();
            list.TypeIsActiveList.Add(new SelectListItem() { Text = "Active", Value = "true" });
            list.TypeIsActiveList.Add(new SelectListItem() { Text = "Inactive", Value = "false" });

            var typeVM = new TypeViewModel();

            _typeFacade = new TypeFacade();

            if (TypeId != null && TypeId != 0)
            {
                var typeItemEntity = _typeFacade.GetTypeById(TypeId);

                typeVM.TypeId = typeItemEntity.TypeId;
                typeVM.TypeName = typeItemEntity.TypeName;
                typeVM.TypeCode = typeItemEntity.TypeCode;
                typeVM.Status = typeItemEntity.Status;
                
                typeVM.CreateDate = DateUtil.ToStringAsDateTime(typeItemEntity.CreateDate);
                typeVM.UpdateDate = DateUtil.ToStringAsDateTime(typeItemEntity.UpdateDate);

                typeVM.CreateUserName = typeItemEntity.CreateUserName;
                typeVM.UpdateUserName = typeItemEntity.UpdateUserName;
                typeVM.TypeIsActiveList = list.TypeIsActiveList;
                typeVM.CreateUser = typeItemEntity.CreateUser;
                typeVM.UpdateUser = typeItemEntity.UpdateUser;

            }

            return View(typeVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchTypeList(TypeSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Type").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _typeFacade = new TypeFacade();
                    TypeViewModel typeVm = new TypeViewModel();
                    typeVm.SearchFilter = searchFilter;

                    typeVm.TypeList = _typeFacade.GetTypeList(typeVm.SearchFilter);
                    ViewBag.PageSize = typeVm.SearchFilter.PageSize;

                    return PartialView("~/Views/Type/_TypeList.cshtml", typeVm);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Type").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
    }
}
