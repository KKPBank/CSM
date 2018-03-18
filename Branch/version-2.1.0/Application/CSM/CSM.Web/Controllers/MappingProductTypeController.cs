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
    public class MappingProductTypeController : BaseController
    {
        private ISrPageFacade _srPageFacade;
        private IQuestionGroupFacade _questionGroupFacade;
        private IMappingProductTypeFacade _mappingProductTypeFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MappingProductTypeController));

        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        public ActionResult Search()
        {
            var mappingProductVM = new MappingProductTypeViewModel();
            mappingProductVM.SearchFilter = new MappingProductSearchFilter()
            {
                ProductGroupId = null,
                ProductId = null,
                CampaignId = null,
                TypeId = null,
                AreaId = null,
                SubAreaId = null,
                IsVerify = null,
                OwnerId = null,
                IsActive = null,
                PageNo = 1,
                PageSize = 15,
                SortField = "AreaName",
                SortOrder = "ASC",
                IsVerifyOTP = null
            };

            return View(mappingProductVM);
        }

        public ActionResult Create()
        {
            _srPageFacade = new SrPageFacade();

            var mappingProductTypeVM = new MappingProductTypeViewModel();

            var srPageList = _srPageFacade.GetSrPageList();
            mappingProductTypeVM.SrPageList = srPageList.Select(item => new SelectListItem()
            {
                Text = item.SrPageName,
                //Value = item.SrPageId.ToString(CultureInfo.InvariantCulture)
                Value = item.SrPageId.ToString()
            }).ToList();

            mappingProductTypeVM.QuestionGroupSearchFilter = new QuestionSelectSearchFilter()
            {
                QuestionName = string.Empty,
                QuestionIdList = string.Empty,
                ProductId = null,
                PageSize = 15,
                PageNo = 1,
                SortField = "test",
                SortOrder = "ASC"
            };

            ViewBag.CreateUsername = UserInfo.FullName;
            ViewBag.UpdateUsername = UserInfo.FullName;
            ViewBag.CreateDate = DateTime.Now;
            ViewBag.UpdateDate = DateTime.Now;

            ViewBag.VerifyOTPList = new SelectList(new SelectListItem[]
                                        {
                                            new SelectListItem() { Value = "true", Text = "Yes" },
                                            new SelectListItem() { Value = "false", Text ="No" },
                                        },
                                        "Value", "Text", string.Empty);

            using (MappingProductTypeFacade _mapFacade = new MappingProductTypeFacade())
            {
                List<SelectListItem> otpTemplate = new List<SelectListItem>();
                _mapFacade.GetOTPTemplate().ForEach(o => otpTemplate.Add(new SelectListItem() { Value = o.OTPTemplateId.ToString(), Text = o.OTPTemplateName }));
                ViewBag.OTPTemplateList = new SelectList(otpTemplate, "Value", "Text", string.Empty);
            }

            using (HpFacade _hpFacade = new HpFacade())
            {
                List<SelectListItem> hpStatus = new List<SelectListItem>();
                _hpFacade.GetHpStatus().ForEach(o => hpStatus.Add(new SelectListItem() { Value = o.HpStatusId.ToString(), Text = $"{o.HpLangIndeCode}-{o.HpSubject}" }));
                ViewBag.HpStatusList = new SelectList(hpStatus, "Value", "Text", string.Empty);
            }

            return View(mappingProductTypeVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetHpStatusById(int? id)
        {
            using (HpFacade _hpFacade = new HpFacade())
            {
                HpStatusEntity stat = _hpFacade.GetHpStatusById(id);
                if (stat != null)
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        HpStatusId = stat.HpStatusId,
                        HpLangIndeCode = stat.HpLangIndeCode,
                        HpSubject = stat.HpSubject
                    });
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        ErrorMessage = $"No HpStatus Id = {id}"
                    });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(MappingProductTypeSaveModel model)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Mapping Product Save").ToInputLogString());
            try
            {
                if (ModelState.IsValid)
                {
                    MappingProductTypeItemEntity mappingProductTypeItemEntity = new MappingProductTypeItemEntity()
                    {
                        MapProductId = model.MapProductId,
                        ProductId = model.ProductId,
                        CampaignServiceId = model.CampaignServiceId,
                        AreaId = model.AreaId,
                        SubAreaId = model.SubAreaId,
                        TypeId = model.TypeId,
                        OwnerBranchId = model.OwnerUserId,
                        OwnerUserId = model.OwnerSrId,
                        SrPageId = model.SrPageId,
                        IsVerify = model.IsVerify,
                        IsActive = model.IsActive,
                        UserId = UserInfo.UserId,
                        IsVerifyOTP = model.IsVerifyOTP,
                        IsSRSecret = model.IsSRSecret,
                        OTPTemplate = new OTPTemplateEntity() { OTPTemplateId = model.OTPTemplateId ?? 0 },
                        HPLanguageIndependentCode = model.HpLangIndeCode,
                        HPSubject = model.HpSubject
                    };

                    _mappingProductTypeFacade = new MappingProductTypeFacade();

                    //check duplicate mapping product
                    var isDuplicate = _mappingProductTypeFacade.CheckDuplicateMappProduct(mappingProductTypeItemEntity);
                    if (isDuplicate)
                    {
                        return Json(new
                        {
                            is_success = false,
                            message = "Map product ซ้ำ ไม่สามารบันทึกได้"
                        });
                    }

                    var list = new JavaScriptSerializer().Deserialize<ProductQuestionGroupItemEntity[]>(model.QuestionGroupList);
                    List<ProductQuestionGroupItemEntity> productQuestionGroupItemEntityList = new List<ProductQuestionGroupItemEntity>();

                    foreach (var item in list)
                    {
                        ProductQuestionGroupItemEntity productQuestionGroupItemEntity = new ProductQuestionGroupItemEntity
                        {
                            id = item.id,
                            pass_value = item.pass_value,
                            seq = item.seq
                        };

                        productQuestionGroupItemEntityList.Add(productQuestionGroupItemEntity);
                    }

                    var isSuccess = _mappingProductTypeFacade.SaveMapProduct(mappingProductTypeItemEntity,

                        productQuestionGroupItemEntityList);
                    return isSuccess
                        ? Json(new { is_success = true, message = "บันทึก Map Product Question สำเร็จ" })
                        : Json(new { is_success = false, message = "บันทึก Map Product Question ไม่สำเร็จ" });
                }

                return Json(new
                {
                    is_success = false,
                    message = string.Empty
                });
            }
            catch (Exception ex)
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Mapping Product Save").ToFailLogString());
                return Json(new { is_success = false, message = string.Format(CultureInfo.InvariantCulture, "Error : {0}", ex.Message) });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchMappingList(MappingProductSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Mapping").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _mappingProductTypeFacade = new MappingProductTypeFacade();
                    MappingProductTypeViewModel mappingVM = new MappingProductTypeViewModel();
                    mappingVM.SearchFilter = searchFilter;
                    mappingVM.MappingProductList = _mappingProductTypeFacade.GetMappingList(mappingVM.SearchFilter);
                    ViewBag.PageSize = mappingVM.SearchFilter.PageSize;
                    return PartialView("~/Views/MappingProductType/_MappingProductList.cshtml", mappingVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Mapping").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? mapProductId)
        {
            if (mapProductId.HasValue)
            {
                var model = new MappingProductTypeEditModel();

                _srPageFacade = new SrPageFacade();

                var srPageList = _srPageFacade.GetSrPageList();
                model.SrPageList = srPageList.Select(item => new SelectListItem()
                {
                    Text = item.SrPageName,
                    //Value = item.SrPageId.ToString(CultureInfo.InvariantCulture)
                    Value = item.SrPageId.ToString()
                }).ToList();

                _mappingProductTypeFacade = new MappingProductTypeFacade();
                MappingProductTypeItemEntity mapItemEntity = _mappingProductTypeFacade.GetMappingById(mapProductId.Value);
                model.MapProductId = mapItemEntity.MapProductId;
                model.ProductGroupId = mapItemEntity.ProductGroupId;
                model.ProductGroupName = mapItemEntity.ProductGroupName;
                model.ProductId = mapItemEntity.ProductId;
                model.ProductName = mapItemEntity.ProductName;
                model.CampaignServiceId = mapItemEntity.CampaignServiceId;
                model.CampaignServiceName = mapItemEntity.CampaignName;
                model.AreaId = mapItemEntity.AreaId;
                model.AreaName = mapItemEntity.AreaName;
                model.SubAreaId = mapItemEntity.SubAreaId;
                model.SubAreaName = mapItemEntity.SubAreaName;
                model.TypeId = mapItemEntity.TypeId;
                model.TypeName = mapItemEntity.TypeName;
                model.OwnerBranchId = mapItemEntity.OwnerBranchId;
                model.OwnerBranchName = mapItemEntity.OwnerBranchName;
                model.OwnerSrId = mapItemEntity.OwnerUserId;
                model.OwnerSrName = mapItemEntity.OwnerSrName;
                model.CreateUser = mapItemEntity.CreateUser != null ? mapItemEntity.CreateUser.FullName : "";
                model.UpdateUser = mapItemEntity.UpdateUser != null ? mapItemEntity.UpdateUser.FullName : "";
                model.CreateDate = DateUtil.ToStringAsDateTime(mapItemEntity.CreateDate);
                model.UpdateDate = DateUtil.ToStringAsDateTime(mapItemEntity.UpdateDate);
                model.SrPageId = mapItemEntity.SrPageId;

                model.VerifyList = new List<SelectListItem>();
                if (mapItemEntity.IsVerify)
                {
                    //verify
                    model.VerifyList.Add(new SelectListItem { Text = "Yes", Value = "true", Selected = true });
                    model.VerifyList.Add(new SelectListItem { Text = "No", Value = "false" });
                }
                else
                {
                    model.VerifyList.Add(new SelectListItem { Text = "Yes", Value = "true" });
                    model.VerifyList.Add(new SelectListItem { Text = "No", Value = "false", Selected = true });
                }

                model.ActiveList = new List<SelectListItem>();
                if (mapItemEntity.IsActive)
                {
                    //active
                    model.ActiveList.Add(new SelectListItem { Text = "Active", Value = "true", Selected = true });
                    model.ActiveList.Add(new SelectListItem { Text = "Inactive", Value = "false" });
                }
                else
                {
                    model.ActiveList.Add(new SelectListItem { Text = "Active", Value = "true" });
                    model.ActiveList.Add(new SelectListItem { Text = "Inactive", Value = "false", Selected = true });
                }

                model.SearchFilter = new QuestionGroupEditSearchFilter

                {
                    MapProductId = null,
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "",
                    SortOrder = "ASC"
                };

                model.QuestionGroupSearchFilter = new QuestionSelectSearchFilter
                {
                    QuestionName = string.Empty,
                    QuestionIdList = string.Empty,
                    ProductId = null,
                    PageNo = 1,
                    PageSize = 15,
                    SortField = "",
                    SortOrder = "ASC"
                };

                model.IsActive = mapItemEntity.IsActive;
                model.IsVerify = mapItemEntity.IsVerify;

                ViewBag.VerifyOTPList = new SelectList(new SelectListItem[]
                                            {
                                                new SelectListItem() { Value = "true", Text = "Yes" },
                                                new SelectListItem() { Value = "false", Text ="No" },
                                            },
                                            "Value", "Text", string.Empty);

                using (MappingProductTypeFacade _mapFacade = new MappingProductTypeFacade())
                {
                    List<SelectListItem> otpTemplate = new List<SelectListItem>();
                    _mapFacade.GetOTPTemplate().ForEach(o => otpTemplate.Add(new SelectListItem() { Value = o.OTPTemplateId.ToString(), Text = o.OTPTemplateName }));
                    ViewBag.OTPTemplateList = new SelectList(otpTemplate, "Value", "Text", string.Empty);
                }

                using (HpFacade _hpFacade = new HpFacade())
                {
                    List<SelectListItem> hpStatus = new List<SelectListItem>();
                    _hpFacade.GetHpStatus().ForEach(o => hpStatus.Add(new SelectListItem() { Value = o.HpStatusId.ToString(), Text = $"{o.HpLangIndeCode}-{o.HpSubject}" }));
                    ViewBag.HpStatusList = new SelectList(hpStatus, "Value", "Text", string.Empty);
                }

                model.IsVerifyOTP = mapItemEntity.IsVerifyOTP;
                model.IsSRSecret = mapItemEntity.IsSRSecret;
                model.OTPTemplate = mapItemEntity.OTPTemplate ?? model.OTPTemplate;
                model.HpStatus = mapItemEntity.HpStatus ?? model.HpStatus;

                return View(model);
            }

            return View("Search");
        }

        // This method has not been used
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchQuestionGroup(QuestionGroupEditSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search GroupQuestion").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _mappingProductTypeFacade = new MappingProductTypeFacade();
                    var model = new MappingProductTypeEditModel();
                    model.SearchFilter = searchFilter;

                    model.QuestionGroupList = _mappingProductTypeFacade.GetQuestionGroupById(searchFilter);
                    ViewBag.PageSize = model.SearchFilter.PageSize;

                    return PartialView("~/Views/MappingProductType/_QuestionGroupEditList.cshtml", model);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Group Question").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchQuestionGroupMapList(QuestionSelectSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Mapping").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _mappingProductTypeFacade = new MappingProductTypeFacade();
                    MappingProductTypeViewModel model = new MappingProductTypeViewModel();
                    model.QuestionGroupSearchFilter = searchFilter;
                    model.QuestionGroupList = _mappingProductTypeFacade.GetQuestionGroupList(model.QuestionGroupSearchFilter);
                    ViewBag.PageSize = model.QuestionGroupSearchFilter.PageSize;
                    return PartialView("~/Views/MappingProductType/_QuestionGroupList.cshtml", model);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Mapping").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RenderQuestionGroupList(string questionGroupStr, string questionGroupModalStr, int? deleteId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Render QuestionGroup").Add("QuestionGroupStr", questionGroupStr)
                .Add("QuestionGroupModalStr", questionGroupModalStr).Add("DeleteId", deleteId).ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    var tableModel = new TableViewModel();
                    var questionGroupDataList = new JavaScriptSerializer().Deserialize<QuestionGroupTableViewModel[]>(questionGroupStr);
                    tableModel.QuestionGroupTableList = new List<QuestionGroupTableViewModel>();

                    foreach (var item in questionGroupDataList)
                    {
                        if (!deleteId.HasValue || deleteId != item.QuestionGroupId)
                        {
                            var model = new QuestionGroupTableViewModel
                            {
                                QuestionGroupId = item.QuestionGroupId,
                                QuestionGroupName = item.QuestionGroupName,
                                QuestionGroupNo = item.QuestionGroupNo,
                                QuestionGroupPassAmount = item.QuestionGroupPassAmount
                            };

                            tableModel.QuestionGroupTableList.Add(model);
                        }
                    }

                    if (!string.IsNullOrEmpty(questionGroupModalStr))
                    {
                        var questionGroupModalDataList = new JavaScriptSerializer().Deserialize<QuestionGroupTableViewModel[]>(questionGroupModalStr);

                        foreach (var item in questionGroupModalDataList)
                        {
                            if (!deleteId.HasValue || deleteId != item.QuestionGroupId)
                            {
                                var model = new QuestionGroupTableViewModel
                                {
                                    QuestionGroupId = item.QuestionGroupId,
                                    QuestionGroupName = item.QuestionGroupName,
                                    QuestionGroupNo = item.QuestionGroupNo
                                };

                                tableModel.QuestionGroupTableList.Add(model);
                            }
                        }
                    }

                    return PartialView("~/Views/MappingProductType/_QuestionGroupCreateList.cshtml", tableModel);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RenderQuestionGroupListEdit(string questionGroupStr, string questionGroupModalStr, int? deleteId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Render QuestionGroup").Add("QuestionGroupStr", questionGroupStr)
                .Add("QuestionGroupModalStr", questionGroupModalStr).Add("DeleteId", deleteId).ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    var tableModel = new TableViewModel();
                    var questionGroupDataList = new JavaScriptSerializer().Deserialize<QuestionGroupTableViewModel[]>(questionGroupStr);
                    tableModel.QuestionGroupTableList = new List<QuestionGroupTableViewModel>();

                    foreach (var item in questionGroupDataList)
                    {
                        if (!deleteId.HasValue || deleteId != item.QuestionGroupId)
                        {
                            var model = new QuestionGroupTableViewModel
                            {
                                QuestionGroupId = item.QuestionGroupId,
                                QuestionGroupName = item.QuestionGroupName,
                                QuestionGroupNo = item.QuestionGroupNo,
                                QuestionGroupPassAmount = item.QuestionGroupPassAmount
                            };

                            tableModel.QuestionGroupTableList.Add(model);
                        }
                    }

                    if (!string.IsNullOrEmpty(questionGroupModalStr))
                    {
                        var questionGroupModalDataList = new JavaScriptSerializer().Deserialize<QuestionGroupTableViewModel[]>(questionGroupModalStr);

                        foreach (var item in questionGroupModalDataList)
                        {
                            if (!deleteId.HasValue || deleteId != item.QuestionGroupId)
                            {
                                var model = new QuestionGroupTableViewModel
                                {
                                    QuestionGroupId = item.QuestionGroupId,
                                    QuestionGroupName = item.QuestionGroupName,
                                    QuestionGroupNo = item.QuestionGroupNo
                                };

                                tableModel.QuestionGroupTableList.Add(model);
                            }
                        }
                    }

                    return PartialView("~/Views/MappingProductType/_QuestionGroupEditList.cshtml", tableModel);
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

        [HttpPost]
        public ActionResult LoadQuestionGroupList(int mapProductId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Load QuestionGroupList").Add("MapProductId", mapProductId).ToInputLogString());

            try
            {
                _mappingProductTypeFacade = new MappingProductTypeFacade();
                var tableModel = new TableViewModel();
                tableModel.QuestionGroupTableEditList = new List<QuestionGroupEditTableItemEntity>();
                tableModel.QuestionGroupTableEditList = _mappingProductTypeFacade.GetLoadQuestionGroupById(mapProductId);
                return PartialView("~/Views/MappingProductType/_QuestionGroupEditList.cshtml", tableModel);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Render SubArea").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoveDownTable(string questionGroupStr, int seq)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Down Table").Add("QuestionGroupStr", questionGroupStr).Add("Sequence", seq).ToInputLogString());

            try
            {
                var tableModel = new TableViewModel();
                tableModel.QuestionGroupTableList = new List<QuestionGroupTableViewModel>();
                var questionGroupDataList = new JavaScriptSerializer().Deserialize<QuestionGroupTableViewModel[]>(questionGroupStr);

                for (int i = 0; i < questionGroupDataList.Count(); i++)
                {
                    var model = new QuestionGroupTableViewModel();

                    if (i == 0)
                    {
                        if (questionGroupDataList[i].QuestionSeq == seq)
                        {
                            model.QuestionGroupId = questionGroupDataList[i + 1].QuestionGroupId;
                            model.QuestionGroupName = questionGroupDataList[i + 1].QuestionGroupName;
                            model.QuestionGroupNo = questionGroupDataList[i + 1].QuestionGroupNo;
                            model.QuestionGroupPassAmount = questionGroupDataList[i + 1].QuestionGroupPassAmount;
                            model.QuestionSeq = questionGroupDataList[i + 1].QuestionSeq;
                        }
                        else
                        {
                            model.QuestionGroupId = questionGroupDataList[i].QuestionGroupId;
                            model.QuestionGroupName = questionGroupDataList[i].QuestionGroupName;
                            model.QuestionGroupNo = questionGroupDataList[i].QuestionGroupNo;
                            model.QuestionGroupPassAmount = questionGroupDataList[i].QuestionGroupPassAmount;
                            model.QuestionSeq = questionGroupDataList[i].QuestionSeq;
                        }
                    }
                    else
                    {
                        if (questionGroupDataList[i].QuestionSeq == seq)
                        {
                            model.QuestionGroupId = questionGroupDataList[i + 1].QuestionGroupId;
                            model.QuestionGroupName = questionGroupDataList[i + 1].QuestionGroupName;
                            model.QuestionGroupNo = questionGroupDataList[i + 1].QuestionGroupNo;
                            model.QuestionGroupPassAmount = questionGroupDataList[i + 1].QuestionGroupPassAmount;
                            model.QuestionSeq = questionGroupDataList[i + 1].QuestionSeq;
                        }
                        else if (questionGroupDataList[i - 1].QuestionSeq == seq)
                        {
                            model.QuestionGroupId = questionGroupDataList[i - 1].QuestionGroupId;
                            model.QuestionGroupName = questionGroupDataList[i - 1].QuestionGroupName;
                            model.QuestionGroupNo = questionGroupDataList[i - 1].QuestionGroupNo;
                            model.QuestionGroupPassAmount = questionGroupDataList[i - 1].QuestionGroupPassAmount;
                            model.QuestionSeq = questionGroupDataList[i - 1].QuestionSeq;
                        }
                        else
                        {
                            model.QuestionGroupId = questionGroupDataList[i].QuestionGroupId;
                            model.QuestionGroupName = questionGroupDataList[i].QuestionGroupName;
                            model.QuestionGroupNo = questionGroupDataList[i].QuestionGroupNo;
                            model.QuestionGroupPassAmount = questionGroupDataList[i].QuestionGroupPassAmount;
                            model.QuestionSeq = questionGroupDataList[i].QuestionSeq;
                        }
                    }

                    tableModel.QuestionGroupTableList.Add(model);
                }


                return PartialView("~/Views/MappingProductType/_QuestionGroupCreateList.cshtml", tableModel);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Down Table").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewQuestionGroup(int id)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("View Question Group").Add("QuestionGroupId", id).ToSuccessLogString());

            try
            {
                _questionGroupFacade = new QuestionGroupFacade();
                var questionGroup = _questionGroupFacade.GetQuestionGroupById(id);
                var questionList = _questionGroupFacade.GetQuestionList(id);

                var model = new ViewQuestionGroupModel();
                model.Name = questionGroup.QuestionGroupName;
                model.Description = questionGroup.Description;
                model.ProductName = questionGroup.ProductName;
                model.Status = (questionGroup.Status ?? false) ? "Active" : "Inactive";
                model.CreateUser = questionGroup.CreateUserName;
                model.UpdateUser = questionGroup.UpdateUserName;
                model.CreateDate = DateUtil.ToStringAsDateTime(questionGroup.CreateDate);
                model.UpdateDate = DateUtil.ToStringAsDateTime(questionGroup.UpdateDate);

                model.QuestionList = questionList.Select(x => new ViewQuestionModel()
                {
                    SeqNo = x.SeqNo,
                    QuestionName = x.QuestionName
                }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("View Question Group").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
    }
}