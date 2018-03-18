using System;
using System.Web.Mvc;
using CSM.Business;
using CSM.Common.Resources;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class HomeController : BaseController
    {
        private INewsFacade _newsFacade;
        private IUserFacade _userFacade; 
        private ICommonFacade _commonFacade;
        private ICustomerFacade _customerFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(HomeController));

        [CheckUserRole(ScreenCode.MainPage)]
        public ActionResult Index()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Index").Add("CallId", this.CallId)
                .Add("UserId", this.UserInfo.UserId).ToInputLogString());

            try
            {
                _newsFacade = new NewsFacade();
                _userFacade = new UserFacade(); 
                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();

                HomeViewModel homeVM = new HomeViewModel();
                homeVM.NewsUnreadSearchFilter = new NewsSearchFilter
                {
                    Topic = string.Empty,
                    Status = Constants.ApplicationStatus.Active.ToString(CultureInfo.InvariantCulture),
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "NewsId",
                    SortOrder = "DESC"
                };

                homeVM.NewsReadSearchFilter = new NewsSearchFilter
                {
                    Topic = string.Empty,
                    Status = Constants.ApplicationStatus.Active.ToString(CultureInfo.InvariantCulture),
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "NewsId",
                    SortOrder = "DESC"
                };

                homeVM.GroupSrSearchFilter = new SrSearchFilter
                {
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "CreateDate",
                    SortOrder = "ASC"
                };

                homeVM.IndividualSrSearchFilter = new SrSearchFilter
                {
                    FilterType = null,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "CreateDate",
                    SortOrder = "ASC"
                };                

                #region "News Unread"

                //homeVM.NewsUnreadSearchFilter.UserId = this.UserInfo.UserId;
                //homeVM.NewsUnreadList = _newsFacade.GetNewsUnreadList(homeVM.NewsUnreadSearchFilter);

                #endregion

                #region "News Read"

                //homeVM.NewsReadSearchFilter.UserId = this.UserInfo.UserId;
                //homeVM.NewsReadList = _newsFacade.GetNewsReadList(homeVM.NewsReadSearchFilter);

                #endregion

                #region "Group ServiceRequest"

                //var lstDummyUser = _userFacade.GetDummyUsers(this.UserInfo); 
                //homeVM.GroupSrSearchFilter.OwnerList = lstDummyUser;
                //homeVM.GroupServiceRequestList = _customerFacade.GetSrList(homeVM.GroupSrSearchFilter);

                #endregion

                #region "Individual ServiceRequest"

                //var lstEmployeeUser = new List<UserEntity>();                
                //lstEmployeeUser = _userFacade.GetEmployees(this.UserInfo);
                //lstEmployeeUser.Add(this.UserInfo); // Add current user                
                //homeVM.IndividualSrSearchFilter.OwnerList = lstEmployeeUser;
                //homeVM.IndividualServiceRequestList = _customerFacade.GetSrList(homeVM.IndividualSrSearchFilter);

                #endregion

                ViewBag.NewsUnreadPageSize = homeVM.NewsUnreadSearchFilter.PageSize;
                ViewBag.NewsReadPageSize = homeVM.NewsReadSearchFilter.PageSize;
                ViewBag.GroupPageSize = homeVM.GroupSrSearchFilter.PageSize;
                ViewBag.IndyPageSize = homeVM.IndividualSrSearchFilter.PageSize;

                var pageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.NewsUnreadPageSizeList = pageSizeList;
                ViewBag.NewsReadPageSizeList = pageSizeList;
                ViewBag.GroupPageSizeList = pageSizeList;
                ViewBag.IndyPageSizeList = pageSizeList;

                ViewBag.Message = string.Empty;

                return View(homeVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Index").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.MainPage)]
        public ActionResult NewsUnreadList(NewsSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get NewsUnreadList").Add("UserId", this.UserInfo.UserId)
                .ToInputLogString());

            try
            {
                _newsFacade = new NewsFacade();
                _commonFacade = new CommonFacade();

                HomeViewModel homeVM = new HomeViewModel();
                homeVM.NewsUnreadSearchFilter = searchFilter;
                homeVM.NewsUnreadSearchFilter.UserId = this.UserInfo.UserId; 

                homeVM.NewsUnreadList = _newsFacade.GetNewsUnreadList(homeVM.NewsUnreadSearchFilter);
                ViewBag.NewsUnreadPageSize = homeVM.NewsUnreadSearchFilter.PageSize;
                ViewBag.NewsUnreadPageSizeList = _commonFacade.GetPageSizeList();

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get NewsUnreadList").ToSuccessLogString());
                return PartialView("~/Views/Home/_NewsUnreadList.cshtml", homeVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get NewsUnreadList").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.MainPage)]
        public ActionResult NewsReadList(NewsSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get NewsReadList").Add("UserId", this.UserInfo.UserId)
                .ToInputLogString());

            try
            {
                _newsFacade = new NewsFacade();
                _commonFacade = new CommonFacade();
                HomeViewModel homeVM = new HomeViewModel();
                homeVM.NewsReadSearchFilter = searchFilter;
                homeVM.NewsReadSearchFilter.UserId = this.UserInfo.UserId;

                homeVM.NewsReadList = _newsFacade.GetNewsReadList(homeVM.NewsReadSearchFilter);
                ViewBag.NewsReadPageSize = homeVM.NewsReadSearchFilter.PageSize;
                ViewBag.NewsReadPageSizeList = _commonFacade.GetPageSizeList();

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get NewsReadList").ToSuccessLogString());
                return PartialView("~/Views/Home/_NewsReadList.cshtml", homeVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get NewsReadList").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        #region "Popup AcceptNews"

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.MainPage)]
        public ActionResult InitAcceptNews(int? newsId, string isShowAcceptNews)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Init AcceptNews").ToInputLogString());

            try
            {
                _newsFacade = new NewsFacade();
                AcceptNewsViewModel acceptNewsVM = new AcceptNewsViewModel();

                if (newsId.HasValue)
                {
                    NewsEntity newsEntity = _newsFacade.GetNewsByID(newsId.Value);
                    acceptNewsVM.Topic = newsEntity.Topic;
                    acceptNewsVM.AnnounceDate = newsEntity.AnnounceDateDisplay;
                    acceptNewsVM.ExpiryDate = newsEntity.ExpiryDateDisplay;
                    acceptNewsVM.Content = newsEntity.Content;
                    acceptNewsVM.FullName = newsEntity.CreateUser.FullName;

                    acceptNewsVM.AttachmentList = _newsFacade.GetNewsAttachmentList(newsId.Value);
                    TempData["NewsAttachmentList"] = acceptNewsVM.AttachmentList; // keep for download
                    
                    // สำหรับ show/hide CheckBox
                    acceptNewsVM.IsShowAcceptNews = (isShowAcceptNews == "1") ? true : false;
                }

                return PartialView("~/Views/Home/_AcceptNews.cshtml", acceptNewsVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Init AcceptNews").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.MainPage)]
        public JsonResult SaveAcceptNews(AcceptNewsViewModel acceptNewsVM)
        {
            try
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save AcceptNews").ToInputLogString());

                if (ModelState.IsValid)
                {
                    // Save
                    ReadNewsEntity readNewsEntity = new ReadNewsEntity
                    {
                        NewsId = acceptNewsVM.NewsId,                        
                        CreateUserId = this.UserInfo.UserId,
                    };

                    _newsFacade = new NewsFacade();
                    _newsFacade.SaveReadNews(readNewsEntity);

                    return Json(new
                    {
                        Valid = true
                    });
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save AcceptNews").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = Resource.Error_System,
                    Errors = string.Empty
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LoadFileNewsAttachment(int attachmentId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Load FileNewsAttachment").ToInputLogString());

            try
            {
                if (TempData["NewsAttachmentList"] != null)
                {
                    var lstNewsAttachment = (List<AttachmentEntity>)TempData["NewsAttachmentList"];
                    TempData["NewsAttachmentList"] = lstNewsAttachment; // keep for download
                    if (lstNewsAttachment != null)
                    {
                        AttachmentEntity selectedAttach = lstNewsAttachment.FirstOrDefault(x => x.AttachmentId == attachmentId);
                        TempData["FILE_DOWNLOAD"] = selectedAttach;

                        _commonFacade = new CommonFacade();
                        string documentFolder = _commonFacade.GetNewsDocumentFolder();
                        string pathFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", documentFolder, selectedAttach.Url);

                        if (!System.IO.File.Exists(pathFile))
                        {
                            return Json(new
                            {
                                Valid = false,
                                Error = "ไม่พบไฟล์ที่ต้องการ Download",
                                Errors = string.Empty
                            });
                        }
                    }
                }

                return Json(new
                {
                    Valid = true
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Load FileNewsAttachment").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = Resource.Error_System,
                    Errors = string.Empty
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PreviewNewsAttachment()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Preview NewsAttachment").ToInputLogString());

            try
            {
                AttachmentEntity selectedAttach = (AttachmentEntity)TempData["FILE_DOWNLOAD"];                

                _commonFacade = new CommonFacade();
                string documentFolder = _commonFacade.GetNewsDocumentFolder();

                string pathFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", documentFolder, selectedAttach.Url);
                byte[] byteArray = System.IO.File.ReadAllBytes(pathFile);
                return File(byteArray, selectedAttach.ContentType, selectedAttach.Filename);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Preview NewsAttachment").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        #endregion

        #region "SR"

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.MainPage)]
        public ActionResult GroupServiceRequestList(SrSearchFilter searchFilter)
        {
            try
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get GroupServiceRequestList").Add("UserId", this.UserInfo.UserId).ToInputLogString());

                _userFacade = new UserFacade(); 
                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();

                HomeViewModel homeVM = new HomeViewModel();
                homeVM.GroupSrSearchFilter = searchFilter;

                var lstDummyUser = _userFacade.GetDummyUsers(this.UserInfo);
                homeVM.GroupSrSearchFilter.OwnerList = lstDummyUser;

                if (searchFilter.CurrentUserId != UserInfo.UserId)
                {
                    // First Load OR Change User
                    searchFilter.CurrentUserId = UserInfo.UserId;
                    searchFilter.CanViewAllUsers = null;
                    searchFilter.CanViewUserIds = string.Empty;
                    searchFilter.CanViewSrPageIds = string.Empty;
                }

                if (searchFilter.CurrentUserRoleCode != UserInfo.RoleCode)
                {
                    // First Load OR Change Role
                    searchFilter.CurrentUserRoleCode = UserInfo.RoleCode;
                }

                homeVM.GroupServiceRequestList = _customerFacade.GetSrList(homeVM.GroupSrSearchFilter);
                ViewBag.GroupPageSize = homeVM.GroupSrSearchFilter.PageSize;
                ViewBag.GroupPageSizeList = _commonFacade.GetPageSizeList();

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get GroupServiceRequestList").ToSuccessLogString());
                return PartialView("~/Views/Home/_GroupServiceRequestList.cshtml", homeVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get GroupServiceRequestList").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.MainPage)]
        public ActionResult IndividualServiceRequestList(SrSearchFilter searchFilter)
        {
            try
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get IndividualServiceRequestList").Add("UserId", this.UserInfo.UserId)
                    .ToInputLogString());

                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();

                var lstEmployeeUser = new List<UserEntity>();

                // กรณีเลือก ทั้งหมด  FilterType จะมีค่าเป็น null
                if(searchFilter.FilterType == null)
                {
                    _userFacade = new UserFacade();
                    lstEmployeeUser = _userFacade.GetEmployees(this.UserInfo);
                }
                
                lstEmployeeUser.Add(this.UserInfo); // add current user

                if (searchFilter.CurrentUserId != UserInfo.UserId)
                {
                    // First Load OR Change User
                    searchFilter.CurrentUserId = UserInfo.UserId;
                    searchFilter.CanViewAllUsers = null;
                    searchFilter.CanViewUserIds = string.Empty;
                    searchFilter.CanViewSrPageIds = string.Empty;
                }

                if (searchFilter.CurrentUserRoleCode != UserInfo.RoleCode)
                {
                    // First Load OR Change Role
                    searchFilter.CurrentUserRoleCode = UserInfo.RoleCode;
                }

                HomeViewModel homeVM = new HomeViewModel();
                homeVM.IndividualSrSearchFilter = searchFilter;
                homeVM.IndividualSrSearchFilter.OwnerList = lstEmployeeUser;

                homeVM.IndividualServiceRequestList = _customerFacade.GetSrList(homeVM.IndividualSrSearchFilter);
                ViewBag.IndyPageSize = homeVM.IndividualSrSearchFilter.PageSize;
                ViewBag.IndyPageSizeList = _commonFacade.GetPageSizeList();

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get IndividualServiceRequestList").ToSuccessLogString());
                return PartialView("~/Views/Home/_IndividualServiceRequestList.cshtml", homeVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get IndividualServiceRequestList").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        #endregion
    }
}
