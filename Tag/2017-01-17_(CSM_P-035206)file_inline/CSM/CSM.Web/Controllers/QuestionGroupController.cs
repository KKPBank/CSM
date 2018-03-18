using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
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
    public class QuestionGroupController : BaseController
    {
        private IProductFacade _productFacade;
        private IQuestionGroupFacade _questionGroupFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ContactController));

        public ActionResult Index()
        {
            var questionGroupVM = new QuestionGroupViewModel();
            questionGroupVM.QuestionGroupIsActiveList = new List<SelectListItem>();
            questionGroupVM.QuestionGroupIsActiveList.Add(new SelectListItem { Text = "ทั้งหมด", Value = "all" });
            questionGroupVM.QuestionGroupIsActiveList.Add(new SelectListItem { Text = "Active", Value = "true" });
            questionGroupVM.QuestionGroupIsActiveList.Add(new SelectListItem { Text = "Inactive", Value = "false" });
            questionGroupVM.Status = true;

            questionGroupVM.SearchFilter = new QuestionGroupSearchFilter
            {
                QuestionGroupName = null,
                ProductId = null,
                Status = "",
                PageNo = 1,
                PageSize = 15,
                SortField = string.Empty,
                SortOrder = "ASC"
            };

            return View(questionGroupVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? questionGroupId)
        {
            if (questionGroupId.HasValue)
            {
                var questionGroupVM = new QuestionGroupEditViewModel();
                _questionGroupFacade = new QuestionGroupFacade();

                //get area section
                QuestionGroupItemEntity questionGroupItemEntity = _questionGroupFacade.GetQuestionGroupById(questionGroupId.Value);
                questionGroupVM.QuestionGroupId = questionGroupItemEntity.QuestionGroupId;
                questionGroupVM.QuestionGroupName = questionGroupItemEntity.QuestionGroupName;
                questionGroupVM.QuestionGroupProductName = questionGroupItemEntity.QuestionGroupProduct;
                questionGroupVM.QuestionGroupProductId = questionGroupItemEntity.QuestionGroupProductId;
                questionGroupVM.CreateUserName = questionGroupItemEntity.CreateUserName != null ? questionGroupItemEntity.CreateUserName.FullName : "";
                questionGroupVM.UpdateUserName = questionGroupItemEntity.UpdateUserName != null ? questionGroupItemEntity.UpdateUserName.FullName : "";
                questionGroupVM.DisplayCreateDate = DateUtil.ToStringAsDateTime(questionGroupItemEntity.CreateDate);
                questionGroupVM.DisplayUpdateDate = DateUtil.ToStringAsDateTime(questionGroupItemEntity.UpdateDate);
                questionGroupVM.QuestionGroupDescription = questionGroupItemEntity.Description;

                questionGroupVM.Status = questionGroupItemEntity.Status;
                questionGroupVM.StatusList = new List<SelectListItem>();
                questionGroupVM.StatusList.Add(new SelectListItem { Text = "Active", Value = "true" });
                questionGroupVM.StatusList.Add(new SelectListItem { Text = "Inactive", Value = "false" });

                questionGroupVM.QuestionGroupInQuestionSearchFilter = new QuestionGroupInQuestionSearchFilter
                {
                    QuestionGroupId = null,
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "GroupName",
                    SortOrder = "ASC"
                };

                questionGroupVM.SearchFilter = new QuestionSelectSearchFilter
                {
                    QuestionName = string.Empty,
                    QuestionIdList = string.Empty,
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "QuestionName",
                    SortOrder = "ASC"
                };

                // Get sub area section
                return View(questionGroupVM);
            }

            return View("Create");
        }

        public ActionResult Create()
        {
            var model = new QuestionGroupEditViewModel();
            model.SearchFilter = new QuestionSelectSearchFilter
            {
                QuestionName = string.Empty,
                QuestionIdList = string.Empty,
                PageNo = 1,
                PageSize = 15,
                SortField = "QuestionName",
                SortOrder = "ASC"
            };

            ViewBag.PageSize = model.SearchFilter.PageSize;
            ViewBag.Message = string.Empty;

            ViewBag.CreateUsername = UserInfo.FullName;
            ViewBag.UpdateUsername = UserInfo.FullName;
            ViewBag.CreateDate = DateTime.Now;
            ViewBag.UpdateDate = DateTime.Now;

            return View(model);
        }

        [HttpPost]
        public ActionResult GetProductList()
        {
            _productFacade = new ProductFacade();
            return Json(new
            {
                IsSuccess = true,
                Products = _productFacade.GetProductList().Select(p => new
                {
                    p.ProductId,
                    p.ProductName
                }).ToList(),
            });
        }

        [HttpPost]
        public JsonResult Autocomplete(string Prefix)
        {
            _questionGroupFacade = new QuestionGroupFacade();
            var totalCount = 0;
            List<QuestionGroupProductEntity> list = _questionGroupFacade.GetProductList(Prefix, ref totalCount);
            return Json(list.Select(item => new
            {
                Product = item.Product,
                ProductGroup = item.ProductGroup
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchQuestionGroupQuestion(QuestionGroupInQuestionSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search QuestionInQuestionGroup").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _questionGroupFacade = new QuestionGroupFacade();
                    var model = new QuestionGroupEditViewModel();
                    model.QuestionGroupInQuestionSearchFilter = searchFilter;
                    model.QuestionGroupInQuestionList = _questionGroupFacade.GetQuestionListById(model.QuestionGroupInQuestionSearchFilter);

                    return PartialView("~/Views/QuestionGroup/_QuestionInQuestionGroupList.cshtml", model);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search SubArea").Add("Error Message", ex).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchQuestion(BootstrapParameters parameters, string questionName, string questionIdList)
        {
            if (ModelState.IsValid)
            {
                var totalCount = 0;
                _questionGroupFacade = new QuestionGroupFacade();
                List<QuestionItemEntity> list = _questionGroupFacade.SearchQuestionGroupQuestion(parameters.offset, parameters.limit, questionName, questionIdList, ref totalCount);

                return Json(new BootstrapTableResult
                {
                    total = totalCount,
                    rows = list.Select(item => new
                    {
                        id = item.QuestionId,
                        // action = string.Format("<a onClick='editQuestion({0})' class='btnEdit'><i class='fa fa-edit'></i></a>", item.QuestionId),
                        question_name = item.QuestionName,
                        status = item.Status.HasValue ? item.Status.Value ? "Active" : "Inactive" : "Inactive",
                        update_name = string.IsNullOrEmpty(item.UpdateUserName.FullName) ? item.CreateUserName.FullName : item.UpdateUserName.FullName,
                        update_date = !item.UpdateDate.HasValue ? DateUtil.ToStringAsDateTime(item.CreateDate) : DateUtil.ToStringAsDateTime(item.UpdateDate)
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
        public ActionResult SaveQuestionGroup(QuestionGroupSaveViewModel questionGroupSaveVM, string idQuestions)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Save Group Question").ToInputLogString());
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(questionGroupSaveVM.QuestionGroupProductIds))
                    {
                        ModelState.AddModelError("QuestionGroupProductIds", "กรุณาระบุ Product อย่างน้อย 1 รายการ");

                        return Json(new
                        {
                            Valid = false,
                            Error = string.Empty,
                            Errors = GetModelValidationErrors()
                        });
                    }

                    _questionGroupFacade = new QuestionGroupFacade();

                    var productIds = questionGroupSaveVM.QuestionGroupProductIds.Split(',').Select(Int32.Parse).ToList();
                    var questionGroupForSaveList = new List<QuestionGroupItemEntity>();

                    var name = questionGroupSaveVM.QuestionGroupName.NullSafeTrim();
                    var desc = questionGroupSaveVM.Description.NullSafeTrim();

                    var duplicateQuestionGroups = _questionGroupFacade.GetQuestionGroupDuplicates(productIds, name, questionGroupSaveVM.QuestionGroupId);

                    if (duplicateQuestionGroups.Count > 0)
                    {
                        var productNames = string.Join(",", duplicateQuestionGroups.Select(x => x.ProductName).ToList());
                        ModelState.AddModelError("QuestionGroupProductIds", string.Format(CultureInfo.InvariantCulture, "ชื่อกลุ่มคำถามที่บันทึก มีอยู่แล้วใน Product '{0}'", productNames));

                        return Json(new
                        {
                            Valid = false,
                            Error = string.Empty,
                            Errors = GetModelValidationErrors()
                        });
                    }

                    foreach (var productId in productIds)
                    {
                        var item = new QuestionGroupItemEntity
                        {
                            QuestionGroupId = questionGroupSaveVM.QuestionGroupId,
                            QuestionGroupName = name,
                            QuestionGroupProductId = productId,
                            Description = desc,
                            Status = questionGroupSaveVM.Status,
                            UserId = UserInfo.UserId
                        };

                        questionGroupForSaveList.Add(item);
                    }

                    foreach (var questionGroup in questionGroupForSaveList)
                    {
                        _questionGroupFacade.SaveQuestionGroup(questionGroup, idQuestions);
                    }

                    return Json(new { is_success = true, message = "บันทึก Group Question สำเร็จ" });
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save Group Question").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchQuestionGroupList(QuestionGroupSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search QuestionGroup").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _questionGroupFacade = new QuestionGroupFacade();
                    QuestionGroupViewModel questionGroupVM = new QuestionGroupViewModel();
                    questionGroupVM.SearchFilter = searchFilter;
                    questionGroupVM.QuestionGroupList = _questionGroupFacade.GetQuestionGroupList(questionGroupVM.SearchFilter);
                    ViewBag.PageSize = questionGroupVM.SearchFilter.PageSize;
                    return PartialView("~/Views/QuestionGroup/_QuestionGroupList.cshtml", questionGroupVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search QuestionGroup").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchQuestionList(QuestionSelectSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Question").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _questionGroupFacade = new QuestionGroupFacade();
                    var model = new QuestionGroupEditViewModel();
                    model.SearchFilter = searchFilter;
                    model.QuestionList = _questionGroupFacade.GetQuestionList(model.SearchFilter);
                    ViewBag.PageSize = model.SearchFilter.PageSize;
                    return PartialView("~/Views/QuestionGroup/_QuestionList.cshtml", model);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RenderQuestionGroupList(string questionGroupList, string questionList, int? deleteId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Render QuestionGroup").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    var questionGroupTableModel = new QuestionGroupViewModel();
                    var questionDataList = new JavaScriptSerializer().Deserialize<QuestionGroupTableModel[]>(questionList);
                    questionGroupTableModel.QuestionGroupTableList = new List<QuestionGroupTableModel>();

                    if (!string.IsNullOrEmpty(questionGroupList))
                    {
                        var questionGroupDataList = new JavaScriptSerializer().Deserialize<QuestionGroupTableModel[]>(questionGroupList);

                        foreach (var item in questionGroupDataList)
                        {
                            if (!deleteId.HasValue || deleteId != item.id)
                            {
                                var model = new QuestionGroupTableModel
                                {
                                    id = item.id,
                                    question_name = item.question_name
                                };

                                questionGroupTableModel.QuestionGroupTableList.Add(model);
                            }
                        }
                    }

                    foreach (var item in questionDataList)
                    {
                        if (!deleteId.HasValue || deleteId != item.id)
                        {
                            var model = new QuestionGroupTableModel
                            {
                                id = item.id,
                                question_name = item.question_name
                            };

                            questionGroupTableModel.QuestionGroupTableList.Add(model);
                        }
                    }

                    // questionGroupTableModel.QuestionGroupTableList = questionGroupTableModel.QuestionGroupTableList.OrderBy(q => q.id).ToList();
                    return PartialView("~/Views/QuestionGroup/_RenderQuestionList.cshtml", questionGroupTableModel);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Render QuestionGroup").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
    }
}
