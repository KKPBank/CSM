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
using DataLabel = CSM.Common.Utilities.Constants.CAR_DataItemLableEntity;

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
            //if (resActivity == null || resActivity.ActivityDataItems == null)
            //    return null;

            List<ActivityDataItem> activities = null;
            List<ActivityDataItem> activities2 = null;
            if (resActivity != null && resActivity.ActivityDataItems != null)
            {
                activities = resActivity.ActivityDataItems.ToList();
                searchFilter.IsConnect = 1;
            }
            else { searchFilter.IsConnect = 0; }

            Logger.Info(_logMsg.Clear().SetPrefixMsg("Merge Activity CAR & CSM DB").ToSuccessLogString());
            activities2 = getLocalActivity(searchFilter);
            if (activities != null)
            { activities.AddRange(activities2.Where(x => (x.IsNotSendCAR ?? false) || x.SendCARStatus != 1)); }
            else
            {
                activities = new List<ActivityDataItem>();
                activities.AddRange(activities2);
            }

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

        private List<ActivityDataItem> getLocalActivity(ActivitySearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Get Local Activity").ToSuccessLogString());
            List<ActivityDataItem> activities = null;
            var srData = new ServiceRequestDataAccess(_context);
            var actCSM = srData.GetSrActivityQueryable(searchFilter.SrOnly, srData.GetServiceRequestNoDetail(searchFilter.SrNo)?.SrId, searchFilter.CustomerId).ToList();
            //var actCSM = srData.GetTabActivityList(new ActivityTabSearchFilter()
            //{
            //    SrOnly = searchFilter.SrOnly,
            //    SrId = srData.GetServiceRequestNoDetail(searchFilter.SrNo)?.SrId,
            //    CustomerId = searchFilter.CustomerId,
            //    SrNo = searchFilter.SrNo,
            //    PageNo = searchFilter.PageNo,
            //    PageSize = _commonFacade.GetPageSizeStart(),
            //    SortField = "ActivityID",
            //    SortOrder = "DESC"
            //}).ToList();

            activities = actCSM.Select(x => new ActivityDataItem()
                            {
                                ActivityID = x.SrActivityId,
                                SubscriptionID = x.CustomerCardNo,
                                ContractID = x.AccountNo,
                                SrID = x.SrNo,
                                ProductName = x.ProductName,
                                TypeName = x.TypeName,
                                AreaName = x.AreaName,
                                SubAreaName = x.SubAreaName,
                                Status = x.StatusDesc,
                                ActivityDateTime = x.Date,
                                ContractInfo = new ContractInfo() { FullName = x.ContactFullName },
                                OfficerInfo = new OfficerInfo() { FullName = x.OfficerUserFullName },
                                ActivityInfo = new ActivityInfo()
                                {
                                    ActivityDescription = x.ActivityDesc,
                                    CreatorBranch = x.CreatorBranchName,
                                    CreatorSR = x.CreatorUserFullName,
                                    OwnerBranch = x.OwnerBranchName,
                                    OwnerSR = x.OwnerUserFullName,
                                    DelegateBranch = x.DelegateBranchName,
                                    DelegateSR = x.DelegateUserFullName,
                                    SendEmail = x.IsSendEmail ? "Y" : "N",
                                    EmailTo = x.EmailTo,
                                    EmailBcc = x.EmailBcc,
                                    EmailCc = x.EmailCc,
                                    EmailSubject = x.EmailSubject,
                                    EmailBody = x.EmailBody,
                                    EmailAttachments = x.EmailAttachments,
                                    ActivityType = x.ActivityTypeName,
                                    SRState = x.SRStateName,
                                    SRStatus = x.SRStatusName
                                },
                                Is_Secret = x.Is_Secret,
                                IsNotSendCAR = x.IsNotSendCAR,
                                SendCARStatus = x.SendCARStatus
                            }).ToList();
            return activities;
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
                if (!string.IsNullOrEmpty(entity.AccountNo) && entity.AccountNo.ToUpperInvariant().InList("DUMMY", "NA"))
                {
                    accountNo = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", entity.AccountNo.ToUpperInvariant(), entity.CustomerId);
                }

                int seqNo = 0;

                #region "Customer Info"

                var customerInfoList = new List<CAR.DataItem>();
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.SubType, DataValue = entity.CustomerSubscriptionTypeName });
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.SubId, DataValue = entity.CustomerCardNo });
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.BirthDay, DataValue = entity.CustomerBirthDateDisplay });
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.Title, DataValue = entity.CustomerTitleTh });
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.FirstName, DataValue = entity.CustomerFirstNameTh });
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.LastName, DataValue = entity.CustomerLastNameTh });
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.TitleEn, DataValue = entity.CustomerTitleEn });
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.FirstNameEn, DataValue = entity.CustomerFirstNameEn });
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.LastNameEn, DataValue = entity.CustomerLastNameEn });
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.PhoneNo1, DataValue = entity.CustomerPhoneNo1 });
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.PhoneNo2, DataValue = entity.CustomerPhoneNo2 });
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.PhoneNo3, DataValue = entity.CustomerPhoneNo3 });
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.Fax, DataValue = entity.CustomerFax });
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.Email, DataValue = entity.CustomerEmail });
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.EmployeeCode, DataValue = entity.CustomerEmployeeCode });
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.ANo, DataValue = entity.ANo });
                customerInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.CustomerInfo.CallId, DataValue = entity.CallId });

                #endregion

                #region "Contract Info"

                seqNo = 0;
                var contractInfoList = new List<CAR.DataItem>();
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.AccountNo, DataValue = accountNo });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.AccountStatus, DataValue = entity.AccountStatus });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.AccountCarNo, DataValue = entity.AccountCarNo });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.AccountProductGroup, DataValue = entity.AccountProductGroup });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.AccountProduct, DataValue = entity.AccountProduct });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.AccountBranchName, DataValue = entity.AccountBranchName });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.SubscriptionTypeName, DataValue = entity.ContactSubscriptionTypeName });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.CardNo, DataValue = entity.ContactCardNo });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.BirthDate, DataValue = entity.ContactBirthDateDisplay });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.TitleTh, DataValue = entity.ContactTitleTh });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.FirstNameTh, DataValue = entity.ContactFirstNameTh });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.LastNameTh, DataValue = entity.ContactLastNameTh });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.TitleEn, DataValue = entity.ContactTitleEn });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.FirstNameEn, DataValue = entity.ContactFirstNameEn });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.LastNameEn, DataValue = entity.ContactLastNameEn });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.PhoneNo1, DataValue = entity.ContactPhoneNo1 });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.PhoneNo2, DataValue = entity.ContactPhoneNo2 });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.PhoneNo3, DataValue = entity.ContactPhoneNo3 });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.Fax, DataValue = entity.ContactFax });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.Email, DataValue = entity.ContactEmail });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.ContactAccountNo, DataValue = entity.ContactAccountNo });
                contractInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ContractInfo.RelationshipName, DataValue = entity.ContactRelationshipName });

                #endregion

                #region "Product Info

                seqNo = 0;
                var productInfoList = new List<CAR.DataItem>();
                productInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ProductInfo.ProductGroupName, DataValue = entity.ProductGroupName });
                productInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ProductInfo.ProductName, DataValue = entity.ProductName });
                productInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ProductInfo.CampaignServiceName, DataValue = entity.CampaignServiceName });

                #endregion

                #region "Office Info"

                var officeInfoList = new List<CAR.DataItem>();
                officeInfoList.Add(new CAR.DataItem { SeqNo = 1, DataLabel = DataLabel.OfficeInfo.CreateUser, DataValue = entity.ActivityCreateUserFullName });

                #endregion

                #region "Activity Info"

                seqNo = 0;
                var activityInfoList = new List<CAR.DataItem>();
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.AreaCode, DataValue = entity.AreaCode.ToString() });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.AreaName, DataValue = entity.AreaName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.SubAreaCode, DataValue = entity.SubAreaCode.ToString() });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.SubAreaName, DataValue = entity.SubAreaName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.TypeCode, DataValue = entity.TypeCode.ToString() });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.TypeName, DataValue = entity.TypeName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.ChannelCode, DataValue = entity.ChannelCode });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.MediaSourceName, DataValue = entity.MediaSourceName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.Subject, DataValue = entity.Subject });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.Remark, DataValue = entity.Remark });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.Verify, DataValue = entity.Verify });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.VerifyResult, DataValue = entity.VerifyResult });

                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.SRCreatorBranchName, DataValue = entity.SRCreatorBranchName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.SRCreatorUserFullName, DataValue = entity.SRCreatorUserFullName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.OwnerBranchName, DataValue = entity.OwnerBranchName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.OwnerUserFullName, DataValue = entity.OwnerUserFullName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.DelegateBranchName, DataValue = entity.DelegateBranchName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.DelegateUserFullName, DataValue = entity.DelegateUserFullName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.SendEmail, DataValue = entity.SendEmail });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.EmailTo, DataValue = entity.EmailTo });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.EmailCc, DataValue = entity.EmailCc });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.EmailBcc, DataValue = entity.EmailBcc });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.EmailSubject, DataValue = entity.EmailSubject });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.EmailBody, DataValue = entity.EmailBody });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.EmailAttachments, DataValue = entity.EmailAttachments });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.ActivityDescription, DataValue = entity.ActivityDescription });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.ActivityTypeName, DataValue = entity.ActivityTypeName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.SRStateName, DataValue = entity.SRStateName });
                activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.SRStatusName, DataValue = entity.SRStatusName });

                if (entity.SrPageId == Constants.SRPage.DefaultPageId)
                {
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.DefaultPage.Address, DataValue = entity.AddressForDisplay });
                }
                else if (entity.SrPageId == Constants.SRPage.AFSPageId)
                {
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.AFSPage.AFSAssetNo, DataValue = entity.AFSAssetNo });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.AFSPage.AFSAssetDesc, DataValue = entity.AFSAssetDesc });
                }
                else if (entity.SrPageId == Constants.SRPage.NCBPageId)
                {
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.NCBPage.NCBCustomerBirthDate, DataValue = entity.NCBCustomerBirthDateDisplay });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.NCBPage.NCBMarketingBranchUpper1Name, DataValue = entity.NCBMarketingBranchUpper1Name });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.NCBPage.NCBMarketingBranchUpper2Name, DataValue = entity.NCBMarketingBranchUpper2Name });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.NCBPage.NCBMarketingBranchName, DataValue = entity.NCBMarketingBranchName });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.NCBPage.NCBMarketingFullName, DataValue = entity.NCBMarketingFullName });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.NCBPage.NCBCheckStatus, DataValue = entity.NCBCheckStatus });
                }
                else if (entity.SrPageId == Constants.SRPage.CPNPageId)
                {
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_ProductGroupName, DataValue = entity.CPN_ProductGroupName });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_ProductName, DataValue = entity.CPN_ProductName });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_CampaignName, DataValue = entity.CPN_CampaignName });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_SubjectName, DataValue = entity.CPN_SubjectName });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_TypeName, DataValue = entity.CPN_TypeName });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_RootCauseName, DataValue = entity.CPN_RootCauseName });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_IssueName, DataValue = entity.CPN_IssueName });

                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_IsSecret, DataValue = entity.CPN_IsSecret ? "Yes" : "No" });
                    //activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = "CAR", DataValue = entity.CPN_NotSend_CAR ? "Yes" : "No" });
                    //activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = "HP Log100", DataValue = entity.CPN_NotSend_HPLog ? "Yes" : "No" });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_BUGroupName, DataValue = entity.CPN_BUGroupName });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_BU1Desc, DataValue = entity.CPN_BU1Desc });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_BU2Desc, DataValue = entity.CPN_BU2Desc });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_BU3Desc, DataValue = entity.CPN_BU3Desc });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_BUBranch, DataValue = entity.CPN_BUBranchName });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_IsSummary, DataValue = !entity.CPN_IsSummary.HasValue ? "N/A" : (entity.CPN_IsSummary.Value ? "Yes" : "No") });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_CauseCustomer, DataValue = (entity.CPN_CauseCustomer ? "Yes" : "No") });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_CauseCustomerDetail, DataValue = entity.CPN_CauseCustomerDetail });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_CauseStaff, DataValue = (entity.CPN_CauseStaff ? "Yes" : "No") });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_CauseStaffDetail, DataValue = entity.CPN_CauseStaffDetail });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_CauseSystem, DataValue = (entity.CPN_CauseSystem ? "Yes" : "No") });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_CauseSystemDetail, DataValue = entity.CPN_CauseSystemDetail });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_CauseProcess, DataValue = (entity.CPN_CauseProcess ? "Yes" : "No") });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_CauseProcessDetail, DataValue = entity.CPN_CauseProcessDetail });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_CauseSummaryName, DataValue = entity.CPN_CauseSummaryName });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_SummaryName, DataValue = entity.CPN_SummaryName });
                    activityInfoList.Add(new CAR.DataItem { SeqNo = ++seqNo, DataLabel = DataLabel.ActivityInfo.CPNPage.CPN_Fixed_Detail, DataValue = entity.CPN_Fixed_Detail });
                }

                #endregion

                #region "ActivitiyLog Data"

                int customerSubscriptionTypeCode;

                try
                {
                    if (!string.IsNullOrEmpty(entity.CustomerSubscriptionTypeCode))
                        customerSubscriptionTypeCode = Convert.ToInt32(entity.CustomerSubscriptionTypeCode, CultureInfo.InvariantCulture);
                    else
                        customerSubscriptionTypeCode = 0;
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
                    ContractInfoList = contractInfoList.ToArray()
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
                        Logger.DebugFormat("-- XMLRequest --\n{0}\n{1}", header.SerializeObject(), activitiyLogData.SerializeObject());
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
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Call CBSHPService.SR_Insert").ToInputLogString());

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Logger.Debug("I:--START--:--CBSHPService.SR_Insert--");

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
                bool firstActivity = !(new ServiceRequestDataAccess(_context)).GetSrActivitys(sr.SrId).Any(x => x.SrActivityId != sr.SrActivityId);
                request.Description = sr.GetHpDescription(firstActivity);

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
                        Logger.DebugFormat("-- XMLRequest --\n{0}", request.SerializeObject());
                        response = ((ServiceSoap)client).SR_Insert(request);
                        if (client != null)
                        {
                            ((ICommunicationObject)client).Abort();
                        }
                    }

                    Logger.DebugFormat("-- XMLResponse --\n{0}", response.SerializeObject());

                    if (!string.IsNullOrWhiteSpace(response) && "SUCCESS".Equals(response.ToUpperInvariant()))
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

        public IEnumerable<ServiceRequestActivityResult> GetSRActivityList(ActivitySearchFilter searchFilter)
        {
            _srDataAccess = new ServiceRequestDataAccess(_context);
            var activities = _srDataAccess.GetSRActivityList(searchFilter);
            return activities.Any() ? activities.ToList() : null;
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
                    TransactionDateTime = DateTime.Now
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
                    ActivityEndDateTime = searchFilter.ActivityEndDateTimeValue.AddDays(1),     // "2016-06-07".ParseDateTime("yyyy-MM-dd").Value
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
                                    ContractID =  ac.ContractID,
                                    TypeID = ac.TypeID,
                                    TypeName = ac.TypeName,
                                    TicketID = ac.TicketID,
                                    ProductID = ac.ProductID,
                                    ProductName = ac.ProductName,
                                    ProductGroupID = ac.ProductGroupID,
                                    ProductGroupName = ac.ProductGroupName,
                                    SrID = ac.SrID,
                                    LeadID = ac.LeadID,
                                    SubAreaID = ac.SubAreaID,
                                    SubAreaName = ac.SubAreaName,
                                    Status = ac.Status,
                                    SubStatus = ac.SubStatus,
                                    SubscriptionTypeName = ac.SubscriptionTypeName,
                                    SubscriptionID = ac.SubscriptonID,
                                    CustomerInfoDataItems = ac.CustomerInfoList.ToList(),
                                    ContractInfoDataItems = ac.ContractInfoList.ToList(),
                                    ActivityInfoDataItems = ac.ActivityInfoList.ToList(),
                                    OfficerInfoDataItems = ac.OfficerInfoList.ToList(),
                                    ProductInfoDataItems = ac.ProductInfoList.ToList(),
                                    //CustomerInfo = new CustomerInfo
                                    //{
                                    //    SubscriptionType = GetDataItemValue(ac.CustomerInfoList.FirstOrDefault(x => x.SeqNo == 1)),
                                    //    SubscriptionID = GetDataItemValue(ac.CustomerInfoList.FirstOrDefault(x => x.SeqNo == 2)),
                                    //    FullName = GetDataItemValue(ac.CustomerInfoList.FirstOrDefault(x => x.SeqNo == 3))
                                    //},
                                    ContractInfo = new ContractInfo
                                    {
                                        //AccountNo = GetDataItemValue(ac.ContractInfoList.FirstOrDefault(x => x.SeqNo == 1)),
                                        //CreateSystem = GetDataItemValue(ac.ContractInfoList.FirstOrDefault(x => x.SeqNo == 2)),
                                        //RegistrationNo = GetDataItemValue(ac.ContractInfoList.FirstOrDefault(x => x.SeqNo == 3)),
                                        //FullName = GetDataItemValue(ac.ContractInfoList.FirstOrDefault(x => x.SeqNo == 11)) + " " + GetDataItemValue(ac.ContractInfoList.FirstOrDefault(x => x.SeqNo == 12)),
                                        AccountNo = GetDataItemValue(ac.ContractInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ContractInfo.AccountNo)),
                                        CreateSystem = GetDataItemValue(ac.ContractInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ContractInfo.AccountStatus)),
                                        RegistrationNo = GetDataItemValue(ac.ContractInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ContractInfo.AccountCarNo)),
                                        FullName = GetDataItemValue(ac.ContractInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ContractInfo.FirstNameTh)) + " " + GetDataItemValue(ac.ContractInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ContractInfo.LastNameTh)),
                                    },
                                    ActivityInfo = new ActivityInfo
                                    {
                                        //CreatorBranch = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 10)),
                                        //CreatorSR = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 11)),
                                        //OwnerBranch = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 12)),
                                        //OwnerSR = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 13)),
                                        //DelegateBranch = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 14)),
                                        //DelegateSR = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 15)),
                                        //SendEmail = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 16)),
                                        //EmailTo = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 17)),
                                        //EmailCc = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 18)),
                                        //EmailBcc = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 19)),
                                        //EmailSubject = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 20)),
                                        //EmailBody = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 21)),
                                        //EmailAttachments = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 22)),
                                        //ActivityDescription = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 23)),
                                        //ActivityType = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 24)),
                                        //SRStatus = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.SeqNo == 25)),
                                        CreatorBranch = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.SRCreatorBranchName)),
                                        CreatorSR = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.SRCreatorUserFullName)),
                                        OwnerBranch = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.OwnerBranchName)),
                                        OwnerSR = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.OwnerUserFullName)),
                                        DelegateBranch = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.DelegateBranchName)),
                                        DelegateSR = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.DelegateUserFullName)),
                                        SendEmail = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.SendEmail)),
                                        EmailTo = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.EmailTo)),
                                        EmailCc = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.EmailCc)),
                                        EmailBcc = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.EmailBcc)),
                                        EmailSubject = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.EmailSubject)),
                                        EmailBody = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.EmailBody)),
                                        EmailAttachments = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.EmailAttachments)),
                                        ActivityDescription = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.ActivityDescription)),
                                        ActivityType = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.ActivityTypeName)),
                                        SRState = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.SRStateName)),
                                        SRStatus = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.SRStatusName)),
                                        Is_Secret = GetDataItemValue(ac.ActivityInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.ActivityInfo.CPNPage.CPN_IsSecret))
                                    },
                                    OfficerInfo = new OfficerInfo
                                    {
                                        //FullName = GetDataItemValue(ac.OfficerInfoList.FirstOrDefault(x => x.SeqNo == 1))
                                        FullName = GetDataItemValue(ac.OfficerInfoList.FirstOrDefault(x => x.DataLabel == DataLabel.OfficeInfo.CreateUser))
                                    },
                                    //ProductInfo = new ProductInfo
                                    //{
                                    //    ProductGroup = GetDataItemValue(ac.ProductInfoList.FirstOrDefault(x => x.SeqNo == 1)),
                                    //    Product = GetDataItemValue(ac.ProductInfoList.FirstOrDefault(x => x.SeqNo == 2)),
                                    //    Campaign = GetDataItemValue(ac.ProductInfoList.FirstOrDefault(x => x.SeqNo == 3))
                                    //}
                                })
                                .OrderByDescending(x => x.SrID)
                                .ThenByDescending(x => x.ActivityDateTime)
                                .ToList();

                            Logger.Debug(_logMsg.Clear().SetPrefixMsg("CASLogServiceSoap.InquiryActivityLog")
                                            .Add("SR No", searchFilter.SrNo)
                                            .Add("ActivityDataItems Count", response.ActivityDataItems?.Count).ToOutputLogString());

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

        public IEnumerable<ServiceRequestActivityResult> GetServiceRequestActivityResults(AuditLogEntity auditLog, ActivitySearchFilter searchFilter)
        {
            var activities = GetActivityLogList(auditLog, searchFilter);
            IEnumerable<ServiceRequestActivityResult> actList = null;
            if (activities != null && activities.Any())
            {
                actList = activities.Select(x => new ServiceRequestActivityResult
                {
                    SystemCode = x.SystemCode,
                    SrActivityId = (int)x.ActivityID,
                    CustomerCardNo = x.SubscriptionID,
                    AccountNo = x.ContractID,
                    SrNo = x.SrNoDisplay,
                    ProductName = x.ProductName,
                    TypeName = x.TypeName,
                    AreaName = x.AreaName,
                    SubAreaName = x.SubAreaName,
                    StatusDesc = x.Status,
                    Date = x.ActivityDateTime,
                    ContactFullName = x.ContractInfo.FullName,
                    OfficerUserFullName = x.OfficerInfo.FullName,
                    ActivityDesc = x.ActivityInfo.ActivityDescription,
                    CreatorBranchName = x.ActivityInfo.CreatorBranch,
                    CreatorUserFullName = x.ActivityInfo.CreatorSR,
                    OwnerBranchName = x.ActivityInfo.OwnerBranch,
                    OwnerUserFullName = x.ActivityInfo.OwnerSR,
                    DelegateBranchName = x.ActivityInfo.DelegateBranch,
                    DelegateUserFullName = x.ActivityInfo.DelegateSR,
                    IsSendEmail = x.ActivityInfo.SendEmail.ToUpper(CultureInfo.InvariantCulture) == "Y",
                    EmailTo = x.ActivityInfo.EmailTo,
                    EmailCc = x.ActivityInfo.EmailCc,
                    EmailBcc = x.ActivityInfo.EmailBcc,
                    EmailSubject = x.ActivityInfo.EmailSubject,
                    EmailBody = x.ActivityInfo.EmailBody,
                    EmailAttachments = x.ActivityInfo.EmailAttachments,
                    ActivityTypeName = x.ActivityInfo.ActivityType,
                    SRStateName = x.ActivityInfo.SRState,
                    SRStatusName = x.ActivityInfo.SRStatus,
                    CustomerInfoDataItems = x.CustomerInfoDataItems?.Select(d => new ActivityDataItemEntity { DataLabel = d.DataLabel, SeqNo = d.SeqNo, DataValue = d.DataValue }).ToList(),
                    ContractInfoDataItems = x.ContractInfoDataItems?.Select(d => new ActivityDataItemEntity { DataLabel = d.DataLabel, SeqNo = d.SeqNo, DataValue = d.DataValue }).ToList(),
                    ActivityInfoDataItems = x.ActivityInfoDataItems?.Select(d => new ActivityDataItemEntity { DataLabel = d.DataLabel, SeqNo = d.SeqNo, DataValue = d.DataValue }).ToList(),
                    OfficerInfoDataItems = x.OfficerInfoDataItems?.Select(d => new ActivityDataItemEntity { DataLabel = d.DataLabel, SeqNo = d.SeqNo, DataValue = d.DataValue }).ToList(),
                    ProductInfoDataItems = x.ProductInfoDataItems?.Select(d => new ActivityDataItemEntity { DataLabel = d.DataLabel, SeqNo = d.SeqNo, DataValue = d.DataValue }).ToList(),
                    Is_Secret = x.Is_Secret
                }).ToList();
            }
            return actList.OrderByDescending(x => x.Date);
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
