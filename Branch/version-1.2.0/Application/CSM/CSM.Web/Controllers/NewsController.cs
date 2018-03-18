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
using Newtonsoft.Json;
using System.Globalization;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class NewsController : BaseController
    {        
        private INewsFacade _newsFacade;
        private ICommonFacade _commonFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(NewsController));

        [CheckUserRole(ScreenCode.SearchNews)]
        public ActionResult Search()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch News").ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();

                NewsViewModel newsVM = new NewsViewModel();
                newsVM.SearchFilter = new NewsSearchFilter
                {
                    Topic = string.Empty,                   
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "AnnounceDate",
                    SortOrder = "DESC"
                };

                var statusList = _commonFacade.GetStatusSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                newsVM.StatusList = new SelectList((IEnumerable)statusList, "Key", "Value", string.Empty);

                ViewBag.PageSize = newsVM.SearchFilter.PageSize;
                ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.Message = string.Empty;

                return View(newsVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch News").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.SearchNews)]
        public ActionResult NewsList(NewsSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search News").Add("Topic", searchFilter.Topic).ToInputLogString());

            try
            {
                #region "Validation"

                bool isValid = TryUpdateModel(searchFilter);
                if (!string.IsNullOrEmpty(searchFilter.DateFrom) && !searchFilter.AnnounceDate.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("txtFromDate", Resource.ValErr_InvalidDate);
                }
                if (!string.IsNullOrEmpty(searchFilter.DateTo) && !searchFilter.ExpiryDate.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("txtToDate", Resource.ValErr_InvalidDate);
                }
                if (searchFilter.AnnounceDate.HasValue && searchFilter.ExpiryDate.HasValue
                    && searchFilter.AnnounceDate.Value > searchFilter.ExpiryDate.Value)
                {
                    isValid = false;
                    ModelState.AddModelError("dvDateRange", Resource.ValErr_InvalidDateRange);
                }

                #endregion

                if (isValid)
                {
                    _newsFacade = new NewsFacade();
                    _commonFacade = new CommonFacade();
                    NewsViewModel newsVM = new NewsViewModel();
                    newsVM.SearchFilter = searchFilter;

                    newsVM.NewsList = _newsFacade.GetNewsList(newsVM.SearchFilter);
                    ViewBag.PageSize = newsVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Search News").ToSuccessLogString());
                    return PartialView("~/Views/News/_NewsList.cshtml", newsVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search News").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [CheckUserRole(ScreenCode.ManageNews)]
        public ActionResult InitEdit(int? newsId = null)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit News").Add("NewsId", newsId).ToInputLogString());

            NewsViewModel newsVM = null;

            if (TempData["NewsVM"] != null)
            {
                newsVM = (NewsViewModel)TempData["NewsVM"];
            }
            else
            {
                newsVM = new NewsViewModel { NewsId = newsId };
            }
           
            _newsFacade = new NewsFacade();
            _commonFacade = new CommonFacade();

            var statusList = _commonFacade.GetStatusSelectList();
            newsVM.StatusList = new SelectList((IEnumerable)statusList, "Key", "Value", string.Empty);

            if (TempData["NewsVM"] == null && newsVM.NewsId != null)
            {
                NewsEntity newsEntity = _newsFacade.GetNewsByID(newsVM.NewsId.Value);
                newsVM.Topic = newsEntity.Topic;
                newsVM.AnnounceDate = newsEntity.AnnounceDateDisplay;
                newsVM.ExpiryDate = newsEntity.ExpiryDateDisplay;
                newsVM.Content = newsEntity.Content;               
                newsVM.FullName = newsEntity.CreateUser.FullName;
                newsVM.Status = newsEntity.Status;

                // User
                newsVM.CreateUser = newsEntity.CreateUser != null ? newsEntity.CreateUser.FullName : "";
                newsVM.CreatedDate = newsEntity.CreatedDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                newsVM.UpdateDate = newsEntity.UpdateDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                newsVM.UpdateUser = newsEntity.UpdateUser != null ? newsEntity.UpdateUser.FullName : "";

                var newsBranches = _newsFacade.GetNewsBranchList(newsVM.NewsId.Value);
                newsVM.NewsBranchList = newsBranches;
                newsVM.JsonBranch = JsonConvert.SerializeObject(newsBranches);

                var newsAttachments = _newsFacade.GetNewsAttachmentList(newsVM.NewsId.Value);
                newsVM.AttachmentList = newsAttachments;
                newsVM.JsonAttach = JsonConvert.SerializeObject(newsAttachments);

                
            }
            else if (TempData["NewsVM"] == null)
            {
                // Default UserLogin
                if(this.UserInfo != null)
                {
                    newsVM.FullName = this.UserInfo.FullName; // ผู้ประกาศ

                    var today = DateTime.Now;
                    newsVM.CreateUser = this.UserInfo.FullName;
                    newsVM.CreatedDate = today.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                    newsVM.UpdateDate = today.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                    newsVM.UpdateUser = this.UserInfo.FullName;
                }                

                newsVM.NewsBranchList = _newsFacade.GetNewsBranchList(newsVM.SelectedBranch)
                    .Where(x => x.IsDelete == false).ToList();
            }
            else
            {
                newsVM.NewsBranchList = _newsFacade.GetNewsBranchList(newsVM.SelectedBranch)
                   .Where(x => x.IsDelete == false).ToList();
            }

            return View("~/Views/News/Edit.cshtml", newsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ManageNews)]
        public ActionResult Edit(NewsViewModel newsVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Save News").Add("NewsId", newsVM.NewsId)
                .Add("Topic", newsVM.Topic).ToInputLogString());

            try
            {
                #region "Validation"

                bool isValid = TryUpdateModel(newsVM);
                string content = ApplicationHelpers.StripHtmlTags(newsVM.Content);

                if (string.IsNullOrWhiteSpace(content))
                {
                    isValid = false;
                    ModelState.AddModelError("Content", string.Format(CultureInfo.InvariantCulture, Resource.ValErr_RequiredField, Resource.Lbl_Content));
                }
                if (!string.IsNullOrEmpty(newsVM.AnnounceDate) && !newsVM.AnnounceDateValue.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("AnnounceDate", Resource.ValErr_InvalidDate);
                }
                else if (newsVM.AnnounceDateValue.HasValue)
                {
                    // เช็คห้ามเลือกวันที่ย้อนหลังเฉพาะกรณี เพิ่มข้อมูลใหม่
                    if ((newsVM.NewsId.HasValue == false || newsVM.NewsId == 0) && newsVM.AnnounceDateValue.Value < DateTime.Now.Date)
                    {
                        isValid = false;
                        ModelState.AddModelError("AnnounceDate", Resource.ValErr_InvalidDate_MustMoreThanToday);
                    }
                }

                if (!string.IsNullOrEmpty(newsVM.ExpiryDate) && !newsVM.ExpiryDateValue.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("ExpiryDate", Resource.ValErr_InvalidDate);
                }
                else if (newsVM.ExpiryDateValue.HasValue)
                {
                    // เช็คห้ามเลือกวันที่ย้อนหลังเฉพาะกรณี เพิ่มข้อมูลใหม่
                    if ((newsVM.NewsId.HasValue == false || newsVM.NewsId == 0) && newsVM.ExpiryDateValue.Value < DateTime.Now.Date)
                    {
                        isValid = false;
                        ModelState.AddModelError("ExpiryDate", Resource.ValErr_InvalidDate_MustMoreThanToday);
                    }
                }

                if (newsVM.AnnounceDateValue.HasValue && newsVM.ExpiryDateValue.HasValue
                    && newsVM.AnnounceDateValue.Value > newsVM.ExpiryDateValue.Value)
                {
                    isValid = false;
                    ModelState.AddModelError("AnnounceDate", Resource.ValErr_InvalidDateRange);
                    ModelState.AddModelError("ExpiryDate", "");
                }

                #endregion

                if (isValid)
                {
                    List<NewsBranchEntity> selectedBranch = newsVM.SelectedBranch;

                    // Validate select at least one branch
                    if (!newsVM.SelectedBranch.Any(x => x.IsDelete == false))
                    {
                        ViewBag.ErrorMessage = Resource.ValErr_AtLeastOneItem;
                        goto Outer;
                    }

                    // Validate MaxLength
                    if (newsVM.Content.Count() > Constants.MaxLength.NewsContent)
                    {
                        ModelState.AddModelError("Content", string.Format(CultureInfo.InvariantCulture, Resource.ValErr_StringLength, Resource.Lbl_Content, Constants.MaxLength.NewsContent));
                        goto Outer;
                    }

                    // Save News
                    NewsEntity newsEntity = new NewsEntity
                    {
                        NewsId = newsVM.NewsId,
                        Topic = newsVM.Topic,
                        AnnounceDate = newsVM.AnnounceDate.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate),
                        ExpiryDate = newsVM.ExpiryDate.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate),
                        Content = newsVM.Content,
                        Status = newsVM.Status,                        
                        CreateUserId = this.UserInfo.UserId,
                        UpdateUserId = this.UserInfo.UserId
                    };

                    _commonFacade = new CommonFacade();
                    newsEntity.DocumentFolder = _commonFacade.GetNewsDocumentFolder();

                    _newsFacade = new NewsFacade();
                    bool success = _newsFacade.SaveNews(newsEntity, selectedBranch, newsVM.AttachmentList);

                    if (success)
                    {
                        return RedirectToAction("Search", "News");
                    }

                    ViewBag.ErrorMessage = Resource.Error_SaveFailed;
                }

            Outer:
                TempData["NewsVM"] = newsVM;
                return InitEdit();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save News").Add("Exception occur:\n", ex).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [CheckUserRole(ScreenCode.ManageNews)]
        public ActionResult NewsBranchList(NewsViewModel newsVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get NewsBranch List").Add("JsonBranch", newsVM.JsonBranch).ToInputLogString());

            try
            {
                _newsFacade = new NewsFacade();
                newsVM.NewsBranchList = _newsFacade.GetNewsBranchList(newsVM.SelectedBranch)
                    .Where(x => x.IsDelete == false).ToList();

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get NewsBranch List").ToSuccessLogString());
                return PartialView("~/Views/News/_NewsBranchList.cshtml", newsVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get NewsBranch List").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [CheckUserRole(ScreenCode.ManageNews)]
        public JsonResult DeleteBranch(NewsViewModel newsVM, int branchId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete NewsBranch").Add("NewsId", newsVM.NewsId).Add("BranchId", branchId).ToInputLogString());

            try
            {
                var deletedBranch = newsVM.SelectedBranch.FirstOrDefault(x => x.BranchId == branchId);

                if (newsVM.NewsId == null)
                {
                    newsVM.SelectedBranch.Remove(deletedBranch);
                }
                else
                {
                    deletedBranch.IsDelete = true;
                }

                return Json(new
                {
                    Valid = true,
                    Data = JsonConvert.SerializeObject(newsVM.SelectedBranch)
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete NewsBranch").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = Resource.Error_System,
                    Errors = string.Empty
                });
            }
        }

        [CheckUserRole(ScreenCode.ManageNews)]
        public ActionResult NewsAttachList(NewsViewModel newsVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get NewsAttach List").Add("JsonAttach", newsVM.JsonAttach).ToInputLogString());

            try
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get NewsAttach List").ToSuccessLogString());
                return PartialView("~/Views/News/_NewsAttachmentList.cshtml", newsVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get NewsAttach List").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [CheckUserRole(ScreenCode.ManageNews)]
        public JsonResult DeleteAttach(NewsViewModel newsVM, int listIndex)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Attach").Add("NewsId", newsVM.NewsId).Add("ListIndex", listIndex).ToInputLogString());

            try
            {
                var deletedAttach = newsVM.AttachmentList[listIndex];

                if (deletedAttach.NewsId == null)
                {
                    newsVM.AttachmentList.Remove(deletedAttach);
                }
                else
                {
                    deletedAttach.IsDelete = true;
                }

                return Json(new
                {
                    Valid = true,
                    Data = JsonConvert.SerializeObject(newsVM.AttachmentList)
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Attach").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = Resource.Error_System,
                    Errors = string.Empty
                });
            }
        }

        #region "Popup NewsPreView"

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ManageNews)]
        public ActionResult InitNewsPreview(NewsViewModel newsVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitNewsPreview").ToInputLogString());

            try
            {                
                AcceptNewsViewModel acceptNewsVM = new AcceptNewsViewModel();
                acceptNewsVM.Topic = newsVM.Topic;
                acceptNewsVM.AnnounceDate = newsVM.AnnounceDate;
                acceptNewsVM.ExpiryDate = newsVM.ExpiryDate;
                acceptNewsVM.Content = newsVM.Content;
                acceptNewsVM.FullName = newsVM.FullName;

                acceptNewsVM.AttachmentList = newsVM.AttachmentList.Where(x=> x.IsDelete == false).ToList();               

                return PartialView("~/Views/News/_NewsPreview.cshtml", acceptNewsVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitNewsPreview").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        #endregion
    }
}
