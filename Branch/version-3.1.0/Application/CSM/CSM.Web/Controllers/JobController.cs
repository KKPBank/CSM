using System;
using System.Linq;
using System.Web.Mvc;
using CSM.Business;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;
using CSM.Common.Resources;
using System.Globalization;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class JobController : BaseController
    {
        private ICommonFacade _commonFacade;
        private ICommPoolFacade _commPoolFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(JobController));

        [CheckUserRole(ScreenCode.SearchCommPool)]
        public ActionResult Search()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Job").ToInputLogString());
            try
            {
                _commonFacade = new CommonFacade();
                _commPoolFacade = new CommPoolFacade();

                ViewBag.jobstatus = _commPoolFacade.GetJobStatusSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.channel = _commPoolFacade.GetChannelWithEmailSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.srstatus = _commPoolFacade.GetSRStatusSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);

                JobViewModel jobVM = new JobViewModel();
                jobVM.SearchFilter = new CommPoolSearchFilter
                {
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    JobStatus = null,
                    Channel = null,
                    Subject = string.Empty,
                    From = string.Empty,
                    DateFrom = string.Empty,
                    DateTo = string.Empty,
                    ActionBy = string.Empty,
                    SRId = string.Empty,
                    SRStatus = null,
                    JobDateFrom = string.Empty,
                    JobDateTo = string.Empty,
                    CreatorSR = string.Empty,
                    OwnerSR = string.Empty,
                    User = null,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "JobId",
                    SortOrder = "DESC"
                };

                var defSearch = _commonFacade.GetShowhidePanelByUserId(this.UserInfo, Constants.Page.CommunicationPage);

                if (defSearch != null)
                {
                    jobVM.IsSelected = defSearch.IsSelectd ? "1" : "0";
                }
                else
                {
                    jobVM.IsSelected = "0";
                }

                ViewBag.PageSize = jobVM.SearchFilter.PageSize;
                ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.Message = string.Empty;

                return View(jobVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Job").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.SearchCommPool)]
        public ActionResult JobList(CommPoolSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Job")
                .Add("FirstName", searchFilter.FirstName).Add("LastName", searchFilter.LastName));

            try
            {
                _commonFacade = new CommonFacade();
                int? monthOfReportExport = _commonFacade.GetCacheParamByName(Constants.ParameterName.ReportExportDate).ParamValue.ToNullable<int>();

                #region "Validation"

                if (!string.IsNullOrWhiteSpace(searchFilter.FirstName)
                    && searchFilter.FirstName.ExtractString().Length < Constants.MinLenght.SearchTerm)
                {
                    ModelState["FirstName"].Errors.Clear();
                    ModelState["FirstName"].Errors.Add(string.Format(CultureInfo.InvariantCulture, Resource.ValErr_MinLength, Constants.MinLenght.SearchTerm));
                }

                if (!string.IsNullOrWhiteSpace(searchFilter.LastName) &&
                    searchFilter.LastName.ExtractString().Length < Constants.MinLenght.SearchTerm)
                {
                    ModelState["LastName"].Errors.Clear();
                    ModelState["LastName"].Errors.Add(string.Format(CultureInfo.InvariantCulture, Resource.ValErr_MinLength, Constants.MinLenght.SearchTerm));
                }

                bool isValid = TryUpdateModel(searchFilter);

                if (!string.IsNullOrEmpty(searchFilter.DateFrom) && !searchFilter.DateFromValue.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("txtFromDate", Resource.ValErr_InvalidDate);
                }
                else if(searchFilter.DateFromValue.HasValue)
                {
                    if (searchFilter.DateFromValue.Value > DateTime.Now.Date)
                    {
                        isValid = false;
                        ModelState.AddModelError("txtFromDate", Resource.ValErr_InvalidDate_MustLessThanToday);
                    }
                }

                if (!string.IsNullOrEmpty(searchFilter.DateTo) && !searchFilter.DateToValue.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("txtToDate", Resource.ValErr_InvalidDate);
                }
                else if (searchFilter.DateToValue.HasValue)
                {
                    if (searchFilter.DateToValue.Value > DateTime.Now.Date)
                    {
                        isValid = false;
                        ModelState.AddModelError("txtToDate", Resource.ValErr_InvalidDate_MustLessThanToday);
                    }
                }

                if (searchFilter.DateFromValue.HasValue && searchFilter.DateToValue.HasValue
                    && searchFilter.DateFromValue.Value > searchFilter.DateToValue.Value)
                {
                    isValid = false;
                    ModelState.AddModelError("dvDateRange", Resource.ValErr_InvalidDateRange);
                }

                // ActionDate
                if (!string.IsNullOrEmpty(searchFilter.DateFrom) && string.IsNullOrEmpty(searchFilter.DateTo))
                {
                    isValid = false;
                    ModelState.AddModelError("txtToDate", Resource.ValErr_Required);
                }
                if (string.IsNullOrEmpty(searchFilter.DateFrom) && !string.IsNullOrEmpty(searchFilter.DateTo))
                {
                    isValid = false;
                    ModelState.AddModelError("txtFromDate", Resource.ValErr_Required);
                }

                if (!string.IsNullOrEmpty(searchFilter.DateFrom) && !string.IsNullOrEmpty(searchFilter.DateTo))
                {
                    if (searchFilter.DateToValue.Value > searchFilter.DateFromValue.Value.AddMonths(monthOfReportExport.Value))
                    {
                        isValid = false;
                        ModelState.AddModelError("dvDateRange", string.Format(CultureInfo.InvariantCulture, Resource.ValError_ReportExportDuration, monthOfReportExport.Value));
                    }
                }

                //JobDate
                if (!string.IsNullOrEmpty(searchFilter.JobDateFrom) && !searchFilter.JobDateFromValue.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("txtJobFromDate", Resource.ValErr_InvalidDate);
                }
                else if (searchFilter.JobDateFromValue.HasValue)
                {
                    if (searchFilter.JobDateFromValue.Value > DateTime.Now.Date)
                    {
                        isValid = false;
                        ModelState.AddModelError("txtJobFromDate", Resource.ValErr_InvalidDate_MustLessThanToday);
                    }
                }

                if (!string.IsNullOrEmpty(searchFilter.JobDateTo) && !searchFilter.JobDateToValue.HasValue)
                {
                    isValid = false;
                    ModelState.AddModelError("txtJobToDate", Resource.ValErr_InvalidDate);
                }
                else if (searchFilter.JobDateToValue.HasValue)
                {
                    if (searchFilter.JobDateToValue.Value > DateTime.Now.Date)
                    {
                        isValid = false;
                        ModelState.AddModelError("txtJobToDate", Resource.ValErr_InvalidDate_MustLessThanToday);
                    }
                }

                if (searchFilter.JobDateFromValue.HasValue && searchFilter.JobDateToValue.HasValue
                    && searchFilter.JobDateFromValue.Value > searchFilter.JobDateToValue.Value)
                {
                    isValid = false;
                    ModelState.AddModelError("dvJobDateRange", Resource.ValErr_InvalidDateRange);
                }

                //--------------------------
                if (!string.IsNullOrEmpty(searchFilter.JobDateFrom) && string.IsNullOrEmpty(searchFilter.JobDateTo))
                {
                    isValid = false;
                    ModelState.AddModelError("txtJobToDate", Resource.ValErr_Required);
                }
                if (string.IsNullOrEmpty(searchFilter.JobDateFrom) && !string.IsNullOrEmpty(searchFilter.JobDateTo))
                {
                    isValid = false;
                    ModelState.AddModelError("txtJobFromDate", Resource.ValErr_Required);
                }
                if (!string.IsNullOrEmpty(searchFilter.JobDateFrom) && !string.IsNullOrEmpty(searchFilter.JobDateTo))
                {
                    if (searchFilter.JobDateToValue.Value > searchFilter.JobDateFromValue.Value.AddMonths(monthOfReportExport.Value))
                    {
                        isValid = false;
                        ModelState.AddModelError("dvJobDateRange", string.Format(CultureInfo.InvariantCulture, Resource.ValError_ReportExportDuration, monthOfReportExport.Value));
                    }
                }

                #endregion

                if (isValid)
                {
                    _commPoolFacade = new CommPoolFacade();
                    JobViewModel jobVM = new JobViewModel();
                    jobVM.SearchFilter = searchFilter;
                    jobVM.SearchFilter.User = this.UserInfo;
                    jobVM.CommunicationPoolList = _commPoolFacade.SearchJobs(searchFilter);
                    ViewBag.PageSize = jobVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    return PartialView("~/Views/Job/_JobList.cshtml", jobVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Job").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCommPool)]
        public ActionResult InitEdit(int? jobId = null)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitEdit Job").Add("JobId", jobId).ToInputLogString());

            try
            {
                JobViewModel jobVM = null;

                if (TempData["jobVM"] != null)
                {
                    jobVM = (JobViewModel)TempData["jobVM"];
                }
                else
                {
                    jobVM = new JobViewModel { JobId = jobId };
                }

                if (jobVM.JobId != null)
                {
                    _commPoolFacade = new CommPoolFacade();
                    CommunicationPoolEntity commPoolEntity = _commPoolFacade.GetJob(jobVM.JobId.Value);

                    jobVM.Sender = commPoolEntity.SenderAddress;
                    jobVM.Subject = commPoolEntity.Subject;
                    jobVM.StatusDisplay = commPoolEntity.StatusDisplay;
                    jobVM.SequenceNo = commPoolEntity.SequenceNo;
                    jobVM.Content = commPoolEntity.Content;
                    jobVM.CreatedDate = commPoolEntity.CreateDateDisplay;
                    jobVM.UpdatedDate = commPoolEntity.UpdateDateDisplay;
                    jobVM.Remark = commPoolEntity.Remark;
                    jobVM.JobStatus = commPoolEntity.Status;

                    if (commPoolEntity.UpdateUser != null)
                        jobVM.ActionBy = commPoolEntity.UpdateUser.FullName;

                    if (commPoolEntity.ChannelEntity != null)
                        jobVM.Channel = commPoolEntity.ChannelEntity.Name;

                    //SR
                    if (commPoolEntity.ServiceRequest != null)
                    {
                        jobVM.SrNo = commPoolEntity.ServiceRequest.SrNo;
                        jobVM.SR_Status = commPoolEntity.ServiceRequest.Status;
                        jobVM.SrSubject = commPoolEntity.ServiceRequest.Subject;
                        jobVM.SrRemark = commPoolEntity.ServiceRequest.Remark;
                        jobVM.SrChannel = commPoolEntity.ServiceRequest.ChannelName;
                        jobVM.SrMediaSource = commPoolEntity.ServiceRequest.MediaSourceName;
                        jobVM.SrCreateDate = commPoolEntity.ServiceRequest.CreateDateDisplay;

                        if (commPoolEntity.ServiceRequest.CreateUser != null)
                        {
                            jobVM.SrCreateUser = commPoolEntity.ServiceRequest.CreateUser.FullName;
                        }

                        if (commPoolEntity.ServiceRequest.Owner != null)
                        {
                            jobVM.SrOwner = commPoolEntity.ServiceRequest.Owner.FullName;
                        }

                        if (commPoolEntity.ServiceRequest.Delegator != null)
                        {
                            jobVM.SrDelegator = commPoolEntity.ServiceRequest.Delegator.FullName;
                        }

                        if (commPoolEntity.ServiceRequest.UpdateUser != null)
                        {
                            jobVM.SrUpdateUser = commPoolEntity.ServiceRequest.UpdateUser.FullName;
                        }

                        if (commPoolEntity.ServiceRequest.Customer != null)
                        {
                            //Customer
                            jobVM.FirstNameThai = commPoolEntity.ServiceRequest.Customer.FirstNameThai;
                            jobVM.LastNameThai = commPoolEntity.ServiceRequest.Customer.LastNameThai;
                            jobVM.FirstNameEnglish = commPoolEntity.ServiceRequest.Customer.FirstNameEnglish;
                            jobVM.LastNameEnglish = commPoolEntity.ServiceRequest.Customer.LastNameEnglish;
                        }

                        if (commPoolEntity.ServiceRequest.ProductMapping != null)
                        {
                            jobVM.ProductGroup = commPoolEntity.ServiceRequest.ProductMapping.ProductGroupName;
                            jobVM.Product = commPoolEntity.ServiceRequest.ProductMapping.ProductName;
                            jobVM.Type = commPoolEntity.ServiceRequest.ProductMapping.TypeName;
                            jobVM.SubArea = commPoolEntity.ServiceRequest.ProductMapping.SubAreaName;
                        }
                    }

                    // Attachments
                    jobVM.Attachments = commPoolEntity.Attachments;
                }

                return View("~/Views/Job/Edit.cshtml", jobVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitEdit Job").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.SearchCommPool)]
        public JsonResult NewSR(int jobId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Save NewSR").Add("JobId", jobId).ToInputLogString());
            try
            {
                int srId = 0;
                int userId = this.UserInfo.UserId;
                _commPoolFacade = new CommPoolFacade();
                _commPoolFacade.SaveNewSR(jobId, userId, ref srId);

                return Json(new
                {
                    Valid = true,
                    SrId = srId
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save NewSR").Add("Error Message", ex.Message).ToFailLogString());
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
        [CheckUserRole(ScreenCode.SearchCommPool)]
        public JsonResult ShowhidePanel(int expandValue)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("ShowhidePanel").Add("expand", expandValue).ToInputLogString());
            try
            {
                _commonFacade = new CommonFacade();
                int userId = this.UserInfo.UserId;
                _commonFacade.SaveShowhidePanel(expandValue, userId, Constants.Page.CommunicationPage);

                return Json(new
                {
                    Valid = true
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("ShowhidePanel").Add("Error Message", ex.Message).ToFailLogString());
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
        [CheckUserRole(ScreenCode.ViewCommPool)]
        public ActionResult InitCloseJob(int? jobId, int? jobStatus)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitCloseJob").Add("JobId", jobId).ToInputLogString());

            try
            {
                var jobVM = new JobViewModel { JobId = jobId, JobStatus = jobStatus };
                return PartialView("~/Views/Job/_RemarkCloseJob.cshtml", jobVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitCloseJob").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCommPool)]
        public ActionResult ReloadCommPoolEditWithMessage(int? jobId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Reload EditCommPool with message").Add("JobId", jobId).ToInputLogString());

            try
            {
                var jobVM = new JobViewModel { JobId = jobId };
                ViewBag.ErrorMessage = Resource.Error_UpdateFailedDuplicate;
                TempData["jobVM"] = jobVM;
                return InitEdit();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Reload EditCommPool with message").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCommPool)]
        public JsonResult SaveRemarkCloseJob(JobViewModel jobVM)
        {
            try
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save RemarkCloseJob").ToInputLogString());

                // Validate MaxLength
                if (!string.IsNullOrWhiteSpace(jobVM.Remark) && jobVM.Remark.Count() > Constants.MaxLength.RemarkCloseJob)
                {
                    ModelState.AddModelError("Remark", string.Format(CultureInfo.InvariantCulture, Resource.ValErr_StringLength, Resource.Lbl_Remark, Constants.MaxLength.RemarkCloseJob));
                }

                if (ModelState.IsValid)
                {
                    int userId = this.UserInfo.UserId;
                    _commPoolFacade = new CommPoolFacade();
                    bool success = _commPoolFacade.UpdateJob(jobVM.JobId, userId, jobVM.JobStatus, jobVM.Remark);

                    if (success)
                    {
                        return Json(new
                        {
                            Valid = true
                        });
                    }

                    return Json(new
                    {
                        Valid = false,
                        Error = Resource.Error_UpdateFailedDuplicate

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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Save RemarkCloseJob").Add("Error Message", ex.Message).ToInputLogString());
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
        [CheckUserRole(ScreenCode.ViewCommPool)]
        public ActionResult CloseJob(JobViewModel jobVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Close Job").Add("JobId", jobVM.JobId).ToInputLogString());

            try
            {
                int userId = this.UserInfo.UserId;
                _commPoolFacade = new CommPoolFacade();
                bool success = _commPoolFacade.UpdateJob(jobVM.JobId, userId, jobVM.JobStatus, string.Empty);

                if (success)
                {
                    return RedirectToAction("Search", "Job");
                }

                ViewBag.ErrorMessage = Resource.Error_UpdateFailedDuplicate;
                TempData["jobVM"] = jobVM;
                return InitEdit();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Close Job").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCommPool)]
        public ActionResult Edit(JobViewModel jobVM)
        {
            TempData["jobVM"] = jobVM;
            return InitEdit();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCommPool)]
        public JsonResult LoadFileAttachment(int attachmentId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Load FileAttachment").ToInputLogString());

            try
            {
                _commPoolFacade = new CommPoolFacade();
                AttachmentEntity selectedAttach = _commPoolFacade.GetAttachmentsById(attachmentId);
                TempData["FILE_DOWNLOAD"] = selectedAttach;

                _commonFacade = new CommonFacade();
                string documentFolder = _commonFacade.GetJobDocumentFolder();
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

                return Json(new
                {
                    Valid = true
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Load FileAttachment").Add("Error Message", ex.Message).ToFailLogString());
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
        [CheckUserRole(ScreenCode.ViewCommPool)]
        public ActionResult PreviewAttachment()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Preview FileAttachment").ToInputLogString());

            try
            {
                AttachmentEntity selectedAttach = (AttachmentEntity)TempData["FILE_DOWNLOAD"];
                TempData["FILE_DOWNLOAD"] = selectedAttach; // keep object

                _commonFacade = new CommonFacade();
                string documentFolder = _commonFacade.GetJobDocumentFolder();
                string pathFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", documentFolder, selectedAttach.Url);
                byte[] byteArray = System.IO.File.ReadAllBytes(pathFile);

                return File(byteArray, selectedAttach.ContentType, selectedAttach.Filename);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Preview FileAttachment").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
    }
}
