using System;
using System.Collections.Generic;
using System.Linq;
using CSM.Common.Utilities;
using CSM.Entity;
using System.Globalization;

namespace CSM.Data.DataAccess
{
    public class SlaDataAccess : ISlaDataAccess
    {
        private readonly CSMContext _context;

        public SlaDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        #region "SLA"

        public bool ValidateSla(int? slaId, int productId, int? campaignServiceId, int areaId, int? subAreaId,
            int typeId, int channelId, int srStatusId)
        {
            if (!slaId.HasValue)
            {
                //add mode
                return _context.TB_M_SLA.Count(q => q.PRODUCT_ID == productId 
                    && q.CAMPAIGNSERVICE_ID == campaignServiceId.Value
                    && q.AREA_ID == areaId
                    && q.SUBAREA_ID == subAreaId.Value
                    && q.TYPE_ID == typeId
                    && q.CHANNEL_ID == channelId
                    && q.SR_STATUS_ID == srStatusId) == 0;
            }
            else
            {
                //edit mode
                return _context.TB_M_SLA.Count(q => q.SLA_ID != slaId.Value
                    && q.PRODUCT_ID == productId
                    && q.CAMPAIGNSERVICE_ID == campaignServiceId
                    && q.AREA_ID == areaId
                    && q.SUBAREA_ID == subAreaId
                    && q.TYPE_ID == typeId
                    && q.CHANNEL_ID == channelId
                    && q.SR_STATUS_ID == srStatusId) == 0;
            }
        }

        public void SaveSla(SlaItemEntity slaItemEntity)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var isEdit = slaItemEntity.SlaId.HasValue;
                TB_M_SLA sla;

                if (!isEdit)
                {
                    //add
                    sla = new TB_M_SLA();
                }
                else
                {
                    //edit
                    sla = _context.TB_M_SLA.SingleOrDefault(s => s.SLA_ID == slaItemEntity.SlaId.Value);
                    if (sla == null)
                        throw new CustomException("Cannot Update SLA: ID={0} does not exist", slaItemEntity.AreaId);
                }

                sla.PRODUCT_ID = slaItemEntity.ProductId;
                sla.CAMPAIGNSERVICE_ID = slaItemEntity.CampaignServiceId;
                sla.AREA_ID = slaItemEntity.AreaId;
                sla.SUBAREA_ID = slaItemEntity.SubAreaId;
                sla.TYPE_ID = slaItemEntity.TypeId;
                sla.CHANNEL_ID = slaItemEntity.ChannelId;
                sla.SR_STATUS_ID = slaItemEntity.SrStatusId;
                sla.SLA_MINUTE = slaItemEntity.SlaMinute;
                sla.SLA_TIMES = slaItemEntity.SlaTimes;

                sla.ALERT_CHIEF_TIMES = slaItemEntity.AlertChiefTimes;
                sla.ALERT_CHIEF1_TIMES = slaItemEntity.AlertChief1Times;
                sla.ALERT_CHIEF2_TIMES = slaItemEntity.AlertChief2Times;
                sla.ALERT_CHIEF3_TIMES = slaItemEntity.AlertChief3Times;

                sla.SLA_DAY = slaItemEntity.SlaDay;
                sla.SLA_IS_ACTIVE = true;
                sla.UPDATE_DATE = DateTime.Now;
                sla.UPDATE_USER = slaItemEntity.UserId;

                if (!isEdit)
                {
                    sla.CREATE_DATE = DateTime.Now;
                    sla.CREATE_USER = slaItemEntity.UserId;
                    _context.TB_M_SLA.Add(sla);
                }
                else
                {
                    SetEntryStateModified(sla);
                }

                Save();
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public SlaItemEntity GetSlaById(int? slaId)
        {
            var areaItemEntity = (from sla in _context.TB_M_SLA
                from createUser in _context.TB_R_USER.Where(c => c.USER_ID == sla.CREATE_USER).DefaultIfEmpty()
                from updateUser in _context.TB_R_USER.Where(u => u.USER_ID == sla.UPDATE_USER).DefaultIfEmpty()
                from st in _context.TB_C_SR_STATUS.Where(u => u.SR_STATUS_ID == sla.SR_STATUS_ID).DefaultIfEmpty()
                from ste in _context.TB_C_SR_STATE.Where(u => u.SR_STATE_ID == st.SR_STATE_ID).DefaultIfEmpty()
                where (sla.SLA_ID == slaId)
                select new SlaItemEntity()
                {
                    SlaId = sla.SLA_ID,
                    ProductGroupId = sla.TB_R_PRODUCT.PRODUCTGROUP_ID,
                    ProductGroupName = sla.TB_R_PRODUCT.TB_R_PRODUCTGROUP.PRODUCTGROUP_NAME,
                    ProductId = sla.PRODUCT_ID,
                    ProductName = sla.TB_R_PRODUCT.PRODUCT_NAME,
                    CampaignServiceId = sla.CAMPAIGNSERVICE_ID,
                    CampaignName = sla.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_NAME,
                    TypeId = sla.TYPE_ID,
                    TypeName = sla.TB_M_TYPE.TYPE_NAME,
                    AreaId = sla.AREA_ID,
                    AreaName = sla.TB_M_AREA.AREA_NAME,
                    SubAreaId = sla.SUBAREA_ID,
                    SubAreaName = sla.TB_M_SUBAREA.SUBAREA_NAME,
                    ChannelId = sla.CHANNEL_ID,
                    SrStatusId = sla.SR_STATUS_ID,
                    StatusName = st.SR_STATUS_NAME,
                    IsActive = sla.SLA_IS_ACTIVE,
                    SrStateId = ste.SR_STATE_ID,
                    StateName = ste.SR_STATE_NAME,
                    SlaMinute = sla.SLA_MINUTE,
                    SlaTimes = sla.SLA_TIMES,
                    SlaDay = sla.SLA_DAY,
                    AlertChiefTimes = sla.ALERT_CHIEF_TIMES,
                    AlertChief1Times = sla.ALERT_CHIEF1_TIMES,
                    AlertChief2Times = sla.ALERT_CHIEF2_TIMES,
                    AlertChief3Times = sla.ALERT_CHIEF3_TIMES,
                    CreateUser = (createUser != null
                        ? new UserEntity
                        {
                            PositionCode = createUser.POSITION_CODE,
                            Firstname = createUser.FIRST_NAME,
                            Lastname = createUser.LAST_NAME
                        }
                        : null),
                    CreateDate = sla.CREATE_DATE,
                    UpdateUser = (updateUser != null ? new UserEntity
                    {
                        PositionCode = updateUser.POSITION_CODE,
                        Firstname = updateUser.FIRST_NAME,
                        Lastname = updateUser.LAST_NAME
                    } : null),
                    UpdateDate = sla.UPDATE_DATE
                }).SingleOrDefault();

            return areaItemEntity;
        }

        public IEnumerable<SlaItemEntity> GetSlaList(SlaSearchFilter searchFilter)
        {
            var resultQuery = (from sla in _context.TB_M_SLA
                               from createUser in _context.TB_R_USER.Where(x => x.USER_ID == sla.CREATE_USER).DefaultIfEmpty()
                               from updateUser in _context.TB_R_USER.Where(x => x.USER_ID == sla.UPDATE_USER).DefaultIfEmpty()
                               from st in _context.TB_C_SR_STATUS.Where(x => x.SR_STATUS_ID == sla.SR_STATUS_ID).DefaultIfEmpty()
                               from ste in _context.TB_C_SR_STATE.Where(x => x.SR_STATE_ID == st.SR_STATE_ID).DefaultIfEmpty()
                               where ((!searchFilter.ProductGroupId.HasValue || sla.TB_R_PRODUCT.PRODUCTGROUP_ID == searchFilter.ProductGroupId) &&
                                      (!searchFilter.ProductId.HasValue || sla.PRODUCT_ID == searchFilter.ProductId) &&
                                      (!searchFilter.CampaignServiceId.HasValue || sla.CAMPAIGNSERVICE_ID == searchFilter.CampaignServiceId) &&
                                      (!searchFilter.AreaId.HasValue || sla.AREA_ID == searchFilter.AreaId) &&
                                      (!searchFilter.SubAreaId.HasValue || sla.SUBAREA_ID == searchFilter.SubAreaId) &&
                                      (!searchFilter.TypeId.HasValue || sla.TYPE_ID == searchFilter.TypeId) &&
                                      (!searchFilter.ChannelId.HasValue || searchFilter.ChannelId == Constants.ApplicationStatus.All || searchFilter.ChannelId == sla.CHANNEL_ID) &&
                                      (!searchFilter.SrStatusId.HasValue || searchFilter.SrStatusId == Constants.ApplicationStatus.All || searchFilter.SrStatusId == sla.SR_STATUS_ID) &&
                                      (!searchFilter.SrStateId.HasValue || searchFilter.SrStateId == Constants.ApplicationStatus.All
                                        || ste.SR_STATE_ID == searchFilter.SrStateId))
                               select new SlaItemEntity
                               {
                                   SlaId = sla.SLA_ID,
                                   ProductGroupName = sla.TB_R_PRODUCT.TB_R_PRODUCTGROUP.PRODUCTGROUP_NAME,
                                   ProductName = sla.TB_R_PRODUCT.PRODUCT_NAME,
                                   CampaignName = sla.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_NAME,
                                   TypeName = sla.TB_M_TYPE.TYPE_NAME,
                                   AreaName = sla.TB_M_AREA.AREA_NAME,
                                   SubAreaName = sla.TB_M_SUBAREA.SUBAREA_NAME,
                                   ChannelName = sla.TB_R_CHANNEL.CHANNEL_NAME,
                                   IsActive = sla.SLA_IS_ACTIVE,
                                   StateName = ste.SR_STATE_NAME,
                                   StatusName = sla.TB_C_SR_STATUS.SR_STATUS_NAME,
                                   SlaMinute = sla.SLA_MINUTE,
                                   SlaTimes = sla.SLA_TIMES,
                                   AlertChiefTimes = sla.ALERT_CHIEF_TIMES,
                                   AlertChief1Times = sla.ALERT_CHIEF1_TIMES,
                                   AlertChief2Times = sla.ALERT_CHIEF2_TIMES,
                                   AlertChief3Times = sla.ALERT_CHIEF3_TIMES,
                                   SlaDay = sla.SLA_DAY,
                                   UpdateUser = (updateUser != null ? new UserEntity
                                   {
                                       PositionCode = updateUser.POSITION_CODE,
                                       Firstname = updateUser.FIRST_NAME,
                                       Lastname = updateUser.LAST_NAME
                                   } : null),
                                   UpdateDate = sla.UPDATE_DATE
                               });

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = resultQuery.Count();

            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            resultQuery = SetSlaListSort(resultQuery, searchFilter);
            return resultQuery.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();
        }

        public bool DeleteSla(int slaId)
        {
            var item = _context.TB_M_SLA.SingleOrDefault(i => i.SLA_ID == slaId);
            if (item == null)
                return false;

            _context.TB_M_SLA.Remove(item);
            Save();

            return true;
        }

        #endregion

        #region "Functions"
        
        private static IQueryable<SlaItemEntity> SetSlaListSort(IQueryable<SlaItemEntity> slaList, SlaSearchFilter searchFilter)
        {
            if (searchFilter.SortField == null)
                searchFilter.SortField = "";

            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "ProductGroup":
                        return slaList.OrderBy(q => q.ProductName);
                    case "Product":
                        return slaList.OrderBy(q => q.ProductName);
                    case "Campaign":
                        return slaList.OrderBy(q => q.CampaignName);
                    case "Type":
                        return slaList.OrderBy(q => q.TypeName);
                    case "Area":
                        return slaList.OrderBy(q => q.AreaName);
                    case "SubArea":
                        return slaList.OrderBy(q => q.SubAreaName);
                    default:
                        return
                            slaList.OrderBy(a => a.ProductGroupName)
                                .ThenBy(a => a.ProductName)
                                .ThenBy(a => a.CampaignName)
                                .ThenBy(a => a.TypeName)
                                .ThenBy(a => a.AreaName)
                                .ThenBy(a => a.SubAreaName);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "ProductGroup":
                        return slaList.OrderByDescending(q => q.ProductName);
                    case "Product":
                        return slaList.OrderByDescending(q => q.ProductName);
                    case "Campaign":
                        return slaList.OrderByDescending(q => q.CampaignName);
                    case "Type":
                        return slaList.OrderByDescending(q => q.TypeName);
                    case "Area":
                        return slaList.OrderByDescending(q => q.AreaName);
                    case "SubArea":
                        return slaList.OrderByDescending(q => q.SubAreaName);
                    default:
                        return
                            slaList.OrderByDescending(a => a.ProductGroupName)
                                .ThenByDescending(a => a.ProductName)
                                .ThenByDescending(a => a.CampaignName)
                                .ThenByDescending(a => a.TypeName)
                                .ThenByDescending(a => a.AreaName)
                                .ThenByDescending(a => a.SubAreaName);
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
