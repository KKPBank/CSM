using CSM.Common.Utilities;
using CSM.Data.DataAccess;
using CSM.Entity;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using CSM.Common.Resources;

/// MOTIF
namespace CSM.Business
{
    public class ServiceRequestFacade : IServiceRequestFacade
    {
        private IUserFacade _userFacade;
        private ICommonFacade _commonFacade;
        private readonly CSMContext _context;
        private IActivityFacade _activityFacade;
        private ServiceRequestDataAccess _serviceRequestDataAccess;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ServiceRequestFacade));

        public ServiceRequestFacade()
        {
            _context = new CSMContext();
        }

        public List<AccountAddressTypeEntity> AddressTypeList()
        {
            return new ServiceRequestDataAccess(_context).AddressTypeList();
        }

        public IEnumerable<AccountAddressSearchResult> SearchAccountAddress(AccountAddressSearchFilter searchFilter)
        {
            return new ServiceRequestDataAccess(_context).SearchAccountAddress(searchFilter);
        }
        public IEnumerable<ServiceRequestCustomerSearchResult> SearchCustomer(CustomerSearchFilter searchFilter)
        {
            return new ServiceRequestDataAccess(_context).SearchCustomer(searchFilter);
        }

        public ServiceRequestCustomerAccount GetCustomerAccount(int accountId)
        {
            return new ServiceRequestDataAccess(_context).GetCustomerAccount(accountId);
        }

        public IEnumerable<ServiceRequestAccountSearchResult> SearchAccount(ContractSearchFilter searchFilter)
        {
            return new ServiceRequestDataAccess(_context).SearchAccount(searchFilter);
        }

        public IEnumerable<ServiceRequestContactSearchResult> SearchContact(CustomerContactSearchFilter searchFilter)
        {
            return new ServiceRequestDataAccess(_context).SearchContact(searchFilter);
        }

        public List<BranchEntity> AutoCompleteSearchBranch(string keyword, int limit)
        {
            return new BranchDataAccess(_context).AutoCompleteSearchBranch(keyword, limit);
        }

        public List<UserEntity> AutoCompleteSearchUser(string keyword, int? branchId, int limit)
        {
            return new UserDataAccess(_context).AutoCompleteSearchUser(keyword, branchId, limit);
        }

        public List<UserEntity> AutoCompleteSearchUserWithJobOnHand(string keyword, int branchId, int limit)
        {
            return new UserDataAccess(_context).AutoCompleteSearchUserWithJobOnHand(keyword, branchId, limit);
        }

        public List<ProductGroupEntity> AutoCompleteSearchProductGroup(string keyword, int limit, int? productId, bool? isAllStatus)
        {
            return new ProductGroupDataAccess(_context).AutoCompleteSearchProductGroup(keyword, limit, productId, isAllStatus);
        }

        public List<ProductEntity> AutoCompleteSearchProduct(string keyword, int? productGroupId, int limit, int? campaignServiceId, bool? isAllStatus)
        {
            return new ProductDataAccess(_context).AutoCompleteSearchProduct(keyword, productGroupId, limit, campaignServiceId, isAllStatus);
        }

        public List<CampaignServiceEntity> AutoCompleteSearchCampaignService(string keyword, int? productGroupId, int? productId, int limit, bool? isAllStatus)
        {
            return new CampaignServiceDataAccess(_context).AutoCompleteSearchCampaignService(keyword, productGroupId, productId, limit, isAllStatus);
        }

        public List<CampaignServiceEntity> AutoCompleteSearchCampaignServiceOnMapping(string keyword, int? areaId,
            int? subAreaId, int? typeId)
        {
            return new CampaignServiceDataAccess(_context).AutoCompleteSearchCampaignServiceOnMapping(keyword, areaId,
                subAreaId, typeId);
        }

        public List<AreaItemEntity> AutoCompleteSearchArea(string keyword, int? subAreaId, int limit, bool? isAllStatus)
        {
            return new AreaDataAccess(_context).AutoCompleteSearchArea(keyword, subAreaId, limit, isAllStatus);
        }

        public List<AreaItemEntity> AutoCompleteSearchAreaOnMapping(string keyword, int? campaignServiceId, int? subAreaId, int? typeId, int limit)
        {
            return new AreaDataAccess(_context).AutoCompleteSearchAreaOnMapping(keyword, campaignServiceId, subAreaId, typeId, limit);
        }

        public List<SubAreaItemEntity> AutoCompleteSearchSubArea(string keyword, int? areaId, int limit, bool? isAllStatus)
        {
            return new SubAreaDataAccess(_context).AutoCompleteSearchSubArea(keyword, areaId, limit, isAllStatus);
        }

        public List<SubAreaItemEntity> AutoCompleteSearchSubAreaOnMapping(string keyword, int? campaignServiceId, int? areaId, int? typeId, int limit)
        {
            return new SubAreaDataAccess(_context).AutoCompleteSearchSubAreaOnMapping(keyword, campaignServiceId, areaId, typeId, limit);
        }

        public List<TypeItemEntity> AutoCompleteSearchType(string keyword, int limit, bool? isAllStatus)
        {
            return new TypeDataAccess(_context).AutoCompleteSearchType(keyword, limit, isAllStatus);
        }

        public List<TypeItemEntity> AutoCompleteSearchTypeOnMapping(string keyword, int? campaignServiceId, int? areaId, int? subAreaId, int limit)
        {
            return new TypeDataAccess(_context).AutoCompleteSearchTypeOnMapping(keyword, campaignServiceId, areaId, subAreaId, limit);
        }

        public List<ChannelEntity> AutoCompleteSearchChannel()
        {
            return new ChannelDataAccess(_context).AutoCompleteSearchChannel();
        }

        public List<SRStatusEntity> AutoCompleteSearchSrStatus()
        {
            return new SrStatusDataAccess(_context).AutoCompleteSearchSrStatus();
        }

        public SingleMapProductEntity GetSingleMapProduct(int? campaignServiceId, int? areaId, int? subAreaId, int? typeId)
        {
            return new ServiceRequestDataAccess(_context).GetSingleMapProduct(campaignServiceId, areaId, subAreaId, typeId);
        }

        public IEnumerable<ServiceRequestEntity> SearchServiceRequest(ServiceRequestSearchFilter searchFilter)
        {
            var roleCode = searchFilter.CurrentUserRoleCode;

            if (string.IsNullOrEmpty(searchFilter.CanViewSrPageIds))
            {
                _serviceRequestDataAccess = new ServiceRequestDataAccess(_context);
                searchFilter.CanViewSrPageIds = string.Join(",", _serviceRequestDataAccess.GetSrPageIdsByRoleCode(roleCode));
            }

            if (!searchFilter.CanViewAllUsers.HasValue || searchFilter.CanViewUserIds == null)
            {
                if (roleCode == Constants.SrRoleCode.ITAdministrator || roleCode == Constants.SrRoleCode.UserAdministrator)
                {
                    // See All Owner & All Delegate
                    searchFilter.CanViewAllUsers = true;
                    searchFilter.CanViewUserIds = string.Empty;
                }
                else
                {
                    // See Only Current User & Sub-ordinate User & Group Assign
                    searchFilter.CanViewAllUsers = false;
                    searchFilter.CanViewUserIds = string.Join(",", GetUserIdsByAuthorize(searchFilter.CurrentUserId));
                }
            }

            var srDataAccess = new ServiceRequestDataAccess(_context);
            return srDataAccess.SearchServiceRequest(searchFilter);
        }

        public List<int> GetUserIdsByAuthorize(int userId)
        {
            _userFacade = new UserFacade();

            var resultUserIds = new List<int>();

            resultUserIds.Add(userId);

            var subOrdinateUserIds = _userFacade.GetUserIdsBySupervisorIds(new List<int> { userId });
            resultUserIds.AddRange(subOrdinateUserIds);

            var dummyUserIds = _userFacade.GetDummyUserIdsByUserIds(resultUserIds);
            resultUserIds.AddRange(dummyUserIds);

            return resultUserIds;
        }

        public IEnumerable<ChannelEntity> GetChannels()
        {
            return new CommPoolDataAccess(_context).GetActiveChannels();
        }

        public IEnumerable<MediaSourceEntity> GetMediaSources()
        {
            return new ServiceRequestDataAccess(_context).GetActiveMediaSources();
        }

        public IEnumerable<ActivityTypeEntity> GetActivityTypes()
        {
            return new ServiceRequestDataAccess(_context).GetActivityTypes();
        }

        public IEnumerable<SrEmailTemplateEntity> GetSrEmailTemplates()
        {
            return new ServiceRequestDataAccess(_context).GetSrEmailTemplates();
        }

        public List<SRStatusEntity> GetAvailableNextSrStatuses(int srId)
        {
            return new ServiceRequestDataAccess(_context).GetAvailableNextSrStatuses(srId);
        }

        public CampaignServiceEntity GetCampaignService(int campaignserviceId)
        {
            return new ServiceRequestDataAccess(_context).GetCampaignService(campaignserviceId);
        }

        public MappingProductEntity GetMapping(int campaignserviceId, int areaId, int subareaId, int typeId)
        {
            if (campaignserviceId == 0)
                throw new ArgumentException("CampaignServiceId cannot be zero", "campaignserviceId");
            if (areaId == 0)
                throw new ArgumentException("AreaId cannot be zero", "areaId");
            if (subareaId == 0)
                throw new ArgumentException("SubAreaId cannot be zero", "subareaId");
            if (typeId == 0)
                throw new ArgumentException("TypeId cannot be zero", "typeId");

            var da = new ServiceRequestDataAccess(_context);
            var mappingProduct = da.GetMappingByCampaign(campaignserviceId, areaId, subareaId, typeId);

            if (mappingProduct == null)
            {
                var campaignService = da.GetCampaignService(campaignserviceId);
                if (campaignService == null)
                    return null;

                mappingProduct = da.GetMappingByProduct(campaignService.ProductId, areaId, subareaId, typeId);
            }

            if (mappingProduct == null)
                return null;

            if (mappingProduct.IsVerify)
            {
                // Get only Active QuestionGroup
                mappingProduct.MappingProductQuestionGroups = da.GetMappingProductQuestionGroups(mappingProduct.MappingProductId);
            }

            return mappingProduct;
        }

        public ServiceRequestForDisplayEntity GetServiceRequest(int srId)
        {
            return new ServiceRequestDataAccess(_context).GetServiceRequest(srId);
        }

        public ServiceRequestNoDetailEntity GetServiceRequestNoDetail(int srId)
        {
            return new ServiceRequestDataAccess(_context).GetServiceRequestNoDetail(srId);
        }

        public ServiceRequestForDisplayEntity GetServiceRequest(string srNo)
        {
            return new ServiceRequestDataAccess(_context).GetServiceRequest(srNo);
        }

        public SrEmailTemplateEntity GetSrEmailTemplate(int id)
        {
            return new ServiceRequestDataAccess(_context).GetSrEmailTemplate(id);
        }

        public AccountAddressEntity GetDefaultAccountAddress(int accountId)
        {
            return new ServiceRequestDataAccess(_context).GetDefaultAccountAddress(accountId);
        }

        public ServiceRequestSaveResult CreateServiceRequest(AuditLogEntity auditLog, ServiceRequestForSaveEntity entity, bool isSaveDraft)
        {
            var warningMessages = new List<string>();

            if (isSaveDraft)
            {
                // Save Draft
                var srStatus = new SrStatusDataAccess(_context).GetSrStatus(Constants.SRStatusCode.Draft);
                if (srStatus == null)
                    return new ServiceRequestSaveResult(false, "No have DRAFT status in database (SR Status)");

                entity.SrStatusId = srStatus.SRStatusId;

                return new ServiceRequestDataAccess(_context).SaveServiceRequest(entity, isSaveDraft);
            }
            else
            {
                // Create Service Request

                // Create SR Number
                var newSeq = new CommonDataAccess(_context).GetNextServiceRequestSeq();
                entity.SrNo = ApplicationHelpers.GenerateSrNo(newSeq);

                if (entity.IsClose)
                {
                    var statusClose = new SrStatusDataAccess(_context).GetSrStatus(Constants.SRStatusCode.Closed);
                    if (statusClose == null)
                        return new ServiceRequestSaveResult(false, "No have CLOSE status in database (SR Status)");

                    entity.SrStatusId = statusClose.SRStatusId;
                    entity.CloseDate = DateTime.Now;
                }
                else
                {
                    var statusOpen = new SrStatusDataAccess(_context).GetSrStatus(Constants.SRStatusCode.Open);
                    if (statusOpen == null)
                        return new ServiceRequestSaveResult(false, "No have DRAFT status in database (SR Status)");

                    entity.SrStatusId = statusOpen.SRStatusId;
                }

                _serviceRequestDataAccess = new ServiceRequestDataAccess(_context);

                // Save to TB_T_SR
                var saveSrResult = _serviceRequestDataAccess.SaveServiceRequest(entity, isSaveDraft);
                if (!saveSrResult.IsSuccess)
                    return saveSrResult;

                // Insert to TB_T_SR_ACTIVITY
                var saveSrActivityResult = _serviceRequestDataAccess.CreateServiceRequestActivity(entity, null, null, null);
                if (!saveSrActivityResult.IsSuccess)
                    return saveSrActivityResult;

                var srId = saveSrResult.SrId.Value;
                var srActivityId = saveSrActivityResult.SrActivityId;
                var createUserId = entity.CreateUserId;
                var createUsername = entity.CreateUsername;
                var customerId = entity.CustomerId;

                // Insert Attachment
                if (entity.SrAttachments != null && entity.SrAttachments.Count > 0)
                    _serviceRequestDataAccess.CreateSrAttachment(entity.SrAttachments, srId, srActivityId, customerId, createUserId, createUsername);

                if (entity.SrEmailTemplateId.HasValue)
                {
                    SendEmail(saveSrActivityResult, entity.SrAttachments);
                }

                // Create Logging
                _serviceRequestDataAccess.CreateLoggingChangeStatus(srId, srActivityId, createUserId, createUsername, Constants.SystemName.CSM, null, entity.SrStatusId);

                _serviceRequestDataAccess.CreateLoggingChangeOwner(srId, srActivityId, createUserId, createUsername, Constants.SystemName.CSM, null, entity.OwnerUserId);

                if (entity.DelegateUserId.HasValue)
                    _serviceRequestDataAccess.CreateLoggingDelegate(srId, srActivityId, createUserId, createUsername, Constants.SystemName.CSM, null, entity.DelegateUserId);

                // Retrieve Activity Info from Database (For InsertLogging)
                var serviceRequestActivityForInsertLog = _serviceRequestDataAccess.GetServiceRequestActivityForInsertLog(srActivityId);
                serviceRequestActivityForInsertLog.EmailAttachments = saveSrActivityResult.EmailAttachments;

                // == Create Activity to CAR System ==
                var carResult = CreateActivityToCAR(auditLog, serviceRequestActivityForInsertLog);
                if (carResult != null)
                {
                    _serviceRequestDataAccess.UpdateSubmitCARStatusToActivity(srActivityId, carResult.Status, carResult.ErrorCode, carResult.ErrorMessage);

                    if (!string.IsNullOrEmpty(carResult.ErrorMessage))
                    {
                        warningMessages.Add("ไม่สามารถส่งข้อมูลที่ระบบ CAS" + (!string.IsNullOrEmpty(carResult.ErrorCode) ? string.Format(CultureInfo.InvariantCulture, " (ErrorCode={0})", carResult.ErrorCode) : ""));
                    }
                }

                FillHpDataFromMapping(serviceRequestActivityForInsertLog);

                string reasonNotSubmitCBSHP = "";

                bool isAccountAllowSubmitCBSHP = IsAccountAllowSubmitCBSHP(entity.AccountId, ref reasonNotSubmitCBSHP);

                if (!serviceRequestActivityForInsertLog.IsSendActivityToHP)
                {
                    isAccountAllowSubmitCBSHP = false;
                    reasonNotSubmitCBSHP = "Mapping Product-Type-Area is not configured to submit to CBS-HP";
                }

                if (serviceRequestActivityForInsertLog.IsSendActivityToHP && isAccountAllowSubmitCBSHP)
                {
                    // == Create Activity to CBS-HP System (Log100) ==
                    var hpResult = CreateActivityToCbsHpLog100(auditLog, serviceRequestActivityForInsertLog);
                    if (hpResult != null)
                    {
                        _serviceRequestDataAccess.UpdateSubmitCBSHPStatusToActivity(srActivityId, hpResult.Status, hpResult.ErrorCode, hpResult.ErrorMessage);

                        if (!string.IsNullOrEmpty(hpResult.ErrorCode) || !string.IsNullOrEmpty(hpResult.ErrorMessage))
                            warningMessages.Add("ไม่สามารถส่งข้อมูลที่ระบบ Log100 บรรทัด" + (!string.IsNullOrEmpty(hpResult.ErrorCode) ? string.Format(CultureInfo.InvariantCulture, " (ErrorCode={0})", hpResult.ErrorCode) : ""));
                    }
                }
                else
                {
                    // Not Send Data to HP
                    string errorMessage = null;
                    if (!string.IsNullOrEmpty(reasonNotSubmitCBSHP))
                        errorMessage = "Not Submit to CBS-HP because " + reasonNotSubmitCBSHP;

                    _serviceRequestDataAccess.UpdateSubmitCBSHPStatusToActivity(srActivityId, null, null, errorMessage);
                }

                return new ServiceRequestSaveResult()
                {
                    IsSuccess = true,
                    WarningMessages = warningMessages,
                    SrNo = saveSrResult != null ? saveSrResult.SrNo : null
                };
            }
        }

        private bool IsAccountAllowSubmitCBSHP(int? accountId, ref string reasonNotSubmitCBSHP)
        {
            if (!accountId.HasValue)
            {
                reasonNotSubmitCBSHP = "Account ID of This SR is null";
                return false;
            }

            var account = _serviceRequestDataAccess.GetCustomerAccount(accountId.Value);
            if (account == null)
            {
                reasonNotSubmitCBSHP = string.Format(CultureInfo.InvariantCulture, "Not found account from database (ACCOUNT_ID={0})", accountId.Value);
                return false;
            }
            else
            {
                _commonFacade = new CommonFacade();

                if (string.IsNullOrEmpty(account.Account.AccountNo))
                {
                    reasonNotSubmitCBSHP = "Account No is null or empty";
                    return false;
                }

                if (account.Account.AccountNo.ToUpper(CultureInfo.InvariantCulture) == _commonFacade.GetTextDummyAccountNo())
                {
                    reasonNotSubmitCBSHP = "Account No is DUMMY";
                    return false;
                }

                if (string.IsNullOrEmpty(account.Account.ProductGroup))
                {
                    reasonNotSubmitCBSHP = "ProductGroup of this account is null or empty";
                    return false;
                }

                var allowProductGroups = _commonFacade.GetProductGroupSubmitCBSHP();
                if (string.IsNullOrEmpty(allowProductGroups))
                {
                    reasonNotSubmitCBSHP = "ProductGroup of this account is not submit to CBS-HP";
                    return false;
                }
                else
                {
                    var allowProductGroupArray = allowProductGroups.Split(',').Select(x => x.Trim().ToUpper(CultureInfo.InvariantCulture));
                    if (allowProductGroupArray.Contains(account.Account.ProductGroup.ToUpper(CultureInfo.InvariantCulture)))
                    {
                        return true;
                    }
                    else
                    {
                        reasonNotSubmitCBSHP = "ProductGroup of this account is not submit to CBS-HP";
                        return false;
                    }
                }
            }
        }

        private bool SendEmail(ServiceRequestSaveActivityResult entity, List<SrAttachmentEntity> list)
        {
            string senderEmail = entity.EmailSender;
            string receiverEmails = entity.EmailReceivers;
            string ccEmails = entity.EmailCcs;

            string subject = entity.EmailSubject;
            string message = entity.EmailBody;

            var attachmentStreams = new List<byte[]>();
            var attachmentFilenames = new List<string>();

            _commonFacade = new CommonFacade();
            var docFolder = _commonFacade.GetSrDocumentFolder();

            if (list != null && list.Count > 0)
            {
                foreach (var srAttachmentEntity in list)
                {
                    var attachToEmail = srAttachmentEntity.AttachToEmail;
                    if (!string.IsNullOrEmpty(attachToEmail) && (attachToEmail.ToLower() == "true") && (srAttachmentEntity.Status ?? 0) == 1)
                    {
                        try
                        {
                            var bytes = File.ReadAllBytes(docFolder + "\\" + srAttachmentEntity.Url);

                            attachmentStreams.Add(bytes);
                            attachmentFilenames.Add(srAttachmentEntity.Filename);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(_logMsg.Clear().SetPrefixMsg("Attach File To Email").ToInputLogString(), ex);
                        }
                    }
                }
            }

            return CSMMailSender.GetCSMMailSender().SendMail(senderEmail, receiverEmails, ccEmails, subject, message, attachmentStreams, attachmentFilenames);
        }

        private ServiceRequestSaveCarResult CreateActivityToCAR(AuditLogEntity auditLog, ServiceRequestForInsertLogEntity entity)
        {
            if (!string.IsNullOrEmpty(entity.CustomerSubscriptionTypeCode))
            {
                try
                {
                    Convert.ToInt32(entity.CustomerSubscriptionTypeCode, CultureInfo.InvariantCulture); //int customerSubscriptionTypeId = Convert.ToInt32(entity.CustomerSubscriptionTypeCode);
                }
                catch (Exception)
                {
                    return new ServiceRequestSaveCarResult()
                    {
                        IsSuccess = false,
                        ErrorCode = "CAST_TYPE_ERROR",
                        ErrorMessage = "รหัส SubscriptionType ไม่เป็นข้อมูลตัวเลข จึงไม่สามารถส่งข้อมูลไปที่ CAR ได้",
                    };
                }
            }

            _activityFacade = new ActivityFacade();
            return _activityFacade.InsertActivityLogToCAR(auditLog, entity);
        }

        private void FillHpDataFromMapping(ServiceRequestForInsertLogEntity entity)
        {
            var mappingProduct = new MappingProductTypeDataAccess(_context).GetMappingById(entity.MappingProductId);
            if (mappingProduct == null)
            {
                throw new CustomException("Not found Mapping Product in database (MappingProductID={0})", entity.MappingProductId);
            }
            else
            {
                entity.HPSubject = mappingProduct.HPSubject;
                entity.HPLanguageIndependentCode = mappingProduct.HPLanguageIndependentCode;
            }
        }

        private ServiceRequestSaveCbsHpResult CreateActivityToCbsHpLog100(AuditLogEntity auditLog, ServiceRequestForInsertLogEntity entity)
        {
            _activityFacade = new ActivityFacade();
            return _activityFacade.InsertActivityLogToLog100(auditLog, entity);
        }

        public void UpdateServiceRequest(ServiceRequestForSaveEntity newValue)
        {
            _serviceRequestDataAccess = new ServiceRequestDataAccess(_context);

            var srId = newValue.SrId.Value;
            //var createUserId = newValue.CreateUserId;

            // Get Value from DB
            var dbValue = _serviceRequestDataAccess.GetServiceRequestNoDetail(srId);
            if (dbValue == null)
                throw new Exception("Technical Error: ไม่พบข้อมูล Service Request (ID=" + srId + ")");

            _serviceRequestDataAccess.UpdateServiceRequest(newValue);
        }


        public ServiceRequestSaveResult CreateServiceRequestActivity(AuditLogEntity auditLog, ServiceRequestForSaveEntity newValue, bool disableSendToHp = false)
        {
            var now = DateTime.Now;

            _serviceRequestDataAccess = new ServiceRequestDataAccess(_context);

            var srId = newValue.SrId.Value;
            var createUserId = newValue.CreateUserId;
            var createUsername = newValue.CreateUsername;

            // Get Value from DB
            var dbValue = _serviceRequestDataAccess.GetServiceRequestNoDetail(srId);
            if (dbValue == null)
                throw new Exception("Technical Error: ไม่พบข้อมูล Service Request (ID=" + srId + ")");

            //var oldOwnerBranchId = dbValue.OwnerBranchId;
            var oldOwnerUserId = dbValue.OwnerUserId;
            //var oldDelegateBrandhId = dbValue.DelegateBranchId;
            var oldDelegateUserId = dbValue.DelegateUserId;
            var oldSrStatusId = dbValue.SrStatusId;

            dbValue.OwnerBranchId = newValue.OwnerBranchId;
            dbValue.OwnerUserId = newValue.OwnerUserId;
            dbValue.DelegateBranchId = newValue.DelegateBranchId;
            dbValue.DelegateUserId = newValue.DelegateUserId;
            dbValue.SrStatusId = newValue.SrStatusId;
            dbValue.IsEmailDelegate = newValue.IsEmailDelegate;

            // Update Service Request
            _serviceRequestDataAccess.UpdateServiceRequest(dbValue, now);

            // Create Activity
            var saveSrActivityResult = _serviceRequestDataAccess.CreateServiceRequestActivity(newValue, oldOwnerUserId, oldDelegateUserId, oldSrStatusId);
            if (!saveSrActivityResult.IsSuccess)
                return saveSrActivityResult;

            var srActivityId = saveSrActivityResult.SrActivityId;

            // Insert Attachment
            if (newValue.SrAttachments != null && newValue.SrAttachments.Count > 0)
                _serviceRequestDataAccess.CreateSrAttachment(newValue.SrAttachments, srId, srActivityId, dbValue.CustomerId, createUserId, createUsername);

            // Send Mail
            if (newValue.SrEmailTemplateId.HasValue)
            {
                SendEmail(saveSrActivityResult, newValue.SrAttachments);
            }

            // Create Logging
            if (oldSrStatusId != newValue.SrStatusId)
                _serviceRequestDataAccess.CreateLoggingChangeStatus(srId, srActivityId, createUserId, createUsername, Constants.SystemName.CSM, oldSrStatusId, newValue.SrStatusId);

            if (oldOwnerUserId != newValue.OwnerUserId)
                _serviceRequestDataAccess.CreateLoggingChangeOwner(srId, srActivityId, createUserId, createUsername, Constants.SystemName.CSM, oldOwnerUserId, newValue.OwnerUserId);

            if (oldDelegateUserId != newValue.DelegateUserId)
                _serviceRequestDataAccess.CreateLoggingDelegate(srId, srActivityId, createUserId, createUsername, Constants.SystemName.CSM, oldDelegateUserId, newValue.DelegateUserId);

            // Retrieve Activity Info from Database (For InsertLogging)
            var serviceRequestActivityForInsertLog = _serviceRequestDataAccess.GetServiceRequestActivityForInsertLog(srActivityId);
            serviceRequestActivityForInsertLog.EmailAttachments = saveSrActivityResult.EmailAttachments;

            var warningMessages = new List<string>();

            // == Create Activity to CAR System ==
            var carResult = CreateActivityToCAR(auditLog, serviceRequestActivityForInsertLog);
            if (carResult != null)
            {
                _serviceRequestDataAccess.UpdateSubmitCARStatusToActivity(srActivityId, carResult.Status, carResult.ErrorCode, carResult.ErrorMessage);

                if (!string.IsNullOrEmpty(carResult.ErrorMessage))
                {
                    warningMessages.Add("ไม่สามารถส่งข้อมูลที่ระบบ CAS" + (!string.IsNullOrEmpty(carResult.ErrorCode) ? string.Format(CultureInfo.InvariantCulture, " (ErrorCode={0})", carResult.ErrorCode) : ""));
                }
            }

            if (!disableSendToHp)
            {
                if (dbValue.MapProductId.HasValue)
                    newValue.MappingProductId = dbValue.MapProductId.Value;

                serviceRequestActivityForInsertLog.MappingProductId = dbValue.MapProductId.Value;

                string reasonNotSubmitCBSHP = "";
                bool isAccountAllowSubmitCBSHP = IsAccountAllowSubmitCBSHP(dbValue.AccountId, ref reasonNotSubmitCBSHP);

                FillHpDataFromMapping(serviceRequestActivityForInsertLog);
                if (!serviceRequestActivityForInsertLog.IsSendActivityToHP)
                {
                    isAccountAllowSubmitCBSHP = false;
                    reasonNotSubmitCBSHP = "Mapping Product-Type-Area is not configured to submit to CBS-HP";
                }

                if (serviceRequestActivityForInsertLog.IsSendActivityToHP && isAccountAllowSubmitCBSHP)
                {
                    // == Create Activity to CBS-HP System (Log100) ==
                    var hpResult = CreateActivityToCbsHpLog100(auditLog, serviceRequestActivityForInsertLog);
                    if (hpResult != null)
                    {
                        _serviceRequestDataAccess.UpdateSubmitCBSHPStatusToActivity(srActivityId, hpResult.Status, hpResult.ErrorCode, hpResult.ErrorMessage);

                        if (!string.IsNullOrEmpty(hpResult.ErrorCode) || !string.IsNullOrEmpty(hpResult.ErrorMessage))
                            warningMessages.Add("ไม่สามารถส่งข้อมูลที่ระบบ Log100 บรรทัด" + (!string.IsNullOrEmpty(hpResult.ErrorCode) ? string.Format(CultureInfo.InvariantCulture, " (ErrorCode={0})", hpResult.ErrorCode) : ""));
                    }
                }
                else
                {
                    // Not Send Data to HP
                    string errorMessage = null;
                    if (!string.IsNullOrEmpty(reasonNotSubmitCBSHP))
                        errorMessage = "Not Submit to CBS-HP because " + reasonNotSubmitCBSHP;

                    _serviceRequestDataAccess.UpdateSubmitCBSHPStatusToActivity(srActivityId, null, null, errorMessage);
                }
            }

            return new ServiceRequestSaveResult()
            {
                IsSuccess = true,
                WarningMessages = warningMessages,
            };
        }

        public List<AfsAssetEntity> AutoCompleteSearchAfsAsset(string keyword, int limit)
        {
            return new ServiceRequestDataAccess(_context).AutoCompleteSearchAfsAsset(keyword, limit);
        }

        public AfsAssetEntity GetAssetInfo(int afsAssetId)
        {
            return new ServiceRequestDataAccess(_context).GetAssetInfo(afsAssetId);
        }

        public Dictionary<string, string> GetNCBCheckStatuses()
        {
            var statuses = new Dictionary<string, string>();
            statuses.Add(Constants.NCBCheckStatus.Found, Constants.NCBCheckStatus.Found);
            statuses.Add(Constants.NCBCheckStatus.NotFound, Constants.NCBCheckStatus.NotFound);

            return statuses;
        }

        public UserMarketingEntity GetUserMarketing(int customerId)
        {
            return new CustomerDataAccess(_context).GetUserMarketing(customerId);
        }

        public UserEntity GetUserById(int id)
        {
            return new UserDataAccess(_context).GetUserById(id);
        }

        public CustomerEntity GetCustomerByID(int id)
        {
            return new CustomerDataAccess(_context).GetCustomerByID(id);
        }


        public ServiceRequestCustomerContactEntity GetCustomerContact(int id)
        {
            var dataAccess = new ServiceRequestDataAccess(_context);

            var customerContact = dataAccess.GetCustomerContact(id);
            if (customerContact == null || !customerContact.ContactId.HasValue || !customerContact.AccountId.HasValue)
                return null;

            var contact = new CustomerDataAccess(_context).GetContactByID(customerContact.ContactId.Value);
            if (contact == null)
                return null;

            var customerAccount = GetCustomerAccount(customerContact.AccountId.Value);
            if (customerAccount == null)
                return null;

            return new ServiceRequestCustomerContactEntity()
            {
                CustomerAccount = customerAccount,
                Contact = contact,
                CustomerContact = customerContact,
            };
        }

        public IEnumerable<ServiceRequestActivityResult> GetTabActivityList(AuditLogEntity auditLog, ActivityTabSearchFilter searchFilter)
        {
            _commonFacade = new CommonFacade();
            _activityFacade = new ActivityFacade();

            searchFilter.IsOnline = true;

            try
            {
                if (searchFilter.IsOnline)
                {
                    var onlineSearchFilter = new ActivitySearchFilter();
                    onlineSearchFilter.ActivityStartDateTime = DateTime.Now.AddMonths(-3).FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
                    onlineSearchFilter.ActivityEndDateTime = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);

                    onlineSearchFilter.SrNo = searchFilter.SrNo;
                    onlineSearchFilter.CardNo = searchFilter.CustomerCardNo;
                    onlineSearchFilter.SubsTypeCode = searchFilter.CustomerSubscriptionTypeCode;
                    onlineSearchFilter.SrOnly = searchFilter.SrOnly;

                    onlineSearchFilter.PageNo = searchFilter.PageNo;
                    onlineSearchFilter.PageSize = _commonFacade.GetPageSizeStart();
                    onlineSearchFilter.SortField = "ActivityID";
                    onlineSearchFilter.SortOrder = "DESC";

                    //                    if (string.IsNullOrWhiteSpace(searchFilter.JsonActivities))
                    //                    {
                    //                        var results = _activityFacade.GetActivityLogList(auditLog, onlineSearchFilter);
                    //                        searchFilter.JsonActivities = JsonConvert.SerializeObject(results);
                    //                        onlineSearchFilter.JsonActivities = searchFilter.JsonActivities;
                    //                    }

                    var data = _activityFacade.GetActivityLogList(auditLog, onlineSearchFilter);
                    searchFilter.TotalRecords = onlineSearchFilter.TotalRecords;
                    searchFilter.IsOnline = true;

                    searchFilter.PageNo = onlineSearchFilter.PageNo;

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Activity (Online)").ToSuccessLogString());

                    return data.Select(x => new ServiceRequestActivityResult
                    {
                        SrActivityId = Convert.ToInt32(x.ActivityID, CultureInfo.InvariantCulture),
                        CustomerCardNo = x.SubscriptionID,
                        AccountNo = x.ContractID,
                        SrNo = x.SrID,
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
                        EmailSubject = x.ActivityInfo.EmailSubject,
                        EmailBody = x.ActivityInfo.EmailBody,
                        EmailAttachments = x.ActivityInfo.EmailAttachments,
                        ActivityTypeName = x.ActivityInfo.ActivityType,
                        SRStatusName = x.ActivityInfo.SRStatus,
                    }).ToList();
                }
            }
            catch
            {
                Logger.Error(_logMsg.Clear().SetPrefixMsg("Search Activity (Online) is Fail").ToFailLogString());
            }

            searchFilter.IsOnline = false;
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Search Activity (Local Database)").ToSuccessLogString());
            return new ServiceRequestDataAccess(_context).GetTabActivityList(searchFilter);

        }

        public IEnumerable<ServiceDocumentEntity> GetTabDocumentList(DocumentSearchFilter searchFilter)
        {
            return new ServiceRequestDataAccess(_context).GetTabDocumentList(searchFilter);
        }

        public IEnumerable<ServiceRequestLoggingResult> GetTabLoggingList(LoggingSearchFilter searchFilter)
        {
            return new ServiceRequestDataAccess(_context).GetTabLoggingList(searchFilter);
        }

        public IEnumerable<ServiceRequestEntity> GetTabExistingList(ExistingSearchFilter searchFilter)
        {
            return new ServiceRequestDataAccess(_context).GetTabExistingList(searchFilter);
        }

        public List<SrVerifyGroupEntity> GetVerifyGroup(int srId)
        {
            return new ServiceRequestDataAccess(_context).GetVerifyGroup(srId);
        }

        public bool SaveSrAttachment(AttachmentEntity attach)
        {
            return new ServiceRequestDataAccess(_context).SaveSrAttachment(attach);
        }

        public bool CheckDuplicateDocumentFilename(AttachmentEntity attach)
        {
            return new ServiceRequestDataAccess(_context).CheckDuplicateDocumentFilename(attach);
        }

        public bool DeleteSrAttachment(int? srAttachId)
        {
            return new ServiceRequestDataAccess(_context).DeleteSrAttachment(srAttachId);
        }

        public AttachmentEntity GetSrAttachmentById(int srAttachId, string documentLevel)
        {
            return new ServiceRequestDataAccess(_context).GetSrAttachmentById(srAttachId, documentLevel);
        }

        public IEnumerable<ServiceDocumentEntity> GetActivityDocumentList(int srId)
        {
            return new ServiceRequestDataAccess(_context).GetActivityDocumentList(srId);
        }

        public void UpdateDocumentCustomerLevel(int attachmentId, string documentDesc, DateTime? expiryDate, List<int> docTypeIds, int updateUserId)
        {
            new ServiceRequestDataAccess(_context).UpdateDocumentCustomerLevel(attachmentId, documentDesc, expiryDate, docTypeIds, updateUserId);
        }

        public bool CheckDuplicateContact(ContactEntity contactEntity)
        {
            return new ServiceRequestDataAccess(_context).CheckDuplicateContact(contactEntity);
        }

        public bool CheckDuplicatePhoneNo(List<string> lstPhoneNo)
        {
            return new ServiceRequestDataAccess(_context).CheckDuplicatePhoneNo(lstPhoneNo);
        }

        public List<ServiceRequestCustomerSearchResult> GetCustomerAccountByPhoneNo(List<string> lstPhoneNo)
        {
            return new ServiceRequestDataAccess(_context).GetCustomerAccountByPhoneNo(lstPhoneNo);
        }

        public int GetServiceAttachTotalFileSize(int srId)
        {
            return new ServiceRequestDataAccess(_context).GetServiceAttachTotalFileSize(srId);
        }

        public ServiceRequestContactSearchResult GetCustomerContactById(int contactId)
        {
            return new ServiceRequestDataAccess(_context).GetCustomerContactById(contactId);
        }

        #region == Batch Processing ==

        public void CreateSRActivityFromReplyEmail(ref int countSuccess, ref int countError)
        {
            _commonFacade = new CommonFacade();
            var minute = _commonFacade.GetMaxMinuteBatchCreateSRActivityFromReplyEmail();

            var endTime = DateTime.Now.AddMinutes(minute);

            _serviceRequestDataAccess = new ServiceRequestDataAccess(_context);

            int rowCount = 0;

            var list = _serviceRequestDataAccess.GetWaitingForProcessReplyEmail();
            foreach (var item in list)
            {
                if (endTime < DateTime.Now)
                {
                    break;
                }

                try
                {
                    rowCount++;

                    var auditLog = new AuditLogEntity();
                    auditLog.Module = Constants.Module.ServiceRequest;
                    auditLog.Action = Constants.AuditAction.SyncSRStatusFromReplyEmail;
                    auditLog.IpAddress = ApplicationHelpers.GetClientIP();

                    Logger.DebugFormat("I:--START--:--CreateSRActivityFromReplyEmail (Item-{0})--:--START--", rowCount);

                    foreach (var srAttachmentEntity in item.SrAttachments)
                    {
                        if (!string.IsNullOrEmpty(srAttachmentEntity.Filename))
                        {
                            try
                            {
                                srAttachmentEntity.Name = Path.GetFileNameWithoutExtension(srAttachmentEntity.Filename);
                            }
                            catch (Exception)
                            {
                                Logger.Error(_logMsg.Clear().SetPrefixMsg("CreateSRActivityFromReplyEmail - Convert Filename to Name is fail").ToFailLogString());
                            }
                        }

                        #region Copy File to SR Folder

                        var jobDocFolder = _commonFacade.GetJobDocumentFolder();
                        var srDocFolder = _commonFacade.GetSrDocumentFolder();

                        var sourceFile = Path.Combine(jobDocFolder, srAttachmentEntity.Url);
                        var targetFile = Path.Combine(srDocFolder, srAttachmentEntity.Url);

                        if (!File.Exists(sourceFile))
                        {
                            Logger.Error(_logMsg.Clear().SetPrefixMsg(string.Format("CreateSRActivityFromReplyEmail - Fail to copy attachment file: Source file is not exist. (Source File Path='{0}')", sourceFile)).ToFailLogString());
                        }

                        var targetDirectory = Path.GetDirectoryName(targetFile);
                        if (!Directory.Exists(targetDirectory))
                        {
                            Directory.CreateDirectory(targetDirectory);
                        }

                        try
                        {
                            File.Copy(sourceFile, targetFile, true);
                        }
                        catch (Exception)
                        {
                            Logger.Error(_logMsg.Clear().SetPrefixMsg(string.Format("CreateSRActivityFromReplyEmail - Fail to copy attachment file from '{0}' to '{1}'", sourceFile, targetFile)).ToFailLogString());
                        }

                        try
                        {
                            srAttachmentEntity.FileSize = Convert.ToInt32(new FileInfo(targetFile).Length);
                        }
                        catch (Exception)
                        {
                            Logger.Error(_logMsg.Clear().SetPrefixMsg(string.Format("CreateSRActivityFromReplyEmail - Cannot get file size: '{0}'", targetFile)).ToFailLogString());
                        }

                        #endregion
                    }

                    var attachmentFilenameListHtml = string.Empty;

                    if (item.SrAttachments.Count > 0)
                    {
                        attachmentFilenameListHtml = "E-Mail Attachments: " + string.Join(",", item.SrAttachments.Select(x => x.Filename).ToList()) + "<br/>";
                    }

                    var activityDescription = string.Format("E-Mail From: {0}<br/>E-Mail Subject: {1}<br/>{2}E-Mail Body: {3}", item.EmailFrom, item.EmailSubject, attachmentFilenameListHtml, item.EmailBody);

                    if (activityDescription.Length > Constants.ActivityDescriptionMaxLength)
                    {
                        activityDescription = string.Format("E-Mail From: {0}<br/>E-Mail Subject: {1}<br/>{2}E-Mail Body: {3}", item.EmailFrom, item.EmailSubject, attachmentFilenameListHtml, SRResource.CannotDisplayEmailBodyBecauseMaxLength);

                        if (activityDescription.Length > Constants.ActivityDescriptionMaxLength)
                        {
                            activityDescription = SRResource.CannotDisplayEmailBodyBecauseMaxLength;
                        }
                    }

                    var data = new ServiceRequestForSaveEntity
                                {
                                    SrId = item.SrId,
                                    CreateUserId = item.CreateUserId,
                                    CreateUsername = item.CreateUsername,
                                    OwnerBranchId = item.OwnerBranchId,
                                    OwnerUserId = item.OwnerUserId,
                                    DelegateBranchId = item.DelegateBranchId,
                                    DelegateUserId = item.DelegateUserId,
                                    SrStatusId = (item.IsChangeStatus && item.CanChangeStatus) ? item.NewSrStatusId : item.OldSrStatusId,
                                    ActivityDescription = activityDescription,
                                    ActivityTypeId = Constants.EmailInboundActivityTypeId,
                                    SrAttachments = item.SrAttachments
                                };

                    var result = CreateServiceRequestActivity(auditLog, data, false);

                    if (result.IsSuccess)
                    {
                        Logger.DebugFormat("I:--START--:--CreateSRActivityFromReplyEmail (Item-{0})--:--END-Success--", rowCount);

                        _serviceRequestDataAccess.UpdateProcessReplyEmail(item.SrReplyEmailId, true, DateTime.Now, result.ErrorCode, result.ErrorMessage);
                        countSuccess++;
                    }
                    else
                    {
                        Logger.DebugFormat("I:--START--:--CreateSRActivityFromReplyEmail (Item-{0})--:--END-Fail--", rowCount);

                        _serviceRequestDataAccess.UpdateProcessReplyEmail(item.SrReplyEmailId, false, DateTime.Now, result.ErrorCode, result.ErrorMessage);
                        countError++;
                    }
                }
                catch (Exception ex)
                {
                    _serviceRequestDataAccess.UpdateProcessReplyEmail(item.SrReplyEmailId, false, DateTime.Now, "1", ex.Message);
                    countError++;
                }
            }
        }

        public void ReSubmitActivityToCARSystem(ref int countSuccess, ref int countError)
        {
            _commonFacade = new CommonFacade();
            var minute = _commonFacade.GetMaxMinuteBatchReSubmitActivityToCARSystem();


            var endTime = DateTime.Now.AddMinutes(minute);

            _serviceRequestDataAccess = new ServiceRequestDataAccess(_context);

            int rowCount = 0;

            var srActivityList = _serviceRequestDataAccess.GetWaitingForProcessReSubmitToCAR();
            foreach (var srActivityId in srActivityList)
            {
                if (endTime < DateTime.Now)
                {
                    break;
                }

                try
                {
                    rowCount++;

                    var auditLog = new AuditLogEntity();
                    auditLog.Module = Constants.Module.ServiceRequest;
                    auditLog.Action = Constants.AuditAction.ReSubmitActivityToCARSystem;
                    auditLog.IpAddress = ApplicationHelpers.GetClientIP();

                    Logger.DebugFormat("I:--START--:--ReSubmitActivityToCARSystem (Item-{0})--:--START--", rowCount);

                    // Retrieve Activity Info from Database (For InsertLogging)
                    var serviceRequestActivityForInsertLog = _serviceRequestDataAccess.GetServiceRequestActivityForInsertLog(srActivityId);

                    // == Create Activity to CAR System ==
                    var carResult = CreateActivityToCAR(auditLog, serviceRequestActivityForInsertLog);
                    if (carResult != null)
                    {
                        _serviceRequestDataAccess.UpdateSubmitCARStatusToActivity(srActivityId, carResult.Status, carResult.ErrorCode, carResult.ErrorMessage);

                        if (carResult.Status == 1)
                        {
                            Logger.DebugFormat("I:--START--:--ReSubmitActivityToCARSystem (Item-{0})--:--END-Success--", rowCount);
                            countSuccess++;
                        }
                        else
                        {
                            Logger.DebugFormat("I:--START--:--ReSubmitActivityToCARSystem (Item-{0})--:--END-Fail--", rowCount);
                            countError++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.DebugFormat("I:--START--:--ReSubmitActivityToCARSystem (Item-{0})--:--END-Fail--", rowCount);
                    _serviceRequestDataAccess.UpdateSubmitCARStatusToActivity(srActivityId, 0, "1", ex.Message);
                    countError++;
                }
            }
        }

        public void ReSubmitActivityToCBSHPSystem(ref int countSuccess, ref int countError)
        {
            _commonFacade = new CommonFacade();
            var minute = _commonFacade.GetMaxMinuteBatchReSubmitActivityToCARSystem();


            var endTime = DateTime.Now.AddMinutes(minute);

            _serviceRequestDataAccess = new ServiceRequestDataAccess(_context);

            int rowCount = 0;

            var srActivityList = _serviceRequestDataAccess.GetWaitingForProcessReSubmitToCBSHP();
            foreach (var srActivityId in srActivityList)
            {
                if (endTime < DateTime.Now)
                {
                    break;
                }

                try
                {
                    rowCount++;

                    var auditLog = new AuditLogEntity();
                    auditLog.Module = Constants.Module.ServiceRequest;
                    auditLog.Action = Constants.AuditAction.ReSubmitActivityToCBSHPSystem;
                    auditLog.IpAddress = ApplicationHelpers.GetClientIP();

                    Logger.DebugFormat("I:--START--:--ReSubmitActivityToCBSHPSystem (Item-{0})--:--START--", rowCount);

                    // Retrieve Activity Info from Database (For InsertLogging)
                    var serviceRequestActivityForInsertLog = _serviceRequestDataAccess.GetServiceRequestActivityForInsertLog(srActivityId);

                    FillHpDataFromMapping(serviceRequestActivityForInsertLog);

                    // == Create Activity to CBS-HP System (Log100) ==
                    var hpResult = CreateActivityToCbsHpLog100(auditLog, serviceRequestActivityForInsertLog);
                    if (hpResult != null)
                    {
                        _serviceRequestDataAccess.UpdateSubmitCBSHPStatusToActivity(srActivityId, hpResult.Status, hpResult.ErrorCode, hpResult.ErrorMessage);

                        if (hpResult.Status == 1)
                        {
                            Logger.DebugFormat("I:--START--:--ReSubmitActivityToCBSHPSystem (Item-{0})--:--END-Success--", rowCount);
                            countSuccess++;
                        }
                        else
                        {
                            Logger.DebugFormat("I:--START--:--ReSubmitActivityToCBSHPSystem (Item-{0})--:--END-Fail--", rowCount);
                            countError++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.DebugFormat("I:--START--:--ReSubmitActivityToCBSHPSystem (Item-{0})--:--END-Fail--", rowCount);
                    _serviceRequestDataAccess.UpdateSubmitCARStatusToActivity(srActivityId, 0, "1", ex.Message);
                    countError++;
                }
            }
        }

        #endregion

        #region == Provide Web Service ==

        public CreateSRResponse CreateSRWebServiceOld(CreateSRRequest request)
        {
            try
            {
                var beforeExecute = DateTime.Now;

                #region == Validate Require Field ==

                var requireFieldFails = new List<string>();

                if (string.IsNullOrEmpty(request.CustomerSubscriptionTypeCode))
                    requireFieldFails.Add("CustomerSubscriptionTypeCode");

                if (string.IsNullOrEmpty(request.CustomerCardNo))
                    requireFieldFails.Add("CustomerCardNo");

                if (string.IsNullOrEmpty(request.AccountNo))
                    requireFieldFails.Add("AccountNo");

                if (string.IsNullOrEmpty(request.ContactSubscriptionTypeCode))
                    requireFieldFails.Add("ContactSubscriptionTypeCode");

                if (string.IsNullOrEmpty(request.ContactCardNo))
                    requireFieldFails.Add("ContactCardNo");

                if (string.IsNullOrEmpty(request.ContactAccountNo))
                    requireFieldFails.Add("ContactAccountNo");

                if (string.IsNullOrEmpty(request.ContactRelationshipName))
                    requireFieldFails.Add("ContactRelationshipName");

                if (string.IsNullOrEmpty(request.Subject))
                    requireFieldFails.Add("Subject");

                if (string.IsNullOrEmpty(request.CampaignServiceCode))
                    requireFieldFails.Add("CampaignServiceCode");

                if (request.AreaCode == 0)
                    requireFieldFails.Add("AreaCode");

                if (request.SubAreaCode == 0)
                    requireFieldFails.Add("SubAreaCode");

                if (request.TypeCode == 0)
                    requireFieldFails.Add("TypeCode");

                if (string.IsNullOrEmpty(request.ChannelCode))
                    requireFieldFails.Add("ChannelCode");

                if (string.IsNullOrEmpty(request.CreatorEmployeeCode))
                    requireFieldFails.Add("CreatorEmployeeCode");

                if (string.IsNullOrEmpty(request.OwnerEmployeeCode))
                    requireFieldFails.Add("OwnerEmployeeCode");

                if (string.IsNullOrEmpty(request.SRStatusCode))
                    requireFieldFails.Add("SRStatusCode");

                if (request.IsSendEmail ?? false)
                {
                    if (string.IsNullOrEmpty(request.SendEmailSender))
                        requireFieldFails.Add("SendEmailSender");

                    if (string.IsNullOrEmpty(request.SendEmailTo))
                        requireFieldFails.Add("SendEmailTo");

                    if (string.IsNullOrEmpty(request.SendEmailSubject))
                        requireFieldFails.Add("SendEmailSubject");

                    if (string.IsNullOrEmpty(request.SendEmailBody))
                        requireFieldFails.Add("SendEmailBody");
                }
                else
                {
                    if (string.IsNullOrEmpty(request.ActivityDescription))
                        requireFieldFails.Add("ActivityDescription");
                }

                if (request.ActivityTypeId == 0)
                    requireFieldFails.Add("ActivityTypeId");

                if (requireFieldFails.Count > 0)
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "2",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Required Fields: {{{0}}}", string.Join(",", requireFieldFails))
                    };
                }

                #endregion

                Logger.DebugFormat("Elapsed time of Validate Require Field: " + (DateTime.Now - beforeExecute));

                beforeExecute = DateTime.Now;

                #region == Validate Fix Value ==

                if (!string.IsNullOrEmpty(request.NCBCheckStatus))
                {
                    var ncbCheckStatusList = new[] { "FOUND", "NOT_FOUND" };
                    if (!ncbCheckStatusList.Contains(request.NCBCheckStatus.ToUpper(CultureInfo.InvariantCulture)))
                    {
                        return new CreateSRResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "3",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "NCBCheckStatus must be in {{{0}}}", string.Join(", ", ncbCheckStatusList))
                        };
                    }
                }

                if (request.IsSendEmail ?? false)
                {
                    if (!StringHelpers.IsValidEmail(request.SendEmailSender))
                    {
                        return new CreateSRResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "3",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "SendEmailSender is wrong format. {0}", request.SendEmailSender),
                        };
                    }

                    if (!StringHelpers.IsValidEmails(request.SendEmailTo))
                    {
                        return new CreateSRResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "3",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "SendEmailTo is wrong format. {0}", request.SendEmailTo),
                        };
                    }

                    if (!StringHelpers.IsValidEmails(request.SendEmailCc))
                    {
                        return new CreateSRResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "3",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "SendEmailCc is wrong format. {0}", request.SendEmailCc),
                        };
                    }
                }

                #endregion

                Logger.DebugFormat("Elapsed time of Validate Fix Value: " + (DateTime.Now - beforeExecute));

                beforeExecute = DateTime.Now;

                #region == Validate Code to DB ==

                var beforeValidateCode = DateTime.Now;

                _serviceRequestDataAccess = new ServiceRequestDataAccess(_context);

                int? customerId = _serviceRequestDataAccess.GetCustomerIdByCode(request.CustomerSubscriptionTypeCode, request.CustomerCardNo);
                if (customerId == null)
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found Customer (SubscriptionTypeCode={0}, CardNo{1})", request.CustomerSubscriptionTypeCode, request.CustomerCardNo),
                    };
                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetCustomerIdByCode(...) = " + (DateTime.Now - beforeValidateCode));

                beforeValidateCode = DateTime.Now;

                int? accountId = _serviceRequestDataAccess.GetAccountIdByAccountNo(customerId.Value, request.AccountNo);
                if (accountId == null)
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found Account in selected customer. (AccountNo={0}, SubscriptionTypeCode={1}, CardNo{2})", request.AccountNo, request.CustomerSubscriptionTypeCode, request.CustomerCardNo),
                    };
                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetAccountIdByAccountNo(...) = " + (DateTime.Now - beforeValidateCode));

                beforeValidateCode = DateTime.Now;

                int? contactId = _serviceRequestDataAccess.GetContactIdByCode(request.ContactSubscriptionTypeCode, request.ContactCardNo);
                if (contactId == null)
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found Contact (SubscriptionTypeCode={0}, CardNo{1})", request.ContactSubscriptionTypeCode, request.ContactCardNo),
                    };
                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetContactIdByCode(...) = " + (DateTime.Now - beforeValidateCode));

                beforeValidateCode = DateTime.Now;

                int? contactRelationshipId = _serviceRequestDataAccess.GetReleationshipIdByName(request.ContactRelationshipName);
                if (contactRelationshipId == null)
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found Relationship (RelationshipName={0})", request.ContactRelationshipName)
                    };
                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetReleationshipIdByName(...) = " + (DateTime.Now - beforeValidateCode));

                beforeValidateCode = DateTime.Now;

                int? contactAccountId = _serviceRequestDataAccess.GetAccountIdByAccountNo(customerId.Value, request.ContactAccountNo);
                if (contactAccountId == null)
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found Contact Account in selected customer. (Contact AccountNo={0}, SubscriptionTypeCode={1}, CardNo{2})", request.ContactAccountNo, request.CustomerSubscriptionTypeCode, request.CustomerCardNo),
                    };
                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetAccountIdByAccountNo(...) = " + (DateTime.Now - beforeValidateCode));

                beforeValidateCode = DateTime.Now;

                int? campaignServiceId = _serviceRequestDataAccess.GetCampaignServiceIdByCode(request.CampaignServiceCode);
                if (campaignServiceId == null)
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found CampaignService (CampaignServiceCode={0})", request.CampaignServiceCode),
                    };
                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetCampaignServiceIdByCode(...) " + (DateTime.Now - beforeValidateCode));

                beforeValidateCode = DateTime.Now;

                int? areaId = _serviceRequestDataAccess.GetAreaIdByCode(request.AreaCode);
                if (areaId == null)
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found Area (AreaCode={0})", request.AreaCode),
                    };
                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetAreaIdByCode(...) = " + (DateTime.Now - beforeValidateCode));

                beforeValidateCode = DateTime.Now;

                int? subAreaId = _serviceRequestDataAccess.GetSubAreaIdByCode(request.SubAreaCode);
                if (subAreaId == null)
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found SubArea (SubAreaCode={0})", request.SubAreaCode),
                    };
                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetSubAreaIdByCode(...) = " + (DateTime.Now - beforeValidateCode));

                beforeValidateCode = DateTime.Now;

                int? typeId = _serviceRequestDataAccess.GetTypeIdByCode(request.TypeCode);
                if (typeId == null)
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found Type (TypeCode={0})", request.TypeCode),
                    };
                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetTypeIdByCode(...) = " + (DateTime.Now - beforeValidateCode));

                beforeValidateCode = DateTime.Now;

                var mappingProduct = GetMapping(campaignServiceId.Value, areaId.Value, subAreaId.Value, typeId.Value);

                if (mappingProduct == null)
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "5",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Mapping Product-Type-Area (6 fields) is not match. (CampaignServiceCode={0},AreaCode={1},SubAreaCode={2},TypeCode={3})", request.CampaignServiceCode, request.AreaCode, request.SubAreaCode, request.TypeCode),
                    };
                }
                else
                {
                    if (mappingProduct.SrPageId == Constants.SRPage.DefaultPageId)
                    {

                    }
                    else if (mappingProduct.SrPageId == Constants.SRPage.AFSPageId)
                    {
                        if (string.IsNullOrEmpty(request.AFSAssetNo))
                        {
                            return new CreateSRResponse()
                            {
                                IsSuccess = false,
                                ErrorCode = "2",
                                ErrorMessage = "AFSAssetNo is required. (because Mapping Product-Type-Area >> SRPage = AFS)",
                            };
                        }
                    }
                    else if (mappingProduct.SrPageId == Constants.SRPage.NCBPageId)
                    {
                        if (request.NCBCustomerBirthDate == null)
                        {
                            return new CreateSRResponse()
                            {
                                IsSuccess = false,
                                ErrorCode = "2",
                                ErrorMessage = "NCBCustomerBirthDate is required. (because Mapping Product-Type-Area >> SRPage = NCB)",
                            };
                        }
                        if (string.IsNullOrEmpty(request.NCBMarketingEmployeeCode))
                        {
                            return new CreateSRResponse()
                            {
                                IsSuccess = false,
                                ErrorCode = "2",
                                ErrorMessage = "NCBMarketingEmployeeCode is required. (because Mapping Product-Type-Area >> SRPage = NCB)",
                            };
                        }
                        if (string.IsNullOrEmpty(request.NCBCheckStatus))
                        {
                            return new CreateSRResponse()
                            {
                                IsSuccess = false,
                                ErrorCode = "2",
                                ErrorMessage = "NCBCheckStatus is required. (because Mapping Product-Type-Area >> SRPage = NCB)",
                            };
                        }
                    }

                    if ((mappingProduct.IsVerify) && !string.IsNullOrEmpty(request.IsVerifyPass))
                    {
                        if (string.IsNullOrEmpty(request.IsVerifyPass))
                        {
                            return new CreateSRResponse()
                            {
                                IsSuccess = false,
                                ErrorCode = "2",
                                ErrorMessage = "IsVerifyPass is required. (because Mapping Product-Type-Area >> IsVerify = true)",
                            };
                        }

                        var verifyPassList = new[] { "PASS", "FAIL", "SKIP" };
                        if (!verifyPassList.Contains(request.IsVerifyPass.ToUpper(CultureInfo.InvariantCulture)))
                        {
                            return new CreateSRResponse()
                            {
                                IsSuccess = false,
                                ErrorCode = "3",
                                ErrorMessage = string.Format(CultureInfo.InvariantCulture, "IsVerifyPass must be in {{{0}}}", string.Join(", ", verifyPassList))
                            };
                        }
                    }

                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetMapping(...) = " + (DateTime.Now - beforeValidateCode));

                beforeValidateCode = DateTime.Now;

                int? channelId = _serviceRequestDataAccess.GetChannelIdByCode(request.ChannelCode);
                if (channelId == null)
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found Channel (ChannelCode={0})", request.ChannelCode),
                    };
                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetChannelIdByCode(...) = " + (DateTime.Now - beforeValidateCode));

                beforeValidateCode = DateTime.Now;

                int? mediaSourceId = null;
                if (!string.IsNullOrEmpty(request.MediaSourceName))
                {
                    mediaSourceId = _serviceRequestDataAccess.GetMediaSourceIdByName(request.MediaSourceName);
                    if (mediaSourceId == null)
                    {
                        return new CreateSRResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "4",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found MediaSource (MediaSourceName={0})", request.MediaSourceName),
                        };
                    }
                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetMediaSourceIdByName(...) = " + (DateTime.Now - beforeValidateCode));

                beforeValidateCode = DateTime.Now;

                var creatorUser = _serviceRequestDataAccess.GetUserByEmployeeCode(request.CreatorEmployeeCode);
                if (creatorUser == null)
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found CreatorUser (CreatorEmployeeCode={0})", request.CreatorEmployeeCode),
                    };
                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetUserByEmployeeCode(...) = " + (DateTime.Now - beforeValidateCode));

                beforeValidateCode = DateTime.Now;

                var ownerUser = _serviceRequestDataAccess.GetUserByEmployeeCode(request.OwnerEmployeeCode);
                if (ownerUser == null)
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found OwnerUser (OwnerEmployeeCode={0})", request.OwnerEmployeeCode),
                    };
                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetUserByEmployeeCode(...) for Owner = " + (DateTime.Now - beforeValidateCode));

                beforeValidateCode = DateTime.Now;

                UserEntity delegateUser = null;
                if (!string.IsNullOrEmpty(request.DelegateEmployeeCode))
                {
                    delegateUser = _serviceRequestDataAccess.GetUserByEmployeeCode(request.DelegateEmployeeCode);
                    if (delegateUser == null)
                    {
                        return new CreateSRResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "4",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found DelegateUser (DelegateEmployeeCode={0})", request.DelegateEmployeeCode),
                        };
                    }
                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetUserByEmployeeCode(...) for Delegate = " + (DateTime.Now - beforeValidateCode));

                var availableStatusForCreateSR = new[] { "DP", "OP", "CL" };
                if (!availableStatusForCreateSR.Contains(request.SRStatusCode.ToUpper(CultureInfo.InvariantCulture)))
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "IsVerifyPass must be in {{{0}}}", string.Join(", ", availableStatusForCreateSR))
                    };
                }

                //                int? srPageId = _serviceRequestDataAccess.GetSrPageIdByCode(request.SRPageCode);
                //                if (srPageId == null)
                //                {
                //                    return new CreateSRResponse()
                //                    {
                //                        IsSuccess = false,
                //                        ErrorCode = "4",
                //                        ErrorMessage = string.Format("Not found SRPage (SRPageCode={0})", request.SRPageCode),
                //                    };
                //                }


                int? afsAssetId = null;
                string afsAssetNo = null;
                string afsAssetDesc = null;

                if (!string.IsNullOrEmpty(request.AFSAssetNo))
                {
                    beforeValidateCode = DateTime.Now;

                    var afsAssetEntity = _serviceRequestDataAccess.GetAssetInfo(request.AFSAssetNo);
                    if (afsAssetEntity == null)
                    {
                        return new CreateSRResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "4",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found AFSAsset (AFSAssetNo={0})", request.AFSAssetNo),
                        };
                    }
                    else
                    {
                        var assetInfoTemplate =
                            "ประเภททรัพย์: {0}\n" +
                            "สถานะ: {1}\n" +
                            "ที่ตั้ง: {2} {3}\n" +
                            "Sale: {4},{5},{6},{7}";

                        afsAssetId = afsAssetEntity.AssetId;
                        afsAssetNo = afsAssetEntity.AssetNo;
                        afsAssetDesc = string.Format(CultureInfo.InvariantCulture, assetInfoTemplate,
                            afsAssetEntity.ProjectDes,
                            afsAssetEntity.StatusDesc,
                            afsAssetEntity.Amphur,
                            afsAssetEntity.Province,
                            afsAssetEntity.SaleName,
                            afsAssetEntity.PhoneNo,
                            afsAssetEntity.MobileNo,
                            afsAssetEntity.Email);
                    }

                    Logger.DebugFormat("Elapsed time of Validate Code: GetAssetInfo(...) = " + (DateTime.Now - beforeValidateCode));
                }

                beforeValidateCode = DateTime.Now;

                int? ncbMarketingUserId = null;
                if (!string.IsNullOrEmpty(request.NCBMarketingEmployeeCode))
                {
                    ncbMarketingUserId = _serviceRequestDataAccess.GetUserIdByEmployeeCode(request.NCBMarketingEmployeeCode);
                    if (ncbMarketingUserId == null)
                    {
                        return new CreateSRResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "4",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found NCBMarketingUser (NCBMarketingEmployeeCode={0})", request.NCBMarketingEmployeeCode),
                        };
                    }
                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetUserIdByEmployeeCode(...) for NCB Marketging = " + (DateTime.Now - beforeValidateCode));

                beforeValidateCode = DateTime.Now;

                var activityType = _serviceRequestDataAccess.GetActivityType(request.ActivityTypeId);
                if (activityType == null)
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found ActivityType (ActivityTypeId={0})", request.ActivityTypeId),
                    };
                }

                Logger.DebugFormat("Elapsed time of Validate Code: GetActivityType(...) = " + (DateTime.Now - beforeValidateCode));

                #endregion

                Logger.DebugFormat("Elapsed time of Total Validate Code to DB: = " + (DateTime.Now - beforeExecute));

                beforeExecute = DateTime.Now;

                #region == Create DTO Object for Create SR ==

                var sr = new ServiceRequestForSaveEntity();

                sr.SrId = null;

                sr.CallId = request.CallID;
                sr.PhoneNo = request.ANo;

                sr.CustomerId = customerId.Value;
                sr.AccountId = accountId.Value;
                sr.ContactId = contactId.Value;

                sr.ContactAccountNo = request.ContactAccountNo;
                sr.ContactRelationshipId = contactRelationshipId.Value;
                sr.ProductGroupId = mappingProduct.ProductGroupId;
                sr.ProductId = mappingProduct.ProductId;
                sr.CampaignServiceId = campaignServiceId.Value;
                sr.AreaId = areaId.Value;
                sr.SubAreaId = subAreaId.Value;
                sr.TypeId = typeId.Value;
                sr.ChannelId = channelId.Value;
                sr.MediaSourceId = mediaSourceId;
                sr.Subject = request.Subject;
                sr.Remark = request.Remark ?? string.Empty;

                sr.MappingProductId = mappingProduct.MappingProductId;
                sr.IsVerify = mappingProduct.IsVerify;
                if (sr.IsVerify)
                {
                    sr.IsVerifyPass = request.IsVerifyPass ?? string.Empty;
                }

                sr.SrPageId = mappingProduct.SrPageId;

                if (sr.SrPageId == 1)
                {
                    sr.AddressSendDocId = null;
                    sr.AddressSendDocText = request.DefaultAmphur;
                }
                else if (sr.SrPageId == 2)
                {
                    sr.AfsAssetId = afsAssetId;
                    sr.AfsAssetNo = afsAssetNo ?? string.Empty;
                    sr.AfsAssetDesc = afsAssetDesc ?? string.Empty;
                }
                else if (sr.SrPageId == 3)
                {
                    sr.NCBBirthDate = request.NCBCustomerBirthDate;
                    sr.NCBCheckStatus = request.NCBCheckStatus;

                    sr.NCBMarketingUserId = ncbMarketingUserId;

                    var userMarketingEntity = GetUserMarketing(ncbMarketingUserId.Value);
                    if (userMarketingEntity != null)
                    {
                        if (userMarketingEntity.UserEntity != null)
                        {
                            sr.NCBMarketingUserId = userMarketingEntity.UserEntity.UserId;
                            sr.NCBMarketingName = userMarketingEntity.UserEntity.FullName;
                        }

                        if (userMarketingEntity.BranchEntity != null)
                        {
                            sr.NCBMarketingBranchId = userMarketingEntity.BranchEntity.BranchId;
                            sr.NCBMarketingBranchName = userMarketingEntity.BranchEntity.BranchName;
                        }

                        if (userMarketingEntity.UpperBranch1 != null)
                        {
                            sr.NCBMarketingBranchUpper1Id = userMarketingEntity.UpperBranch1.BranchId;
                            sr.NCBMarketingBranchUpper1Name = userMarketingEntity.UpperBranch1.BranchName;
                        }

                        if (userMarketingEntity.UpperBranch2 != null)
                        {
                            sr.NCBMarketingBranchUpper2Id = userMarketingEntity.UpperBranch2.BranchId;
                            sr.NCBMarketingBranchUpper2Name = userMarketingEntity.UpperBranch2.BranchName;
                        }
                    }
                }

                sr.OwnerBranchId = ownerUser.BranchId ?? 0;
                sr.OwnerUserId = ownerUser.UserId;

                if (delegateUser != null)
                {
                    sr.DelegateBranchId = delegateUser.BranchId;
                    sr.DelegateUserId = delegateUser.UserId;
                }
                else
                {
                    sr.DelegateBranchId = null;
                    sr.DelegateUserId = null;
                }

                sr.SrEmailTemplateId = (request.IsSendEmail ?? false) ? 1 : (int?)null; // Set to 1 for marking flag as sent mail but not use for apply template.

                if (sr.SrEmailTemplateId == null)
                {
                    sr.ActivityDescription = request.ActivityDescription;
                }
                else
                {
                    sr.SendMailSender = request.SendEmailSender;
                    sr.SendMailTo = request.SendEmailTo;
                    sr.SendMailCc = request.SendEmailCc;
                    sr.SendMailSubject = request.SendEmailSubject;
                    sr.SendMailBody = request.SendEmailBody;
                }

                sr.ActivityTypeId = request.ActivityTypeId;
                sr.IsEmailDelegate = request.IsSendDelegateEmail ?? true;
                sr.IsClose = request.SRStatusCode == Constants.SRStatusCode.Closed;

                //sr.AttachmentJson = model.AttachmentJson;

                sr.CreateBranchId = creatorUser.BranchId;
                sr.CreateUserId = creatorUser.UserId;

                var auditLog = new AuditLogEntity();
                auditLog.Module = Constants.Module.ServiceRequest;
                auditLog.Action = Constants.AuditAction.ActivityLog;
                auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                auditLog.CreateUserId = creatorUser.UserId;

                #endregion

                var result = CreateServiceRequest(auditLog, sr, false);

                Logger.DebugFormat("Elapsed time of Save Service Request: " + (DateTime.Now - beforeExecute));

                if (result.IsSuccess)
                {
                    return new CreateSRResponse
                    {
                        IsSuccess = true,
                        ErrorCode = "",
                        ErrorMessage = "",
                        SRNo = result.SrNo,
                    };
                }
                else
                {
                    return new CreateSRResponse
                    {
                        IsSuccess = false,
                        ErrorCode = result.ErrorCode,
                        ErrorMessage = result.ErrorMessage,
                        SRNo = result.SrNo,
                    };
                }

            }
            catch (Exception ex)
            {
                return new CreateSRResponse
                {
                    IsSuccess = false,
                    ErrorCode = "1",
                    ErrorMessage = ex.Message
                };
            }
        }


        public CreateSRResponse CreateSRWebService(CreateSRRequest request)
        {
            try
            {
                Logger.DebugFormat("Start Create SR Web Service");

                var beforeExecute = DateTime.Now;

                #region == Validate Require Field ==

                var requireFieldFails = new List<string>();

                if (string.IsNullOrEmpty(request.CustomerSubscriptionTypeCode))
                    requireFieldFails.Add("CustomerSubscriptionTypeCode");

                if (string.IsNullOrEmpty(request.CustomerCardNo))
                    requireFieldFails.Add("CustomerCardNo");

                if (string.IsNullOrEmpty(request.AccountNo))
                    requireFieldFails.Add("AccountNo");

                if (string.IsNullOrEmpty(request.ContactSubscriptionTypeCode))
                    requireFieldFails.Add("ContactSubscriptionTypeCode");

                if (string.IsNullOrEmpty(request.ContactCardNo))
                    requireFieldFails.Add("ContactCardNo");

                if (string.IsNullOrEmpty(request.ContactAccountNo))
                    requireFieldFails.Add("ContactAccountNo");

                if (string.IsNullOrEmpty(request.ContactRelationshipName))
                    requireFieldFails.Add("ContactRelationshipName");

                if (string.IsNullOrEmpty(request.Subject))
                    requireFieldFails.Add("Subject");

                if (string.IsNullOrEmpty(request.CampaignServiceCode))
                    requireFieldFails.Add("CampaignServiceCode");

                if (request.AreaCode == 0)
                    requireFieldFails.Add("AreaCode");

                if (request.SubAreaCode == 0)
                    requireFieldFails.Add("SubAreaCode");

                if (request.TypeCode == 0)
                    requireFieldFails.Add("TypeCode");

                if (string.IsNullOrEmpty(request.ChannelCode))
                    requireFieldFails.Add("ChannelCode");

                if (string.IsNullOrEmpty(request.CreatorEmployeeCode))
                    requireFieldFails.Add("CreatorEmployeeCode");

                if (string.IsNullOrEmpty(request.OwnerEmployeeCode))
                    requireFieldFails.Add("OwnerEmployeeCode");

                if (string.IsNullOrEmpty(request.SRStatusCode))
                    requireFieldFails.Add("SRStatusCode");

                if (request.IsSendEmail ?? false)
                {
                    if (string.IsNullOrEmpty(request.SendEmailSender))
                        requireFieldFails.Add("SendEmailSender");

                    if (string.IsNullOrEmpty(request.SendEmailTo))
                        requireFieldFails.Add("SendEmailTo");

                    if (string.IsNullOrEmpty(request.SendEmailSubject))
                        requireFieldFails.Add("SendEmailSubject");

                    if (string.IsNullOrEmpty(request.SendEmailBody))
                        requireFieldFails.Add("SendEmailBody");
                }
                else
                {
                    if (string.IsNullOrEmpty(request.ActivityDescription))
                        requireFieldFails.Add("ActivityDescription");
                }

                if (request.ActivityTypeId == 0)
                    requireFieldFails.Add("ActivityTypeId");

                if (requireFieldFails.Count > 0)
                {
                    return new CreateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "2",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Required Fields: {{{0}}}", string.Join(",", requireFieldFails))
                    };
                }

                #endregion

                Logger.DebugFormat("Elapsed time of Validate Require Field: " + (DateTime.Now - beforeExecute));

                beforeExecute = DateTime.Now;

                #region == Validate Fix Value ==

                if (!string.IsNullOrEmpty(request.NCBCheckStatus))
                {
                    var ncbCheckStatusList = new[] { "FOUND", "NOT_FOUND" };
                    if (!ncbCheckStatusList.Contains(request.NCBCheckStatus.ToUpper(CultureInfo.InvariantCulture)))
                    {
                        return new CreateSRResponse
                        {
                            IsSuccess = false,
                            ErrorCode = "3",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "NCBCheckStatus must be in {{{0}}}", string.Join(", ", ncbCheckStatusList))
                        };
                    }
                }

                if (request.IsSendEmail ?? false)
                {
                    if (!StringHelpers.IsValidEmail(request.SendEmailSender))
                    {
                        return new CreateSRResponse
                        {
                            IsSuccess = false,
                            ErrorCode = "3",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "SendEmailSender is wrong format. {0}", request.SendEmailSender),
                        };
                    }

                    if (!StringHelpers.IsValidEmails(request.SendEmailTo))
                    {
                        return new CreateSRResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "3",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "SendEmailTo is wrong format. {0}", request.SendEmailTo),
                        };
                    }

                    if (!StringHelpers.IsValidEmails(request.SendEmailCc))
                    {
                        return new CreateSRResponse
                        {
                            IsSuccess = false,
                            ErrorCode = "3",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "SendEmailCc is wrong format. {0}", request.SendEmailCc),
                        };
                    }
                }

                #endregion

                Logger.DebugFormat("Elapsed time of Validate Fix Value: " + (DateTime.Now - beforeExecute));

                beforeExecute = DateTime.Now;

                #region == Validate Code to DB ==

                _serviceRequestDataAccess = new ServiceRequestDataAccess(_context);
                var validateResult = _serviceRequestDataAccess.ValidateCreateSR(request);

                if (!string.IsNullOrWhiteSpace(validateResult.ErrorCode))
                {
                    string errorCode = string.Empty;
                    string errorMessage = string.Empty;

                    // STEP 1: Get CustomerID by Code
                    if ("ERR001".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found Customer (SubscriptionTypeCode={0}, CardNo{1})",
                            request.CustomerSubscriptionTypeCode, request.CustomerCardNo);
                    }
                    // STEP 2: Get AccountId by AccountNo
                    if ("ERR002".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found Account in selected customer. (AccountNo={0}, SubscriptionTypeCode={1}, CardNo{2})",
                            request.AccountNo, request.CustomerSubscriptionTypeCode, request.CustomerCardNo);
                    }
                    // STEP 3: Get ContactID by Code
                    if ("ERR003".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found Contact (SubscriptionTypeCode={0}, CardNo{1})", request.ContactSubscriptionTypeCode, request.ContactCardNo);
                    }
                    // STEP 4: Get RelationshipID by Name
                    if ("ERR004".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found Relationship (RelationshipName={0})", request.ContactRelationshipName);
                    }
                    // STEP 5: Get AccountID by AccountNo
                    if ("ERR005".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found Contact Account in selected customer. (Contact AccountNo={0}, SubscriptionTypeCode={1}, CardNo{2})", request.ContactAccountNo, request.CustomerSubscriptionTypeCode, request.CustomerCardNo);
                    }
                    // STEP 6: Get CampaignServiceID by Code
                    if ("ERR006".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found CampaignService (CampaignServiceCode={0})", request.CampaignServiceCode);
                    }
                    // Get AreaID by Code
                    if ("ERR007".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found Area (AreaCode={0})", request.AreaCode);
                    }
                    // STEP 8: Get SubAreaID by Code
                    if ("ERR008".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found SubArea (SubAreaCode={0})", request.SubAreaCode);
                    }
                    // STEP 9: Get TypeID by Code
                    if ("ERR009".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found Type (TypeCode={0})", request.TypeCode);
                    }
                    // STEP 10: Get ChannelID by Code
                    if ("ERR010".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found Channel (ChannelCode={0})", request.ChannelCode);
                    }
                    // STEP 11: Get MediaSourceID by Name
                    if ("ERR011".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found MediaSource (MediaSourceName={0})", request.MediaSourceName);
                    }
                    // STEP 12: Get Creator by EmployeeCode
                    if ("ERR012".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found CreatorUser (CreatorEmployeeCode={0})", request.CreatorEmployeeCode);
                    }
                    // STEP 13: Get Owner by EmployeeCode
                    if ("ERR013".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found OwnerUser (OwnerEmployeeCode={0})", request.OwnerEmployeeCode);
                    }
                    // STEP 14: Get Delegate by EmployeeCode
                    if ("ERR014".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found DelegateUser (DelegateEmployeeCode={0})", request.DelegateEmployeeCode);
                    }
                    // STEP 15: Get AFS AssetNo
                    if ("ERR015".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found AFSAsset (AFSAssetNo={0})", request.AFSAssetNo);
                    }
                    // STEP 16: Get NCB Marketing by EmployeeCode
                    if ("ERR016".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found NCBMarketingUser (NCBMarketingEmployeeCode={0})", request.NCBMarketingEmployeeCode);
                    }
                    // STEP 17: Get ActivityType By ID
                    if ("ERR017".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Not found ActivityType (ActivityTypeId={0})", request.ActivityTypeId);
                    }
                    // STEP 18: Get Mapping by Campaign
                    if ("ERR018".Equals(validateResult.ErrorCode))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "Mapping Product-Type-Area (6 fields) is not match. (CampaignServiceCode={0},AreaCode={1},SubAreaCode={2},TypeCode={3})",
                            request.CampaignServiceCode, request.AreaCode, request.SubAreaCode, request.TypeCode);
                    }

                    var availableStatusForCreateSR = new[] { "DP", "OP", "CL" };
                    if (!availableStatusForCreateSR.Contains(request.SRStatusCode.ToUpper(CultureInfo.InvariantCulture)))
                    {
                        errorCode = "4";
                        errorMessage = string.Format(CultureInfo.InvariantCulture, "IsVerifyPass must be in {{{0}}}",
                            string.Join(", ", availableStatusForCreateSR));
                    }

                    if (validateResult.SrPageId == Constants.SRPage.AFSPageId)
                    {
                        if (string.IsNullOrEmpty(request.AFSAssetNo))
                        {
                            errorCode = "2";
                            errorMessage = "AFSAssetNo is required. (because Mapping Product-Type-Area >> SRPage = AFS)";
                        }
                    }
                    if (validateResult.SrPageId == Constants.SRPage.NCBPageId)
                    {
                        if (request.NCBCustomerBirthDate == null)
                        {
                            errorCode = "2";
                            errorMessage = "NCBCustomerBirthDate is required. (because Mapping Product-Type-Area >> SRPage = NCB)";
                        }
                        if (string.IsNullOrEmpty(request.NCBMarketingEmployeeCode))
                        {
                            errorCode = "2";
                            errorMessage = "NCBMarketingEmployeeCode is required. (because Mapping Product-Type-Area >> SRPage = NCB)";
                        }
                        if (string.IsNullOrEmpty(request.NCBCheckStatus))
                        {
                            errorCode = "2";
                            errorMessage = "NCBCheckStatus is required. (because Mapping Product-Type-Area >> SRPage = NCB)";
                        }
                    }

                    if (validateResult.IsVerify && !string.IsNullOrEmpty(request.IsVerifyPass))
                    {
                        if (string.IsNullOrEmpty(request.IsVerifyPass))
                        {
                            errorCode = "2";
                            errorMessage = "IsVerifyPass is required. (because Mapping Product-Type-Area >> IsVerify = true)";
                        }

                        var verifyPassList = new[] { "PASS", "FAIL", "SKIP" };
                        if (!verifyPassList.Contains(request.IsVerifyPass.ToUpper(CultureInfo.InvariantCulture)))
                        {
                            errorCode = "3";
                            errorMessage = string.Format(CultureInfo.InvariantCulture, "IsVerifyPass must be in {{{0}}}", string.Join(", ", verifyPassList));
                        }
                    }

                    return new CreateSRResponse
                    {
                        IsSuccess = false,
                        ErrorCode = errorCode,
                        ErrorMessage = errorMessage
                    };
                }

                #endregion

                Logger.DebugFormat("Elapsed time of Total Validate Code to DB: = " + (DateTime.Now - beforeExecute));

                beforeExecute = DateTime.Now;

                #region == Create DTO Object for Create SR ==

                var sr = new ServiceRequestForSaveEntity();

                sr.SrId = null;

                sr.CallId = request.CallID;
                sr.PhoneNo = request.ANo;

                sr.CustomerId = validateResult.CustomerId;
                sr.AccountId = validateResult.AccountId;
                sr.ContactId = validateResult.ContactId;

                sr.ContactAccountNo = request.ContactAccountNo;
                sr.ContactRelationshipId = validateResult.ContactRelationshipId;
                sr.ProductGroupId = validateResult.ProductGroupId;
                sr.ProductId = validateResult.ProductId;
                sr.CampaignServiceId = validateResult.CampaignServiceId;
                sr.AreaId = validateResult.AreaId;
                sr.SubAreaId = validateResult.SubAreaId;
                sr.TypeId = validateResult.TypeId;
                sr.ChannelId = validateResult.ChannelId;
                sr.MediaSourceId = validateResult.MediaSourceId == -1 ? (int?)null : validateResult.MediaSourceId;
                sr.Subject = request.Subject;
                sr.Remark = request.Remark ?? string.Empty;

                sr.MappingProductId = validateResult.MapProductId;
                sr.IsVerify = validateResult.IsVerify;
                if (sr.IsVerify)
                {
                    sr.IsVerifyPass = request.IsVerifyPass ?? string.Empty;
                }

                sr.SrPageId = validateResult.SrPageId;

                if (sr.SrPageId == 1)
                {
                    sr.AddressSendDocId = null;
                    sr.AddressSendDocText = request.DefaultAmphur;
                }
                else if (sr.SrPageId == 2)
                {
                    sr.AfsAssetId = validateResult.AfsAssetId;
                    sr.AfsAssetNo = validateResult.AfsAssetNo;
                    sr.AfsAssetDesc = validateResult.AfsAssetDesc;
                }
                else if (sr.SrPageId == 3)
                {
                    sr.NCBBirthDate = request.NCBCustomerBirthDate;
                    sr.NCBCheckStatus = request.NCBCheckStatus;

                    sr.NCBMarketingUserId = validateResult.NcbMarketingUserId;

                    var userMarketingEntity = GetUserMarketing(validateResult.NcbMarketingUserId);
                    if (userMarketingEntity != null)
                    {
                        if (userMarketingEntity.UserEntity != null)
                        {
                            sr.NCBMarketingUserId = userMarketingEntity.UserEntity.UserId;
                            sr.NCBMarketingName = userMarketingEntity.UserEntity.FullName;
                        }

                        if (userMarketingEntity.BranchEntity != null)
                        {
                            sr.NCBMarketingBranchId = userMarketingEntity.BranchEntity.BranchId;
                            sr.NCBMarketingBranchName = userMarketingEntity.BranchEntity.BranchName;
                        }

                        if (userMarketingEntity.UpperBranch1 != null)
                        {
                            sr.NCBMarketingBranchUpper1Id = userMarketingEntity.UpperBranch1.BranchId;
                            sr.NCBMarketingBranchUpper1Name = userMarketingEntity.UpperBranch1.BranchName;
                        }

                        if (userMarketingEntity.UpperBranch2 != null)
                        {
                            sr.NCBMarketingBranchUpper2Id = userMarketingEntity.UpperBranch2.BranchId;
                            sr.NCBMarketingBranchUpper2Name = userMarketingEntity.UpperBranch2.BranchName;
                        }
                    }
                }

                sr.OwnerBranchId = validateResult.OwnerBranchId; // ?? 0;
                sr.OwnerUserId = validateResult.OwnerUserId;

                if (validateResult.DelegateUserId != -1)
                {
                    sr.DelegateBranchId = validateResult.DelegateBranchId;
                    sr.DelegateUserId = validateResult.DelegateUserId;
                }
                else
                {
                    sr.DelegateBranchId = null;
                    sr.DelegateUserId = null;
                }

                sr.SrEmailTemplateId = (request.IsSendEmail ?? false) ? 1 : (int?)null; // Set to 1 for marking flag as sent mail but not use for apply template.

                if (sr.SrEmailTemplateId == null)
                {
                    sr.ActivityDescription = request.ActivityDescription;
                }
                else
                {
                    sr.SendMailSender = request.SendEmailSender;
                    sr.SendMailTo = request.SendEmailTo;
                    sr.SendMailCc = request.SendEmailCc;
                    sr.SendMailSubject = request.SendEmailSubject;
                    sr.SendMailBody = request.SendEmailBody;
                }

                sr.ActivityTypeId = request.ActivityTypeId;
                sr.IsEmailDelegate = request.IsSendDelegateEmail ?? true;
                sr.IsClose = request.SRStatusCode == Constants.SRStatusCode.Closed;

                //sr.AttachmentJson = model.AttachmentJson;

                sr.CreateBranchId = validateResult.CreatorBranchId;
                sr.CreateUserId = validateResult.CreatorUserId;

                var auditLog = new AuditLogEntity();
                auditLog.Module = Constants.Module.ServiceRequest;
                auditLog.Action = Constants.AuditAction.ActivityLog;
                auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                auditLog.CreateUserId = validateResult.CreatorUserId;

                #endregion

                var result = CreateServiceRequest(auditLog, sr, false);

                Logger.DebugFormat("Elapsed time of Save Service Request: " + (DateTime.Now - beforeExecute));

                if (result.IsSuccess)
                {
                    return new CreateSRResponse
                    {
                        IsSuccess = true,
                        ErrorCode = "",
                        ErrorMessage = "",
                        SRNo = result.SrNo,
                    };
                }
                else
                {
                    return new CreateSRResponse
                    {
                        IsSuccess = false,
                        ErrorCode = result.ErrorCode,
                        ErrorMessage = result.ErrorMessage,
                        SRNo = result.SrNo,
                    };
                }

            }
            catch (Exception ex)
            {
                return new CreateSRResponse
                {
                    IsSuccess = false,
                    ErrorCode = "1",
                    ErrorMessage = ex.Message
                };
            }
        }

        public UpdateSRResponse UpdateSRWebService(UpdateSRRequest request)
        {
            try
            {
                #region == Validate Require Field ==

                var requireFieldFails = new List<string>();

                if (string.IsNullOrEmpty(request.UpdateByEmployeeCode))
                    requireFieldFails.Add("UpdateByEmployeeCode");

                if (string.IsNullOrEmpty(request.OwnerEmployeeCode))
                    requireFieldFails.Add("OwnerEmployeeCode");

                if (string.IsNullOrEmpty(request.SRStatusCode))
                    requireFieldFails.Add("SRStatusCode");

                if (request.IsSendEmail ?? false)
                {
                    if (string.IsNullOrEmpty(request.SendEmailSender))
                        requireFieldFails.Add("SendEmailSender");

                    if (string.IsNullOrEmpty(request.SendEmailTo))
                        requireFieldFails.Add("SendEmailTo");

                    if (string.IsNullOrEmpty(request.SendEmailSubject))
                        requireFieldFails.Add("SendEmailSubject");

                    if (string.IsNullOrEmpty(request.SendEmailBody))
                        requireFieldFails.Add("SendEmailBody");
                }
                else
                {
                    if (string.IsNullOrEmpty(request.ActivityDescription))
                        requireFieldFails.Add("ActivityDescription");
                }

                if (request.ActivityTypeId == 0)
                    requireFieldFails.Add("ActivityTypeId");

                if (requireFieldFails.Count > 0)
                {
                    return new UpdateSRResponse
                    {
                        IsSuccess = false,
                        ErrorCode = "2",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Required Fields: {{{0}}}", string.Join(",", requireFieldFails))
                    };
                }

                #endregion

                #region == Validate Fix Value ==

                if (!string.IsNullOrEmpty(request.NCBCheckStatus))
                {
                    var ncbCheckStatusList = new[] { "FOUND", "NOT_FOUND" };
                    if (!ncbCheckStatusList.Contains(request.NCBCheckStatus.ToUpper(CultureInfo.InvariantCulture)))
                    {
                        return new UpdateSRResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "3",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "NCBCheckStatus must be in {{{0}}}", string.Join(", ", ncbCheckStatusList))
                        };
                    }
                }

                if (request.IsSendEmail ?? false)
                {
                    if (!StringHelpers.IsValidEmail(request.SendEmailSender))
                    {
                        return new UpdateSRResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "3",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "SendEmailSender is wrong format. {0}", request.SendEmailSender),
                        };
                    }

                    if (!StringHelpers.IsValidEmails(request.SendEmailTo))
                    {
                        return new UpdateSRResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "3",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "SendEmailTo is wrong format. {0}", request.SendEmailTo),
                        };
                    }

                    if (!StringHelpers.IsValidEmails(request.SendEmailCc))
                    {
                        return new UpdateSRResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "3",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "SendEmailCc is wrong format. {0}", request.SendEmailCc),
                        };
                    }
                }

                #endregion


                #region == Validate Code to DB ==

                _serviceRequestDataAccess = new ServiceRequestDataAccess(_context);

                var serviceRequest = _serviceRequestDataAccess.GetServiceRequest(request.SRNo);
                if (serviceRequest == null)
                {
                    return new UpdateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found SR (SRNo={0})", request.SRNo),
                    };
                }

                if (request.IsUpdateInfo ?? false)
                {
                    if (string.IsNullOrEmpty(request.Subject))
                    {
                        return new UpdateSRResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "2",
                            ErrorMessage = "Subject is required. (because IsUpdateInfo is TRUE)",
                        };
                    }

                    if (serviceRequest.SrPageId == Constants.SRPage.DefaultPageId)
                    {

                    }
                    else if (serviceRequest.SrPageId == Constants.SRPage.AFSPageId)
                    {
                        //                        if (string.IsNullOrEmpty(request.AFSAssetNo))
                        //                        {
                        //                            return new UpdateSRResponse()
                        //                            {
                        //                                IsSuccess = false,
                        //                                ErrorCode = "2",
                        //                                ErrorMessage = "AFSAssetNo is required. (because Mapping Product-Type-Area >> SRPage = AFS)",
                        //                            };
                        //                        }
                    }
                    else if (serviceRequest.SrPageId == Constants.SRPage.NCBPageId)
                    {
                        if (request.NCBCustomerBirthDate == null)
                        {
                            return new UpdateSRResponse()
                            {
                                IsSuccess = false,
                                ErrorCode = "2",
                                ErrorMessage = "NCBCustomerBirthDate is required. (because Mapping Product-Type-Area >> SRPage = NCB)",
                            };
                        }
                        if (string.IsNullOrEmpty(request.NCBMarketingEmployeeCode))
                        {
                            return new UpdateSRResponse()
                            {
                                IsSuccess = false,
                                ErrorCode = "2",
                                ErrorMessage = "NCBMarketingEmployeeCode is required. (because Mapping Product-Type-Area >> SRPage = NCB)",
                            };
                        }
                        if (string.IsNullOrEmpty(request.NCBCheckStatus))
                        {
                            return new UpdateSRResponse()
                            {
                                IsSuccess = false,
                                ErrorCode = "2",
                                ErrorMessage = "NCBCheckStatus is required. (because Mapping Product-Type-Area >> SRPage = NCB)",
                            };
                        }
                    }
                }

                var updateByUser = _serviceRequestDataAccess.GetUserByEmployeeCode(request.UpdateByEmployeeCode);
                if (updateByUser == null)
                {
                    return new UpdateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found UpdateByUser (UpdateByEmployeeCode={0})", request.UpdateByEmployeeCode),
                    };
                }

                var ownerUser = _serviceRequestDataAccess.GetUserByEmployeeCode(request.OwnerEmployeeCode);
                if (ownerUser == null)
                {
                    return new UpdateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found OwnerUser (OwnerEmployeeCode={0})", request.OwnerEmployeeCode),
                    };
                }

                UserEntity delegateUser = null;
                if (!string.IsNullOrEmpty(request.DelegateEmployeeCode))
                {
                    delegateUser = _serviceRequestDataAccess.GetUserByEmployeeCode(request.DelegateEmployeeCode);
                    if (delegateUser == null)
                    {
                        return new UpdateSRResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "4",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found DelegateUser (DelegateEmployeeCode={0})", request.DelegateEmployeeCode),
                        };
                    }
                }

                int? newSrStatusId = null;
                if (!string.IsNullOrEmpty(request.SRStatusCode))
                {
                    newSrStatusId = _serviceRequestDataAccess.GetSrStatusIdByCode(request.SRStatusCode);
                    if (newSrStatusId == null)
                    {
                        return new UpdateSRResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "4",
                            ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found SRStatus (SRStatusCode={0})", request.SRStatusCode),
                        };
                    }
                }

                if (!_serviceRequestDataAccess.CanChangeStatus(serviceRequest.SRStatus.SRStatusId, newSrStatusId.Value,
                    serviceRequest.ProductGroup.ProductGroupId ?? 0,
                    serviceRequest.Product.ProductId ?? 0,
                    serviceRequest.CampaignService.CampaignServiceId ?? 0,
                    serviceRequest.Area.AreaId ?? 0,
                    serviceRequest.SubArea.SubareaId ?? 0,
                    serviceRequest.Type.TypeId ?? 0))
                {
                    return new UpdateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "6",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Cannot change status to {0}", request.SRStatusCode),
                    };
                }

                int? afsAssetId = null;
                string afsAssetNo = null;
                string afsAssetDesc = null;

                if (request.IsUpdateInfo ?? false)
                {
                    //                    if (!string.IsNullOrEmpty(request.AFSAssetNo))
                    //                    {
                    //                        var afsAssetEntity = _serviceRequestDataAccess.GetAssetInfo(request.AFSAssetNo);
                    //                        if (afsAssetEntity == null)
                    //                        {
                    //                            return new UpdateSRResponse()
                    //                            {
                    //                                IsSuccess = false,
                    //                                ErrorCode = "4",
                    //                                ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found AFSAsset (AFSAssetNo={0})", request.AFSAssetNo),
                    //                            };
                    //                        }
                    //                        else
                    //                        {
                    //                            var assetInfoTemplate =
                    //                                "ประเภททรัพย์: {0}\n" +
                    //                                "สถานะ: {1}\n" +
                    //                                "ที่ตั้ง: {2} {3}\n" +
                    //                                "Sale: {4},{5},{6},{7}";
                    //
                    //                            afsAssetId = afsAssetEntity.AssetId;
                    //                            afsAssetNo = afsAssetEntity.AssetNo;
                    //                            afsAssetDesc = string.Format(CultureInfo.InvariantCulture, assetInfoTemplate,
                    //                                afsAssetEntity.ProjectDes,
                    //                                afsAssetEntity.StatusDesc,
                    //                                afsAssetEntity.Amphur,
                    //                                afsAssetEntity.Province,
                    //                                afsAssetEntity.SaleName,
                    //                                afsAssetEntity.PhoneNo,
                    //                                afsAssetEntity.MobileNo,
                    //                                afsAssetEntity.Email);
                    //                        }
                    //                    }

                    int? ncbMarketingUserId = null;
                    if (!string.IsNullOrEmpty(request.NCBMarketingEmployeeCode))
                    {
                        ncbMarketingUserId = _serviceRequestDataAccess.GetUserIdByEmployeeCode(request.NCBMarketingEmployeeCode);
                        if (ncbMarketingUserId == null)
                        {
                            return new UpdateSRResponse()
                            {
                                IsSuccess = false,
                                ErrorCode = "4",
                                ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found NCBMarketingUser (NCBMarketingEmployeeCode={0})", request.NCBMarketingEmployeeCode),
                            };
                        }
                    }
                }

                var activityType = _serviceRequestDataAccess.GetActivityType(request.ActivityTypeId);
                if (activityType == null)
                {
                    return new UpdateSRResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = string.Format(CultureInfo.InvariantCulture, "Not found ActivityType (ActivityTypeId={0})", request.ActivityTypeId),
                    };
                }

                #endregion


                #region == Create DTO Object for Create SR ==

                var sr = new ServiceRequestForSaveEntity();

                sr.SrId = serviceRequest.SrId;

                sr.OwnerBranchId = ownerUser.BranchId ?? 0;
                sr.OwnerUserId = ownerUser.UserId;

                if (!string.IsNullOrEmpty(request.DelegateEmployeeCode) && delegateUser != null)
                {
                    sr.DelegateBranchId = delegateUser.BranchId;
                    sr.DelegateUserId = delegateUser.UserId;
                }

                sr.SrStatusId = newSrStatusId.Value;
                sr.IsEmailDelegate = request.IsSendDelegateEmail ?? true;

                if (request.IsSendEmail ?? false)
                    sr.SrEmailTemplateId = 1;
                else
                    sr.SrEmailTemplateId = null;

                if (sr.SrEmailTemplateId == null)
                {
                    sr.ActivityDescription = request.ActivityDescription;
                }
                else
                {
                    sr.SendMailSender = request.SendEmailSender;
                    sr.SendMailTo = request.SendEmailTo;
                    sr.SendMailCc = request.SendEmailCc;
                    sr.SendMailSubject = request.SendEmailSubject;
                    sr.SendMailBody = request.SendEmailBody;
                }

                sr.ActivityTypeId = request.ActivityTypeId;

                sr.CreateBranchId = updateByUser.BranchId;
                sr.CreateUserId = updateByUser.UserId;

                var _auditLog = new AuditLogEntity();
                _auditLog.Module = Constants.Module.ServiceRequest;
                _auditLog.Action = Constants.AuditAction.ActivityLog;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.CreateUserId = updateByUser.UserId;

                #endregion

                var result = CreateServiceRequestActivity(_auditLog, sr, false);

                if (result.IsSuccess)
                {
                    if (request.IsUpdateInfo ?? false)
                    {
                        _serviceRequestDataAccess.UpdateServiceRequestWebService(serviceRequest.SrId, request);
                    }

                    return new UpdateSRResponse
                    {
                        IsSuccess = true,
                        ErrorCode = "",
                        ErrorMessage = "",
                        SRNo = request.SRNo,
                    };
                }
                else
                {
                    return new UpdateSRResponse
                    {
                        IsSuccess = false,
                        ErrorCode = result.ErrorCode,
                        ErrorMessage = result.ErrorMessage,
                        SRNo = request.SRNo,
                    };
                }

            }
            catch (Exception ex)
            {
                return new UpdateSRResponse
                {
                    IsSuccess = false,
                    ErrorCode = "1",
                    ErrorMessage = ex.Message
                };
            }
        }

        public SearchSRResponse SearchSRWebService(SearchSRRequest request)
        {
            try
            {
                var response = new SearchSRResponse();

                if (string.IsNullOrEmpty(request.CustomerSubscriptionTypeCode)
                    && string.IsNullOrEmpty(request.CustomerCardNo)
                    && string.IsNullOrEmpty(request.AccountNo)
                    && string.IsNullOrEmpty(request.ContactSubscriptionTypeCode)
                    && string.IsNullOrEmpty(request.ContactCardNo)
                    && string.IsNullOrEmpty(request.ProductGroupCode)
                    && string.IsNullOrEmpty(request.ProductCode)
                    && string.IsNullOrEmpty(request.CampaignServiceCode)
                    && (request.AreaCode == 0)
                    && (request.SubAreaCode == 0)
                    && (request.TypeCode == 0)
                    && string.IsNullOrEmpty(request.ChannelCode)
                    && string.IsNullOrEmpty(request.EmployeeCodeforOwnerSR)
                    && string.IsNullOrEmpty(request.EmployeeCodeforDelegateSR)
                    && string.IsNullOrEmpty(request.SRStatusCode)
                    && string.IsNullOrEmpty(request.ActivityTypeCode))
                {
                    response.IsSuccess = false;
                    response.ErrorCode = "2";
                    response.ErrorMessage = "Search SR is required at least 1 condition.";
                    return response;
                }

                if (string.IsNullOrEmpty(request.CustomerSubscriptionTypeCode) && !string.IsNullOrEmpty(request.CustomerCardNo))
                {
                    response.IsSuccess = false;
                    response.ErrorCode = "3";
                    response.ErrorMessage = "When CustomerCardNo is not null or empty, CustomerSubscriptionTypeCode is required.";
                    return response;
                }

                if (!string.IsNullOrEmpty(request.CustomerSubscriptionTypeCode) && string.IsNullOrEmpty(request.CustomerCardNo))
                {
                    response.IsSuccess = false;
                    response.ErrorCode = "3";
                    response.ErrorMessage = "When CustomerSubscriptionTypeCode is not null or empty, CustomerCardNo is required.";
                    return response;
                }

                if (string.IsNullOrEmpty(request.ContactSubscriptionTypeCode) && !string.IsNullOrEmpty(request.ContactCardNo))
                {
                    response.IsSuccess = false;
                    response.ErrorCode = "3";
                    response.ErrorMessage = "When ContactCardNo is not null or empty, ContactSubscriptionTypeCode is required.";
                    return response;
                }

                if (!string.IsNullOrEmpty(request.ContactSubscriptionTypeCode) && string.IsNullOrEmpty(request.ContactCardNo))
                {
                    response.IsSuccess = false;
                    response.ErrorCode = "3";
                    response.ErrorMessage = "When ContactSubscriptionTypeCode is not null or empty, ContactCardNo is required.";
                    return response;
                }

                // Limit Maximum Result
                if (request.PageSize > 50)
                    request.PageSize = 50;

                int totalRecords = 0;

                _serviceRequestDataAccess = new ServiceRequestDataAccess(_context);
                response.SearchSRResponseItems = _serviceRequestDataAccess.SearchServiceRequestWebService(request, ref totalRecords);

                foreach (var item in response.SearchSRResponseItems)
                {
                    item.ReFormatData();
                }

                response.TotalRecords = totalRecords;
                response.StartPageIndex = request.StartPageIndex;
                response.PageSize = request.PageSize;

                response.IsSuccess = true;
                return response;
            }
            catch (Exception ex)
            {
                return new SearchSRResponse
                {
                    IsSuccess = false,
                    ErrorCode = "1",
                    ErrorMessage = ex.Message
                };
            }
        }

        public GetSRResponse GetSRWebService(string srNo)
        {
            try
            {

                if (string.IsNullOrEmpty(srNo))
                {
                    return new GetSRResponse
                    {
                        IsSuccess = false,
                        IsFound = false,
                        ErrorCode = "2",
                        ErrorMessage = "SR No is required",
                    };
                }

                _serviceRequestDataAccess = new ServiceRequestDataAccess(_context);
                var response = _serviceRequestDataAccess.GetServiceRequestWebService(srNo);

                if (response == null)
                {
                    return new GetSRResponse()
                    {
                        IsSuccess = true,
                        IsFound = false,
                        ErrorCode = "",
                        ErrorMessage = "Not found",
                    };
                }

                response.IsSuccess = true;
                response.IsFound = false;
                return response;
            }
            catch (Exception ex)
            {
                return new GetSRResponse
                {
                    IsSuccess = false,
                    IsFound = false,
                    ErrorCode = "1",
                    ErrorMessage = ex.Message
                };
            }
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
                    if (_userFacade != null) { _userFacade.Dispose(); }
                    if (_activityFacade != null) { _activityFacade.Dispose(); }
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
