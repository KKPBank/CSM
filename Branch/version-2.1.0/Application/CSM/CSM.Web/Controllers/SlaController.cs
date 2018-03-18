using System;
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
using System.Globalization;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class SlaController : BaseController
    {
        private ISrChannelFacade _srChannelFacade;
        private ISrStatusFacade _srStatusFacade;
        private ISlaFacade _slaFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ContactController));
        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        public ActionResult Search()
        {
            _srChannelFacade = new SrChannelFacade();
            _srStatusFacade = new SrStatusFacade();

            var viewSlaVM = new SlaViewModel();
            var srChannelList = _srChannelFacade.GetSrChannelList();

            viewSlaVM.SrChannelList = srChannelList.Select(item => new SelectListItem()
            {
                Text = item.ChannelName,
                Value = item.ChannelId.ToString(CultureInfo.InvariantCulture)
            }).ToList();
            viewSlaVM.SrChannelList.Insert(0, new SelectListItem() { Text = "ทั้งหมด", Value = "-1" });

            var srStatusList = _srStatusFacade.GetSrStatusList();
            viewSlaVM.SrStatusList = srStatusList.Select(item => new SelectListItem()
            {
                Text = item.SRStatusName,
                Value = item.SRStatusId.ToString()
            }).ToList();
            viewSlaVM.SrStatusList.Insert(0, new SelectListItem() { Text = "ทั้งหมด", Value = "-1" });

            viewSlaVM.SearchFilter = new SlaSearchFilter()
            {
                ProductGroupId = null,
                ProductId = null,
                CampaignServiceId = null,
                AreaId = null,
                SubAreaId = null,
                TypeId = null,
                ChannelId = null,
                SrStatusId = null,
                PageNo = 1,
                PageSize = 15,
                SortField = "",
                SortOrder = "ASC"
            };

            return View(viewSlaVM);
        }

        public ActionResult Create()
        {
            _srChannelFacade = new SrChannelFacade();
            _srStatusFacade = new SrStatusFacade();
            var createSlaVM = new SlaCreateModel();
            var srChannelList = _srChannelFacade.GetSrChannelList();

            createSlaVM.SrChannelList = srChannelList.Select(item => new SelectListItem()
            {
                Text = item.ChannelName,
                Value = item.ChannelId.ToString(CultureInfo.InvariantCulture)
            }).ToList();
            createSlaVM.SrChannelList.Insert(0, new SelectListItem() { Text = "กรุณาเลือก", Value = "" });

            var srStatusList = _srStatusFacade.GetSrStatusList();
            createSlaVM.SrStatusList = srStatusList.Select(item => new SelectListItem()
            {
                Text = item.SRStatusName,
                Value = item.SRStatusId.ToString(CultureInfo.InvariantCulture)
            }).ToList();
            createSlaVM.SrStatusList.Insert(0, new SelectListItem() { Text = "กรุณาเลือก", Value = "" });

            ViewBag.CreateUsername = UserInfo.FullName;
            ViewBag.UpdateUsername = UserInfo.FullName;
            ViewBag.CreateDate = DateTime.Now;
            ViewBag.UpdateDate = DateTime.Now;

            return View(createSlaVM);
        }

        public ActionResult Edit()
        {
            return RedirectToAction("Search", "Sla");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? slaId)
        {
            _srChannelFacade = new SrChannelFacade();
            _srStatusFacade = new SrStatusFacade();
            _slaFacade = new SlaFacade();
            var editSlaVM = new SlaEditModel();

            var srChannelList = _srChannelFacade.GetSrChannelList();
            editSlaVM.SrChannelList = srChannelList.Select(item => new SelectListItem()
            {
                Text = item.ChannelName,
                Value = item.ChannelId.ToString(CultureInfo.InvariantCulture)
            }).ToList();
            editSlaVM.SrChannelList.Insert(0, new SelectListItem() { Text = "กรุณาเลือก", Value = "" });

            var srStatusList = _srStatusFacade.GetSrStatusList();
            editSlaVM.SrStatusList = srStatusList.Select(item => new SelectListItem()
            {
                Text = item.SRStatusName,
                Value = item.SRStatusId.ToString(CultureInfo.InvariantCulture)
            }).ToList();
            editSlaVM.SrStatusList.Insert(0, new SelectListItem() { Text = "กรุณาเลือก", Value = "" });

            if (slaId.HasValue)
            {
                SlaItemEntity slaItemEntity = _slaFacade.GetSlaById(slaId);
                if (slaItemEntity != null && slaItemEntity.SlaId.HasValue)
                {
                    editSlaVM.SlaId = slaItemEntity.SlaId;
                    editSlaVM.ProductId = slaItemEntity.ProductId;
                    editSlaVM.ProductGroupId = slaItemEntity.ProductGroupId;
                    editSlaVM.CampaignServiceId = slaItemEntity.CampaignServiceId;
                    editSlaVM.AreaId = slaItemEntity.AreaId;
                    editSlaVM.SubAreaId = slaItemEntity.SubAreaId;
                    editSlaVM.TypeId = slaItemEntity.TypeId;

                    editSlaVM.ProductName = slaItemEntity.ProductName;
                    editSlaVM.ProductGroupName = slaItemEntity.ProductGroupName;
                    editSlaVM.CampaignServiceName = slaItemEntity.CampaignName;
                    editSlaVM.AreaName = slaItemEntity.AreaName;
                    editSlaVM.SubAreaName = slaItemEntity.SubAreaName;
                    editSlaVM.TypeName = slaItemEntity.TypeName;

                    editSlaVM.ChannelId = slaItemEntity.ChannelId;
                    editSlaVM.SrChannelId = slaItemEntity.ChannelId;
                    editSlaVM.SrStatusId = slaItemEntity.SrStatusId;
                    editSlaVM.SrStatusName = slaItemEntity.StatusName;
                    editSlaVM.SrStateId = slaItemEntity.SrStateId;
                    editSlaVM.SrStateName = slaItemEntity.StateName;
                    editSlaVM.SlaMinute = slaItemEntity.SlaMinute;
                    editSlaVM.SlaTimes = slaItemEntity.SlaTimes;

                    editSlaVM.AlertChiefTimes = slaItemEntity.AlertChiefTimes;
                    editSlaVM.AlertChief1Times = slaItemEntity.AlertChief1Times;
                    editSlaVM.AlertChief2Times = slaItemEntity.AlertChief2Times;
                    editSlaVM.AlertChief3Times = slaItemEntity.AlertChief3Times;

                    editSlaVM.SlaDay = slaItemEntity.SlaDay;
                    editSlaVM.CreateUserName = slaItemEntity.CreateUser != null ? slaItemEntity.CreateUser.FullName : "";
                    editSlaVM.CreateDate = DateUtil.ToStringAsDateTime(slaItemEntity.CreateDate);
                    editSlaVM.UpdateUserName = slaItemEntity.UpdateUser != null ? slaItemEntity.UpdateUser.FullName : "";
                    editSlaVM.UpdateDate = DateUtil.ToStringAsDateTime(slaItemEntity.UpdateDate);
                }
            }

            return View(editSlaVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Save(SlaSaveModel slaSaveVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("SLA Save").ToInputLogString());
            try
            {
                if (ModelState.IsValid)
                {
                    _slaFacade = new SlaFacade();

                    //validate duplicate sla information
                    var isValidate = _slaFacade.ValidateSla(
                        slaSaveVM.SlaId,
                        slaSaveVM.ProductId,
                        slaSaveVM.CampaignServiceId,
                        slaSaveVM.AreaId,
                        slaSaveVM.SubAreaId,
                        slaSaveVM.TypeId,
                        slaSaveVM.ChannelId,
                        slaSaveVM.SrStatusId);

                    if (!isValidate)
                        return Json(new { Valid = false, Error = Resource.Error_SaveFailedDuplicate}) ;
 
                    SlaItemEntity slaItemEntity = new SlaItemEntity()
                    {
                        SlaId = slaSaveVM.SlaId,
                        ProductId = slaSaveVM.ProductId,
                        CampaignServiceId = slaSaveVM.CampaignServiceId,
                        AreaId = slaSaveVM.AreaId,
                        SubAreaId = slaSaveVM.SubAreaId,
                        TypeId = slaSaveVM.TypeId,
                        ChannelId = slaSaveVM.ChannelId,
                        SrStatusId = slaSaveVM.SrStatusId,
                        SlaMinute = slaSaveVM.SlaMinute,
                        SlaTimes = slaSaveVM.SlaTimes,
                        SlaDay = slaSaveVM.SlaDay,
                        UserId = UserInfo.UserId,
                        AlertChiefTimes = slaSaveVM.AlertChiefTimes,
                        AlertChief1Times = slaSaveVM.AlertChief1Times,
                        AlertChief2Times = slaSaveVM.AlertChief2Times,
                        AlertChief3Times = slaSaveVM.AlertChief3Times
                    };
                    
                    _slaFacade.SaveSLA(slaItemEntity);

                    return Json(new
                    {
                        IsSuccess = true
                    });
                }

                return Json(new
                {
                    Valid = false,
                    Error = string.Empty,
                    Errors = GetModelValidationErrors(),
                });

            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("SLA Save").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = string.Format(CultureInfo.InvariantCulture, "Technical Error : {0}", ex.Message),
                });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchSlaList(SlaSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search SLA").ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _slaFacade = new SlaFacade();
                    SlaViewModel slaVM = new SlaViewModel();
                    slaVM.SearchFilter = searchFilter;

                    slaVM.SlaList = _slaFacade.GetSlaList(slaVM.SearchFilter);
                    ViewBag.PageSize = slaVM.SearchFilter.PageSize;

                    return PartialView("~/Views/Sla/_SlaList.cshtml", slaVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search SLA").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public JsonResult Delete(int slaId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Sla").ToInputLogString());
            try
            {
                _slaFacade = new SlaFacade();
                SlaDeleteModel model = new SlaDeleteModel();
                model.SlaId = slaId;

                var isSuccess = _slaFacade.DeleteSla(slaId);

                return Json(new
                {
                    Valid = isSuccess,
                    Message = "Delete Success"
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Sla").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = Resource.Error_System,
                    Errors = string.Empty
                });
            }
        }
    }
}