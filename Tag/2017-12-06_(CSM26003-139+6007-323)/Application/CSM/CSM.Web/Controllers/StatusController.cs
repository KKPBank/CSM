using CSM.Business;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Web.Controllers.Common;
using CSM.Web.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSM.Web.Controllers
{
    public class StatusController : BaseController
    {
        const int AutoCompleteMaxResult = 10;

        private AuditLogEntity _auditLog;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ServiceRequestController));

        #region Autocomplete

        [HttpPost]
        public ActionResult AutoCompleteState(string keyword, int? statusId, int? srId, bool? isAllStatus)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Status :: Auto Complete Status").Add("Keyword", keyword)
                .Add("statusId", statusId).Add("srId", srId).Add("IsAllStatus", isAllStatus).ToInputLogString());
            try
            {
                List<SRStateEntity> result = null;
                using (SrStatusFacade facade = new SrStatusFacade())
                {

                    result = facade.AutoCompleteState(AutoCompleteMaxResult, keyword, statusId, srId, isAllStatus);
                }
                return Json(result.Select(r => new
                {
                    r.SRStateId,
                    r.SRStateName
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Status :: Auto Complete State").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult AutoCompleteStatus(string keyword, int? stateId, int? srId, bool? isAllStatus)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Status :: Auto Complete Status").Add("Keyword", keyword)
                .Add("stateId", stateId)
                .Add("srId", srId)
                .Add("IsAllStatus", isAllStatus).ToInputLogString());
            try
            {
                List<SRStatusEntity> result = null;
                using (SrStatusFacade facade = new SrStatusFacade())
                {

                    result = facade.AutoCompleteStatus(AutoCompleteMaxResult, keyword, stateId, srId, isAllStatus);
                }
                return Json(result.Select(r => new
                {
                    r.SRStatusId,
                    r.SRStatusCode,
                    r.SRStatusName
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Status :: Auto Complete Status").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        #endregion

        public ActionResult Index()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("SR Status Index").ToInputLogString());
            try
            {
                SRStatusViewModel model = new SRStatusViewModel()
                {
                    SearchFilter = new SRStatusSearchFilter()
                    {
                        PageNo = 1,
                        PageSize = 15,
                        SortField = "",
                        SortOrder = ""
                    }
                };
                initList();
                return View(model);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("SR Status Index").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, ControllerContext.RouteData.Values["controller"].ToString(),
                    ControllerContext.RouteData.Values["action"].ToString()));
            }

        }

        private void initList(byte type = 0)
        {
            using (SrStatusFacade statfacade = new SrStatusFacade())
            {
                ViewBag.SRStateList = statfacade.GetSrState()
                                        .OrderBy(x => x.SRStateId)
                                        .Select(x => new SelectListItem()
                                        {
                                            Value = x.SRStateId.ToString(),
                                            Text = x.SRStateName
                                        });
                using (SrPageFacade pageFacade = new SrPageFacade())
                {
                    ViewBag.SRPageList = pageFacade.GetSrPages()
                                            .OrderBy(x => x.SrPageName)
                                            .Select(x => new SelectListItem()
                                            {
                                                Value = x.SrPageId.ToString(),
                                                Text = x.SrPageName
                                            });
                }
                switch (type)
                {
                    case 1:
                        {
                            //Create Page
                        }
                        break;
                    default:
                        {
                            //Search Page
                            ViewBag.SRStatusList = statfacade.GetSrStatus()
                            .OrderBy(x => x.SRStatusName)
                            .Select(x => new SelectListItem()
                            {
                                Value = x.SRStatusId.ToString(),
                                Text = x.SRStatusName
                            });
                        }
                        break;
                }
            }
        }

        [HttpPost]
        public ActionResult Search(SRStatusSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("SR Status List").ToInputLogString());
            try
            {
                if (ModelState.IsValid)
                {
                    SearchSRStatusModel model = new SearchSRStatusModel();
                    model.SearchFilter = searchFilter;

                    if (searchFilter.Status == "all") { searchFilter.Status = null; }
                    using (SrStatusFacade facade = new SrStatusFacade())
                    {
                        model.SearchList = facade.SearchSrStatus(searchFilter);
                    }

                    ViewBag.PageSize = model.SearchFilter.PageSize;
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Search SR Status").ToSuccessLogString());
                    return PartialView("~/Views/Status/SearchList.cshtml", model);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search SR Status").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, ControllerContext.RouteData.Values["controller"].ToString(),
                    ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SRStatusViewModel model)
        {
            try
            {
                initList(1);
                List<SrPageItemEntity> pageCurr = null;
                List<SrPageItemEntity> pageAll = null;

                if (!isSaveMode)
                {
                    model = new SRStatusViewModel()
                    {
                        SRState = new SRStateEntity(),
                        SendHP = false,
                        SendRule = false,
                        Status = "A"
                    };

                    model.CreateUser = UserInfo;
                    model.UpdateUser = UserInfo;
                    ModelState.Clear();
                }

                using (SrPageFacade pageFacade = new SrPageFacade())
                {
                    pageAll = pageFacade.GetSrPages()
                                            .OrderBy(x => x.SrPageName)
                                            .ToList();
                }

                model.SRPageIdAll = pageAll.OrderBy(x => x.SrPageName)
                    .Select(x => x.SrPageId ?? 0).ToList();

                if (model.SRPageIdList != null)
                {
                    pageCurr = pageAll.Where(x => model.SRPageIdList.Contains(x.SrPageId ?? 0)).ToList();
                    model.SRPageList = new MultiSelectList(pageCurr.ToDictionary(x => x.SrPageId.ConvertToString(), x => x.SrPageName), "Key", "Value");
                }
                else
                {
                    model.SRPageList = new MultiSelectList(new List<int>() { });
                }

                model.SRPageListAll = new MultiSelectList(pageAll
                                            .OrderBy(x => x.SrPageName)
                                            .ToDictionary(x => x.SrPageId.ConvertToString(), x => x.SrPageName), "Key", "Value");

                model.Old_SRPageIdList = new List<int>() { };

                ViewBag.EditMode = "Create";
                return View("~/Views/Status/Create.cshtml", model);
            }
            catch (Exception ex)
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Catch- SR Status").Add("Error Message", ex.ToString()).ToFailLogString());
                ViewBag.ErrorMessage = ex.ToString();
                return View("~/Views/Status/Create.cshtml", model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id)
        {
            SRStatusViewModel model = null;
            using (SrStatusFacade facade = new SrStatusFacade())
            {
                var st = facade.GetSrStatus(id).FirstOrDefault();
                model = loadDataToViewModel(st);
            }
            return Edit(model);
        }

        private SRStatusViewModel loadDataToViewModel(SRStatusEntity st)
        {
            SRStatusViewModel model = new SRStatusViewModel();

            model.SRStatusId = st.SRStatusId;
            model.SRStatusCode = st.SRStatusCode;
            model.SRStatusName = st.SRStatusName;
            model.SRStateId = st.SRStateId;
            model.SRState = st.SRState;

            model.Status = st.Status;
            model.SendHP = st.SendHP;
            model.SendRule = st.SendRule;

            model.CreateDate = st.CreateDate;
            model.CreateUser = st.CreateUser;
            model.UpdateDate = st.UpdateDate;
            model.UpdateUser = st.UpdateUser;

            return model;
        }

        private ActionResult Edit(SRStatusViewModel model)
        {
            try
            {
                ViewBag.EditMode = "Edit";
                initList(1);

                //Initialize mutiselectlist
                List<SrPageItemEntity> pageCurr = null;
                List<SrPageItemEntity> pageAll = null;

                if (!isSaveMode)
                {
                    using (SrPageFacade pageFacade = new SrPageFacade())
                    {
                        pageCurr = pageFacade.GetSrPages(null, model.SRStatusId).ToList();
                        pageAll = pageFacade.GetSrPages()
                                                .OrderBy(x => x.SrPageName)
                                                .ToList();
                    }

                    model.SRPageIdList = pageCurr.Select(x => x.SrPageId ?? 0).ToList();
                    model.SRPageIdAll = pageAll.OrderBy(x => x.SrPageName)
                                            .Select(x => x.SrPageId ?? 0).ToList();

                    model.Old_SRPageIdList = model.SRPageIdList;
                }
                else
                {
                    using (SrPageFacade pageFacade = new SrPageFacade())
                    {
                        pageAll = pageFacade.GetSrPages()
                                                .OrderBy(x => x.SrPageName)
                                                .ToList();
                    }
                }

                if (model.SRPageIdList != null)
                {
                    model.SRPageListAll = new MultiSelectList(pageAll.Where(x => !model.SRPageIdList.Contains(x.SrPageId ?? 0)).ToDictionary(x => x.SrPageId.ConvertToString(), x => x.SrPageName), "Key", "Value");
                    pageCurr = pageAll.Where(x => model.SRPageIdList.Contains(x.SrPageId ?? 0)).ToList();
                    model.SRPageList = new MultiSelectList(pageCurr.ToDictionary(x => x.SrPageId.ConvertToString(), x => x.SrPageName), "Key", "Value");
                }
                else
                {
                    model.SRPageListAll = new MultiSelectList(pageAll.ToDictionary(x => x.SrPageId.ConvertToString(), x => x.SrPageName), "Key", "Value");
                    model.SRPageList = new MultiSelectList(new List<int>());
                }
                return View("~/Views/Status/Create.cshtml", model);
            }
            catch (Exception ex)
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Catch- SR Status").Add("Error Message", ex.ToString()).ToFailLogString());
                ViewBag.ErrorMessage = ex.ToString();
                return View("~/Views/Status/Create.cshtml", model);
            }
        }

        bool isSaveMode = false;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(SRStatusViewModel model)
        {
            try
            {
                isSaveMode = true;
                if (ModelState.IsValid)
                {
                    string msg;
                    model.CreateUser = UserInfo;
                    model.UpdateUser = UserInfo;

                    using (var facade = new SrStatusFacade())
                    {
                        if (facade.Save(model, out msg))
                        {
                            return Json(new
                            {
                                IsSuccess = true,
                                ErrorMessage = ""
                            });

                        }
                    }
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Save Failed - SR Status").Add("Message", msg).ToFailLogString());
                    return Json(new
                    {
                        IsSuccess = false,
                        ErrorMessage = msg
                    });
                }
                return Json(new
                {
                    IsSuccess = false,
                    ErrorMessage = GetModelValidationErrors()
                                    .Select(x => x.Value.ToString())
                                    .Aggregate((i, j) => i + ", " + j)
                });
            }
            catch (Exception ex)
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save Catch- SR Status").Add("Error Message", ex.ToString()).ToFailLogString());
                ViewBag.ErrorMessage = ex.ToString();
                return Json(new
                {
                    IsSuccess = false,
                    ErrorMessage = ex.ToString()
                });
            }
            finally { isSaveMode = false; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetByState(int stateId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Status :: Auto Complete Status").Add("stateId", stateId)
                .ToInputLogString());
            try
            {
                List<SRStatusEntity> result = null;
                using (SrStatusFacade facade = new SrStatusFacade())
                {

                    result = facade.GetByState(stateId);
                }
                return Json(result.Select(r => new
                {
                    r.SRStatusId,
                    r.SRStatusCode,
                    r.SRStatusName
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Status :: Auto Complete Status").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
    }
}
