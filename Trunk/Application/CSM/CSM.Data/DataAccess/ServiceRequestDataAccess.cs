using System;
using CSM.Entity;
using log4net;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CSM.Common.Utilities;
using System.Data.Entity.Core.Objects;
using CSM.Service.Messages.Sr;
using CSM.Service.Messages.OTP;

namespace CSM.Data.DataAccess
{
    public class ServiceRequestDataAccess : IServiceRequestDataAccess
    {
        private readonly CSMContext _context;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ServiceRequestDataAccess));

        public ServiceRequestDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        #region == Search Service Request ==

        public IEnumerable<ServiceRequestEntity> SearchServiceRequest(ServiceRequestSearchFilter searchFilter)
        {
            var query = _context.TB_T_SR.AsQueryable();

            if (!string.IsNullOrEmpty(searchFilter.CustomerFirstName))
            {
                var originalKeyword = searchFilter.CustomerFirstName.Trim();

                if (!string.IsNullOrEmpty(originalKeyword) && originalKeyword != "*" && originalKeyword != "**")
                {
                    if (originalKeyword.StartsWith("*") && originalKeyword.EndsWith("*"))
                    {
                        var keyword = originalKeyword.Substring(1, originalKeyword.Length - 2);
                        query = query.Where(q => q.TB_M_CUSTOMER.FIRST_NAME_TH.Contains(keyword) || q.TB_M_CUSTOMER.FIRST_NAME_EN.Contains(keyword));
                    }
                    else if (originalKeyword.StartsWith("*"))
                    {
                        var keyword = originalKeyword.Substring(1, originalKeyword.Length - 1);
                        query = query.Where(q => q.TB_M_CUSTOMER.FIRST_NAME_TH.EndsWith(keyword) || q.TB_M_CUSTOMER.FIRST_NAME_EN.EndsWith(keyword));
                    }
                    else if (originalKeyword.EndsWith("*"))
                    {
                        var keyword = originalKeyword.Substring(0, originalKeyword.Length - 1);
                        query = query.Where(q => q.TB_M_CUSTOMER.FIRST_NAME_TH.StartsWith(keyword) || q.TB_M_CUSTOMER.FIRST_NAME_EN.StartsWith(keyword));
                    }
                    else
                    {
                        query = query.Where(q => q.TB_M_CUSTOMER.FIRST_NAME_TH == originalKeyword || q.TB_M_CUSTOMER.FIRST_NAME_EN == originalKeyword);
                    }
                }
            }

            if (!string.IsNullOrEmpty(searchFilter.CustomerLastName))
            {
                var originalKeyword = searchFilter.CustomerLastName.Trim();

                if (!string.IsNullOrEmpty(originalKeyword) && originalKeyword != "*" && originalKeyword != "**")
                {
                    if (originalKeyword.StartsWith("*") && originalKeyword.EndsWith("*"))
                    {
                        var keyword = originalKeyword.Substring(1, originalKeyword.Length - 2);
                        query = query.Where(q => q.TB_M_CUSTOMER.LAST_NAME_TH.Contains(keyword) || q.TB_M_CUSTOMER.LAST_NAME_EN.Contains(keyword));
                    }
                    else if (originalKeyword.StartsWith("*"))
                    {
                        var keyword = originalKeyword.Substring(1, originalKeyword.Length - 1);
                        query = query.Where(q => q.TB_M_CUSTOMER.LAST_NAME_TH.EndsWith(keyword) || q.TB_M_CUSTOMER.LAST_NAME_EN.EndsWith(keyword));
                    }
                    else if (originalKeyword.EndsWith("*"))
                    {
                        var keyword = originalKeyword.Substring(0, originalKeyword.Length - 1);
                        query = query.Where(q => q.TB_M_CUSTOMER.LAST_NAME_TH.StartsWith(keyword) || q.TB_M_CUSTOMER.LAST_NAME_EN.StartsWith(keyword));
                    }
                    else
                    {
                        query = query.Where(q => q.TB_M_CUSTOMER.LAST_NAME_TH == originalKeyword || q.TB_M_CUSTOMER.LAST_NAME_EN == originalKeyword);
                    }
                }
            }

            //if (!string.IsNullOrEmpty(searchFilter.PhoneNo))
            //{
            //    var originalKeyword = searchFilter.PhoneNo.Trim();

            //    if (!string.IsNullOrEmpty(originalKeyword) && originalKeyword != "*" && originalKeyword != "**")
            //    {
            //        if (originalKeyword.StartsWith("*") && originalKeyword.EndsWith("*"))
            //        {
            //            var keyword = originalKeyword.Substring(1, originalKeyword.Length - 2);
            //            query = query.Where(q => q.TB_M_CUSTOMER.TB_M_PHONE.Any(p => p.PHONE_NO.Contains(keyword)));
            //        }
            //        else if (originalKeyword.StartsWith("*"))
            //        {
            //            var keyword = originalKeyword.Substring(1, originalKeyword.Length - 1);
            //            query = query.Where(q => q.TB_M_CUSTOMER.TB_M_PHONE.Any(p => p.PHONE_NO.EndsWith(keyword)));
            //        }
            //        else if (originalKeyword.EndsWith("*"))
            //        {
            //            var keyword = originalKeyword.Substring(0, originalKeyword.Length - 1);
            //            query = query.Where(q => q.TB_M_CUSTOMER.TB_M_PHONE.Any(p => p.PHONE_NO.StartsWith(keyword)));
            //        }
            //        else
            //        {
            //            query = query.Where(q => q.TB_M_CUSTOMER.TB_M_PHONE.Any(p => p.PHONE_NO == originalKeyword));
            //        }
            //    }
            //}

            if (!string.IsNullOrEmpty(searchFilter.ContactFirstName))
            {
                var originalKeyword = searchFilter.ContactFirstName.Trim();

                if (!string.IsNullOrEmpty(originalKeyword) && originalKeyword != "*" && originalKeyword != "**")
                {
                    if (originalKeyword.StartsWith("*") && originalKeyword.EndsWith("*"))
                    {
                        var keyword = originalKeyword.Substring(1, originalKeyword.Length - 2);
                        query = query.Where(q => q.TB_M_CONTACT.FIRST_NAME_TH.Contains(keyword) || q.TB_M_CONTACT.FIRST_NAME_EN.Contains(keyword));
                    }
                    else if (originalKeyword.StartsWith("*"))
                    {
                        var keyword = originalKeyword.Substring(1, originalKeyword.Length - 1);
                        query = query.Where(q => q.TB_M_CONTACT.FIRST_NAME_TH.EndsWith(keyword) || q.TB_M_CONTACT.FIRST_NAME_EN.EndsWith(keyword));
                    }
                    else if (originalKeyword.EndsWith("*"))
                    {
                        var keyword = originalKeyword.Substring(0, originalKeyword.Length - 1);
                        query = query.Where(q => q.TB_M_CONTACT.FIRST_NAME_TH.StartsWith(keyword) || q.TB_M_CONTACT.FIRST_NAME_EN.StartsWith(keyword));
                    }
                    else
                    {
                        query = query.Where(q => q.TB_M_CONTACT.FIRST_NAME_TH == originalKeyword || q.TB_M_CONTACT.FIRST_NAME_EN == originalKeyword);
                    }
                }
            }

            if (!string.IsNullOrEmpty(searchFilter.ContactLastName))
            {
                var originalKeyword = searchFilter.ContactLastName.Trim();

                if (!string.IsNullOrEmpty(originalKeyword) && originalKeyword != "*" && originalKeyword != "**")
                {
                    if (originalKeyword.StartsWith("*") && originalKeyword.EndsWith("*"))
                    {
                        var keyword = originalKeyword.Substring(1, originalKeyword.Length - 2);
                        query = query.Where(q => q.TB_M_CONTACT.LAST_NAME_TH.Contains(keyword) || q.TB_M_CONTACT.LAST_NAME_EN.Contains(keyword));
                    }
                    else if (originalKeyword.StartsWith("*"))
                    {
                        var keyword = originalKeyword.Substring(1, originalKeyword.Length - 1);
                        query = query.Where(q => q.TB_M_CONTACT.LAST_NAME_TH.EndsWith(keyword) || q.TB_M_CONTACT.LAST_NAME_EN.EndsWith(keyword));
                    }
                    else if (originalKeyword.EndsWith("*"))
                    {
                        var keyword = originalKeyword.Substring(0, originalKeyword.Length - 1);
                        query = query.Where(q => q.TB_M_CONTACT.LAST_NAME_TH.StartsWith(keyword) || q.TB_M_CONTACT.LAST_NAME_EN.StartsWith(keyword));
                    }
                    else
                    {
                        query = query.Where(q => q.TB_M_CONTACT.LAST_NAME_TH == originalKeyword || q.TB_M_CONTACT.LAST_NAME_EN == originalKeyword);
                    }
                }
            }

            // Filter : CardNo
            if (!string.IsNullOrEmpty(searchFilter.CustomerCardNo))
            {
                query = query.Where(q => q.TB_M_CUSTOMER.CARD_NO == searchFilter.CustomerCardNo);
            }

            //Filter : SrId
            if (!string.IsNullOrEmpty(searchFilter.SrNo))
            {
                query = query.Where(q => q.SR_NO == searchFilter.SrNo);
            }

            //Filter : AccountNo
            if (!string.IsNullOrEmpty(searchFilter.AccountNo))
            {
                query = query.Where(q => q.TB_M_ACCOUNT.ACCOUNT_NO == searchFilter.AccountNo);
            }

            //Filter : PhoneNo
            if (!string.IsNullOrEmpty(searchFilter.PhoneNo))
            {
                query = query.Where(q => q.TB_M_CUSTOMER.TB_M_PHONE.Any(p => p.PHONE_NO == searchFilter.PhoneNo));
            }

            //Filter : Owner Branch
            if (searchFilter.OwnerBranchId.HasValue)
            {
                query = query.Where(q => q.OWNER_BRANCH_ID == searchFilter.OwnerBranchId);
            }

            //Filter : Owner SR
            if (searchFilter.OwnerUserId.HasValue)
            {
                query = query.Where(q => q.OWNER_USER_ID == searchFilter.OwnerUserId);
            }

            //Filter : Delegate Branch
            if (searchFilter.DelegateBranchId.HasValue)
            {
                query = query.Where(q => q.DELEGATE_BRANCH_ID == searchFilter.DelegateBranchId);
            }

            //Filter : Delegate SR
            if (searchFilter.DelegateUserId.HasValue)
            {
                query = query.Where(q => q.DELEGATE_USER_ID == searchFilter.DelegateUserId);
            }

            //Filter : Creator Branch
            if (searchFilter.CreatorBranchId.HasValue)
            {
                query = query.Where(q => q.CREATE_BRANCH_ID == searchFilter.CreatorBranchId);
            }

            //Filter : Creator SR
            if (searchFilter.CreatorUserId.HasValue)
            {
                query = query.Where(q => q.CREATE_USER == searchFilter.CreatorUserId);
            }

            //ADVANCE FILTER
            //Filter : Create From & Create To
            DateTime? minCreateDate = null;
            DateTime? maxCreateDate = null;
            if (searchFilter.CreateDateFromValue.HasValue)
            {
                minCreateDate = searchFilter.CreateDateFromValue.Value.Date;
                query = query.Where(q => q.CREATE_DATE.HasValue && EntityFunctions.TruncateTime(q.CREATE_DATE.Value) >= minCreateDate);
            }
            if (searchFilter.CreateDateToValue.HasValue)
            {
                maxCreateDate = searchFilter.CreateDateToValue.Value.Date;
                query = query.Where(q => q.CREATE_DATE.HasValue && EntityFunctions.TruncateTime(q.CREATE_DATE.Value) <= maxCreateDate);
            }

            //Filter : Close From & Clost to
            DateTime? minCloseDate = null;
            DateTime? maxCloseDate = null;
            if (searchFilter.CloseDateFromValue.HasValue)
            {
                minCloseDate = searchFilter.CloseDateFromValue.Value.Date;
                query = query.Where(q => q.CLOSE_DATE.HasValue && EntityFunctions.TruncateTime(q.CLOSE_DATE.Value) >= minCloseDate);
            }
            if (searchFilter.CloseDateToValue.HasValue)
            {
                maxCloseDate = searchFilter.CloseDateToValue.Value.Date;
                query = query.Where(q => q.CLOSE_DATE.HasValue && EntityFunctions.TruncateTime(q.CLOSE_DATE.Value) <= maxCloseDate);
            }

            //Filter: Subject
            if (!string.IsNullOrEmpty(searchFilter.Subject))
            {
                query = query.Where(q => q.SR_SUBJECT == searchFilter.Subject);
            }

            //Filter: SR Channel
            if (searchFilter.ChannelId.HasValue)
            {
                query = query.Where(q => q.CHANNEL_ID == searchFilter.ChannelId);
            }

            //Filter: Product Group
            if (searchFilter.ProductGroupId.HasValue)
            {
                query = query.Where(q => q.PRODUCTGROUP_ID == searchFilter.ProductGroupId);
            }

            //Filter: Product
            if (searchFilter.ProductId.HasValue)
            {
                query = query.Where(q => q.PRODUCT_ID == searchFilter.ProductId);
            }

            //Filter: Campaign
            if (searchFilter.CampaignServiceId.HasValue)
            {
                query = query.Where(q => q.CAMPAIGNSERVICE_ID == searchFilter.CampaignServiceId);
            }

            //Filter: Type
            if (searchFilter.TypeId.HasValue)
            {
                query = query.Where(q => q.TYPE_ID == searchFilter.TypeId);
            }

            //Filter: Area
            if (searchFilter.AreaId.HasValue)
            {
                query = query.Where(q => q.AREA_ID == searchFilter.AreaId);
            }

            //Filter: SubArea
            if (searchFilter.SubAreaId.HasValue)
            {
                query = query.Where(q => q.SUBAREA_ID == searchFilter.SubAreaId);
            }

            //Filter: ContactCardNo
            if (!string.IsNullOrEmpty(searchFilter.ContactCardNo))
            {
                query = query.Where(q => q.TB_M_CONTACT.CARD_NO == searchFilter.ContactCardNo);
            }

            //Filter: SR Status
            if (!string.IsNullOrWhiteSpace(searchFilter.SRStatusStringList))
            {
                List<int?> statusList = new List<int?>();
                searchFilter.SRStatusStringList.Split(',').ToList().ForEach(p =>
                {
                    if (!string.IsNullOrWhiteSpace(p))
                    {
                        statusList.Add(int.Parse(p));
                    }
                });
                if (statusList.Count > 0)
                {
                    query = query.Where(q => statusList.Contains(q.SR_STATUS_ID));
                }
            }

            //Filter: SR State
            if (searchFilter.SRStateId.HasValue)
            {
                query = query.Where(q => q.TB_C_SR_STATUS.SR_STATE_ID == searchFilter.SRStateId);
            }

            List<int> pageIds;

            if (!string.IsNullOrEmpty(searchFilter.CanViewSrPageIds))
            {
                try
                {
                    pageIds = searchFilter.CanViewSrPageIds.Split(',').Select(u => Convert.ToInt32(u)).ToList();
                }
                catch (Exception)
                {
                    pageIds = new List<int>();
                    searchFilter.CanViewSrPageIds = string.Empty;
                }
            }
            else
            {
                pageIds = new List<int>();
            }


            if (!(searchFilter.CanViewAllUsers ?? false) && !string.IsNullOrEmpty(searchFilter.CanViewUserIds))
            {
                try
                {
                    var userIds = searchFilter.CanViewUserIds.Split(',').Select(u => Convert.ToInt32(u)).ToList();

                    query = query.Where(q =>
                                             (q.OWNER_USER_ID.HasValue && userIds.Contains(q.OWNER_USER_ID.Value))
                                             ||
                                             (q.DELEGATE_USER_ID.HasValue && userIds.Contains(q.DELEGATE_USER_ID.Value))
                                             ||
                                             (q.SR_STATUS_ID.HasValue && q.SR_STATUS_ID.Value == Constants.SRStatusId.Draft && q.CREATE_USER == searchFilter.CurrentUserId)
                                             ||
                                             (q.SR_PAGE_ID.HasValue && pageIds.Contains(q.SR_PAGE_ID.Value))
                                       );
                }
                catch (Exception ex)
                {
                    searchFilter.CanViewUserIds = string.Empty;
                    Logger.Error("Exception occur:\n", ex);
                }
            }

            // After Insert all filters >> Count It
            searchFilter.TotalRecords = query.Count();

            var results = from q in query
                          from ownerUser in _context.TB_R_USER.Where(user => q.OWNER_USER_ID == user.USER_ID).DefaultIfEmpty()
                          from delegateUser in _context.TB_R_USER.Where(user => q.DELEGATE_USER_ID == user.USER_ID).DefaultIfEmpty()
                          select new ServiceRequestEntity
                          {
                              SrId = q.SR_ID,
                              SrNo = q.SR_NO,
                              ThisAlert = q.RULE_THIS_ALERT,
                              NextSLA = q.RULE_NEXT_SLA,
                              //TotalWorkingHours = q.RULE_TOTAL_WORK,
                              TotalWorkingHours = q.TB_T_SR_ACTIVITY.Sum(x => x.WORKING_MINUTE ?? 0),
                              CustomerCardNo = q.TB_M_CUSTOMER.CARD_NO,
                              ChannelId = q.CHANNEL_ID,
                              ChannelName = q.TB_R_CHANNEL.CHANNEL_NAME,
                              ProductId = q.PRODUCT_ID,
                              ProductName = q.TB_R_PRODUCT.PRODUCT_NAME,
                              CampaignServiceId = q.CAMPAIGNSERVICE_ID,
                              CampaignServiceName = q.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_NAME,
                              AreaId = q.AREA_ID,
                              AreaName = q.TB_M_AREA.AREA_NAME,
                              SubAreaId = q.SUBAREA_ID,
                              SubAreaName = q.TB_M_SUBAREA.SUBAREA_NAME,
                              Subject = q.SR_SUBJECT,
                              SrStatusName = q.TB_C_SR_STATUS.SR_STATUS_NAME,
                              SRStateName = _context.TB_C_SR_STATE.Where(x => x.SR_STATE_ID == q.TB_C_SR_STATUS.SR_STATE_ID).FirstOrDefault().SR_STATE_NAME,
                              CreateDate = q.CREATE_DATE,
                              ClosedDate = q.CLOSE_DATE,
                              OwnerUserId = ownerUser != null ? ownerUser.USER_ID : (int?)null,
                              OwnerUserPosition = ownerUser != null ? ownerUser.POSITION_CODE : null,
                              OwnerUserFirstName = ownerUser != null ? ownerUser.FIRST_NAME : null,
                              OwnerUserLastName = ownerUser != null ? ownerUser.LAST_NAME : null,
                              DelegateUserId = delegateUser != null ? delegateUser.USER_ID : (int?)null,
                              DelegateUserPosition = delegateUser != null ? delegateUser.POSITION_CODE : null,
                              DelegateUserFirstName = delegateUser != null ? delegateUser.FIRST_NAME : null,
                              DelegateUserLastName = delegateUser != null ? delegateUser.LAST_NAME : null,
                              ANo = q.SR_ANO,
                              CPN_IsSecret = q.CPN_SECRET,
                              OwnerSupUserId = ownerUser != null ? ownerUser.SUPERVISOR_ID : (int?)null,
                              DelegateSupUserId = delegateUser != null ? delegateUser.SUPERVISOR_ID : (int?)null,
                              SrPageId = q.SR_PAGE_ID
                          };

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            switch (searchFilter.SortField)
            {
                case "ProductName":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC") ? results.OrderBy(q => q.ProductName) : results.OrderByDescending(q => q.ProductName);
                    break;
                case "AreaName":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(q => q.AreaName)
                        : results.OrderByDescending(q => q.AreaName);
                    break;
                case "SubAreaName":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(q => q.SubAreaName)
                        : results.OrderByDescending(q => q.SubAreaName);
                    break;
                case "SrStatus":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(q => q.SrStatusName)
                        : results.OrderByDescending(q => q.SrStatusName);
                    break;
                case "CreateDate":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(q => q.CreateDate)
                        : results.OrderByDescending(q => q.CreateDate);
                    break;
                case "CloseDate":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(q => q.ClosedDate)
                        : results.OrderByDescending(q => q.ClosedDate);
                    break;
                case "OwnerUserFirstName":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(q => q.OwnerUserFirstName)
                        : results.OrderByDescending(q => q.OwnerUserFirstName);
                    break;
                case "DelegateUserFirstName":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(q => q.DelegateUserFirstName)
                        : results.OrderByDescending(q => q.DelegateUserFirstName);
                    break;

                default:
                    results = results.OrderByDescending(q => q.ThisAlert);
                    break;
            }

            return results.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<ServiceRequestEntity>();
        }

        #endregion

        #region == ServiceRequest: Save Draft, Save, Update SR, Create Activity ==

        public ServiceRequestSaveSrResult SaveServiceRequest(ServiceRequestForSaveEntity entity, bool isSaveDraft)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var isEdit = entity.SrId.HasValue;

                var now = DateTime.Now;

                TB_T_SR sr;

                if (!isEdit)
                {
                    sr = new TB_T_SR();
                }
                else
                {
                    sr = _context.TB_T_SR.SingleOrDefault(s => s.SR_ID == entity.SrId.Value);

                    if (sr == null)
                    {
                        return new ServiceRequestSaveSrResult(false, string.Format("SR_ID: {0} does not exist in database.", entity.SrId.Value));
                    }
                }

                sr.SR_NO = string.IsNullOrEmpty(entity.SrNo) ? null : entity.SrNo;

                sr.SR_CALL_ID = entity.CallId;
                sr.SR_ANO = entity.PhoneNo;

                sr.CUSTOMER_ID = entity.CustomerId;
                sr.ACCOUNT_ID = entity.AccountId;
                sr.CONTACT_ID = entity.ContactId;

                sr.CONTACT_ACCOUNT_NO = entity.ContactAccountNo;
                sr.CONTACT_RELATIONSHIP_ID = entity.ContactRelationshipId;

                sr.PRODUCTGROUP_ID = entity.ProductGroupId;
                sr.PRODUCT_ID = entity.ProductId;
                sr.CAMPAIGNSERVICE_ID = entity.CampaignServiceId;

                sr.AREA_ID = entity.AreaId;
                sr.SUBAREA_ID = entity.SubAreaId;
                sr.TYPE_ID = entity.TypeId;

                sr.CHANNEL_ID = entity.ChannelId;
                sr.MEDIA_SOURCE_ID = entity.MediaSourceId;

                sr.SR_SUBJECT = entity.Subject ?? string.Empty;
                sr.SR_REMARK = entity.Remark ?? string.Empty;

                sr.MAP_PRODUCT_ID = entity.MappingProductId;
                sr.SR_IS_VERIFY = entity.IsVerify;

                sr.SR_IS_VERIFY_PASS = FormatVerifyPass(entity.IsVerifyPass);


                sr.DRAFT_VERIFY_ANSWER_JSON = isSaveDraft ? entity.VerifyAnswerJson : null;

                if (!isSaveDraft && entity.IsVerify && entity.VerifyAnswers != null && entity.VerifyAnswers.Count > 0)
                {
                    var mappingProductId = entity.MappingProductId;

                    var questionGroups = (from grp in _context.TB_M_MAP_PRODUCT_QUESTIONGROUP
                                          where grp.MAP_PRODUCT_ID == mappingProductId
                                          orderby grp.SEQ_NO, grp.MAP_PRODUCT_QUESTIONGROUP_ID
                                          select new
                                          {
                                              GroupId = grp.QUESTIONGROUP_ID,
                                              GroupName = grp.TB_M_QUESTIONGROUP.QUESTIONGROUP_NAME,
                                              RequireAmountPass = grp.REQUIRE_AMOUNT_PASS,
                                              Questions = (from q in grp.TB_M_QUESTIONGROUP.TB_M_QUESTIONGROUP_QUESTION
                                                           orderby q.SEQ_NO
                                                           select new
                                                           {
                                                               QuestionId = q.QUESTION_ID,
                                                               QuestionName = q.TB_M_QUESTION.QUESTION_NAME
                                                           }).ToList(),
                                          }).ToList();

                    for (int i = 0; i < questionGroups.Count; i++)
                    {
                        var questionGroup = questionGroups[i];
                        var grp = new TB_T_SR_VERIFY_RESULT_GROUP();
                        grp.QUESTIONGROUP_ID = questionGroup.GroupId;
                        grp.QUESTIONGROUP_NAME = questionGroup.GroupName;
                        grp.QUESTIONGROUP_PASS_AMOUNT = questionGroup.RequireAmountPass;
                        grp.SEQ_NO = (i + 1);
                        grp.TB_T_SR = sr;

                        _context.TB_T_SR_VERIFY_RESULT_GROUP.Add(grp);

                        var questions = questionGroup.Questions;
                        for (int j = 0; j < questions.Count(); j++)
                        {
                            var question = questions[j];
                            var answer = entity.VerifyAnswers.FirstOrDefault(a => a.GroupId == questionGroup.GroupId && a.QuestionId == question.QuestionId);

                            var q = new TB_T_SR_VERIFY_RESULT_QUESTION();
                            q.QUESTION_ID = question.QuestionId;
                            q.QUESTION_NAME = question.QuestionName;
                            q.RESULT = answer != null ? answer.Value : Constants.VerifyResultStatus.Skip;
                            q.SEQ_NO = (j + 1);
                            q.TB_T_SR_VERIFY_RESULT_GROUP = grp;

                            _context.TB_T_SR_VERIFY_RESULT_QUESTION.Add(q);
                        }
                    }
                }

                sr.SR_PAGE_ID = entity.SrPageId;

                if (entity.SrPageId == 1)
                {
                    sr.SR_DEF_ACCOUNT_ADDRESS_ID = entity.AddressSendDocId;

                    if (!isSaveDraft)
                    {
                        if (entity.AddressSendDocId.HasValue)
                        {
                            var accountAddress = GetAccountAddress(entity.AddressSendDocId.Value);
                            sr.SR_DEF_ADDRESS_HOUSE_NO = accountAddress.AddressNo;
                            sr.SR_DEF_ADDRESS_ROOM_NO = accountAddress.RoomNo;
                            sr.SR_DEF_ADDRESS_BUILDING = accountAddress.Building;
                            sr.SR_DEF_ADDRESS_FLOOR_NO = accountAddress.FloorNo;
                            sr.SR_DEF_ADDRESS_MOO = accountAddress.Moo;
                            sr.SR_DEF_ADDRESS_VILLAGE = accountAddress.Village;
                            sr.SR_DEF_ADDRESS_SOI = accountAddress.Soi;
                            sr.SR_DEF_ADDRESS_STREET = accountAddress.Street;
                            sr.SR_DEF_ADDRESS_TAMBOL = accountAddress.SubDistrict;
                            sr.SR_DEF_ADDRESS_AMPHUR = accountAddress.District;
                            sr.SR_DEF_ADDRESS_PROVINCE = accountAddress.Province;
                            sr.SR_DEF_ADDRESS_ZIPCODE = accountAddress.Postcode;
                        }
                    }
                    else
                    {
                        sr.DRAFT_ACCOUNT_ADDRESS_TEXT = entity.AddressSendDocText;
                    }
                }
                else if (entity.SrPageId == 2)
                {
                    sr.SR_AFS_ASSET_ID = entity.AfsAssetId;

                    if (!string.IsNullOrEmpty(entity.AfsAssetNo))
                    {
                        sr.SR_AFS_ASSET_NO = entity.AfsAssetNo;
                        sr.SR_AFS_ASSET_DESC = entity.AfsAssetDesc;
                    }
                    else
                    {
                        var afs = _context.TB_M_AFS_ASSET.SingleOrDefault(x => x.ASSET_ID == entity.AfsAssetId);
                        if (afs != null)
                        {
                            sr.SR_AFS_ASSET_NO = afs.ASSET_NO;

                            var assetInfoTemplate = "ประเภททรัพย์: {0}\nสถานะ: {1}\nที่ตั้ง: {2} {3}\nSale: {4},{5},{6},{7}";
                            sr.SR_AFS_ASSET_DESC = string.Format(CultureInfo.InvariantCulture, assetInfoTemplate,
                                afs.PROJECT_DES,
                                afs.STATUS_DESC,
                                afs.AMPHUR,
                                afs.PROVINCE,
                                afs.SALE_NAME,
                                afs.PHONE_NO,
                                afs.MOBILE_NO,
                                afs.EMAIL);
                        }
                        else
                        {
                            sr.SR_AFS_ASSET_NO = string.Empty;
                            sr.SR_AFS_ASSET_DESC = string.Empty;
                        }
                    }
                }
                else if (entity.SrPageId == 3)
                {
                    sr.SR_NCB_CUSTOMER_BIRTHDATE = entity.NCBBirthDate;
                    sr.SR_NCB_CHECK_STATUS = entity.NCBCheckStatus;
                    sr.SR_NCB_MARKETING_USER_ID = entity.NCBMarketingUserId;
                    sr.SR_NCB_MARKETING_FULL_NAME = entity.NCBMarketingName;
                    sr.SR_NCB_MARKETING_BRANCH_ID = entity.NCBMarketingBranchId;
                    sr.SR_NCB_MARKETING_BRANCH_NAME = entity.NCBMarketingBranchName;
                    sr.SR_NCB_MARKETING_BRANCH_UPPER_1_ID = entity.NCBMarketingBranchUpper1Id;
                    sr.SR_NCB_MARKETING_BRANCH_UPPER_1_NAME = entity.NCBMarketingBranchUpper1Name;
                    sr.SR_NCB_MARKETING_BRANCH_UPPER_2_ID = entity.NCBMarketingBranchUpper2Id;
                    sr.SR_NCB_MARKETING_BRANCH_UPPER_2_NAME = entity.NCBMarketingBranchUpper2Name;
                }
                else if (entity.SrPageId == 4)
                {
                    // Complaint
                    sr.CPN_PRODUCT_GROUP_ID = entity.CPN_ProductGroupId;
                    sr.CPN_PRODUCT_ID = entity.CPN_ProductId;
                    sr.CPN_CAMPAIGNSERVICE_ID = entity.CPN_CampaignId;
                    sr.CPN_SUBJECT_ID = entity.CPN_SubjectId;
                    sr.CPN_TYPE_ID = entity.CPN_TypeId;
                    sr.CPN_ROOT_CAUSE_ID = entity.CPN_RootCauseId;
                    sr.CPN_MAPPING_ID = entity.CPN_MappingId;
                    sr.CPN_ISSUES_ID = entity.CPN_IssuesId;
                    sr.CPN_SECRET = entity.CPN_IsSecret;
                    sr.CPN_CAR = entity.CPN_IsCAR;
                    sr.CPN_HPLog100 = entity.CPN_IsHPLog;
                    sr.CPN_BU_GROUP_ID = entity.CPN_BUGroupId;
                    sr.CPN_SUMMARY = entity.CPN_IsSummary;
                    sr.CPN_CAUSE_CUSTOMER = entity.CPN_Cause_Customer;
                    sr.CPN_CAUSE_STAFF = entity.CPN_Cause_Staff;
                    sr.CPN_CAUSE_SYSTEM = entity.CPN_Cause_System;
                    sr.CPN_CAUSE_PROCESS = entity.CPN_Cause_Process;
                    sr.CPN_CAUSE_CUSTOMER_DETAIL = entity.CPN_Cause_Customer_Detail;
                    sr.CPN_CAUSE_STAFF_DETAIL = entity.CPN_Cause_Staff_Detail;
                    sr.CPN_CAUSE_SYSTEM_DETAIL = entity.CPN_Cause_System_Detail;
                    sr.CPN_CAUSE_PROCESS_DETAIL = entity.CPN_Cause_Process_Detail;
                    sr.CPN_SUMMARY_ID = entity.CPN_SummaryId;
                    sr.CPN_CAUSE_SUMMARY_ID = entity.CPN_CauseSummaryId;
                    sr.CPN_FIXED_DETAIL = entity.CPN_Fixed_Detail;
                    sr.CPN_BU1_CODE = entity.CPN_BU1Code;
                    sr.CPN_BU2_CODE = entity.CPN_BU2Code;
                    sr.CPN_BU3_CODE = entity.CPN_BU3Code;
                    sr.CPN_MSHBranchId = entity.CPN_MSHBranchId;
                }

                sr.OWNER_BRANCH_ID = entity.OwnerBranchId;
                sr.OWNER_USER_ID = entity.OwnerUserId;
                sr.DELEGATE_BRANCH_ID = entity.DelegateBranchId;
                sr.DELEGATE_USER_ID = entity.DelegateUserId;
                sr.UPDATE_DATE_BY_OWNER = now;

                if (entity.DelegateUserId.HasValue)
                {
                    sr.RULE_DELEGATE_BRANCH_ID = entity.DelegateBranchId;
                    sr.RULE_DELEGATE_FLAG = "1";
                    sr.RULE_DELEGATE_DATE = now;
                    sr.UPDATE_DATE_BY_DELEGATE = now;
                }

                // Set value for DRAFT or clear when OPEN
                sr.DRAFT_SR_EMAIL_TEMPLATE_ID = isSaveDraft ? entity.SrEmailTemplateId : null;
                sr.DRAFT_ACTIVITY_DESC = isSaveDraft ? entity.ActivityDescription : null;
                sr.DRAFT_MAIL_SENDER = isSaveDraft ? entity.SendMailSender : null;
                sr.DRAFT_MAIL_TO = isSaveDraft ? entity.SendMailTo : null;
                sr.DRAFT_MAIL_CC = isSaveDraft ? entity.SendMailCc : null;
                sr.DRAFT_MAIL_BCC = isSaveDraft ? entity.SendMailBcc : null;
                sr.DRAFT_MAIL_SUBJECT = isSaveDraft ? entity.SendMailSubject : null;
                sr.DRAFT_MAIL_BODY = isSaveDraft ? entity.SendMailBody : null;
                sr.DRAFT_IS_SEND_EMAIL_FOR_DELEGATE = isSaveDraft ? entity.IsEmailDelegate : (bool?)null;
                sr.DRAFT_IS_CLOSE = isSaveDraft ? entity.IsClose : (bool?)null;
                sr.DRAFT_ATTACHMENT_JSON = isSaveDraft ? entity.AttachmentJson : null;
                sr.DRAFT_ACTIVITY_TYPE_ID = isSaveDraft ? entity.ActivityTypeId : (int?)null;
                sr.SR_STATUS_ID = entity.SrStatusId;
                sr.CLOSE_DATE = entity.CloseDate;

                if (!isEdit)
                {
                    // Save
                    sr.CREATE_BRANCH_ID = entity.CreateBranchId;
                    sr.CREATE_USER = entity.CreateUserId;
                    sr.UPDATE_USER = entity.CreateUserId;
                    sr.CREATE_DATE = now;
                    sr.UPDATE_DATE = now;
                    sr.RULE_STATUS_DATE = now;
                    _context.TB_T_SR.Add(sr);
                }
                else
                {
                    sr.UPDATE_USER = entity.CreateUserId;
                    sr.UPDATE_DATE = now;
                    SetEntryStateModified(sr);
                }

                if (entity.DelegateBranchId.HasValue && entity.DelegateUserId.HasValue)
                {
                    sr.RULE_DELEGATE_FLAG = "1";
                    sr.RULE_DELEGATE_BRANCH_ID = entity.DelegateBranchId;
                }

                sr.RULE_EMAIL_FLAG = entity.IsEmailDelegate ? "1" : "0";
                Save();

                entity.SrId = sr.SR_ID;
                entity.CreateDate = now;

                return new ServiceRequestSaveSrResult(true, "", sr.SR_ID, "", sr.SR_NO);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                return new ServiceRequestSaveSrResult(false, ex.Message);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        private string FormatVerifyPass(string original)
        {
            if (string.IsNullOrEmpty(original))
                return null;

            var tmp = original.ToUpper();

            if (tmp == "PASS" || tmp == "PASSED")
                return "PASS";

            if (tmp == "SKIP" || tmp == "SKIPED" || tmp == "SKIPPED")
                return "SKIP";

            if (tmp == "FAIL" || tmp == "FAILED")
                return "FAIL";

            return original;
        }

        private AccountAddressEntity GetAccountAddress(int id)
        {
            var accountAddress = _context.TB_M_ACCOUNT_ADDRESS.SingleOrDefault(q => q.ADDRESS_ID == id);

            if (accountAddress == null)
                return null;

            return new AccountAddressEntity
            {
                AddressId = accountAddress.ADDRESS_ID,
                AddressNo = accountAddress.ADDRESS_NO ?? "",
                Village = accountAddress.VILLAGE ?? "",
                Building = accountAddress.BUILDING ?? "",
                FloorNo = accountAddress.FLOOR_NO ?? "",
                RoomNo = accountAddress.ROOM_NO ?? "",
                Moo = accountAddress.MOO ?? "",
                Street = accountAddress.STREET ?? "",
                Soi = accountAddress.SOI ?? "",
                SubDistrict = accountAddress.SUB_DISTRICT ?? "",
                District = accountAddress.DISTRICT ?? "",
                Province = accountAddress.PROVINCE ?? "",
                Country = accountAddress.COUNTRY ?? "",
                Postcode = accountAddress.POSTCODE ?? ""
            };
        }

        public void UpdateServiceRequest(ServiceRequestForSaveEntity entity)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                if (entity == null)
                    throw new ArgumentNullException("Technical Error: Argument is null");

                if (!entity.SrId.HasValue || entity.SrId.Value == 0)
                    throw new ArgumentException("Technical Error: Sr Id is null or 0");

                var sr = _context.TB_T_SR.SingleOrDefault(x => x.SR_ID == entity.SrId.Value);

                sr.SR_SUBJECT = entity.Subject;
                sr.SR_REMARK = entity.Remark;

                sr.UPDATE_DATE = DateTime.Now;
                sr.UPDATE_USER = entity.CreateUserId;

                if (sr.OWNER_USER_ID == entity.CreateUserId)
                    sr.UPDATE_DATE_BY_OWNER = DateTime.Now;

                if (sr.DELEGATE_USER_ID == entity.CreateUserId)
                    sr.UPDATE_DATE_BY_DELEGATE = DateTime.Now;

                if (sr.SR_PAGE_ID.HasValue)
                {
                    var srPageId = sr.SR_PAGE_ID.Value;

                    if (srPageId == 1)
                    {
                        if (entity.AddressSendDocId.HasValue)
                        {
                            sr.SR_DEF_ACCOUNT_ADDRESS_ID = entity.AddressSendDocId.Value;

                            var accountAddress = GetAccountAddress(entity.AddressSendDocId.Value);
                            if (accountAddress != null)
                            {
                                sr.SR_DEF_ADDRESS_HOUSE_NO = accountAddress.AddressNo;
                                sr.SR_DEF_ADDRESS_ROOM_NO = accountAddress.RoomNo;
                                sr.SR_DEF_ADDRESS_BUILDING = accountAddress.Building;
                                sr.SR_DEF_ADDRESS_FLOOR_NO = accountAddress.FloorNo;
                                sr.SR_DEF_ADDRESS_MOO = accountAddress.Moo;
                                sr.SR_DEF_ADDRESS_VILLAGE = accountAddress.Village;
                                sr.SR_DEF_ADDRESS_SOI = accountAddress.Soi;
                                sr.SR_DEF_ADDRESS_STREET = accountAddress.Street;
                                sr.SR_DEF_ADDRESS_TAMBOL = accountAddress.SubDistrict;
                                sr.SR_DEF_ADDRESS_AMPHUR = accountAddress.District;
                                sr.SR_DEF_ADDRESS_PROVINCE = accountAddress.Province;
                                sr.SR_DEF_ADDRESS_ZIPCODE = accountAddress.Postcode;
                            }
                        }
                        else
                        {
                            sr.SR_DEF_ACCOUNT_ADDRESS_ID = null;
                            sr.SR_DEF_ADDRESS_HOUSE_NO = null;
                            sr.SR_DEF_ADDRESS_ROOM_NO = null;
                            sr.SR_DEF_ADDRESS_BUILDING = null;
                            sr.SR_DEF_ADDRESS_FLOOR_NO = null;
                            sr.SR_DEF_ADDRESS_MOO = null;
                            sr.SR_DEF_ADDRESS_VILLAGE = null;
                            sr.SR_DEF_ADDRESS_SOI = null;
                            sr.SR_DEF_ADDRESS_STREET = null;
                            sr.SR_DEF_ADDRESS_TAMBOL = null;
                            sr.SR_DEF_ADDRESS_AMPHUR = null;
                            sr.SR_DEF_ADDRESS_PROVINCE = null;
                            sr.SR_DEF_ADDRESS_ZIPCODE = null;
                        }
                    }
                    else if (srPageId == 2)
                    {
                        sr.SR_AFS_ASSET_ID = entity.AfsAssetId;
                        sr.SR_AFS_ASSET_NO = entity.AfsAssetNo;
                        sr.SR_AFS_ASSET_DESC = entity.AfsAssetDesc;
                    }
                    else if (srPageId == 3)
                    {
                        sr.SR_NCB_CUSTOMER_BIRTHDATE = entity.NCBBirthDate;
                        sr.SR_NCB_CHECK_STATUS = entity.NCBCheckStatus;

                        sr.SR_NCB_MARKETING_USER_ID = entity.NCBMarketingUserId;

                        sr.SR_NCB_MARKETING_FULL_NAME = entity.NCBMarketingName;
                        sr.SR_NCB_MARKETING_BRANCH_ID = entity.NCBMarketingBranchId;
                        sr.SR_NCB_MARKETING_BRANCH_NAME = entity.NCBMarketingBranchName;
                        sr.SR_NCB_MARKETING_BRANCH_UPPER_1_ID = entity.NCBMarketingBranchUpper1Id;
                        sr.SR_NCB_MARKETING_BRANCH_UPPER_1_NAME = entity.NCBMarketingBranchUpper1Name;
                        sr.SR_NCB_MARKETING_BRANCH_UPPER_2_ID = entity.NCBMarketingBranchUpper2Id;
                        sr.SR_NCB_MARKETING_BRANCH_UPPER_2_NAME = entity.NCBMarketingBranchUpper2Name;
                    }
                    else if (srPageId == Constants.SRPage.CPNPageId)
                    {
                        //Complaint
                        sr.CPN_PRODUCT_GROUP_ID = entity.CPN_ProductGroupId;
                        sr.CPN_PRODUCT_ID = entity.CPN_ProductId;
                        sr.CPN_CAMPAIGNSERVICE_ID = entity.CPN_CampaignId;
                        sr.CPN_SUBJECT_ID = entity.CPN_SubjectId;
                        sr.CPN_TYPE_ID = entity.CPN_TypeId;
                        sr.CPN_ROOT_CAUSE_ID = entity.CPN_RootCauseId;
                        sr.CPN_MAPPING_ID = entity.CPN_MappingId;
                        sr.CPN_ISSUES_ID = entity.CPN_IssuesId;
                        sr.CPN_SECRET = entity.CPN_IsSecret;
                        sr.CPN_CAR = entity.CPN_IsCAR;
                        sr.CPN_HPLog100 = entity.CPN_IsHPLog;

                        sr.CPN_BU_GROUP_ID = entity.CPN_BUGroupId;
                        sr.CPN_SUMMARY = entity.CPN_IsSummary;
                        sr.CPN_CAUSE_CUSTOMER = entity.CPN_Cause_Customer;
                        sr.CPN_CAUSE_STAFF = entity.CPN_Cause_Staff;
                        sr.CPN_CAUSE_SYSTEM = entity.CPN_Cause_System;
                        sr.CPN_CAUSE_PROCESS = entity.CPN_Cause_Process;

                        sr.CPN_CAUSE_CUSTOMER_DETAIL = entity.CPN_Cause_Customer_Detail;
                        sr.CPN_CAUSE_STAFF_DETAIL = entity.CPN_Cause_Staff_Detail;
                        sr.CPN_CAUSE_SYSTEM_DETAIL = entity.CPN_Cause_System_Detail;
                        sr.CPN_CAUSE_PROCESS_DETAIL = entity.CPN_Cause_Process_Detail;

                        sr.CPN_SUMMARY_ID = entity.CPN_SummaryId;
                        sr.CPN_CAUSE_SUMMARY_ID = entity.CPN_CauseSummaryId;

                        sr.CPN_FIXED_DETAIL = entity.CPN_Fixed_Detail;

                        sr.CPN_BU1_CODE = entity.CPN_BU1Code;
                        sr.CPN_BU2_CODE = entity.CPN_BU2Code;
                        sr.CPN_BU3_CODE = entity.CPN_BU3Code;

                        sr.CPN_MSHBranchId = entity.CPN_MSHBranchId;
                    }
                }

                SetEntryStateModified(sr);
                Save();
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public ServiceRequestSaveActivityResult CreateServiceRequestActivity(ServiceRequestForSaveEntity entity, int? oldOwnerUserId, int? oldDelegateUserId, int? oldSrStatusId)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var result = new ServiceRequestSaveActivityResult();

                if (entity == null)
                    return new ServiceRequestSaveActivityResult() { IsSuccess = false, ErrorMessage = "Technical Error: Argument is null" };

                if (!entity.SrId.HasValue || entity.SrId.Value == 0)
                    return new ServiceRequestSaveActivityResult() { IsSuccess = false, ErrorMessage = "Technical Error: Sr Id is null or 0" };

                var isEdit = entity.SrId.HasValue;

                var activity = new TB_T_SR_ACTIVITY();
                activity.SR_ID = entity.SrId.Value;
                activity.OWNER_BRANCH_ID = entity.OwnerBranchId;
                activity.OWNER_USER_ID = entity.OwnerUserId;
                activity.SR_STATUS_ID = entity.SrStatusId;
                activity.DELEGATE_BRANCH_ID = entity.DelegateBranchId;
                activity.DELEGATE_USER_ID = entity.DelegateUserId;
                activity.OLD_OWNER_USER_ID = oldOwnerUserId;
                activity.OLD_DELEGATE_USER_ID = oldDelegateUserId;
                activity.OLD_SR_STATUS_ID = oldSrStatusId;
                activity.IS_SEND_DELEGATE_EMAIL = entity.IsEmailDelegate;
                activity.SR_ACTIVITY_DESC = entity.ActivityDescription;
                activity.SR_EMAIL_TEMPLATE_ID = entity.SrEmailTemplateId;

                if (entity.SrEmailTemplateId.HasValue)
                {
                    //var subject = entity.SendMailSubject;
                    //subject = FillEmailParameterPreProcessing(subject, entity.SrId.Value);
                    //subject = FillEmailParameterPostProcessing(subject, entity.SrNo, entity.CreateDate, entity.SrStatusId);

                    //var body = entity.SendMailBody;
                    //body = FillEmailParameterPreProcessing(body, entity.SrId.Value);
                    //body = FillEmailParameterPostProcessing(body, entity.SrNo, entity.CreateDate, entity.SrStatusId);

                    var fullSR = GetServiceRequest(entity.SrId.Value);

                    var subject = FillEmailParameterPostProcessing(entity.SendMailSubject, fullSR);
                    var body = FillEmailParameterPostProcessing(entity.SendMailBody, fullSR);

                    activity.SR_ACTIVITY_EMAIL_SENDER = entity.SendMailSender;
                    activity.SR_ACTIVITY_EMAIL_TO = entity.SendMailTo;
                    activity.SR_ACTIVITY_EMAIL_CC = entity.SendMailCc;
                    activity.SR_ACTIVITY_EMAIL_BCC = entity.SendMailBcc;
                    activity.SR_ACTIVITY_EMAIL_SUBJECT = subject;
                    activity.SR_ACTIVITY_EMAIL_BODY = body;

                    if (entity.SrAttachments != null && entity.SrAttachments.Count > 0)
                    {
                        var filenames = entity.SrAttachments.Where(x => !string.IsNullOrEmpty(x.AttachToEmail) && x.AttachToEmail.ToLower() == "true").Select(x => x.Name).ToList();
                        if (filenames.Count > 0)
                            activity.SR_ACTIVITY_EMAIL_ATTACHMENTS = string.Join(",", filenames);
                    }

                    result.EmailSender = entity.SendMailSender;
                    result.EmailReceivers = entity.SendMailTo;
                    result.EmailCcs = entity.SendMailCc;
                    result.EmailBccs = entity.SendMailBcc;
                    result.EmailSubject = subject;
                    result.EmailBody = body;
                    result.EmailAttachments = activity.SR_ACTIVITY_EMAIL_ATTACHMENTS;

                }
                else
                {
                    activity.SR_ACTIVITY_EMAIL_SENDER = null;
                    activity.SR_ACTIVITY_EMAIL_TO = null;
                    activity.SR_ACTIVITY_EMAIL_CC = null;
                    activity.SR_ACTIVITY_EMAIL_BCC = null;
                    activity.SR_ACTIVITY_EMAIL_SUBJECT = null;
                    activity.SR_ACTIVITY_EMAIL_BODY = null;
                }

                activity.SR_ACTIVITY_TYPE_ID = entity.ActivityTypeId;

                activity.CREATE_BRANCH_ID = entity.CreateBranchId;
                activity.CREATE_USER = entity.CreateUserId;
                activity.CREATE_DATE = DateTime.Now;
                activity.CREATE_USERNAME = entity.CreateUsername;

                activity.ACTIVITY_CAR_SUBMIT_STATUS = 0;
                activity.ACTIVITY_CAR_SUBMIT_DATE = null;
                activity.ACTIVITY_CAR_SUBMIT_ERROR_CODE = null;
                activity.ACTIVITY_CAR_SUBMIT_ERROR_DESC = null;
                activity.ACTIVITY_HP_SUBMIT_STATUS = 0;
                activity.ACTIVITY_HP_SUBMIT_DATE = null;
                activity.ACTIVITY_HP_SUBMIT_ERROR_CODE = null;
                activity.ACTIVITY_HP_SUBMIT_ERROR_DESC = null;

                activity.IS_SECRET = entity.CPN_IsSecret;
                activity.IS_SEND_CAR = entity.CPN_IsCAR;

                activity.WORKING_MINUTE = (int)CalTotalWorkingMinute(activity.OWNER_BRANCH_ID ?? 0, activity.SR_ID
                                                                        , activity.CREATE_DATE ?? DateTime.Now);

                _context.TB_T_SR_ACTIVITY.Add(activity);
                Save();

                result.IsSuccess = true;
                result.SrActivityId = activity.SR_ACTIVITY_ID;
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                var result = new ServiceRequestSaveActivityResult();
                result.IsSuccess = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public bool GetOTPResult(string refNo, out string status, out string errCode, out string errDesc)
        {
            SendOTPEntity q = (from a in _context.TB_T_SEND_OTP.AsNoTracking()
                               where a.CSM_REFNO == refNo && (a.RESULT_STATUS_CODE ?? "") != ""
                               select new SendOTPEntity()
                               {
                                   ResultStatusCode = a.RESULT_STATUS_CODE,
                                   ResultStatusCodeDisplay = a.RESULT_STATUS_CODE != null
                                                                ? (from x in _context.TB_C_PARAMETER
                                                                   where x.PARAMETER_NAME == "OTP_STATUS_CODE" && x.PARAMETER_VALUE == a.RESULT_STATUS_CODE
                                                                   select x.PARAMETER_DESC).FirstOrDefault()
                                                                : null,
                                   ResultErrorCode = a.RESULT_ERROR_CODE,
                                   ResultErrorDesc = a.RESULT_ERROR_DESC,
                                   RequestStatusCode = a.REQUEST_STATUS_CODE,
                                   RequestStatusCodeDisplay = a.REQUEST_STATUS_CODE != null
                                                                ? (from x in _context.TB_C_PARAMETER
                                                                   where x.PARAMETER_NAME == "OTP_STATUS_CODE" && x.PARAMETER_VALUE == a.REQUEST_STATUS_CODE
                                                                   select x.PARAMETER_DESC).FirstOrDefault()
                                                                : null,
                                   RequestErrorCode = a.REQUEST_ERROR_CODE,
                                   RequestErrorDesc = a.REQUEST_ERROR_DESC
                               }).FirstOrDefault();
            errDesc =
            errCode = string.Empty;
            if (q != null)
            {
                status = q.ResultStatusCodeDisplay;
                errCode = q.ResultErrorCode;
                errDesc = q.ResultErrorDesc;
                return true;
            }
            else
            {
                q = (from a in _context.TB_T_SEND_OTP.AsNoTracking()
                     where a.CSM_REFNO == refNo && (a.REQUEST_STATUS_CODE ?? "") != ""
                     select new SendOTPEntity()
                     {
                         RequestStatusCode = a.REQUEST_STATUS_CODE,
                         RequestStatusCodeDisplay = a.REQUEST_STATUS_CODE != null
                                                                ? (from x in _context.TB_C_PARAMETER
                                                                   where x.PARAMETER_NAME == "OTP_STATUS_CODE" && x.PARAMETER_VALUE == a.REQUEST_STATUS_CODE
                                                                   select x.PARAMETER_DESC).FirstOrDefault()
                                                                : null,
                         RequestErrorCode = a.REQUEST_ERROR_CODE,
                         RequestErrorDesc = a.REQUEST_ERROR_DESC
                     }).FirstOrDefault();

                if (q != null)
                {
                    status = q.RequestStatusCodeDisplay;
                    errCode = q.RequestErrorCode;
                    errDesc = q.RequestErrorDesc;
                    return true;
                }
                else
                {
                    q = (from a in _context.TB_T_SEND_OTP.AsNoTracking()
                         where a.CSM_REFNO == refNo
                         select new SendOTPEntity()
                         {
                             RequestStatusCode = a.REQUEST_STATUS_CODE,
                             RequestStatusCodeDisplay = a.REQUEST_STATUS_CODE != null
                                                                    ? (from x in _context.TB_C_PARAMETER
                                                                       where x.PARAMETER_NAME == "OTP_STATUS_CODE" && x.PARAMETER_VALUE == a.REQUEST_STATUS_CODE
                                                                       select x.PARAMETER_DESC).FirstOrDefault()
                                                                    : null,
                             RequestErrorCode = a.REQUEST_ERROR_CODE,
                             RequestErrorDesc = a.REQUEST_ERROR_DESC
                         }).FirstOrDefault();
                    if (q != null)
                    {
                        status = q.RequestStatusCodeDisplay ?? string.Empty;
                        errCode = q.RequestErrorCode ?? string.Empty;
                        errDesc = q.RequestErrorDesc ?? string.Empty;
                        return true;
                    }
                    else
                    {
                        status =
                        errCode =
                        errDesc = "N/A";
                        return false;
                    }
                }
            }
        }

        public string GetOTPStatusDescByCode(string statusCode)
        {
            return (from a in _context.TB_C_PARAMETER.AsNoTracking()
                    where a.PARAMETER_NAME == "OTP_STATUS_CODE" && a.PARAMETER_VALUE == statusCode
                    select a.PARAMETER_DESC).FirstOrDefault();
        }

        public List<SendOTPEntity> GetSendOTPHistory(string callId, string cardNo)
        {
            return (from a in _context.TB_T_SEND_OTP.AsNoTracking()
                    where a.CALL_ID == callId && (a.ID_CARD == null ? "" : a.ID_CARD) == cardNo
                    orderby a.CREATE_DATE
                    select new SendOTPEntity()
                    {
                        CSM_RefNo = a.CSM_REFNO,
                        CardNo = a.ID_CARD,
                        MobileNo = a.MOBILE_PHONE,
                        RequestIVRRefNo = a.REQUEST_IVR_REFNO,
                        RequestDate = a.REQUEST_DATE,
                        RequestStatusCode = a.REQUEST_STATUS_CODE,
                        RequestStatusCodeDisplay = a.REQUEST_STATUS_CODE != null
                                                                ? (from x in _context.TB_C_PARAMETER
                                                                   where x.PARAMETER_NAME == "OTP_STATUS_CODE" && x.PARAMETER_VALUE == a.REQUEST_STATUS_CODE
                                                                   select x.PARAMETER_DESC).FirstOrDefault()
                                                                : null,
                        RequestErrorCode = a.REQUEST_ERROR_CODE,
                        RequestErrorDesc = a.REQUEST_ERROR_DESC,
                        RequestCaaErrorCode = a.REQUEST_CAA_ERROR_CODE,
                        RequestCaaErrorDesc = a.REQUEST_CAA_ERROR_DESC,
                        RequestOTPPrefix = a.REQUEST_OTP_PREFIX,
                        RequestOTPDetail = a.REQUEST_OTP_DETAIL,
                        RequestOTPSuffix = a.REQUEST_OTP_SUFFIX,
                        ResultIVRRefNo = a.RESULT_IVR_REFNO,
                        ResultDate = a.RESULT_DATE,
                        ResultStatusCode = a.RESULT_STATUS_CODE,
                        ResultStatusCodeDisplay = a.RESULT_STATUS_CODE != null
                                                                ? (from x in _context.TB_C_PARAMETER
                                                                   where x.PARAMETER_NAME == "OTP_STATUS_CODE" && x.PARAMETER_VALUE == a.RESULT_STATUS_CODE
                                                                   select x.PARAMETER_DESC).FirstOrDefault()
                                                                : null,
                        ResultErrorCode = a.RESULT_ERROR_CODE,
                        ResultErrorDesc = a.RESULT_ERROR_DESC,
                        ResultCaaErrorCode = a.RESULT_CAA_ERROR_CODE,
                        ResultCaaErrorDesc = a.RESULT_CAA_ERROR_DESC,
                        ResultOTPPrefix = a.RESULT_OTP_PREFIX,
                        ResultOTPDetail = a.RESULT_OTP_DETAIL,
                        ResultOTPSuffix = a.RESULT_OTP_SUFFIX,
                        UserAction = a.USER_ACTION,
                        UpdateDate = a.UPDATE_DATE
                    }).ToList();
        }

        public List<CampaignServiceEntity> GetDefaultCampaign(int? productGroupId, int? productId, int? campaignServiceId)
        {
            return (from cp in _context.TB_R_CAMPAIGNSERVICE.AsNoTracking()
                    from p in _context.TB_R_PRODUCT.Where(x => x.PRODUCT_ID == cp.PRODUCT_ID).DefaultIfEmpty()
                    from pg in _context.TB_R_PRODUCTGROUP.Where(x => x.PRODUCTGROUP_ID == p.PRODUCTGROUP_ID).DefaultIfEmpty()
                    where (!productGroupId.HasValue || pg.PRODUCTGROUP_ID == productGroupId)
                        && (!productId.HasValue || p.PRODUCT_ID == productId)
                        && (!campaignServiceId.HasValue || cp.CAMPAIGNSERVICE_ID == campaignServiceId)
                    select new CampaignServiceEntity()
                    {
                        CampaignServiceId = cp.CAMPAIGNSERVICE_ID,
                        CampaignServiceCode = cp.CAMPAIGNSERVICE_CODE,
                        CampaignServiceName = cp.CAMPAIGNSERVICE_NAME,
                        ProductId = p.PRODUCT_ID,
                        ProductName = p.PRODUCT_NAME,
                        ProductGroupId = pg.PRODUCTGROUP_ID,
                        ProductGroupName = pg.PRODUCTGROUP_NAME
                    }).ToList();
        }

        public bool SaveSMSTrans(SendOTPEntity model, out string msg)
        {
            return SaveSMSTrans(model, null, null, null, out msg);
        }

        public bool SaveSMSTrans(SendOTPEntity model, string statCode, string errorCode, string errorDesc, out string msg)
        {
            bool succ = false;
            msg = string.Empty;
            try
            {
                DateTime now = DateTime.Now;
                TB_T_SEND_OTP ss = null;
                if (!model.SendOTPId.HasValue)
                {
                    ss = new TB_T_SEND_OTP();
                    ss.CREATE_DATE = now;
                    ss.CREATE_USER = model.CreateUser;
                }
                else
                {
                    ss = _context.TB_T_SEND_OTP.SingleOrDefault(x => x.SEND_OTP_ID == model.SendOTPId.Value);
                    if (ss == null)
                    {
                        msg = $"SMS Send ID: {model.SendOTPId} does not exist in database.";
                        return false;
                    }
                }
                ss.UPDATE_DATE = now;
                ss.UPDATE_USER = model.UpdateUser;

                ss.CALL_ID = model.CallId;
                ss.CLIENT_IP = model.ClientIP;
                ss.IDENTIFICATION_TYPE = model.CardType;
                ss.ID_CARD = model.CardNo;
                ss.METHOD = model.Method;
                ss.MOBILE_PHONE = model.MobileNo;
                ss.PRODUCT_DESC = model.ProductDesc;
                ss.CSM_REFNO = model.CSM_RefNo;
                ss.RESERVE_FIELD1 = model.ReserveField1;
                ss.TEMPLATE_ID = model.TemplateId;
                ss.TXN_ID = model.TxnId;
                ss.USER_ACTION = model.UserAction;

                if (!string.IsNullOrWhiteSpace(statCode))
                {
                    ss.REQUEST_STATUS_CODE = statCode;
                    ss.REQUEST_ERROR_CODE = errorCode;
                    ss.REQUEST_ERROR_DESC = errorDesc;
                    ss.REQUEST_DATE = now;
                }

                if (!model.SendOTPId.HasValue)
                    _context.TB_T_SEND_OTP.Add(ss);
                else
                    SetEntryStateModified(ss);
                Save();
                succ = true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw;
            }
            return succ;
        }

        public bool UpdateRequestOTPTimeout(string refNo, string msg)
        {
            try
            {
                TB_T_SEND_OTP ss = _context.TB_T_SEND_OTP.FirstOrDefault(x => x.CSM_REFNO == refNo);
                if (ss == null)
                {
                    msg = $"Update OTP Request Timeout : OTP RefNo [{refNo}] does not exists in database!";
                    Logger.Error(msg);
                }
                else
                {
                    DateTime now = DateTime.Now;
                    Logger.Info($"Update OTP Request Timeout : RefNo [{refNo}]");
                    ss.REQUEST_STATUS_CODE = "1";
                    ss.REQUEST_ERROR_CODE = "CSM-300";
                    ss.REQUEST_ERROR_DESC = msg;
                    ss.REQUEST_DATE = now;
                    ss.UPDATE_DATE = now;
                    ss.UPDATE_USER = 1;
                    SetEntryStateModified(ss);
                    Save();
                }
            }
            catch (Exception ex)
            {
                msg = $"update OTP RefNo [{refNo}] error. {ex.Message}";
                Logger.Error("Exception occur:\n", ex);
            }
            return true;
        }

        public bool UpdateOTPVerify(OTPResultSvcRequest req, out string msg)
        {
            bool succ = false;
            msg = string.Empty;
            string txnId = req.TXN_ID;
            string refNo = req.Header.reference_no;
            string verifyStatus = req.VERIFY_OTP_STATUS;
            string verifyErrorCode = req.VERIFY_OTP_FAILURE_CODE;
            string verifyErrorReason = req.VERIFY_OTP_FAILURE_REASON;
            string caaErrorCode = req.CAA_FAILURE_CODE;
            string caaErrorReason = req.CAA_FAILURE_REASON;
            string mobilePhoneNo = req.MOBILE_PHONE;
            string OTPPrefix = req.OTP_PREFIX;
            string OTPDetail = req.OTP_DETAIL;
            string OTPSuffix = req.OTP_SUFFIX;

            try
            {
                TB_T_SEND_OTP ss = _context.TB_T_SEND_OTP.FirstOrDefault(x => x.CSM_REFNO == txnId);
                if (ss == null)
                {
                    msg = $"OTP RefNo [{txnId}] does not exists in database!";
                    Logger.Error(msg);
                }
                else
                {
                    Logger.Info($"Update Send SMS Verify : RefNo [{txnId}] Status [{verifyStatus}] Error Code [{verifyErrorCode}] Error Reason [{verifyErrorReason}]");
                    switch (req.Header.service_name.ToUpper())
                    {
                        case "OTPREQUEST":
                            {
                                ss.TXN_ID = txnId;
                                ss.MOBILE_PHONE = req.MOBILE_PHONE;
                                ss.REQUEST_IVR_REFNO = txnId;
                                ss.REQUEST_STATUS_CODE = verifyStatus;
                                ss.REQUEST_ERROR_CODE = verifyErrorCode;
                                ss.REQUEST_ERROR_DESC = verifyErrorReason;
                                ss.REQUEST_CAA_ERROR_CODE = caaErrorCode;
                                ss.REQUEST_CAA_ERROR_DESC = caaErrorReason;
                                ss.REQUEST_OTP_PREFIX = OTPPrefix;
                                ss.REQUEST_OTP_DETAIL = OTPDetail;
                                ss.REQUEST_OTP_SUFFIX = OTPSuffix;
                                ss.REQUEST_DATE = DateTime.Now;
                            }
                            break;
                        default:
                            {
                                //OTPRESULT
                                ss.RESULT_IVR_REFNO = txnId;
                                ss.RESULT_STATUS_CODE = verifyStatus;
                                ss.RESULT_ERROR_CODE = verifyErrorCode;
                                ss.RESULT_ERROR_DESC = verifyErrorReason;
                                ss.RESULT_CAA_ERROR_CODE = verifyErrorCode;
                                ss.RESULT_CAA_ERROR_DESC = verifyErrorReason;
                                //ss.RESULT_OTP_PREFIX = OTPPrefix;
                                //ss.RESULT_OTP_DETAIL = OTPDetail;
                                //ss.RESULT_OTP_SUFFIX = OTPSuffix;
                                ss.RESULT_DATE = DateTime.Now;
                            }
                            break;
                    }
                    ss.UPDATE_DATE = DateTime.Now;
                    ss.UPDATE_USER = 1;
                    SetEntryStateModified(ss);
                    Save();
                    succ = true;
                }
            }
            catch (Exception ex)
            {
                msg = $"update OTP RefNo [{refNo}] error. {ex.Message}";
                Logger.Error("Exception occur:\n", ex);
            }
            return succ;
        }

        public string GetSendOTPNextSeq(out string retMsg)
        {
            string seq = string.Empty;
            retMsg = string.Empty;
            try
            {
                ObjectParameter currDate = new ObjectParameter("o_currDate", typeof(DateTime));
                ObjectParameter nextSeq = new ObjectParameter("o_nextSeq", typeof(int));
                ObjectParameter succ = new ObjectParameter("o_succ", typeof(bool));
                ObjectParameter msg = new ObjectParameter("o_msg", typeof(string));
                _context.SP_GET_NEXT_OTP_SEQ(currDate, nextSeq, succ, msg);
                if ((bool)succ.Value)
                {
                    seq = $"{((DateTime)currDate.Value).ToString("yyMMdd")}{((int)nextSeq.Value).ToString("0000000")}";
                }
                else
                {
                    retMsg = msg.Value.ToString();
                    Logger.Error($"GetSendOTPNextSeq Error: {retMsg}\n");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetSendOTPNextSeq Error:\n", ex);
            }
            return seq;
        }

        private string FillEmailParameterPreProcessing(string original, int srId)
        {
            return original;
        }

        //private string FillEmailParameterPostProcessing(string original, string srNo, DateTime createDate, int statusId)
        //{
        //    var tmp = original;
        //    tmp = tmp.Replace("%SR_NO%", srNo);
        //    tmp = tmp.Replace("%CREATE_DATE%", string.Format(new CultureInfo("en-US"), "{0:yyyy-MM-dd HH:mm:ss}", createDate));
        //    tmp = tmp.Replace("%STATUS%", Constants.SRStatusId.GetStatusName(statusId));
        //    return tmp;
        //}

        private string FillEmailParameterPostProcessing(string original, ServiceRequestForDisplayEntity entity)
        {
            var tmp = original;
            tmp = tmp.Replace("%SR_NO%", entity.SrNo);
            tmp = tmp.Replace("%CREATE_DATE%", string.Format(new CultureInfo("en-US"), "{0:yyyy-MM-dd HH:mm:ss}", entity.CreateDate));
            tmp = tmp.Replace("%STATE%", entity.SRState.SRStateName);
            tmp = tmp.Replace("%STATUS%", entity.SRStatus.SRStatusName);
            tmp = tmp.Replace("%CPN_PRODUCT_GROUP_NAME%", entity.CPN_ProductGroupName);
            tmp = tmp.Replace("%CPN_PRODUCT_NAME%", entity.CPN_ProductName);
            tmp = tmp.Replace("%CPN_CAMPAIGN_NAME%", entity.CPN_CampaignName);
            tmp = tmp.Replace("%COMPLAINT_SUBJECT_NAME%", entity.CPN_SubjectName);
            tmp = tmp.Replace("%COMPLAINT_TYPE_NAME%", entity.CPN_TypeName);
            tmp = tmp.Replace("%ROOT_CAUSE_NAME%", entity.CPN_RootCauseName);
            tmp = tmp.Replace("%COMPLAINT_ISSUES_NAME%", entity.CPN_IssuesName);
            return tmp;
        }

        public void UpdateServiceRequest(ServiceRequestNoDetailEntity newValue, DateTime now)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var dbValue = _context.TB_T_SR.Single(s => s.SR_ID == newValue.SrId);

                if (dbValue.OWNER_USER_ID != newValue.OwnerUserId)
                {
                    // Change Owner
                    dbValue.OLD_OWNER_USER_ID = dbValue.OWNER_USER_ID;
                    dbValue.OWNER_USER_ID = newValue.OwnerUserId;
                    dbValue.OWNER_BRANCH_ID = newValue.OwnerBranchId;
                    dbValue.RULE_ASSIGN_FLAG = "0";
                    dbValue.UPDATE_DATE_BY_OWNER = now;
                }

                if (dbValue.DELEGATE_USER_ID != newValue.DelegateUserId)
                {
                    // Change Delegate
                    dbValue.OLD_DELEGATE_USER_ID = dbValue.DELEGATE_USER_ID;
                    dbValue.DELEGATE_USER_ID = newValue.DelegateUserId;
                    dbValue.DELEGATE_BRANCH_ID = newValue.DelegateBranchId;
                    dbValue.RULE_DELEGATE_FLAG = "1";
                    dbValue.RULE_DELEGATE_DATE = now;
                    dbValue.UPDATE_DATE_BY_DELEGATE = now;
                }

                if (dbValue.SR_STATUS_ID != newValue.SrStatusId)
                {
                    // Change Status
                    dbValue.OLD_SR_STATUS_ID = dbValue.SR_STATUS_ID;
                    dbValue.SR_STATUS_ID = newValue.SrStatusId;
                    dbValue.RULE_STATUS_DATE = now;
                }

                if (newValue.SrStatusId != null)
                {
                    if (newValue.SrStatusId == Constants.SRStatusId.Closed && !dbValue.CLOSE_DATE.HasValue)
                    {
                        dbValue.CLOSE_DATE = now;
                        dbValue.CLOSE_USER = newValue.CloseUserId;
                        dbValue.CLOSE_USERNAME = newValue.CloseUserName;
                    }
                }

                dbValue.RULE_EMAIL_FLAG = newValue.IsEmailDelegate ? "1" : "0";
                dbValue.UPDATE_DATE = now;

                if (newValue.OwnerUserId == newValue.CreatorUserId)
                    dbValue.UPDATE_DATE_BY_OWNER = now;

                if (newValue.DelegateUserId == newValue.CreatorUserId)
                    dbValue.UPDATE_DATE_BY_DELEGATE = now;

                SetEntryStateModified(dbValue);
                Save();
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public void UpdateSubmitCARStatusToActivity(int srActivityId, short status, string errorCode, string errorMessage)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var activity = _context.TB_T_SR_ACTIVITY.SingleOrDefault(a => a.SR_ACTIVITY_ID == srActivityId);
                if (activity == null)
                    throw new ArgumentException("Technical Error: Not found SR Activity in database. (TB_T_SR_ACTIVITY)");

                activity.ACTIVITY_CAR_SUBMIT_STATUS = status;
                if (status == 1)
                    activity.ACTIVITY_CAR_SUBMIT_DATE = DateTime.Now;

                if (errorCode.Length > 50)
                    errorCode = errorCode.Substring(0, 50);

                if (errorMessage.Length > 4000)
                    errorMessage = errorMessage.Substring(0, 4000);

                activity.ACTIVITY_CAR_SUBMIT_ERROR_CODE = errorCode;
                activity.ACTIVITY_CAR_SUBMIT_ERROR_DESC = errorMessage;

                SetEntryStateModified(activity);
                Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                //throw new Exception("Technical Error: Exception on update status of 'Submit CAR' to TB_T_SR_ACTIVITY", ex);
                throw;
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public void UpdateSubmitCBSHPStatusToActivity(int srActivityId, short? status, string errorCode, string errorMessage)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var activity = _context.TB_T_SR_ACTIVITY.SingleOrDefault(a => a.SR_ACTIVITY_ID == srActivityId);
                if (activity == null)
                    throw new ArgumentException("Technical Error: Not found SR Activity in database. (TB_T_SR_ACTIVITY)");

                activity.ACTIVITY_HP_SUBMIT_STATUS = status;
                if (status == 1)
                    activity.ACTIVITY_HP_SUBMIT_DATE = DateTime.Now;

                if (errorCode != null && errorCode.Length > 50)
                    errorCode = errorCode.Substring(0, 50);

                if (errorMessage != null && errorMessage.Length > 4000)
                    errorMessage = errorMessage.Substring(0, 4000);

                activity.ACTIVITY_HP_SUBMIT_ERROR_CODE = errorCode;
                activity.ACTIVITY_HP_SUBMIT_ERROR_DESC = errorMessage;

                SetEntryStateModified(activity);
                Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                //throw new Exception("Technical Error: Exception on update status of 'Submit CBS-HP (Log100)' to TB_T_SR_ACTIVITY", ex);
                throw;
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public TransferOwnerDelegateResult TransferOwnerDelegate(List<SrTransferEntity> transfers, int currentUserId)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var result = new TransferOwnerDelegateResult();

                foreach (var transfer in transfers)
                {
                    try
                    {
                        var sr = _context.TB_T_SR.SingleOrDefault(s => s.SR_ID == transfer.SrId);
                        if (sr == null)
                        {
                            Logger.ErrorFormat("SR_ID: {0} does not exist", transfer.SrId);

                            result.IsSuccess = false;
                            result.ErrorMessages.Add(string.Format("ไม่พบข้อมูล Service Request ในฐานข้อมูล (ID={0})", transfer.SrId));
                            return result;
                        }

                        var user = _context.TB_R_USER.SingleOrDefault(u => u.USER_ID == transfer.TransferToUserId);
                        if (user == null)
                        {
                            Logger.ErrorFormat("User: {0} does not exist", transfer.TransferToUserId);

                            result.IsSuccess = false;
                            result.ErrorMessages.Add(string.Format("ไม่พบข้อมูล User ในฐานข้อมูล (ID={0})", transfer.TransferToUserId));
                            return result;
                        }

                        const string currentUsername = "";

                        if (transfer.IsTransferOwner)
                        {
                            var oldOwnerUserId = sr.OWNER_USER_ID;
                            sr.OWNER_USER_ID = user.USER_ID;
                            sr.OWNER_BRANCH_ID = user.BRANCH_ID;

                            // for RULE
                            sr.OLD_OWNER_USER_ID = oldOwnerUserId;
                            sr.RULE_ASSIGN_FLAG = "0";

                            // Take Logging
                            CreateLoggingChangeOwner(sr.SR_ID, null, currentUserId, currentUsername, Constants.SystemName.CSM, oldOwnerUserId, transfer.TransferToUserId, false);
                        }
                        else
                        {
                            var oldDelegateUserId = sr.DELEGATE_USER_ID;
                            sr.DELEGATE_USER_ID = user.USER_ID;
                            sr.DELEGATE_BRANCH_ID = user.BRANCH_ID;

                            // for RULE
                            sr.OLD_DELEGATE_USER_ID = oldDelegateUserId;
                            sr.RULE_DELEGATE_FLAG = "1";
                            sr.RULE_DELEGATE_DATE = DateTime.Now;

                            // Take Logging
                            CreateLoggingDelegate(sr.SR_ID, null, currentUserId, currentUsername, Constants.SystemName.CSM, oldDelegateUserId, transfer.TransferToUserId, false);
                        }

                        sr.UPDATE_DATE = DateTime.Now;
                        sr.UPDATE_USER = currentUserId;

                        if (sr.OWNER_USER_ID == currentUserId)
                            sr.UPDATE_DATE_BY_OWNER = DateTime.Now;

                        if (sr.DELEGATE_USER_ID == currentUserId)
                            sr.UPDATE_DATE_BY_DELEGATE = DateTime.Now;

                        SetEntryStateModified(sr);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Exception occur:\n", ex);
                        result.IsSuccess = false;
                        result.ErrorMessages.Add("Technical Error: " + ex.Message);
                        return result;
                    }
                }

                Save();

                result.IsSuccess = true;
                return result;
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        #endregion

        #region == Logging ==

        public int CreateLoggingChangeStatus(int srId, int? srActivityId, int? createByUserId, string createUsername, string systemName, int? oldSrStatusId, int? newSrStatusId)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {

                var logging = new TB_L_SR_LOGGING();
                logging.SR_ID = srId;
                logging.SR_ACTIVITY_ID = srActivityId;
                logging.SR_LOGGING_SYSTEM_ACTION = systemName;
                logging.SR_LOGGING_ACTION = Constants.SrLogAction.ChangeStatus;
                logging.CREATE_USER = createByUserId;
                logging.CREATE_USERNAME = createUsername;
                logging.CREATE_DATE = DateTime.Now;
                logging.SR_STATUS_ID_OLD = oldSrStatusId;
                logging.SR_STATUS_ID_NEW = newSrStatusId;
                _context.TB_L_SR_LOGGING.Add(logging);

                var sr = _context.TB_T_SR.SingleOrDefault(x => x.SR_ID == srId);

                if (sr.OWNER_BRANCH_ID.HasValue && sr != null)
                {
                    CalculateTotalSLA(sr.OWNER_BRANCH_ID.Value, sr, logging, DateTime.Now);
                    logging.WORKING_HOUR = (int)CalTotalWorkingHour(sr.OWNER_BRANCH_ID.Value, srId, srActivityId ?? 0);
                }

                SetEntryStateModified(sr);
                Save();

                return logging.SR_LOGGING_ID;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                //throw new Exception("Technical Error: Exception on Create Logging: (Action=Change Status, SR ID=" + srId + ")", ex);
                throw;
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        private void CalculateTotalSLA(int branchId, TB_T_SR sr, TB_L_SR_LOGGING logging, DateTime createdDate)
        {
            try
            {
                DateTime currentDate = DeleteSeconds(createdDate);
                int thisWork = sr.RULE_THIS_WORK ?? 0;
                int thisAlert = sr.RULE_THIS_ALERT ?? 0;

                int workingMinPerDay = 0;
                int startTimeHour = 0;
                int startTimeMin = 0;
                int endTimeHour = 0;
                int endTimeMin = 0;

                var calendarTab = _context.TB_R_BRANCH_CALENDAR.Where(p => p.BRANCH_ID == branchId).ToList();
                var branch = _context.TB_R_BRANCH.Where(p => p.BRANCH_ID == branchId).FirstOrDefault();

                if (branch == null) { throw new ArgumentException("Not Found Branch ID : " + branchId); }

                if (!branch.START_TIME_HOUR.HasValue
                    || !branch.START_TIME_MINUTE.HasValue
                    || !branch.END_TIME_HOUR.HasValue
                    || !branch.END_TIME_MINUTE.HasValue)
                {
                    string start = _context.TB_C_RULE_OPTION.Where(p => p.OPTION_CODE.ToUpper() == "STARTTIME").Select(p => p.OPTION_DESC).FirstOrDefault();
                    string end = _context.TB_C_RULE_OPTION.Where(p => p.OPTION_CODE.ToUpper() == "ENDTIME").Select(p => p.OPTION_DESC).FirstOrDefault();

                    if (start != null)
                    {
                        string[] str = start.Split(':');
                        if (str.Count() == 2 && str[0].Trim() != "" && str[1].Trim() != "")
                        {
                            startTimeHour = Convert.ToInt32(str[0]);
                            startTimeMin = Convert.ToInt32(str[1]);
                        }
                        else
                            throw new ArgumentException("Invalid: " + branch.BRANCH_NAME);
                    }
                    else
                        throw new ArgumentException("Invalid: " + branch.BRANCH_NAME);

                    if (end != null)
                    {
                        string[] str = end.Split(':');
                        if (str.Count() == 2 && str[0].Trim() != "" && str[1].Trim() != "")
                        {
                            endTimeHour = Convert.ToInt32(str[0]);
                            endTimeMin = Convert.ToInt32(str[1]);
                        }
                        else
                            throw new ArgumentException("Invalid: " + branch.BRANCH_NAME);
                    }
                    else
                        throw new ArgumentException("Invalid: " + branch.BRANCH_NAME);
                }
                else
                {
                    startTimeHour = Convert.ToInt32(branch.START_TIME_HOUR ?? 0);
                    startTimeMin = Convert.ToInt32(branch.START_TIME_MINUTE ?? 0);
                    endTimeHour = Convert.ToInt32(branch.END_TIME_HOUR ?? 0);
                    endTimeMin = Convert.ToInt32(branch.END_TIME_MINUTE ?? 0);

                    DateTime tmpStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startTimeHour, startTimeMin, 0);
                    DateTime tmpEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, endTimeHour, endTimeMin, 0);

                    TimeSpan tmpTs = tmpEnd.Subtract(tmpStart);
                    workingMinPerDay = Convert.ToInt32(tmpTs.TotalMinutes);
                }

                DateTime currentSla;
                if (thisAlert == 0)
                {
                    currentSla = sr.RULE_STATUS_DATE != null ? DeleteSeconds(sr.RULE_STATUS_DATE.Value) : currentDate;
                }
                else
                {
                    currentSla = sr.RULE_CURRENT_SLA != null ? DeleteSeconds(sr.RULE_CURRENT_SLA.Value) : currentDate;
                }

                int checkStartTime = Convert.ToInt32(startTimeHour.ToString("00") + startTimeMin.ToString("00"));
                int checkEndTime = Convert.ToInt32(endTimeHour.ToString("00") + endTimeMin.ToString("00"));
                int timeToCheck = Convert.ToInt32(currentSla.Hour.ToString("00") + currentSla.Minute.ToString("00"));

                if (timeToCheck < checkStartTime || timeToCheck > checkEndTime)
                {
                    if (timeToCheck >= checkEndTime && timeToCheck <= 2359)
                    {
                        currentSla = currentSla.AddDays(1);
                        currentSla = new DateTime(currentSla.Year, currentSla.Month, currentSla.Day, startTimeHour, startTimeMin, 0);
                    }
                    else
                        currentSla = new DateTime(currentSla.Year, currentSla.Month, currentSla.Day, startTimeHour, startTimeMin, 0);
                }

                while (calendarTab.Where(p => p.HOLIDAY_DATE.Date == currentSla.Date).Count() > 0)
                {
                    currentSla = currentSla.AddDays(1);
                }

                timeToCheck = Convert.ToInt32(currentDate.Hour.ToString("00") + currentDate.Minute.ToString("00"));

                if (timeToCheck < checkStartTime || timeToCheck > checkEndTime)
                {
                    if (timeToCheck >= checkEndTime && timeToCheck <= 2359)
                    {
                        currentDate = currentDate.AddDays(1);
                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, startTimeHour, startTimeMin, 0);
                    }
                    else
                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, startTimeHour, startTimeMin, 0);
                }

                while (calendarTab.Where(p => p.HOLIDAY_DATE.Date == currentDate.Date).Count() > 0)
                {
                    currentDate = currentDate.AddDays(1);
                }

                if (currentSla.Date == currentDate.Date)
                {
                    TimeSpan ts = currentDate.Subtract(currentSla);
                    thisWork += Convert.ToInt32(ts.TotalMinutes);
                }
                else
                {
                    DateTime startDate = currentSla.Date;
                    DateTime endDate = currentDate.Date;

                    DateTime tmpDate = startDate.AddDays(1);
                    while (tmpDate < endDate)
                    {
                        if (calendarTab.Where(p => p.HOLIDAY_DATE == tmpDate).Count() == 0)
                        {
                            thisWork += workingMinPerDay;
                        }
                        tmpDate = tmpDate.AddDays(1);
                    }

                    DateTime endWorkTime_for_currentSla = new DateTime(currentSla.Year, currentSla.Month, currentSla.Day, endTimeHour, endTimeMin, 0);
                    DateTime startWorkTime_for_currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, startTimeHour, startTimeMin, 0);

                    TimeSpan ts = endWorkTime_for_currentSla.Subtract(currentSla);
                    thisWork += Convert.ToInt32(ts.TotalMinutes);

                    ts = currentDate.Subtract(startWorkTime_for_currentDate);
                    thisWork += Convert.ToInt32(ts.TotalMinutes);
                }

                sr.RULE_TOTAL_ALERT = (sr.RULE_TOTAL_ALERT ?? 0) + thisAlert;
                sr.RULE_TOTAL_WORK = (sr.RULE_TOTAL_WORK ?? 0) + thisWork;
                sr.RULE_THIS_ALERT = 0;
                sr.RULE_THIS_WORK = 0;
                sr.RULE_CURRENT_SLA = null;

                var slaMinute = GetSlaMinute(sr.PRODUCT_ID, sr.CAMPAIGNSERVICE_ID, sr.AREA_ID, sr.SUBAREA_ID, sr.TYPE_ID, sr.CHANNEL_ID, sr.SR_STATUS_ID);

                if (slaMinute.HasValue && thisWork >= slaMinute.Value)
                    logging.OVER_SLA_MINUTE = thisWork - slaMinute.Value;
                else
                    logging.OVER_SLA_MINUTE = null;

                logging.OVER_SLA_TIMES = thisAlert;
                logging.WORKING_MINUTE = thisWork;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw;
            }
        }

        //SR_ACTIVITY.WORKING_MINUTE
        private double CalTotalWorkingMinute(int branchId, int srId, DateTime currAct)
        {
            double workMin = 0;
            var branch = _context.TB_R_BRANCH.Where(p => p.BRANCH_ID == branchId).FirstOrDefault();
            if (branch == null) { throw new Exception($"Not Found Branch ID : {branchId}."); }

            var lastSRAct = (from a in _context.TB_T_SR_ACTIVITY.AsNoTracking()
                             where a.SR_ID == srId
                             orderby a.CREATE_DATE descending
                             select a).FirstOrDefault();
            if (lastSRAct != null)
            {
                DateTime lastAct = DeleteSeconds(lastSRAct.CREATE_DATE ?? DateTime.MinValue);
                currAct = DeleteSeconds(currAct);
                workMin = SumWorkMinuteByCalendar(branch, lastAct, currAct);
            }
            return workMin;
        }

        //SR_LOGGING.WORKING_HOUR
        private double CalTotalWorkingHour(int branchId, int srId, int srActivityId)
        {
            double workMin = 0;
            var branch = _context.TB_R_BRANCH.Where(p => p.BRANCH_ID == branchId).FirstOrDefault();
            if (branch == null) { throw new Exception($"Not Found Branch ID : {branchId}."); }

            var lastRec = (from a in _context.TB_L_SR_LOGGING.AsNoTracking()
                           from b in _context.TB_T_SR_ACTIVITY.Where(x => x.SR_ACTIVITY_ID == a.SR_ACTIVITY_ID)
                           where a.SR_ID == srId && a.SR_LOGGING_ACTION == Constants.SrLogAction.ChangeStatus
                           orderby a.CREATE_DATE descending
                           select b).FirstOrDefault();
            if (lastRec != null)
            {
                DateTime lastAct = DeleteSeconds(lastRec.CREATE_DATE ?? DateTime.MinValue);
                DateTime currAct = DeleteSeconds(_context.TB_T_SR_ACTIVITY.AsNoTracking()
                                                    .Where(x => x.SR_ACTIVITY_ID == srActivityId)
                                                    .Select(x => x.CREATE_DATE.Value).FirstOrDefault());
                workMin = SumWorkMinuteByCalendar(branch, lastAct, currAct);
            }
            return workMin;
        }

        double SumWorkMinuteByCalendar(TB_R_BRANCH branch, DateTime lastAct, DateTime currAct)
        {
            double workMin = 0;
            int branchStartHour, branchStartMin, branchEndHour, branchEndMin;
            GetBranchWorkHour(branch, out branchStartHour, out branchStartMin, out branchEndHour, out branchEndMin);

            var calendarTab = _context.TB_R_BRANCH_CALENDAR.Where(p => p.BRANCH_ID == branch.BRANCH_ID).ToList();

            TimeSpan tsWorkMin;
            DateTime startWorkHour;
            DateTime endWorkHour;
            if (lastAct.Date == currAct.Date)
            {
                //lastAct เริ่มหลังเลิกงาน และเป็นวันเดียวกันกับ currAct
                if (lastAct > GenTimeOfDate(lastAct, branchEndHour, branchEndMin))
                { return 0; }
                startWorkHour = GetWorkingHourDatetime(lastAct, branchStartHour, branchStartMin, branchEndHour, branchEndMin);
                endWorkHour = GetWorkingHourDatetime(currAct, branchStartHour, branchStartMin, branchEndHour, branchEndMin);
                tsWorkMin = endWorkHour - startWorkHour;
                workMin = tsWorkMin.TotalMinutes >= 0 ? tsWorkMin.TotalMinutes : 0;
            }
            else
            {
                if (lastAct.Date < currAct.Date)
                {
                    //lastAct หลังสุดเริ่มหลังเลิกงาน
                    if (lastAct > GenTimeOfDate(lastAct, branchEndHour, branchEndMin))
                    {
                        lastAct = lastAct.AddDays(1);
                        lastAct = new DateTime(lastAct.Year, lastAct.Month, lastAct.Day, branchStartHour, branchStartMin, 0);
                        while (calendarTab.Where(p => p.HOLIDAY_DATE.Date == lastAct.Date).Count() > 0)
                        {
                            lastAct = lastAct.AddDays(1);
                        }
                    }
                    workMin = 0;
                    while (lastAct.Date <= currAct.Date)
                    {
                        startWorkHour = GetWorkingHourDatetime(lastAct, branchStartHour, branchStartMin, branchEndHour, branchEndMin);

                        if (lastAct.Date == currAct.Date)
                            endWorkHour = GetWorkingHourDatetime(currAct, branchStartHour, branchStartMin, branchEndHour, branchEndMin);
                        else
                            endWorkHour = new DateTime(lastAct.Year, lastAct.Month, lastAct.Day, branchEndHour, branchEndMin, 0);

                        tsWorkMin = endWorkHour - startWorkHour;
                        workMin += tsWorkMin.TotalMinutes >= 0 ? tsWorkMin.TotalMinutes : 0;

                        lastAct = lastAct.AddDays(1);
                        lastAct = new DateTime(lastAct.Year, lastAct.Month, lastAct.Day, branchStartHour, branchStartMin, 0);
                        while (calendarTab.Where(p => p.HOLIDAY_DATE.Date == lastAct.Date).Count() > 0)
                        {
                            lastAct = lastAct.AddDays(1);
                        }
                    }
                }
            }
            return workMin;
        }

        private void GetBranchWorkHour(TB_R_BRANCH branch, out int branchStartHour, out int branchStartMin, out int branchEndHour, out int branchEndMin)
        {
            if (!branch.START_TIME_HOUR.HasValue
                || !branch.START_TIME_MINUTE.HasValue
                || !branch.END_TIME_HOUR.HasValue
                || !branch.END_TIME_MINUTE.HasValue)
            {
                string start = _context.TB_C_RULE_OPTION.Where(p => p.OPTION_CODE.ToUpper() == "STARTTIME").Select(p => p.OPTION_DESC).FirstOrDefault();
                string end = _context.TB_C_RULE_OPTION.Where(p => p.OPTION_CODE.ToUpper() == "ENDTIME").Select(p => p.OPTION_DESC).FirstOrDefault();

                if (start != null)
                {
                    string[] str = start.Split(':');
                    if (str.Count() == 2 && str[0].Trim() != "" && str[1].Trim() != "")
                    {
                        branchStartHour = Convert.ToInt32(str[0]);
                        branchStartMin = Convert.ToInt32(str[1]);
                    }
                    else
                        throw new Exception("Invalid branch start work hour: " + branch.BRANCH_NAME);
                }
                else
                    throw new Exception("Invalid branch start work hour: " + branch.BRANCH_NAME);

                if (end != null)
                {
                    string[] str = end.Split(':');
                    if (str.Count() == 2 && str[0].Trim() != "" && str[1].Trim() != "")
                    {
                        branchEndHour = Convert.ToInt32(str[0]);
                        branchEndMin = Convert.ToInt32(str[1]);
                    }
                    else
                        throw new Exception("Invalid branch end work hour: " + branch.BRANCH_NAME);
                }
                else
                    throw new Exception("Invalid branch end work hour: " + branch.BRANCH_NAME);
            }
            else
            {
                branchStartHour = Convert.ToInt32(branch.START_TIME_HOUR ?? 0);
                branchStartMin = Convert.ToInt32(branch.START_TIME_MINUTE ?? 0);
                branchEndHour = Convert.ToInt32(branch.END_TIME_HOUR ?? 0);
                branchEndMin = Convert.ToInt32(branch.END_TIME_MINUTE ?? 0);
            }
        }

        private DateTime GetWorkingHourDatetime(DateTime actDateTime, int startHour, int startMin, int endHour, int endMin)
        {
            DateTime workHour;
            string beg = string.Format("{0:00}{1:00}", startHour, startMin);
            string end = string.Format("{0:00}{1:00}", endHour, endMin);

            string actTime = string.Format("{0:00}{1:00}", actDateTime.Hour, actDateTime.Minute);

            if (actTime.CompareTo(beg) >= 0 && actTime.CompareTo(end) < 0)
            { workHour = actDateTime; }
            else
            {
                if (actTime.CompareTo(beg) < 0)
                    workHour = GenTimeOfDate(actDateTime.Date, startHour, startMin);
                else
                    workHour = GenTimeOfDate(actDateTime.Date, endHour, endMin);
            }
            return workHour;
        }

        private DateTime GenTimeOfDate(DateTime date, int hour, int minute)
        {
            return new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
        }

        private int? GetSlaMinute(int? productId, int? campaignServiceId, int? areaId, int? subAreaId, int? typeId, int? channelId, int? srStatusId)
        {
            if (!productId.HasValue || !campaignServiceId.HasValue
                || !areaId.HasValue || !subAreaId.HasValue || !typeId.HasValue
                || !channelId.HasValue || !srStatusId.HasValue)
            {
                return null;
            }

            var productIdValue = productId.Value;
            var campaignServiceIdValue = campaignServiceId.Value;
            var areaIdValue = areaId.Value;
            var subAreaIdValue = subAreaId.Value;
            var typeIdValue = typeId.Value;
            var channelIdValue = channelId.Value;
            var srStatusIdValue = srStatusId.Value;

            // Check Set All Value
            var slaMinute = (from sla in _context.TB_M_SLA
                             where sla.PRODUCT_ID == productIdValue
                                   && sla.CAMPAIGNSERVICE_ID == campaignServiceIdValue
                                   && sla.AREA_ID == areaIdValue
                                   && sla.SUBAREA_ID == subAreaIdValue
                                   && sla.TYPE_ID == typeIdValue
                                   && sla.CHANNEL_ID == channelIdValue
                                   && sla.SR_STATUS_ID == srStatusIdValue
                                   && sla.SLA_IS_ACTIVE
                             select new
                             {
                                 sla.SLA_ID,
                                 sla.SLA_MINUTE
                             }).FirstOrDefault();

            if (slaMinute != null)
                return slaMinute.SLA_MINUTE;

            // Check No Set Campaign/Service
            slaMinute = (from sla in _context.TB_M_SLA
                         where sla.PRODUCT_ID == productIdValue
                               && sla.AREA_ID == areaIdValue
                               && sla.SUBAREA_ID == subAreaIdValue
                               && sla.TYPE_ID == typeIdValue
                               && sla.CHANNEL_ID == channelIdValue
                               && sla.SR_STATUS_ID == srStatusIdValue
                               && sla.SLA_IS_ACTIVE
                         select new
                         {
                             sla.SLA_ID,
                             sla.SLA_MINUTE
                         }).FirstOrDefault();

            if (slaMinute != null)
                return slaMinute.SLA_MINUTE;

            // Check No Set SubArea
            slaMinute = (from sla in _context.TB_M_SLA
                         where sla.PRODUCT_ID == productIdValue
                               && sla.CAMPAIGNSERVICE_ID == campaignServiceIdValue
                               && sla.AREA_ID == areaIdValue
                               && sla.TYPE_ID == typeIdValue
                               && sla.CHANNEL_ID == channelIdValue
                               && sla.SR_STATUS_ID == srStatusIdValue
                               && sla.SLA_IS_ACTIVE
                         select new
                         {
                             sla.SLA_ID,
                             sla.SLA_MINUTE
                         }).FirstOrDefault();

            if (slaMinute != null)
                return slaMinute.SLA_MINUTE;

            // Check No Set Campaign/Service & SubArea
            slaMinute = (from sla in _context.TB_M_SLA
                         where sla.PRODUCT_ID == productIdValue
                               && sla.AREA_ID == areaIdValue
                               && sla.TYPE_ID == typeIdValue
                               && sla.CHANNEL_ID == channelIdValue
                               && sla.SR_STATUS_ID == srStatusIdValue
                               && sla.SLA_IS_ACTIVE
                         select new
                         {
                             sla.SLA_ID,
                             sla.SLA_MINUTE
                         }).FirstOrDefault();

            if (slaMinute != null)
                return slaMinute.SLA_MINUTE;

            return null;
        }

        private DateTime DeleteSeconds(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
        }

        public int CreateLoggingChangeOwner(int srId, int? srActivityId, int? createByUserId, string createUsername, string systemName, int? oldOwnerUserId, int? newOwnerUserId, bool isAutoSave = true)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var log = new TB_L_SR_LOGGING();
                log.SR_ID = srId;
                log.SR_ACTIVITY_ID = srActivityId;
                log.SR_LOGGING_SYSTEM_ACTION = systemName;
                log.SR_LOGGING_ACTION = Constants.SrLogAction.ChangeOwner;
                log.CREATE_USER = createByUserId;
                log.CREATE_USERNAME = createUsername;
                log.CREATE_DATE = DateTime.Now;
                log.OWNER_USER_ID_OLD = oldOwnerUserId;
                log.OWNER_USER_ID_NEW = newOwnerUserId;

                _context.TB_L_SR_LOGGING.Add(log);

                if (isAutoSave)
                    Save();

                return log.SR_LOGGING_ID;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                //throw new Exception("Technical Error: Exception on Create Logging: (Action=Change Owner, SR ID=" + srId + ")", ex);
                throw;
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public int CreateLoggingDelegate(int srId, int? srActivityId, int? createByUserId, string createUsername, string systemName, int? oldDelegateUserId, int? newDelegateOwnerUserId, bool isAutoSave = true)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var log = new TB_L_SR_LOGGING();
                log.SR_ID = srId;
                log.SR_ACTIVITY_ID = srActivityId;
                log.SR_LOGGING_SYSTEM_ACTION = systemName;
                log.SR_LOGGING_ACTION = Constants.SrLogAction.Delegate;
                log.CREATE_USER = createByUserId;
                log.CREATE_USERNAME = createUsername;
                log.CREATE_DATE = DateTime.Now;
                log.DELEGATE_USER_ID_OLD = oldDelegateUserId;
                log.DELEGATE_USER_ID_NEW = newDelegateOwnerUserId;
                _context.TB_L_SR_LOGGING.Add(log);

                if (isAutoSave)
                    Save();

                return log.SR_LOGGING_ID;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new Exception("Technical Error: Exception on Create Logging: (Action=Delegate, SR ID=" + srId + ")", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public int CreateSRLogging(string srLogType, int srId, int? srActivityId, int? createByUserId
            , string createUsername, string systemName, int? oldId, int? newId)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var log = new TB_L_SR_LOGGING();
                log.SR_ID = srId;
                log.SR_ACTIVITY_ID = srActivityId;
                log.SR_LOGGING_SYSTEM_ACTION = systemName;
                log.SR_LOGGING_ACTION = srLogType;
                log.CREATE_USER = createByUserId;
                log.CREATE_USERNAME = createUsername;
                log.CREATE_DATE = DateTime.Now;
                log.INT_ID_OLD = oldId;
                log.INT_ID_NEW = newId;
                _context.TB_L_SR_LOGGING.Add(log);

                Save();

                return log.SR_LOGGING_ID;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new Exception("Technical Error: Exception on Create Logging: (Action=Delegate, SR ID=" + srId + ")", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public int CreateSRLogging(string srLogType, int srId, int? srActivityId, int? createByUserId
            , string createUsername, string systemName, bool? oldVal, bool? newVal)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var log = new TB_L_SR_LOGGING();
                log.SR_ID = srId;
                log.SR_ACTIVITY_ID = srActivityId;
                log.SR_LOGGING_SYSTEM_ACTION = systemName;
                log.SR_LOGGING_ACTION = srLogType;
                log.CREATE_USER = createByUserId;
                log.CREATE_USERNAME = createUsername;
                log.CREATE_DATE = DateTime.Now;
                log.BIT_VALUE_OLD = oldVal;
                log.BIT_VALUE_NEW = newVal;
                _context.TB_L_SR_LOGGING.Add(log);

                Save();

                return log.SR_LOGGING_ID;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                //throw new Exception("Technical Error: Exception on Create Logging: (Action=Delegate, SR ID=" + srId + ")", ex);
                throw;
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        #endregion

        public List<AccountAddressTypeEntity> AddressTypeList()
        {
            return (from add in _context.TB_M_ACCOUNT_ADDRESS
                    select new AccountAddressTypeEntity
                    {
                        code = add.ADDRESS_TYPE_CODE.Trim(),
                        name = add.ADDRESS_TYPE_NAME.Trim()
                    }).Distinct().OrderBy(c => c.name).ToList();
        }

        public IEnumerable<AccountAddressSearchResult> SearchAccountAddress(AccountAddressSearchFilter searchFilter)
        {
            var query = _context.TB_M_ACCOUNT_ADDRESS.AsQueryable();
            query = query.Where(q => q.CUSTOMER_ID == searchFilter.CustomerId);

            //Filter: Type
            if (!string.IsNullOrEmpty(searchFilter.Type))
            {
                query = query.Where(q => q.ADDRESS_TYPE_NAME.Contains(searchFilter.Type));
            }

            //Filter: Address
            if (!string.IsNullOrEmpty(searchFilter.Address))
            {
                query =
                    query.Where(
                        q => q.ADDRESS_NO.Contains(searchFilter.Address) || q.VILLAGE.Contains(searchFilter.Address) ||
                             q.BUILDING.Contains(searchFilter.Address) || q.FLOOR_NO.Contains(searchFilter.Address) ||
                             q.ROOM_NO.Contains(searchFilter.Address) ||
                             q.MOO.Contains(searchFilter.Address) || q.STREET.Contains(searchFilter.Address) ||
                             q.SOI.Contains(searchFilter.Address) ||
                             q.SUB_DISTRICT.Contains(searchFilter.Address) || q.DISTRICT.Contains(searchFilter.Address) ||
                             q.PROVINCE.Contains(searchFilter.Address) ||
                             q.COUNTRY.Contains(searchFilter.Address) || q.POSTCODE.Contains(searchFilter.Address));
            }

            var resultQuery = query.Select(q => new AccountAddressSearchResult
            {
                AccountAddressId = q.ADDRESS_ID,
                Type = q.ADDRESS_TYPE_NAME,
                AddressNo = q.ADDRESS_NO,
                Village = q.VILLAGE,
                Building = q.BUILDING,
                FloorNo = q.FLOOR_NO,
                RoomNo = q.ROOM_NO,
                Moo = q.MOO,
                Street = q.STREET,
                Soi = q.SOI,
                SubDistrict = q.SUB_DISTRICT,
                District = q.DISTRICT,
                Province = q.PROVINCE,
                Postcode = q.POSTCODE,
                Country = q.COUNTRY,
                Product = q.TB_M_ACCOUNT.SUBSCRIPTION_DESC,
                AccountNo = q.TB_M_ACCOUNT.ACCOUNT_NO
            });

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = resultQuery.Count();

            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            resultQuery = SetAccountAddressListSort(resultQuery, searchFilter);
            return resultQuery.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();
        }

        public IQueryable<AccountAddressSearchResult> SetAccountAddressListSort(IQueryable<AccountAddressSearchResult> addressList,
            AccountAddressSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper().Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper())
                {
                    default:
                        return addressList.OrderBy(a => a.Type);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper())
                {
                    default:
                        return addressList.OrderByDescending(a => a.Type);
                }
            }
        }

        public IEnumerable<ServiceRequestCustomerSearchResult> SearchCustomer(CustomerSearchFilter searchFilter)
        {
            var query = _context.TB_M_ACCOUNT.AsQueryable();

            // Filter: Status
            // query = query.Where(q => q.STATUS == "1");

            // Filter: FirstName
            if (!string.IsNullOrEmpty(searchFilter.FirstName))
            {
                var firstName = searchFilter.FirstName.Trim();

                if (firstName.StartsWith("*") && firstName.EndsWith("*"))
                {
                    var keyword = firstName.Replace("*", "");
                    query = query.Where(q => q.TB_M_CUSTOMER.FIRST_NAME_EN.Contains(keyword) || q.TB_M_CUSTOMER.FIRST_NAME_TH.Contains(keyword));
                }
                else if (firstName.StartsWith("*"))
                {
                    var keyword = firstName.Replace("*", "");
                    query = query.Where(q => q.TB_M_CUSTOMER.FIRST_NAME_EN.EndsWith(keyword) || q.TB_M_CUSTOMER.FIRST_NAME_TH.EndsWith(keyword));
                }
                else if (firstName.EndsWith("*"))
                {
                    var keyword = firstName.Replace("*", "");
                    query = query.Where(q => q.TB_M_CUSTOMER.FIRST_NAME_EN.StartsWith(keyword) || q.TB_M_CUSTOMER.FIRST_NAME_TH.StartsWith(keyword));
                }
                else
                {
                    var keyword = firstName.Replace("*", "");
                    query = query.Where(q => q.TB_M_CUSTOMER.FIRST_NAME_EN == firstName || q.TB_M_CUSTOMER.FIRST_NAME_TH == firstName);
                }
            }

            // Filter: LastName
            if (!string.IsNullOrEmpty(searchFilter.LastName))
            {
                var lastName = searchFilter.LastName.Trim();

                if (lastName.StartsWith("*") && lastName.EndsWith("*"))
                {
                    var keyword = lastName.Replace("*", "");
                    query = query.Where(q => q.TB_M_CUSTOMER.LAST_NAME_EN.Contains(keyword) || q.TB_M_CUSTOMER.LAST_NAME_TH.Contains(keyword));
                }
                else if (lastName.StartsWith("*"))
                {
                    var keyword = lastName.Replace("*", "");
                    query = query.Where(q => q.TB_M_CUSTOMER.LAST_NAME_EN.EndsWith(keyword) || q.TB_M_CUSTOMER.LAST_NAME_TH.EndsWith(keyword));
                }
                else if (lastName.EndsWith("*"))
                {
                    var keyword = lastName.Replace("*", "");
                    query = query.Where(q => q.TB_M_CUSTOMER.LAST_NAME_EN.StartsWith(keyword) || q.TB_M_CUSTOMER.LAST_NAME_TH.StartsWith(keyword));
                }
                else
                {
                    var keyword = lastName.Replace("*", "");
                    query = query.Where(q => q.TB_M_CUSTOMER.LAST_NAME_EN == lastName || q.TB_M_CUSTOMER.LAST_NAME_TH == lastName);
                }
            }

            // Filter: CardNo
            if (!string.IsNullOrEmpty(searchFilter.CardNo))
            {
                query = query.Where(q => q.TB_M_CUSTOMER.CARD_NO == searchFilter.CardNo);
            }

            // Filter: CarNo
            if (!string.IsNullOrEmpty(searchFilter.Registration))
            {
                query = query.Where(q => q.CAR_NO == searchFilter.Registration);
            }

            // Filter: Account No
            if (!string.IsNullOrEmpty(searchFilter.AccountNo))
            {
                query = query.Where(q => q.ACCOUNT_NO == searchFilter.AccountNo);
            }

            // Filter: PhoneNo
            if (!string.IsNullOrEmpty(searchFilter.PhoneNo))
            {
                //query = query.Where(q => q.TB_M_CUSTOMER.TB_M_PHONE.Any(p => p.PHONE_NO == searchFilter.PhoneNo));
                string phoneNo = searchFilter.PhoneNo.Replace("*", string.Empty);
               query = query.Where(q => q.TB_M_CUSTOMER.TB_M_PHONE.Any(p => p.PHONE_NO.Contains(phoneNo)));
            }

            var resultQuery = query.Select(q => new ServiceRequestCustomerSearchResult
            {
                CustomerId = q.CUSTOMER_ID ?? 0,
                AccountId = q.ACCOUNT_ID,
                CardNo = q.TB_M_CUSTOMER.CARD_NO,
                CustomerFirstNameTh = q.TB_M_CUSTOMER.FIRST_NAME_TH,
                CustomerLastNameTh = q.TB_M_CUSTOMER.LAST_NAME_TH,
                CustomerFirstNameEn = q.TB_M_CUSTOMER.FIRST_NAME_EN,
                CustomerLastNameEn = q.TB_M_CUSTOMER.LAST_NAME_EN,
                AccountNo = q.ACCOUNT_NO,
                //PhoneList = q.TB_M_CUSTOMER.TB_M_PHONE.Select(x => x.PHONE_NO).ToList(),
                CarNo = q.CAR_NO,
                AccountStatus = q.STATUS,
                ProductName = q.SUBSCRIPTION_DESC,
                BranchName = q.BRANCH_NAME,
                CustomerType = string.Empty,
                SubscriptionTypeName = q.TB_M_CUSTOMER.TB_M_SUBSCRIPT_TYPE.SUBSCRIPT_TYPE_NAME,
                PhoneList = q.TB_M_CUSTOMER.TB_M_PHONE.Where(x => x.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != "05").Select(item => new ServiceRequestCustomerAccountPhone()
                {
                    PhoneNo = item.PHONE_NO,
                    PhoneTypeCode = "",
                    PhoneTypeName = ""
                }).Take(3).ToList()
            });

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = resultQuery.Count();

            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            resultQuery = SetCustomerListSort(resultQuery, searchFilter);

            var tmp = resultQuery.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();

            // Solution 1:
            //            foreach (var item in tmp)
            //            {
            //                item.PhoneNo = string.Join(",", _context.TB_M_PHONE.Where(x => x.CUSTOMER_ID == item.CustomerId && x.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != "05").Select(x => x.PHONE_NO).ToList());
            //            }

            // Solution 2 (More Response Time):
            //            var customerIdList = tmp.Select(x => x.CustomerId).ToList();
            //
            //            var phoneList = _context.TB_M_PHONE.Where(x => customerIdList.Contains(x.CUSTOMER_ID.Value) && x.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != "05").Select(x => new { x.CUSTOMER_ID, x.PHONE_NO } );
            //
            //            foreach (var item in tmp)
            //            {
            //                item.PhoneNo = string.Join(",", phoneList.Where(x => item.CustomerId == x.CUSTOMER_ID.Value).Select(x => x.PHONE_NO).Distinct().Take(3).ToList());
            //            }

            foreach (var item in tmp)
            {
                item.PhoneNo = string.Join(",", item.PhoneList.Select(x => x.PhoneNo).ToList());
            }

            return tmp;
        }

        public IQueryable<ServiceRequestCustomerSearchResult> SetCustomerListSort(IQueryable<ServiceRequestCustomerSearchResult> customerList,
            CustomerSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper().Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper())
                {
                    default:
                        return customerList.OrderBy(a => a.CardNo);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper())
                {
                    default:
                        return customerList.OrderByDescending(a => a.CardNo);
                }
            }
        }

        public ServiceRequestCustomerAccount GetCustomerAccount(int accountId)
        {
            var query = from a in _context.TB_M_ACCOUNT
                        from c in _context.TB_M_CUSTOMER.Where(x => x.CUSTOMER_ID == a.CUSTOMER_ID).DefaultIfEmpty()
                        from st in _context.TB_M_SUBSCRIPT_TYPE.Where(x => x.SUBSCRIPT_TYPE_ID == c.SUBSCRIPT_TYPE_ID).DefaultIfEmpty()
                        from titleTH in _context.TB_M_TITLE.Where(x => x.TITLE_ID == c.TITLE_TH_ID).DefaultIfEmpty()
                        from titleEN in _context.TB_M_TITLE.Where(x => x.TITLE_ID == c.TITLE_EN_ID).DefaultIfEmpty()
                        where a.ACCOUNT_ID == accountId
                        select new ServiceRequestCustomerAccount
                        {
                            Account = new AccountEntity()
                            {
                                AccountId = a.ACCOUNT_ID,
                                AccountNo = a.ACCOUNT_NO,
                                Status = a.STATUS,
                                ProductGroup = a.PRODUCT_GROUP,
                                Product = a.SUBSCRIPTION_DESC,
                                CarNo = a.CAR_NO,
                                BranchCode = a.BRANCH_CODE,
                                BranchName = a.BRANCH_NAME
                            },
                            Customer = new CustomerEntity()
                            {
                                CustomerId = c.CUSTOMER_ID,
                                FirstNameThai = c.FIRST_NAME_TH,
                                FirstNameEnglish = c.FIRST_NAME_EN,
                                LastNameThai = c.LAST_NAME_TH,
                                LastNameEnglish = c.LAST_NAME_EN,
                                CardNo = c.CARD_NO,
                                BirthDate = c.BIRTH_DATE,
                                Email = c.EMAIL,
                                Fax = (from p in c.TB_M_PHONE
                                       where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE == Constants.PhoneTypeCode.Fax
                                       select new { p.PHONE_NO }
                                      ).FirstOrDefault().PHONE_NO,
                                SubscriptType = (st != null ? new SubscriptTypeEntity
                                {
                                    SubscriptTypeId = st.SUBSCRIPT_TYPE_ID,
                                    SubscriptTypeName = st.SUBSCRIPT_TYPE_NAME,
                                } : null),
                                TitleThai = (titleTH != null ? new TitleEntity
                                {
                                    TitleId = titleTH.TITLE_ID,
                                    TitleName = titleTH.TITLE_NAME,
                                } : null),
                                TitleEnglish = (titleEN != null ? new TitleEntity
                                {
                                    TitleId = titleEN.TITLE_ID,
                                    TitleName = titleEN.TITLE_NAME,
                                } : null),
                                PhoneList = (from p in c.TB_M_PHONE
                                             where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax
                                             orderby p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE ascending
                                             select new PhoneEntity
                                             {
                                                 CustomerId = p.CUSTOMER_ID,
                                                 PhoneTypeId = p.PHONE_TYPE_ID,
                                                 PhoneId = p.PHONE_ID,
                                                 PhoneNo = p.PHONE_NO,
                                                 PhoneTypeName = p.TB_M_PHONE_TYPE.PHONE_TYPE_NAME
                                             }).ToList(),
                                EmployeeId = c.EMPLOYEE_ID
                            }
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        public IEnumerable<ServiceRequestAccountSearchResult> SearchAccount(ContractSearchFilter searchFilter)
        {
            var query = _context.TB_M_ACCOUNT.AsQueryable();

            query = query.Where(q => q.CUSTOMER_ID == searchFilter.CustomerId);

            // Filter: AccountNo
            if (!string.IsNullOrEmpty(searchFilter.AccountNo))
                query = query.Where(q => q.ACCOUNT_NO == searchFilter.AccountNo);

            // Filter: BranchName
            if (!string.IsNullOrEmpty(searchFilter.BranchName))
                query = query.Where(q => q.BRANCH_NAME == searchFilter.BranchName);

            // Filter: CarNo
            if (!string.IsNullOrEmpty(searchFilter.CarNo))
            {
                var originalKeyword = searchFilter.CarNo.Trim();

                if (!string.IsNullOrEmpty(originalKeyword) && originalKeyword != "*" && originalKeyword != "**")
                {
                    if (originalKeyword.StartsWith("*") && originalKeyword.EndsWith("*"))
                    {
                        var keyword = originalKeyword.Substring(1, originalKeyword.Length - 2);
                        query = query.Where(q => q.CAR_NO.Contains(keyword));
                    }
                    else if (originalKeyword.StartsWith("*"))
                    {
                        var keyword = originalKeyword.Substring(1, originalKeyword.Length - 1);
                        query = query.Where(q => q.CAR_NO.EndsWith(keyword));
                    }
                    else if (originalKeyword.EndsWith("*"))
                    {
                        var keyword = originalKeyword.Substring(0, originalKeyword.Length - 1);
                        query = query.Where(q => q.CAR_NO.StartsWith(keyword));
                    }
                    else
                    {
                        query = query.Where(q => q.CAR_NO == originalKeyword);
                    }
                }
            }

            // Filter: ProductGroupName
            if (!string.IsNullOrEmpty(searchFilter.ProductGroupName))
                query = query.Where(q => q.PRODUCT_GROUP == searchFilter.ProductGroupName);

            // Filter: ProductName
            if (!string.IsNullOrEmpty(searchFilter.ProductName))
                query = query.Where(q => q.SUBSCRIPTION_DESC == searchFilter.ProductName);

            // Filter: Status
            if (!string.IsNullOrEmpty(searchFilter.Status))
            {
                if (searchFilter.Status == "1")
                    query = query.Where(q => q.STATUS.ToUpper() == "A");
                else if (searchFilter.Status == "2")
                    query = query.Where(q => q.STATUS.ToUpper() != "A");
                else
                {
                    // Nothing (searchFilter.Status == "0" is All)
                }
            }

            var resultQuery = query.Select(q => new ServiceRequestAccountSearchResult
            {
                AccountId = q.ACCOUNT_ID,
                AccountNo = q.ACCOUNT_NO,
                CarNo = q.CAR_NO,
                ProductGroupName = q.PRODUCT_GROUP,
                ProductName = q.SUBSCRIPTION_DESC,
                BranchName = q.BRANCH_NAME,
                Status = q.STATUS,
                IsDefault = q.IS_DEFAULT,
            });

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = resultQuery.Count();

            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            resultQuery = SetContractListSort(resultQuery, searchFilter);
            return resultQuery.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();
        }

        public IQueryable<ServiceRequestAccountSearchResult> SetContractListSort(IQueryable<ServiceRequestAccountSearchResult> contractList,
            ContractSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper().Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper())
                {
                    default:
                        return contractList.OrderBy(a => a.IsDefault).ThenBy(a => a.CarNo);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper())
                {
                    default:
                        return contractList.OrderByDescending(a => a.IsDefault).ThenByDescending(a => a.CarNo);
                }
            }
        }

        public IEnumerable<ServiceRequestContactSearchResult> SearchContact(CustomerContactSearchFilter searchFilter)
        {
            var query = _context.TB_M_CUSTOMER_CONTACT.AsQueryable();
            query = query.Where(q => q.CUSTOMER_ID == searchFilter.CustomerId && q.RELATIONSHIP_ID != null);

            // Filter: FirstName
            if (!string.IsNullOrEmpty(searchFilter.FirstName))
            {
                var firstName = searchFilter.FirstName.Trim();

                if (firstName.StartsWith("*") && firstName.EndsWith("*"))
                {
                    var keyword = firstName.Replace("*", "");
                    query = query.Where(q => q.TB_M_CONTACT.FIRST_NAME_EN.Contains(keyword) || q.TB_M_CONTACT.FIRST_NAME_TH.Contains(keyword));
                }
                else if (firstName.StartsWith("*"))
                {
                    var keyword = firstName.Replace("*", "");
                    query = query.Where(q => q.TB_M_CONTACT.FIRST_NAME_EN.EndsWith(keyword) || q.TB_M_CONTACT.FIRST_NAME_TH.EndsWith(keyword));
                }
                else if (firstName.EndsWith("*"))
                {
                    var keyword = firstName.Replace("*", "");
                    query = query.Where(q => q.TB_M_CONTACT.FIRST_NAME_EN.StartsWith(keyword) || q.TB_M_CONTACT.FIRST_NAME_TH.StartsWith(keyword));
                }
                else
                {
                    var keyword = firstName.Replace("*", "");
                    query = query.Where(q => q.TB_M_CONTACT.FIRST_NAME_EN == firstName || q.TB_M_CONTACT.FIRST_NAME_TH == firstName);
                }
            }

            // Filter: LastName
            if (!string.IsNullOrEmpty(searchFilter.LastName))
            {
                var lastName = searchFilter.LastName.Trim();

                if (lastName.StartsWith("*") && lastName.EndsWith("*"))
                {
                    var keyword = lastName.Replace("*", "");
                    query = query.Where(q => q.TB_M_CONTACT.LAST_NAME_EN.Contains(keyword) || q.TB_M_CONTACT.LAST_NAME_TH.Contains(keyword));
                }
                else if (lastName.StartsWith("*"))
                {
                    var keyword = lastName.Replace("*", "");
                    query = query.Where(q => q.TB_M_CONTACT.LAST_NAME_EN.EndsWith(keyword) || q.TB_M_CONTACT.LAST_NAME_TH.EndsWith(keyword));
                }
                else if (lastName.EndsWith("*"))
                {
                    var keyword = lastName.Replace("*", "");
                    query = query.Where(q => q.TB_M_CONTACT.LAST_NAME_EN.StartsWith(keyword) || q.TB_M_CONTACT.LAST_NAME_TH.StartsWith(keyword));
                }
                else
                {
                    var keyword = lastName.Replace("*", "");
                    query = query.Where(q => q.TB_M_CONTACT.LAST_NAME_EN == lastName || q.TB_M_CONTACT.LAST_NAME_TH == lastName);
                }
            }

            // Filter : CardNo
            if (!string.IsNullOrEmpty(searchFilter.CardNo))
                query = query.Where(q => q.TB_M_CONTACT.CARD_NO == searchFilter.CardNo);

            // Filter : AccountNo
            if (!string.IsNullOrEmpty(searchFilter.AccountNo))
                query = query.Where(q => q.TB_M_ACCOUNT.ACCOUNT_NO == searchFilter.AccountNo);

            // Filter: PhoneNo
            if (!string.IsNullOrEmpty(searchFilter.PhoneNo))
            {
                query = query.Where(q => q.TB_M_CONTACT.TB_M_CONTACT_PHONE.Any(p => p.PHONE_NO == searchFilter.PhoneNo));
            }

            var resultQuery = query.Select(q => new ServiceRequestContactSearchResult
            {
                ContactId = q.CONTACT_ID,
                AccountNo = q.TB_M_ACCOUNT.ACCOUNT_NO,
                Product = q.TB_M_ACCOUNT.SUBSCRIPTION_DESC,
                CardNo = q.TB_M_CONTACT.CARD_NO,
                FirstNameTh = q.TB_M_CONTACT.FIRST_NAME_TH,
                LastNameTh = q.TB_M_CONTACT.LAST_NAME_TH,
                // PhoneNo = q.TB_M_CONTACT.TB_M_CONTACT_PHONE.Any() ? q.TB_M_CONTACT.TB_M_CONTACT_PHONE.FirstOrDefault().PHONE_NO : string.Empty,
                RelationshipId = q.RELATIONSHIP_ID ?? 0,
                RelationName = q.TB_M_RELATIONSHIP != null ? q.TB_M_RELATIONSHIP.RELATIONSHIP_NAME : "",
                SubscriptionType = q.TB_M_CONTACT.TB_M_SUBSCRIPT_TYPE.SUBSCRIPT_TYPE_NAME,
                BirthDay = q.TB_M_CONTACT.BIRTH_DATE,
                TitleTh = q.TB_M_CONTACT.TB_M_TITLE1.TITLE_NAME, // TitleTh = q.TB_M_CONTACT.TB_M_TITLE.TITLE_TH,
                TitleEn = q.TB_M_CONTACT.TB_M_TITLE.TITLE_NAME,  // TitleEn = q.TB_M_CONTACT.TB_M_TITLE.TITLE_EN,
                FirstNameEn = q.TB_M_CONTACT.FIRST_NAME_EN,
                LastNameEn = q.TB_M_CONTACT.LAST_NAME_EN,
                Email = q.TB_M_CONTACT.EMAIL,
                IsDefault = q.TB_M_CONTACT.IS_DEFAULT,
                ContactFromSystem = q.SYSTEM,
                UpdateUser = q.UPDATE_USER != null ? (from ur in _context.TB_R_USER where ur.USER_ID == q.UPDATE_USER select new UserEntity { Username = ur.USERNAME }).FirstOrDefault() : null,
                CustomerPhones = q.TB_M_CONTACT.TB_M_CONTACT_PHONE.Select(item => new ServiceRequestCustomerAccountPhone
                {
                    PhoneNo = item.PHONE_NO,
                    PhoneTypeCode = item.TB_M_PHONE_TYPE.PHONE_TYPE_CODE,
                    PhoneTypeName = item.TB_M_PHONE_TYPE.PHONE_TYPE_NAME
                }).OrderBy(o => o.PhoneTypeCode).ToList()
            });

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = resultQuery.Count();

            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            resultQuery = SetCustomerContractListSort(resultQuery, searchFilter);
            var tmpList = resultQuery.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();

            foreach (var item in tmpList)
            {
                item.PhoneNo = string.Join(",", item.CustomerPhones.Where(x => x.PhoneTypeCode != "05").Select(x => x.PhoneNo).ToList());
            }

            return tmpList;
        }

        public IQueryable<ServiceRequestContactSearchResult> SetCustomerContractListSort(IQueryable<ServiceRequestContactSearchResult> customerContractList,
            CustomerContactSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper().Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper())
                {
                    default:
                        return customerContractList.OrderBy(a => a.IsDefault).ThenBy(a => a.CardNo);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper())
                {
                    default:
                        return customerContractList.OrderByDescending(a => a.IsDefault).ThenByDescending(a => a.CardNo);
                }
            }
        }

        public IEnumerable<ServiceRequestEntity> GetServiceRequestById(ServiceRequestSearchFilter searchFilter)
        {
            var query = _context.TB_T_SR.AsQueryable();
            query = query.Where(p => p.OWNER_USER_ID == searchFilter.OwnerUserId || p.DELEGATE_USER_ID == searchFilter.DelegateUserId);

            if (!String.IsNullOrEmpty(searchFilter.StatusCode))
            {
                var statusObj = _context.TB_C_SR_STATUS.FirstOrDefault(p => p.SR_STATUS_CODE == searchFilter.StatusCode);
                query = query.Where(p => p.SR_STATUS_ID.HasValue && p.SR_STATUS_ID.Value == statusObj.SR_STATUS_ID);
            }
            else
            {
                query = query.Where(p => p.SR_STATUS_ID.HasValue
                    && p.TB_C_SR_STATUS.SR_STATUS_CODE != Constants.SRStatusCode.Closed
                    && p.TB_C_SR_STATUS.SR_STATUS_CODE != Constants.SRStatusCode.Cancelled);
            }

            // After Insert all filters >> Count It
            searchFilter.TotalRecords = query.Count();

            if (searchFilter.SortOrder == "ProductName")
            {
                query = query.OrderByDescending(q => q.SR_NO);
            }

            query = query.OrderByDescending(q => q.SR_NO);

            var results = query.Select(q => new ServiceRequestEntity
            {
                SrId = q.SR_ID,
                SrNo = q.SR_NO,
                ThisAlert = q.RULE_THIS_ALERT,
                NextSLA = q.RULE_NEXT_SLA,
                TotalWorkingHours = q.RULE_TOTAL_WORK,
                CustomerCardNo = q.TB_M_CUSTOMER.CARD_NO,
                ChannelId = q.CHANNEL_ID,
                ChannelName = q.TB_R_CHANNEL.CHANNEL_NAME,
                ProductId = q.PRODUCT_ID,
                ProductName = q.TB_R_PRODUCT.PRODUCT_NAME,
                CampaignServiceId = q.CAMPAIGNSERVICE_ID,
                CampaignServiceName = q.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_NAME,
                AreaId = q.AREA_ID,
                AreaName = q.TB_M_AREA.AREA_NAME,
                SubAreaId = q.SUBAREA_ID,
                SubAreaName = q.TB_M_SUBAREA.SUBAREA_NAME,
                Subject = q.SR_SUBJECT,
                SRStateName = _context.TB_C_SR_STATE.Where(x => x.SR_STATE_ID == q.TB_C_SR_STATUS.SR_STATE_ID).FirstOrDefault().SR_STATE_NAME,
                SrStatusName = q.TB_C_SR_STATUS.SR_STATUS_NAME,
                SrStatusCode = q.TB_C_SR_STATUS.SR_STATUS_CODE,
                CreateDate = q.CREATE_DATE,
                ClosedDate = q.CREATE_DATE,
                OwnerUserId = q.TB_R_USER3.USER_ID,
                OwnerUserPosition = q.TB_R_USER3 != null ? q.TB_R_USER3.POSITION_CODE : null,
                OwnerUserFirstName = q.TB_R_USER3 != null ? q.TB_R_USER3.FIRST_NAME : null,
                OwnerUserLastName = q.TB_R_USER3 != null ? q.TB_R_USER3.LAST_NAME : null,
                DelegateUserId = q.TB_R_USER6.USER_ID,
                DelegateUserPosition = q.TB_R_USER6 != null ? q.TB_R_USER6.POSITION_CODE : null,
                DelegateUserFirstName = q.TB_R_USER6 != null ? q.TB_R_USER6.FIRST_NAME : null,
                DelegateUserLastName = q.TB_R_USER6 != null ? q.TB_R_USER6.LAST_NAME : null,
                ANo = q.SR_ANO
            });

            if (searchFilter.PageSize != 0)
            {
                int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
                if (startPageIndex >= searchFilter.TotalRecords)
                {
                    startPageIndex = 0;
                    searchFilter.PageNo = 1;
                }

                return results.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<ServiceRequestEntity>();
            }

            return results.ToList<ServiceRequestEntity>();
        }

        public IEnumerable<ServiceRequestEntity> GetServiceRequestById(List<int> userIds, string statusCode)
        {
            var query = _context.TB_T_SR.AsQueryable();

            query = query.Where(p => (p.OWNER_USER_ID.HasValue && userIds.Contains(p.OWNER_USER_ID.Value)) || (p.DELEGATE_USER_ID.HasValue && userIds.Contains(p.DELEGATE_USER_ID.Value)));

            if (!String.IsNullOrEmpty(statusCode))
            {
                var statusObj = _context.TB_C_SR_STATUS.FirstOrDefault(p => p.SR_STATUS_CODE == statusCode);
                query = query.Where(p => p.SR_STATUS_ID.HasValue && p.SR_STATUS_ID.Value == statusObj.SR_STATUS_ID);
            }

            query = query.OrderByDescending(q => q.SR_NO);

            var results = query.Select(q => new ServiceRequestEntity
            {
                SrId = q.SR_ID,
                SrNo = q.SR_NO,
                ThisAlert = q.RULE_THIS_ALERT,
                NextSLA = q.RULE_NEXT_SLA,
                TotalWorkingHours = q.RULE_TOTAL_WORK,
                CustomerCardNo = q.TB_M_CUSTOMER.CARD_NO,
                ChannelId = q.CHANNEL_ID,
                ChannelName = q.TB_R_CHANNEL.CHANNEL_NAME,
                ProductId = q.PRODUCT_ID,
                ProductName = q.TB_R_PRODUCT.PRODUCT_NAME,
                CampaignServiceId = q.CAMPAIGNSERVICE_ID,
                CampaignServiceName = q.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_NAME,
                AreaId = q.AREA_ID,
                AreaName = q.TB_M_AREA.AREA_NAME,
                SubAreaId = q.SUBAREA_ID,
                SubAreaName = q.TB_M_SUBAREA.SUBAREA_NAME,
                Subject = q.SR_SUBJECT,
                SrStatusName = q.TB_C_SR_STATUS.SR_STATUS_NAME,
                SrStatusCode = q.TB_C_SR_STATUS.SR_STATUS_CODE,
                SRStateName = _context.TB_C_SR_STATE.Where(x => x.SR_STATE_ID == q.TB_C_SR_STATUS.SR_STATE_ID).FirstOrDefault().SR_STATE_NAME,
                CreateDate = q.CREATE_DATE,
                ClosedDate = q.CREATE_DATE,
                OwnerUserId = q.TB_R_USER3.USER_ID,
                OwnerUserPosition = q.TB_R_USER3 != null ? q.TB_R_USER3.POSITION_CODE : null,
                OwnerUserFirstName = q.TB_R_USER3 != null ? q.TB_R_USER3.FIRST_NAME : null,
                OwnerUserLastName = q.TB_R_USER3 != null ? q.TB_R_USER3.LAST_NAME : null,
                DelegateUserId = q.TB_R_USER6.USER_ID,
                DelegateUserPosition = q.TB_R_USER6 != null ? q.TB_R_USER6.POSITION_CODE : null,
                DelegateUserFirstName = q.TB_R_USER6 != null ? q.TB_R_USER6.FIRST_NAME : null,
                DelegateUserLastName = q.TB_R_USER6 != null ? q.TB_R_USER6.LAST_NAME : null,
                ANo = q.SR_ANO
            });

            return results.ToList<ServiceRequestEntity>();
        }

        public List<MediaSourceEntity> GetActiveMediaSources()
        {
            return (from ch in _context.TB_M_MEDIA_SOURCE
                    orderby ch.MEDIA_SOURCE_NAME
                    select new MediaSourceEntity
                    {
                        MediaSourceId = ch.MEDIA_SOURCE_ID,
                        Name = ch.MEDIA_SOURCE_NAME
                    }).ToList();
        }

        public List<ActivityTypeEntity> GetActivityTypes()
        {
            return (from ch in _context.TB_C_SR_ACTIVITY_TYPE
                    orderby ch.SR_ACTIVITY_TYPE_NAME
                    select new ActivityTypeEntity
                    {
                        ActivityTypeId = ch.SR_ACTIVITY_TYPE_ID,
                        Name = ch.SR_ACTIVITY_TYPE_NAME
                    }).ToList();
        }

        public List<SrEmailTemplateEntity> GetSrEmailTemplates()
        {
            return (from ch in _context.TB_C_SR_EMAIL_TEMPLATE
                    orderby ch.SR_EMAIL_TEMPLATE_NAME
                    select new SrEmailTemplateEntity
                    {
                        SrEmailTemplateId = ch.SR_EMAIL_TEMPLATE_ID,
                        Name = ch.SR_EMAIL_TEMPLATE_NAME
                    }).ToList();
        }

        public List<SRStatusEntity> GetAvailableNextSrStatuses(int srId)
        {
            var sr = _context.TB_T_SR.SingleOrDefault(s => s.SR_ID == srId);

            if (sr == null || !sr.SR_STATUS_ID.HasValue
                || !sr.PRODUCTGROUP_ID.HasValue || !sr.PRODUCT_ID.HasValue || !sr.CAMPAIGNSERVICE_ID.HasValue
                || !sr.AREA_ID.HasValue || !sr.SUBAREA_ID.HasValue || !sr.TYPE_ID.HasValue)
            {
                // Data Not Available
                return new List<SRStatusEntity>();
            }

            return (from c in _context.TB_C_SR_STATUS_CHANGE
                    from s in _context.TB_C_SR_STATUS.Where(x => x.SR_STATUS_ID == c.TO_SR_STATUS_ID).DefaultIfEmpty()
                    where c.FROM_SR_STATUS_ID == sr.SR_STATUS_ID.Value
                        && c.PRODUCTGROUP_ID == sr.PRODUCTGROUP_ID.Value
                        && c.PRODUCT_ID == sr.PRODUCT_ID.Value
                        && c.CAMPAIGNSERVICE_ID == sr.CAMPAIGNSERVICE_ID.Value
                        && c.AREA_ID == sr.AREA_ID.Value
                        && c.SUBAREA_ID == sr.SUBAREA_ID.Value
                        && c.TYPE_ID == sr.TYPE_ID.Value
                    orderby s.SR_STATUS_ID
                    select new SRStatusEntity
                    {
                        SRStatusId = c.TO_SR_STATUS_ID,
                        SRStatusCode = s.SR_STATUS_CODE,
                        SRStatusName = s.SR_STATUS_NAME
                    }).Distinct().ToList();
        }

        public CampaignServiceEntity GetCampaignService(int campaignserviceId)
        {
            return (from c in _context.TB_R_CAMPAIGNSERVICE
                    where c.CAMPAIGNSERVICE_ID == campaignserviceId
                    select new CampaignServiceEntity
                    {
                        CampaignServiceId = c.CAMPAIGNSERVICE_ID,
                        CampaignServiceName = c.CAMPAIGNSERVICE_NAME,
                        IsActive = c.CAMPAIGNSERVICE_IS_ACTIVE,
                        ProductGroupId = c.TB_R_PRODUCT.PRODUCTGROUP_ID,
                        ProductGroupName = c.TB_R_PRODUCT.TB_R_PRODUCTGROUP.PRODUCTGROUP_NAME,
                        ProductId = c.PRODUCT_ID,
                        ProductName = c.TB_R_PRODUCT.PRODUCT_NAME
                    }).SingleOrDefault();
        }

        public ProductGroupEntity GetProductGroup(int productGroupId)
        {
            return (from p in _context.TB_R_PRODUCTGROUP
                    where p.PRODUCTGROUP_ID == productGroupId
                    select new ProductGroupEntity
                    {
                        ProductGroupId = p.PRODUCTGROUP_ID,
                        ProductGroupCode = p.PRODUCTGROUP_CODE,
                        ProductGroupName = p.PRODUCTGROUP_NAME,
                        IsActive = p.PRODUCTGROUP_IS_ACTIVE
                    }).SingleOrDefault();
        }

        public ProductEntity GetProduct(int productId)
        {
            return (from p in _context.TB_R_PRODUCT
                    where p.PRODUCT_ID == productId
                    select new ProductEntity
                    {
                        ProductId = p.PRODUCT_ID,
                        ProductCode = p.PRODUCT_CODE,
                        ProductName = p.PRODUCT_NAME,
                        IsActive = p.PRODUCT_IS_ACTIVE,
                        ProductGroupId = p.TB_R_PRODUCTGROUP.PRODUCTGROUP_ID,
                        ProductGroupName = p.TB_R_PRODUCTGROUP.PRODUCTGROUP_NAME
                    }).SingleOrDefault();
        }

        public MappingProductEntity GetMappingByCampaign(int campaignserviceId, int areaId, int subareaId, int typeId)
        {
            return (from mp in _context.TB_M_MAP_PRODUCT
                    from pg in _context.TB_C_SR_PAGE.Where(x => x.SR_PAGE_ID == mp.SR_PAGE_ID).DefaultIfEmpty()
                    from ownerUser in _context.TB_R_USER.Where(x => mp.DEFAULT_OWNER_USER_ID.HasValue && x.USER_ID == mp.DEFAULT_OWNER_USER_ID).DefaultIfEmpty()
                    from ownerBranch in _context.TB_R_BRANCH.Where(x => ownerUser.BRANCH_ID.HasValue && x.BRANCH_ID == ownerUser.BRANCH_ID).DefaultIfEmpty()
                    from otp in _context.TB_M_OTP_TEMPLATE.Where(o => o.OTP_TEMP_ID == mp.OTP_TEMPLATE_ID).DefaultIfEmpty()
                    from hp in _context.TB_M_HP_STATUS.Where(h => h.HP_LANGUAGE_INDEPENDENT_CODE == mp.HP_LANGUAGE_INDEPENDENT_CODE).DefaultIfEmpty()
                    where mp.CAMPAIGNSERVICE_ID == campaignserviceId
                          && mp.AREA_ID == areaId
                          && mp.SUBAREA_ID == subareaId
                          && mp.TYPE_ID == typeId
                          && mp.MAP_PRODUCT_IS_ACTIVE
                    orderby mp.UPDATE_DATE descending
                    select new MappingProductEntity
                    {
                        MappingProductId = mp.MAP_PRODUCT_ID,
                        ProductGroupId = mp.TB_R_PRODUCT.PRODUCTGROUP_ID,
                        ProductId = mp.PRODUCT_ID,
                        CampaignServiceId = mp.CAMPAIGNSERVICE_ID,
                        AreaId = mp.AREA_ID,
                        SubAreaId = mp.SUBAREA_ID,
                        TypeId = mp.TYPE_ID,
                        IsVerify = mp.MAP_PRODUCT_IS_VERIFY,
                        IsActive = mp.MAP_PRODUCT_IS_ACTIVE,
                        SrPageId = mp.SR_PAGE_ID,
                        SrPageCode = pg.SR_PAGE_CODE,
                        DefaultOwnerUser = ownerUser != null ? new UserEntity
                        {
                            UserId = ownerUser.USER_ID,
                            Firstname = ownerUser.FIRST_NAME,
                            Lastname = ownerUser.LAST_NAME,
                            PositionCode = ownerUser.POSITION_CODE
                        } : null,
                        DefaultOwnerBranch = ownerBranch != null ? new BranchEntity
                        {
                            BranchId = ownerBranch.BRANCH_ID,
                            BranchCode = ownerBranch.BRANCH_CODE,
                            BranchName = ownerBranch.BRANCH_NAME
                        } : null,
                        IsSRSecret = mp.IS_SR_SECRET,
                        IsVerifyOTP = mp.IS_VERIFY_OTP,
                        OTPTemplate = (otp != null ? new OTPTemplateEntity()
                        {
                            OTPTemplateId = otp.OTP_TEMP_ID,
                            OTPTemplateCode = otp.OTP_TEMP_CODE,
                            OTPTemplateName = otp.OTP_TEMP_NAME
                        } : null),
                        HpStatus = (hp != null ? new HpStatusEntity()
                        {
                            HpStatusId = hp.HP_ID,
                            HpLangIndeCode = hp.HP_LANGUAGE_INDEPENDENT_CODE,
                            HpSubject = hp.HP_SUBJECT
                        } : null)
                    }).FirstOrDefault();
        }

        public MappingProductEntity GetMappingByProduct(int productId, int areaId, int subareaId, int typeId)
        {
            return (from mp in _context.TB_M_MAP_PRODUCT
                    from pg in _context.TB_C_SR_PAGE.Where(x => x.SR_PAGE_ID == mp.SR_PAGE_ID).DefaultIfEmpty()
                    from otp in _context.TB_M_OTP_TEMPLATE.Where(o => o.OTP_TEMP_ID == mp.OTP_TEMPLATE_ID).DefaultIfEmpty()
                    from hp in _context.TB_M_HP_STATUS.Where(h => h.HP_LANGUAGE_INDEPENDENT_CODE == mp.HP_LANGUAGE_INDEPENDENT_CODE).DefaultIfEmpty()
                    where mp.PRODUCT_ID == productId
                          && mp.CAMPAIGNSERVICE_ID == null
                          && mp.AREA_ID == areaId
                          && mp.SUBAREA_ID == subareaId
                          && mp.TYPE_ID == typeId
                          && mp.MAP_PRODUCT_IS_ACTIVE
                    orderby mp.UPDATE_DATE descending
                    select new MappingProductEntity
                    {
                        MappingProductId = mp.MAP_PRODUCT_ID,
                        ProductGroupId = mp.TB_R_PRODUCT.PRODUCTGROUP_ID,
                        ProductId = mp.PRODUCT_ID,
                        CampaignServiceId = mp.CAMPAIGNSERVICE_ID,
                        AreaId = mp.AREA_ID,
                        SubAreaId = mp.SUBAREA_ID,
                        TypeId = mp.TYPE_ID,
                        IsVerify = mp.MAP_PRODUCT_IS_VERIFY,
                        IsActive = mp.MAP_PRODUCT_IS_ACTIVE,
                        SrPageId = mp.SR_PAGE_ID,
                        SrPageCode = pg.SR_PAGE_CODE,
                        IsSRSecret = mp.IS_SR_SECRET,
                        IsVerifyOTP = mp.IS_VERIFY_OTP,
                        OTPTemplate = (otp != null ? new OTPTemplateEntity()
                        {
                            OTPTemplateId = otp.OTP_TEMP_ID,
                            OTPTemplateCode = otp.OTP_TEMP_CODE,
                            OTPTemplateName = otp.OTP_TEMP_NAME
                        } : null),
                        HpStatus = (hp != null ? new HpStatusEntity()
                        {
                            HpStatusId = hp.HP_ID,
                            HpLangIndeCode = hp.HP_LANGUAGE_INDEPENDENT_CODE,
                            HpSubject = hp.HP_SUBJECT
                        } : null)
                    }).FirstOrDefault();
        }

        public List<MappingProductQuestionGroupEntity> GetMappingProductQuestionGroups(int mappingProductId)
        {
            return _context.TB_M_MAP_PRODUCT_QUESTIONGROUP
                    .Where(g => g.MAP_PRODUCT_ID == mappingProductId && (g.TB_M_QUESTIONGROUP.QUESTIONGROUP_IS_ACTIVE ?? false))
                    .OrderBy(g => g.SEQ_NO)
                    .Select(g => new MappingProductQuestionGroupEntity
                    {
                        QuestionGroupId = g.TB_M_QUESTIONGROUP.QUESTIONGROUP_ID,
                        QuestionGroupName = g.TB_M_QUESTIONGROUP.QUESTIONGROUP_NAME,
                        RequireAmountPass = g.REQUIRE_AMOUNT_PASS,
                        SeqNo = g.SEQ_NO,
                        Questions = g.TB_M_QUESTIONGROUP.TB_M_QUESTIONGROUP_QUESTION
                                     .Where(q => q.TB_M_QUESTION.IS_ACTIVE ?? false)
                                     .OrderBy(q => q.SEQ_NO)
                                     .Select(q => new MappingProductQuestionEntity
                                     {
                                         QuestionId = q.QUESTION_ID,
                                         QuestionName = q.TB_M_QUESTION.QUESTION_NAME
                                     })
                                     .ToList()
                    }).ToList();
        }

        public ServiceRequestNoDetailEntity GetServiceRequestNoDetail(string srNo)
        {
            if (string.IsNullOrWhiteSpace(srNo) == false)
            {
                var srId = _context.TB_T_SR.Where(x => x.SR_NO == srNo).SingleOrDefault()?.SR_ID;
                if (srId.HasValue)
                    return GetServiceRequestNoDetail(srId.Value);
            }
            return null;
        }

        public ServiceRequestNoDetailEntity GetServiceRequestNoDetail(int srId)
        {
            return (from sr in _context.TB_T_SR.AsNoTracking()
                    from st in _context.TB_C_SR_STATUS.Where(x => x.SR_STATUS_ID == sr.SR_STATUS_ID)
                    where sr.SR_ID == srId && sr.SR_STATUS_ID.HasValue
                    select new ServiceRequestNoDetailEntity
                    {
                        SrId = sr.SR_ID,
                        SrNo = sr.SR_NO,
                        CustomerId = sr.CUSTOMER_ID,
                        AccountId = sr.ACCOUNT_ID,
                        ContactId = sr.CONTACT_ID,
                        ContactAccountNo = sr.CONTACT_ACCOUNT_NO,
                        ContactRelationshipId = sr.CONTACT_RELATIONSHIP_ID,
                        ProductGroupId = sr.PRODUCTGROUP_ID,
                        ProductId = sr.PRODUCT_ID,
                        CampaignServiceId = sr.CAMPAIGNSERVICE_ID,
                        AreaId = sr.AREA_ID,
                        SubAreaId = sr.SUBAREA_ID,
                        TypeId = sr.TYPE_ID,
                        MapProductId = sr.MAP_PRODUCT_ID,
                        ChannelId = sr.CHANNEL_ID,
                        MediaSourceId = sr.MEDIA_SOURCE_ID,
                        Subject = sr.SR_SUBJECT,
                        Remark = sr.SR_REMARK,
                        CreatorBranchId = sr.CREATE_BRANCH_ID,
                        CreatorUserId = sr.CREATE_USER,
                        OwnerBranchId = sr.OWNER_BRANCH_ID,
                        OwnerUserId = sr.OWNER_USER_ID,
                        DelegateBranchId = sr.DELEGATE_BRANCH_ID,
                        DelegateUserId = sr.DELEGATE_USER_ID,
                        SrPageId = sr.SR_PAGE_ID,
                        SrStatusId = sr.SR_STATUS_ID,
                        CPN_IsSecret = sr.CPN_SECRET,
                        IsNotSendCar = sr.CPN_CAR,
                        SRStatusName = st.SR_STATUS_NAME,
                        CreateDate = sr.CREATE_DATE
                    }).SingleOrDefault();
        }

        public ServiceRequestForDisplayEntity GetServiceRequest(string srNo)
        {
            srNo = srNo.Trim();
            var sr = _context.TB_T_SR.Where(x => x.SR_NO.ToUpper() == srNo.ToUpper()).Select(x => new { x.SR_ID }).FirstOrDefault();
            if (sr == null)
                return null;

            return GetServiceRequest(sr.SR_ID);
        }

        public ServiceRequestForDisplayEntity GetServiceRequest(int srId)
        {
            var sr = _context.SP_GET_SR(srId).FirstOrDefault();

            if (sr == null)
                return null;

            var result = new ServiceRequestForDisplayEntity
            {
                SrId = sr.SR_ID,
                SrNo = sr.SR_NO,
                CallId = sr.SR_CALL_ID,
                PhoneNo = sr.SR_ANO,
                RuleAssignFlag = sr.RULE_ASSIGN_FLAG,
                Customer = sr.CUSTOMER_ID.HasValue ? new CustomerEntity
                {
                    CustomerId = sr.CUSTOMER_ID,
                    CardNo = sr.CUSTOMER_CARD_NO,
                    BirthDate = sr.CUSTOMER_BIRTH_DATE,
                    TitleThai = sr.CUSTOMER_TITLE_ID_TH.HasValue ? new TitleEntity
                    {
                        TitleId = sr.CUSTOMER_TITLE_ID_TH,
                        TitleName = sr.CUSTOMER_TITLE_NAME_TH,
                    } : null,
                    TitleEnglish = sr.CUSTOMER_TITLE_ID_EN.HasValue ? new TitleEntity
                    {
                        TitleId = sr.CUSTOMER_TITLE_ID_EN,
                        TitleName = sr.CUSTOMER_TITLE_NAME_EN,
                    } : null,
                    FirstNameThai = sr.CUSTOMER_FIRST_NAME_TH,
                    FirstNameEnglish = sr.CUSTOMER_FIRST_NAME_EN,
                    LastNameThai = sr.CUSTOMER_LAST_NAME_TH,
                    LastNameEnglish = sr.CUSTOMER_LAST_NAME_EN,
                    //PhoneList = customer.TB_M_PHONE.Select(cp => new PhoneEntity
                    //{
                    //    PhoneId = cp.PHONE_ID,
                    //    PhoneNo = cp.PHONE_NO,
                    //    PhoneTypeId = cp.PHONE_TYPE_ID,
                    //    PhoneTypeName = cp.TB_M_PHONE_TYPE.PHONE_TYPE_NAME,
                    //    PhoneTypeCode = cp.TB_M_PHONE_TYPE.PHONE_TYPE_CODE,
                    //}).OrderBy(p => p.PhoneId).ToList(),
                    //Fax = customer.FAX,
                    Email = sr.CUSTOMER_EMAIL,
                    CustomerType = sr.CUSTOMER_TYPE
                } : null,
                CustomerSubscriptType = sr.CUSTOMER_SUBSCRIPT_TYPE_ID.HasValue ? new SubscriptTypeEntity
                {
                    SubscriptTypeId = sr.CUSTOMER_SUBSCRIPT_TYPE_ID,
                    SubscriptTypeCode = sr.CUSTOMER_SUBSCRIPT_TYPE_CODE,
                    SubscriptTypeName = sr.CUSTOMER_SUBSCRIPT_TYPE_NAME
                } : null,
                CustomerEmployeeCode = sr.CUSTOMER_EMPLOYEE_CODE,
                Account = sr.ACCOUNT_ID.HasValue ? new AccountEntity
                {
                    AccountId = sr.ACCOUNT_ID,
                    AccountNo = sr.ACCOUNT_NO,
                    Status = sr.ACCOUNT_STATUS,
                    CarNo = sr.ACCOUNT_CAR_NO,
                    ProductGroup = sr.ACCOUNT_PRODUCT_GROUP,
                    Product = sr.ACCOUNT_PRODUCT,
                    BranchCode = sr.ACCOUNT_BRANCH_CODE,
                    BranchName = sr.ACCOUNT_BRANCH_NAME
                } : null,
                Contact = sr.CONTACT_ID.HasValue ? new ContactEntity
                {
                    ContactId = sr.CONTACT_ID,
                    CardNo = sr.CONTACT_CARD_NO,
                    BirthDate = sr.CONTACT_BIRTH_DATE,
                    TitleThai = sr.CONTACT_TITLE_ID_TH.HasValue ? new TitleEntity
                    {
                        TitleId = sr.CONTACT_TITLE_ID_TH,
                        TitleName = sr.CONTACT_TITLE_NAME_TH
                    } : null,
                    TitleEnglish = sr.CONTACT_TITLE_ID_EN.HasValue ? new TitleEntity
                    {
                        TitleId = sr.CONTACT_TITLE_ID_EN,
                        TitleName = sr.CONTACT_TITLE_NAME_EN
                    } : null,
                    FirstNameThai = sr.CONTACT_FIRST_NAME_TH,
                    FirstNameEnglish = sr.CONTACT_FIRST_NAME_EN,
                    LastNameThai = sr.CONTACT_LAST_NAME_TH,
                    LastNameEnglish = sr.CONTACT_LAST_NAME_EN,
                    //PhoneList = contact.TB_M_CONTACT_PHONE.Select(cp => new PhoneEntity
                    //{
                    //    PhoneId = cp.CONTACT_PHONE_ID,
                    //    PhoneNo = cp.PHONE_NO,
                    //    PhoneTypeId = cp.PHONE_TYPE_ID,
                    //    PhoneTypeName = cp.TB_M_PHONE_TYPE.PHONE_TYPE_NAME,
                    //    PhoneTypeCode = cp.TB_M_PHONE_TYPE.PHONE_TYPE_CODE,
                    //}).OrderBy(p => p.PhoneId).ToList(),
                    Email = sr.CONTACT_EMAIL
                } : null,
                ContactSubscriptType = sr.CONTACT_SUBSCRIPT_TYPE_ID.HasValue ? new SubscriptTypeEntity
                {
                    SubscriptTypeId = sr.CONTACT_SUBSCRIPT_TYPE_ID,
                    SubscriptTypeCode = sr.CONTACT_SUBSCRIPT_TYPE_CODE,
                    SubscriptTypeName = sr.CONTACT_SUBSCRIPT_TYPE_NAME
                } : null,

                Relationship = sr.CONTACT_RELATIONSHIP_ID.HasValue ? new RelationshipEntity
                {
                    RelationshipId = sr.CONTACT_RELATIONSHIP_ID ?? 0,
                    RelationshipName = sr.CONTACT_RELATIONSHIP_NAME,
                    RelationshipDesc = sr.CONTACT_RELATIONSHIP_DESC
                } : null,
                ContactAccountNo = sr.CONTACT_ACCOUNT_NO ?? string.Empty,

                MappingProductId = sr.MAP_PRODUCT_ID,
                ProductGroup = sr.PRODUCTGROUP_ID.HasValue ? new ProductGroupEntity
                {
                    ProductGroupId = sr.PRODUCTGROUP_ID,
                    ProductGroupName = sr.PRODUCTGROUP_NAME
                } : null,
                Product = sr.PRODUCT_ID.HasValue ? new ProductEntity
                {
                    ProductId = sr.PRODUCT_ID,
                    ProductName = sr.PRODUCT_NAME,
                } : null,
                CampaignService = sr.CAMPAIGNSERVICE_ID.HasValue ? new CampaignServiceEntity
                {
                    CampaignServiceId = sr.CAMPAIGNSERVICE_ID,
                    CampaignServiceName = sr.CAMPAIGNSERVICE_NAME
                } : null,
                Area = sr.AREA_ID.HasValue ? new AreaEntity
                {
                    AreaId = sr.AREA_ID,
                    AreaName = sr.AREA_NAME
                } : null,
                SubArea = sr.SUBAREA_ID.HasValue ? new SubAreaEntity
                {
                    SubareaId = sr.SUBAREA_ID,
                    SubareaName = sr.SUBAREA_NAME
                } : null,
                Type = sr.TYPE_ID.HasValue ? new TypeEntity
                {
                    TypeId = sr.TYPE_ID,
                    TypeName = sr.TYPE_NAME
                } : null,
                MediaSource = sr.MEDIA_SOURCE_ID.HasValue ? new MediaSourceEntity
                {
                    MediaSourceId = sr.MEDIA_SOURCE_ID ?? 0,
                    Name = sr.MEDIA_SOURCE_NAME
                } : null,
                Channel = sr.CHANNEL_ID.HasValue ? new ChannelEntity
                {
                    ChannelId = sr.CHANNEL_ID ?? 0,
                    Code = sr.CHANNEL_CODE,
                    Name = sr.CHANNEL_NAME
                } : null,
                Subject = sr.SR_SUBJECT,
                Remark = sr.SR_REMARK,
                SrPageId = sr.SR_PAGE_ID,
                SrPageCode = sr.SR_PAGE_CODE,
                OwnerUser = sr.OWNER_USER_ID.HasValue ? new UserEntity
                {
                    UserId = sr.OWNER_USER_ID ?? 0,
                    Firstname = sr.OWNER_USER_FIRST_NAME,
                    Lastname = sr.OWNER_USER_LAST_NAME,
                    PositionCode = sr.OWNER_USER_POSITION_CODE
                } : null,
                OwnerUserBranch = sr.OWNER_BRANCH_ID.HasValue ? new BranchEntity
                {
                    BranchId = sr.OWNER_BRANCH_ID,
                    BranchCode = sr.OWNER_BRANCH_CODE,
                    BranchName = sr.OWNER_BRANCH_NAME,
                } : null,
                DelegateUser = sr.DELEGATE_USER_ID.HasValue ? new UserEntity
                {
                    UserId = sr.DELEGATE_USER_ID ?? 0,
                    Firstname = sr.DELEGATE_USER_FIRST_NAME,
                    Lastname = sr.DELEGATE_USER_LAST_NAME,
                    PositionCode = sr.DELEGATE_USER_POSITION_CODE
                } : null,
                DelegateUserBranch = sr.DELEGATE_BRANCH_ID.HasValue ? new BranchEntity
                {
                    BranchId = sr.DELEGATE_BRANCH_ID,
                    BranchCode = sr.DELEGATE_BRANCH_CODE,
                    BranchName = sr.DELEGATE_BRANCH_NAME
                } : null,
                CreateUser = sr.CREATE_USER_ID.HasValue ? new UserEntity
                {
                    UserId = sr.CREATE_USER_ID ?? 0,
                    Firstname = sr.CREATE_USER_FIRST_NAME,
                    Lastname = sr.CREATE_USER_LAST_NAME,
                    PositionCode = sr.CREATE_USER_POSITION_CODE
                } : null,
                CreateBranch = sr.CREATE_BRANCH_ID.HasValue ? new BranchEntity
                {
                    BranchId = sr.CREATE_BRANCH_ID,
                    BranchCode = sr.CREATE_BRANCH_CODE,
                    BranchName = sr.CREATE_BRANCH_NAME
                } : null,
                UpdateUser = sr.UPDATE_USER_ID.HasValue ? new UserEntity
                {
                    UserId = sr.UPDATE_USER_ID ?? 0,
                    Firstname = sr.UPDATE_USER_FIRST_NAME,
                    Lastname = sr.UPDATE_USER_LAST_NAME,
                    PositionCode = sr.UPDATE_USER_POSITION_CODE
                } : null,
                SRState = sr.SR_STATE_ID.HasValue ? new SRStateEntity()
                {
                    SRStateId = sr.SR_STATE_ID,
                    SRStateName = sr.SR_STATE_NAME
                } : null,
                SRStatus = sr.SR_STATUS_ID.HasValue ? new SRStatusEntity
                {
                    SRStatusId = sr.SR_STATUS_ID ?? 0,
                    SRStatusCode = sr.SR_STATUS_CODE,
                    SRStatusName = sr.SR_STATUS_NAME
                } : null,
                IsVerify = sr.SR_IS_VERIFY,
                IsVerifyPass = sr.SR_IS_VERIFY_PASS,

                DraftSrEmailTemplateId = (sr.SR_STATUS_ID.HasValue && sr.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_SR_EMAIL_TEMPLATE_ID : null,
                DraftActivityDescription = (sr.SR_STATUS_ID.HasValue && sr.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_ACTIVITY_DESC : string.Empty,
                DraftActivityTypeId = (sr.SR_STATUS_ID.HasValue && sr.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_ACTIVITY_TYPE_ID : null,
                DraftSendMailSender = (sr.SR_STATUS_ID.HasValue && sr.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_MAIL_SENDER : string.Empty,
                DraftSendMailTo = (sr.SR_STATUS_ID.HasValue && sr.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_MAIL_TO : string.Empty,
                DraftSendMailCc = (sr.SR_STATUS_ID.HasValue && sr.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_MAIL_CC : string.Empty,
                DraftSendMailSubject = (sr.SR_STATUS_ID.HasValue && sr.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_MAIL_SUBJECT : string.Empty,
                DraftSendMailBody = (sr.SR_STATUS_ID.HasValue && sr.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_MAIL_BODY : string.Empty,
                DraftIsEmailDelegate = (sr.SR_STATUS_ID.HasValue && sr.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? (sr.DRAFT_IS_SEND_EMAIL_FOR_DELEGATE ?? false) : false,
                DraftIsClose = (sr.SR_STATUS_ID.HasValue && sr.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? (sr.DRAFT_IS_CLOSE ?? false) : false,
                DraftVerifyAnswerJson = (sr.SR_STATUS_ID.HasValue && sr.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_VERIFY_ANSWER_JSON : string.Empty,
                DraftAttachmentJson = (sr.SR_STATUS_ID.HasValue && sr.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_ATTACHMENT_JSON : string.Empty,
                DraftAccountAddressId = (sr.SR_STATUS_ID.HasValue && sr.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.SR_DEF_ACCOUNT_ADDRESS_ID : (int?)null,
                DraftAccountAddressText = (sr.SR_STATUS_ID.HasValue && sr.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_ACCOUNT_ADDRESS_TEXT : string.Empty,
                AddressSendDocId = sr.SR_DEF_ACCOUNT_ADDRESS_ID,
                AccountAddress = new AccountAddressEntity
                {
                    AddressNo = sr.SR_DEF_ADDRESS_HOUSE_NO,
                    Village = sr.SR_DEF_ADDRESS_VILLAGE,
                    Building = sr.SR_DEF_ADDRESS_BUILDING,
                    FloorNo = sr.SR_DEF_ADDRESS_FLOOR_NO,
                    RoomNo = sr.SR_DEF_ADDRESS_ROOM_NO,
                    Moo = sr.SR_DEF_ADDRESS_MOO,
                    Soi = sr.SR_DEF_ADDRESS_SOI,
                    Street = sr.SR_DEF_ADDRESS_STREET,
                    SubDistrict = sr.SR_DEF_ADDRESS_TAMBOL,
                    District = sr.SR_DEF_ADDRESS_AMPHUR,
                    Province = sr.SR_DEF_ADDRESS_PROVINCE,
                    Postcode = sr.SR_DEF_ADDRESS_ZIPCODE
                },

                AfsAssetId = sr.SR_AFS_ASSET_ID,
                AfsAssetNo = sr.SR_AFS_ASSET_NO,
                AfsAssetDesc = sr.SR_AFS_ASSET_DESC,

                NCBCustomerBirthDate = sr.SR_NCB_CUSTOMER_BIRTHDATE,
                NCBCheckStatus = sr.SR_NCB_CHECK_STATUS,
                NCBMarketingUserId = sr.SR_NCB_MARKETING_USER_ID,
                NCBMarketingFullName = sr.SR_NCB_MARKETING_FULL_NAME,
                NCBMarketingBranchId = sr.SR_NCB_MARKETING_BRANCH_ID,
                NCBMarketingBranchName = sr.SR_NCB_MARKETING_BRANCH_NAME,
                NCBMarketingBranchUpper1Id = sr.SR_NCB_MARKETING_BRANCH_UPPER_1_ID,
                NCBMarketingBranchUpper1Name = sr.SR_NCB_MARKETING_BRANCH_UPPER_1_NAME,
                NCBMarketingBranchUpper2Id = sr.SR_NCB_MARKETING_BRANCH_UPPER_2_ID,
                NCBMarketingBranchUpper2Name = sr.SR_NCB_MARKETING_BRANCH_UPPER_2_NAME,

                CreateDate = sr.CREATE_DATE,
                CloseDate = sr.CLOSE_DATE,

                CPN_ProductGroupId = sr.CPN_PRODUCT_GROUP_ID,
                CPN_ProductGroupName = sr.CPN_PRODUCTGROUP_NAME,
                CPN_ProductId = sr.CPN_PRODUCT_ID,
                CPN_ProductName = sr.CPN_PRODUCT_NAME,
                CPN_CampaignId = sr.CPN_CAMPAIGNSERVICE_ID,
                CPN_CampaignName = sr.CPN_CAMPAIGNSERVICE_NAME,
                CPN_SubjectId = sr.CPN_SUBJECT_ID,
                CPN_SubjectName = sr.CPN_SUBJECT_NAME,
                CPN_TypeId = sr.CPN_TYPE_ID,
                CPN_TypeName = sr.CPN_TYPE_NAME,
                CPN_RootCauseId = sr.CPN_ROOT_CAUSE_ID,
                CPN_RootCauseName = sr.CPN_ROOT_CAUSE_NAME,
                CPN_IssuesId = sr.CPN_ISSUES_ID,
                CPN_IssuesName = sr.CPN_ISSUES_NAME,
                CPN_MappingId = sr.CPN_MAPPING_ID,
                CPN_IsSecret = sr.CPN_SECRET,
                CPN_IsCAR = sr.CPN_CAR,
                CPN_IsHPLog = sr.CPN_HPLog100,
                CPN_BUGroupId = sr.CPN_BU_GROUP_ID,
                CPN_IsSummary = sr.CPN_SUMMARY,
                CPN_Cause_Customer = sr.CPN_CAUSE_CUSTOMER,
                CPN_Cause_Customer_Detail = sr.CPN_CAUSE_CUSTOMER_DETAIL,
                CPN_Cause_Staff = sr.CPN_CAUSE_STAFF,
                CPN_Cause_Staff_Detail = sr.CPN_CAUSE_STAFF_DETAIL,
                CPN_Cause_System = sr.CPN_CAUSE_SYSTEM,
                CPN_Cause_System_Detail = sr.CPN_CAUSE_SYSTEM_DETAIL,
                CPN_Cause_Process = sr.CPN_CAUSE_PROCESS,
                CPN_Cause_Process_Detail = sr.CPN_CAUSE_PROCESS_DETAIL,
                CPN_CauseSummaryId = sr.CPN_CAUSE_SUMMARY_ID,
                CPN_SummaryId = sr.CPN_SUMMARY_ID,
                CPN_Fixed_Detail = sr.CPN_FIXED_DETAIL,
                CPN_BU1Code = sr.CPN_BU1_CODE,
                CPN_BU2Code = sr.CPN_BU2_CODE,
                CPN_BU3Code = sr.CPN_BU3_CODE,
                CPN_MSHBranchId = sr.CPN_MSHBranchId
            };

            if (sr.CUSTOMER_ID.HasValue)
            {
                var phones = (from phone in _context.TB_M_PHONE
                              from phoneType in _context.TB_M_PHONE_TYPE.Where(x => x.PHONE_TYPE_ID == phone.PHONE_TYPE_ID).DefaultIfEmpty()
                              where phone.CUSTOMER_ID == sr.CUSTOMER_ID.Value
                              orderby phone.PHONE_ID
                              select new
                              {
                                  phone.PHONE_NO,
                                  phoneType.PHONE_TYPE_CODE,
                                  phoneType.PHONE_TYPE_NAME
                              }).ToList();

                result.Customer.PhoneList = phones.Where(x => x.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax)
                    .Select(cp => new PhoneEntity
                    {
                        PhoneNo = cp.PHONE_NO,
                        PhoneTypeName = cp.PHONE_TYPE_NAME,
                        PhoneTypeCode = cp.PHONE_TYPE_CODE
                    }).OrderBy(p => p.PhoneTypeCode).ToList();

                var fax = phones.FirstOrDefault(x => x.PHONE_TYPE_CODE == Constants.PhoneTypeCode.Fax);
                if (fax != null)
                    result.Customer.Fax = fax.PHONE_NO;
            }

            if (sr.CONTACT_ID.HasValue)
            {
                var phones = (from phone in _context.TB_M_CONTACT_PHONE
                              from phoneType in _context.TB_M_PHONE_TYPE.Where(x => x.PHONE_TYPE_ID == phone.PHONE_TYPE_ID).DefaultIfEmpty()
                              where phone.CONTACT_ID == sr.CONTACT_ID.Value
                              orderby phone.CONTACT_PHONE_ID
                              select new
                              {
                                  phone.PHONE_NO,
                                  phoneType.PHONE_TYPE_CODE,
                                  phoneType.PHONE_TYPE_NAME
                              }).ToList();

                result.Contact.PhoneList = phones.Where(x => x.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax)
                    .Select(cp => new PhoneEntity
                    {
                        PhoneNo = cp.PHONE_NO,
                        PhoneTypeName = cp.PHONE_TYPE_NAME,
                        PhoneTypeCode = cp.PHONE_TYPE_CODE
                    }).OrderBy(p => p.PhoneTypeCode).ToList();

                var fax = phones.FirstOrDefault(x => x.PHONE_TYPE_CODE == Constants.PhoneTypeCode.Fax);
                if (fax != null)
                    result.Contact.Fax = fax.PHONE_NO;
            }

            return result;
        }

        public ServiceRequestForDisplayEntity GetServiceRequestOld(int srId)
        {
            return (from sr in _context.TB_T_SR
                    from customer in _context.TB_M_CUSTOMER.Where(x => x.CUSTOMER_ID == sr.CUSTOMER_ID).DefaultIfEmpty()
                    from customerSubscriptionType in _context.TB_M_SUBSCRIPT_TYPE.Where(x => x.SUBSCRIPT_TYPE_ID == customer.SUBSCRIPT_TYPE_ID).DefaultIfEmpty()
                    from customerTitleTh in _context.TB_M_TITLE.Where(x => x.TITLE_ID == customer.TITLE_TH_ID).DefaultIfEmpty()
                    from customerTitleEn in _context.TB_M_TITLE.Where(x => x.TITLE_ID == customer.TITLE_EN_ID).DefaultIfEmpty()
                    from customerEmployee in _context.TB_R_USER.Where(x => x.USER_ID == customer.EMPLOYEE_ID).DefaultIfEmpty()
                    from customerEmployeeBranch in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == customerEmployee.BRANCH_ID).DefaultIfEmpty()
                    from customerEmployeeUpperBranch1 in _context.TB_R_BRANCH.Where(x => customerEmployeeBranch != null && x.BRANCH_ID == customerEmployeeBranch.BRANCH_ID).DefaultIfEmpty()
                    from customerEmployeeUpperBranch2 in _context.TB_R_BRANCH.Where(x => customerEmployeeUpperBranch1 != null && x.BRANCH_ID == customerEmployeeUpperBranch1.BRANCH_ID).DefaultIfEmpty()
                    from account in _context.TB_M_ACCOUNT.Where(x => x.ACCOUNT_ID == sr.ACCOUNT_ID).DefaultIfEmpty()
                    from contact in _context.TB_M_CONTACT.Where(x => x.CONTACT_ID == sr.CONTACT_ID).DefaultIfEmpty()
                    from contactSubscriptionType in _context.TB_M_SUBSCRIPT_TYPE.Where(x => x.SUBSCRIPT_TYPE_ID == contact.SUBSCRIPT_TYPE_ID).DefaultIfEmpty()
                    from contactTitleTh in _context.TB_M_TITLE.Where(x => x.TITLE_ID == contact.TITLE_TH_ID).DefaultIfEmpty()
                    from contactTitleEn in _context.TB_M_TITLE.Where(x => x.TITLE_ID == contact.TITLE_EN_ID).DefaultIfEmpty()
                    from contactRelationship in _context.TB_M_RELATIONSHIP.Where(x => x.RELATIONSHIP_ID == sr.CONTACT_RELATIONSHIP_ID).DefaultIfEmpty()
                    from productGroup in _context.TB_R_PRODUCTGROUP.Where(x => x.PRODUCTGROUP_ID == sr.PRODUCTGROUP_ID).DefaultIfEmpty()
                    from product in _context.TB_R_PRODUCT.Where(x => x.PRODUCT_ID == sr.PRODUCT_ID).DefaultIfEmpty()
                    from campaignService in _context.TB_R_CAMPAIGNSERVICE.Where(x => x.CAMPAIGNSERVICE_ID == sr.CAMPAIGNSERVICE_ID).DefaultIfEmpty()
                    from area in _context.TB_M_AREA.Where(x => x.AREA_ID == sr.AREA_ID).DefaultIfEmpty()
                    from subArea in _context.TB_M_SUBAREA.Where(x => x.SUBAREA_ID == sr.SUBAREA_ID).DefaultIfEmpty()
                    from type in _context.TB_M_TYPE.Where(x => x.TYPE_ID == sr.TYPE_ID).DefaultIfEmpty()
                    from channel in _context.TB_R_CHANNEL.Where(x => x.CHANNEL_ID == sr.CHANNEL_ID).DefaultIfEmpty()
                    from mediaSource in _context.TB_M_MEDIA_SOURCE.Where(x => x.MEDIA_SOURCE_ID == sr.MEDIA_SOURCE_ID).DefaultIfEmpty()
                    from mappingProduct in _context.TB_M_MAP_PRODUCT.Where(x => x.MAP_PRODUCT_ID == sr.MAP_PRODUCT_ID).DefaultIfEmpty()
                    from srStatus in _context.TB_C_SR_STATUS.Where(x => x.SR_STATUS_ID == sr.SR_STATUS_ID).DefaultIfEmpty()
                    from srEmailTemplate in _context.TB_C_SR_EMAIL_TEMPLATE.Where(x => x.SR_EMAIL_TEMPLATE_ID == sr.DRAFT_SR_EMAIL_TEMPLATE_ID).DefaultIfEmpty()
                    from activityType in _context.TB_C_SR_ACTIVITY_TYPE.Where(x => x.SR_ACTIVITY_TYPE_ID == sr.DRAFT_ACTIVITY_TYPE_ID).DefaultIfEmpty()
                    from ownerUser in _context.TB_R_USER.Where(x => x.USER_ID == sr.OWNER_USER_ID).DefaultIfEmpty()
                    from ownerUserBranch in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == ownerUser.BRANCH_ID).DefaultIfEmpty()
                    from delegateUser in _context.TB_R_USER.Where(x => x.USER_ID == sr.DELEGATE_USER_ID).DefaultIfEmpty()
                    from delegateUserBranch in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == delegateUser.BRANCH_ID).DefaultIfEmpty()
                    from createUser in _context.TB_R_USER.Where(x => x.USER_ID == sr.CREATE_USER).DefaultIfEmpty()
                    from createUserBranch in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == sr.CREATE_BRANCH_ID).DefaultIfEmpty()
                    from updateUser in _context.TB_R_USER.Where(x => x.USER_ID == sr.UPDATE_USER).DefaultIfEmpty()
                    from srPage in _context.TB_C_SR_PAGE.Where(x => x.SR_PAGE_ID == sr.SR_PAGE_ID).DefaultIfEmpty()
                    where sr.SR_ID == srId && sr.SR_STATUS_ID.HasValue
                    select new ServiceRequestForDisplayEntity
                    {
                        SrId = sr.SR_ID,
                        SrNo = sr.SR_NO,
                        CallId = sr.SR_CALL_ID,
                        PhoneNo = sr.SR_ANO,
                        RuleAssignFlag = sr.RULE_ASSIGN_FLAG,
                        Customer = customer != null ? new CustomerEntity
                        {
                            CustomerId = customer.CUSTOMER_ID,
                            CardNo = customer.CARD_NO,
                            BirthDate = customer.BIRTH_DATE,
                            TitleThai = customerTitleTh != null ? new TitleEntity
                            {
                                TitleId = customerTitleTh.TITLE_ID,
                                TitleName = customerTitleTh.TITLE_NAME
                            } : null,
                            TitleEnglish = customerTitleEn != null ? new TitleEntity
                            {
                                TitleId = customerTitleEn.TITLE_ID,
                                TitleName = customerTitleEn.TITLE_NAME
                            } : null,
                            FirstNameThai = customer.FIRST_NAME_TH,
                            FirstNameEnglish = customer.FIRST_NAME_EN,
                            LastNameThai = customer.LAST_NAME_TH,
                            LastNameEnglish = customer.LAST_NAME_EN,
                            PhoneList = customer.TB_M_PHONE.Select(cp => new PhoneEntity
                            {
                                PhoneId = cp.PHONE_ID,
                                PhoneNo = cp.PHONE_NO,
                                PhoneTypeId = cp.PHONE_TYPE_ID,
                                PhoneTypeName = cp.TB_M_PHONE_TYPE.PHONE_TYPE_NAME,
                                PhoneTypeCode = cp.TB_M_PHONE_TYPE.PHONE_TYPE_CODE
                            }).OrderBy(p => p.PhoneId).ToList(),
                            Fax = customer.FAX,
                            Email = customer.EMAIL,
                            CustomerType = customer.TYPE
                        } : null,
                        CustomerSubscriptType = customerSubscriptionType != null ? new SubscriptTypeEntity
                        {
                            SubscriptTypeId = customerSubscriptionType.SUBSCRIPT_TYPE_ID,
                            SubscriptTypeCode = customerSubscriptionType.SUBSCRIPT_TYPE_CODE,
                            SubscriptTypeName = customerSubscriptionType.SUBSCRIPT_TYPE_NAME
                        } : null,
                        CustomerEmployeeCode = customerEmployee != null ? (customerEmployee.EMPLOYEE_CODE ?? string.Empty) : string.Empty,
                        Account = account != null ? new AccountEntity
                        {
                            AccountId = account.ACCOUNT_ID,
                            AccountNo = account.ACCOUNT_NO,
                            Status = account.STATUS,
                            CarNo = account.CAR_NO,
                            ProductGroup = account.PRODUCT_GROUP,
                            Product = account.SUBSCRIPTION_DESC,
                            BranchCode = account.BRANCH_CODE,
                            BranchName = account.BRANCH_NAME
                        } : null,
                        Contact = contact != null ? new ContactEntity
                        {
                            ContactId = contact.CONTACT_ID,
                            CardNo = contact.CARD_NO,
                            BirthDate = contact.BIRTH_DATE,
                            TitleThai = contactTitleTh != null ? new TitleEntity
                            {
                                TitleId = contactTitleTh.TITLE_ID,
                                TitleName = contactTitleTh.TITLE_NAME
                            } : null,
                            TitleEnglish = contactTitleEn != null ? new TitleEntity
                            {
                                TitleId = contactTitleEn.TITLE_ID,
                                TitleName = contactTitleEn.TITLE_NAME
                            } : null,
                            FirstNameThai = contact.FIRST_NAME_TH,
                            FirstNameEnglish = contact.FIRST_NAME_EN,
                            LastNameThai = contact.LAST_NAME_TH,
                            LastNameEnglish = contact.LAST_NAME_EN,
                            PhoneList = contact.TB_M_CONTACT_PHONE.Select(cp => new PhoneEntity
                            {
                                PhoneId = cp.CONTACT_PHONE_ID,
                                PhoneNo = cp.PHONE_NO,
                                PhoneTypeId = cp.PHONE_TYPE_ID,
                                PhoneTypeName = cp.TB_M_PHONE_TYPE.PHONE_TYPE_NAME,
                                PhoneTypeCode = cp.TB_M_PHONE_TYPE.PHONE_TYPE_CODE
                            }).OrderBy(p => p.PhoneId).ToList(),
                            Email = contact.EMAIL
                        } : null,
                        ContactSubscriptType = contactSubscriptionType != null ? new SubscriptTypeEntity
                        {
                            SubscriptTypeId = contactSubscriptionType.SUBSCRIPT_TYPE_ID,
                            SubscriptTypeCode = customerSubscriptionType.SUBSCRIPT_TYPE_CODE,
                            SubscriptTypeName = contactSubscriptionType.SUBSCRIPT_TYPE_NAME
                        } : null,
                        Relationship = contactRelationship != null ? new RelationshipEntity
                        {
                            RelationshipId = contactRelationship.RELATIONSHIP_ID,
                            RelationshipName = contactRelationship.RELATIONSHIP_NAME,
                            RelationshipDesc = contactRelationship.RELATIONSHIP_DESC
                        } : null,
                        ContactAccountNo = sr.CONTACT_ACCOUNT_NO ?? string.Empty,
                        MappingProductId = sr.MAP_PRODUCT_ID,
                        ProductGroup = productGroup != null ? new ProductGroupEntity
                        {
                            ProductGroupId = productGroup.PRODUCTGROUP_ID,
                            ProductGroupName = productGroup.PRODUCTGROUP_NAME
                        } : null,
                        Product = product != null ? new ProductEntity
                        {
                            ProductId = product.PRODUCT_ID,
                            ProductName = product.PRODUCT_NAME
                        } : null,
                        CampaignService = campaignService != null ? new CampaignServiceEntity
                        {
                            CampaignServiceId = campaignService.CAMPAIGNSERVICE_ID,
                            CampaignServiceName = campaignService.CAMPAIGNSERVICE_NAME
                        } : null,
                        Area = area != null ? new AreaEntity
                        {
                            AreaId = area.AREA_ID,
                            AreaName = area.AREA_NAME
                        } : null,
                        SubArea = subArea != null ? new SubAreaEntity
                        {
                            SubareaId = subArea.SUBAREA_ID,
                            SubareaName = subArea.SUBAREA_NAME
                        } : null,
                        Type = type != null ? new TypeEntity
                        {
                            TypeId = type.TYPE_ID,
                            TypeName = type.TYPE_NAME
                        } : null,
                        MediaSource = mediaSource != null ? new MediaSourceEntity
                        {
                            MediaSourceId = mediaSource.MEDIA_SOURCE_ID,
                            Name = mediaSource.MEDIA_SOURCE_NAME
                        } : null,
                        Channel = channel != null ? new ChannelEntity
                        {
                            ChannelId = channel.CHANNEL_ID,
                            Code = channel.CHANNEL_CODE,
                            Name = channel.CHANNEL_NAME
                        } : null,
                        Subject = sr.SR_SUBJECT,
                        Remark = sr.SR_REMARK,
                        SrPageId = sr.SR_PAGE_ID,
                        SrPageCode = srPage.SR_PAGE_CODE,
                        OwnerUser = ownerUser != null ? new UserEntity
                        {
                            UserId = ownerUser.USER_ID,
                            Firstname = ownerUser.FIRST_NAME,
                            Lastname = ownerUser.LAST_NAME,
                            PositionCode = ownerUser.POSITION_CODE
                        } : null,
                        OwnerUserBranch = ownerUserBranch != null ? new BranchEntity
                        {
                            BranchId = ownerUserBranch.BRANCH_ID,
                            BranchCode = createUserBranch.BRANCH_CODE,
                            BranchName = ownerUserBranch.BRANCH_NAME
                        } : null,
                        DelegateUser = delegateUser != null ? new UserEntity
                        {
                            UserId = delegateUser.USER_ID,
                            Firstname = delegateUser.FIRST_NAME,
                            Lastname = delegateUser.LAST_NAME,
                            PositionCode = delegateUser.POSITION_CODE
                        } : null,
                        DelegateUserBranch = delegateUserBranch != null ? new BranchEntity
                        {
                            BranchId = delegateUserBranch.BRANCH_ID,
                            BranchCode = createUserBranch.BRANCH_CODE,
                            BranchName = delegateUserBranch.BRANCH_NAME
                        } : null,
                        CreateUser = createUser != null ? new UserEntity
                        {
                            UserId = createUser.USER_ID,
                            Firstname = createUser.FIRST_NAME,
                            Lastname = createUser.LAST_NAME,
                            PositionCode = createUser.POSITION_CODE
                        } : null,
                        CreateBranch = createUserBranch != null ? new BranchEntity
                        {
                            BranchId = createUserBranch.BRANCH_ID,
                            BranchCode = createUserBranch.BRANCH_CODE,
                            BranchName = createUserBranch.BRANCH_NAME
                        } : null,
                        UpdateUser = updateUser != null ? new UserEntity
                        {
                            UserId = updateUser.USER_ID,
                            Firstname = updateUser.FIRST_NAME,
                            Lastname = updateUser.LAST_NAME,
                            PositionCode = updateUser.POSITION_CODE
                        } : null,
                        SRStatus = srStatus != null ? new SRStatusEntity
                        {
                            SRStatusId = srStatus.SR_STATUS_ID,
                            SRStatusCode = srStatus.SR_STATUS_CODE,
                            SRStatusName = srStatus.SR_STATUS_NAME
                        } : null,
                        IsVerify = sr.SR_IS_VERIFY,
                        IsVerifyPass = sr.SR_IS_VERIFY_PASS,
                        DraftSrEmailTemplateId = (srStatus != null && srStatus.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_SR_EMAIL_TEMPLATE_ID : null,
                        DraftActivityDescription = (srStatus != null && srStatus.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_ACTIVITY_DESC : string.Empty,
                        DraftActivityTypeId = (srStatus != null && srStatus.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_ACTIVITY_TYPE_ID : null,
                        DraftSendMailSender = (srStatus != null && srStatus.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_MAIL_SENDER : string.Empty,
                        DraftSendMailTo = (srStatus != null && srStatus.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_MAIL_TO : string.Empty,
                        DraftSendMailCc = (srStatus != null && srStatus.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_MAIL_CC : string.Empty,
                        DraftSendMailSubject = (srStatus != null && srStatus.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_MAIL_SUBJECT : string.Empty,
                        DraftSendMailBody = (srStatus != null && srStatus.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_MAIL_BODY : string.Empty,
                        DraftIsEmailDelegate = (srStatus != null && srStatus.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? (sr.DRAFT_IS_SEND_EMAIL_FOR_DELEGATE ?? false) : false,
                        DraftIsClose = (srStatus != null && srStatus.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? (sr.DRAFT_IS_CLOSE ?? false) : false,
                        DraftVerifyAnswerJson = (srStatus != null && srStatus.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_VERIFY_ANSWER_JSON : string.Empty,
                        DraftAttachmentJson = (srStatus != null && srStatus.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_ATTACHMENT_JSON : string.Empty,
                        DraftAccountAddressId = (srStatus != null && srStatus.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.SR_DEF_ACCOUNT_ADDRESS_ID : (int?)null,
                        DraftAccountAddressText = (srStatus != null && srStatus.SR_STATUS_CODE == Constants.SRStatusCode.Draft) ? sr.DRAFT_ACCOUNT_ADDRESS_TEXT : string.Empty,
                        AddressSendDocId = sr.SR_DEF_ACCOUNT_ADDRESS_ID,
                        AccountAddress = new AccountAddressEntity
                        {
                            AddressNo = sr.SR_DEF_ADDRESS_HOUSE_NO,
                            Village = sr.SR_DEF_ADDRESS_VILLAGE,
                            Building = sr.SR_DEF_ADDRESS_BUILDING,
                            FloorNo = sr.SR_DEF_ADDRESS_FLOOR_NO,
                            RoomNo = sr.SR_DEF_ADDRESS_ROOM_NO,
                            Moo = sr.SR_DEF_ADDRESS_MOO,
                            Soi = sr.SR_DEF_ADDRESS_SOI,
                            Street = sr.SR_DEF_ADDRESS_STREET,
                            SubDistrict = sr.SR_DEF_ADDRESS_TAMBOL,
                            District = sr.SR_DEF_ADDRESS_AMPHUR,
                            Province = sr.SR_DEF_ADDRESS_PROVINCE,
                            Postcode = sr.SR_DEF_ADDRESS_ZIPCODE
                        },

                        AfsAssetId = sr.SR_AFS_ASSET_ID,
                        AfsAssetNo = sr.SR_AFS_ASSET_NO,
                        AfsAssetDesc = sr.SR_AFS_ASSET_DESC,

                        NCBCustomerBirthDate = sr.SR_NCB_CUSTOMER_BIRTHDATE,
                        NCBCheckStatus = sr.SR_NCB_CHECK_STATUS,
                        NCBMarketingUserId = sr.SR_NCB_MARKETING_USER_ID,
                        NCBMarketingFullName = sr.SR_NCB_MARKETING_FULL_NAME,
                        NCBMarketingBranchId = sr.SR_NCB_MARKETING_BRANCH_ID,
                        NCBMarketingBranchName = sr.SR_NCB_MARKETING_BRANCH_NAME,
                        NCBMarketingBranchUpper1Id = sr.SR_NCB_MARKETING_BRANCH_UPPER_1_ID,
                        NCBMarketingBranchUpper1Name = sr.SR_NCB_MARKETING_BRANCH_UPPER_1_NAME,
                        NCBMarketingBranchUpper2Id = sr.SR_NCB_MARKETING_BRANCH_UPPER_2_ID,
                        NCBMarketingBranchUpper2Name = sr.SR_NCB_MARKETING_BRANCH_UPPER_2_NAME,

                        CreateDate = sr.CREATE_DATE,
                        CloseDate = sr.CLOSE_DATE
                    }).SingleOrDefault();
        }

        public AccountAddressEntity GetDefaultAccountAddress(int accountId)
        {
            var account = _context.TB_M_ACCOUNT.Single(c => c.ACCOUNT_ID == accountId);
            var query = _context.TB_M_ACCOUNT_ADDRESS
                .Where(
                    q => q.CUSTOMER_ID == account.CUSTOMER_ID
                        && q.PRODUCT_GROUP == account.PRODUCT_GROUP
                        //&& q.SUBSCRIPTION_CODE == account.TB_M_CUSTOMER.TB_M_SUBSCRIPT_TYPE.SUBSCRIPT_TYPE_CODE
                        && q.SUBSCRIPTION_CODE == account.SUBSCRIPTION_CODE
                        && q.ADDRESS_TYPE_NAME == Constants.AddressType.SendingDoc
                    ).ToList();

            if (query.Count != 1)
                return null;

            var result = query.First();
            return new AccountAddressEntity
            {
                AddressId = result.ADDRESS_ID,
                AddressNo = result.ADDRESS_NO ?? "",
                Village = result.VILLAGE ?? "",
                Building = result.BUILDING ?? "",
                FloorNo = result.FLOOR_NO ?? "",
                RoomNo = result.ROOM_NO ?? "",
                Moo = result.MOO ?? "",
                Street = result.STREET ?? "",
                Soi = result.SOI ?? "",
                SubDistrict = result.SUB_DISTRICT ?? "",
                District = result.DISTRICT ?? "",
                Province = result.PROVINCE ?? "",
                Country = result.COUNTRY ?? "",
                Postcode = result.POSTCODE ?? ""
            };
        }

        public SrEmailTemplateEntity GetSrEmailTemplate(int id)
        {
            return (from t in _context.TB_C_SR_EMAIL_TEMPLATE
                    where t.SR_EMAIL_TEMPLATE_ID == id
                    select new SrEmailTemplateEntity
                    {
                        SrEmailTemplateId = t.SR_EMAIL_TEMPLATE_ID,
                        Name = t.SR_EMAIL_TEMPLATE_NAME,
                        Sender = t.SR_EMAIL_TEMPLATE_SENDER,
                        Subject = t.SR_EMAIL_TEMPLATE_SUBJECT,
                        Content = t.SR_EMAIL_TEMPLATE_CONTENT
                    }).SingleOrDefault();
        }

        public List<AfsAssetEntity> AutoCompleteSearchAfsAsset(string keyword, int limit)
        {
            var query = _context.TB_M_AFS_ASSET.AsQueryable();
            query = query.Where(q => q.ASSET_NO.Contains(keyword) && q.STATUS.ToUpper() == "Y");
            query = query.OrderBy(q => q.ASSET_NO);

            return query.Take(limit).Select(item => new AfsAssetEntity
            {
                AssetId = item.ASSET_ID,
                AssetNo = item.ASSET_NO
            }).ToList();
        }

        public AfsAssetEntity GetAssetInfo(int afsAssetId)
        {
            return (from asset in _context.TB_M_AFS_ASSET
                    from user in _context.TB_R_USER.Where(x => x.USER_ID == asset.SALE_USER_ID).DefaultIfEmpty()
                    from userBranch in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == user.BRANCH_ID).DefaultIfEmpty()
                    where asset.ASSET_ID == afsAssetId
                    select new AfsAssetEntity
                    {
                        AssetId = asset.ASSET_ID,
                        ProjectDes = asset.PROJECT_DES,
                        StatusDesc = asset.STATUS_DESC,
                        Amphur = asset.AMPHUR,
                        Province = asset.PROVINCE,
                        SaleName = asset.SALE_NAME,
                        PhoneNo = asset.PHONE_NO,
                        MobileNo = asset.MOBILE_NO,
                        Email = asset.EMAIL,
                        SaleOwnerUser = user != null ? new UserEntity
                        {
                            UserId = user.USER_ID,
                            Firstname = user.FIRST_NAME,
                            Lastname = user.LAST_NAME,
                            PositionCode = user.POSITION_CODE
                        } : null,
                        SaleOwnerBranch = userBranch != null ? new BranchEntity
                        {
                            BranchId = userBranch.BRANCH_ID,
                            BranchCode = userBranch.BRANCH_CODE,
                            BranchName = userBranch.BRANCH_NAME
                        } : null
                    }).SingleOrDefault();
        }

        public AfsAssetEntity GetAssetInfo(string assetNo)
        {
            return (from asset in _context.TB_M_AFS_ASSET
                    from user in _context.TB_R_USER.Where(x => x.USER_ID == asset.SALE_USER_ID).DefaultIfEmpty()
                    from userBranch in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == user.BRANCH_ID).DefaultIfEmpty()
                    where asset.ASSET_NO == assetNo
                    select new AfsAssetEntity
                    {
                        AssetId = asset.ASSET_ID,
                        ProjectDes = asset.PROJECT_DES,
                        StatusDesc = asset.STATUS_DESC,
                        Amphur = asset.AMPHUR,
                        Province = asset.PROVINCE,
                        SaleName = asset.SALE_NAME,
                        PhoneNo = asset.PHONE_NO,
                        MobileNo = asset.MOBILE_NO,
                        Email = asset.EMAIL,
                        SaleOwnerUser = user != null ? new UserEntity
                        {
                            UserId = user.USER_ID,
                            Firstname = user.FIRST_NAME,
                            Lastname = user.LAST_NAME,
                            PositionCode = user.POSITION_CODE
                        } : null,
                        SaleOwnerBranch = userBranch != null ? new BranchEntity
                        {
                            BranchId = userBranch.BRANCH_ID,
                            BranchCode = userBranch.BRANCH_CODE,
                            BranchName = userBranch.BRANCH_NAME
                        } : null
                    }).SingleOrDefault();
        }

        public CustomerContactEntity GetCustomerContact(int id)
        {
            return (from cc in _context.TB_M_CUSTOMER_CONTACT
                    from con in _context.TB_M_CONTACT.Where(x => x.CONTACT_ID == cc.CONTACT_ID).DefaultIfEmpty()
                    from ac in _context.TB_M_ACCOUNT.Where(x => x.ACCOUNT_ID == cc.ACCOUNT_ID).DefaultIfEmpty()
                    from rl in _context.TB_M_RELATIONSHIP.Where(x => x.RELATIONSHIP_ID == cc.RELATIONSHIP_ID).DefaultIfEmpty()
                    from cust in _context.TB_M_CUSTOMER.Where(x => x.CUSTOMER_ID == cc.CUSTOMER_ID).DefaultIfEmpty()
                    from ttTh in _context.TB_M_TITLE.Where(x => x.TITLE_ID == cust.TITLE_TH_ID).DefaultIfEmpty()
                    from ttEn in _context.TB_M_TITLE.Where(x => x.TITLE_ID == cust.TITLE_EN_ID).DefaultIfEmpty()
                    where cc.CUSTOMER_CONTACT_ID == id
                    select new CustomerContactEntity
                    {
                        CustomerContactId = cc.CUSTOMER_CONTACT_ID,
                        ContactId = con.CONTACT_ID,
                        AccountId = ac.ACCOUNT_ID,
                        AccountNo = ac.ACCOUNT_NO,
                        Product = ac.SUBSCRIPTION_DESC,
                        RelationshipId = rl != null ? rl.RELATIONSHIP_ID : 0,
                        RelationshipName = rl != null ? rl.RELATIONSHIP_NAME : "",
                        IsEdit = cc.IS_EDIT.Value,
                        CustomerFullNameTh = cust != null ? (ttTh != null ? ttTh.TITLE_NAME : "") + cust.FIRST_NAME_TH + " " + cust.LAST_NAME_TH : "",
                        CustomerFullNameEn = cust != null ? (ttEn != null ? ttEn.TITLE_NAME : "") + cust.FIRST_NAME_EN + " " + cust.LAST_NAME_EN : ""
                    }).SingleOrDefault();
        }

        public IEnumerable<ServiceDocumentEntity> GetTabDocumentList(DocumentSearchFilter searchFilter)
        {
            IEnumerable<ServiceDocumentEntity> results;

            if (searchFilter.SrOnly)
            {
                results = _context.TB_T_SR_ATTACHMENT
                            .Where(q => q.SR_ID == searchFilter.SrId)
                            .Select(q => new ServiceDocumentEntity
                            {
                                SrAttachId = q.SR_ATTACHMENT_ID,
                                SrId = q.SR_ID,
                                SrActivityId = q.SR_ACTIVITY_ID,
                                AttachmentName = q.SR_ATTACHMENT_NAME,
                                AttachmentDesc = q.SR_ATTACHMENT_DESC,
                                SrReference = q.TB_T_SR.SR_NO,
                                DocumentLevel = Constants.DocumentLevel.Sr,
                                CreateDate = q.CREATE_DATE,
                                ExpireDate = q.EXPIRY_DATE,
                                DocumentTypes = q.TB_T_SR_ATTACHMENT_DOCUMENT_TYPE.Select(item => new DocumentTypeEntity
                                {
                                    Name = item.TB_M_DOCUMENT_TYPE.DOCUMENT_TYPE_NAME
                                }).ToList(),
                                Status = q.STATUS,
                                CreateUserId = q.CREATE_USER
                            });
            }
            else
            {
                var query = from at in _context.TB_M_CUSTOMER_ATTACHMENT
                            where at.CUSTOMER_ID == searchFilter.CustomerId
                            select new ServiceDocumentEntity
                            {
                                SrAttachId = at.CUSTOMER_ATTACHMENT_ID,
                                SrId = null,
                                SrActivityId = null,
                                AttachmentName = at.ATTACHMENT_NAME,
                                AttachmentDesc = at.ATTACHMENT_DESC,
                                SrReference = "",
                                DocumentLevel = Constants.DocumentLevel.Customer,
                                //FileName = at.FILE_NAME,
                                //ContentType = at.CONTENT_TYPE,
                                //Url = at.URL,
                                CreateDate = at.CREATE_DATE,
                                ExpireDate = at.EXPIRY_DATE,
                                Status = at.STATUS,
                                CreateUserId = at.CREATE_USER
                            };

                var querySR = from at in _context.TB_T_SR_ATTACHMENT
                              where at.CUSTOMER_ID == searchFilter.CustomerId
                              && at.TB_T_SR.TB_C_SR_STATUS.SR_STATUS_CODE != Constants.SRStatusCode.Draft
                              select new ServiceDocumentEntity
                              {
                                  SrAttachId = at.SR_ATTACHMENT_ID,
                                  SrId = at.TB_T_SR != null ? at.TB_T_SR.SR_ID : (int?)null,
                                  SrActivityId = null,
                                  AttachmentName = at.SR_ATTACHMENT_NAME,
                                  AttachmentDesc = at.SR_ATTACHMENT_DESC,
                                  SrReference = at.TB_T_SR != null ? at.TB_T_SR.SR_NO : null,
                                  DocumentLevel = Constants.DocumentLevel.Sr,
                                  //Filename = at.SR_ATTACHMENT_FILE_NAME,
                                  //ContentType = at.SR_ATTACHMENT_CONTENT_TYPE,
                                  //Url = at.SR_ATTACHMENT_URL,
                                  CreateDate = at.CREATE_DATE,
                                  ExpireDate = at.EXPIRY_DATE,
                                  Status = at.STATUS,
                                  CreateUserId = at.CREATE_USER
                              };

                results = query.Concat(querySR); // union all
            }

            searchFilter.TotalRecords = results.Count();

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            switch (searchFilter.SortField)
            {
                case "FileName":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(r => r.AttachmentName)
                        : results.OrderByDescending(r => r.AttachmentName);
                    break;

                case "DescName":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(r => r.AttachmentDesc)
                        : results.OrderByDescending(r => r.AttachmentDesc);
                    break;

                case "SrRef":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(r => r.SrReference)
                        : results.OrderByDescending(r => r.SrReference);
                    break;

                case "ExpireDate":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(r => r.ExpireDate)
                        : results.OrderByDescending(r => r.ExpireDate);
                    break;

                case "CreateDate":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(r => r.CreateDate)
                        : results.OrderByDescending(r => r.CreateDate);
                    break;
                case "Status":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(r => r.Status)
                        : results.OrderByDescending(r => r.Status);
                    break;
                default:
                    results = results.OrderByDescending(r => r.CreateDate);
                    break;
            }

            var serviceDocumentEntityList = results.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<ServiceDocumentEntity>();

            if (!searchFilter.SrOnly)
            {
                // Get Document Types
                foreach (var document in serviceDocumentEntityList)
                {
                    if (document.DocumentLevel == Constants.DocumentLevel.Customer)
                    {
                        document.DocumentTypes = _context.TB_T_ATTACHMENT_TYPE
                                .Where(x => x.CUSTOMER_ATTACHMENT_ID == document.SrAttachId)
                                .Select(item => new DocumentTypeEntity
                                {
                                    Name = item.TB_M_DOCUMENT_TYPE.DOCUMENT_TYPE_NAME
                                }).ToList();
                    }
                    else if (document.DocumentLevel == Constants.DocumentLevel.Sr)
                    {
                        document.DocumentTypes = _context.TB_T_SR_ATTACHMENT_DOCUMENT_TYPE
                                .Where(x => x.SR_ATTACHMENT_ID == document.SrAttachId)
                                .Select(item => new DocumentTypeEntity
                                {
                                    Name = item.TB_M_DOCUMENT_TYPE.DOCUMENT_TYPE_NAME
                                }).ToList();
                    }
                    else
                    {
                        document.DocumentTypes = new List<DocumentTypeEntity>();
                    }
                }
            }

            return serviceDocumentEntityList;
        }

        public IEnumerable<ServiceRequestLoggingResult> GetTabLoggingList(LoggingSearchFilter searchFilter)
        {
            var query = (from logging in _context.TB_L_SR_LOGGING
                         from oldStatus in _context.TB_C_SR_STATUS.Where(s => s.SR_STATUS_ID == logging.SR_STATUS_ID_OLD).DefaultIfEmpty()
                         from newStatus in _context.TB_C_SR_STATUS.Where(s => s.SR_STATUS_ID == logging.SR_STATUS_ID_NEW).DefaultIfEmpty()
                         from createUser in _context.TB_R_USER.Where(s => s.USER_ID == logging.CREATE_USER).DefaultIfEmpty()
                         from oldOwnerUser in _context.TB_R_USER.Where(s => s.USER_ID == logging.OWNER_USER_ID_OLD).DefaultIfEmpty()
                         from newOwnerUser in _context.TB_R_USER.Where(s => s.USER_ID == logging.OWNER_USER_ID_NEW).DefaultIfEmpty()
                         from oldDelegateUser in _context.TB_R_USER.Where(s => s.USER_ID == logging.DELEGATE_USER_ID_OLD).DefaultIfEmpty()
                         from newDelegateUser in _context.TB_R_USER.Where(s => s.USER_ID == logging.DELEGATE_USER_ID_NEW).DefaultIfEmpty()
                         where (logging.SR_ID == searchFilter.SrId.Value)
                         select new ServiceRequestLoggingResult
                         {
                             SrLoggingId = logging.SR_LOGGING_ID,
                             CreateDate = logging.CREATE_DATE,
                             SystemAction = logging.SR_LOGGING_SYSTEM_ACTION,
                             Action = logging.SR_LOGGING_ACTION,
                             StatusOld = oldStatus != null ? oldStatus.SR_STATUS_NAME : "",
                             StatusNew = newStatus != null ? newStatus.SR_STATUS_NAME : "",
                             CreateUser = (createUser != null
                                 ? new UserEntity
                                 {
                                     UserId = createUser.USER_ID,
                                     Firstname = createUser.FIRST_NAME,
                                     Lastname = createUser.LAST_NAME,
                                     PositionCode = createUser.POSITION_CODE
                                 }
                                 : null),
                             CreateUsername = logging.CREATE_USERNAME,
                             OwnerOldUser = (oldOwnerUser != null
                                 ? new UserEntity
                                 {
                                     UserId = oldOwnerUser.USER_ID,
                                     Firstname = oldOwnerUser.FIRST_NAME,
                                     Lastname = oldOwnerUser.LAST_NAME,
                                     PositionCode = oldOwnerUser.POSITION_CODE
                                 }
                                 : null),
                             OwnerNewUser = (newOwnerUser != null
                                 ? new UserEntity
                                 {
                                     UserId = newOwnerUser.USER_ID,
                                     Firstname = newOwnerUser.FIRST_NAME,
                                     Lastname = newOwnerUser.LAST_NAME,
                                     PositionCode = newOwnerUser.POSITION_CODE
                                 }
                                 : null),
                             DelegateOldUser = (oldDelegateUser != null
                                 ? new UserEntity
                                 {
                                     UserId = oldDelegateUser.USER_ID,
                                     Firstname = oldDelegateUser.FIRST_NAME,
                                     Lastname = oldDelegateUser.LAST_NAME,
                                     PositionCode = oldDelegateUser.POSITION_CODE
                                 }
                                 : null),
                             DelegateNewUser = (newDelegateUser != null
                                 ? new UserEntity
                                 {
                                     UserId = newDelegateUser.USER_ID,
                                     Firstname = newDelegateUser.FIRST_NAME,
                                     Lastname = newDelegateUser.LAST_NAME,
                                     PositionCode = newDelegateUser.POSITION_CODE
                                 }
                                 : null),
                             OverSlaMinute = logging.OVER_SLA_MINUTE,
                             OverSlaTimes = logging.OVER_SLA_TIMES,
                             WorkingMinute = logging.WORKING_MINUTE,
                             CPN_IsSecretOld = logging.SR_LOGGING_ACTION == Constants.SrLogAction.Secret ? logging.BIT_VALUE_OLD : null,
                             CPN_IsSecretNew = logging.SR_LOGGING_ACTION == Constants.SrLogAction.Secret ? logging.BIT_VALUE_NEW : null,
                             WorkingHour = logging.WORKING_HOUR
                         });

            searchFilter.TotalRecords = query.Count();

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            switch (searchFilter.SortField)
            {
                default:
                    query = query.OrderByDescending(r => r.CreateDate);
                    break;
            }

            return query.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<ServiceRequestLoggingResult>();
        }

        public IEnumerable<ServiceRequestActivityEntity> GetSrActivitys(int? srId)
        {
            return (from sa in _context.TB_T_SR_ACTIVITY.AsNoTracking()
                    where (!srId.HasValue || sa.SR_ID == srId.Value)
                    select new ServiceRequestActivityEntity()
                    {
                        SrActivityId = sa.SR_ACTIVITY_ID,
                        SrNo = sa.TB_T_SR.SR_NO
                    });
        }

        public IQueryable<ServiceRequestActivityResult> GetSrActivityQueryable(bool isSrOnly, int? srId, int? customerId)
        {
            return (from sa in _context.TB_T_SR_ACTIVITY
                    from sr in _context.TB_T_SR.Where(x => x.SR_ID == sa.SR_ID).DefaultIfEmpty()
                    from customer in _context.TB_M_CUSTOMER.Where(x => x.CUSTOMER_ID == sr.CUSTOMER_ID).DefaultIfEmpty()
                    from account in _context.TB_M_ACCOUNT.Where(x => x.ACCOUNT_ID == sr.ACCOUNT_ID).DefaultIfEmpty()
                    from status in _context.TB_C_SR_STATUS.Where(s => s.SR_STATUS_ID == sa.SR_STATUS_ID).DefaultIfEmpty()
                    from office in _context.TB_R_USER.Where(x => x.USER_ID == sa.CREATE_USER).DefaultIfEmpty()
                    from cb in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == sa.CREATE_BRANCH_ID).DefaultIfEmpty()
                    from cu in _context.TB_R_USER.Where(x => x.USER_ID == sa.CREATE_USER).DefaultIfEmpty()
                    from ob in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == sa.OWNER_BRANCH_ID).DefaultIfEmpty()
                    from ou in _context.TB_R_USER.Where(x => x.USER_ID == sa.OWNER_USER_ID).DefaultIfEmpty()
                    from db in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == sa.DELEGATE_BRANCH_ID).DefaultIfEmpty()
                    from du in _context.TB_R_USER.Where(x => x.USER_ID == sa.DELEGATE_USER_ID).DefaultIfEmpty()
                    from at in _context.TB_C_SR_ACTIVITY_TYPE.Where(x => x.SR_ACTIVITY_TYPE_ID == sa.SR_ACTIVITY_TYPE_ID).DefaultIfEmpty()
                    from st in _context.TB_C_SR_STATUS.Where(x => x.SR_STATUS_ID == sa.SR_STATUS_ID).DefaultIfEmpty()
                    from ste in _context.TB_C_SR_STATE.Where(x => x.SR_STATE_ID == st.SR_STATE_ID).DefaultIfEmpty()
                    from pd in _context.TB_R_PRODUCT.Where(x => x.PRODUCT_ID == sr.PRODUCT_ID).DefaultIfEmpty()
                    from ty in _context.TB_M_TYPE.Where(x => x.TYPE_ID == sr.TYPE_ID).DefaultIfEmpty()
                    where (isSrOnly ? sa.SR_ID == srId.Value : sa.TB_T_SR.CUSTOMER_ID == customerId.Value)
                    orderby sr.SR_NO descending, sa.CREATE_DATE descending
                    select new ServiceRequestActivityResult
                    {
                        SystemCode = "CSM",
                        SrActivityId = sa.SR_ACTIVITY_ID,
                        CustomerCardNo = customer != null ? customer.CARD_NO : string.Empty,
                        AccountNo = account != null ? account.ACCOUNT_NO : string.Empty,
                        SrNo = sr.SR_NO,
                        ProductName = pd != null ? pd.PRODUCT_NAME : string.Empty,
                        TypeName = ty != null ? ty.TYPE_NAME : string.Empty,
                        AreaName = sr.TB_M_AREA != null ? sr.TB_M_AREA.AREA_NAME : string.Empty,
                        SubAreaName = sr.TB_M_SUBAREA != null ? sr.TB_M_SUBAREA.SUBAREA_NAME : string.Empty,
                        StatusDesc = status != null ? status.SR_STATUS_NAME : string.Empty,
                        Date = sa.CREATE_DATE,
                        ContactUserFirstName = sr.TB_M_CONTACT != null ? sr.TB_M_CONTACT.FIRST_NAME_TH : string.Empty,
                        ContactUserLastName = sr.TB_M_CONTACT != null ? sr.TB_M_CONTACT.LAST_NAME_TH : string.Empty,
                        ActivityDesc = sa.SR_ACTIVITY_DESC,
                        OfficerUserFirstName = office != null ? office.FIRST_NAME : sa.CREATE_USERNAME,
                        OfficerUserLastName = office != null ? office.LAST_NAME : string.Empty,
                        OfficerUserPositionCode = office != null ? office.POSITION_CODE : string.Empty,
                        CreatorBranchName = cb.BRANCH_NAME,
                        CreatorUserFirstName = cu.FIRST_NAME,
                        CreatorUserLastName = cu.LAST_NAME,
                        CreatorUserPositionCode = cu.POSITION_CODE,
                        OwnerBranchName = ob.BRANCH_NAME,
                        OwnerUserFirstName = ou.FIRST_NAME,
                        OwnerUserLastName = ou.LAST_NAME,
                        OwnerUserPositionCode = ou.POSITION_CODE,
                        DelegateBranchName = db.BRANCH_NAME,
                        DelegateUserFirstName = du.FIRST_NAME,
                        DelegateUserLastName = du.LAST_NAME,
                        DelegateUserPositionCode = du.POSITION_CODE,
                        IsSendEmail = sa.SR_EMAIL_TEMPLATE_ID.HasValue,
                        EmailSender = sa.SR_EMAIL_TEMPLATE_ID.HasValue ? sa.SR_ACTIVITY_EMAIL_SENDER : "",
                        EmailTo = sa.SR_EMAIL_TEMPLATE_ID.HasValue ? sa.SR_ACTIVITY_EMAIL_TO : "",
                        EmailCc = sa.SR_EMAIL_TEMPLATE_ID.HasValue ? sa.SR_ACTIVITY_EMAIL_CC : "",
                        EmailBcc = sa.SR_EMAIL_TEMPLATE_ID.HasValue ? sa.SR_ACTIVITY_EMAIL_BCC : "",
                        EmailSubject = sa.SR_EMAIL_TEMPLATE_ID.HasValue ? sa.SR_ACTIVITY_EMAIL_SUBJECT : "",
                        EmailBody = sa.SR_ACTIVITY_EMAIL_BODY,
                        EmailAttachments = sa.SR_EMAIL_TEMPLATE_ID.HasValue ? sa.SR_ACTIVITY_EMAIL_ATTACHMENTS : "",
                        ActivityTypeName = at.SR_ACTIVITY_TYPE_NAME,
                        SRStatusName = st.SR_STATUS_NAME,
                        SRStateName = ste.SR_STATE_NAME,

                        ChannelName = sr.CHANNEL_ID.HasValue ? sr.TB_R_CHANNEL.CHANNEL_NAME : "",
                        MediaSourceName = sr.MEDIA_SOURCE_ID.HasValue ? sr.TB_M_MEDIA_SOURCE.MEDIA_SOURCE_NAME : "",
                        Subject = sr.SR_SUBJECT,
                        Remark = sr.SR_REMARK,
                        Verify = sr.SR_IS_VERIFY,
                        VerifyResult = sr.SR_IS_VERIFY_PASS,
                        SrPageId = sr.SR_PAGE_ID.Value,
                        AddressHouseNo = sr.SR_DEF_ADDRESS_HOUSE_NO,
                        AddressVillage = sr.SR_DEF_ADDRESS_VILLAGE,
                        AddressBuilding = sr.SR_DEF_ADDRESS_BUILDING,
                        AddressFloorNo = sr.SR_DEF_ADDRESS_FLOOR_NO,
                        AddressRoomNo = sr.SR_DEF_ADDRESS_ROOM_NO,
                        AddressMoo = sr.SR_DEF_ADDRESS_MOO,
                        AddressSoi = sr.SR_DEF_ADDRESS_SOI,
                        AddressStreet = sr.SR_DEF_ADDRESS_STREET,
                        AddressTambol = sr.SR_DEF_ADDRESS_TAMBOL,
                        AddressAmphur = sr.SR_DEF_ADDRESS_AMPHUR,
                        AddressProvince = sr.SR_DEF_ADDRESS_PROVINCE,
                        AddressZipCode = sr.SR_DEF_ADDRESS_ZIPCODE,
                        AFSAssetNo = sr.SR_AFS_ASSET_NO,
                        AFSAssetDesc = sr.SR_AFS_ASSET_DESC,
                        NCBCustomerBirthDate = sr.SR_NCB_CUSTOMER_BIRTHDATE,
                        NCBMarketingBranchUpper1Name = sr.SR_NCB_MARKETING_BRANCH_UPPER_1_NAME,
                        NCBMarketingBranchUpper2Name = sr.SR_NCB_MARKETING_BRANCH_UPPER_2_NAME,
                        NCBMarketingBranchName = sr.SR_NCB_MARKETING_BRANCH_NAME,
                        NCBMarketingFullName = sr.SR_NCB_MARKETING_FULL_NAME,
                        NCBCheckStatus = sr.SR_NCB_CHECK_STATUS,
                        Is_Secret = sa.IS_SECRET,
                        IsNotSendCAR = sa.IS_SEND_CAR,
                        SendCARStatus = sa.ACTIVITY_CAR_SUBMIT_STATUS
                    });
        }

        public IEnumerable<ServiceRequestActivityResult> GetTabActivityList(ActivityTabSearchFilter searchFilter)
        {
            var query = GetSrActivityQueryable(searchFilter.SrOnly, searchFilter.SrId, searchFilter.CustomerId);
            searchFilter.TotalRecords = query.Count();

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            switch (searchFilter.SortField)
            {
                default:
                    query = query.OrderByDescending(r => r.Date);
                    break;
            }

            return query.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<ServiceRequestActivityResult>();
        }

        public IEnumerable<ServiceRequestEntity> GetTabExistingList(ExistingSearchFilter searchFilter)
        {
            var query = _context.TB_T_SR.AsQueryable();
            query = query.Where(q => q.CUSTOMER_ID == searchFilter.CustomerId);

            // After Insert all filters >> Count It
            searchFilter.TotalRecords = query.Count();

            var results = from q in query
                          from ownerUser in _context.TB_R_USER.Where(user => q.OWNER_USER_ID == user.USER_ID).DefaultIfEmpty()
                          from delegateUser in _context.TB_R_USER.Where(user => q.DELEGATE_USER_ID == user.USER_ID).DefaultIfEmpty()
                          select new ServiceRequestEntity
                          {
                              SrId = q.SR_ID,
                              SrNo = q.SR_NO,
                              ThisAlert = q.RULE_THIS_ALERT,
                              NextSLA = q.RULE_NEXT_SLA,
                              TotalWorkingHours = q.RULE_TOTAL_WORK,
                              CustomerCardNo = q.TB_M_CUSTOMER.CARD_NO,
                              AccountNo = q.TB_M_ACCOUNT != null ? q.TB_M_ACCOUNT.ACCOUNT_NO : string.Empty,
                              ChannelId = q.CHANNEL_ID,
                              ChannelName = q.TB_R_CHANNEL != null ? q.TB_R_CHANNEL.CHANNEL_NAME : string.Empty,
                              ProductId = q.PRODUCT_ID,
                              ProductName = q.TB_R_PRODUCT != null ? q.TB_R_PRODUCT.PRODUCT_NAME : string.Empty,
                              CampaignServiceId = q.CAMPAIGNSERVICE_ID,
                              CampaignServiceName = q.TB_R_CAMPAIGNSERVICE != null ? q.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_NAME : string.Empty,
                              AreaId = q.AREA_ID,
                              AreaName = q.TB_M_AREA != null ? q.TB_M_AREA.AREA_NAME : string.Empty,
                              SubAreaId = q.SUBAREA_ID,
                              SubAreaName = q.TB_M_SUBAREA != null ? q.TB_M_SUBAREA.SUBAREA_NAME : string.Empty,
                              Subject = q.SR_SUBJECT,
                              SrStatusName = q.TB_C_SR_STATUS.SR_STATUS_NAME,
                              SRStateName = _context.TB_C_SR_STATE.Where(x => x.SR_STATE_ID == q.TB_C_SR_STATUS.SR_STATE_ID).FirstOrDefault().SR_STATE_NAME,
                              CreateDate = q.CREATE_DATE,
                              ClosedDate = q.CLOSE_DATE,
                              OwnerUserId = ownerUser != null ? ownerUser.USER_ID : (int?)null,
                              OwnerUserPosition = ownerUser != null ? ownerUser.POSITION_CODE : null,
                              OwnerUserFirstName = ownerUser != null ? ownerUser.FIRST_NAME : null,
                              OwnerUserLastName = ownerUser != null ? ownerUser.LAST_NAME : null,
                              DelegateUserId = delegateUser != null ? delegateUser.USER_ID : (int?)null,
                              DelegateUserPosition = delegateUser != null ? delegateUser.POSITION_CODE : null,
                              DelegateUserFirstName = delegateUser != null ? delegateUser.FIRST_NAME : null,
                              DelegateUserLastName = delegateUser != null ? delegateUser.LAST_NAME : null,
                              ANo = q.SR_ANO
                          };

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            switch (searchFilter.SortField)
            {
                case "ProductName":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC") ? results.OrderBy(q => q.ProductName) : results.OrderByDescending(q => q.ProductName);
                    break;
                case "AreaName":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(q => q.AreaName)
                        : results.OrderByDescending(q => q.AreaName);
                    break;
                case "SubAreaName":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(q => q.SubAreaName)
                        : results.OrderByDescending(q => q.SubAreaName);
                    break;
                case "SrStatus":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(q => q.SrStatusName)
                        : results.OrderByDescending(q => q.SrStatusName);
                    break;
                case "CreateDate":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(q => q.CreateDate)
                        : results.OrderByDescending(q => q.CreateDate);
                    break;
                case "CloseDate":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(q => q.ClosedDate)
                        : results.OrderByDescending(q => q.ClosedDate);
                    break;
                case "OwnerUserFirstName":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(q => q.OwnerUserFirstName)
                        : results.OrderByDescending(q => q.OwnerUserFirstName);
                    break;
                case "DelegateUserFirstName":
                    results = searchFilter.SortOrder.ToUpper().Equals("ASC")
                        ? results.OrderBy(q => q.DelegateUserFirstName)
                        : results.OrderByDescending(q => q.DelegateUserFirstName);
                    break;

                default:
                    results = results.OrderByDescending(q => q.ThisAlert);
                    break;
            }

            return results.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<ServiceRequestEntity>();
        }

        public List<SrVerifyGroupEntity> GetVerifyGroup(int srId)
        {
            return (from grp in _context.TB_T_SR_VERIFY_RESULT_GROUP
                    where grp.SR_ID == srId
                    orderby grp.SEQ_NO
                    select new SrVerifyGroupEntity
                    {
                        GroupId = grp.QUESTIONGROUP_ID,
                        GroupName = grp.QUESTIONGROUP_NAME,
                        RequirePass = grp.QUESTIONGROUP_PASS_AMOUNT ?? 0,
                        VerifyQuestions = (from q in grp.TB_T_SR_VERIFY_RESULT_QUESTION
                                           orderby q.SEQ_NO
                                           select new SrVerifyQuestionEntity
                                           {
                                               QuestionId = q.QUESTION_ID,
                                               QuestionName = q.QUESTION_NAME,
                                               Result = q.RESULT
                                           }).ToList(),
                    }).ToList();
        }

        public bool SaveSrAttachment(AttachmentEntity attach)
        {
            var isEdit = attach.SrAttachmentId.HasValue;
            TB_T_SR_ATTACHMENT dbAttach;

            if (!isEdit)
            {
                //add mode
                dbAttach = new TB_T_SR_ATTACHMENT();
            }
            else
            {
                //edit mode
                dbAttach = _context.TB_T_SR_ATTACHMENT.SingleOrDefault(d => d.SR_ATTACHMENT_ID == attach.SrAttachmentId);
                if (dbAttach == null)
                {
                    return false;
                }
            }

            dbAttach.SR_ATTACHMENT_DESC = attach.Description;
            dbAttach.UPDATE_USER = attach.CreateUserId;
            dbAttach.UPDATE_DATE = DateTime.Now;
            dbAttach.EXPIRY_DATE = attach.ExpiryDate;
            dbAttach.STATUS = attach.Status;

            if (!isEdit)
            {
                //add mode
                dbAttach.CUSTOMER_ID = attach.CustomerId;
                dbAttach.SR_ID = attach.SrId;
                dbAttach.SR_ACTIVITY_ID = attach.SrActivityId;
                dbAttach.SR_ATTACHMENT_FILE_NAME = attach.Filename;
                dbAttach.SR_ATTACHMENT_CONTENT_TYPE = attach.ContentType;
                dbAttach.SR_ATTACHMENT_URL = attach.Url;
                dbAttach.SR_ATTACHMENT_NAME = attach.Name;
                dbAttach.FILE_SIZE = attach.FileSize;
                dbAttach.CREATE_USER = attach.CreateUserId;
                dbAttach.CREATE_DATE = DateTime.Now;
                _context.TB_T_SR_ATTACHMENT.Add(dbAttach);
            }

            this.Save();

            //save attachment type
            if (attach.AttachTypeList != null && attach.AttachTypeList.Count > 0)
            {
                this.SaveAttachTypes(dbAttach.SR_ATTACHMENT_ID, attach.CreateUserId, attach.AttachTypeList, isEdit);
            }

            return true;
        }

        private void SaveAttachTypes(int attachmentId, int? userId, IEnumerable<AttachmentTypeEntity> attachTypes, bool isEdit)
        {
            if (isEdit)
            {
                //edit
                //delete old type
                var typeList = _context.TB_T_SR_ATTACHMENT_DOCUMENT_TYPE.Where(q => q.SR_ATTACHMENT_ID == attachmentId);
                foreach (var item in typeList)
                {
                    _context.TB_T_SR_ATTACHMENT_DOCUMENT_TYPE.Remove(item);
                }

                this.Save();
            }

            foreach (AttachmentTypeEntity item in attachTypes)
            {
                var dbSrAttachType = new TB_T_SR_ATTACHMENT_DOCUMENT_TYPE();
                dbSrAttachType.SR_ATTACHMENT_ID = attachmentId;
                dbSrAttachType.DOCUMENT_TYPE_ID = item.DocTypeId.Value;
                dbSrAttachType.CREATE_USER = userId.Value;
                dbSrAttachType.CREATE_DATE = DateTime.Now;
                _context.TB_T_SR_ATTACHMENT_DOCUMENT_TYPE.Add(dbSrAttachType);
            }

            this.Save();
        }

        public bool CheckDuplicateDocumentFilename(AttachmentEntity attach)
        {
            if (attach.SrId.HasValue)
            {
                return _context.TB_T_SR_ATTACHMENT.Any(i => i.SR_ID == attach.SrId && i.SR_ATTACHMENT_NAME == attach.Name);
            }
            else
            {
                return _context.TB_T_SR_ATTACHMENT.Any(i => i.SR_ACTIVITY_ID == attach.SrActivityId && i.SR_ATTACHMENT_NAME == attach.Name);
            }
        }

        public bool DeleteSrAttachment(int? srAttachId)
        {
            try
            {
                //delete document type
                //var typeList = _context.TB_T_SR_ATTACHMENT_DOCUMENT_TYPE.Where(t => t.SR_ATTACHMENT_ID == srAttachId);
                //foreach (var item in typeList)
                //{
                //    _context.TB_T_SR_ATTACHMENT_DOCUMENT_TYPE.Remove(item);
                //}

                //this.Save();

                //delete sr attachment
                var srAttachment =
                    _context.TB_T_SR_ATTACHMENT.SingleOrDefault(s => s.SR_ATTACHMENT_ID == srAttachId);
                if (srAttachment != null)
                {
                    //                    _context.TB_T_SR_ATTACHMENT.Remove(srAttachment);
                    srAttachment.STATUS = 0;
                    this.Save();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                return false;
            }
        }

        public AttachmentEntity GetSrAttachmentById(int attachmentId, string documentLevel)
        {
            try
            {
                if (documentLevel.ToUpper() == Constants.DocumentLevel.Sr.ToUpper())
                {
                    // SR Level
                    var query = _context.TB_T_SR_ATTACHMENT.SingleOrDefault(q => q.SR_ATTACHMENT_ID == attachmentId);

                    if (query != null)
                    {
                        return new AttachmentEntity
                        {
                            AttachmentId = query.SR_ATTACHMENT_ID,
                            Url = query.SR_ATTACHMENT_URL,
                            Filename = query.SR_ATTACHMENT_FILE_NAME,
                            Name = query.SR_ATTACHMENT_NAME,
                            Description = query.SR_ATTACHMENT_DESC,
                            ExpiryDate = query.EXPIRY_DATE,
                            ContentType = query.SR_ATTACHMENT_CONTENT_TYPE,
                            Status = query.STATUS,
                            DocumentLevel = Constants.DocumentLevel.Sr,
                            AttachTypeList = query.TB_T_SR_ATTACHMENT_DOCUMENT_TYPE.Select(x => new AttachmentTypeEntity
                            {
                                AttachmentId = x.SR_ATTACHMENT_ID,
                                DocTypeId = x.DOCUMENT_TYPE_ID,
                                Name = x.TB_M_DOCUMENT_TYPE.DOCUMENT_TYPE_NAME
                            }).ToList()
                        };
                    }
                }

                if (documentLevel.ToUpper() == Constants.DocumentLevel.Customer.ToUpper())
                {
                    // SR Level
                    var query = _context.TB_M_CUSTOMER_ATTACHMENT.SingleOrDefault(q => q.CUSTOMER_ATTACHMENT_ID == attachmentId);

                    if (query != null)
                    {
                        return new AttachmentEntity
                        {
                            AttachmentId = query.CUSTOMER_ATTACHMENT_ID,
                            Url = query.URL,
                            Filename = query.FILE_NAME,
                            Name = query.ATTACHMENT_NAME,
                            Description = query.ATTACHMENT_DESC,
                            ExpiryDate = query.EXPIRY_DATE,
                            ContentType = query.CONTENT_TYPE,
                            Status = query.STATUS,
                            DocumentLevel = Constants.DocumentLevel.Customer,
                            AttachTypeList = query.TB_T_ATTACHMENT_TYPE
                                                .Select(x => new AttachmentTypeEntity
                                                {
                                                    AttachmentId = x.CUSTOMER_ATTACHMENT_ID,
                                                    DocTypeId = x.DOCUMENT_TYPE_ID,
                                                    Name = x.TB_M_DOCUMENT_TYPE.DOCUMENT_TYPE_NAME
                                                }).ToList()
                        };
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                return null;
            }
        }

        public void CreateSrAttachment(List<SrAttachmentEntity> attachments, int srId, int srActivityId, int? customerId, int? createUserId, string createUsername)
        {
            var isEdit = false;

            foreach (var srAttachment in attachments)
            {
                var attach = new TB_T_SR_ATTACHMENT();
                if (srAttachment.IsEditable)
                {
                    attach.SR_ID = srId;
                    attach.SR_ACTIVITY_ID = srActivityId;
                    attach.SR_ATTACHMENT_URL = srAttachment.Url;
                    attach.SR_ATTACHMENT_FILE_NAME = srAttachment.Filename;
                    attach.SR_ATTACHMENT_NAME = srAttachment.Name;
                    attach.SR_ATTACHMENT_DESC = srAttachment.DocDesc;
                    attach.SR_ATTACHMENT_CONTENT_TYPE = srAttachment.ContentType;
                    attach.FILE_SIZE = srAttachment.FileSize;
                    attach.STATUS = srAttachment.Status;

                    if (!string.IsNullOrEmpty(srAttachment.ExpiryDate))
                        attach.EXPIRY_DATE = srAttachment.ExpiryDate.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate);

                    attach.CREATE_USER = ConvertStringIdToInt32OrNull(srAttachment.CreateUserId);
                    attach.CREATE_USERNAME = srAttachment.CreateUserName;
                    attach.UPDATE_USER = createUserId;

                    attach.CREATE_DATE = srAttachment.CreateDateAsDateTime ?? DateTime.Now;
                    attach.UPDATE_DATE = attach.CREATE_DATE;
                    attach.CUSTOMER_ID = customerId;
                    _context.TB_T_SR_ATTACHMENT.Add(attach);
                    Save();

                    //save attachment type
                    if (srAttachment.DocumentType != null && srAttachment.DocumentType.Count > 0)
                    {
                        this.SaveAttachTypes(attach.SR_ATTACHMENT_ID, createUserId, srAttachment.DocumentType, isEdit);
                    }
                }
            }

            var srAttachmentNoCustomers = _context.TB_T_SR_ATTACHMENT.Where(x => x.SR_ID == srId && x.CUSTOMER_ID == null).ToList();
            if (srAttachmentNoCustomers.Count > 0)
            {
                foreach (var item in srAttachmentNoCustomers)
                {
                    item.CUSTOMER_ID = customerId;
                }
                Save();
            }
        }

        private int? ConvertStringIdToInt32OrNull(string src)
        {
            if (!string.IsNullOrEmpty(src))
            {
                try
                {
                    var result = Convert.ToInt32(src);
                    if (result == 0)
                        return null;
                    else
                        return result;
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception occur:\n", ex);
                    return null;
                }
            }

            return null;
        }

        public IEnumerable<ServiceDocumentEntity> GetActivityDocumentList(int srId)
        {
            var query = _context.TB_T_SR_ATTACHMENT.AsQueryable();
            query = query.Where(q => q.SR_ID == srId);

            var results = query.Select(q => new ServiceDocumentEntity
            {
                SrAttachId = q.SR_ATTACHMENT_ID,
                SrId = q.SR_ID,
                SrActivityId = q.SR_ACTIVITY_ID,
                AttachmentFilename = q.SR_ATTACHMENT_FILE_NAME,
                AttachmentName = q.SR_ATTACHMENT_NAME,
                ContentType = q.SR_ATTACHMENT_CONTENT_TYPE,
                Url = q.SR_ATTACHMENT_URL,
                AttachmentDesc = q.SR_ATTACHMENT_DESC,
                CreateDate = q.CREATE_DATE,
                ExpireDate = q.EXPIRY_DATE,
                DocumentTypes = q.TB_T_SR_ATTACHMENT_DOCUMENT_TYPE.Select(item => new DocumentTypeEntity
                {
                    DocTypeId = item.TB_M_DOCUMENT_TYPE.DOCUMENT_TYPE_ID,
                    Name = item.TB_M_DOCUMENT_TYPE.DOCUMENT_TYPE_NAME
                }).ToList(),
                FileSize = q.FILE_SIZE,
                Status = q.STATUS,
                CreateUserId = q.CREATE_USER
            });

            return results;
        }

        public bool CheckDuplicateContact(ContactEntity contactEntity)
        {
            var firstnameTh = contactEntity.FirstNameThai ?? "";
            var lastnameTh = contactEntity.LastNameThai ?? "";
            var firstnameEn = contactEntity.FirstNameEnglish ?? "";
            var lastnameEn = contactEntity.LastNameEnglish ?? "";
            var phones = contactEntity.PhoneList.Where(x => !string.IsNullOrEmpty(x.PhoneNo)).Select(x => x.PhoneNo).ToList();

            if (!string.IsNullOrEmpty(firstnameTh))
            {
                return _context.TB_M_CONTACT.Any(x =>
                    (x.FIRST_NAME_TH ?? "") == firstnameTh
                    && (x.LAST_NAME_TH ?? "") == lastnameTh
                    && x.TB_M_CONTACT_PHONE.Any(p => phones.Contains(p.PHONE_NO)));
            }

            if (!string.IsNullOrEmpty(firstnameEn))
            {
                return _context.TB_M_CONTACT.Any(x =>
                    (x.FIRST_NAME_EN ?? "") == firstnameEn
                    && (x.LAST_NAME_EN ?? "") == lastnameEn
                    && x.TB_M_CONTACT_PHONE.Any(p => phones.Contains(p.PHONE_NO)));
            }

            return false;
        }

        public bool CheckDuplicatePhoneNo(List<string> lstPhoneNo)
        {
            return _context.TB_M_PHONE.Any(q => q.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax && lstPhoneNo.Contains(q.PHONE_NO));
        }

        public List<ServiceRequestCustomerSearchResult> GetCustomerAccountByPhoneNo(List<string> lstPhoneNo)
        {
            return _context.TB_M_ACCOUNT
                .Where(x => x.TB_M_CUSTOMER.TB_M_PHONE.Any(
                        phone => phone.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax && lstPhoneNo.Contains(phone.PHONE_NO)
                    )).OrderBy(x => x.TB_M_CUSTOMER.FIRST_NAME_TH).Take(100).AsEnumerable()
                .Select(q => new ServiceRequestCustomerSearchResult
                {
                    CustomerId = q.CUSTOMER_ID ?? 0,
                    AccountId = q.ACCOUNT_ID,
                    CardNo = q.TB_M_CUSTOMER.CARD_NO,
                    CustomerFirstNameTh = q.TB_M_CUSTOMER.FIRST_NAME_TH,
                    CustomerLastNameTh = q.TB_M_CUSTOMER.LAST_NAME_TH,
                    CustomerFirstNameEn = q.TB_M_CUSTOMER.FIRST_NAME_EN,
                    CustomerLastNameEn = q.TB_M_CUSTOMER.LAST_NAME_EN,
                    AccountNo = q.ACCOUNT_NO,
                    //PhoneNo = q.TB_M_CUSTOMER.TB_M_PHONE.Any() ? q.TB_M_CUSTOMER.TB_M_PHONE.FirstOrDefault().PHONE_NO : string.Empty,
                    PhoneNo = q.TB_M_CUSTOMER.TB_M_PHONE.Any() ? (string.Join(",", q.TB_M_CUSTOMER.TB_M_PHONE.Select(x => x.PHONE_NO))) : string.Empty,
                    CarNo = q.CAR_NO,
                    AccountStatus = q.STATUS,
                    ProductName = q.SUBSCRIPTION_DESC,
                    BranchName = q.BRANCH_NAME,
                    CustomerType = string.Empty,
                    SubscriptionTypeName = q.TB_M_CUSTOMER.TB_M_SUBSCRIPT_TYPE != null ? q.TB_M_CUSTOMER.TB_M_SUBSCRIPT_TYPE.SUBSCRIPT_TYPE_NAME : string.Empty
                }).ToList();
        }

        //public List<ServiceRequestCustomerSearchResult> GetCustomerAccountByPhoneNo(List<string> lstPhoneNo)
        //{
        //    string strPhoneNo1 = (lstPhoneNo.Count > 0) ? lstPhoneNo[0] : "";
        //    string strPhoneNo2 = (lstPhoneNo.Count > 1) ? lstPhoneNo[1] : "";
        //    string strPhoneNo3 = (lstPhoneNo.Count > 2) ? lstPhoneNo[2] : "";

        //    // Call SP_GET_CUSTOMER_BY_PHONE
        //    var lstCustomer = _context.SP_GET_CUSTOMER_BY_PHONE(null, strPhoneNo1, strPhoneNo2, strPhoneNo3);

        //    var result = (from c in lstCustomer
        //                  select new ServiceRequestCustomerSearchResult
        //                  {
        //                      CustomerId = c.CUSTOMER_ID,
        //                      AccountId = c.ACCOUNT_ID,
        //                      CardNo = c.CARD_NO,
        //                      CustomerFirstNameTh = c.FIRST_NAME_TH,
        //                      CustomerLastNameTh = c.LAST_NAME_TH,
        //                      CustomerFirstNameEn = c.FIRST_NAME_EN,
        //                      CustomerLastNameEn = c.LAST_NAME_EN,
        //                      AccountNo = c.ACCOUNT_NO,
        //                      //PhoneNo = c.
        //                      CarNo = c.CAR_NO,
        //                      AccountStatus = c.STATUS,
        //                      //ProductName = q.SUBSCRIPTION_DESC,
        //                      BranchName = c.BRANCH_NAME,
        //                      //CustomerType = c.TYPE,
        //                      SubscriptionTypeName = c.SUBSCRIPT_TYPE_NAME
        //                  }).AsQueryable();

        //    //return result.ToList();
        //    return result.Take(100).ToList();
        //}

        public List<ServiceRequestCustomerSearchResult> GetCustomerByName(string customerTHName)
        {
            var result = from c in _context.TB_M_CUSTOMER
                         from st in _context.TB_M_SUBSCRIPT_TYPE.Where(x => x.SUBSCRIPT_TYPE_ID == c.SUBSCRIPT_TYPE_ID).DefaultIfEmpty()
                         from ac in _context.TB_M_ACCOUNT.Where(x => x.CUSTOMER_ID == c.CUSTOMER_ID).DefaultIfEmpty()
                         where c.FIRST_NAME_TH == customerTHName
                         select new ServiceRequestCustomerSearchResult
                         {
                             CustomerId = c.CUSTOMER_ID,
                             AccountId = ac.ACCOUNT_ID,
                             CardNo = c.CARD_NO,
                             CustomerFirstNameTh = c.FIRST_NAME_TH,
                             CustomerLastNameTh = c.LAST_NAME_TH,
                             CustomerFirstNameEn = c.FIRST_NAME_EN,
                             CustomerLastNameEn = c.LAST_NAME_EN,
                             AccountNo = ac.ACCOUNT_NO,
                             PhoneNo = c.TB_M_PHONE.Any() ? c.TB_M_PHONE.FirstOrDefault().PHONE_NO : string.Empty,
                             CarNo = ac.CAR_NO,
                             AccountStatus = ac.STATUS,
                             ProductName = ac.SUBSCRIPTION_DESC,
                             BranchName = ac.BRANCH_NAME,
                             CustomerType = string.Empty,
                             SubscriptionTypeName = c.TB_M_SUBSCRIPT_TYPE.SUBSCRIPT_TYPE_NAME
                         };

            return result.Take(100).ToList();
        }

        public int GetServiceAttachTotalFileSize(int srId)
        {
            var totalSize = 0;
            var srAttachmentList = _context.TB_T_SR_ATTACHMENT.Where(q => q.SR_ID == srId && q.STATUS != 0);
            foreach (var item in srAttachmentList)
            {
                totalSize = totalSize + (item.FILE_SIZE.HasValue ? item.FILE_SIZE.Value : 0);
            }

            return totalSize;
        }

        public ServiceRequestContactSearchResult GetCustomerContactById(int contactId)
        {
            var query = _context.TB_M_CUSTOMER_CONTACT.SingleOrDefault(q => q.CONTACT_ID == contactId);
            var resultQuery = new ServiceRequestContactSearchResult
            {
                ContactId = query.CONTACT_ID,
                AccountNo = query.TB_M_ACCOUNT.ACCOUNT_NO,
                CardNo = query.TB_M_CONTACT.CARD_NO,
                FirstNameTh = query.TB_M_CONTACT.FIRST_NAME_TH,
                LastNameTh = query.TB_M_CONTACT.LAST_NAME_TH,
                PhoneNo = query.TB_M_CONTACT.TB_M_CONTACT_PHONE.Any() ? query.TB_M_CONTACT.TB_M_CONTACT_PHONE.FirstOrDefault().PHONE_NO : string.Empty,
                RelationshipId = query.RELATIONSHIP_ID ?? 0,
                RelationName = query.TB_M_RELATIONSHIP != null ? query.TB_M_RELATIONSHIP.RELATIONSHIP_NAME : "",
                SubscriptionType = query.TB_M_CONTACT.TB_M_SUBSCRIPT_TYPE != null ? query.TB_M_CONTACT.TB_M_SUBSCRIPT_TYPE.SUBSCRIPT_TYPE_NAME : string.Empty,
                BirthDay = query.TB_M_CONTACT.BIRTH_DATE,
                TitleTh = query.TB_M_CONTACT.TB_M_TITLE != null ? query.TB_M_CONTACT.TB_M_TITLE.TITLE_NAME : string.Empty, //TitleTh = q.TB_M_CONTACT.TB_M_TITLE.TITLE_TH,
                TitleEn = query.TB_M_CONTACT.TB_M_TITLE != null ? query.TB_M_CONTACT.TB_M_TITLE1.TITLE_NAME : string.Empty, //TitleEn = q.TB_M_CONTACT.TB_M_TITLE.TITLE_EN,
                FirstNameEn = query.TB_M_CONTACT.FIRST_NAME_EN,
                LastNameEn = query.TB_M_CONTACT.LAST_NAME_EN,
                Email = query.TB_M_CONTACT.EMAIL,
                IsDefault = query.TB_M_CONTACT.IS_DEFAULT,
                CustomerPhones = query.TB_M_CONTACT.TB_M_CONTACT_PHONE.Select(item => new ServiceRequestCustomerAccountPhone
                {
                    PhoneNo = item.PHONE_NO,
                    PhoneTypeCode = item.TB_M_PHONE_TYPE.PHONE_TYPE_CODE,
                    PhoneTypeName = item.TB_M_PHONE_TYPE.PHONE_TYPE_NAME
                }).ToList()
            };
            return resultQuery;
        }

        public List<UserEntity> AutoCompleteSearchUser(string keyword, List<int> userIds, int branchId, int limit)
        {
            return (from user in _context.TB_R_USER
                    where
                        userIds.Contains(user.USER_ID)
                            && user.BRANCH_ID == branchId
                            && (string.IsNullOrEmpty(keyword) || user.FIRST_NAME.Contains(keyword) || user.LAST_NAME.Contains(keyword))
                    select new UserEntity
                    {
                        UserId = user.USER_ID,
                        Username = user.USERNAME,
                        Firstname = user.FIRST_NAME,
                        Lastname = user.LAST_NAME,
                        PositionCode = user.POSITION_CODE
                    }).Take(limit).ToList();
        }

        public List<int> GetSrPageIdsByRoleCode(string roleCode)
        {
            roleCode = roleCode.ToUpper();
            return _context.TB_C_ROLE_SR_PAGE.Where(r => r.TB_C_ROLE.ROLE_CODE.ToUpper() == roleCode).Select(r => r.SR_PAGE_ID).ToList();
        }

        /// <summary>
        /// When CSM cannot connect to CARService, it will use the local database instead
        /// </summary>
        /// <returns>List of ServiceRequestActivityResult</returns>
        public IEnumerable<ServiceRequestActivityResult> GetSRActivityList(ActivitySearchFilter searchFilter)
        {
            DateTime dateTo = searchFilter.ActivityEndDateTimeValue.AddDays(1);
            var query = GetSrActivityQueryable(false, 0, searchFilter.CustomerId.Value);

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = query.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            return query.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<ServiceRequestActivityResult>();
        }

        public ServiceRequestForInsertLogEntity GetServiceRequestActivityForInsertLog(int srActivityId)
        {
            var result = _context.SP_GET_SR_ACTIVITY(srActivityId).FirstOrDefault();

            if (result == null)
                return null;

            var activity = new ServiceRequestForInsertLogEntity
            {
                SrId = result.SR_ID,
                SrActivityId = result.SR_ACTIVITY_ID,
                SrNo = result.SR_NO,
                CallId = result.SR_CALL_ID,
                ANo = result.SR_ANO,
                CustomerId = result.CUSTOMER_ID ?? 0,
                CustomerSubscriptionTypeId = result.CUSTOMER_SUBSCRIPT_TYPE_ID ?? 0,
                CustomerSubscriptionTypeName = result.CUSTOMER_SUBSCRIPT_TYPE_NAME,
                CustomerSubscriptionTypeCode = result.CUSTOMER_SUBSCRIPT_TYPE_CODE,
                CustomerCardNo = result.CUSTOMER_CARD_NO,
                CustomerBirthDate = result.CUSTOMER_BIRTH_DATE,
                CustomerTitleTh = result.CUSTOMER_TITLE_NAME_TH,
                CustomerFirstNameTh = result.CUSTOMER_FIRST_NAME_TH,
                CustomerLastNameTh = result.CUSTOMER_LAST_NAME_TH,
                CustomerTitleEn = result.CUSTOMER_TITLE_NAME_EN,
                CustomerFirstNameEn = result.CUSTOMER_FIRST_NAME_EN,
                CustomerLastNameEn = result.CUSTOMER_LAST_NAME_EN,
                CustomerEmail = result.CUSTOMER_EMAIL,
                CustomerEmployeeCode = result.CUSTOMER_EMPLOYEE_CODE,
                KKCISID = result.CUSTOMER_KKCIS_ID,

                AccountId = result.ACCOUNT_ID ?? 0,
                AccountNo = result.ACCOUNT_NO,
                AccountStatus = result.ACCOUNT_STATUS,
                AccountCarNo = result.ACCOUNT_CAR_NO,
                AccountProductGroup = result.ACCOUNT_PRODUCT_GROUP,
                AccountProduct = result.ACCOUNT_PRODUCT,
                AccountBranchName = result.ACCOUNT_BRANCH_NAME,

                ContactId = result.CONTACT_ID ?? 0,
                ContactSubscriptionTypeName = result.CONTACT_SUBSCRIPT_TYPE_NAME,
                ContactCardNo = result.CONTACT_CARD_NO,
                ContactBirthDate = result.CONTACT_BIRTH_DATE,
                ContactTitleTh = result.CONTACT_TITLE_NAME_TH,
                ContactFirstNameTh = result.CONTACT_FIRST_NAME_TH,
                ContactLastNameTh = result.CONTACT_LAST_NAME_TH,
                ContactTitleEn = result.CONTACT_TITLE_NAME_EN,
                ContactFirstNameEn = result.CONTACT_FIRST_NAME_EN,
                ContactLastNameEn = result.CONTACT_LAST_NAME_EN,
                ContactEmail = result.CONTACT_EMAIL,

                ContactAccountNo = result.CONTACT_ACCOUNT_NO,
                ContactRelationshipName = result.CONTACT_RELATIONSHIP_NAME,

                ProductGroupCode = result.PRODUCTGROUP_CODE,
                ProductGroupName = result.PRODUCTGROUP_NAME,
                ProductCode = result.PRODUCT_CODE,
                ProductName = result.PRODUCT_NAME,
                CampaignServiceCode = result.CAMPAIGNSERVICE_CODE,
                CampaignServiceName = result.CAMPAIGNSERVICE_NAME,
                AreaId = result.AREA_ID ?? 0,
                AreaCode = result.AREA_CODE ?? 0m,
                AreaName = result.AREA_NAME,
                SubAreaId = result.SUBAREA_ID ?? 0,
                SubAreaCode = result.SUBAREA_CODE ?? 0m,
                SubAreaName = result.SUBAREA_NAME,
                TypeId = result.TYPE_ID ?? 0,
                TypeCode = result.TYPE_CODE ?? 0m,
                TypeName = result.TYPE_NAME,
                ChannelCode = result.CHANNEL_CODE,
                ChannelName = result.CHANNEL_NAME,
                MediaSourceName = result.MEDIA_SOURCE_NAME,
                Subject = result.SR_SUBJECT,
                Remark = result.SR_REMARK,
                Verify = result.SR_IS_VERIFY,
                VerifyResult = result.SR_IS_VERIFY_PASS,
                SrPageId = result.SR_PAGE_ID ?? 0,

                // Officer
                SRCreatorBranchName = result.CREATE_BRANCH_NAME,
                SRCreatorUserFirstName = result.CREATE_FIRST_NAME,
                SRCreatorUserLastName = result.CREATE_LAST_NAME,
                SRCreatorUserPositionCode = result.CREATE_POSITION_CODE,

                OwnerBranchName = result.OWNER_BRANCH_NAME,
                OwnerUserFirstName = result.OWNER_FIRST_NAME,
                OwnerUserLastName = result.OWNER_LAST_NAME,
                OwnerUserPositionCode = result.OWNER_POSITION_CODE,

                DelegateBranchName = result.DELEGATE_BRANCH_NAME,
                DelegateUserFirstName = result.DELEGATE_FIRST_NAME,
                DelegateUserLastName = result.DELEGATE_LAST_NAME,
                DelegateUserPositionCode = result.DELEGATE_POSITION_CODE,

                SendEmail = result.SR_EMAIL_TEMPLATE_ID,
                EmailSender = result.SR_ACTIVITY_EMAIL_SENDER,
                EmailTo = result.SR_ACTIVITY_EMAIL_TO,
                EmailCc = result.SR_ACTIVITY_EMAIL_CC,
                EmailBcc = result.SR_ACTIVITY_EMAIL_BCC,
                EmailSubject = result.SR_ACTIVITY_EMAIL_SUBJECT,
                EmailBody = result.SR_ACTIVITY_EMAIL_BODY,
                EmailAttachments = result.SR_ACTIVITY_EMAIL_ATTACHMENTS,

                ActivityDescription = result.SR_ACTIVITY_DESC,
                ActivityTypeId = result.SR_ACTIVITY_TYPE_ID ?? 0,
                ActivityTypeName = result.SR_ACTIVITY_TYPE_NAME,

                SRStateName = result.SR_STATE_NAME,
                SRStatusName = result.SR_STATUS_NAME,

                AddressHouseNo = result.SR_DEF_ADDRESS_HOUSE_NO,
                AddressVillage = result.SR_DEF_ADDRESS_VILLAGE,
                AddressBuilding = result.SR_DEF_ADDRESS_BUILDING,
                AddressFloorNo = result.SR_DEF_ADDRESS_FLOOR_NO,
                AddressRoomNo = result.SR_DEF_ADDRESS_ROOM_NO,
                AddressMoo = result.SR_DEF_ADDRESS_MOO,
                AddressSoi = result.SR_DEF_ADDRESS_SOI,
                AddressStreet = result.SR_DEF_ADDRESS_STREET,
                AddressTambol = result.SR_DEF_ADDRESS_TAMBOL,
                AddressAmphur = result.SR_DEF_ADDRESS_AMPHUR,
                AddressProvince = result.SR_DEF_ADDRESS_PROVINCE,
                AddressZipCode = result.SR_DEF_ADDRESS_ZIPCODE,

                AFSAssetNo = result.SR_AFS_ASSET_NO,
                AFSAssetDesc = result.SR_AFS_ASSET_DESC,

                NCBCustomerBirthDate = result.SR_NCB_CUSTOMER_BIRTHDATE,
                NCBCheckStatus = result.SR_NCB_CHECK_STATUS,
                NCBMarketingFullName = result.SR_NCB_MARKETING_FULL_NAME,
                NCBMarketingBranchName = result.SR_NCB_MARKETING_BRANCH_NAME,
                NCBMarketingBranchUpper1Name = result.SR_NCB_MARKETING_BRANCH_UPPER_1_NAME,
                NCBMarketingBranchUpper2Name = result.SR_NCB_MARKETING_BRANCH_UPPER_2_NAME,

                ActivityCreateDate = result.ACTIVITY_CREATE_DATE,
                ActivityCreateUserFirstName = result.ACTIVITY_CREATE_FIRST_NAME,
                ActivityCreateUserLastName = result.ACTIVITY_CREATE_LAST_NAME,
                ActivityCreateUserPositionCode = result.ACTIVITY_CREATE_POSITION_CODE,
                ActivityCreateUserEmployeeCode = result.ACTIVITY_CREATE_EMPLOYEE_CODE,

                MappingProductId = result.MAP_PRODUCT_ID ?? 0,

                HPSubject = result.HP_SUBJECT,
                HPLanguageIndependentCode = result.HP_LANGUAGE_INDEPENDENT_CODE,

                CPN_ProductGroupName = result.CPN_PRODUCTGROUP_NAME,
                CPN_ProductName = result.CPN_PRODUCT_NAME,
                CPN_CampaignName = result.CPN_CAMPAIGNSERVICE_NAME,
                CPN_SubjectName = result.CPN_SUBJECT_NAME,
                CPN_TypeName = result.CPN_TYPE_NAME,
                CPN_RootCauseName = result.CPN_ROOTCAUSE_NAME,
                CPN_IssueName = result.CPN_ISSUES_NAME,
                CPN_IsSecret = result.CPN_SECRET ?? false,
                CPN_NotSend_CAR = result.CPN_CAR ?? false,
                CPN_NotSend_HPLog = result.CPN_HPLog100 ?? false,
                CPN_BUGroupName = result.CPN_BUGROUP_NAME,
                CPN_IsSummary = result.CPN_SUMMARY,
                CPN_CauseCustomer = result.CPN_CAUSE_CUSTOMER ?? false,
                CPN_CauseCustomerDetail = result.CPN_CAUSE_CUSTOMER_DETAIL,
                CPN_CauseStaff = result.CPN_CAUSE_STAFF ?? false,
                CPN_CauseStaffDetail = result.CPN_CAUSE_STAFF_DETAIL,
                CPN_CauseSystem = result.CPN_CAUSE_SYSTEM ?? false,
                CPN_CauseSystemDetail = result.CPN_CAUSE_SYSTEM_DETAIL,
                CPN_CauseProcess = result.CPN_CAUSE_PROCESS ?? false,
                CPN_CauseProcessDetail = result.CPN_CAUSE_PROCESS_DETAIL,
                CPN_CauseSummaryName = result.CPN_CAUSE_NAME,
                CPN_SummaryName = result.CPN_SUMMARY_NAME,
                CPN_Fixed_Detail = result.CPN_FIXED_DETAIL,
                CPN_BU1Desc = result.CPN_BU1DESC,
                CPN_BU2Desc = result.CPN_BU2DESC,
                CPN_BU3Desc = result.CPN_BU3DESC,
                CPN_BUBranchName = result.BRANCH_NAME
            };

            #region "Comment out old query"

            //var q = from act in _context.TB_T_SR_ACTIVITY
            //        from actStatus in _context.TB_C_SR_STATUS.Where(x => x.SR_STATUS_ID == act.SR_STATUS_ID).DefaultIfEmpty()
            //        from actCreator in _context.TB_R_USER.Where(x => x.USER_ID == act.CREATE_USER).DefaultIfEmpty()
            //        from actType in _context.TB_C_SR_ACTIVITY_TYPE.Where(x => x.SR_ACTIVITY_TYPE_ID == act.SR_ACTIVITY_TYPE_ID).DefaultIfEmpty()
            //        from sr in _context.TB_T_SR.Where(x => x.SR_ID == act.SR_ID).DefaultIfEmpty()
            //        from customer in _context.TB_M_CUSTOMER.Where(x => x.CUSTOMER_ID == sr.CUSTOMER_ID).DefaultIfEmpty()
            //        from customerSubscriptionType in _context.TB_M_SUBSCRIPT_TYPE.Where(x => x.SUBSCRIPT_TYPE_ID == customer.SUBSCRIPT_TYPE_ID).DefaultIfEmpty()
            //        from customerTitleTh in _context.TB_M_TITLE.Where(x => x.TITLE_ID == customer.TITLE_TH_ID).DefaultIfEmpty()
            //        from customerTitleEn in _context.TB_M_TITLE.Where(x => x.TITLE_ID == customer.TITLE_EN_ID).DefaultIfEmpty()
            //        from customerEmployee in _context.TB_R_USER.Where(x => x.USER_ID == customer.EMPLOYEE_ID).DefaultIfEmpty()
            //        from account in _context.TB_M_ACCOUNT.Where(x => x.ACCOUNT_ID == sr.ACCOUNT_ID).DefaultIfEmpty()
            //        from contact in _context.TB_M_CONTACT.Where(x => x.CONTACT_ID == sr.CONTACT_ID).DefaultIfEmpty()
            //        from contactSubscriptionType in _context.TB_M_SUBSCRIPT_TYPE.Where(x => x.SUBSCRIPT_TYPE_ID == contact.SUBSCRIPT_TYPE_ID).DefaultIfEmpty()
            //        from contactTitleTh in _context.TB_M_TITLE.Where(x => x.TITLE_ID == contact.TITLE_TH_ID).DefaultIfEmpty()
            //        from contactTitleEn in _context.TB_M_TITLE.Where(x => x.TITLE_ID == contact.TITLE_EN_ID).DefaultIfEmpty()
            //        from contactRelationship in _context.TB_M_RELATIONSHIP.Where(x => x.RELATIONSHIP_ID == sr.CONTACT_RELATIONSHIP_ID).DefaultIfEmpty()
            //        from creator in _context.TB_R_USER.Where(x => x.USER_ID == sr.CREATE_USER).DefaultIfEmpty()
            //        from creatorBranch in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == sr.CREATE_BRANCH_ID).DefaultIfEmpty()
            //        from ownerUser in _context.TB_R_USER.Where(x => x.USER_ID == sr.OWNER_USER_ID).DefaultIfEmpty()
            //        from ownerBranch in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == sr.OWNER_BRANCH_ID).DefaultIfEmpty()
            //        from delegateUser in _context.TB_R_USER.Where(x => x.USER_ID == sr.DELEGATE_USER_ID).DefaultIfEmpty()
            //        from delegateBranch in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == sr.DELEGATE_BRANCH_ID).DefaultIfEmpty()
            //        from mapping in _context.TB_M_MAP_PRODUCT.Where(x => x.MAP_PRODUCT_ID == sr.MAP_PRODUCT_ID).DefaultIfEmpty()
            //        from productGroup in _context.TB_R_PRODUCTGROUP.Where(x => x.PRODUCTGROUP_ID == sr.PRODUCTGROUP_ID).DefaultIfEmpty()
            //        from product in _context.TB_R_PRODUCT.Where(x => x.PRODUCT_ID == sr.PRODUCT_ID).DefaultIfEmpty()
            //        from campaignService in _context.TB_R_CAMPAIGNSERVICE.Where(x => x.CAMPAIGNSERVICE_ID == sr.CAMPAIGNSERVICE_ID).DefaultIfEmpty()
            //        from area in _context.TB_M_AREA.Where(x => x.AREA_ID == sr.AREA_ID).DefaultIfEmpty()
            //        from subArea in _context.TB_M_SUBAREA.Where(x => x.SUBAREA_ID == sr.SUBAREA_ID).DefaultIfEmpty()
            //        from type in _context.TB_M_TYPE.Where(x => x.TYPE_ID == sr.TYPE_ID).DefaultIfEmpty()
            //        from channel in _context.TB_R_CHANNEL.Where(x => x.CHANNEL_ID == sr.CHANNEL_ID).DefaultIfEmpty()
            //        from mediaSource in _context.TB_M_MEDIA_SOURCE.Where(x => x.MEDIA_SOURCE_ID == sr.MEDIA_SOURCE_ID).DefaultIfEmpty()
            //        where act.SR_ACTIVITY_ID == srActivityId
            //        select new ServiceRequestForInsertLogEntity()
            //        {
            //            SrId = sr == null ? 0 : sr.SR_ID,
            //            SrActivityId = sr == null ? 0 : act.SR_ACTIVITY_ID,
            //            SrNo = sr == null ? "" : sr.SR_NO,
            //            CallId = sr == null ? "" : sr.SR_CALL_ID,
            //            ANo = sr == null ? "" : sr.SR_ANO,
            //            CustomerId = sr == null ? 0 : (sr.CUSTOMER_ID ?? 0),
            //            CustomerSubscriptionTypeId = customerSubscriptionType == null ? 0 : customerSubscriptionType.SUBSCRIPT_TYPE_ID,
            //            CustomerSubscriptionTypeName = customerSubscriptionType == null ? "" : customerSubscriptionType.SUBSCRIPT_TYPE_NAME,
            //            CustomerSubscriptionTypeCode = customerSubscriptionType == null ? "" : customerSubscriptionType.SUBSCRIPT_TYPE_CODE,
            //            CustomerCardNo = customer == null ? "" : customer.CARD_NO,
            //            CustomerBirthDate = customer == null ? null : customer.BIRTH_DATE,
            //            CustomerTitleTh = customerTitleTh == null ? "" : customerTitleTh.TITLE_NAME,
            //            CustomerFirstNameTh = customer == null ? "" : customer.FIRST_NAME_TH,
            //            CustomerLastNameTh = customer == null ? "" : customer.LAST_NAME_TH,
            //            CustomerTitleEn = customerTitleEn == null ? "" : customerTitleEn.TITLE_NAME,
            //            CustomerFirstNameEn = customer == null ? "" : customer.FIRST_NAME_EN,
            //            CustomerLastNameEn = customer == null ? "" : customer.LAST_NAME_EN,
            //            CustomerEmail = customer == null ? "" : customer.EMAIL,
            //            CustomerEmployeeCode = customerEmployee == null ? "" : customerEmployee.EMPLOYEE_CODE,
            //            KKCISID = customerEmployee == null ? "" : customer.KKCIS_ID + "",

            //            AccountNo = account == null ? "" : account.ACCOUNT_NO,
            //            AccountStatus = account == null ? "" : account.STATUS,
            //            AccountCarNo = account == null ? "" : account.CAR_NO,
            //            AccountProductGroup = account == null ? "" : account.PRODUCT_GROUP,
            //            AccountProduct = account == null ? "" : account.PRODUCT,
            //            AccountBranchName = account == null ? "" : account.BRANCH_NAME,

            //            ContactId = contact == null ? 0 : (sr.CONTACT_ID ?? 0),
            //            ContactSubscriptionTypeName = contactSubscriptionType == null ? "" : contactSubscriptionType.SUBSCRIPT_TYPE_NAME,
            //            ContactCardNo = contact == null ? "" : contact.CARD_NO,
            //            ContactBirthDate = contact == null ? null : contact.BIRTH_DATE,
            //            ContactTitleTh = contactTitleTh == null ? "" : contactTitleTh.TITLE_NAME,
            //            ContactFirstNameTh = contact == null ? "" : contact.FIRST_NAME_TH,
            //            ContactLastNameTh = contact == null ? "" : contact.LAST_NAME_TH,
            //            ContactTitleEn = contactTitleEn == null ? "" : contactTitleEn.TITLE_NAME,
            //            ContactFirstNameEn = contact == null ? "" : contact.FIRST_NAME_EN,
            //            ContactLastNameEn = contact == null ? "" : contact.LAST_NAME_EN,
            //            ContactEmail = contact == null ? "" : contact.EMAIL,

            //            ContactAccountNo = sr == null ? "" : sr.CONTACT_ACCOUNT_NO,
            //            ContactRelationshipName = contactRelationship == null ? "" : contactRelationship.RELATIONSHIP_NAME,

            //            ProductGroupCode = productGroup == null ? "" : productGroup.PRODUCTGROUP_CODE,
            //            ProductGroupName = productGroup == null ? "" : productGroup.PRODUCTGROUP_NAME,
            //            ProductCode = product == null ? "" : product.PRODUCT_CODE,
            //            ProductName = product == null ? "" : product.PRODUCT_NAME,
            //            CampaignServiceCode = campaignService == null ? "" : campaignService.CAMPAIGNSERVICE_CODE,
            //            CampaignServiceName = campaignService == null ? "" : campaignService.CAMPAIGNSERVICE_NAME,
            //            AreaId = area == null ? 0 : area.AREA_ID,
            //            AreaCode = area == null ? 0 : (area.AREA_CODE ?? 0),
            //            AreaName = area == null ? "" : area.AREA_NAME,
            //            SubAreaId = subArea == null ? 0 : subArea.SUBAREA_ID,
            //            SubAreaCode = subArea == null ? 0 : (subArea.SUBAREA_CODE ?? 0),
            //            SubAreaName = subArea == null ? "" : subArea.SUBAREA_NAME,
            //            TypeId = type == null ? 0 : type.TYPE_ID,
            //            TypeCode = type == null ? 0 : (type.TYPE_CODE ?? 0),
            //            TypeName = type == null ? "" : type.TYPE_NAME,
            //            ChannelCode = channel == null ? "" : channel.CHANNEL_CODE,
            //            ChannelName = channel == null ? "" : channel.CHANNEL_NAME,
            //            MediaSourceName = mediaSource == null ? "" : mediaSource.MEDIA_SOURCE_NAME,
            //            Subject = sr == null ? "" : sr.SR_SUBJECT,
            //            Remark = sr == null ? "" : sr.SR_REMARK,
            //            Verify = sr == null ? "" : ((sr.SR_IS_VERIFY ?? false) ? "Y" : "N"),
            //            VerifyResult = sr == null ? "" : sr.SR_IS_VERIFY_PASS,
            //            SrPageId = sr == null ? 0 : (sr.SR_PAGE_ID ?? 0),

            //            // Officer
            //            SRCreatorBranchName = creatorBranch == null ? "" : creatorBranch.BRANCH_NAME,
            //            SRCreatorUserFirstName = creator == null ? "" : creator.FIRST_NAME,
            //            SRCreatorUserLastName = creator == null ? "" : creator.LAST_NAME,
            //            SRCreatorUserPositionCode = creator == null ? "" : creator.POSITION_CODE,

            //            OwnerBranchName = ownerBranch == null ? "" : ownerBranch.BRANCH_NAME,
            //            OwnerUserFirstName = ownerUser == null ? "" : ownerUser.FIRST_NAME,
            //            OwnerUserLastName = ownerUser == null ? "" : ownerUser.LAST_NAME,
            //            OwnerUserPositionCode = ownerUser == null ? "" : ownerUser.POSITION_CODE,

            //            DelegateBranchName = delegateBranch == null ? "" : delegateBranch.BRANCH_NAME,
            //            DelegateUserFirstName = delegateUser == null ? "" : delegateUser.FIRST_NAME,
            //            DelegateUserLastName = delegateUser == null ? "" : delegateUser.LAST_NAME,
            //            DelegateUserPositionCode = delegateUser == null ? "" : delegateUser.POSITION_CODE,

            //            SendEmail = act == null ? "" : act.SR_EMAIL_TEMPLATE_ID.HasValue ? "Y" : "N",
            //            EmailSender = act == null ? "" : act.SR_ACTIVITY_EMAIL_SENDER,
            //            EmailTo = act == null ? "" : act.SR_ACTIVITY_EMAIL_TO,
            //            EmailCc = act == null ? "" : act.SR_ACTIVITY_EMAIL_CC,
            //            EmailSubject = act == null ? "" : act.SR_ACTIVITY_EMAIL_SUBJECT,
            //            EmailBody = act == null ? "" : act.SR_ACTIVITY_EMAIL_BODY,
            //            EmailAttachments = act == null ? "" : act.SR_ACTIVITY_EMAIL_ATTACHMENTS,

            //            ActivityDescription = act == null ? "" : act.SR_ACTIVITY_DESC,
            //            ActivityTypeId = actType == null ? 0 : actType.SR_ACTIVITY_TYPE_ID,
            //            ActivityTypeName = actType == null ? "" : actType.SR_ACTIVITY_TYPE_NAME,

            //            SRStatusName = actStatus == null ? "" : actStatus.SR_STATUS_NAME,

            //            AddressHouseNo = sr == null ? "" : sr.SR_DEF_ADDRESS_HOUSE_NO,
            //            AddressVillage = sr == null ? "" : sr.SR_DEF_ADDRESS_VILLAGE,
            //            AddressBuilding = sr == null ? "" : sr.SR_DEF_ADDRESS_BUILDING,
            //            AddressFloorNo = sr == null ? "" : sr.SR_DEF_ADDRESS_FLOOR_NO,
            //            AddressRoomNo = sr == null ? "" : sr.SR_DEF_ADDRESS_ROOM_NO,
            //            AddressMoo = sr == null ? "" : sr.SR_DEF_ADDRESS_MOO,
            //            AddressSoi = sr == null ? "" : sr.SR_DEF_ADDRESS_SOI,
            //            AddressStreet = sr == null ? "" : sr.SR_DEF_ADDRESS_STREET,
            //            AddressTambol = sr == null ? "" : sr.SR_DEF_ADDRESS_TAMBOL,
            //            AddressAmphur = sr == null ? "" : sr.SR_DEF_ADDRESS_AMPHUR,
            //            AddressProvince = sr == null ? "" : sr.SR_DEF_ADDRESS_PROVINCE,
            //            AddressZipCode = sr == null ? "" : sr.SR_DEF_ADDRESS_ZIPCODE,

            //            AFSAssetNo = sr == null ? "" : sr.SR_AFS_ASSET_NO,
            //            AFSAssetDesc = sr == null ? "" : sr.SR_AFS_ASSET_DESC,

            //            NCBCustomerBirthDate = sr == null ? null : sr.SR_NCB_CUSTOMER_BIRTHDATE,
            //            NCBCheckStatus = sr == null ? "" : sr.SR_NCB_CHECK_STATUS,
            //            NCBMarketingFullName = sr == null ? "" : sr.SR_NCB_MARKETING_FULL_NAME,
            //            NCBMarketingBranchName = sr == null ? "" : sr.SR_NCB_MARKETING_BRANCH_NAME,
            //            NCBMarketingBranchUpper1Name = sr == null ? "" : sr.SR_NCB_MARKETING_BRANCH_UPPER_1_NAME,
            //            NCBMarketingBranchUpper2Name = sr == null ? "" : sr.SR_NCB_MARKETING_BRANCH_UPPER_2_NAME,

            //            ActivityCreateDate = act == null ? null : act.CREATE_DATE,
            //            ActivityCreateUserFirstName = actCreator == null ? "" : actCreator.FIRST_NAME,
            //            ActivityCreateUserLastName = actCreator == null ? "" : actCreator.LAST_NAME,
            //            ActivityCreateUserPositionCode = actCreator == null ? "" : actCreator.POSITION_CODE,
            //            ActivityCreateUserEmployeeCode = actCreator == null ? "" : actCreator.EMPLOYEE_CODE,

            //            MappingProductId = sr == null ? 0 : (sr.MAP_PRODUCT_ID ?? 0),

            //            HPSubject = mapping == null ? "" : mapping.HP_SUBJECT,
            //            HPLanguageIndependentCode = mapping == null ? "" : mapping.HP_LANGUAGE_INDEPENDENT_CODE,
            //        };

            #endregion

            if (activity.CustomerId > 0)
            {
                var customerPhones = (from phone in _context.TB_M_PHONE
                                      from phoneType in _context.TB_M_PHONE_TYPE.Where(x => x.PHONE_TYPE_ID == phone.PHONE_TYPE_ID)
                                      where phone.CUSTOMER_ID == activity.CustomerId
                                      orderby phone.PHONE_ID
                                      select new
                                      {
                                          phone.PHONE_NO,
                                          phoneType.PHONE_TYPE_CODE,
                                          phoneType.PHONE_TYPE_NAME
                                      }).ToList();

                var customerPhoneNoFaxes = customerPhones.Where(x => x.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax).ToList();
                if (customerPhoneNoFaxes.Count > 0)
                {
                    activity.CustomerPhoneNo1 = customerPhoneNoFaxes[0].PHONE_NO;
                    activity.CustomerPhoneType1 = customerPhoneNoFaxes[0].PHONE_TYPE_NAME;

                    if (customerPhoneNoFaxes.Count > 1)
                    {
                        activity.CustomerPhoneNo2 = customerPhoneNoFaxes[1].PHONE_NO;
                        activity.CustomerPhoneType2 = customerPhoneNoFaxes[1].PHONE_TYPE_NAME;

                        if (customerPhoneNoFaxes.Count > 2)
                        {
                            activity.CustomerPhoneNo3 = customerPhoneNoFaxes[2].PHONE_NO;
                            activity.CustomerPhoneType3 = customerPhoneNoFaxes[2].PHONE_TYPE_NAME;
                        }
                    }
                }

                var customerFax = customerPhones.Where(x => x.PHONE_TYPE_CODE == Constants.PhoneTypeCode.Fax).FirstOrDefault();
                if (customerFax != null)
                    activity.CustomerFax = customerFax.PHONE_NO;
            }

            if (activity.ContactId > 0)
            {
                var contactPhones = _context.TB_M_CONTACT_PHONE.Where(x => x.CONTACT_ID == activity.ContactId)
                                            .Select(x => new
                                            {
                                                x.PHONE_NO,
                                                x.TB_M_PHONE_TYPE.PHONE_TYPE_CODE,
                                                x.TB_M_PHONE_TYPE.PHONE_TYPE_NAME
                                            }).ToList();

                var contactPhoneNoFaxes = contactPhones.Where(x => x.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax).ToList();
                if (contactPhoneNoFaxes.Count > 0)
                {
                    activity.ContactPhoneNo1 = contactPhoneNoFaxes[0].PHONE_NO;
                    activity.ContactPhoneType1 = contactPhoneNoFaxes[0].PHONE_TYPE_NAME;

                    if (contactPhoneNoFaxes.Count > 1)
                    {
                        activity.ContactPhoneNo2 = contactPhoneNoFaxes[1].PHONE_NO;
                        activity.ContactPhoneType2 = contactPhoneNoFaxes[1].PHONE_TYPE_NAME;

                        if (contactPhoneNoFaxes.Count > 2)
                        {
                            activity.ContactPhoneNo3 = contactPhoneNoFaxes[2].PHONE_NO;
                            activity.ContactPhoneType3 = contactPhoneNoFaxes[2].PHONE_TYPE_NAME;
                        }
                    }
                }

                var contactFax = contactPhones.FirstOrDefault(x => x.PHONE_TYPE_CODE == Constants.PhoneTypeCode.Fax);
                if (contactFax != null)
                    activity.ContactFax = contactFax.PHONE_NO;
            }

            return activity;
        }

        public List<SRReplyEmailEntity> GetWaitingForProcessReplyEmail()
        {
            return (from re in _context.TB_T_SR_REPLY_EMAIL
                    from sr in _context.TB_T_SR.Where(x => x.SR_ID == re.SR_ID).DefaultIfEmpty()
                    from jb in _context.TB_T_JOB.Where(x => x.JOB_ID == re.JOB_ID).DefaultIfEmpty()
                    from ch in _context.TB_C_SR_STATUS_CHANGE.Where(x =>
                            sr.SR_STATUS_ID != re.SR_STATUS_ID
                            && x.FROM_SR_STATUS_ID == sr.SR_STATUS_ID
                            && x.TO_SR_STATUS_ID == re.SR_STATUS_ID
                            && x.PRODUCTGROUP_ID == sr.PRODUCTGROUP_ID
                            && x.PRODUCT_ID == sr.PRODUCT_ID
                            && x.CAMPAIGNSERVICE_ID == sr.CAMPAIGNSERVICE_ID
                            && x.AREA_ID == sr.AREA_ID
                            && x.SUBAREA_ID == sr.SUBAREA_ID
                            && x.TYPE_ID == sr.TYPE_ID).DefaultIfEmpty()
                    where (!re.IS_PROCESS.HasValue || re.IS_PROCESS.Value == false)
                          && re.SR_ID.HasValue && (re.SR_ID.Value != 0)
                    orderby re.CREATE_DATE ascending
                    select new SRReplyEmailEntity
                    {
                        SrReplyEmailId = re.SR_REPLY_EMAIL_ID,
                        SrId = re.SR_ID.Value,
                        NewSrStatusId = re.SR_STATUS_ID ?? 0,
                        OldSrStatusId = sr.SR_STATUS_ID ?? 0,
                        JobId = re.JOB_ID ?? 0,
                        CreateDate = re.CREATE_DATE,
                        IsChangeStatus = sr.SR_STATUS_ID != re.SR_STATUS_ID,
                        CanChangeStatus = (ch != null),
                        OwnerBranchId = sr.OWNER_BRANCH_ID ?? 0,
                        OwnerUserId = sr.OWNER_USER_ID ?? 0,
                        DelegateBranchId = sr.DELEGATE_BRANCH_ID,
                        DelegateUserId = sr.DELEGATE_USER_ID,
                        CreateUserId = jb.CREATE_USER,
                        CreateUsername = jb.CREATE_USERNAME,
                        EmailFrom = jb.FROM,
                        EmailSubject = jb.SUBJECT,
                        EmailBody = jb.CONTENT,
                        SrAttachments = jb.TB_T_JOB_ATTACHMENT.Select(x => new
                        {
                            x.URL,
                            x.ATTACHMENT_NAME,
                            x.ATTACHMENT_DESC,
                            x.FILE_NAME,
                            x.CONTENT_TYPE,
                            x.CREATE_DATE
                        })
                        .ToList()
                        .Select(x => new SrAttachmentEntity
                        {
                            Url = x.URL,
                            Name = x.ATTACHMENT_NAME,
                            DocDesc = x.ATTACHMENT_DESC,
                            Filename = x.FILE_NAME,
                            ContentType = x.CONTENT_TYPE,
                            ExpiryDate = "",
                            AttachToEmail = "",
                            CreateDateAsDateTime = x.CREATE_DATE,
                            IsEditable = true,
                            CreateUserId = jb.CREATE_USER.HasValue ? jb.CREATE_USER + "" : null,
                            CreateUserName = jb.CREATE_USERNAME
                        }).ToList()
                    }).ToList();
        }

        public SRReplyEmailEntity GetProcessReplyEmail(int srReplyEmailId)
        {
            return (from re in _context.TB_T_SR_REPLY_EMAIL
                    from sr in _context.TB_T_SR.Where(x => x.SR_ID == re.SR_ID).DefaultIfEmpty()
                    from jb in _context.TB_T_JOB.Where(x => x.JOB_ID == re.JOB_ID).DefaultIfEmpty()
                    from ch in _context.TB_C_SR_STATUS_CHANGE.Where(x =>
                            sr.SR_STATUS_ID != re.SR_STATUS_ID
                            && x.FROM_SR_STATUS_ID == sr.SR_STATUS_ID
                            && x.TO_SR_STATUS_ID == re.SR_STATUS_ID
                            && x.PRODUCTGROUP_ID == sr.PRODUCTGROUP_ID
                            && x.PRODUCT_ID == sr.PRODUCT_ID
                            && x.CAMPAIGNSERVICE_ID == sr.CAMPAIGNSERVICE_ID
                            && x.AREA_ID == sr.AREA_ID
                            && x.SUBAREA_ID == sr.SUBAREA_ID
                            && x.TYPE_ID == sr.TYPE_ID).DefaultIfEmpty()
                    where (re.SR_REPLY_EMAIL_ID == srReplyEmailId)
                    select new SRReplyEmailEntity
                    {
                        SrReplyEmailId = re.SR_REPLY_EMAIL_ID,
                        SrId = re.SR_ID.Value,
                        NewSrStatusId = re.SR_STATUS_ID ?? 0,
                        OldSrStatusId = sr.SR_STATUS_ID ?? 0,
                        JobId = re.JOB_ID ?? 0,
                        CreateDate = re.CREATE_DATE,
                        IsChangeStatus = sr.SR_STATUS_ID != re.SR_STATUS_ID,
                        CanChangeStatus = (ch != null),
                        OwnerBranchId = sr.OWNER_BRANCH_ID ?? 0,
                        OwnerUserId = sr.OWNER_USER_ID ?? 0,
                        DelegateBranchId = sr.DELEGATE_BRANCH_ID,
                        DelegateUserId = sr.DELEGATE_USER_ID,
                        CreateUserId = jb.CREATE_USER,
                        CreateUsername = jb.CREATE_USERNAME,
                        EmailFrom = jb.FROM,
                        EmailSubject = jb.SUBJECT,
                        EmailBody = jb.CONTENT,
                        SrAttachments = jb.TB_T_JOB_ATTACHMENT.Select(x => new
                        {
                            x.URL,
                            x.ATTACHMENT_NAME,
                            x.ATTACHMENT_DESC,
                            x.FILE_NAME,
                            x.CONTENT_TYPE,
                            x.CREATE_DATE
                        })
                        .ToList()
                        .Select(x => new SrAttachmentEntity
                        {
                            Url = x.URL,
                            Name = x.ATTACHMENT_NAME,
                            DocDesc = x.ATTACHMENT_DESC,
                            Filename = x.FILE_NAME,
                            ContentType = x.CONTENT_TYPE,
                            ExpiryDate = "",
                            AttachToEmail = "",
                            CreateDateAsDateTime = x.CREATE_DATE,
                            IsEditable = true,
                            CreateUserId = jb.CREATE_USER.HasValue ? jb.CREATE_USER + "" : null,
                            CreateUserName = jb.CREATE_USERNAME
                        }).ToList()
                    }).SingleOrDefault();
        }

        //public List<SrAttachmentEntity> GetSrAttachmentsByJobId(int jobId)
        //{
        //    if (jobId == 0)
        //        return new List<SrAttachmentEntity>();

        //    return (from j in _context.TB_T_JOB_ATTACHMENT
        //            where j.JOB_ID == jobId
        //            select new SrAttachmentEntity
        //            {
        //                Url = j.URL,
        //                Name = j.ATTACHMENT_NAME,
        //                Filename = j.FILE_NAME,
        //                DocDesc = j.ATTACHMENT_DESC,
        //                ContentType = j.CONTENT_TYPE,
        //                ExpiryDate = null,
        //                AttachToEmail = null,
        //                CreateDate = null,
        //                DocumentType = j.TB_T_ATTACHMENT_TYPE.Select(t => new AttachmentTypeEntity() { DocTypeId = t.DOCUMENT_TYPE_ID }).ToList()
        //            }).ToList();
        //}

        public void UpdateProcessReplyEmail(int srReplyEmailId, bool isProcess, DateTime processDateTime, string errorCode, string errorMessage)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var item = _context.TB_T_SR_REPLY_EMAIL.SingleOrDefault(x => x.SR_REPLY_EMAIL_ID == srReplyEmailId);

                if (item == null)
                    throw new CustomException("Not found ReplyEmail in database (ID={0}", srReplyEmailId);

                item.IS_PROCESS = isProcess;
                item.PROCESS_DATE = processDateTime;
                item.ERROR_CODE = errorCode;
                item.ERROR_MESSAGE = errorMessage;

                SetEntryStateModified(item);
                Save();
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public List<int> GetWaitingForProcessReSubmitToCAR()
        {
            return (from ac in _context.TB_T_SR_ACTIVITY
                    where (!ac.ACTIVITY_CAR_SUBMIT_STATUS.HasValue || ac.ACTIVITY_CAR_SUBMIT_STATUS.Value == 0)
                    orderby ac.CREATE_DATE ascending
                    select ac.SR_ACTIVITY_ID).ToList();
        }

        public List<int> GetWaitingForProcessReSubmitToCBSHP()
        {
            return (from ac in _context.TB_T_SR_ACTIVITY
                    where (ac.ACTIVITY_HP_SUBMIT_STATUS.Value == 0)
                    orderby ac.CREATE_DATE ascending
                    select ac.SR_ACTIVITY_ID).ToList();
        }

        public List<SearchSRResponseItem> SearchServiceRequestWebService(SearchSRRequest searchFilter, ref int totalRecords)
        {
            var query = _context.TB_T_SR.AsQueryable();
            query = query.Where(x => x.SR_STATUS_ID != Constants.SRStatusId.Cancelled && x.SR_STATUS_ID != Constants.SRStatusId.Closed);

            if (!string.IsNullOrEmpty(searchFilter.CustomerCardNo) && !string.IsNullOrEmpty(searchFilter.CustomerSubscriptionTypeCode))
            {
                query = query.Where(q =>
                    q.TB_M_CUSTOMER.CARD_NO == searchFilter.CustomerCardNo
                    && q.TB_M_CUSTOMER.TB_M_SUBSCRIPT_TYPE.SUBSCRIPT_TYPE_CODE == searchFilter.CustomerSubscriptionTypeCode);
            }

            if (!string.IsNullOrEmpty(searchFilter.AccountNo))
            {
                query = query.Where(q => q.TB_M_ACCOUNT.ACCOUNT_NO == searchFilter.AccountNo);
            }

            if (!string.IsNullOrEmpty(searchFilter.ContactCardNo) && !string.IsNullOrEmpty(searchFilter.ContactSubscriptionTypeCode))
            {
                query = query.Where(q =>
                    q.TB_M_CONTACT.CARD_NO == searchFilter.ContactCardNo
                    && q.TB_M_CONTACT.TB_M_SUBSCRIPT_TYPE.SUBSCRIPT_TYPE_CODE == searchFilter.ContactSubscriptionTypeCode);
            }

            if (!string.IsNullOrEmpty(searchFilter.ProductGroupCode))
            {
                query = query.Where(q => q.TB_R_PRODUCTGROUP.PRODUCTGROUP_CODE == searchFilter.ProductGroupCode);
            }

            if (!string.IsNullOrEmpty(searchFilter.ProductCode))
            {
                query = query.Where(q => q.TB_R_PRODUCT.PRODUCT_CODE == searchFilter.ProductCode);
            }

            if (!string.IsNullOrEmpty(searchFilter.CampaignServiceCode))
            {
                query = query.Where(q => q.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_CODE == searchFilter.CampaignServiceCode);
            }

            if (searchFilter.AreaCode != 0)
            {
                query = query.Where(q => q.TB_M_AREA.AREA_CODE == searchFilter.AreaCode);
            }

            if (searchFilter.SubAreaCode != 0)
            {
                query = query.Where(q => q.TB_M_SUBAREA.SUBAREA_CODE == searchFilter.SubAreaCode);
            }

            if (searchFilter.TypeCode != 0)
            {
                query = query.Where(q => q.TB_M_TYPE.TYPE_CODE == searchFilter.TypeCode);
            }

            if (!string.IsNullOrEmpty(searchFilter.ChannelCode))
            {
                query = query.Where(q => q.TB_R_CHANNEL.CHANNEL_CODE == searchFilter.ChannelCode);
            }

            if (!string.IsNullOrEmpty(searchFilter.SRStatusCode))
            {
                query = query.Where(q => q.TB_C_SR_STATUS.SR_STATUS_CODE == searchFilter.SRStatusCode);
            }

            if (!string.IsNullOrEmpty(searchFilter.ActivityTypeCode))
            {
                query = query.Where(q => q.TB_C_SR_ACTIVITY_TYPE.SR_ACTIVITY_TYPE_NAME == searchFilter.ActivityTypeCode);
            }

            var employeeCodeForOwnerSR = searchFilter.EmployeeCodeforOwnerSR;
            var employeeCodeForDelegateSR = searchFilter.EmployeeCodeforDelegateSR;

            var isSearchOwner = !string.IsNullOrEmpty(employeeCodeForOwnerSR);
            var isSearchDelegate = !string.IsNullOrEmpty(employeeCodeForDelegateSR);

            if (!string.IsNullOrEmpty(employeeCodeForOwnerSR))
            {
                query = from s in query
                        from ownerUser in _context.TB_R_USER.Where(u => u.USER_ID == s.OWNER_USER_ID).DefaultIfEmpty()
                        where ownerUser.EMPLOYEE_CODE == employeeCodeForOwnerSR
                        select s;
            }

            if (!string.IsNullOrEmpty(employeeCodeForDelegateSR))
            {
                query = from s in query
                        from delegateUser in _context.TB_R_USER.Where(u => u.USER_ID == s.DELEGATE_USER_ID).DefaultIfEmpty()
                        where delegateUser.EMPLOYEE_CODE == employeeCodeForDelegateSR
                        select s;
            }

            // After Insert all filters >> Count It
            totalRecords = query.Count();

            var results = from q in query
                          from ownerUser in _context.TB_R_USER.Where(user => q.OWNER_USER_ID == user.USER_ID).DefaultIfEmpty()
                          from delegateUser in _context.TB_R_USER.Where(user => q.DELEGATE_USER_ID == user.USER_ID).DefaultIfEmpty()
                          select new SearchSRResponseItem
                          {
                              SrId = q.SR_ID,
                              SrNo = q.SR_NO,

                              ThisAlert = q.RULE_THIS_ALERT,
                              NextSLA = q.RULE_NEXT_SLA,
                              TotalWorkingHours = q.RULE_TOTAL_WORK,

                              CustomerFirstNameTh = q.TB_M_CUSTOMER.FIRST_NAME_TH,
                              CustomerLastNameTh = q.TB_M_CUSTOMER.LAST_NAME_TH,
                              CustomerFirstNameEn = q.TB_M_CUSTOMER.FIRST_NAME_EN,
                              CustomerLastNameEn = q.TB_M_CUSTOMER.LAST_NAME_EN,
                              CustomerSubscriptionTypeCode = q.TB_M_CUSTOMER.TB_M_SUBSCRIPT_TYPE.CARD_TYPE_CODE,
                              CustomerSubscriptionTypeName = q.TB_M_CUSTOMER.TB_M_SUBSCRIPT_TYPE.SUBSCRIPT_TYPE_NAME,
                              CustomerCardNo = q.TB_M_CUSTOMER.CARD_NO,
                              AccountNo = q.TB_M_ACCOUNT.ACCOUNT_NO,
                              ContactSubscriptionTypeCode = q.TB_M_CONTACT.TB_M_SUBSCRIPT_TYPE.CARD_TYPE_CODE,
                              ContactCardNo = q.TB_M_CONTACT.CARD_NO,

                              Subject = q.SR_SUBJECT,
                              Remark = q.SR_REMARK,
                              ANo = q.SR_ANO,
                              CallId = q.SR_CALL_ID,

                              ProductGroupCode = q.TB_R_PRODUCTGROUP.PRODUCTGROUP_CODE,
                              ProductGroupName = q.TB_R_PRODUCTGROUP.PRODUCTGROUP_NAME,
                              ProductCode = q.TB_R_PRODUCT.PRODUCT_CODE,
                              ProductName = q.TB_R_PRODUCT.PRODUCT_NAME,
                              CampaignServiceCode = q.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_CODE,
                              CampaignServiceName = q.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_NAME,

                              AreaCode = q.TB_M_AREA.AREA_CODE,
                              AreaName = q.TB_M_AREA.AREA_NAME,
                              SubAreaCode = q.TB_M_SUBAREA.SUBAREA_CODE,
                              SubAreaName = q.TB_M_SUBAREA.SUBAREA_NAME,
                              TypeCode = q.TB_M_TYPE.TYPE_CODE,
                              TypeName = q.TB_M_TYPE.TYPE_NAME,

                              ChannelCode = q.TB_R_CHANNEL.CHANNEL_CODE,
                              ChannelName = q.TB_R_CHANNEL.CHANNEL_NAME,

                              ActivityTypeName = q.TB_C_SR_ACTIVITY_TYPE.SR_ACTIVITY_TYPE_NAME,
                              MediaSourceName = q.TB_M_MEDIA_SOURCE.MEDIA_SOURCE_NAME,

                              SrStatusCode = q.TB_C_SR_STATUS.SR_STATUS_CODE,
                              SrStatusName = q.TB_C_SR_STATUS.SR_STATUS_NAME,

                              CreateDate = q.CREATE_DATE,
                              ClosedDate = q.CLOSE_DATE,

                              OwnerUserEmployeeCode = ownerUser != null ? ownerUser.EMPLOYEE_CODE : null,
                              OwnerUserPositionCode = ownerUser != null ? ownerUser.POSITION_CODE : null,
                              OwnerUserFirstName = ownerUser != null ? ownerUser.FIRST_NAME : null,
                              OwnerUserLastName = ownerUser != null ? ownerUser.LAST_NAME : null,

                              OwnerBranchName = ownerUser != null ? ownerUser.TB_R_BRANCH.BRANCH_NAME : null,

                              DelegateUserEmployeeCode = delegateUser != null ? delegateUser.EMPLOYEE_CODE : null,
                              DelegateUserPosition = delegateUser != null ? delegateUser.POSITION_CODE : null,
                              DelegateUserFirstName = delegateUser != null ? delegateUser.FIRST_NAME : null,
                              DelegateUserLastName = delegateUser != null ? delegateUser.LAST_NAME : null,

                              DelegateBranchName = delegateUser != null ? delegateUser.TB_R_BRANCH.BRANCH_NAME : null,

                              IsVerifyQuestion = q.SR_IS_VERIFY,
                              VerifyQuestionPass = q.SR_IS_VERIFY_PASS,
                              DefaultHouseNo = q.SR_DEF_ADDRESS_HOUSE_NO,
                              DefaultVillage = q.SR_DEF_ADDRESS_VILLAGE,
                              DefaultBuilding = q.SR_DEF_ADDRESS_BUILDING,
                              DefaultFloorNo = q.SR_DEF_ADDRESS_FLOOR_NO,
                              DefaultRoomNo = q.SR_DEF_ADDRESS_ROOM_NO,
                              DefaultMoo = q.SR_DEF_ADDRESS_MOO,
                              DefaultSoi = q.SR_DEF_ADDRESS_SOI,
                              DefaultStreet = q.SR_DEF_ADDRESS_STREET,
                              DefaultTambol = q.SR_DEF_ADDRESS_TAMBOL,
                              DefaultAmphur = q.SR_DEF_ADDRESS_AMPHUR,
                              DefaultProvince = q.SR_DEF_ADDRESS_PROVINCE,
                              DefaultZipCode = q.SR_DEF_ADDRESS_ZIPCODE,
                              AFSAssetNo = q.SR_AFS_ASSET_NO,
                              AFSAssetDesc = q.SR_AFS_ASSET_DESC,
                              NCBCustomerBirthDate = q.SR_NCB_CUSTOMER_BIRTHDATE,
                              NCBCheckStatus = q.SR_NCB_CHECK_STATUS,
                              NCBMarkeingFullName = q.SR_NCB_MARKETING_FULL_NAME,
                              NCBMarkeingBranchName = q.SR_NCB_MARKETING_BRANCH_NAME,
                              NCBMarkeingBranchUpper1Name = q.SR_NCB_MARKETING_BRANCH_UPPER_1_NAME,
                              NCBMarkeingBranchUpper2Name = q.SR_NCB_MARKETING_BRANCH_UPPER_2_NAME
                          };

            results = results.OrderByDescending(q => q.ThisAlert);

            //switch (searchFilter.SortField)
            //{
            //    case "ProductName":
            //        results = searchFilter.SortOrder.ToUpper().Equals("ASC") ? results.OrderBy(q => q.ProductName) : results.OrderByDescending(q => q.ProductName);
            //        break;
            //    case "AreaName":
            //        results = searchFilter.SortOrder.ToUpper().Equals("ASC")
            //            ? results.OrderBy(q => q.AreaName)
            //            : results.OrderByDescending(q => q.AreaName);
            //        break;
            //    case "SubAreaName":
            //        results = searchFilter.SortOrder.ToUpper().Equals("ASC")
            //            ? results.OrderBy(q => q.SubAreaName)
            //            : results.OrderByDescending(q => q.SubAreaName);
            //        break;
            //    case "SrStatus":
            //        results = searchFilter.SortOrder.ToUpper().Equals("ASC")
            //            ? results.OrderBy(q => q.SrStatusName)
            //            : results.OrderByDescending(q => q.SrStatusName);
            //        break;
            //    case "CreateDate":
            //        results = searchFilter.SortOrder.ToUpper().Equals("ASC")
            //            ? results.OrderBy(q => q.CreateDate)
            //            : results.OrderByDescending(q => q.CreateDate);
            //        break;
            //    case "CloseDate":
            //        results = searchFilter.SortOrder.ToUpper().Equals("ASC")
            //            ? results.OrderBy(q => q.ClosedDate)
            //            : results.OrderByDescending(q => q.ClosedDate);
            //        break;
            //    case "OwnerUserFirstName":
            //        results = searchFilter.SortOrder.ToUpper().Equals("ASC")
            //            ? results.OrderBy(q => q.OwnerUserFirstName)
            //            : results.OrderByDescending(q => q.OwnerUserFirstName);
            //        break;
            //    case "DelegateUserFirstName":
            //        results = searchFilter.SortOrder.ToUpper().Equals("ASC")
            //            ? results.OrderBy(q => q.DelegateUserFirstName)
            //            : results.OrderByDescending(q => q.DelegateUserFirstName);
            //        break;

            //    default:
            //        results = results.OrderByDescending(q => q.ThisAlert);
            //        break;
            //}

            return results.Skip(searchFilter.StartPageIndex).Take(searchFilter.PageSize).ToList();
        }

        public GetSRResponse GetServiceRequestWebService(string srNo)
        {
            var query = _context.TB_T_SR.Where(q => q.SR_NO == srNo);

            var results = from q in query
                          from ownerUser in _context.TB_R_USER.Where(user => q.OWNER_USER_ID == user.USER_ID).DefaultIfEmpty()
                          from delegateUser in _context.TB_R_USER.Where(user => q.DELEGATE_USER_ID == user.USER_ID).DefaultIfEmpty()
                          select new GetSRResponse()
                          {
                              SRId = q.SR_ID,
                              SRNo = q.SR_NO,

                              SLA = (q.RULE_THIS_ALERT ?? 0) > 0,
                              AlertNo = q.RULE_THIS_ALERT ?? 0,
                              NextSLA = q.RULE_NEXT_SLA ?? DateTime.Now,
                              TotalWorkingHours = q.RULE_TOTAL_WORK ?? 0,

                              //                              CustomerFirstNameTh = q.TB_M_CUSTOMER.FIRST_NAME_TH,
                              //                              CustomerLastNameTh = q.TB_M_CUSTOMER.LAST_NAME_TH,
                              //                              CustomerFirstNameEn = q.TB_M_CUSTOMER.FIRST_NAME_EN,
                              //                              CustomerLastNameEn = q.TB_M_CUSTOMER.LAST_NAME_EN,
                              //                              CustomerSubscriptionTypeCode = q.TB_M_CUSTOMER.TB_M_SUBSCRIPT_TYPE.CARD_TYPE_CODE,
                              //                              CustomerSubscriptionTypeName = q.TB_M_CUSTOMER.TB_M_SUBSCRIPT_TYPE.SUBSCRIPT_TYPE_NAME,
                              CustomerCardNo = q.TB_M_CUSTOMER.CARD_NO,
                              AccountNO = q.TB_M_ACCOUNT.ACCOUNT_NO,
                              ContactSubscriptionType = q.TB_M_CONTACT.TB_M_SUBSCRIPT_TYPE.CARD_TYPE_CODE,
                              ContactCardNo = q.TB_M_CONTACT.CARD_NO,

                              Subject = q.SR_SUBJECT,
                              Remark = q.SR_REMARK,
                              ANo = q.SR_ANO,
                              //                              CallId = q.SR_CALL_ID,

                              //                              ProductGroupCode = q.TB_R_PRODUCTGROUP.PRODUCTGROUP_CODE,
                              ProductGroupName = q.TB_R_PRODUCTGROUP.PRODUCTGROUP_NAME,
                              //                              ProductCode = q.TB_R_PRODUCT.PRODUCT_CODE,
                              ProductName = q.TB_R_PRODUCT.PRODUCT_NAME,
                              //                              CampaignServiceCode = q.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_CODE,
                              CampaignServiceName = q.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_NAME,

                              //                              AreaCode = q.TB_M_AREA.AREA_CODE,
                              AreaName = q.TB_M_AREA.AREA_NAME,
                              //                              SubAreaCode = q.TB_M_SUBAREA.SUBAREA_CODE,
                              SubAreaName = q.TB_M_SUBAREA.SUBAREA_NAME,
                              //                              TypeCode = q.TB_M_TYPE.TYPE_CODE,
                              TypeName = q.TB_M_TYPE.TYPE_NAME,

                              //                              ChannelCode = q.TB_R_CHANNEL.CHANNEL_CODE,
                              SRChannelName = q.TB_R_CHANNEL.CHANNEL_NAME,

                              //                              ActivityTypeName = q.TB_C_SR_ACTIVITY_TYPE.SR_ACTIVITY_TYPE_NAME,
                              MediaSourceName = q.TB_M_MEDIA_SOURCE.MEDIA_SOURCE_NAME,

                              //                              SrStatusCode = q.TB_C_SR_STATUS.SR_STATUS_CODE,
                              CurrentSRStatus = q.TB_C_SR_STATUS.SR_STATUS_NAME,

                              CreateDateDt = q.CREATE_DATE,
                              CloseDateDt = q.CLOSE_DATE,
                              //                              CreateDate = DateUtil.ToStringAsDateTime(q.CREATE_DATE),
                              //                              CloseDate = DateUtil.ToStringAsDateTime(q.CLOSE_DATE),

                              OwnerSREmployeeCode = ownerUser != null ? ownerUser.EMPLOYEE_CODE : null,
                              //                              OwnerUserPositionCode = ownerUser != null ? ownerUser.POSITION_CODE : null,
                              //                              OwnerUserFirstName = ownerUser != null ? ownerUser.FIRST_NAME : null,
                              //                              OwnerUserLastName = ownerUser != null ? ownerUser.LAST_NAME : null,

                              //                              OwnerBranchName = ownerUser != null ? ownerUser.TB_R_BRANCH.BRANCH_NAME : null,

                              DelegateSREmployeeCode = delegateUser != null ? delegateUser.EMPLOYEE_CODE : null,
                              //                              DelegateUserPosition = delegateUser != null ? delegateUser.POSITION_CODE : null,
                              //                              DelegateUserFirstName = delegateUser != null ? delegateUser.FIRST_NAME : null,
                              //                              DelegateUserLastName = delegateUser != null ? delegateUser.LAST_NAME : null,

                              //                              DelegateBranchName = delegateUser != null ? delegateUser.TB_R_BRANCH.BRANCH_NAME : null,

                              IsVerifyQuestion = q.SR_IS_VERIFY ?? false,
                              VerifyQuestionPass = q.SR_IS_VERIFY_PASS,
                              DefaultHouseNo = q.SR_DEF_ADDRESS_HOUSE_NO,
                              DefaultVillage = q.SR_DEF_ADDRESS_VILLAGE,
                              DefaultBuilding = q.SR_DEF_ADDRESS_BUILDING,
                              DefaultFloorNo = q.SR_DEF_ADDRESS_FLOOR_NO,
                              DefaultRoomNo = q.SR_DEF_ADDRESS_ROOM_NO,
                              DefaultMoo = q.SR_DEF_ADDRESS_MOO,
                              DefaultSoi = q.SR_DEF_ADDRESS_SOI,
                              DefaultStreet = q.SR_DEF_ADDRESS_STREET,
                              DefaultTambol = q.SR_DEF_ADDRESS_TAMBOL,
                              DefaultAmphur = q.SR_DEF_ADDRESS_AMPHUR,
                              DefaultProvince = q.SR_DEF_ADDRESS_PROVINCE,
                              DefaultZipCode = q.SR_DEF_ADDRESS_ZIPCODE,
                              AFSAssetNo = q.SR_AFS_ASSET_NO,
                              //                              AFSAssetDesc = q.SR_AFS_ASSET_DESC,
                              NCBCustomerBirthDate = q.SR_NCB_CUSTOMER_BIRTHDATE,
                              NCBCheckStatus = q.SR_NCB_CHECK_STATUS,
                              //                              NCBMarkeingFullName = q.SR_NCB_MARKETING_FULL_NAME,
                              //                              NCBMarkeingBranchName = q.SR_NCB_MARKETING_BRANCH_NAME,
                              //                              NCBMarkeingBranchUpper1Name = q.SR_NCB_MARKETING_BRANCH_UPPER_1_NAME,
                              //                              NCBMarkeingBranchUpper2Name = q.SR_NCB_MARKETING_BRANCH_UPPER_2_NAME,
                          };

            return results.FirstOrDefault();
        }

        public void UpdateServiceRequestWebService(int srId, UpdateSRRequest request)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var sr = _context.TB_T_SR.Single(x => x.SR_ID == srId);
                sr.SR_SUBJECT = request.Subject;
                sr.SR_REMARK = request.Remark;

                if (sr.SR_PAGE_ID == Constants.SRPage.NCBPageId)
                {
                    sr.SR_NCB_CHECK_STATUS = request.NCBCheckStatus;
                    sr.SR_NCB_CUSTOMER_BIRTHDATE = request.NCBCustomerBirthDate;
                    sr.SR_NCB_MARKETING_USER_ID = GetUserIdByEmployeeCode(request.NCBMarketingEmployeeCode);
                }

                SetEntryStateModified(sr);

                Save();
            }
            catch (Exception ex)
            {
                Logger.Warn("Cannot update SR information.", ex);
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public void UpdateDocumentCustomerLevel(int attachmentId, string documentDesc, DateTime? expiryDate, List<int> docTypeIds, int updateUserId)
        {
            var attachment = _context.TB_M_CUSTOMER_ATTACHMENT.SingleOrDefault(x => x.CUSTOMER_ATTACHMENT_ID == attachmentId);

            if (attachment == null)
                throw new CustomException("Not found CustomerAttachment in database. (ID={0})", attachmentId);

            attachment.ATTACHMENT_DESC = documentDesc;
            attachment.EXPIRY_DATE = expiryDate;

            // Remove Un-Check DocumentType
            var attachmentTypes = _context.TB_T_ATTACHMENT_TYPE.Where(x => x.CUSTOMER_ATTACHMENT_ID == attachmentId).ToList();

            var removeList = attachmentTypes.Where(x => x.DOCUMENT_TYPE_ID.HasValue && !docTypeIds.Contains(x.DOCUMENT_TYPE_ID.Value)).ToList();

            var oldDocTypeIds = attachmentTypes.Select(x => x.DOCUMENT_TYPE_ID).ToList();
            var newDocTypeIdList = docTypeIds.Where(x => !oldDocTypeIds.Contains(x)).ToList();

            var newDocTypeList = new List<TB_T_ATTACHMENT_TYPE>();
            foreach (var docTypeId in docTypeIds)
            {
                var docType = new TB_T_ATTACHMENT_TYPE();
                docType.CUSTOMER_ATTACHMENT_ID = attachment.CUSTOMER_ATTACHMENT_ID;
                docType.DOCUMENT_TYPE_ID = docTypeId;
                docType.CREATE_USER = updateUserId;
                docType.CREATE_DATE = DateTime.Now;

                newDocTypeList.Add(docType);
            }

            _context.TB_T_ATTACHMENT_TYPE.RemoveRange(removeList);
            _context.TB_T_ATTACHMENT_TYPE.AddRange(newDocTypeList);
            Save();
        }

        public int? GetCustomerIdByCode(string subscriptionTypeCode, string customerCardNo)
        {
            subscriptionTypeCode = subscriptionTypeCode.ToUpper();
            customerCardNo = customerCardNo.ToUpper();

            var item = _context.TB_M_CUSTOMER
                .Where(x =>
                    x.TB_M_SUBSCRIPT_TYPE.SUBSCRIPT_TYPE_CODE.ToUpper() == subscriptionTypeCode
                    && x.CARD_NO.ToUpper() == customerCardNo)
                .Select(x => new { x.CUSTOMER_ID })
                .FirstOrDefault();

            if (item == null)
                return null;

            return item.CUSTOMER_ID;
        }

        public int? GetAccountIdByAccountNo(int customerId, string accountNo)
        {
            accountNo = accountNo.ToUpper();

            var item = _context.TB_M_ACCOUNT
                .Where(x => x.ACCOUNT_NO.ToUpper() == accountNo && x.CUSTOMER_ID == customerId)
                .Select(x => new { x.ACCOUNT_ID })
                .FirstOrDefault();

            if (item == null)
                return null;

            return item.ACCOUNT_ID;
        }

        public int? GetContactIdByCode(string subscriptionTypeCode, string contactCardNo)
        {
            subscriptionTypeCode = subscriptionTypeCode.ToUpper();
            contactCardNo = contactCardNo.ToUpper();

            var item = _context.TB_M_CONTACT
                .Where(x =>
                    x.TB_M_SUBSCRIPT_TYPE.SUBSCRIPT_TYPE_CODE.ToUpper() == subscriptionTypeCode
                    && x.CARD_NO.ToUpper() == contactCardNo)
                .Select(x => new { x.CONTACT_ID })
                .FirstOrDefault();

            if (item == null)
                return null;

            return item.CONTACT_ID;
        }

        public int? GetReleationshipIdByName(string relationshipName)
        {
            relationshipName = relationshipName.ToUpper();

            var item = _context.TB_M_RELATIONSHIP
                .Where(x => x.RELATIONSHIP_NAME.ToUpper() == relationshipName)
                .Select(x => new { x.RELATIONSHIP_ID })
                .FirstOrDefault();

            if (item == null)
                return null;

            return item.RELATIONSHIP_ID;
        }

        public int? GetProductGroupIdByCode(string code)
        {
            code = code.ToUpper();

            var item = _context.TB_R_PRODUCTGROUP
                .Where(x => x.PRODUCTGROUP_CODE.ToUpper() == code)
                .Select(x => new { x.PRODUCTGROUP_ID })
                .FirstOrDefault();

            if (item == null)
                return null;

            return item.PRODUCTGROUP_ID;
        }

        public int? GetProductIdByCode(string code)
        {
            code = code.ToUpper();

            var item = _context.TB_R_PRODUCT
                .Where(x => x.PRODUCT_CODE.ToUpper() == code)
                .Select(x => new { x.PRODUCT_ID })
                .FirstOrDefault();

            if (item == null)
                return null;

            return item.PRODUCT_ID;
        }

        public int? GetCampaignServiceIdByCode(string code)
        {
            code = code.ToUpper();

            var item = _context.TB_R_CAMPAIGNSERVICE
                .Where(x => x.CAMPAIGNSERVICE_CODE.ToUpper() == code)
                .Select(x => new { x.CAMPAIGNSERVICE_ID })
                .FirstOrDefault();

            if (item == null)
                return null;

            return item.CAMPAIGNSERVICE_ID;
        }

        public int? GetAreaIdByCode(decimal code)
        {
            var item = _context.TB_M_AREA
                .Where(x => x.AREA_CODE == code)
                .Select(x => new { x.AREA_ID })
                .FirstOrDefault();

            if (item == null)
                return null;

            return item.AREA_ID;
        }

        public int? GetSubAreaIdByCode(decimal code)
        {
            var item = _context.TB_M_SUBAREA
                .Where(x => x.SUBAREA_CODE == code)
                .Select(x => new { x.SUBAREA_ID })
                .FirstOrDefault();

            if (item == null)
                return null;

            return item.SUBAREA_ID;
        }

        public int? GetTypeIdByCode(decimal code)
        {
            var item = _context.TB_M_TYPE
                .Where(x => x.TYPE_CODE == code)
                .Select(x => new { x.TYPE_ID })
                .FirstOrDefault();

            if (item == null)
                return null;

            return item.TYPE_ID;
        }

        public int? GetChannelIdByCode(string code)
        {
            code = code.ToUpper();

            var item = _context.TB_R_CHANNEL
                .Where(x => x.CHANNEL_CODE == code)
                .Select(x => new { x.CHANNEL_ID })
                .FirstOrDefault();

            if (item == null)
                return null;

            return item.CHANNEL_ID;
        }

        public int? GetMediaSourceIdByName(string name)
        {
            name = name.ToUpper();

            var item = _context.TB_M_MEDIA_SOURCE
                .Where(x => x.MEDIA_SOURCE_NAME == name)
                .Select(x => new { x.MEDIA_SOURCE_ID })
                .FirstOrDefault();

            if (item == null)
                return null;

            return item.MEDIA_SOURCE_ID;
        }

        public int? GetUserIdByEmployeeCode(string code)
        {
            code = code.ToUpper();

            var item = _context.TB_R_USER
                .Where(x => x.EMPLOYEE_CODE == code)
                .Select(x => new { x.USER_ID })
                .FirstOrDefault();

            if (item == null)
                return null;

            return item.USER_ID;
        }

        public UserEntity GetUserByEmployeeCode(string code)
        {
            code = code.ToUpper();

            var item = (from u in _context.TB_R_USER
                        join b in _context.TB_R_BRANCH on u.BRANCH_ID equals b.BRANCH_ID
                        join c in _context.TB_R_CHANNEL on b.CHANNEL_ID equals c.CHANNEL_ID
                        join r in _context.TB_C_ROLE on u.ROLE_ID equals r.ROLE_ID
                        where u.EMPLOYEE_CODE.ToUpper() == code
                        select new UserEntity
                        {
                            UserId = u.USER_ID,
                            Username = u.USERNAME,
                            Firstname = u.FIRST_NAME,
                            Lastname = u.LAST_NAME,
                            Status = u.STATUS,
                            Email = u.EMAIL,
                            BranchId = u.BRANCH_ID,
                            BranchCode = b.BRANCH_CODE,
                            BranchName = b.BRANCH_NAME,
                            RoleCode = r.ROLE_CODE,
                            RoleValue = r.ROLE_VALUE ?? 0,
                            PositionCode = u.POSITION_CODE,
                            ChannelId = c.CHANNEL_ID,
                            ChannelName = c.CHANNEL_NAME
                        }).FirstOrDefault();

            if (item == null)
                return null;

            return item;
        }

        public int? GetSrStatusIdByCode(string code)
        {
            code = code.ToUpper();

            var item = _context.TB_C_SR_STATUS
                .Where(x => x.SR_STATUS_CODE == code)
                .Select(x => new { x.SR_STATUS_ID })
                .FirstOrDefault();

            if (item == null)
                return null;

            return item.SR_STATUS_ID;
        }

        public int? GetSrPageIdByCode(string code)
        {
            code = code.ToUpper();

            var item = _context.TB_C_SR_PAGE
                .Where(x => x.SR_PAGE_CODE == code)
                .Select(x => new { x.SR_PAGE_ID })
                .FirstOrDefault();

            if (item == null)
                return null;

            return item.SR_PAGE_ID;
        }

        public ActivityTypeEntity GetActivityType(int id)
        {
            var item = _context.TB_C_SR_ACTIVITY_TYPE.FirstOrDefault(x => x.SR_ACTIVITY_TYPE_ID == id);

            if (item == null)
                return null;

            var entity = new ActivityTypeEntity();
            entity.ActivityTypeId = item.SR_ACTIVITY_TYPE_ID;
            entity.Name = item.SR_ACTIVITY_TYPE_NAME;

            return entity;
        }

        public bool CanChangeStatus(int fromSrStatusId, int toSrStatusId, int productGroupId, int productId, int campaignServiceId, int areaId, int subAreaId, int typeId)
        {
            return _context.TB_C_SR_STATUS_CHANGE.Any(x =>
                x.FROM_SR_STATUS_ID == fromSrStatusId
                && x.TO_SR_STATUS_ID == toSrStatusId
                && x.PRODUCTGROUP_ID == productGroupId
                && x.PRODUCT_ID == productId
                && x.CAMPAIGNSERVICE_ID == campaignServiceId
                && x.AREA_ID == areaId
                && x.SUBAREA_ID == subAreaId
                && x.TYPE_ID == typeId);
        }

        public SingleMapProductEntity GetSingleMapProduct(int? campaignServiceId, int? areaId, int? subAreaId, int? typeId)
        {
            var query = _context.TB_M_MAP_PRODUCT.AsQueryable();
            query = query.Where(q => q.MAP_PRODUCT_IS_ACTIVE);

            if (campaignServiceId.HasValue)
            {
                query = query.Where(q => q.CAMPAIGNSERVICE_ID == campaignServiceId);
            }

            if (areaId.HasValue)
            {
                query = query.Where(q => q.AREA_ID == areaId);
            }

            if (subAreaId.HasValue)
            {
                query = query.Where(q => q.SUBAREA_ID == subAreaId);
            }

            if (typeId.HasValue)
            {
                query = query.Where(q => q.TYPE_ID == typeId);
            }

            var list = query.ToList();

            if (list.Count() == 1)
            {
                var result = list.First();
                return new SingleMapProductEntity
                {
                    IsSingleQuery = true,
                    CampaignServiceId = result.CAMPAIGNSERVICE_ID,
                    CampaignServiceName = result.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_NAME,
                    AreaId = result.AREA_ID,
                    AreaName = result.TB_M_AREA.AREA_NAME,
                    SubAreaId = result.SUBAREA_ID,
                    SubAreaName = result.TB_M_SUBAREA.SUBAREA_NAME,
                    TypeId = result.TYPE_ID,
                    TypeName = result.TB_M_TYPE.TYPE_NAME,
                    ProductGroupName = result.TB_R_PRODUCT.TB_R_PRODUCTGROUP.PRODUCTGROUP_NAME,
                    ProductGroupId = result.TB_R_PRODUCT.PRODUCTGROUP_ID,
                    ProductName = result.TB_R_PRODUCT.PRODUCT_NAME,
                    ProductId = result.PRODUCT_ID
                };
            }
            else
            {
                return new SingleMapProductEntity
                {
                    IsSingleQuery = false
                };
            }
        }

        public SRValidateResult ValidateCreateSR(CreateSRRequest request)
        {
            string cusCardNo = request.CustomerCardNo;
            string cusSubsTypeCode = request.CustomerSubscriptionTypeCode;
            string cusAccountNo = request.AccountNo;
            string conCardNo = request.ContactCardNo;
            string conSubsTypeCode = request.ContactSubscriptionTypeCode;
            string conRelatName = request.ContactRelationshipName;
            string conAccountNo = request.ContactAccountNo;
            string campServCode = request.CampaignServiceCode;
            string areaCode = request.AreaCode.ConvertToString();
            string subAreaCode = request.SubAreaCode.ConvertToString();
            string typeCode = request.TypeCode.ConvertToString();
            string channelCode = request.ChannelCode;
            string mediaSrcName = request.MediaSourceName;
            string creatorEmpCode = request.CreatorEmployeeCode;
            string ownerEmpCode = request.OwnerEmployeeCode;
            string delegateEmpCode = request.DelegateEmployeeCode;
            string afsAssetNo = request.AFSAssetNo;
            string ncbEmpCode = request.NCBMarketingEmployeeCode;
            int srActTypeId = request.ActivityTypeId;

            var lst = _context.SP_VALIDATE_CREATE_SR(cusSubsTypeCode, cusCardNo, cusAccountNo, conSubsTypeCode, conCardNo, conRelatName, conAccountNo,
                campServCode, areaCode, subAreaCode, typeCode, channelCode, mediaSrcName, creatorEmpCode, ownerEmpCode, delegateEmpCode, afsAssetNo,
                ncbEmpCode, srActTypeId);

            var result = (from sr in lst
                          select new SRValidateResult
                          {
                              ErrorCode = sr.ErrorCode,
                              CustomerId = sr.CustomerId.Value,
                              AccountId = sr.AccountId.Value,
                              ContactId = sr.ContactId.Value,
                              ContactRelationshipId = sr.ContactRelationshipId.Value,
                              ProductGroupId = sr.ProductGroupId.Value,
                              ProductId = sr.ProductId.Value,
                              CampaignServiceId = sr.CampaignServiceId.Value,
                              AreaId = sr.AreaId.Value,
                              SubAreaId = sr.SubAreaId.Value,
                              TypeId = sr.TypeId.Value,
                              ChannelId = sr.ChannelId.Value,
                              MediaSourceId = sr.MediaSourceId.Value,
                              MapProductId = sr.MapProductId.Value,
                              IsVerify = (sr.IsVerify == 1),
                              SrPageId = sr.SrPageId.Value,
                              AfsAssetId = sr.AfsAssetId.Value,
                              AfsAssetNo = sr.AfsAssetNo,
                              AfsAssetDesc = sr.AfsAssetDesc,
                              NcbMarketingUserId = sr.NcbMarketingUserId.Value,
                              OwnerBranchId = sr.OwnerBranchId == -1 ? 0 : sr.OwnerBranchId.Value,
                              OwnerUserId = sr.OwnerUserId.Value,
                              DelegateBranchId = sr.DelegateBranchId.Value,
                              DelegateUserId = sr.DelegateUserId.Value,
                              CreatorBranchId = sr.CreatorBranchId.Value,
                              CreatorUserId = sr.CreatorUserId.Value
                          }).ToList();

            return result == null || result.Count == 0 ? null : result[0];
        }

        #region "Functions"

        private IQueryable<SubAreaItemEntity> SetSelectSubAreaListSort(IQueryable<SubAreaItemEntity> subAreaList, SelectSubAreaSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper().Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper())
                {
                    default:
                        return subAreaList.OrderBy(a => a.SubAreaName);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper())
                {
                    default:
                        return subAreaList.OrderByDescending(a => a.SubAreaName);
                }
            }
        }

        #endregion

        #region "Persistence"

        private int Save()
        {
            return _context.SaveChanges();
        }

        private void SetEntryCurrentValues(object entityTo, object entityFrom)
        {
            _context.Entry(entityTo).CurrentValues.SetValues(entityFrom);
            // Set state to Modified
            _context.Entry(entityTo).State = System.Data.Entity.EntityState.Modified;
        }

        private void SetEntryStateModified(object entity)
        {
            if (_context.Configuration.AutoDetectChangesEnabled == false)
            {
                // Set state to Modified
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
        }

        #endregion
    }
}
