using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.Hosting;
using System.Web.Mvc;
using CSM.Business;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;
using CSM.Entity.Common;
using CSM.Common.Resources;
using System.Globalization;
using System.ComponentModel;
using System.Linq;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class ReportController : BaseController
    {
        private IReportFacade _reportFacade;
        private ICommonFacade _commonFacade;
        private ICommPoolFacade _commPoolFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReportController));

        public ActionResult List()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List Report").ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();
                ViewBag.ReportList = _commonFacade.GetReportList(this.UserInfo.RoleValue);
                return View();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Report").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [CheckUserRole(ScreenCode.ReportCommPool)]
        public ActionResult ExportJobs()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Init ExportJobs").ToInputLogString());
            try
            {
                _reportFacade = new ReportFacade();
                _commonFacade = new CommonFacade();
                _commPoolFacade = new CommPoolFacade();

                ExportJobsViewModel jobVM = new ExportJobsViewModel();
                jobVM.SearchFilter = new ExportJobsSearchFilter
                {
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = string.Empty,
                    SortOrder = "DESC"
                };

                var jobstatusList = _commPoolFacade.GetJobStatusSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.jobstatus = new SelectList((IEnumerable)jobstatusList, "Key", "Value", string.Empty);

                var attachfileList = _reportFacade.GetAttachfileSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.Attachfile = new SelectList((IEnumerable)attachfileList, "Key", "Value", string.Empty);

                return View("~/Views/Report/ExportJobs.cshtml", jobVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Init ExportJobs").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ReportCommPool)]
        public JsonResult LoadJobsExcel(ExportJobsSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Export Jobs").Add("From", searchFilter.FromValue).Add("Subject", searchFilter.Subject)
                .Add("FirstName", searchFilter.FirstName).Add("LastName", searchFilter.LastName).ToInputLogString());

            try
            {
                //Force report to export only job belong to the login user
                searchFilter.LoginUser = this.UserInfo;

                #region "Check Time to Export"

                string errorMessage;
                bool isValid = CheckTimeToExport(out errorMessage);

                if (!isValid)
                {
                    return Json(new
                    {
                        Valid = false,
                        Error = errorMessage
                    });
                }

                #endregion

                #region "Validation Report Criteria"
                
                isValid = ValidateCommPoolReport(searchFilter);

                #endregion

                if (isValid)
                {
                    if (_reportFacade == null) { _reportFacade = new ReportFacade(); }
                    var jobList = _reportFacade.GetExportJobs(searchFilter);

                    if (jobList != null && jobList.Count > 0)
                    {
                        //var bytes = ExcelHelpers.WriteToExcelJobs(HostingEnvironment.MapPath("~/Templates/Reports/rpt_comm_pool.xlsx"), DateTime.Now, ds);
                        TempData["FILE_EXPORT_JOBS"] = _reportFacade.CreateReportCommPool(jobList, searchFilter);

                        return Json(new
                        {
                            Valid = true,
                            Message = string.Empty,
                            Error = string.Empty,
                        });
                    }

                    return Json(new
                    {
                        Valid = false,
                        Error = Resource.Msg_NoRecords,
                    });
                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Export Jobs").ToFailLogString());
                    return Json(new
                    {
                        Valid = false,
                        Error = string.Empty,
                        Errors = GetModelValidationErrors()
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Export Jobs").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Message = string.Empty,
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ReportCommPool)]
        public ActionResult PrintJobsExcel()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Print Jobs").ToInputLogString());

            try
            {
                var bytes = TempData["FILE_EXPORT_JOBS"] as Byte[];
                string dateStr = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.ExportDateTime);
                string fileDownloadName = string.Format(CultureInfo.InvariantCulture, "{0}_{1}.{2}", Resource.Report_CommunicationPool_FileName, dateStr, Resource.Report_FileExtention);
                return File(bytes, "application/octet-stream", fileDownloadName);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Print Jobs").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [CheckUserRole(ScreenCode.ReportSR)]
        public ActionResult ExportSR()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Export SR").ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();
                _reportFacade = new ReportFacade();
                _commPoolFacade = new CommPoolFacade();

                var slaList = _reportFacade.GetSlaSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.sla = new SelectList((IEnumerable)slaList, "Key", "Value", string.Empty);

                var statusList = _commPoolFacade.GetSRStatusSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.srstatus = new SelectList((IEnumerable)statusList, "Key", "Value", string.Empty);

                var channelList = _commPoolFacade.GetChannelSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.channel = new SelectList((IEnumerable)channelList, "Key", "Value", string.Empty);

                ExportSRViewModel srVM = new ExportSRViewModel();
                srVM.SearchFilter = new ExportSRSearchFilter
                {
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = string.Empty,
                    SortOrder = "DESC"
                };

                return View("~/Views/Report/ExportSR.cshtml", srVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Export SR").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ReportSR)]
        public JsonResult LoadSRExcel(ExportSRSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Export SR").Add("FirstName", searchFilter.FirstName)
                .Add("LastName", searchFilter.LastName).ToInputLogString());

            try
            {
                #region "Check Time to Export"

                string errorMessage;
                bool isValid = CheckTimeToExport(out errorMessage);

                if (!isValid)
                {
                    return Json(new
                    {
                        Valid = false,
                        Error = errorMessage
                    });
                }

                #endregion

                #region "SR Time"

                if (!string.IsNullOrEmpty(searchFilter.SRTimeFrom) || !string.IsNullOrEmpty(searchFilter.SRTimeTo))
                {
                    if (string.IsNullOrEmpty(searchFilter.SRDateFrom))
                    {
                        ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_Required);
                    }
                    if (string.IsNullOrEmpty(searchFilter.SRDateTo))
                    {
                        ModelState.AddModelError("txtSRDateTo", Resource.ValErr_Required);
                    }
                    if (string.IsNullOrEmpty(searchFilter.SRTimeFrom))
                    {
                        ModelState.AddModelError("txtSRTimeFrom", Resource.ValErr_Required);
                    }
                    if (string.IsNullOrEmpty(searchFilter.SRTimeTo))
                    {
                        ModelState.AddModelError("txtSRTimeTo", Resource.ValErr_Required);
                    }
                }

                if (!string.IsNullOrEmpty(searchFilter.SRTimeFrom) && !string.IsNullOrEmpty(searchFilter.SRTimeTo))
                {
                    if (searchFilter.SRDateFromValue.HasValue && searchFilter.SRDateToValue.HasValue
                        && searchFilter.SRDateFromValue.Value == searchFilter.SRDateToValue.Value)
                    {
                        if ((searchFilter.SRTimeFrom.Replace(":", "")).ToNullable<int>() > (searchFilter.SRTimeTo.Replace(":", "")).ToNullable<int>())
                        {
                            ModelState.AddModelError("dvTimeRange", Resource.ValErr_InvalidTimeRange);
                        }
                    }
                }

                #endregion

                #region "Validation Report Criteria"

                _commonFacade = new CommonFacade();
                int? monthOfReportExport = _commonFacade.GetCacheParamByName(Constants.ParameterName.ReportExportDate).ParamValue.ToNullable<int>();

                if (!string.IsNullOrEmpty(searchFilter.SRDateFrom) && !searchFilter.SRDateFromValue.HasValue)
                {
                    ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_InvalidDate);
                }
                else if (searchFilter.SRDateFromValue.HasValue && searchFilter.SRDateFromValue.Value > DateTime.Now.Date)
                {
                    ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_InvalidDate_MustLessThanToday);
                }

                if (!string.IsNullOrEmpty(searchFilter.SRDateTo) && !searchFilter.SRDateToValue.HasValue)
                {
                    ModelState.AddModelError("txtSRDateTo", Resource.ValErr_InvalidDate);
                }
                else if (searchFilter.SRDateToValue.HasValue && searchFilter.SRDateToValue.Value > DateTime.Now.Date)
                {
                    ModelState.AddModelError("txtSRDateTo", Resource.ValErr_InvalidDate_MustLessThanToday);
                }

                if (searchFilter.SRDateFromValue.HasValue && searchFilter.SRDateToValue.HasValue
                    && searchFilter.SRDateFromValue.Value > searchFilter.SRDateToValue.Value)
                {
                    ModelState.AddModelError("dvDateRange", Resource.ValErr_InvalidDateRange);
                }

                // SRDate                
                if (!string.IsNullOrEmpty(searchFilter.SRDateFrom) && string.IsNullOrEmpty(searchFilter.SRDateTo))
                {
                    ModelState.AddModelError("txtSRDateTo", Resource.ValErr_Required);
                }

                if (string.IsNullOrEmpty(searchFilter.SRDateFrom) && !string.IsNullOrEmpty(searchFilter.SRDateTo))
                {
                    ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_Required);
                }

                if (!string.IsNullOrEmpty(searchFilter.SRDateFrom) && !string.IsNullOrEmpty(searchFilter.SRDateTo))
                {
                    if (searchFilter.SRDateToValue.Value > searchFilter.SRDateFromValue.Value.AddMonths(monthOfReportExport.Value))
                    {
                        ModelState.AddModelError("dvDateRange", string.Format(CultureInfo.InvariantCulture, Resource.ValError_ReportExportDuration, monthOfReportExport.Value));
                    }
                }

                #endregion

                if (ModelState.IsValid)
                {
                    if (_reportFacade == null) { _reportFacade = new ReportFacade(); }
                    var srList = _reportFacade.GetExportSR(searchFilter);

                    if (srList != null && srList.Count > 0)
                    {
                        //var bytes = ExcelHelpers.WriteToExcelSR(HostingEnvironment.MapPath("~/Templates/Reports/rpt_sr.xlsx"), DateTime.Now, ds);
                        TempData["FILE_EXPORT_SR"] = _reportFacade.CreateReportSR(srList, searchFilter);

                        return Json(new
                        {
                            Valid = true,
                            Message = string.Empty,
                            Error = string.Empty,
                        });
                    }

                    return Json(new
                    {
                        Valid = false,
                        Error = Resource.Msg_NoRecords,
                    });

                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Export SR").ToFailLogString());
                    return Json(new
                    {
                        Valid = false,
                        Error = string.Empty,
                        Errors = GetModelValidationErrors()
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Export SR").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Message = string.Empty,
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ReportSR)]
        public ActionResult PrintSRExcel()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Print SR").ToInputLogString());

            try
            {
                var bytes = TempData["FILE_EXPORT_SR"] as Byte[];

                string dateStr = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.ExportDateTime);
                string fileDownloadName = string.Format(CultureInfo.InvariantCulture, "{0}_{1}.{2}", Resource.Report_SR_FileName, dateStr, Resource.Report_FileExtention);

                return File(bytes, "application/octet-stream", fileDownloadName);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Print SR").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [CheckUserRole(ScreenCode.ReportComplaint)]
        public ActionResult ExportComplaint()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Export Complaint").ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();
                _reportFacade = new ReportFacade();
                _commPoolFacade = new CommPoolFacade();

                var slaList = _reportFacade.GetSlaSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.sla = new SelectList((IEnumerable)slaList, "Key", "Value", string.Empty);

                var statusList = _commPoolFacade.GetSRStatusSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.srstatus = new SelectList((IEnumerable)statusList, "Key", "Value", string.Empty);

                var channelList = _commPoolFacade.GetChannelSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.channel = new SelectList((IEnumerable)channelList, "Key", "Value", string.Empty);

                ViewBag.ReportCode = "CPN";

                ExportSRViewModel srVM = new ExportSRViewModel();
                srVM.SearchFilter = new ExportSRSearchFilter
                {
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = string.Empty,
                    SortOrder = "DESC",
                    ReportType = "Complaint"
                };

                return View("~/Views/Report/ExportSR.cshtml", srVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Export SR").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ReportSR)]
        public JsonResult LoadComplaintReport(ExportSRSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Export SR").Add("FirstName", searchFilter.FirstName)
                .Add("LastName", searchFilter.LastName).ToInputLogString());

            try
            {
                #region "Check Time to Export"

                string errorMessage;
                bool isValid = CheckTimeToExport(out errorMessage);

                if (!isValid)
                {
                    return Json(new
                    {
                        Valid = false,
                        Error = errorMessage
                    });
                }

                #endregion

                #region "SR Time"

                if (!string.IsNullOrEmpty(searchFilter.SRTimeFrom) || !string.IsNullOrEmpty(searchFilter.SRTimeTo))
                {
                    if (string.IsNullOrEmpty(searchFilter.SRDateFrom))
                    {
                        ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_Required);
                    }
                    if (string.IsNullOrEmpty(searchFilter.SRDateTo))
                    {
                        ModelState.AddModelError("txtSRDateTo", Resource.ValErr_Required);
                    }
                    if (string.IsNullOrEmpty(searchFilter.SRTimeFrom))
                    {
                        ModelState.AddModelError("txtSRTimeFrom", Resource.ValErr_Required);
                    }
                    if (string.IsNullOrEmpty(searchFilter.SRTimeTo))
                    {
                        ModelState.AddModelError("txtSRTimeTo", Resource.ValErr_Required);
                    }
                }

                if (!string.IsNullOrEmpty(searchFilter.SRTimeFrom) && !string.IsNullOrEmpty(searchFilter.SRTimeTo))
                {
                    if (searchFilter.SRDateFromValue.HasValue && searchFilter.SRDateToValue.HasValue
                        && searchFilter.SRDateFromValue.Value == searchFilter.SRDateToValue.Value)
                    {
                        if ((searchFilter.SRTimeFrom.Replace(":", "")).ToNullable<int>() > (searchFilter.SRTimeTo.Replace(":", "")).ToNullable<int>())
                        {
                            ModelState.AddModelError("dvTimeRange", Resource.ValErr_InvalidTimeRange);
                        }
                    }
                }

                #endregion

                #region "Validation Report Criteria"

                _commonFacade = new CommonFacade();
                int? monthOfReportExport = _commonFacade.GetCacheParamByName(Constants.ParameterName.ReportExportDate).ParamValue.ToNullable<int>();

                if (!string.IsNullOrEmpty(searchFilter.SRDateFrom) && !searchFilter.SRDateFromValue.HasValue)
                {
                    ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_InvalidDate);
                }
                else if (searchFilter.SRDateFromValue.HasValue && searchFilter.SRDateFromValue.Value > DateTime.Now.Date)
                {
                    ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_InvalidDate_MustLessThanToday);
                }

                if (!string.IsNullOrEmpty(searchFilter.SRDateTo) && !searchFilter.SRDateToValue.HasValue)
                {
                    ModelState.AddModelError("txtSRDateTo", Resource.ValErr_InvalidDate);
                }
                else if (searchFilter.SRDateToValue.HasValue && searchFilter.SRDateToValue.Value > DateTime.Now.Date)
                {
                    ModelState.AddModelError("txtSRDateTo", Resource.ValErr_InvalidDate_MustLessThanToday);
                }

                if (searchFilter.SRDateFromValue.HasValue && searchFilter.SRDateToValue.HasValue
                    && searchFilter.SRDateFromValue.Value > searchFilter.SRDateToValue.Value)
                {
                    ModelState.AddModelError("dvDateRange", Resource.ValErr_InvalidDateRange);
                }

                // SRDate                
                if (!string.IsNullOrEmpty(searchFilter.SRDateFrom) && string.IsNullOrEmpty(searchFilter.SRDateTo))
                {
                    ModelState.AddModelError("txtSRDateTo", Resource.ValErr_Required);
                }

                if (string.IsNullOrEmpty(searchFilter.SRDateFrom) && !string.IsNullOrEmpty(searchFilter.SRDateTo))
                {
                    ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_Required);
                }

                if (!string.IsNullOrEmpty(searchFilter.SRDateFrom) && !string.IsNullOrEmpty(searchFilter.SRDateTo))
                {
                    if (searchFilter.SRDateToValue.Value > searchFilter.SRDateFromValue.Value.AddMonths(monthOfReportExport.Value))
                    {
                        ModelState.AddModelError("dvDateRange", string.Format(CultureInfo.InvariantCulture, Resource.ValError_ReportExportDuration, monthOfReportExport.Value));
                    }
                }

                #endregion

                if (ModelState.IsValid)
                {
                    if (_reportFacade == null) { _reportFacade = new ReportFacade(); }
                    using (var ds = new DataSet("Item"))
                    {
                        ds.Locale = CultureInfo.CurrentCulture;
                        DataTable dt = _reportFacade.GetExportComplaint(searchFilter);
                        ds.Tables.Add(dt);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            string csv = $"{searchFilter.SearchFilterDisplay}\n\n{dt.ToCSV(getExportNumAsStringCols(typeof(ExportSRComplaintEntity)))}";
                            TempData["EXPORT_COMPLAINT_CSV"] = System.Text.Encoding.GetEncoding(874).GetBytes(csv);

                            return Json(new
                            {
                                Valid = true,
                                Message = string.Empty,
                                Error = string.Empty,
                            });
                        }

                        return Json(new
                        {
                            Valid = false,
                            Error = Resource.Msg_NoRecords,
                        });
                    }
                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Export SR").ToFailLogString());
                    return Json(new
                    {
                        Valid = false,
                        Error = string.Empty,
                        Errors = GetModelValidationErrors()
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Export SR").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Message = string.Empty,
                    Error = ex.Message
                });
            }
        }

        private string[] getExportNumAsStringCols(Type type)
        {
            var props = TypeDescriptor.GetProperties(type).Cast<PropertyDescriptor>();

            var list = props.Where(x => x.Attributes.OfType<ExportAttribute>().FirstOrDefault() != null
                && x.Attributes.OfType<ExportAttribute>().FirstOrDefault().IsNumberAsString)
                .Select(x => x.Attributes.OfType<ExportAttribute>().FirstOrDefault().DisplayName);
            if (list != null && list.Count() > 0)
            {
                return list.ToArray();
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ReportSR)]
        public ActionResult PrintComplaintCSV()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Print SR").ToInputLogString());

            try
            {
                var bytes = TempData["EXPORT_COMPLAINT_CSV"] as byte[];

                string dateStr = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.ExportDateTime);
                string fileDownloadName = string.Format(CultureInfo.InvariantCulture, "{0}_{1}.{2}", Resource.Report_Complaint_FileName, dateStr, Resource.Report_FileExtention);

                return File(bytes, "bytes", fileDownloadName);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Print SR").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [CheckUserRole(ScreenCode.ReportVerifyDetail)]
        public ActionResult ExportVerifyDetail()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Export VerifyDetail").ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();
                _reportFacade = new ReportFacade();

                var verifyList = _reportFacade.GetVerifyResultSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.verify = new SelectList((IEnumerable)verifyList, "Key", "Value", string.Empty);

                ExportVerifyDetailViewModel verifyDetailVM = new ExportVerifyDetailViewModel();
                verifyDetailVM.SearchFilter = new ExportVerifyDetailSearchFilter
                {
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = string.Empty,
                    SortOrder = "DESC"
                };

                return View("~/Views/Report/ExportVerifyDetail.cshtml", verifyDetailVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Export VerifyDetail").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ReportVerifyDetail)]
        public JsonResult LoadVerifyDetailExcel(ExportVerifyDetailSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Export VerifyDetail").Add("ProductGroup", searchFilter.ProductGroup).Add("Product", searchFilter.Product)
                 .Add("Campaign", searchFilter.Campaign).Add("Type", searchFilter.Type).Add("Area", searchFilter.Area)
                 .Add("SubArea", searchFilter.SubArea).Add("OwnerSR", searchFilter.OwnerSR).Add("OwnerBranch", searchFilter.OwnerBranch)
                 .Add("SRId", searchFilter.SRId).Add("SRIsverify", searchFilter.SRIsverify).ToInputLogString());

            try
            {
                #region "Check Time to Export"

                string errorMessage;
                bool isValid = CheckTimeToExport(out errorMessage);

                if (!isValid)
                {
                    return Json(new
                    {
                        Valid = false,
                        Error = errorMessage
                    });
                }

                #endregion

                #region "SR Time"

                if (!string.IsNullOrEmpty(searchFilter.SRTimeFrom) || !string.IsNullOrEmpty(searchFilter.SRTimeTo))
                {
                    if (string.IsNullOrEmpty(searchFilter.SRDateFrom))
                    {
                        ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_Required);
                    }
                    if (string.IsNullOrEmpty(searchFilter.SRDateTo))
                    {
                        ModelState.AddModelError("txtSRDateTo", Resource.ValErr_Required);
                    }
                    if (string.IsNullOrEmpty(searchFilter.SRTimeFrom))
                    {
                        ModelState.AddModelError("txtSRTimeFrom", Resource.ValErr_Required);
                    }
                    if (string.IsNullOrEmpty(searchFilter.SRTimeTo))
                    {
                        ModelState.AddModelError("txtSRTimeTo", Resource.ValErr_Required);
                    }
                }

                if (!string.IsNullOrEmpty(searchFilter.SRTimeFrom) && !string.IsNullOrEmpty(searchFilter.SRTimeTo))
                {
                    if (searchFilter.SRDateFromValue.HasValue && searchFilter.SRDateToValue.HasValue
                        && searchFilter.SRDateFromValue.Value == searchFilter.SRDateToValue.Value)
                    {
                        if ((searchFilter.SRTimeFrom.Replace(":", "")).ToNullable<int>() > (searchFilter.SRTimeTo.Replace(":", "")).ToNullable<int>())
                        {
                            ModelState.AddModelError("dvTimeRange", Resource.ValErr_InvalidTimeRange);
                        }
                    }
                }

                #endregion

                #region "Validation Report Criteria"

                _commonFacade = new CommonFacade();
                int? monthOfReportExport = _commonFacade.GetCacheParamByName(Constants.ParameterName.ReportExportDate).ParamValue.ToNullable<int>();

                if (!string.IsNullOrEmpty(searchFilter.SRDateFrom) && !searchFilter.SRDateFromValue.HasValue)
                {
                    ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_InvalidDate);
                }
                else if (searchFilter.SRDateFromValue.HasValue && searchFilter.SRDateFromValue.Value > DateTime.Now.Date)
                {
                    ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_InvalidDate_MustLessThanToday);
                }

                if (!string.IsNullOrEmpty(searchFilter.SRDateTo) && !searchFilter.SRDateToValue.HasValue)
                {
                    ModelState.AddModelError("txtSRDateTo", Resource.ValErr_InvalidDate);
                }
                else if (searchFilter.SRDateToValue.HasValue && searchFilter.SRDateToValue.Value > DateTime.Now.Date)
                {
                    ModelState.AddModelError("txtSRDateTo", Resource.ValErr_InvalidDate_MustLessThanToday);
                }

                if (searchFilter.SRDateFromValue.HasValue && searchFilter.SRDateToValue.HasValue
                    && searchFilter.SRDateFromValue.Value > searchFilter.SRDateToValue.Value)
                {
                    ModelState.AddModelError("dvDateRange", Resource.ValErr_InvalidDateRange);
                }

                // SRDate                
                if (!string.IsNullOrEmpty(searchFilter.SRDateFrom) && string.IsNullOrEmpty(searchFilter.SRDateTo))
                {
                    ModelState.AddModelError("txtSRDateTo", Resource.ValErr_Required);
                }

                if (string.IsNullOrEmpty(searchFilter.SRDateFrom) && !string.IsNullOrEmpty(searchFilter.SRDateTo))
                {
                    ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_Required);
                }

                if (!string.IsNullOrEmpty(searchFilter.SRDateFrom) && !string.IsNullOrEmpty(searchFilter.SRDateTo))
                {
                    if (searchFilter.SRDateToValue.Value > searchFilter.SRDateFromValue.Value.AddMonths(monthOfReportExport.Value))
                    {
                        ModelState.AddModelError("dvDateRange", string.Format(CultureInfo.InvariantCulture, Resource.ValError_ReportExportDuration, monthOfReportExport.Value));
                    }
                }

                #endregion

                if (ModelState.IsValid)
                {
                    if (_reportFacade == null) { _reportFacade = new ReportFacade(); }
                    var vfdList = _reportFacade.GetExportVerifyDetail(searchFilter);

                    if (vfdList != null && vfdList.Count > 0)
                    {
                        //var bytes = ExcelHelpers.WriteToExcelVerifyDetail(HostingEnvironment.MapPath("~/Templates/Reports/rpt_verify_detail.xlsx"), DateTime.Now, ds);
                        TempData["FILE_EXPORT_VERIFYDETAIL"] = _reportFacade.CreateReportVerifyDetail(vfdList, searchFilter);

                        return Json(new
                        {
                            Valid = true,
                            Message = string.Empty,
                            Error = string.Empty,
                        });
                    }

                    return Json(new
                    {
                        Valid = false,
                        Error = Resource.Msg_NoRecords,
                    });
                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Export VerifyDetail").ToFailLogString());
                    return Json(new
                    {
                        Valid = false,
                        Error = string.Empty,
                        Errors = GetModelValidationErrors()
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Export VerifyDetail").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Message = string.Empty,
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ReportVerifyDetail)]
        public ActionResult PrintVerifyDetailExcel()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Print VerifyDetail").ToInputLogString());

            try
            {
                var bytes = TempData["FILE_EXPORT_VERIFYDETAIL"] as Byte[];

                string dateStr = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.ExportDateTime);
                string fileDownloadName = string.Format(CultureInfo.InvariantCulture, "{0}_{1}.{2}", Resource.Report_Verify_Detail_FileName, dateStr, Resource.Report_FileExtention);

                return File(bytes, "application/octet-stream", fileDownloadName);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Print VerifyDetail").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [CheckUserRole(ScreenCode.ReportVerify)]
        public ActionResult ExportVerify()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Export Verify").ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();
                _reportFacade = new ReportFacade();

                var verifyList = _reportFacade.GetVerifyResultSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.verify = new SelectList((IEnumerable)verifyList, "Key", "Value", string.Empty);

                ExportVerifyViewModel verifyVM = new ExportVerifyViewModel();
                verifyVM.SearchFilter = new ExportVerifySearchFilter()
                {
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = string.Empty,
                    SortOrder = "DESC"
                };

                return View("~/Views/Report/ExportVerify.cshtml", verifyVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Export Verify").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ReportVerify)]
        public JsonResult LoadVerifyExcel(ExportVerifySearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Export Verify").Add("ProductGroup", searchFilter.ProductGroup).Add("Product", searchFilter.Product)
                .Add("Campaign", searchFilter.Campaign).Add("Type", searchFilter.Type).Add("Area", searchFilter.Area)
                .Add("SubArea", searchFilter.SubArea).Add("OwnerSR", searchFilter.OwnerSR).Add("OwnerBranch", searchFilter.OwnerBranch)
                .Add("SRId", searchFilter.SRId).Add("SRIsverify", searchFilter.SRIsverify).Add("SRDateFrom", searchFilter.SRDateFrom)
                .Add("SRDateTo", searchFilter.SRDateTo).Add("Description", searchFilter.Description).ToInputLogString());

            try
            {
                #region "Check Time to Export"

                string errorMessage;
                bool isValid = CheckTimeToExport(out errorMessage);

                if (!isValid)
                {
                    return Json(new
                    {
                        Valid = false,
                        Error = errorMessage
                    });
                }

                #endregion

                #region "SR Time"

                if (!string.IsNullOrEmpty(searchFilter.SRTimeFrom) || !string.IsNullOrEmpty(searchFilter.SRTimeTo))
                {
                    if (string.IsNullOrEmpty(searchFilter.SRDateFrom))
                    {
                        ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_Required);
                    }
                    if (string.IsNullOrEmpty(searchFilter.SRDateTo))
                    {
                        ModelState.AddModelError("txtSRDateTo", Resource.ValErr_Required);
                    }
                    if (string.IsNullOrEmpty(searchFilter.SRTimeFrom))
                    {
                        ModelState.AddModelError("txtSRTimeFrom", Resource.ValErr_Required);
                    }
                    if (string.IsNullOrEmpty(searchFilter.SRTimeTo))
                    {
                        ModelState.AddModelError("txtSRTimeTo", Resource.ValErr_Required);
                    }
                }

                if (!string.IsNullOrEmpty(searchFilter.SRTimeFrom) && !string.IsNullOrEmpty(searchFilter.SRTimeTo))
                {
                    if (searchFilter.SRDateFromValue.HasValue && searchFilter.SRDateToValue.HasValue
                        && searchFilter.SRDateFromValue.Value == searchFilter.SRDateToValue.Value)
                    {
                        if ((searchFilter.SRTimeFrom.Replace(":", "")).ToNullable<int>() > (searchFilter.SRTimeTo.Replace(":", "")).ToNullable<int>())
                        {
                            ModelState.AddModelError("dvTimeRange", Resource.ValErr_InvalidTimeRange);
                        }
                    }
                }

                #endregion

                #region "Validation Report Criteria"

                _commonFacade = new CommonFacade();
                int? monthOfReportExport = _commonFacade.GetCacheParamByName(Constants.ParameterName.ReportExportDate).ParamValue.ToNullable<int>();

                if (!string.IsNullOrEmpty(searchFilter.SRDateFrom) && !searchFilter.SRDateFromValue.HasValue)
                {
                    ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_InvalidDate);
                }
                else if (searchFilter.SRDateFromValue.HasValue && searchFilter.SRDateFromValue.Value > DateTime.Now.Date)
                {
                    ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_InvalidDate_MustLessThanToday);
                }

                if (!string.IsNullOrEmpty(searchFilter.SRDateTo) && !searchFilter.SRDateToValue.HasValue)
                {
                    ModelState.AddModelError("txtSRDateTo", Resource.ValErr_InvalidDate);
                }
                else if (searchFilter.SRDateToValue.HasValue && searchFilter.SRDateToValue.Value > DateTime.Now.Date)
                {
                    ModelState.AddModelError("txtSRDateTo", Resource.ValErr_InvalidDate_MustLessThanToday);
                }

                if (searchFilter.SRDateFromValue.HasValue && searchFilter.SRDateToValue.HasValue
                    && searchFilter.SRDateFromValue.Value > searchFilter.SRDateToValue.Value)
                {
                    ModelState.AddModelError("dvDateRange", Resource.ValErr_InvalidDateRange);
                }

                // SRDate                
                if (!string.IsNullOrEmpty(searchFilter.SRDateFrom) && string.IsNullOrEmpty(searchFilter.SRDateTo))
                {
                    ModelState.AddModelError("txtSRDateTo", Resource.ValErr_Required);
                }

                if (string.IsNullOrEmpty(searchFilter.SRDateFrom) && !string.IsNullOrEmpty(searchFilter.SRDateTo))
                {
                    ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_Required);
                }

                if (!string.IsNullOrEmpty(searchFilter.SRDateFrom) && !string.IsNullOrEmpty(searchFilter.SRDateTo))
                {
                    if (searchFilter.SRDateToValue.Value > searchFilter.SRDateFromValue.Value.AddMonths(monthOfReportExport.Value))
                    {
                        ModelState.AddModelError("dvDateRange", string.Format(CultureInfo.InvariantCulture, Resource.ValError_ReportExportDuration, monthOfReportExport.Value));
                    }
                }

                #endregion

                if (ModelState.IsValid)
                {
                    if (_reportFacade == null) { _reportFacade = new ReportFacade(); }
                    var vfList = _reportFacade.GetExportVerify(searchFilter);

                    if (vfList != null && vfList.Count > 0)
                    {
                        //var bytes = ExcelHelpers.WriteToExcelVerify(HostingEnvironment.MapPath("~/Templates/Reports/rpt_verify.xlsx"), DateTime.Now, ds);
                        TempData["FILE_EXPORT_VERIFY"] = _reportFacade.CreateReportVerify(vfList, searchFilter);

                        return Json(new
                        {
                            Valid = true,
                            Message = string.Empty,
                            Error = string.Empty,
                        });
                    }

                    return Json(new
                    {
                        Valid = false,
                        Error = Resource.Msg_NoRecords,
                    });
                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Export Verify").ToFailLogString());
                    return Json(new
                    {
                        Valid = false,
                        Error = string.Empty,
                        Errors = GetModelValidationErrors()
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Export Verify").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Message = string.Empty,
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ReportVerify)]
        public ActionResult PrintVerifyExcel()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Print Verify").ToInputLogString());

            try
            {
                var bytes = TempData["FILE_EXPORT_VERIFY"] as Byte[];

                string dateStr = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.ExportDateTime);
                string fileDownloadName = string.Format(CultureInfo.InvariantCulture, "{0}_{1}.{2}", Resource.Report_Verify_FileName, dateStr, Resource.Report_FileExtention);

                return File(bytes, "application/octet-stream", fileDownloadName);
            }
            catch (Exception ex)
            {
                Logger.Error("Excepton occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Print Verify").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [CheckUserRole(ScreenCode.ReportNCB)]
        public ActionResult ExportNcb()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Export Ncb").ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();
                _reportFacade = new ReportFacade();
                _commPoolFacade = new CommPoolFacade();

                var customertypeList = _commonFacade.GetCustomerTypeSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.customertype = new SelectList((IEnumerable)customertypeList, "Key", "Value", string.Empty);

                var slaList = _reportFacade.GetSlaSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.sla = new SelectList((IEnumerable)slaList, "Key", "Value", string.Empty);

                var statusList = _commPoolFacade.GetSRStatusSelectList(Resource.Ddl_Status_All, Constants.ApplicationStatus.All);
                ViewBag.srstatus = new SelectList((IEnumerable)statusList, "Key", "Value", string.Empty);

                ExportNcbViewModel veVM = new ExportNcbViewModel();
                veVM.SearchFilter = new ExportNcbSearchFilter()
                {
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = string.Empty,
                    SortOrder = "DESC"
                };

                return View("~/Views/Report/ExportNcb.cshtml", veVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Export Ncb").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ReportNCB)]
        public JsonResult LoadNcbExcel(ExportNcbSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Export Ncb").Add("ProductGroup", searchFilter.ProductGroup).Add("Product", searchFilter.Product)
                .Add("Campaign", searchFilter.Campaign).Add("Type", searchFilter.Type).Add("Area", searchFilter.Area)
                .Add("SubArea", searchFilter.SubArea).Add("OwnerSR", searchFilter.OwnerSR).Add("OwnerBranch", searchFilter.OwnerBranch)
                .Add("SRId", searchFilter.SRId).Add("SRDateFrom", searchFilter.SRDateFrom).Add("SRDateTo", searchFilter.SRDateTo).ToInputLogString());

            try
            {
                #region "Check Time to Export"

                string errorMessage;
                bool isValid = CheckTimeToExport(out errorMessage);

                if (!isValid)
                {
                    return Json(new
                    {
                        Valid = false,
                        Error = errorMessage
                    });
                }

                #endregion

                #region "SR Time"

                if (!string.IsNullOrEmpty(searchFilter.SRTimeFrom) || !string.IsNullOrEmpty(searchFilter.SRTimeTo))
                {
                    if (string.IsNullOrEmpty(searchFilter.SRDateFrom))
                    {
                        ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_Required);
                    }
                    if (string.IsNullOrEmpty(searchFilter.SRDateTo))
                    {
                        ModelState.AddModelError("txtSRDateTo", Resource.ValErr_Required);
                    }
                    if (string.IsNullOrEmpty(searchFilter.SRTimeFrom))
                    {
                        ModelState.AddModelError("txtSRTimeFrom", Resource.ValErr_Required);
                    }
                    if (string.IsNullOrEmpty(searchFilter.SRTimeTo))
                    {
                        ModelState.AddModelError("txtSRTimeTo", Resource.ValErr_Required);
                    }
                }

                if (!string.IsNullOrEmpty(searchFilter.SRTimeFrom) && !string.IsNullOrEmpty(searchFilter.SRTimeTo))
                {
                    if (searchFilter.SRDateFromValue.HasValue && searchFilter.SRDateToValue.HasValue
                        && searchFilter.SRDateFromValue.Value == searchFilter.SRDateToValue.Value)
                    {
                        if ((searchFilter.SRTimeFrom.Replace(":", "")).ToNullable<int>() > (searchFilter.SRTimeTo.Replace(":", "")).ToNullable<int>())
                        {
                            ModelState.AddModelError("dvTimeRange", Resource.ValErr_InvalidTimeRange);
                        }
                    }
                }

                #endregion

                #region "Validation Report Criteria"

                _commonFacade = new CommonFacade();
                int? monthOfReportExport = _commonFacade.GetCacheParamByName(Constants.ParameterName.ReportExportDate).ParamValue.ToNullable<int>();

                if (!string.IsNullOrEmpty(searchFilter.BirthDate) && !searchFilter.BirthDateValue.HasValue)
                {
                    ModelState.AddModelError("txtBirthDate", Resource.ValErr_InvalidDate);
                }
                else if (searchFilter.BirthDateValue.HasValue && searchFilter.BirthDateValue.Value > DateTime.Now.Date)
                {
                    ModelState.AddModelError("txtBirthDate", Resource.ValErr_InvalidDate_MustLessThanToday);
                }

                if (!string.IsNullOrEmpty(searchFilter.SRDateFrom) && !searchFilter.SRDateFromValue.HasValue)
                {
                    ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_InvalidDate);
                }
                else if (searchFilter.SRDateFromValue.HasValue && searchFilter.SRDateFromValue.Value > DateTime.Now.Date)
                {
                    ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_InvalidDate_MustLessThanToday);
                }


                if (!string.IsNullOrEmpty(searchFilter.SRDateTo) && !searchFilter.SRDateToValue.HasValue)
                {
                    ModelState.AddModelError("txtSRDateTo", Resource.ValErr_InvalidDate);
                }
                else if (searchFilter.SRDateToValue.HasValue && searchFilter.SRDateToValue.Value > DateTime.Now.Date)
                {
                    ModelState.AddModelError("txtSRDateTo", Resource.ValErr_InvalidDate_MustLessThanToday);
                }

                if (searchFilter.SRDateFromValue.HasValue && searchFilter.SRDateToValue.HasValue
                    && searchFilter.SRDateFromValue.Value > searchFilter.SRDateToValue.Value)
                {
                    ModelState.AddModelError("dvDateRange", Resource.ValErr_InvalidDateRange);
                }

                // SRDate                
                if (!string.IsNullOrEmpty(searchFilter.SRDateFrom) && string.IsNullOrEmpty(searchFilter.SRDateTo))
                {
                    ModelState.AddModelError("txtSRDateTo", Resource.ValErr_Required);
                }

                if (string.IsNullOrEmpty(searchFilter.SRDateFrom) && !string.IsNullOrEmpty(searchFilter.SRDateTo))
                {
                    ModelState.AddModelError("txtSRDateFrom", Resource.ValErr_Required);
                }

                if (!string.IsNullOrEmpty(searchFilter.SRDateFrom) && !string.IsNullOrEmpty(searchFilter.SRDateTo))
                {
                    if (searchFilter.SRDateToValue.Value > searchFilter.SRDateFromValue.Value.AddMonths(monthOfReportExport.Value))
                    {
                        ModelState.AddModelError("dvDateRange", string.Format(CultureInfo.InvariantCulture, Resource.ValError_ReportExportDuration, monthOfReportExport.Value));
                    }
                }

                #endregion

                if (ModelState.IsValid)
                {
                    if (_reportFacade == null) { _reportFacade = new ReportFacade(); }
                    var ncbList = _reportFacade.GetExportNcb(searchFilter);

                    if (ncbList != null && ncbList.Count > 0)
                    {
                        //var bytes = ExcelHelpers.WriteToExcelNcb(HostingEnvironment.MapPath("~/Templates/Reports/rpt_ncb.xlsx"), DateTime.Now, ds);
                        TempData["FILE_EXPORT_NCB"] = _reportFacade.CreateReportNCB(ncbList, searchFilter);

                        return Json(new
                        {
                            Valid = true,
                            Message = string.Empty,
                            Error = string.Empty,
                        });
                    }

                    return Json(new
                    {
                        Valid = false,
                        Error = Resource.Msg_NoRecords,
                    });
                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Export Ncb").ToFailLogString());
                    return Json(new
                    {
                        Valid = false,
                        Error = string.Empty,
                        Errors = GetModelValidationErrors()
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Export Ncb").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Message = string.Empty,
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ReportNCB)]
        public ActionResult PrintNcbExcel()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Print Ncb").ToInputLogString());

            try
            {
                var bytes = TempData["FILE_EXPORT_NCB"] as Byte[];

                string dateStr = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.ExportDateTime);
                string fileDownloadName = string.Format(CultureInfo.InvariantCulture, "{0}_{1}.{2}", Resource.Report_Ncb_FileName, dateStr, Resource.Report_FileExtention);

                return File(bytes, "application/octet-stream", fileDownloadName);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Print Ncb").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpGet]
        public JsonResult SearchByActionName(string searchTerm, int pageSize, int pageNum, int? branchId)
        {
            //Get the paged results and the total count of the results for this query. 
            _commonFacade = new CommonFacade();
            List<UserEntity> users = _commonFacade.GetActionByName(searchTerm, pageSize, pageNum, branchId);
            int userCount = _commonFacade.GetActionCountByName(searchTerm, pageSize, pageNum, branchId);

            //Translate the attendees into a format the select2 dropdown expects
            //Select2PagedResult pagedUsers = ActionToSelect2Format(users, userCount);        

            Select2PagedResult pagedUsers = new Select2PagedResult();
            pagedUsers.Results = new List<Select2Result>();

            foreach (UserEntity user in users)
            {
                pagedUsers.Results.Add(new Select2Result { id = user.UserId, text = user.FullName });
            }

            pagedUsers.Total = userCount;


            //Return the data as a jsonp result
            return Json(pagedUsers, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchByBranchName(string searchTerm, int pageSize, int pageNum, int? userId)
        {
            //Get the paged results and the total count of the results for this query. 
            _commonFacade = new CommonFacade();
            List<BranchEntity> branches = _commonFacade.GetBranchesByName(searchTerm, pageSize, pageNum, userId);
            int branchCount = _commonFacade.GetBranchCountByName(searchTerm, pageSize, pageNum, userId);

            //Translate the attendees into a format the select2 dropdown expects
            //Select2PagedResult pagedBranches = this.BranchesToSelect2Format(branches);            

            Select2PagedResult pagedBranches = new Select2PagedResult();
            pagedBranches.Results = new List<Select2Result>();

            //Loop through our branches and translate it into a text value and an id for the select list
            foreach (BranchEntity branch in branches)
            {
                pagedBranches.Results.Add(new Select2Result { id = branch.BranchId, text = branch.BranchName });
            }
            pagedBranches.Total = branchCount;

            //Return the data as a jsonp result
            return Json(pagedBranches, JsonRequestBehavior.AllowGet);
        }

        #region "Functions"

        private bool CheckTimeToExport(out string errorMessage)
        {
            try
            {
                errorMessage = string.Empty;
                if (_reportFacade == null) { _reportFacade = new ReportFacade(); }

                string reportTimeStart = _reportFacade.GetParameter(Constants.ParameterName.ReportTimeStart);
                string reportTimeEnd = _reportFacade.GetParameter(Constants.ParameterName.ReportTimeEnd);

                IList<object> start = StringHelpers.ConvertStringToList(reportTimeStart, ':');
                IList<object> end = StringHelpers.ConvertStringToList(reportTimeEnd, ':');

                string hour = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.ShortTime);
                IList<object> hours = StringHelpers.ConvertStringToList(hour, ':');

                TimeSpan tsStart = new TimeSpan(Convert.ToInt32(start[0], CultureInfo.InvariantCulture), Convert.ToInt32(start[1], CultureInfo.InvariantCulture), 0);
                TimeSpan tsEnd = new TimeSpan(Convert.ToInt32(end[0], CultureInfo.InvariantCulture), Convert.ToInt32(end[1], CultureInfo.InvariantCulture), 0);
                TimeSpan tsHour = new TimeSpan(Convert.ToInt32(hours[0], CultureInfo.InvariantCulture), Convert.ToInt32(hours[1], CultureInfo.InvariantCulture), 0);

                if (tsHour >= tsStart && tsHour <= tsEnd)
                {
                    //return Json(new
                    //{
                    //    Valid = false,
                    //    Error = string.Format(CultureInfo.InvariantCulture, Resource.ValErr_ReportTime, reportTimeStart, reportTimeEnd)
                    //});

                    errorMessage = string.Format(CultureInfo.InvariantCulture, Resource.ValErr_ReportTime, reportTimeStart, reportTimeEnd);
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMessage = Resource.ValErr_ReportTimeConfiguration;
                Logger.Error("Exception occur:\n", ex);
                //return Json(new
                //{
                //    Valid = false,
                //    Error = Resource.ValErr_ReportTimeConfiguration
                //});
            }

            return true;
        }

        private bool ValidateCommPoolReport(ExportJobsSearchFilter searchFilter)
        {
            if (_commonFacade == null) { _commonFacade = new CommonFacade(); }
            int? monthOfReportExport = _commonFacade.GetCacheParamByName(Constants.ParameterName.ReportExportDate).ParamValue.ToNullable<int>();

            #region "Action Time"

            if (!string.IsNullOrEmpty(searchFilter.ActionTimeFrom) || !string.IsNullOrEmpty(searchFilter.ActionTimeTo))
            {
                if (string.IsNullOrEmpty(searchFilter.ActionDateFrom))
                {
                    ModelState.AddModelError("txtActionDateFrom", Resource.ValErr_Required);
                }
                if (string.IsNullOrEmpty(searchFilter.ActionDateTo))
                {
                    ModelState.AddModelError("txtActionDateTo", Resource.ValErr_Required);
                }
                if (string.IsNullOrEmpty(searchFilter.ActionTimeFrom))
                {
                    ModelState.AddModelError("txtActionTimeFrom", Resource.ValErr_Required);
                }
                if (string.IsNullOrEmpty(searchFilter.ActionTimeTo))
                {
                    ModelState.AddModelError("txtActionTimeTo", Resource.ValErr_Required);
                }
            }

            if (!string.IsNullOrEmpty(searchFilter.ActionTimeFrom) && !string.IsNullOrEmpty(searchFilter.ActionTimeTo))
            {
                if (searchFilter.ActionDateFromValue.HasValue && searchFilter.ActionDateToValue.HasValue
                    && searchFilter.ActionDateFromValue.Value == searchFilter.ActionDateToValue.Value)
                {
                    if ((searchFilter.ActionTimeFrom.Replace(":", "")).ToNullable<int>() > (searchFilter.ActionTimeTo.Replace(":", "")).ToNullable<int>())
                    {
                        ModelState.AddModelError("dvActionTimeRange", Resource.ValErr_InvalidTimeRange);
                    }
                }
            }

            #endregion

            #region "Job Time"

            if (!string.IsNullOrEmpty(searchFilter.JobTimeFrom) || !string.IsNullOrEmpty(searchFilter.JobTimeTo))
            {
                if (string.IsNullOrEmpty(searchFilter.JobDateFrom))
                {
                    ModelState.AddModelError("txtJobDateFrom", Resource.ValErr_Required);
                }
                if (string.IsNullOrEmpty(searchFilter.JobDateTo))
                {
                    ModelState.AddModelError("txtJobDateTo", Resource.ValErr_Required);
                }
                if (string.IsNullOrEmpty(searchFilter.JobTimeFrom))
                {
                    ModelState.AddModelError("txtJobTimeFrom", Resource.ValErr_Required);
                }
                if (string.IsNullOrEmpty(searchFilter.JobTimeTo))
                {
                    ModelState.AddModelError("txtJobTimeTo", Resource.ValErr_Required);
                }
            }

            if (!string.IsNullOrEmpty(searchFilter.JobTimeFrom) && !string.IsNullOrEmpty(searchFilter.JobTimeTo))
            {
                if (searchFilter.JobDateFromValue.HasValue && searchFilter.JobDateToValue.HasValue
                    && searchFilter.JobDateFromValue.Value == searchFilter.JobDateToValue.Value)
                {
                    if ((searchFilter.JobTimeFrom.Replace(":", "")).ToNullable<int>() > (searchFilter.JobTimeTo.Replace(":", "")).ToNullable<int>())
                    {
                        ModelState.AddModelError("dvJobTimeRange", Resource.ValErr_InvalidTimeRange);
                    }
                }
            }

            #endregion

            if (!string.IsNullOrEmpty(searchFilter.ActionDateFrom) && !searchFilter.ActionDateFromValue.HasValue)
            {
                ModelState.AddModelError("txtActionDateFrom", Resource.ValErr_InvalidDate);
            }
            else if (searchFilter.ActionDateFromValue.HasValue && searchFilter.ActionDateFromValue.Value > DateTime.Now.Date)
            {
                ModelState.AddModelError("txtActionDateFrom", Resource.ValErr_InvalidDate_MustLessThanToday);
            }

            if (!string.IsNullOrEmpty(searchFilter.ActionDateTo) && !searchFilter.ActionDateToValue.HasValue)
            {
                ModelState.AddModelError("txtActionDateTo", Resource.ValErr_InvalidDate);
            }
            else if (searchFilter.ActionDateToValue.HasValue && searchFilter.ActionDateToValue.Value > DateTime.Now.Date)
            {
                ModelState.AddModelError("txtActionDateTo", Resource.ValErr_InvalidDate_MustLessThanToday);
            }

            if (searchFilter.ActionDateFromValue.HasValue && searchFilter.ActionDateToValue.HasValue
                && searchFilter.ActionDateFromValue.Value > searchFilter.ActionDateToValue.Value)
            {
                ModelState.AddModelError("dvActionDateRange", Resource.ValErr_InvalidDateRange);
            }

            if (!string.IsNullOrEmpty(searchFilter.JobDateFrom) && !searchFilter.JobDateFromValue.HasValue)
            {
                ModelState.AddModelError("txtJobDateFrom", Resource.ValErr_InvalidDate);
            }
            else if (searchFilter.JobDateFromValue.HasValue && searchFilter.JobDateFromValue.Value > DateTime.Now.Date)
            {
                ModelState.AddModelError("txtJobDateFrom", Resource.ValErr_InvalidDate_MustLessThanToday);
            }

            if (!string.IsNullOrEmpty(searchFilter.JobDateTo) && !searchFilter.JobDateToValue.HasValue)
            {
                ModelState.AddModelError("txtJobDateTo", Resource.ValErr_InvalidDate);
            }
            else if (searchFilter.JobDateToValue.HasValue && searchFilter.JobDateToValue.Value > DateTime.Now.Date)
            {
                ModelState.AddModelError("txtJobDateTo", Resource.ValErr_InvalidDate_MustLessThanToday);
            }

            if (searchFilter.JobDateFromValue.HasValue && searchFilter.JobDateToValue.HasValue
                && searchFilter.JobDateFromValue.Value > searchFilter.JobDateToValue.Value)
            {
                ModelState.AddModelError("dvJobDateRange", Resource.ValErr_InvalidDateRange);
            }

            #region "Action Date"

            if (!string.IsNullOrEmpty(searchFilter.ActionDateFrom) && string.IsNullOrEmpty(searchFilter.ActionDateTo))
            {
                ModelState.AddModelError("txtActionDateTo", Resource.ValErr_Required);
            }
            if (string.IsNullOrEmpty(searchFilter.ActionDateFrom) && !string.IsNullOrEmpty(searchFilter.ActionDateTo))
            {
                ModelState.AddModelError("txtActionDateFrom", Resource.ValErr_Required);
            }

            if (!string.IsNullOrEmpty(searchFilter.ActionDateFrom) && !string.IsNullOrEmpty(searchFilter.ActionDateTo))
            {
                if (searchFilter.ActionDateToValue.Value > searchFilter.ActionDateFromValue.Value.AddMonths(monthOfReportExport.Value))
                {
                    ModelState.AddModelError("dvActionDateRange", string.Format(CultureInfo.InvariantCulture, Resource.ValError_ReportExportDuration, monthOfReportExport.Value));
                }
            }

            #endregion

            #region "Job Date"

            if (!string.IsNullOrEmpty(searchFilter.JobDateFrom) && string.IsNullOrEmpty(searchFilter.JobDateTo))
            {
                ModelState.AddModelError("txtJobDateTo", Resource.ValErr_Required);
            }

            if (string.IsNullOrEmpty(searchFilter.JobDateFrom) && !string.IsNullOrEmpty(searchFilter.JobDateTo))
            {
                ModelState.AddModelError("txtJobDateFrom", Resource.ValErr_Required);
            }

            if (!string.IsNullOrEmpty(searchFilter.JobDateFrom) && !string.IsNullOrEmpty(searchFilter.JobDateTo))
            {
                if (searchFilter.JobDateToValue.Value > searchFilter.JobDateFromValue.Value.AddMonths(monthOfReportExport.Value))
                {
                    ModelState.AddModelError("dvJobDateRange", string.Format(CultureInfo.InvariantCulture, Resource.ValError_ReportExportDuration, monthOfReportExport.Value));
                }
            }

            #endregion

            return ModelState.IsValid;
        }

        #endregion
    }
}