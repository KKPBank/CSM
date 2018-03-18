using System;
using System.Web.Mvc;
using CSM.Business;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Web.Controllers.Common;
using CSM.Web.Filters;
using CSM.Web.Models;
using log4net;
using CSM.Common.Resources;
using System.Collections.Generic;
using System.Linq;
using CSM.Entity.Common;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class DocumentController : BaseController
    {
        private ICommonFacade _commonFacade;
        private ICustomerFacade _customerFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DocumentController));

        public ActionResult List(string encryptedString)
        {
            try
            {
                int? customerId = encryptedString.ToCustomerId();
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Document").Add("CustomerId", customerId).ToInputLogString());

                if (customerId == null)
                {
                    return RedirectToAction("Search", "Customer");
                }

                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();

                DocumentViewModel documentVM = new DocumentViewModel();
                CustomerInfoViewModel custInfoVM = this.MappingCustomerInfoView(customerId.Value);
                documentVM.CustomerInfo = custInfoVM;

                // Attachment list
                documentVM.SearchFilter = new AttachmentSearchFilter
                {
                    CustomerId = custInfoVM.CustomerId.Value,
                    PageNo = 1,
                    PageSize = _commonFacade.GetPageSizeStart(),
                    SortField = "ExpiryDate",
                    SortOrder = "DESC"
                };

                documentVM.AttachmentList = _customerFacade.GetAttachmentList(documentVM.SearchFilter);
                ViewBag.CurrentUserId = this.UserInfo.UserId; // for check btnEdit btnDelete
                ViewBag.PageSize = documentVM.SearchFilter.PageSize;
                ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                ViewBag.Message = string.Empty;

                return View(documentVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List Customer").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerDocument)]
        public ActionResult CustomerAttachmentList(AttachmentSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List CustomerAttachment").Add("CustomerId", searchFilter.CustomerId).ToInputLogString());

            try
            {
                if (ModelState.IsValid)
                {
                    _commonFacade = new CommonFacade();
                    _customerFacade = new CustomerFacade();
                    DocumentViewModel docVM = new DocumentViewModel();
                    docVM.SearchFilter = searchFilter;

                    docVM.AttachmentList = _customerFacade.GetAttachmentList(docVM.SearchFilter);
                    ViewBag.PageSize = docVM.SearchFilter.PageSize;
                    ViewBag.PageSizeList = _commonFacade.GetPageSizeList();
                    ViewBag.CurrentUserId = this.UserInfo.UserId; // for check btnEdit btnDelete

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("CustomerAttachmentList").ToSuccessLogString());
                    return PartialView("~/Views/Document/_CustomerAttachmentList.cshtml", docVM);
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
                Logger.Info(_logMsg.Clear().SetPrefixMsg("List CustomerAttachment").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerDocument)]
        public ActionResult InitEdit(int? attachmentId, int? customerId, string documentLevel)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitEdit Attachment").Add("AttachmentId", attachmentId)
                .Add("CustomerId", customerId).Add("DocumentLevel", documentLevel).ToInputLogString());

            try
            {
                _commonFacade = new CommonFacade();
                _customerFacade = new CustomerFacade();

                #region "For show in hint"

                ParameterEntity param = _commonFacade.GetCacheParamByName(Constants.ParameterName.RegexFileExt);
                ParameterEntity paramSingleFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.SingleFileSize);

                int? limitSingleFileSize = paramSingleFileSize.ParamValue.ToNullable<int>();
                var singleLimitSize = limitSingleFileSize.HasValue ? (limitSingleFileSize / 1048576) : 0;
                ViewBag.UploadLimitType = string.Format(CultureInfo.InvariantCulture, param.ParamDesc, singleLimitSize);

                #endregion

                var attachVM = new AttachViewModel();
                List<DocumentTypeEntity> docTypeList = null;

                if (attachmentId.HasValue)
                {
                    AttachmentEntity selectedAttach = _customerFacade.GetAttachmentByID(attachmentId.Value, documentLevel);
                    attachVM.Filename = selectedAttach.Filename;
                    attachVM.DocName = selectedAttach.Name;
                    attachVM.DocDesc = selectedAttach.Description;
                    attachVM.ExpiryDate = selectedAttach.ExpiryDateDisplay;
                    attachVM.CustomerId = selectedAttach.CustomerId;
                    attachVM.Status = selectedAttach.Status;

                    attachVM.AttachmentId = attachmentId.Value;

                    docTypeList = _commonFacade.GetDocumentTypeList(selectedAttach.AttachTypeList, Constants.DocumentCategory.Customer);
                }
                else
                {
                    attachVM.CustomerId = customerId; // case new
                    docTypeList = _commonFacade.GetActiveDocumentTypes(Constants.DocumentCategory.Customer);
                }

                if (docTypeList != null)
                {
                    attachVM.JsonAttachType = JsonConvert.SerializeObject(docTypeList);
                    attachVM.DocTypeCheckBoxes = docTypeList.Select(x => new CheckBoxes
                    {
                        Value = x.DocTypeId.ToString(),
                        Text = x.Name,
                        Checked = x.IsChecked
                    }).ToList();
                }

                //var statusList = _commonFacade.GetStatusSelectList();
                //attachVM.StatusList = new SelectList((IEnumerable)statusList, "Key", "Value", string.Empty);

                return PartialView("~/Views/Document/_EditAttachment.cshtml", attachVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitEdit Attachment").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerDocument)]
        public ActionResult Edit(AttachViewModel attachVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Attachment").Add("DocName", attachVM.DocName)
                .Add("DocDesc", attachVM.DocDesc).ToInputLogString());

            try
            {
                if (attachVM.AttachmentId.HasValue && attachVM.AttachmentId != 0)
                {
                    ModelState.Remove("FileAttach");
                }

                if (string.IsNullOrEmpty(attachVM.ExpiryDate))
                {
                    ModelState.AddModelError("ExpiryDate", string.Format(CultureInfo.InvariantCulture, Resource.ValErr_RequiredField, Resource.Lbl_ExpiryDate));
                }
                else
                {
                    if (attachVM.ExpiryDate.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate) < DateTime.Now.Date)
                    {
                        ModelState.AddModelError("ExpiryDate", Resource.ValErr_InvalidDate_MustMoreThanToday);
                    }
                }

                //if (attachVM.Status.HasValue == false)
                //{
                //    ModelState.AddModelError("Status", string.Format(CultureInfo.InvariantCulture, Resource.ValErr_RequiredField, Resource.Lbl_Status_Thai));
                //}

                // Validate MaxLength
                if (attachVM.DocDesc != null && attachVM.DocDesc.Count() > Constants.MaxLength.AttachDesc)
                {
                    ModelState.AddModelError("DocDesc", string.Format(CultureInfo.InvariantCulture, Resource.ValErr_StringLength, Resource.Lbl_Detail, Constants.MaxLength.AttachDesc));
                    goto Outer;
                }

                if (ModelState.IsValid)
                {
                    AttachmentEntity attach = new AttachmentEntity();

                    var file = attachVM.FileAttach;
                    attach.Name = attachVM.DocName;
                    attach.Description = attachVM.DocDesc;
                    attach.ExpiryDate = attachVM.ExpiryDate.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate);
                    attach.CustomerId = attachVM.CustomerId;
                    attach.AttachmentId = attachVM.AttachmentId.HasValue ? attachVM.AttachmentId.Value : 0;
                    attach.Status = Constants.ApplicationStatus.Active;

                    #region "AttachType"

                    var selectedAttachType = attachVM.DocTypeCheckBoxes.Where(x => x.Checked == true)
                        .Select(x => new AttachmentTypeEntity
                        {
                            DocTypeId = x.Value.ToNullable<int>(),
                            CreateUserId = this.UserInfo.UserId
                        }).ToList();

                    if (attachVM.AttachTypeList != null && attachVM.AttachTypeList.Count > 0)
                    {
                        var prevAttachTypes = (from at in attachVM.AttachTypeList
                                               select new AttachmentTypeEntity
                                               {
                                                   AttachmentId = at.AttachmentId,
                                                   Code = at.Code,
                                                   Name = at.Name,
                                                   DocTypeId = at.DocTypeId,
                                                   IsDelete = !selectedAttachType.Select(x => x.DocTypeId).Contains(at.DocTypeId),
                                                   Status = at.Status,
                                                   CreateUserId = this.UserInfo.UserId
                                               }).ToList();

                        var dupeAttachTypes = new List<AttachmentTypeEntity>(selectedAttachType);
                        dupeAttachTypes.AddRange(prevAttachTypes);

                        var duplicates = dupeAttachTypes.GroupBy(x => new { x.DocTypeId })
                            .Where(g => g.Count() > 1)
                            .Select(g => (object)g.Key.DocTypeId);

                        if (duplicates.Any())
                        {
                            //Logger.Info(_logMsg.Clear().SetPrefixMsg("Duplicate ID in list")
                            //    .Add("IDs", StringHelpers.ConvertListToString(duplicates.ToList(), ",")).ToInputLogString());
                            prevAttachTypes.RemoveAll(x => duplicates.Contains(x.DocTypeId));
                        }

                        selectedAttachType.AddRange(prevAttachTypes);
                    }

                    attach.AttachTypeList = selectedAttachType;

                    #endregion

                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {
                        _commonFacade = new CommonFacade();
                        ParameterEntity paramSingleFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.SingleFileSize);
                        int? limitSingleFileSize = paramSingleFileSize.ParamValue.ToNullable<int>();

                        if (file.ContentLength > limitSingleFileSize.Value)
                        {
                            ModelState.AddModelError("FileAttach", string.Format(CultureInfo.InvariantCulture, Resource.ValError_SingleFileSizeExceedMaxLimit, (limitSingleFileSize.Value / 1048576)));
                            goto Outer;
                        }

                        // extract only the filename
                        var fileName = Path.GetFileName(file.FileName);


                        ParameterEntity param = _commonFacade.GetCacheParamByName(Constants.ParameterName.RegexFileExt);
                        Match match = Regex.Match(fileName, param.ParamValue, RegexOptions.IgnoreCase);

                        if (!match.Success)
                        {
                            ModelState.AddModelError("FileAttach", Resource.ValError_FileExtension);
                            goto Outer;
                        }

                        var docFolder = _commonFacade.GetCSMDocumentFolder();
                        int seqNo = _commonFacade.GetNextAttachmentSeq();
                        var fileNameUrl = ApplicationHelpers.GenerateFileName(docFolder, Path.GetExtension(file.FileName), seqNo, Constants.AttachmentPrefix.Customer);

                        var targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", docFolder, fileNameUrl);
                        file.SaveAs(targetFile);

                        attach.Url = fileNameUrl;
                        attach.Filename = fileName;
                        attach.ContentType = file.ContentType;
                    }

                    attach.CreateUserId = this.UserInfo.UserId; // for add CustomerLog

                    _customerFacade = new CustomerFacade();
                    _customerFacade.SaveCustomerAttachment(attach);

                    return Json(new
                    {
                        Valid = true
                    }, "text/html");
                }

            Outer:
                return Json(new
                {
                    Valid = false,
                    Error = string.Empty,
                    Errors = GetModelValidationErrors()
                }, "text/html");
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Edit Attachment").Add("Error Message", ex.Message).ToFailLogString());
                return Json(new
                {
                    Valid = false,
                    Error = Resource.Error_System,
                    Errors = string.Empty
                }, "text/html");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerDocument)]
        public JsonResult DeleteAttachment(int attachmentId)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Attachment").Add("AttachmentId", attachmentId).ToInputLogString());

            try
            {
                _customerFacade = new CustomerFacade();
                _customerFacade.DeleteCustomerAttachment(attachmentId, this.UserInfo.UserId);

                return Json(new
                {
                    Valid = true
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Delete Attachment").Add("Error Message", ex.Message).ToFailLogString());
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
        [CheckUserRole(ScreenCode.ViewCustomerDocument)]
        public ActionResult InitView(int attachmentId, string documentLevel)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("InitView Attachment").Add("AttachmentId", attachmentId).
                Add("DocumentLevel", documentLevel).ToInputLogString());

            try
            {
                _customerFacade = new CustomerFacade();
                var attachVM = new AttachViewModel();
                AttachmentEntity selectedAttach = _customerFacade.GetAttachmentByID(attachmentId, documentLevel);

                if (selectedAttach != null)
                {
                    attachVM.AttachmentId = selectedAttach.AttachmentId;
                    attachVM.Filename = selectedAttach.Filename;
                    attachVM.DocName = selectedAttach.Name;
                    attachVM.DocDesc = selectedAttach.Description;
                    attachVM.ExpiryDate = selectedAttach.ExpiryDateDisplay;
                    attachVM.CustomerId = selectedAttach.CustomerId;
                    attachVM.Status = selectedAttach.Status;
                    attachVM.CreateUserFullName = selectedAttach.CreateUserFullName;

                    attachVM.AttachTypeDisplay = StringHelpers.ConvertListToString(selectedAttach.AttachTypeList.Select(x => x.Name).ToList<object>(), ", ");
                }

                TempData["FILE_DOWNLOAD"] = selectedAttach;

                return PartialView("~/Views/Document/_ViewAttachment.cshtml", attachVM);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("InitView Attachment").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUserRole(ScreenCode.ViewCustomerDocument)]
        public JsonResult LoadFileAttachment()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Load FileAttachment").ToInputLogString());

            try
            {
                if (TempData["FILE_DOWNLOAD"] != null)
                {
                    AttachmentEntity selectedAttach = (AttachmentEntity)TempData["FILE_DOWNLOAD"];
                    TempData["FILE_DOWNLOAD"] = selectedAttach;

                    _commonFacade = new CommonFacade();
                    string documentFolder = selectedAttach.DocumentLevel == Constants.DocumentLevel.Customer ?
                        _commonFacade.GetCSMDocumentFolder() : _commonFacade.GetSrDocumentFolder();
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
        [CheckUserRole(ScreenCode.ViewCustomerDocument)]
        public ActionResult PreviewAttachment()
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Preview Attachment").ToInputLogString());

            try
            {
                AttachmentEntity selectedAttach = (AttachmentEntity)TempData["FILE_DOWNLOAD"];
                TempData["FILE_DOWNLOAD"] = selectedAttach; // keep object

                _commonFacade = new CommonFacade();
                string documentFolder = selectedAttach.DocumentLevel == Constants.DocumentLevel.Customer ?
                    _commonFacade.GetCSMDocumentFolder() : _commonFacade.GetSrDocumentFolder();
                string pathFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", documentFolder, selectedAttach.Url);
                byte[] byteArray = System.IO.File.ReadAllBytes(pathFile);

                return File(byteArray, selectedAttach.ContentType, selectedAttach.Filename);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Preview Attachment").Add("Error Message", ex.Message).ToFailLogString());
                return Error(new HandleErrorInfo(ex, this.ControllerContext.RouteData.Values["controller"].ToString(),
                    this.ControllerContext.RouteData.Values["action"].ToString()));
            }
        }
    }
}
