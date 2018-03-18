using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
using Newtonsoft.Json;
using System.Globalization;

namespace CSM.Web.Controllers
{
    [CheckUserSession]
    public class AttachmentController : BaseController
    {
        private ICommonFacade _commonFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AttachmentController));

        public ActionResult InitEdit(string jsonData)
        {
            _commonFacade = new CommonFacade();

            #region "For show in hint"

            ParameterEntity param = _commonFacade.GetCacheParamByName(Constants.ParameterName.RegexFileExt);
            ParameterEntity paramSingleFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.SingleFileSize);  
         
            int? limitSingleFileSize = paramSingleFileSize.ParamValue.ToNullable<int>();
            var singleLimitSize = limitSingleFileSize.HasValue ? (limitSingleFileSize / 1048576) : 0;
            ViewBag.UploadLimitType = string.Format(CultureInfo.InvariantCulture, param.ParamDesc, singleLimitSize);

            #endregion

            List<DocumentTypeEntity> docTypeList = null;
            var attachVM = JsonConvert.DeserializeObject<AttachViewModel>(jsonData);

            if (attachVM.ListIndex != null)
            {
                AttachmentEntity selectedAttach = attachVM.AttachmentList[attachVM.ListIndex.Value];
                attachVM.Filename = selectedAttach.Filename;
                attachVM.DocName = selectedAttach.Name;
                attachVM.DocDesc = selectedAttach.Description;
                attachVM.ExpiryDate = selectedAttach.ExpiryDateDisplay;
                docTypeList = _commonFacade.GetDocumentTypeList(selectedAttach.AttachTypeList, Constants.DocumentCategory.Announcement);
            }
            else
            {
                docTypeList = _commonFacade.GetActiveDocumentTypes(Constants.DocumentCategory.Announcement);
            }

            if (docTypeList != null)
            {
                attachVM.DocTypeCheckBoxes = docTypeList.Select(x => new CheckBoxes
                {
                    Value = x.DocTypeId.ToString(),
                    Text = x.Name,
                    Checked = x.IsChecked
                }).ToList();
            }

            return PartialView("~/Views/Attachment/_AttachEdit.cshtml", attachVM);
        }

        public ActionResult Edit(AttachViewModel attachVM)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("List Attachment").Add("DocName", attachVM.DocName)
                .Add("DocDesc", attachVM.DocDesc).ToInputLogString());

            AttachmentEntity attach = attachVM.ListIndex != null ? attachVM.AttachmentList[attachVM.ListIndex.Value] : new AttachmentEntity();
            int? oldFileSize = 0; // เพื่อเช็คขนาดไฟล์รวม

            if (attach.AttachmentId != 0)
            {
                oldFileSize = attach.FileSize; // เพื่อเช็คขนาดไฟล์รวม
                ModelState.Remove("FileAttach");
            }

            if (ModelState.IsValid)
            {
                var file = attachVM.FileAttach;
                attach.Name = attachVM.DocName;
                attach.Description = attachVM.DocDesc;
                attach.ExpiryDate = attachVM.ExpiryDate.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate);

                var selectedAttachType = attachVM.DocTypeCheckBoxes.Where(x => x.Checked == true)
                               .Select(x => new AttachmentTypeEntity
                               {
                                   DocTypeId = x.Value.ToNullable<int>(),
                                   CreateUserId = this.UserInfo.UserId
                               }).ToList();

                if (attach.AttachTypeList != null && attach.AttachTypeList.Count > 0)
                {
                    var prevAttachTypes = (from at in attach.AttachTypeList
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

                // Verify that the user selected a file
                if (file != null && file.ContentLength > 0)
                {
                    _commonFacade = new CommonFacade();
                    ParameterEntity paramSingleFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.SingleFileSize);
                    ParameterEntity paramTotalFileSize = _commonFacade.GetCacheParamByName(Constants.ParameterName.TotalFileSize);
                    int? limitSingleFileSize = paramSingleFileSize.ParamValue.ToNullable<int>();
                    int? limitTotalFileSize = paramTotalFileSize.ParamValue.ToNullable<int>();

                    if (file.ContentLength > limitSingleFileSize.Value)
                    {
                        ModelState.AddModelError("FileAttach", string.Format(CultureInfo.InvariantCulture, Resource.ValError_SingleFileSizeExceedMaxLimit, (limitSingleFileSize.Value / 1048576)));
                        goto Outer;
                    }

                    int? totalSize =  attachVM.AttachmentList.Where(x=> x.IsDelete == false).Sum(x => x.FileSize);
                    totalSize -= oldFileSize; // กรณีแก้ไข จะลบขนาดไฟล์เก่าออก 
                    totalSize += file.ContentLength;

                    if (totalSize > limitTotalFileSize.Value)
                    {
                        ModelState.AddModelError("FileAttach", string.Format(CultureInfo.InvariantCulture, Resource.ValError_TotalFileSizeExceedMaxLimit, (limitTotalFileSize.Value / 1048576)));
                        goto Outer;
                    }

                    // extract only the filename
                    var fileName = Path.GetFileName(file.FileName);
                    //const string regexPattern = @"^.*\.(jpg|jpeg|doc|docx|xls|xlsx|ppt|txt)$";
                    
                    ParameterEntity param = _commonFacade.GetCacheParamByName(Constants.ParameterName.RegexFileExt);
                    Match match = Regex.Match(fileName, param.ParamValue, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        ModelState.AddModelError("FileAttach", Resource.ValError_FileExtension);
                        goto Outer;
                    }

                    string fileExtension = Path.GetExtension(file.FileName);
                    string path;

                    using (var tmp = new TempFile())
                    {
                        path = tmp.Path;
                        Logger.Debug("-- Upload File --:Is File Exists/" + System.IO.File.Exists(path));
                    }

                    // store the file inside ~/App_Data/uploads folder
                    //var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                    file.SaveAs(path);

                    attach.Filename = fileName;
                    attach.ContentType = file.ContentType;
                    attach.TempPath = path;
                    attach.FileExtension = fileExtension;
                    attach.FileSize = file.ContentLength;

                    if (attachVM.ListIndex == null)
                    {
                        attachVM.AttachmentList.Add(attach);
                    }
                }

                return Json(new
                {
                    Valid = true,
                    Data = attachVM.AttachmentList
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
    }
}
