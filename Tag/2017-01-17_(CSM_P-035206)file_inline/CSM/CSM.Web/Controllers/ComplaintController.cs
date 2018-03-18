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
    public class ComplaintController : BaseController
    {
        const int AutoCompleteMaxResult = 10;

        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CommPoolController));

        ComplaintFacade _complaintFacade = new ComplaintFacade();

        #region Autocomplete

        [HttpPost]
        public ActionResult AutoCompleteSubject(string keyword, int? typeId, int? rootCauseId, bool? isAllStatus)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: Auto Complete Subject").Add("Keyword", keyword)
                .Add("typeId", typeId).Add("rootCauseId", rootCauseId).Add("IsAllStatus", isAllStatus).ToInputLogString());
            try
            {
                List<ComplaintSubjectEntity> result = _complaintFacade.AutoCompleteSubject(AutoCompleteMaxResult, keyword, typeId, rootCauseId, isAllStatus);
                return Json(result.Select(r => new
                {
                    r.ComplaintSubjectId,
                    r.ComplaintSubjectName,
                    r.ComplaintSubjectStatus
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: Auto Complete Subject").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult AutoCompleteType(string keyword, int? subjectId, int? rootCauseId, bool? isAllStatus)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: Auto Complete Type").Add("Keyword", keyword)
                .Add("subjectId", subjectId).Add("rootCauseId", rootCauseId).Add("IsAllStatus", isAllStatus).ToInputLogString());
            try
            {
                List<ComplaintTypeEntity> result = _complaintFacade.AutoCompleteType(AutoCompleteMaxResult, keyword, subjectId, rootCauseId, isAllStatus);
                return Json(result.Select(r => new
                {
                    r.ComplaintTypeId,
                    r.ComplaintTypeName,
                    r.ComplaintTypeStatus
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: Auto Complete Type").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult AutoCompleteRootCause(string keyword, int? subjectId, int? typeId, bool? isAllStatus)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: Auto Complete RootCause").Add("Keyword", keyword)
                .Add("subjectId", subjectId).Add("typeId", typeId).Add("IsAllStatus", isAllStatus).ToInputLogString());
            try
            {
                List<RootCauseEntity> result = _complaintFacade.AutoCompleteRootCause(20, keyword, subjectId, typeId, isAllStatus);
                return Json(result.Select(r => new
                {
                    r.RootCauseId,
                    r.RootCauseName,
                    r.RootCauseStatus
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: Auto Complete RootCause").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AutoCompleteBU3(string keyword, string BU1, string BU2, bool? isAllStatus)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: AutoComplete BU3").Add("Keyword", keyword)
                .Add("BU1", BU1).Add("BU2", BU2).Add("IsAllStatus", isAllStatus).ToInputLogString());
            try
            {
                List<ComplaintBUEntity> result = _complaintFacade.AutoCompleteBU3(AutoCompleteMaxResult, keyword, BU1, BU2, isAllStatus);
                return Json(result.Select(r => new
                {
                    r.BU_Code,
                    r.BU_Desc,
                    r.BU_Status
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: Auto Complete BU3").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AutoCompleteBU2(string keyword, string BU1, string BU3, bool? isAllStatus)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: AutoComplete BU2").Add("Keyword", keyword)
                .Add("BU1", BU1).Add("BU3", BU3).Add("IsAllStatus", isAllStatus).ToInputLogString());
            try
            {
                List<ComplaintBUEntity> result = _complaintFacade.AutoCompleteBU2(AutoCompleteMaxResult, keyword, BU1, BU3, isAllStatus);
                return Json(result.Select(r => new
                {
                    r.BU_Code,
                    r.BU_Desc,
                    r.BU_Status
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: Auto Complete BU3").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AutoCompleteBU1(string keyword, string BU2, string BU3, bool? isAllStatus)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: AutoComplete BU1").Add("Keyword", keyword)
                .Add("BU2", BU2).Add("BU3", BU3).Add("IsAllStatus", isAllStatus).ToInputLogString());
            try
            {
                List<ComplaintBUEntity> result = _complaintFacade.AutoCompleteBU1(AutoCompleteMaxResult, keyword, BU2, BU3, isAllStatus);
                return Json(result.Select(r => new
                {
                    r.BU_Code,
                    r.BU_Desc,
                    r.BU_Status
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: Auto Complete BU3").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AutoCompleteHRBranch(string keyword, bool? isAllStatus)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: AutoCompleteHRBranch")
                .Add("Keyword", keyword).Add("IsAllStatus", isAllStatus).ToInputLogString());
            try
            {
                List<MSHBranchEntity> result = _complaintFacade.AutoCompleteHRBranch(AutoCompleteMaxResult, keyword, isAllStatus);
                return Json(result.Select(r => new
                {
                    r.Branch_Id,
                    r.Branch_Code,
                    r.Branch_Name
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: Auto Complete HRBranch").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDefaultBU(string BU1, string BU2, string BU3, bool? isAllStatus)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: GetDefaultBU").Add("BU1", BU1)
                .Add("BU2", BU2).Add("BU3", BU3).Add("IsAllStatus", isAllStatus).ToInputLogString());
            try
            {
                List<ComplaintHRBUEntity> result = _complaintFacade.GetDefaultBU(AutoCompleteMaxResult, BU1, BU2, BU3, isAllStatus);
                if (result != null && result.Count == 1)
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        ErrorMessage = "",
                        data = result.FirstOrDefault()
                    });
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Can not default record for BU1 [{BU1}] BU2 [{BU2}] BU3 [{BU3}]"
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: Auto Complete BU3").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDefaultCPNMapping(int? subjectId = null, int? typeId = null, int? rootCauseId = null, bool? isAllStatus = null, int? take = null)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Mapping").Add("subjectId", subjectId)
                .Add("typeId", typeId)
                .Add("rootCauseId", rootCauseId)
                .Add("isAllStatus", isAllStatus)
                .Add("take", take).ToInputLogString());
            try
            {
                List<ComplaintMappingEntity> maps = _complaintFacade.GetComplaintMapping(subjectId, typeId, rootCauseId, isAllStatus, take);
                ComplaintSubjectEntity subj = null;
                ComplaintTypeEntity type = null;
                RootCauseEntity cause = null;
                if (maps != null)
                {
                    if (maps.Count == 1)
                    {
                        ComplaintMappingEntity map = maps.FirstOrDefault();
                        subj = _complaintFacade.GetSubjectById(map.ComplaintSubjectId.Value);
                        type = _complaintFacade.GetTypeById(map.ComplaintTypetId.Value);
                        cause = _complaintFacade.GetRootCauseById(map.RootCauseId.Value);
                        return Json(new
                        {
                            IsSuccess = true,
                            MappingId = map.ComplaintMappingId,
                            ComplaintSubjectId = subj.ComplaintSubjectId,
                            ComplaintSubjectName = subj.ComplaintSubjectName,
                            ComplaintTypeId = type.ComplaintTypeId,
                            ComplaintTypeName = type.ComplaintTypeName,
                            RootCauseId = cause.RootCauseId,
                            RootCauseName = cause.RootCauseName
                        });
                    }
                }
                return Json(new
                {
                    IsSuccess = false,
                    //ErrorMessage = $"No mapping found! <br />for subjectId [{subjectId}], typeId [{typeId}], rootCauseId [{rootCauseId}], isAllStatus [{isAllStatus}]"
                    ErrorMessage = ""
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Subject By Id").Add("Error Message", ex.Message).ToInputLogString());
                return Json(new
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetHRBranchById(int id)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: GetHRBranch").Add("id", id).ToInputLogString());
            try
            {
                MSHBranchEntity result = _complaintFacade.GetHRBranch(id).FirstOrDefault();
                if (result != null)
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        data = result
                    });
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        ErrorMessage = $"No record for MSHBranch id = {id}."
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Complaint :: Auto Complete BU3").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetSubjectById(int subjectId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Subject By Id").Add("subjectId", subjectId).ToInputLogString());
            try
            {
                ComplaintSubjectEntity subj = _complaintFacade.GetSubjectById(subjectId);
                return Json(new
                {
                    IsSuccess = true,
                    ComplaintSubjectId = subj.ComplaintSubjectId,
                    ComplaintSubjectName = subj.ComplaintSubjectName,
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Subject By Id").Add("Error Message", ex.Message).ToInputLogString());
                return Json(new
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetTypeById(int typeId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Type By Id").Add("typeId", typeId).ToInputLogString());
            try
            {
                ComplaintSubjectEntity subj = _complaintFacade.GetSubjectById(typeId);
                ComplaintMappingEntity map = _complaintFacade.GetComplaintMapping(typeId: typeId, isAllStatus: false, take: 1).FirstOrDefault();
                ComplaintTypeEntity type = null;
                RootCauseEntity cause = null;
                if (map != null)
                {
                    type = _complaintFacade.GetTypeById(map.ComplaintTypetId.Value);
                    cause = _complaintFacade.GetRootCauseById(map.RootCauseId.Value);
                    return Json(new
                    {
                        IsSuccess = true,
                        ComplaintSubjectId = subj.ComplaintSubjectId,
                        ComplaintSubjectName = subj.ComplaintSubjectName,
                        ComplaintTypeId = type?.ComplaintTypeId,
                        ComplaintTypeName = type?.ComplaintTypeName,
                        RootCauseId = cause?.RootCauseId,
                        RootCauseName = cause?.RootCauseName
                    });
                }
                return Json(new
                {
                    IsSuccess = false,
                    ErrorMessage = ""
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Get CPNSubject By Id").Add("Error Message", ex.Message).ToInputLogString());
                return Json(new
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
        }

    }
}
