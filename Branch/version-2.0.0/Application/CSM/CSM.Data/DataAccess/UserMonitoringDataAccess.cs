using System;
using System.Collections.Generic;
using System.Linq;
using CSM.Common.Utilities;
using CSM.Entity;
using log4net;
using System.Data.Entity.Core.Objects;
using System.Globalization;

namespace CSM.Data.DataAccess
{
    public class UserMonitoringDataAccess
    {
        private readonly CSMContext _context;

        public UserMonitoringDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        #region "Functions"

        public List<GroupAssignEntity> SearchGroupAssign(GroupAssignSearchFilter searchFilter)
        {
            //var branchIds = searchFilter.BranchIds.Split(',').Select(Int32.Parse).ToList();
            var ignoreStatusList = new int?[] { Constants.SRStatusId.Cancelled , Constants.SRStatusId.Closed };
            var branchIds = StringHelpers.ConvertStringToList(searchFilter.BranchIds, ',').Select(x => Int32.Parse((string)x)).ToList();

            var fromDate = searchFilter.AssignDateFromValue.HasValue ? searchFilter.AssignDateFromValue.Value.Date : new Nullable<DateTime>();
            var toDate = searchFilter.AssignDateToValue.HasValue ? searchFilter.AssignDateToValue.Value.Date : new Nullable<DateTime>();

            var query = from branch in _context.TB_R_BRANCH
                        from dummyUser in _context.TB_R_USER.Where(x => x.BRANCH_ID.HasValue && x.BRANCH_ID.Value == branch.BRANCH_ID && (x.IS_GROUP ?? false)).OrderByDescending(x => x.EMAIL).Take(1).DefaultIfEmpty()
                        where branchIds.Contains(branch.BRANCH_ID) && branch.STATUS == 1
                        select new GroupAssignEntity
                        {
                            BranchId = branch.BRANCH_ID,
                            BranchCode = branch.BRANCH_CODE,
                            BranchName = branch.BRANCH_NAME,
                            Email = dummyUser != null ? dummyUser.EMAIL : string.Empty,
                            OwnerSrList = (from sr in _context.TB_T_SR
                                           where sr.OWNER_USER_ID.HasValue
                                                 && _context.TB_R_USER.Where(x => (x.IS_GROUP ?? false) && x.BRANCH_ID == branch.BRANCH_ID).Select(x => x.USER_ID).Contains(sr.OWNER_USER_ID.Value)
                                                 && (!searchFilter.ProductId.HasValue || sr.PRODUCT_ID == searchFilter.ProductId.Value)
                                                 && (!searchFilter.CampaignServiceId.HasValue || sr.CAMPAIGNSERVICE_ID == searchFilter.CampaignServiceId.Value)
                                                 && (!searchFilter.TypeId.HasValue || sr.TYPE_ID == searchFilter.TypeId.Value)
                                                 && (!searchFilter.AreaId.HasValue || sr.AREA_ID == searchFilter.AreaId.Value)
                                                 && (!searchFilter.SubAreaId.HasValue || sr.SUBAREA_ID == searchFilter.SubAreaId.Value)
                                                 && (!fromDate.HasValue || EntityFunctions.TruncateTime(sr.CREATE_DATE.Value) >= fromDate.Value)
                                                 && (!toDate.HasValue || EntityFunctions.TruncateTime(sr.CREATE_DATE.Value) <= toDate.Value)
                                                 //&& sr.SR_STATUS_ID != Constants.SRStatusId.Cancelled
                                                 //&& sr.SR_STATUS_ID != Constants.SRStatusId.Closed
                                                 && !ignoreStatusList.Contains(sr.SR_STATUS_ID)
                                           select new ServiceRequestWithStatusEntity
                                           {
                                               SrId = sr.SR_ID,
                                               SrStatusCode = sr.TB_C_SR_STATUS.SR_STATUS_CODE,
                                               SrStateId = sr.TB_C_SR_STATUS.SR_STATE_ID
                                           }).ToList(),
                            DelegateSrList = (from sr in _context.TB_T_SR
                                              where sr.DELEGATE_USER_ID.HasValue
                                                    && _context.TB_R_USER.Where(x => (x.IS_GROUP ?? false) && x.BRANCH_ID == branch.BRANCH_ID).Select(x => x.USER_ID).Contains(sr.DELEGATE_USER_ID.Value)
                                                    && (!searchFilter.ProductId.HasValue || sr.PRODUCT_ID == searchFilter.ProductId.Value)
                                                    && (!searchFilter.CampaignServiceId.HasValue || sr.CAMPAIGNSERVICE_ID == searchFilter.CampaignServiceId.Value)
                                                    && (!searchFilter.TypeId.HasValue || sr.TYPE_ID == searchFilter.TypeId.Value)
                                                    && (!searchFilter.AreaId.HasValue || sr.AREA_ID == searchFilter.AreaId.Value)
                                                    && (!searchFilter.SubAreaId.HasValue || sr.SUBAREA_ID == searchFilter.SubAreaId.Value)
                                                    && (!fromDate.HasValue || EntityFunctions.TruncateTime(sr.CREATE_DATE.Value) >= fromDate.Value)
                                                    && (!toDate.HasValue || EntityFunctions.TruncateTime(sr.CREATE_DATE.Value) <= toDate.Value)
                                                    //&& sr.SR_STATUS_ID != Constants.SRStatusId.Cancelled
                                                    //&& sr.SR_STATUS_ID != Constants.SRStatusId.Closed
                                                    && !ignoreStatusList.Contains(sr.SR_STATUS_ID)
                                              select new ServiceRequestWithStatusEntity
                                              {
                                                  SrId = sr.SR_ID,
                                                  SrStatusCode = sr.TB_C_SR_STATUS.SR_STATUS_CODE,
                                                  SrStateId = sr.TB_C_SR_STATUS.SR_STATE_ID
                                              }).ToList(),
                            CreateSrList = (from sr in _context.TB_T_SR
                                            where sr.CREATE_USER.HasValue
                                                  && _context.TB_R_USER.Where(x => (x.IS_GROUP ?? false) && x.BRANCH_ID == branch.BRANCH_ID).Select(x => x.USER_ID).Contains(sr.CREATE_USER.Value)
                                                  && (!searchFilter.ProductId.HasValue || sr.PRODUCT_ID == searchFilter.ProductId.Value)
                                                  && (!searchFilter.CampaignServiceId.HasValue || sr.CAMPAIGNSERVICE_ID == searchFilter.CampaignServiceId.Value)
                                                  && (!searchFilter.TypeId.HasValue || sr.TYPE_ID == searchFilter.TypeId.Value)
                                                  && (!searchFilter.AreaId.HasValue || sr.AREA_ID == searchFilter.AreaId.Value)
                                                  && (!searchFilter.SubAreaId.HasValue || sr.SUBAREA_ID == searchFilter.SubAreaId.Value)
                                                  && (!fromDate.HasValue || EntityFunctions.TruncateTime(sr.CREATE_DATE.Value) >= fromDate.Value)
                                                  && (!toDate.HasValue || EntityFunctions.TruncateTime(sr.CREATE_DATE.Value) <= toDate.Value)
                                                  //&& sr.SR_STATUS_ID != Constants.SRStatusId.Cancelled
                                                  //&& sr.SR_STATUS_ID != Constants.SRStatusId.Closed
                                                  && !ignoreStatusList.Contains(sr.SR_STATUS_ID)
                                            select new ServiceRequestWithStatusEntity
                                            {
                                                SrId = sr.SR_ID,
                                                SrStatusCode = sr.TB_C_SR_STATUS.SR_STATUS_CODE,
                                                SrStateId = sr.TB_C_SR_STATUS.SR_STATE_ID
                                            }).ToList(),
                        };

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = query.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            query = SetGroupAssignSort(query, searchFilter);
            return query.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();
        }

        private static IQueryable<GroupAssignEntity> SetGroupAssignSort(IQueryable<GroupAssignEntity> resultList, GroupAssignSearchFilter searchFilter)
        {
            if (string.IsNullOrEmpty(searchFilter.SortOrder))
                searchFilter.SortOrder = "ASC";

            if (string.IsNullOrEmpty(searchFilter.SortField))
                searchFilter.SortField = "BranchCode";

            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    default:
                        return resultList.OrderBy(a => a.BranchCode);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    default:
                        return resultList.OrderByDescending(a => a.BranchCode);
                }
            }
        }

        public List<UserAssignEntity> SearchUserAssign(UserAssignSearchFilter searchFilter)
        {
            var currentUserId = searchFilter.CurrentUserId;
            //var userIds = searchFilter.UserIds.Split(',').Select(Int32.Parse).ToList();
            var ignoreStatusList = new int?[] { Constants.SRStatusId.Cancelled, Constants.SRStatusId.Closed };
            var userIds = StringHelpers.ConvertStringToList(searchFilter.UserIds, ',').Select(x => Int32.Parse((string)x)).ToList();

            var fromDate = searchFilter.AssignDateFromValue.HasValue ? searchFilter.AssignDateFromValue.Value.Date : new Nullable<DateTime>();
            var toDate = searchFilter.AssignDateToValue.HasValue ? searchFilter.AssignDateToValue.Value.Date : new Nullable<DateTime>();

            var query = from user in _context.TB_R_USER
                        from role in _context.TB_C_ROLE.Where(x => x.ROLE_ID == user.ROLE_ID).DefaultIfEmpty()
                        from branch in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == user.BRANCH_ID).DefaultIfEmpty()
                        where (userIds.Contains(user.USER_ID) && user.STATUS == 1 && user.IS_GROUP == false)
                              && (!searchFilter.BranchId.HasValue || user.BRANCH_ID == searchFilter.BranchId.Value)
                        select new UserAssignEntity()
                        {
                            IsCurrentUser = (user.USER_ID == currentUserId),
                            UserId = user.USER_ID,
                            RoleName = role.ROLE_NAME,
                            Username = user.USERNAME,
                            UserPositionCode = user.POSITION_CODE,
                            UserFirstname = user.FIRST_NAME,
                            UserLastname = user.LAST_NAME,
                            BranchName = branch.BRANCH_NAME,
                            OwnerSrList = (from sr in _context.TB_T_SR
                                           where sr.OWNER_USER_ID.HasValue && sr.OWNER_USER_ID.Value == user.USER_ID
                                                && (!searchFilter.ProductId.HasValue || sr.PRODUCT_ID == searchFilter.ProductId.Value)
                                                && (!searchFilter.CampaignServiceId.HasValue || sr.CAMPAIGNSERVICE_ID == searchFilter.CampaignServiceId.Value)
                                                && (!searchFilter.TypeId.HasValue || sr.TYPE_ID == searchFilter.TypeId.Value)
                                                && (!searchFilter.AreaId.HasValue || sr.AREA_ID == searchFilter.AreaId.Value)
                                                && (!searchFilter.SubAreaId.HasValue || sr.SUBAREA_ID == searchFilter.SubAreaId.Value)
                                                && (!fromDate.HasValue || EntityFunctions.TruncateTime(sr.CREATE_DATE.Value) >= fromDate.Value)
                                                && (!toDate.HasValue || EntityFunctions.TruncateTime(sr.CREATE_DATE.Value) <= toDate.Value)
                                                //&& sr.SR_STATUS_ID != Constants.SRStatusId.Cancelled
                                                //&& sr.SR_STATUS_ID != Constants.SRStatusId.Closed
                                                && !ignoreStatusList.Contains(sr.SR_STATUS_ID)
                                           select new ServiceRequestWithStatusEntity
                                           {
                                               SrId = sr.SR_ID,
                                               SrStatusCode = sr.TB_C_SR_STATUS.SR_STATUS_CODE,
                                               SrStateId = sr.TB_C_SR_STATUS.SR_STATE_ID
                                           }).ToList(),
                            DelegateSrList = (from sr in _context.TB_T_SR
                                              where sr.DELEGATE_USER_ID.HasValue && sr.DELEGATE_USER_ID.Value == user.USER_ID
                                                    && (!searchFilter.ProductId.HasValue || sr.PRODUCT_ID == searchFilter.ProductId.Value)
                                                    && (!searchFilter.CampaignServiceId.HasValue || sr.CAMPAIGNSERVICE_ID == searchFilter.CampaignServiceId.Value)
                                                    && (!searchFilter.TypeId.HasValue || sr.TYPE_ID == searchFilter.TypeId.Value)
                                                    && (!searchFilter.AreaId.HasValue || sr.AREA_ID == searchFilter.AreaId.Value)
                                                    && (!searchFilter.SubAreaId.HasValue || sr.SUBAREA_ID == searchFilter.SubAreaId.Value)
                                                    && (!fromDate.HasValue || EntityFunctions.TruncateTime(sr.CREATE_DATE.Value) >= fromDate.Value)
                                                    && (!toDate.HasValue || EntityFunctions.TruncateTime(sr.CREATE_DATE.Value) <= toDate.Value)
                                                    //&& sr.SR_STATUS_ID != Constants.SRStatusId.Cancelled
                                                    //&& sr.SR_STATUS_ID != Constants.SRStatusId.Closed
                                                    && !ignoreStatusList.Contains(sr.SR_STATUS_ID)
                                              select new ServiceRequestWithStatusEntity
                                              {
                                                  SrId = sr.SR_ID,
                                                  SrStatusCode = sr.TB_C_SR_STATUS.SR_STATUS_CODE,
                                                  SrStateId = sr.TB_C_SR_STATUS.SR_STATE_ID
                                              }).ToList(),
                            CreateSrList = (from sr in _context.TB_T_SR
                                            where sr.CREATE_USER.HasValue && sr.CREATE_USER.Value == user.USER_ID
                                                 && (!searchFilter.ProductId.HasValue || sr.PRODUCT_ID == searchFilter.ProductId.Value)
                                                 && (!searchFilter.CampaignServiceId.HasValue || sr.CAMPAIGNSERVICE_ID == searchFilter.CampaignServiceId.Value)
                                                 && (!searchFilter.TypeId.HasValue || sr.TYPE_ID == searchFilter.TypeId.Value)
                                                 && (!searchFilter.AreaId.HasValue || sr.AREA_ID == searchFilter.AreaId.Value)
                                                 && (!searchFilter.SubAreaId.HasValue || sr.SUBAREA_ID == searchFilter.SubAreaId.Value)
                                                 && (!fromDate.HasValue || EntityFunctions.TruncateTime(sr.CREATE_DATE.Value) >= fromDate.Value)
                                                 && (!toDate.HasValue || EntityFunctions.TruncateTime(sr.CREATE_DATE.Value) <= toDate.Value)
                                                 //&& sr.SR_STATUS_ID != Constants.SRStatusId.Cancelled
                                                 //&& sr.SR_STATUS_ID != Constants.SRStatusId.Closed
                                                 && !ignoreStatusList.Contains(sr.SR_STATUS_ID)
                                            select new ServiceRequestWithStatusEntity
                                            {
                                                SrId = sr.SR_ID,
                                                SrStatusCode = sr.TB_C_SR_STATUS.SR_STATUS_CODE,
                                                SrStateId = sr.TB_C_SR_STATUS.SR_STATE_ID
                                            }).ToList(),
                        };

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = query.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            query = SetUserAssignSort(query, searchFilter);
            return query.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();
        }

        private static IQueryable<UserAssignEntity> SetUserAssignSort(IQueryable<UserAssignEntity> resultList, UserAssignSearchFilter searchFilter)
        {
            if (string.IsNullOrEmpty(searchFilter.SortOrder))
                searchFilter.SortOrder = "ASC";

            if (string.IsNullOrEmpty(searchFilter.SortField))
                searchFilter.SortField = "UserFirstname";

            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    default:
                        return resultList.OrderByDescending(a => a.IsCurrentUser).ThenBy(a => a.UserFirstname);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    default:
                        return resultList.OrderByDescending(a => a.UserFirstname);
                }
            }
        }

        public List<ServiceRequestEntity> SearchServiceRequest(UserMonitoringSrSearchFilter searchFilter)
        {
            var userIds = GetFilterUserId(searchFilter);
            var ignoreStatusList = new string[] { Constants.SRStatusCode.Closed, Constants.SRStatusCode.Cancelled, Constants.SRStatusCode.Draft };
            var ignoreStateList = new int?[] { Constants.SRStateId.Draft, Constants.SRStateId.Cancelled, Constants.SRStateId.Closed };

            var fromDate = searchFilter.AssignDateFromValue.HasValue ? searchFilter.AssignDateFromValue.Value.Date : new Nullable<DateTime>();
            var toDate = searchFilter.AssignDateToValue.HasValue ? searchFilter.AssignDateToValue.Value.Date : new Nullable<DateTime>();

            var query = (from sr in _context.TB_T_SR
                         from ownerUser in _context.TB_R_USER.Where(user => sr.OWNER_USER_ID == user.USER_ID).DefaultIfEmpty()
                         from delegateUser in _context.TB_R_USER.Where(user => sr.DELEGATE_USER_ID == user.USER_ID).DefaultIfEmpty()
                         from status in _context.TB_C_SR_STATUS.Where(x => x.SR_STATUS_ID == sr.SR_STATUS_ID).DefaultIfEmpty()
                         where sr.OWNER_USER_ID.HasValue && userIds.Contains(sr.OWNER_USER_ID.Value)
                              && (!searchFilter.ProductId.HasValue || sr.PRODUCT_ID == searchFilter.ProductId.Value)
                              && (!searchFilter.CampaignServiceId.HasValue || sr.CAMPAIGNSERVICE_ID == searchFilter.CampaignServiceId.Value)
                              && (!searchFilter.TypeId.HasValue || sr.TYPE_ID == searchFilter.TypeId.Value)
                              && (!searchFilter.AreaId.HasValue || sr.AREA_ID == searchFilter.AreaId.Value)
                              && (!searchFilter.SubAreaId.HasValue || sr.SUBAREA_ID == searchFilter.SubAreaId.Value)
                              && (!fromDate.HasValue || EntityFunctions.TruncateTime(sr.CREATE_DATE.Value) >= fromDate.Value)
                              && (!toDate.HasValue || EntityFunctions.TruncateTime(sr.CREATE_DATE.Value) <= toDate.Value)
                                //&&
                                //(
                                //    (!string.IsNullOrEmpty(searchFilter.StatusCode) && status.SR_STATUS_CODE == searchFilter.StatusCode)
                                //    ||
                                //    (string.IsNullOrEmpty(searchFilter.StatusCode) && !ignoreStatusList.Contains(status.SR_STATUS_CODE))
                                //      //&& status.SR_STATUS_CODE != Constants.SRStatusCode.Closed && status.SR_STATUS_CODE != Constants.SRStatusCode.Cancelled
                                //)
                              && (
                                  (searchFilter.StateId.HasValue && status.SR_STATE_ID == searchFilter.StateId)
                                  ||
                                  (!searchFilter.StateId.HasValue && !ignoreStateList.Contains(status.SR_STATE_ID))
                              )
                         select new ServiceRequestEntity
                         {
                             TransferType = "OWNER",
                             SrId = sr.SR_ID,
                             SrNo = sr.SR_NO,
                             ThisAlert = sr.RULE_THIS_ALERT,
                             NextSLA = sr.RULE_NEXT_SLA,
                             TotalWorkingHours = sr.RULE_TOTAL_WORK,
                             CustomerCardNo = sr.TB_M_CUSTOMER.CARD_NO,
                             ChannelId = sr.CHANNEL_ID,
                             ChannelName = sr.TB_R_CHANNEL.CHANNEL_NAME,
                             ProductId = sr.PRODUCT_ID,
                             ProductName = sr.TB_R_PRODUCT.PRODUCT_NAME,
                             CampaignServiceId = sr.CAMPAIGNSERVICE_ID,
                             CampaignServiceName = sr.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_NAME,
                             TypeId = sr.TB_M_TYPE.TYPE_ID,
                             TypeName = sr.TB_M_TYPE.TYPE_NAME,
                             AreaId = sr.AREA_ID,
                             AreaName = sr.TB_M_AREA.AREA_NAME,
                             SubAreaId = sr.SUBAREA_ID,
                             SubAreaName = sr.TB_M_SUBAREA.SUBAREA_NAME,
                             Subject = sr.SR_SUBJECT,
                             SrStatusName = sr.TB_C_SR_STATUS.SR_STATUS_NAME,
                             SRStateName = _context.TB_C_SR_STATE.Where(x => x.SR_STATE_ID == sr.TB_C_SR_STATUS.SR_STATE_ID).FirstOrDefault().SR_STATE_NAME,
                             CreateDate = sr.CREATE_DATE,
                             ClosedDate = sr.CLOSE_DATE,
                             OwnerUserId = ownerUser != null ? ownerUser.USER_ID : (int?)null,
                             OwnerUserPosition = ownerUser != null ? ownerUser.POSITION_CODE : null,
                             OwnerUserFirstName = ownerUser != null ? ownerUser.FIRST_NAME : null,
                             OwnerUserLastName = ownerUser != null ? ownerUser.LAST_NAME : null,
                             DelegateUserId = delegateUser != null ? delegateUser.USER_ID : (int?)null,
                             DelegateUserPosition = delegateUser != null ? delegateUser.POSITION_CODE : null,
                             DelegateUserFirstName = delegateUser != null ? delegateUser.FIRST_NAME : null,
                             DelegateUserLastName = delegateUser != null ? delegateUser.LAST_NAME : null,
                             ANo = sr.SR_ANO,
                         }).Concat(from sr in _context.TB_T_SR
                                   from ownerUser in _context.TB_R_USER.Where(user => sr.OWNER_USER_ID == user.USER_ID).DefaultIfEmpty()
                                   from delegateUser in _context.TB_R_USER.Where(user => sr.DELEGATE_USER_ID == user.USER_ID).DefaultIfEmpty()
                                   from status in _context.TB_C_SR_STATUS.Where(x => x.SR_STATUS_ID == sr.SR_STATUS_ID).DefaultIfEmpty()
                                   where sr.DELEGATE_USER_ID.HasValue && userIds.Contains(sr.DELEGATE_USER_ID.Value)
                                         && (!searchFilter.ProductId.HasValue || sr.PRODUCT_ID == searchFilter.ProductId.Value)
                                         && (!searchFilter.CampaignServiceId.HasValue || sr.CAMPAIGNSERVICE_ID == searchFilter.CampaignServiceId.Value)
                                         && (!searchFilter.TypeId.HasValue || sr.TYPE_ID == searchFilter.TypeId.Value)
                                         && (!searchFilter.AreaId.HasValue || sr.AREA_ID == searchFilter.AreaId.Value)
                                         && (!searchFilter.SubAreaId.HasValue || sr.SUBAREA_ID == searchFilter.SubAreaId.Value)
                                         && (!fromDate.HasValue || EntityFunctions.TruncateTime(sr.CREATE_DATE.Value) >= fromDate.Value)
                                         && (!toDate.HasValue || EntityFunctions.TruncateTime(sr.CREATE_DATE.Value) <= toDate.Value)
                                          //&&
                                          //(
                                          //    (!string.IsNullOrEmpty(searchFilter.StatusCode) && status.SR_STATUS_CODE == searchFilter.StatusCode)
                                          //    ||
                                          //    (string.IsNullOrEmpty(searchFilter.StatusCode) && status.SR_STATUS_CODE != Constants.SRStatusCode.Closed && status.SR_STATUS_CODE != Constants.SRStatusCode.Cancelled)
                                          //)
                                         && (
                                              (searchFilter.StateId.HasValue && status.SR_STATE_ID == searchFilter.StateId)
                                              ||
                                              (!searchFilter.StateId.HasValue && !ignoreStateList.Contains(status.SR_STATE_ID))
                                          )
                                   select new ServiceRequestEntity
                                   {
                                       TransferType = "DELEGATE",
                                       SrId = sr.SR_ID,
                                       SrNo = sr.SR_NO,
                                       ThisAlert = sr.RULE_THIS_ALERT,
                                       NextSLA = sr.RULE_NEXT_SLA,
                                       TotalWorkingHours = sr.RULE_TOTAL_WORK,
                                       CustomerCardNo = sr.TB_M_CUSTOMER.CARD_NO,
                                       ChannelId = sr.CHANNEL_ID,
                                       ChannelName = sr.TB_R_CHANNEL.CHANNEL_NAME,
                                       ProductId = sr.PRODUCT_ID,
                                       ProductName = sr.TB_R_PRODUCT.PRODUCT_NAME,
                                       CampaignServiceId = sr.CAMPAIGNSERVICE_ID,
                                       CampaignServiceName = sr.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_NAME,
                                       TypeId = sr.TB_M_TYPE.TYPE_ID,
                                       TypeName = sr.TB_M_TYPE.TYPE_NAME,
                                       AreaId = sr.AREA_ID,
                                       AreaName = sr.TB_M_AREA.AREA_NAME,
                                       SubAreaId = sr.SUBAREA_ID,
                                       SubAreaName = sr.TB_M_SUBAREA.SUBAREA_NAME,
                                       Subject = sr.SR_SUBJECT,
                                       SrStatusName = sr.TB_C_SR_STATUS.SR_STATUS_NAME,
                                       SRStateName = _context.TB_C_SR_STATE.Where(x => x.SR_STATE_ID == sr.TB_C_SR_STATUS.SR_STATE_ID).FirstOrDefault().SR_STATE_NAME,
                                       CreateDate = sr.CREATE_DATE,
                                       ClosedDate = sr.CLOSE_DATE,
                                       OwnerUserId = ownerUser != null ? ownerUser.USER_ID : (int?)null,
                                       OwnerUserPosition = ownerUser != null ? ownerUser.POSITION_CODE : null,
                                       OwnerUserFirstName = ownerUser != null ? ownerUser.FIRST_NAME : null,
                                       OwnerUserLastName = ownerUser != null ? ownerUser.LAST_NAME : null,
                                       DelegateUserId = delegateUser != null ? delegateUser.USER_ID : (int?)null,
                                       DelegateUserPosition = delegateUser != null ? delegateUser.POSITION_CODE : null,
                                       DelegateUserFirstName = delegateUser != null ? delegateUser.FIRST_NAME : null,
                                       DelegateUserLastName = delegateUser != null ? delegateUser.LAST_NAME : null,
                                       ANo = sr.SR_ANO
                                   });

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = query.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            query = SetServiceRequestSort(query, searchFilter);
            return query.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<ServiceRequestEntity>();
        }

        public List<int> GetFilterUserId(UserMonitoringSrSearchFilter searchFilter)
        {
            if (searchFilter.ViewType.ToUpper(CultureInfo.InvariantCulture) == "BRANCH" && searchFilter.BranchId.HasValue)
            {
                return _context.TB_R_USER.Where(x => x.STATUS == 1 && (x.IS_GROUP ?? false) && x.BRANCH_ID == searchFilter.BranchId.Value).Select(x => x.USER_ID).ToList();
            }
            else if (searchFilter.ViewType.ToUpper(CultureInfo.InvariantCulture) == "USER" && searchFilter.UserId.HasValue)
            {
                return new List<int> { searchFilter.UserId.Value };
            }
            else
            {
                return new List<int>();
            }
        }

        private static IQueryable<ServiceRequestEntity> SetServiceRequestSort(IQueryable<ServiceRequestEntity> resultList, UserMonitoringSrSearchFilter searchFilter)
        {
            if (string.IsNullOrEmpty(searchFilter.SortOrder))
                searchFilter.SortOrder = "ASC";

            if (string.IsNullOrEmpty(searchFilter.SortField))
                searchFilter.SortField = "ThisAlert";

            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField)
                {
                    case "ProductName":
                        return resultList.OrderBy(a => a.ProductName);
                    case "AreaName":
                        return resultList.OrderBy(a => a.AreaName);
                    case "SubAreaName":
                        return resultList.OrderBy(a => a.SubAreaName);
                    case "SrStatus":
                        return resultList.OrderBy(a => a.SrStatusName);
                    case "CreateDate":
                        return resultList.OrderBy(a => a.CreateDate);
                    case "CloseDated":
                        return resultList.OrderBy(a => a.ClosedDate);
                    case "OwnerUserFullname":
                        return resultList.OrderBy(a => a.OwnerUserFullname);
                    case "DelegateUserFullname":
                        return resultList.OrderBy(a => a.DelegateUserFullname);

                    default:
                        return resultList.OrderByDescending(a => a.ThisAlert);
                }
            }
            else
            {
                switch (searchFilter.SortField)
                {
                    case "ProductName":
                        return resultList.OrderByDescending(a => a.ProductName);
                    case "AreaName":
                        return resultList.OrderByDescending(a => a.AreaName);
                    case "SubAreaName":
                        return resultList.OrderByDescending(a => a.SubAreaName);
                    case "SrStatus":
                        return resultList.OrderByDescending(a => a.SrStatusName);
                    case "CreateDate":
                        return resultList.OrderByDescending(a => a.CreateDate);
                    case "CloseDated":
                        return resultList.OrderByDescending(a => a.ClosedDate);
                    case "OwnerUserFullname":
                        return resultList.OrderByDescending(a => a.OwnerUserFullname);
                    case "DelegateUserFullname":
                        return resultList.OrderByDescending(a => a.DelegateUserFullname);

                    default:
                        return resultList.OrderBy(a => a.ThisAlert);
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
