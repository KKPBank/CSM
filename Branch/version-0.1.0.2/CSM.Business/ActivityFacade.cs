using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using CSM.Business.Common;
using CSM.Common.Utilities;
using CSM.Data.DataAccess;
using CSM.Entity;
using CSM.Service.Messages.Activity;
using CSM.Service.Messages.Common;
using log4net;
using CAR = CSM.Service.CARLogService;
using CSM.Service.CBSHPService;

namespace CSM.Business
{
    public class ActivityFacade : BaseFacade, IActivityFacade
    {
        private ICommonFacade _commonFacade;
        private readonly CSMContext _context;
        private IServiceRequestDataAccess _srDataAccess;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ActivityFacade));

        public ActivityFacade()
        {
            _context = new CSMContext();
        }

        public IEnumerable<ActivityDataItem> GetActivityLogList(AuditLogEntity auditLog, ActivitySearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Call CARLogService.InquiryActivityLog").ToInputLogString());

            var resActivity = InquiryActivityLog(auditLog, searchFilter);
            if (resActivity == null || resActivity.ActivityDataItems == null)
                return null;

            // var activities = resActivity.ActivityDataItems.Where(x => x.SubscriptionID == searchFilter.CardNo).ToList();
            var activities = resActivity.ActivityDataItems.ToList();

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = activities.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            var sortResult = SetActivityListSort(activities, searchFilter);

            return sortResult.Skip(startPageIndex).Take(searchFilter.PageSize);
        }

        private static IEnumerable<ActivityDataItem> SetActivityListSort(List<ActivityDataItem> list, ActivitySearchFilter searchFilter)
        {
            var sortField = (searchFilter != null && !string.IsNullOrEmpty(searchFilter.SortField)) ? searchFilter.SortField : string.Empty;
            var sortOrder = (searchFilter != null && !string.IsNullOrEmpty(searchFilter.SortOrder)) ? searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture) : "ASC";

            if (sortOrder.Equals("ASC"))
            {
                switch (sortField)
                {
                    case "SrId":
                        return list.OrderBy(a => a.SrID);
                    case "Product":
                        return list.OrderBy(a => a.ProductName);
                    case "Type":
                        return list.OrderBy(a => a.TypeName);
                    case "Area":
                        return list.OrderBy(a => a.AreaName);
                    case "SubArea":
                        return list.OrderBy(a => a.SubAreaName);
                    case "SrStatus":
                        return list.OrderBy(a => a.Status);
                    case "CreateDate":
                        return list.OrderBy(a => a.ActivityDateTime);
                    case "ContactName":
                        return list.OrderBy(a => a.ContractInfo.FullName);
                    default:
                        return list.OrderByDescending(a => a.SrID).ThenByDescending(x => x.ActivityDateTime);
                }
            }
            else
            {
                switch (sortField)
                {
                    case "SrId":
                        return list.OrderByDescending(a => a.SrID);
                    case "Product":
                        return list.OrderByDescending(a => a.ProductName);
                    case "Type":
                        return list.OrderByDescending(a => a.TypeName);
                    case "Area":
                        return list.OrderByDescending(a => a.AreaName);
                    case "SubArea":
                        return list.OrderByDescending(a => a.SubAreaName);
                    case "SrStatus":
                        return list.OrderByDescending(a => a.Status);
                    case "CreateDate":
                        return list.OrderByDescending(a => a.ActivityDateTime);
                    case "ContactName":
                        return list.OrderByDescending(a => a.ContractInfo.FullName);
                    default:
                        return list.OrderByDescending(a => a.SrID);
                }
            }
        }

        public ServiceRequestSaveCarResult InsertActivityLogToCAR(AuditLogEntity auditLog, ServiceRequestForInsertLogEntity entity)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Call CARLogService.CreateActivityLog").ToInputLogString());

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Logger.Debug("I:--START--:--CARLogService.CreateActivityLog--");

            ServiceRequestSaveCarResult result = new ServiceRequestSaveCarResult();

            try
            {
                Header profile = GetHeaderByServiceName<Header>(Constants.ServiceName.CreateActivityLog);

                var header = new CAR.LogServiceHeader
                {
                    ReferenceNo = entity.SrActivityId.ConvertToString(),
                    ServiceName = Constants.ServiceName.CreateActivityLog,
                    SystemCode = profile.system_code,
                    SecurityKey = profile.password,
                    TransactionDateTime = DateTime.Now
                };

                var accountNo = entity.AccountNo;
                if (!string.IsNullOrEmpty(entity.AccountNo) && entity.AccountNo.ToUpper() == "DUMMY")
                {
                    accountNo = string.Format("DUMMY-{0}", entity.CustomerId);
                }

                #region "Customer Info"

                var customerInfoList = new List<CAR.DataItem>();
                customerInfoList.Add(new CAR.DataItem { SeqNo = 1, DataLabel = "Subscription Type", DataValue = entity.CustomerSubscriptionTypeName });
                customerInfoList.Add(new CAR.DataItem { SeqNo = 2, DataLabel = "Subscription ID", DataValue = entity.CustomerCardNo });
                customerInfoList.Add(new CAR.DataItem { SeqNo = 3, DataLabel = "วันเกิด", DataValue = entity.CustomerBirthDateDisplay });
                customerInfoList.Add(new CAR.DataItem { SeqNo = 4, DataLabel = "คำนำหน้า", DataValue = entity.CustomerTitleTh });
                customerInfoList.Add(new CAR.DataItem { SeqNo = 5, DataLabel = "ชื่อลูกค้า", DataValue = entity.CustomerFirstNameTh });
                customerInfoList.Add(new CAR.DataItem { SeqNo = 6, DataLabel = "นามสกุลลูกค้า", DataValue = entity.CustomerLastNameTh });
                customerInfoList.Add(new CAR.DataItem { SeqNo = 7, DataLabel = "Title", DataValue = entity.CustomerTitleEn });
                customerInfoList.Add(new CAR.DataItem { SeqNo = 8, DataLabel = "First Name", DataValue = entity.CustomerFirstNameEn });
                customerInfoList.Add(new CAR.DataItem { SeqNo = 9, DataLabel = "Last Name", DataValue = entity.CustomerLastNameEn });
                customerInfoList.Add(new CAR.DataItem { SeqNo = 10, DataLabel = "เบอร์โทรศัพท์ #1", DataValue = entity.CustomerPhoneNo1 });
                customerInfoList.Add(new CAR.DataItem { SeqNo = 11, DataLabel = "เบอร์โทรศัพท์ #2", DataValue = entity.CustomerPhoneNo2 });
                customerInfoList.Add(new CAR.DataItem { SeqNo = 12, DataLabel = "เบอร์โทรศัพท์ #3", DataValue = entity.CustomerPhoneNo3 });
                customerInfoList.Add(new CAR.DataItem { SeqNo = 13, DataLabel = "เบอร์แฟกซ์", DataValue = entity.CustomerFax });
                customerInfoList.Add(new CAR.DataItem { SeqNo = 14, DataLabel = "อีเมล์", DataValue = entity.CustomerEmail });
                customerInfoList.Add(new CAR.DataItem { SeqNo = 15, DataLabel = "รหัสพนักงาน", DataValue = entity.CustomerEmployeeCode });
                customerInfoList.Add(new CAR.DataItem { SeqNo = 16, DataLabel = "A Number.", DataValue = entity.ANo });
                customerInfoList.Add(new CAR.DataItem { SeqNo = 17, DataLabel = "Call ID.", DataValue = entity.CallId });

                #endregion

                #region "Contract Info"

                var contractInfoList = new List<CAR.DataItem>();
                contractInfoList.Add(new CAR.DataItem { SeqNo = 1, DataLabel = "เลขที่บัญชี/สัญญา", DataValue = accountNo });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 2, DataLabel = "สถานะบัญชี", DataValue = entity.AccountStatus });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 3, DataLabel = "ทะเบียนรถ", DataValue = entity.AccountCarNo });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 4, DataLabel = "Product Group", DataValue = entity.AccountProductGroup });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 5, DataLabel = "Product", DataValue = entity.AccountProduct });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 6, DataLabel = "ชื่อสาขา", DataValue = entity.AccountBranchName });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 7, DataLabel = "Subscription Type", DataValue = entity.ContactSubscriptionTypeName });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 8, DataLabel = "Subscription ID", DataValue = entity.ContactCardNo });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 9, DataLabel = "วันเกิด", DataValue = entity.ContactBirthDateDisplay });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 10, DataLabel = "คำนำหน้า", DataValue = entity.ContactTitleTh });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 11, DataLabel = "ชื่อลูกค้า", DataValue = entity.ContactFirstNameTh });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 12, DataLabel = "นามสกุลลูกค้า", DataValue = entity.ContactLastNameTh });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 13, DataLabel = "Title", DataValue = entity.ContactTitleEn });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 14, DataLabel = "First Name", DataValue = entity.ContactFirstNameEn });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 15, DataLabel = "Last Name", DataValue = entity.ContactLastNameEn });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 16, DataLabel = "เบอร์โทรศัพท์ #1", DataValue = entity.ContactPhoneNo1 });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 17, DataLabel = "เบอร์โทรศัพท์ #2", DataValue = entity.ContactPhoneNo2 });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 18, DataLabel = "เบอร์โทรศัพท์ #3", DataValue = entity.ContactPhoneNo3 });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 19, DataLabel = "เบอร์แฟกซ์", DataValue = entity.ContactFax });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 20, DataLabel = "อีเมล์", DataValue = entity.ContactEmail });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 21, DataLabel = "เลขที่สัญญาที่เกี่ยวข้องกับผู้ติดต่อ", DataValue = entity.ContactAccountNo });
                contractInfoList.Add(new CAR.DataItem { SeqNo = 22, DataLabel = "ความสัมพันธ์", DataValue = entity.ContactRelationshipName });

                #endregion

                #region "Product Info

                var productInfoList = new List<CAR.DataItem>();
                productInfoList.Add(new CAR.DataItem { SeqNo = 1, DataLabel = "Product Group", DataValue = entity.ProductGroupName });
                productInfoList.Add(new CAR.DataItem { SeqNo = 2, DataLabel = "Product", DataValue = entity.ProductName });
                productInfoList.Add(new CAR.DataItem { SeqNo = 3, DataLabel = "Campaign/Service", DataValue = entity.CampaignServiceName });

                #endregion

                #region "Office Info"

                var officeInfoList = new List<CAR.DataItem>();
                officeInfoList.Add(new CAR.DataItem { SeqNo = 1, DataLabel = "Officer", DataValue = entity.ActivityCreateUserFullName });

                #endregion

                #region "Activity Info"

                var activityInfoList = new List<CAR.DataItem>();

                activityInfoList.Add(new CAR.DataItem { SeqNo = 1, DataLabel = "Area", DataValue = entity.AreaName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 2, DataLabel = "Sub Area", DataValue = entity.SubAreaName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 3, DataLabel = "Type", DataValue = entity.TypeName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 4, DataLabel = "SR Channel", DataValue = entity.ChannelCode });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 5, DataLabel = "Media Source", DataValue = entity.MediaSourceName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 6, DataLabel = "Subject", DataValue = entity.Subject });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 7, DataLabel = "Remark", DataValue = entity.Remark });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 8, DataLabel = "Verify", DataValue = entity.Verify });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 9, DataLabel = "Verify Result", DataValue = entity.VerifyResult });

                activityInfoList.Add(new CAR.DataItem { SeqNo = 10, DataLabel = "Creator Branch", DataValue = entity.SRCreatorBranchName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 11, DataLabel = "Creator SR", DataValue = entity.SRCreatorUserFullName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 12, DataLabel = "Owner Branch", DataValue = entity.OwnerBranchName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 13, DataLabel = "Owner SR", DataValue = entity.OwnerUserFullName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 14, DataLabel = "Delegate Branch", DataValue = entity.DelegateBranchName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 15, DataLabel = "Delegate SR", DataValue = entity.DelegateUserFullName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 16, DataLabel = "Send E-Mail", DataValue = entity.SendEmail });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 17, DataLabel = "E-Mail To", DataValue = entity.EmailTo });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 18, DataLabel = "E-Mail Cc", DataValue = entity.EmailCc });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 19, DataLabel = "E-Mail Subject", DataValue = entity.EmailSubject });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 20, DataLabel = "E-Mail Body", DataValue = entity.EmailBody });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 21, DataLabel = "E-Mail Attachments", DataValue = entity.EmailAttachments });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 22, DataLabel = "รายละเอียดการติดต่อ", DataValue = entity.ActivityDescription });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 23, DataLabel = "Activity Type", DataValue = entity.ActivityTypeName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = 24, DataLabel = "SR Status", DataValue = entity.SRStatusName });

                if (entity.SrPageId == Constants.SRPage.DefaultPageId)
                {
                    activityInfoList.Add(new CAR.DataItem { SeqNo = 25, DataLabel = "ที่อยู่ในการจัดส่งเอกสาร", DataValue = entity.AddressForDisplay });
                }
                else if (entity.SrPageId == Constants.SRPage.AFSPageId)
                {
                    activityInfoList.Add(new CAR.DataItem { SeqNo = 25, DataLabel = "รหัสสินทรัพย์รอขาย", DataValue = entity.AFSAssetNo });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = 26, DataLabel = "รายละเอียดทรัพย์", DataValue = entity.AFSAssetDesc });
                }
                else if (entity.SrPageId == Constants.SRPage.NCBPageId)
                {
                    activityInfoList.Add(new CAR.DataItem { SeqNo = 25, DataLabel = "วันเกิด/วันที่จดทะเบียน (พ.ศ.)", DataValue = entity.NCBCustomerBirthDateDisplay });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = 26, DataLabel = "Marketing Branch Upper #1", DataValue = entity.NCBMarketingBranchUpper1Name });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = 27, DataLabel = "Marketing Branch Upper #2", DataValue = entity.NCBMarketingBranchUpper2Name });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = 28, DataLabel = "Marketing Branch", DataValue = entity.NCBMarketingBranchName });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = 29, DataLabel = "Marketing", DataValue = entity.NCBMarketingFullName });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = 30, DataLabel = "NCB Check Status", DataValue = entity.NCBCheckStatus });
                }

                #endregion

                #region "ActivitiyLog Data"

                int customerSubscriptionTypeCode;

                try
                {
                    customerSubscriptionTypeCode = Convert.ToInt32(entity.CustomerSubscriptionTypeCode, CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    customerSubscriptionTypeCode = 0;
                    Logger.Error("Exception occur:\n", ex);
                }

                var customerCardNo = entity.CustomerCardNo;

                if (string.IsNullOrEmpty(customerCardNo) || customerSubscriptionTypeCode == 0)
                {
                    customerCardNo = null;
                    customerSubscriptionTypeCode = 0;
                }

                var activitiyLogData = new CAR.CreateActivityLogData
                {
                    ChannelID = profile.channel_id,
                    ActivityDateTime = entity.ActivityCreateDate.Value,
                    SubscriptionTypeID = customerSubscriptionTypeCode,
                    SubscriptionID = customerCardNo,
                    LeadID = null,
                    TicketID = null,
                    SrID = entity.SrNo,
                    ContractID = accountNo,
                    ProductGroupID = entity.ProductGroupCode,
                    ProductID = entity.ProductCode,
                    CampaignID = entity.CampaignServiceCode,
                    ActivityTypeID = entity.ActivityTypeId,
                    ActivityTypeName = entity.ActivityTypeName,
                    TypeID = entity.TypeCode,
                    AreaID = entity.AreaCode,
                    SubAreaID = entity.SubAreaCode,
                    TypeName = entity.TypeName,
                    AreaName = entity.AreaName,
                    SubAreaName = entity.SubAreaName,
                    Status = entity.SRStatusName,
                    KKCISID = entity.KKCISID,
                    NoncustomerID = entity.CustomerId.ConvertToString(),
                    OfficerInfoList = officeInfoList.ToArray(),
                    ProductInfoList = productInfoList.ToArray(),
                    CustomerInfoList = customerInfoList.ToArray(),
                    ActivityInfoList = activityInfoList.ToArray(),
                    ContractInfoList = contractInfoList.ToArray(),
                };

                #endregion

                CAR.CreateActivityLogResponse resActivity = null;

                #region "Call Service"

                string flgCatchErrorCode = string.Empty;

                try
                {
                    flgCatchErrorCode = string.Empty;
                    using (var client = new CAR.CASLogServiceSoapClient("CASLogServiceSoap"))
                    {
                        resActivity = ((CAR.CASLogServiceSoapClient)client).CreateActivityLog(header, activitiyLogData);
                        if (client != null)
                        {
                            ((ICommunicationObject)client).Abort();
                        }
                    }
                }
                catch (AggregateException aex)
                {
                    aex.Handle((x) =>
                    {
                        if (x is EndpointNotFoundException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0002;
                            Logger.Error("EndpointNotFoundException occur:\n", x);
                            return true;
                        }
                        else if (x is CommunicationException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0002;
                            Logger.Error("CommunicationException occur:\n", x);
                            return true;
                        }
                        else if (x is TimeoutException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0001;
                            Logger.Error("TimeoutException occur:\n", x);
                            return true;
                        }
                        else
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0003;
                            Logger.Error("Exception occur:\n", x);
                            return true;
                        }
                    });

                    result.IsSuccess = false;
                    result.ErrorCode = flgCatchErrorCode;
                    result.ErrorMessage = aex.Message;
                    return result;
                }

                if (!string.IsNullOrEmpty(flgCatchErrorCode))
                {
                    AppLog.AuditLog(auditLog, LogStatus.Fail, GetMessageResource(flgCatchErrorCode, true));
                    throw new CustomException(GetMessageResource(flgCatchErrorCode, false));
                }

                #endregion

                if (resActivity != null)
                {
                    Logger.DebugFormat("-- XMLResponse --\n{0}", resActivity.SerializeObject());

                    if (Constants.ActivityResponse.Success.Equals(resActivity.ResponseStatus.ResponseCode))
                    {
                        AppLog.AuditLog(auditLog, LogStatus.Success, string.Empty);

                        result.IsSuccess = true;
                        result.ErrorCode = "";
                        result.ErrorMessage = "";
                        return result;
                    }
                    else
                    {
                        // Log DB
                        AppLog.AuditLog(auditLog, LogStatus.Fail, GetMessageResource(Constants.SystemName.CAR, Constants.ServiceName.CreateActivityLog,
                            resActivity.ResponseStatus.ResponseCode, true));

                        result.IsSuccess = false;
                        result.ErrorCode = resActivity.ResponseStatus.ResponseCode;
                        result.ErrorMessage = resActivity.ResponseStatus.ResponseMessage;
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                result.IsSuccess = false;
                result.ErrorCode = "1";
                result.ErrorMessage = ex.Message;
                return result;
            }
            finally
            {
                stopwatch.Stop();
                Logger.DebugFormat("O:--Finish--:ElapsedMilliseconds/{0}", stopwatch.ElapsedMilliseconds);
            }

            return null;
        }

        public ServiceRequestSaveCbsHpResult InsertActivityLogToLog100(AuditLogEntity auditLog, ServiceRequestForInsertLogEntity sr)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Call CARLogService.CreateActivityLog").ToInputLogString());

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Logger.Debug("I:--START--:--CARLogService.CreateActivityLog--");

            var result = new ServiceRequestSaveCbsHpResult();

            try
            {
                var request = new KKB_EAI_ServiceRequest();

                // Required Field
                request.FinancialAccountNumber = sr.AccountNo;
                request.SRType = sr.HPLanguageIndependentCode;
                request.Area = sr.HPSubject;
                request.SRNumber = sr.SrNo;
                request.Created = string.Format(new CultureInfo("th-TH"), "{0:yyyyMMdd HH:mm:ss}", sr.ActivityCreateDate);
                request.CreatedByName = sr.ActivityCreateUserEmployeeCode;
                request.Description = sr.GetHpDescription();

                // Not Show to HP
                request.OwnedById = "";
                request.CustomerNumber = "";
                request.SubArea = "";
                request.Abstract = null;
                request.Status = "3";
                request.SubStatus = null;
                request.ClosedDate = "";
                request.ContactFullName = "";
                request.MainPhoneNumber = null;
                request.Owner = "";
                request.Priority = "3-Medium";

                try
                {
                    string response;
                    using (var client = new ServiceSoapClient())
                    {
                        response = ((ServiceSoap)client).SR_Insert(request);
                        if (client != null)
                        {
                            ((ICommunicationObject)client).Abort();
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(response) && "SUCCESS".Equals(response.ToUpper(CultureInfo.InvariantCulture)))
                    {
                        result.IsSuccess = true;
                        result.ErrorCode = "";
                        result.ErrorMessage = "";
                        return result;
                    }
                    else
                    {
                        Logger.ErrorFormat("Call Service Error (ResponseMessage = {0})", response);
                        result.IsSuccess = false;
                        result.ErrorCode = "1";
                        result.ErrorMessage = response;
                        return result;
                    }
                }
                catch (AggregateException aex)
                {
                    var flgCatchErrorCode = "";
                    aex.Handle((x) =>
                    {
                        if (x is EndpointNotFoundException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0002;
                            Logger.Error("EndpointNotFoundException occur:\n", x);
                            return true;
                        }
                        else if (x is CommunicationException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0002;
                            Logger.Error("CommunicationException occur:\n", x);
                            return true;
                        }
                        else if (x is TimeoutException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0001;
                            Logger.Error("TimeoutException occur:\n", x);
                            return true;
                        }
                        else
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0003;
                            Logger.Error("Exception occur:\n", x);
                            return true;
                        }
                    });

                    result.IsSuccess = false;
                    result.ErrorCode = flgCatchErrorCode;
                    result.ErrorMessage = aex.Message;
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                result.IsSuccess = false;
                result.ErrorCode = "1";
                result.ErrorMessage = ex.Message;
                return result;
            }
            finally
            {
                stopwatch.Stop();
                Logger.DebugFormat("O:--Finish--:ElapsedMilliseconds/{0}", stopwatch.ElapsedMilliseconds);
            }
        }

        public IEnumerable<ActivityDataItem> GetSRActivityList(ActivitySearchFilter searchFilter)
        {
            _srDataAccess = new ServiceRequestDataAccess(_context);
            var activities = _srDataAccess.GetSRActivityList(searchFilter);

            if (activities.Any())
            {
                var results = activities.Select(x => new ActivityDataItem
                {
                    AreaName = x.AreaName,
                    ProductName = x.ProductName,
                    TypeName = x.TypeName,
                    SubAreaName = x.SubAreaName,
                    Status = x.SRStatusName,
                    ActivityDateTime = x.Date,
                    SrID = x.SrNo,
                    ChannelName = x.ChannelName,
                    CustomerInfo = new CustomerInfo
                    {
                        SubscriptionID = x.CustomerCardNo,
                    },
                    ContractInfo = new ContractInfo
                    {
                        AccountNo = x.AccountNo,
                        CreateSystem = Constants.SystemName.CSM,
                        FullName = x.ContactFullName
                    },
                    ActivityInfo = new ActivityInfo
                    {
                        CreatorBranch = x.CreatorBranchName,
                        CreatorSR = x.CreatorUserFullName,
                        OwnerBranch = x.OwnerBranchName,
                        OwnerSR = x.OwnerUserFullName,
                        DelegateBranch = x.DelegateBranchName,
                        DelegateSR = x.DelegateUserFullName,
                        SendEmail = x.IsSendEmail ? "Y" : "N",
                        EmailTo = x.EmailTo,
                        EmailCc = x.EmailCc,
                        EmailSubject = x.EmailSubject,
                        EmailBody = x.EmailBody,
                        EmailAttachments = x.EmailAttachments,
                        ActivityDescription = x.ActivityDesc,
                        SRStatus = x.SRStatusName,
                    },
                    OfficerInfo = new OfficerInfo
                    {
                        FullName = x.OfficerUserFullName
                    }
                });

                return results.Any() ? results.ToList() : null;
            }

            return null;
        }

        private InquiryActivityLogResponse InquiryActivityLog(AuditLogEntity auditLog, ActivitySearchFilter searchFilter)
        {
            CAR.CASLogServiceSoapClient client = null;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Logger.Debug("I:--START--:--CARLogService.InquiryActivityLog--");

            try
            {
                Header profile = GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryActivityLog);

                var header = new CAR.LogServiceHeader
                {
                    ReferenceNo = profile.reference_no,
                    ServiceName = profile.service_name,
                    SystemCode = profile.system_code,
                    SecurityKey = profile.password,
                    TransactionDateTime = DateTime.Now,
                };

                decimal subsTypeCodeAsDecimal;
                try
                {
                    subsTypeCodeAsDecimal = Convert.ToDecimal(searchFilter.SubsTypeCode, CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    subsTypeCodeAsDecimal = 0;
                    Logger.Error("Exception occur:\n", ex);
                }

                if (subsTypeCodeAsDecimal == 0 || string.IsNullOrEmpty(searchFilter.CardNo))
                {
                    subsTypeCodeAsDecimal = 0;
                    searchFilter.CardNo = null;
                }

                var activitiyLogData = new CAR.InqueryActivityLogData
                {
                    ActivityStartDateTime = searchFilter.ActivityStartDateTimeValue, // "2015-01-01".ParseDateTime("yyyy-MM-dd").Value
                    ActivityEndDateTime = searchFilter.ActivityEndDateTimeValue,     // "2016-06-07".ParseDateTime("yyyy-MM-dd").Value
                    ChannelID = null,
                    SubscriptionTypeID = searchFilter.SrOnly ? 0 : subsTypeCodeAsDecimal,  // 1,
                    SubscriptionID = searchFilter.SrOnly ? null : searchFilter.CardNo,              // "3601000025739",
                    SrID = searchFilter.SrOnly ? searchFilter.SrNo : null,                          // "162817263527",
                    CampaignID = string.Empty,
                    TypeID = 0,
                    AreaID = 0,
                    SubAreaID = 0
                };

                CAR.InqueryActivytyLogResponse resActivity = null;

                #region "Call Service"

                string flgCatchErrorCode = string.Empty;

                // Avoid error codes
                _commonFacade = new CommonFacade();
                List<string> exceptionErrorCodes = _commonFacade.GetExceptionErrorCodes(Constants.SystemName.CAR, Constants.ServiceName.InquiryActivityLog);

                try
                {
                    Retry.Do(() =>
                    {
                        flgCatchErrorCode = string.Empty;
                        Logger.DebugFormat("-- XMLRequest --\n{0}\n{1}", header.SerializeObject(), activitiyLogData.SerializeObject());
                        client = new CAR.CASLogServiceSoapClient("CASLogServiceSoap");
                        resActivity = client.InquiryActivityLog(header, activitiyLogData);
                        if (client != null)
                        {
                            ((ICommunicationObject)client).Abort();
                        }

                    }, TimeSpan.FromSeconds(WebConfig.GetServiceRetryInterval()), WebConfig.GetServiceRetryNo());
                }
                catch (AggregateException aex)
                {
                    aex.Handle((x) =>
                    {
                        if (x is EndpointNotFoundException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0002;
                            Logger.Error("EndpointNotFoundException occur:\n", x);
                            return true;
                        }
                        else if (x is CommunicationException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0002;
                            Logger.Error("CommunicationException occur:\n", x);
                            return true;
                        }
                        else if (x is TimeoutException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0001;
                            Logger.Error("TimeoutException occur:\n", x);
                            return true;
                        }
                        else
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0003;
                            Logger.Error("Exception occur:\n", x);
                            return true;
                        }
                    });
                }

                if (!string.IsNullOrEmpty(flgCatchErrorCode))
                {
                    AppLog.AuditLog(auditLog, LogStatus.Fail, GetMessageResource(flgCatchErrorCode, true));
                    throw new CustomException(GetMessageResource(flgCatchErrorCode, false));
                }

                #endregion

                if (resActivity != null)
                {
                    Logger.DebugFormat("-- XMLResponse --\n{0}", resActivity.SerializeObject());

                    InquiryActivityLogResponse response = new InquiryActivityLogResponse();
                    response.StatusResponse.ErrorCode = resActivity.ResponseStatus.ResponseCode;
                    response.StatusResponse.Description = resActivity.ResponseStatus.ResponseMessage;

                    if (exceptionErrorCodes != null && exceptionErrorCodes.Contains(resActivity.ResponseStatus.ResponseCode))
                    {
                        response.StatusResponse.ErrorCode = Constants.ActivityResponse.Success;
                    }

                    if (Constants.ActivityResponse.Success.Equals(response.StatusResponse.ErrorCode))
                    {
                        if (resActivity.InquiryActivityDataList != null)
                        {
                            response.ActivityDataItems = resActivity.InquiryActivityDataList
                                .Select(ac => new ActivityDataItem
                                {
                                    ActivityDateTime = ac.ActivityDateTime,
                                    ActivityID = ac.ActivityID,
                                    ActivityTypeID = ac.ActivityTypeID,
                                    ActivityTypeName = ac.ActivityTypeName,
                                    AreaID = ac.AreaID,
                                    AreaName = ac.AreaName,
                                    CISID = ac.CISID,
                                    CampaignID = ac.CampaignID,
                                    CampaignName = ac.CampaignName,
                                    ChannelID = ac.ChannelID,
                                    ChannelName = ac.ChannelName,
                                    ContractID = ac.ContractID,
                                    TypeID = ac.TypeID,
                                    TypeName = ac.TypeName,
                                    TicketID = ac.TicketID,
                                    ProductID = ac.ProductID,
                                    ProductName = ac.ProductName,
                                    ProductGroupID = ac.ProductGroupID,
                                    ProductGroupName = ac.ProductGroupName,
                                    SrID = ac.SrID,
                                    SubAreaID = ac.SubAreaID,
                                    SubAreaName = ac.SubAreaName,
                                    Status = ac.Status,
                                    SubStatus = ac.SubStatus,
                                    SubscriptionTypeName = ac.SubscriptionTypeName,
                                    SubscriptionID = ac.SubscriptonID,
                                    CustomerInfo = new CustomerInfo
                                    {
                                        SubscriptionType = GetDataItemValue(ac.CustomerInfoList.FirstOrDefault(x => x.SeqNo == 1)),
                                        SubscriptionID = GetDataItemValue(ac.CustomerInfoList.FirstOrDefault(x => x.SeqNo == 2)),
                                        FullName = GetDataItemValue(ac.CustomerInfoList.FirstOrDefault(x => x.SeqNo == 3))
                                    },
                                    ContractInfo = new ContractInfo
                                    {
                                        AccountNo = GetDataItemValue(ac.ContractInfoList.FirstOrDefault(x => x.SeqNo == 1)),
                                        CreateSystem = GetDataItemValue(ac.ContractInfoList.FirstOrDefault(x => x.SeqNo == 2)),
                                        RegistrationNo = GetDataItemValue(ac.ContractInfoList.FirstOrDefault(x => x.SeqNo == 3)),
                                        FullName = GetDataItemValue(ac.ContractInfoList.FirstOrDefault(x => x.SeqNo == 11)) + " " + GetDataItemValue(ac.ContractInfoList.FirstOrDefault(x => x.SeqNo == 12)),
                                    },
                                    ActivityInfo = new ActivityInfo
                                    {
                                        CreatorBranch = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 10)),
                                        CreatorSR = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 11)),
                                        OwnerBranch = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 12)),
                                        OwnerSR = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 13)),
                                        DelegateBranch = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 14)),
                                        DelegateSR = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 15)),
                                        SendEmail = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 16)),
                                        EmailTo = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 17)),
                                        EmailCc = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 18)),
                                        EmailSubject = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 19)),
                                        EmailBody = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 20)),
                                        EmailAttachments = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 21)),
                                        ActivityDescription = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 22)),
                                        ActivityType = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 23)),
                                        SRStatus = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 24)),
                                    },
                                    OfficerInfo = new OfficerInfo
                                    {
                                        FullName = GetDataItemValue(ac.OfficerInfoList.FirstOrDefault(x => x.SeqNo == 1))
                                    },
                                    ProductInfo = new ProductInfo
                                    {
                                        ProductGroup = GetDataItemValue(ac.ProductInfoList.FirstOrDefault(x => x.SeqNo == 1)),
                                        Product = GetDataItemValue(ac.ProductInfoList.FirstOrDefault(x => x.SeqNo == 2)),
                                        Campaign = GetDataItemValue(ac.ProductInfoList.FirstOrDefault(x => x.SeqNo == 3))
                                    }
                                })
                                .OrderByDescending(x => x.SrID)
                                .ThenByDescending(x => x.ActivityDateTime)
                                .ToList();
                        }

                        AppLog.AuditLog(auditLog, LogStatus.Success, string.Empty);
                        return response;
                    }

                    // Log DB
                    AppLog.AuditLog(auditLog, LogStatus.Fail, GetMessageResource(Constants.SystemName.CAR, Constants.ServiceName.InquiryActivityLog,
                        response.StatusResponse.ErrorCode, true));
                    throw new CustomException(GetMessageResource(Constants.SystemName.CAR, Constants.ServiceName.InquiryActivityLog,
                        response.StatusResponse.ErrorCode, false));
                }
            }
            finally
            {
                stopwatch.Stop();
                Logger.DebugFormat("O:--Finish--:ElapsedMilliseconds/{0}", stopwatch.ElapsedMilliseconds);
            }

            return null;
        }

        #region "Functions"

        private static string GetDataItemValue(CAR.DataItem item)
        {
            if (item == null)
                return string.Empty;

            return item.DataValue;
        }

        private dynamic GetHeaderByServiceName<T>(string serviceName)
        {
            _commonFacade = new CommonFacade();
            return _commonFacade.GetHeaderByServiceName<T>(serviceName);
        }

        #endregion

        #region "IDisposable"

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (_context != null) { _context.Dispose(); }
                    if (_commonFacade != null) { _commonFacade.Dispose(); }
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
