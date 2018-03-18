using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CSM.Business;
using CSM.Common.Resources;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Entity.Common;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class BranchController : BaseController
    {
        private ICommonFacade _commonFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BranchController));

        public ActionResult SearchBranch()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Branch").ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();

                BranchViewModel branchVM = new BranchViewModel();
                branchVM.SearchFilter = new BranchSearchFilter
                {
                    BranchName = string.Empty,
                    JsonBranch = string.Empty,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "BranchId",
                    SortOrder = "DESC"
                };

                ViewBag.PageSize = branchVM.SearchFilter.PageSize;
                ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.Message = string.Empty;

                return PartialView("~/Views/Branch/_BranchSearch.cshtml", branchVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitSearch Branch").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        public ActionResult BranchList(BranchSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Branch").Add("BranchName", searchFilter.BranchName).ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _commonFacade = new CommonFacade();
                    BranchViewModel branchVM = new BranchViewModel();
                    branchVM.SearchFilter = searchFilter;

                    branchVM.BranchList = _commonFacade.GetBranchList(branchVM.SearchFilter);
                    ViewBag.PageSize = branchVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();

                    return PartialView("~/Views/Branch/_BranchList.cshtml", branchVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Branch").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpGet]
        public JsonResult SearchByBranchName(string searchTerm, int pageSize, int pageNum)
        {
            //Get the paged results and the total count of the results for this query. 
            _commonFacade = new CommonFacade();
            List<BranchEntity> branches = _commonFacade.GetBranchesByName(searchTerm, pageSize, pageNum);
            int branchCount = _commonFacade.GetBranchCountByName(searchTerm, pageSize, pageNum);

            //Translate the attendees into a format the select2 dropdown expects
            //Select2PagedResult pagedBranches = BranchesToSelect2Format(branches, branchCount);

            Select2PagedResult pagedBranches = new Select2PagedResult();
            pagedBranches.Results = new List<Select2Result>();

            //Loop through our branches and translate it into a text value and an id for the select list
            foreach (BranchEntity branch in branches)
            {
                pagedBranches.Results.Add(new Select2Result { id = branch.BranchId, text = branch.BranchName });
            }

            //Set the total count of the results from the query.
            pagedBranches.Total = branchCount;

            //Return the data as a jsonp result
            return Json(pagedBranches, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SelectBranch(BranchViewModel branchVM)
        {
            string errorMsg = string.Empty;
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Select Branch").ToInputLogString());

            try
            {

                if (ModelState.IsValid)
                {
                    if (branchVM.BranchCheckBoxes == null || branchVM.BranchCheckBoxes.Count == 0)
                    {
                        errorMsg = Resource.ValErr_AtLeastOneItem;
                        goto Outer;
                    }

                    var selectedBranch = branchVM.BranchCheckBoxes.Where(x => x.Checked == true)
                                .Select(x => new PoolBranchEntity
                                {
                                    BranchId = x.Value.ToNullable<int>()
                                }).ToList();

                    if (selectedBranch.Any() == false)
                    {
                        errorMsg = Resource.ValErr_AtLeastOneItem;
                        goto Outer;
                    }

                    var prevBranches = branchVM.SearchFilter.Branches;
                    var dupeBranches = new List<BranchEntity>(selectedBranch);
                    dupeBranches.AddRange(prevBranches);

                    var duplicates = dupeBranches.GroupBy(x => new { x.BranchId })
                           .Where(g => g.Count() > 1)
                           .Select(g => (object)g.Key.BranchId);

                    if (duplicates.Any())
                    {
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Duplicate ID in list")
                            .Add("IDs", StringHelpers.ConvertListToString(duplicates.ToList(), ",")).ToInputLogString());
                        prevBranches.RemoveAll(x => duplicates.Contains(x.BranchId));
                    }

                    selectedBranch.AddRange(prevBranches);

                    return Json(new
                    {
                        Valid = true,
                        Data = selectedBranch
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
                errorMsg = ex.Message;
                Logger.Error("Exception occur:\n", ex);
            }

        Outer:
            return Json(new
            {
                Valid = false,
                Error = errorMsg
            });
        }

        //private Select2PagedResult BranchesToSelect2Format(List<BranchEntity> branches, int total)
        //{
        //    Select2PagedResult jsonBranches = new Select2PagedResult();
        //    jsonBranches.Results = new List<Select2Result>();

        //    //Loop through our branches and translate it into a text value and an id for the select list
        //    foreach (BranchEntity branch in branches)
        //    {
        //        jsonBranches.Results.Add(new Select2Result { id = branch.BranchId, text = branch.BranchName });
        //    }

        //    //Set the total count of the results from the query.
        //    jsonBranches.Total = total;

        //    return jsonBranches;
        //}
    }
}
