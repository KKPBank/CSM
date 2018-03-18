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
    public class QuestionController : BaseController
    {
        private IQuestionFacade _questionFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(QuestionController));

        public ActionResult Index()
        {
            try
            {
                var questionVM = new QuestionViewModel();
                questionVM.QuestionIsActiveList = new List<SelectListItem>();
                questionVM.QuestionIsActiveList.Add(new SelectListItem { Text = "ทั้งหมด", Value = "all"});
                questionVM.QuestionIsActiveList.Add(new SelectListItem { Text = "Active", Value = "true" });
                questionVM.QuestionIsActiveList.Add(new SelectListItem { Text = "Inactive", Value = "false" });
                questionVM.Status = null;

                questionVM.SearchFilter = new QuestionSearchFilter
                {
                    QuestionName = string.Empty,
                    Status = "",
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "QuestionName",
                    SortOrder = "ASC"
                };

                ViewBag.PageSize = questionVM.SearchFilter.PageSize;
                ViewBag.Message = string.Empty;

                return View(questionVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Question").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        public ActionResult Search()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Create()
        {
            var model = new QuestionViewModel();
            model.QuestionIsActiveList = new List<SelectListItem>();
            model.QuestionIsActiveList.Add(new SelectListItem { Text = "Active", Value = "true" });
            model.QuestionIsActiveList.Add(new SelectListItem { Text = "Inactive", Value = "false" });
            model.Status = true;

            ViewBag.CreateUsername = UserInfo.FullName;
            ViewBag.UpdateUsername = UserInfo.FullName;
            ViewBag.CreateDate = DateTime.Now;
            ViewBag.UpdateDate = DateTime.Now;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveQuestion(QuestionSaveViewModel questionSaveVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Question Save").ToInputLogString());
            try
            {
                if (ModelState.IsValid)
                {
                    QuestionItemEntity questionEntity = new QuestionItemEntity
                    {
                        QuestionId = questionSaveVM.QuestionId,
                        QuestionName = questionSaveVM.QuestionName,
                        Status = questionSaveVM.Status,
                        UserId = UserInfo.UserId,
                        CreateUser = questionSaveVM.CreateUser,
                        CreateDate = questionSaveVM.CreateDate
                    };

                    _questionFacade = new QuestionFacade();

                    if (questionEntity.QuestionName.Length <= 8000 && questionEntity.QuestionName.Length > 0)
                    {
                        var checkQuestion = _questionFacade.CheckQuestionName(questionEntity);

                        if (checkQuestion == false)
                        {
                            return Json(new { is_success = false, message = "ชื่อ Question ซ้ำ" });
                        }
                        else
                        {
                            var isSuccess = _questionFacade.SaveQuestion(questionEntity);
                            return isSuccess
                                ? Json(new { is_success = true, message = "บันทึก Question สำเร็จ" })
                                : Json(new { is_success = false, message = "บันทึก Question ไม่สำเร็จ" });
                        }
                    }
                    else
                    {
                        return Json(new { is_success = false, message = "ชื่อ Question ต้องมากกว่าหรือเท่ากับ 1 ตัวอักษร และไม่เกิน 100 ตัวอักษร" });
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Question Save").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new { is_success = false, message = string.Format(CultureInfo.InvariantCulture, "Error : {0}", ex.Message) });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int QuestionId)
        {
            var questionVM = new QuestionViewModel();
            questionVM.QuestionIsActiveList = new List<SelectListItem>();
            questionVM.QuestionIsActiveList.Add(new SelectListItem() { Text = "Active", Value = "true" });
            questionVM.QuestionIsActiveList.Add(new SelectListItem() { Text = "Inactive", Value = "false" });

            if (QuestionId != null && QuestionId != 0)
            {
                _questionFacade = new QuestionFacade();
                var questionItemEntity = _questionFacade.GetQuestionById(QuestionId);

                questionVM.QuestionId = questionItemEntity.QuestionId;
                questionVM.QuestionName = questionItemEntity.QuestionName;
                questionVM.Status = questionItemEntity.Status;
                if (questionItemEntity.CreateDate != null)
                    questionVM.CreateDate = questionItemEntity.CreateDate.Value;
                if (questionItemEntity.UpdateDate != null)
                    questionVM.UpdateDate = questionItemEntity.UpdateDate.Value;
                questionVM.CreateUserName = questionItemEntity.CreateUserName;
                questionVM.UpdateUserName = questionItemEntity.UpdateUserName;
                questionVM.CreateUser = questionItemEntity.CreateUser;
                questionVM.UpdateUser = questionItemEntity.UpdateUser;
            }

            return View(questionVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchQuestionList(QuestionSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Question").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _questionFacade = new QuestionFacade();
                    QuestionViewModel questionVM = new QuestionViewModel();
                    questionVM.SearchFilter = searchFilter;

                    questionVM.QuestionList = _questionFacade.GetQuestionList(questionVM.SearchFilter);
                    ViewBag.PageSize = questionVM.SearchFilter.PageSize;

                    return PartialView("~/Views/Question/_QuestionList.cshtml", questionVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Question").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
    }
}
