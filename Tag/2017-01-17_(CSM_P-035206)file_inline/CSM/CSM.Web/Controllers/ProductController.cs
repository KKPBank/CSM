using System;
using System.Collections;
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
using CSM.Entity.Common;
using Newtonsoft.Json;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class ProductController : BaseController
    {
        private ICommonFacade _commonFacade;
        private IProductFacade _productFacade;
        private ICommPoolFacade _commPoolFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ProductController));

        [CheckUserRole(ScreenCode.SearchSRStatus)]
        public ActionResult Search()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Product").ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();
                _commPoolFacade = new CommPoolFacade();

                var statusList = _commPoolFacade.GetSRStatusSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.srstatus = new SelectList((IEnumerable)statusList, "Key", "Value", string.Empty);

                ProductViewModel productVM = new ProductViewModel();
                productVM.SearchFilter = new ProductSearchFilter
                {
                    ProductGroupId = null,
                    ProductId = null,
                    CampaignId = null,
                    TypeId = null,
                    AreaId = null,
                    SubAreaId = null,
                    FromSRStatus = null,
                    ToSRStatus = null,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "ProductGroup",
                    SortOrder = "DESC"
                };

                ViewBag.PageSize = productVM.SearchFilter.PageSize;
                ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.Message = string.Empty;

                return View(productVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Product").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.SearchSRStatus)]
        public ActionResult ProductList(ProductSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Product").Add("ProductGroup", searchFilter.ProductGroupId)
                .Add("Product", searchFilter.ProductId).Add("Campaign", searchFilter.CampaignId).Add("Type", searchFilter.TypeId)
                .Add("Area", searchFilter.AreaId).Add("SubArea", searchFilter.SubAreaId).Add("FromSRStatus", searchFilter.FromSRStatus)
                .Add("ToSRStatus", searchFilter.ToSRStatus));

            try
            {
                if (ModelState.IsValid)
                {
                    _commonFacade = new CommonFacade();
                    _productFacade = new ProductFacade();

                    ProductViewModel productVM = new ProductViewModel();
                    productVM.SearchFilter = searchFilter;

                    productVM.ProductList = _productFacade.SearchProducts(searchFilter);
                    ViewBag.PageSize = productVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    return PartialView("~/Views/Product/_ProductList.cshtml", productVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Product").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [CheckUserRole(ScreenCode.ManageSRStatus)]
        public ActionResult InitEdit(string jsonData = "{}")
        {
            ProductViewModel productVM = null;
            _commPoolFacade = new CommPoolFacade();

            var toRTStatuses = new Dictionary<string, string>();
            var fromStatuses = _commPoolFacade.GetSRStatusSelectList();
            var toLFStatuses = new Dictionary<string, string>(fromStatuses);
            ProductSearchFilter searchFilter = JsonConvert.DeserializeObject<ProductSearchFilter>(jsonData);

            if (TempData["productVM"] != null)
            {
                productVM = (ProductViewModel)TempData["productVM"];

                if (productVM.ToSRStatusRightId != null && productVM.ToSRStatusRightId.Count > 0)
                {
                    foreach (var s in productVM.ToSRStatusRightId)
                    {
                        var statusId = s.ConvertToString();
                        var statusName = toLFStatuses[statusId];
                        toLFStatuses.RemoveByValue(statusId);
                        toRTStatuses.Add(statusId, statusName);
                    }
                }
            }
            else
            {
                productVM = new ProductViewModel(searchFilter);

                if (searchFilter != null)
                {
                    _productFacade = new ProductFacade();
                    ProductSREntity product = _productFacade.GetProduct(searchFilter);

                    if (product != null)
                    {
                        productVM.FromSRStatusId = product.FromSRStatusId;
                        productVM.FromSRStatusName = product.FromSRStatusName;
                        productVM.FromSRStateId = product.FromSRStateId;
                        productVM.FromSRStateName = product.FromSRStateName;
                        productVM.ProductGroupId = product.ProductGroupId;
                        productVM.ProductGroupName = product.ProductGroupName;
                        productVM.ProductId = product.ProductId;
                        productVM.ProductName = product.ProductName;
                        productVM.CampaignId = product.CampaignId;
                        productVM.CampaignName = product.CampaignName;
                        productVM.TypeId = product.TypeId;
                        productVM.TypeName = product.TypeName;
                        productVM.AreaId = product.AreaId;
                        productVM.AreaName = product.AreaName;
                        productVM.SubAreaId = product.SubAreaId;
                        productVM.SubAreaName = product.SubAreaName;
                        productVM.IsEdit = product.IsEdit;

                        productVM.CreateUser = product.CreateUser != null ? product.CreateUser.FullName : "";
                        productVM.CreatedDate = product.CreatedDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                        productVM.UpdateDate = product.Updatedate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                        productVM.UpdateUser = product.UpdateUser != null ? product.UpdateUser.FullName : "";
                        
                        toRTStatuses = product.ToSRStatusList.ToDictionary(x => x.SRStatusId.ConvertToString(), x => x.SRStatusName);
                        productVM.ToSRStatusRightList = new MultiSelectList(toRTStatuses, "Key", "Value");

                        foreach (var s in toRTStatuses)
                        {
                            toLFStatuses.RemoveByValue(s.Key);
                        }
                    }
                }
            }

            productVM.FromStatusList = new SelectList((IEnumerable)fromStatuses, "Key", "Value", string.Empty);
            productVM.ToSRStatusLeftList = new MultiSelectList(toLFStatuses, "Key", "Value");
            productVM.ToSRStatusRightList = new MultiSelectList(toRTStatuses, "Key", "Value");

            if (productVM.IsEdit == false)
            {
                // default UserLogin
                if (this.UserInfo != null)
                {
                    var today = DateTime.Now;
                    productVM.CreateUser = this.UserInfo.FullName;
                    productVM.CreatedDate = today.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                    productVM.UpdateDate = today.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                    productVM.UpdateUser = this.UserInfo.FullName;
                }
            }


            return View("~/Views/Product/Edit.cshtml", productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ManageSRStatus)]
        public ActionResult Edit(ProductViewModel productVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Save SR Status").Add("ProductGroupId", productVM.ProductGroupId)
                .Add("ProductId", productVM.ProductId).Add("CampaignId", productVM.CampaignId)
                .Add("TypeId", productVM.TypeId).Add("AreaId", productVM.AreaId).Add("SubAreaId", productVM.SubAreaId)
                .Add("FromSRStatusId", productVM.FromSRStatusId).Add("ToSRStatusId", productVM.ToSRStatusLeftId));

            try
            {
                if (ModelState.IsValid)
                {
                    ProductSREntity product = new ProductSREntity
                    {
                        ProductGroupId = productVM.ProductGroupId,
                        ProductId = productVM.ProductId,
                        CampaignId = productVM.CampaignId,
                        TypeId = productVM.TypeId,
                        AreaId = productVM.AreaId,
                        SubAreaId = productVM.SubAreaId,
                        FromSRStatusId = productVM.FromSRStatusId ?? 0,
                        FromSRStatusName = productVM.FromSRStatusName,
                        FromSRStateId = productVM.FromSRStateId ?? 0,
                        FromSRStateName = productVM.FromSRStateName,
                        ToSRStatusIds = productVM.ToSRStatusRightId,
                        IsEdit = productVM.IsEdit,
                        CreateUser = UserInfo,
                        UpdateUser = UserInfo
                    };

                    _productFacade = new ProductFacade();

                    #region "Check Duplicate"

                    if (productVM.IsEdit == false)
                    {
                        if(_productFacade.IsDuplicateSRStatus(product) == true)
                        {
                            ViewBag.ErrorMessage = Resource.Error_SaveFailedDuplicate;
                            goto Outer;
                        }
                    }

                    #endregion

                    bool success = _productFacade.SaveSRStatus(product);
                    if (success)
                    {
                        return RedirectToAction("Search", "Product");
                    }
                    ViewBag.ErrorMessage = Resource.Error_SaveFailed;
                }

                Outer:
                TempData["productVM"] = productVM;
                return InitEdit();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save SR Status").ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ManageSRStatus)]
        public ActionResult DeleteStatusChanges(string jsonData = "{}")
        {
            ProductSearchFilter searchFilter = JsonConvert.DeserializeObject<ProductSearchFilter>(jsonData);

            try
            {
                _productFacade = new ProductFacade();
                bool success = _productFacade.DeleteSRStatus(searchFilter);

                return Json(new
                {
                    Valid = success,
                    Error = success ? string.Empty : Resource.Error_SaveFailed
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Status Changes").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = Resource.Error_System,
                    Errors = string.Empty
                });
            }
        }

        [HttpGet]
        public JsonResult SearchByProductGroupName(string searchTerm, int pageSize, int pageNum, int? productId, int? campaignId)
        {
            _productFacade = new ProductFacade();
            List<ProductGroupEntity> productgroups = _productFacade.GetProductGroupByName(searchTerm, pageSize, pageNum, productId, campaignId);
            int productgroupCount = _productFacade.GetProductGroupCountByName(searchTerm, pageSize, pageNum, productId, campaignId);

            //Select2PagedResult pagedBranches = ProductGroupToSelect2Format(productgroups, productgroupCount);

            Select2PagedResult pagedBranches = new Select2PagedResult();
            pagedBranches.Results = new List<Select2Result>();

            foreach (ProductGroupEntity productgroup in productgroups)
            {
                pagedBranches.Results.Add(new Select2Result { id = productgroup.ProductGroupId, text = productgroup.ProductGroupName });
            }

            pagedBranches.Total = productgroupCount;

            return Json(pagedBranches, JsonRequestBehavior.AllowGet);
        }

        //private Select2PagedResult ProductGroupToSelect2Format(List<ProductGroupEntity> productgroups, int total)
        //{
        //    Select2PagedResult jsonAction = new Select2PagedResult();
        //    jsonAction.Results = new List<Select2Result>();

        //    foreach (ProductGroupEntity productgroup in productgroups)
        //    {
        //        jsonAction.Results.Add(new Select2Result { id = productgroup.ProductGroupId, text = productgroup.ProductGroupName });
        //    }

        //    jsonAction.Total = total;

        //    return jsonAction;
        //}

        [HttpGet]
        public JsonResult SearchByProductName(string searchTerm, int pageSize, int pageNum, int? productGroupId, int? campaignId)
        {
            _productFacade = new ProductFacade();
            List<ProductEntity> products = _productFacade.GetProductByName(searchTerm, pageSize, pageNum, productGroupId, campaignId);
            int productCount = _productFacade.GetProductCountByName(searchTerm, pageSize, pageNum, productGroupId, campaignId);

            //Select2PagedResult pagedBranches = ProductToSelect2Format(products, productCount);

            Select2PagedResult pagedBranches = new Select2PagedResult();
            pagedBranches.Results = new List<Select2Result>();

            foreach (ProductEntity product in products)
            {
                pagedBranches.Results.Add(new Select2Result { id = product.ProductId, text = product.ProductName });
            }

            pagedBranches.Total = productCount;

            return Json(pagedBranches, JsonRequestBehavior.AllowGet);
        }

        //private Select2PagedResult ProductToSelect2Format(List<ProductEntity> products, int total)
        //{
        //    Select2PagedResult jsonAction = new Select2PagedResult();
        //    jsonAction.Results = new List<Select2Result>();

        //    foreach (ProductEntity product in products)
        //    {
        //        jsonAction.Results.Add(new Select2Result { id = product.ProductId, text = product.ProductName });
        //    }

        //    jsonAction.Total = total;

        //    return jsonAction;
        //}

        [HttpGet]
        public JsonResult SearchByCampaignName(string searchTerm, int pageSize, int pageNum, int? productGroupId, int? productId)
        {
            _productFacade = new ProductFacade();
            List<CampaignServiceEntity> campaigns = _productFacade.GetCampaignServiceByName(searchTerm, pageSize, pageNum, productGroupId, productId);
            int campaignsCount = _productFacade.GetCampaignServiceCountByName(searchTerm, pageSize, pageNum, productGroupId, productId);

            //Select2PagedResult pagedBranches = CampaignToSelect2Format(campaigns, campaignsCount);

            Select2PagedResult pagedBranches = new Select2PagedResult();
            pagedBranches.Results = new List<Select2Result>();

            foreach (CampaignServiceEntity campaign in campaigns)
            {
                pagedBranches.Results.Add(new Select2Result { id = campaign.CampaignServiceId, text = campaign.CampaignServiceName });
            }

            pagedBranches.Total = campaignsCount;

            return Json(pagedBranches, JsonRequestBehavior.AllowGet);
        }

        //private Select2PagedResult CampaignToSelect2Format(List<CampaignServiceEntity> campaigns, int total)
        //{
        //    Select2PagedResult jsonAction = new Select2PagedResult();
        //    jsonAction.Results = new List<Select2Result>();

        //    foreach (CampaignServiceEntity campaign in campaigns)
        //    {
        //        jsonAction.Results.Add(new Select2Result { id = campaign.CampaignServiceId, text = campaign.CampaignServiceName });
        //    }

        //    jsonAction.Total = total;

        //    return jsonAction;
        //}

        [HttpGet]
        public JsonResult SearchByTypeName(string searchTerm, int pageSize, int pageNum)
        {
            //Get the paged results and the total count of the results for this query. 
            _productFacade = new ProductFacade();
            List<TypeEntity> types = _productFacade.GetTypeByName(searchTerm, pageSize, pageNum);
            int typesCount = _productFacade.GetTypeCountByName(searchTerm, pageSize, pageNum);

            //Translate the attendees into a format the select2 dropdown expects
            //Select2PagedResult pagedBranches = TypeToSelect2Format(types, typesCount);

            Select2PagedResult pagedBranches = new Select2PagedResult();
            pagedBranches.Results = new List<Select2Result>();

            foreach (TypeEntity type in types)
            {
                pagedBranches.Results.Add(new Select2Result { id = type.TypeId, text = type.TypeName });
            }

            pagedBranches.Total = typesCount;

            //Return the data as a jsonp result
            return Json(pagedBranches, JsonRequestBehavior.AllowGet);
        }

        //private Select2PagedResult TypeToSelect2Format(List<TypeEntity> types, int total)
        //{
        //    Select2PagedResult jsonAction = new Select2PagedResult();
        //    jsonAction.Results = new List<Select2Result>();

        //    foreach (TypeEntity type in types)
        //    {
        //        jsonAction.Results.Add(new Select2Result { id = type.TypeId, text = type.TypeName });
        //    }

        //    jsonAction.Total = total;

        //    return jsonAction;
        //}

        [HttpGet]
        public JsonResult SearchByAreaName(string searchTerm, int pageSize, int pageNum, int? subAreaId)
        {
            //Get the paged results and the total count of the results for this query. 
            _productFacade = new ProductFacade();
            List<AreaEntity> areas = _productFacade.GetAreaByName(searchTerm, pageSize, pageNum, subAreaId);
            int areasCount = _productFacade.GetAreaCountByName(searchTerm, pageSize, pageNum, subAreaId);

            //Translate the attendees into a format the select2 dropdown expects
            //Select2PagedResult pagedBranches = AreaToSelect2Format(areas, areasCount);

            Select2PagedResult pagedBranches = new Select2PagedResult();
            pagedBranches.Results = new List<Select2Result>();

            foreach (AreaEntity area in areas)
            {
                pagedBranches.Results.Add(new Select2Result { id = area.AreaId, text = area.AreaName });
            }

            pagedBranches.Total = areasCount;

            //Return the data as a jsonp result
            return Json(pagedBranches, JsonRequestBehavior.AllowGet);
        }

        //private Select2PagedResult AreaToSelect2Format(List<AreaEntity> areas, int total)
        //{
        //    Select2PagedResult jsonAction = new Select2PagedResult();
        //    jsonAction.Results = new List<Select2Result>();

        //    foreach (AreaEntity area in areas)
        //    {
        //        jsonAction.Results.Add(new Select2Result { id = area.AreaId, text = area.AreaName });
        //    }

        //    jsonAction.Total = total;

        //    return jsonAction;
        //}

        [HttpGet]
        public JsonResult SearchBySubAreaName(string searchTerm, int pageSize, int pageNum, int? areaId)
        {
            //Get the paged results and the total count of the results for this query. 
            _productFacade = new ProductFacade();
            List<SubAreaEntity> subareas = _productFacade.GetSubAreaByName(searchTerm, pageSize, pageNum, areaId);
            int subareasCount = _productFacade.GetSubAreaCountByName(searchTerm, pageSize, pageNum, areaId);

            //Translate the attendees into a format the select2 dropdown expects
            //Select2PagedResult pagedBranches = SubAreaToSelect2Format(subareas, subareasCount);

            Select2PagedResult pagedBranches = new Select2PagedResult();
            pagedBranches.Results = new List<Select2Result>();

            foreach (SubAreaEntity subarea in subareas)
            {
                pagedBranches.Results.Add(new Select2Result { id = subarea.SubareaId, text = subarea.SubareaName });
            }

            pagedBranches.Total = subareasCount;

            //Return the data as a jsonp result
            return Json(pagedBranches, JsonRequestBehavior.AllowGet);
        }

        //private Select2PagedResult SubAreaToSelect2Format(List<SubAreaEntity> subareas, int total)
        //{
        //    Select2PagedResult jsonAction = new Select2PagedResult();
        //    jsonAction.Results = new List<Select2Result>();

        //    foreach (SubAreaEntity subarea in subareas)
        //    {
        //        jsonAction.Results.Add(new Select2Result { id = subarea.SubareaId, text = subarea.SubareaName });
        //    }

        //    jsonAction.Total = total;

        //    return jsonAction;
        //}
    }
}
