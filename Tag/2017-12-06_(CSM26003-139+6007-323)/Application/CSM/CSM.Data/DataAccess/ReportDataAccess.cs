using System;
using System.Data.Entity;
using System.Linq;
using CSM.Common.Resources;
using CSM.Common.Utilities;
using CSM.Entity;
using log4net;
using System.Collections.Generic;

namespace CSM.Data.DataAccess
{
    public class ReportDataAccess : IReportDataAccess
    {
        private readonly CSMContext _context;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReportDataAccess));

        public ReportDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        public IList<ExportJobsEntity> GetExportJobs(ExportJobsSearchFilter searchFilter)
        {
            int? attachFile = searchFilter.AttachFile.ToNullable<int>();
            int? OwnerBranchId = searchFilter.OwnerBranch.ToNullable<int>();
            int? CreatorBranchId = searchFilter.CreatorBranch.ToNullable<int>();
            int? OwnerId = searchFilter.OwnerSR.ToNullable<int>();
            int? CreatorId = searchFilter.CreatorSR.ToNullable<int>();
            int? ActionId = searchFilter.ActionBy.ToNullable<int>();
            int? ActionBranchId = searchFilter.ActionBranch.ToNullable<int>();
            int? BranchId = searchFilter.LoginUser.BranchId;

            DateTime? jobMinDate = null;
            DateTime? jobMaxDate = null;

            if (searchFilter.JobDateTimeFromValue.HasValue && searchFilter.JobDateTimeToValue.HasValue)
            {
                jobMinDate = searchFilter.JobDateTimeFromValue.Value;
                jobMaxDate = searchFilter.JobDateTimeToValue.Value;
            }
            else
            {
                int? monthOfReportExport =
                    _context.TB_C_PARAMETER.Where(x => x.PARAMETER_NAME == Constants.ParameterName.ReportExportDate)
                        .FirstOrDefault().PARAMETER_VALUE.ToNullable<int>();

                jobMinDate = DateTime.Now.Date.AddMonths(monthOfReportExport.Value * -1);
                jobMaxDate = DateTime.Now.Date.AddDays(1);
            }


            var query = (from jb in _context.TB_T_JOB.AsNoTracking()
                         from u2 in _context.TB_R_USER.Where(u => u.USER_ID == jb.UPDATE_USER).DefaultIfEmpty()
                         from ab in _context.TB_R_BRANCH.Where(u => u.BRANCH_ID == u2.BRANCH_ID).DefaultIfEmpty()
                         from pb in _context.TB_M_POOL_BRANCH.Where(u => u.POOL_ID == jb.POOL_ID).DefaultIfEmpty()
                         from pl in _context.TB_M_POOL.Where(u => u.POOL_ID == jb.POOL_ID).DefaultIfEmpty()

                         from sr in _context.TB_T_SR.Where(s => s.SR_ID == jb.SR_ID).DefaultIfEmpty()
                         from cs in _context.TB_M_CUSTOMER.Where(c => c.CUSTOMER_ID == sr.CUSTOMER_ID).DefaultIfEmpty()
                         from u1 in _context.TB_R_USER.Where(u => u.USER_ID == sr.CREATE_USER).DefaultIfEmpty()
                         from u3 in _context.TB_R_USER.Where(u => u.USER_ID == sr.OWNER_USER_ID).DefaultIfEmpty()
                         from js in _context.TB_C_JOB_STATUS.Where(j => j.STATUS_VALUE == jb.JOB_STATUS).DefaultIfEmpty()
                         from st in _context.TB_C_SR_STATUS.Where(u => u.SR_STATUS_ID == sr.SR_STATUS_ID).DefaultIfEmpty()
                         from ste in _context.TB_C_SR_STATE.Where(u => u.SR_STATE_ID == st.SR_STATE_ID).DefaultIfEmpty()

                         join ch in _context.TB_R_CHANNEL on jb.CHANNEL_ID equals ch.CHANNEL_ID
                         where
                         (string.IsNullOrEmpty(searchFilter.FirstName) || (cs.FIRST_NAME_TH.Contains(searchFilter.FirstName) || cs.FIRST_NAME_EN.ToUpper().Contains(searchFilter.FirstName.ToUpper())))
                         && (string.IsNullOrEmpty(searchFilter.LastName) || (cs.LAST_NAME_TH.Contains(searchFilter.LastName) || cs.LAST_NAME_TH.ToUpper().Contains(searchFilter.LastName.ToUpper())))
                          && (searchFilter.JobStatus == null || searchFilter.JobStatus == Constants.ApplicationStatus.All || jb.JOB_STATUS == searchFilter.JobStatus)
                          && (string.IsNullOrEmpty(searchFilter.FromValue) || jb.FROM.ToUpper().Contains(searchFilter.FromValue.ToUpper()))
                          && (string.IsNullOrEmpty(searchFilter.Subject) || jb.SUBJECT.ToUpper().Contains(searchFilter.Subject.ToUpper()))
                         && (jb.JOB_DATE >= jobMinDate)
                         && (jb.JOB_DATE <= jobMaxDate)
                         && (!searchFilter.ActionDateTimeFromValue.HasValue || jb.UPDATE_DATE >= searchFilter.ActionDateTimeFromValue.Value)
                         && (!searchFilter.ActionDateTimeToValue.HasValue || jb.UPDATE_DATE <= searchFilter.ActionDateTimeToValue.Value)
                         && (attachFile == null || attachFile == Constants.ApplicationStatus.All
                                || (Constants.AttachFile.Yes == attachFile ? jb.TB_T_JOB_ATTACHMENT.Any() == true : jb.TB_T_JOB_ATTACHMENT.Any() == false))

                         && (string.IsNullOrEmpty(searchFilter.SRId) || sr.SR_NO == searchFilter.SRId)
                         && (string.IsNullOrEmpty(searchFilter.OwnerBranch) || sr.OWNER_BRANCH_ID == OwnerBranchId)
                         && (string.IsNullOrEmpty(searchFilter.CreatorBranch) || sr.CREATE_BRANCH_ID == CreatorBranchId)
                         && (string.IsNullOrEmpty(searchFilter.OwnerSR) || sr.OWNER_USER_ID == OwnerId)
                         && (string.IsNullOrEmpty(searchFilter.CreatorSR) || sr.CREATE_USER == CreatorId)
                         && (string.IsNullOrEmpty(searchFilter.ActionBy) || jb.UPDATE_USER == ActionId)
                         && (string.IsNullOrEmpty(searchFilter.ActionBranch) || ab.BRANCH_ID == ActionBranchId)
                         && pb.BRANCH_ID == BranchId
                           && jb.CHANNEL_ID != null
                         select new ExportJobsEntity
                         {
                             FirstName = !string.IsNullOrEmpty(cs.FIRST_NAME_TH) ? cs.FIRST_NAME_TH : cs.FIRST_NAME_EN,
                             LastName = !string.IsNullOrEmpty(cs.FIRST_NAME_TH) ? cs.LAST_NAME_TH : cs.LAST_NAME_EN,
                             JobType = ch.CHANNEL_NAME,
                             JobStatus = js.STATUS_NAME,
                             JobDateTime = jb.JOB_DATE,
                             From = jb.FROM,
                             Subject = jb.SUBJECT,
                             SRID = sr != null ? sr.SR_NO : string.Empty,
                             SRCreator = u1 != null ? u1.POSITION_CODE + " - " + u1.FIRST_NAME + " " + u1.LAST_NAME : string.Empty,
                             SROwner = u3 != null ? u3.POSITION_CODE + " - " + u3.FIRST_NAME + " " + u3.LAST_NAME : string.Empty,
                             SRState = ste.SR_STATE_NAME,
                             SRStatus = st.SR_STATUS_NAME,
                             AttachFile = jb.TB_T_JOB_ATTACHMENT.Any() ? Resource.Ddl_AttachFile_Yes : Resource.Ddl_AttachFile_No,
                             Remark = jb.REMARK,
                             PoolName = pl.POOL_NAME
                         });

            return query.ToList();
        }

        public IList<ExportSREntity> GetExportSR(ExportSRSearchFilter searchFilter)
        {
            int? productGroupId = searchFilter.ProductGroup.ToNullable<int>();
            int? productId = searchFilter.Product.ToNullable<int>();
            int? campaignId = searchFilter.Campaign.ToNullable<int>();
            int? typeId = searchFilter.Type.ToNullable<int>();
            int? areaId = searchFilter.Area.ToNullable<int>();
            int? subareaId = searchFilter.SubArea.ToNullable<int>();
            int? slaId = searchFilter.Sla.ToNullable<int>();
            int? srstatusId = searchFilter.SRStatus.ToNullable<int>();
            int? channelId = searchFilter.SRChannel.ToNullable<int>();
            int? ownerId = searchFilter.OwnerSR.ToNullable<int>();
            int? ownerBranchId = searchFilter.OwnerBranch.ToNullable<int>();
            int? creatorId = searchFilter.CreatorSR.ToNullable<int>();
            int? creatorBranchId = searchFilter.CreatorBranch.ToNullable<int>();

            DateTime? srMinDate = null;
            DateTime? srMaxDate = null;

            if (searchFilter.SRDateTimeFromValue.HasValue && searchFilter.SRDateTimeToValue.HasValue)
            {
                srMinDate = searchFilter.SRDateTimeFromValue.Value;
                srMaxDate = searchFilter.SRDateTimeToValue.Value;
            }
            else
            {
                int? monthOfReportExport =
                    _context.TB_C_PARAMETER.Where(x => x.PARAMETER_NAME == Constants.ParameterName.ReportExportDate)
                        .FirstOrDefault().PARAMETER_VALUE.ToNullable<int>();

                srMinDate = DateTime.Now.Date.AddMonths(monthOfReportExport.Value * -1);
                srMaxDate = DateTime.Now.Date.AddDays(1);
            }


            #region "Find Role CallCenter"

            List<int> lstRole = _context.TB_C_ROLE.Where
                (o => (o.ROLE_CODE == Constants.SrRoleCode.ContactCenterAgent || o.ROLE_CODE == Constants.SrRoleCode.ContactCenterFollowUp
                    || o.ROLE_CODE == Constants.SrRoleCode.ContactCenterManager || o.ROLE_CODE == Constants.SrRoleCode.ContactCenterSupervisor)
                ).Select(o => o.ROLE_ID).ToList();

            #endregion

            var query = (from sr in _context.TB_T_SR.AsNoTracking()
                         from cs in _context.TB_M_CUSTOMER.Where(c => c.CUSTOMER_ID == sr.CUSTOMER_ID).DefaultIfEmpty()
                         from ac in _context.TB_M_ACCOUNT.Where(c => c.ACCOUNT_ID == sr.ACCOUNT_ID).DefaultIfEmpty()
                         from ch in _context.TB_R_CHANNEL.Where(c => c.CHANNEL_ID == sr.CHANNEL_ID).DefaultIfEmpty()
                         from pg in _context.TB_R_PRODUCTGROUP.Where(c => c.PRODUCTGROUP_ID == sr.PRODUCTGROUP_ID).DefaultIfEmpty()
                         from pr in _context.TB_R_PRODUCT.Where(c => c.PRODUCT_ID == sr.PRODUCT_ID).DefaultIfEmpty()
                         from cp in _context.TB_R_CAMPAIGNSERVICE.Where(c => c.CAMPAIGNSERVICE_ID == sr.CAMPAIGNSERVICE_ID).DefaultIfEmpty()
                         from ty in _context.TB_M_TYPE.Where(c => c.TYPE_ID == sr.TYPE_ID).DefaultIfEmpty()
                         from ar in _context.TB_M_AREA.Where(c => c.AREA_ID == sr.AREA_ID).DefaultIfEmpty()
                         from sb in _context.TB_M_SUBAREA.Where(c => c.SUBAREA_ID == sr.SUBAREA_ID).DefaultIfEmpty()
                         from st in _context.TB_C_SR_STATUS.Where(c => c.SR_STATUS_ID == sr.SR_STATUS_ID).DefaultIfEmpty()
                         from ste in _context.TB_C_SR_STATE.Where(c => c.SR_STATE_ID == st.SR_STATE_ID).DefaultIfEmpty()
                         from cu in _context.TB_R_USER.Where(c => c.USER_ID == sr.CREATE_USER).DefaultIfEmpty()
                         from ou in _context.TB_R_USER.Where(c => c.USER_ID == sr.OWNER_USER_ID).DefaultIfEmpty()
                         from du in _context.TB_R_USER.Where(c => c.USER_ID == sr.DELEGATE_USER_ID).DefaultIfEmpty()
                         from uu in _context.TB_R_USER.Where(c => c.USER_ID == sr.UPDATE_USER).DefaultIfEmpty()
                         from clu in _context.TB_R_USER.Where(c => c.USER_ID == sr.CLOSE_USER).DefaultIfEmpty()
                         from ct in _context.TB_M_CONTACT.Where(c => c.CONTACT_ID == sr.CONTACT_ID).DefaultIfEmpty()
                         from me in _context.TB_M_MEDIA_SOURCE.Where(c => c.MEDIA_SOURCE_ID == sr.MEDIA_SOURCE_ID).DefaultIfEmpty()
                         from jb in _context.TB_T_JOB.Where(c => c.SR_ID == sr.SR_ID).DefaultIfEmpty()
                         from jc in _context.TB_R_CHANNEL.Where(c => c.CHANNEL_ID == jb.CHANNEL_ID).DefaultIfEmpty()
                         from cb in _context.TB_R_BRANCH.Where(c => c.BRANCH_ID == sr.CREATE_BRANCH_ID).DefaultIfEmpty()
                         from ob in _context.TB_R_BRANCH.Where(o => o.BRANCH_ID == sr.OWNER_BRANCH_ID).DefaultIfEmpty()
                         from rp in _context.TB_M_RELATIONSHIP.Where(c => c.RELATIONSHIP_ID == sr.CONTACT_RELATIONSHIP_ID).DefaultIfEmpty()
                         where
                            (sr.SR_PAGE_ID == Constants.SRPage.DefaultPageId || sr.SR_PAGE_ID == Constants.SRPage.AFSPageId
                            || sr.SR_PAGE_ID == Constants.SRPage.CPNPageId || !sr.SR_PAGE_ID.HasValue ||
                            (cu.ROLE_ID.HasValue && lstRole.Contains(cu.ROLE_ID.Value)))
                           && (ownerId == null || sr.OWNER_USER_ID == ownerId)
                           && (ownerBranchId == null || sr.OWNER_BRANCH_ID == ownerBranchId)
                           && (creatorId == null || sr.CREATE_USER == creatorId)
                           && (creatorBranchId == null || sr.CREATE_BRANCH_ID == creatorBranchId)
                         && (string.IsNullOrEmpty(searchFilter.FirstName) || (cs.FIRST_NAME_TH.Contains(searchFilter.FirstName) || cs.FIRST_NAME_EN.ToUpper().Contains(searchFilter.FirstName.ToUpper())))
                         && (string.IsNullOrEmpty(searchFilter.LastName) || (cs.LAST_NAME_TH.Contains(searchFilter.LastName) || cs.LAST_NAME_TH.ToUpper().Contains(searchFilter.LastName.ToUpper())))
                         && (string.IsNullOrEmpty(searchFilter.CardId) || cs.CARD_NO.Contains(searchFilter.CardId))
                         && (productGroupId == null || pg.PRODUCTGROUP_ID == productGroupId)
                         && (productId == null || pr.PRODUCT_ID == productId)
                         && (campaignId == null || cp.CAMPAIGNSERVICE_ID == campaignId)
                         && (typeId == null || ty.TYPE_ID == typeId)
                         && (areaId == null || ar.AREA_ID == areaId)
                         && (subareaId == null || sb.SUBAREA_ID == subareaId)
                         && (slaId == null || slaId == Constants.ApplicationStatus.All
                                    || (slaId == 1 && (sr.RULE_THIS_ALERT == null || sr.RULE_THIS_ALERT == 0))
                                    || (slaId == 2 && (sr.RULE_THIS_ALERT.HasValue && sr.RULE_THIS_ALERT > 0))
                               )
                         && (srstatusId == null || srstatusId == -1 || st.SR_STATUS_ID == srstatusId)
                         && (channelId == null || channelId == -1 || ch.CHANNEL_ID == channelId)
                         && (sr.CREATE_DATE >= srMinDate)
                         && (sr.CREATE_DATE <= srMaxDate)
                         && (string.IsNullOrEmpty(searchFilter.Subject) || sr.SR_SUBJECT.Contains(searchFilter.Subject))
                         && (string.IsNullOrEmpty(searchFilter.Description) || sr.SR_REMARK.Contains(searchFilter.Description))
                         && (string.IsNullOrEmpty(searchFilter.SRId) || sr.SR_NO.Contains(searchFilter.SRId))
                         && (string.IsNullOrEmpty(searchFilter.AccountNo) || ac.ACCOUNT_NO == searchFilter.AccountNo)
                         select new ExportSREntity
                         {
                             TotalSla = sr.RULE_TOTAL_ALERT, //sr.RULE_TOTAL_WORK,
                             CurrentAlert = sr.RULE_THIS_ALERT,
                             CustomerFistname = !string.IsNullOrEmpty(cs.FIRST_NAME_TH) ? cs.FIRST_NAME_TH : cs.FIRST_NAME_EN,
                             CustomerLastname = !string.IsNullOrEmpty(cs.FIRST_NAME_TH) ? cs.LAST_NAME_TH : cs.LAST_NAME_EN,
                             CardNo = cs.CARD_NO,
                             AccountNo = ac.ACCOUNT_NO,
                             CarRegisNo = ac.CAR_NO,
                             SRNo = sr.SR_NO,
                             CreatorBranch = cb.BRANCH_NAME,
                             ChannelName = ch.CHANNEL_NAME,
                             CallId = sr.SR_CALL_ID,
                             ANo = sr.SR_ANO,
                             ProductGroupName = pg.PRODUCTGROUP_NAME,
                             ProductName = pr.PRODUCT_NAME,
                             CampaignServiceName = cp.CAMPAIGNSERVICE_NAME,
                             TypeName = ty.TYPE_NAME,
                             AreaName = ar.AREA_NAME,
                             SubAreaName = sb.SUBAREA_NAME,
                             SRStateName = ste.SR_STATE_NAME,
                             SRStatusName = st.SR_STATUS_NAME,
                             CloseDate = sr.CLOSE_DATE,
                             SRIsverifyPass = sr.SR_IS_VERIFY_PASS,
                             CreatorName = ((cu.POSITION_CODE != null && cu.POSITION_CODE.Length > 0)
                                            ? cu.POSITION_CODE + "-" : "") + cu.FIRST_NAME + " " + cu.LAST_NAME,
                             CreateDate = sr.CREATE_DATE,
                             OwnerName = ((ou.POSITION_CODE != null && ou.POSITION_CODE.Length > 0)
                                            ? ou.POSITION_CODE + "-" : "") + ou.FIRST_NAME + " " + ou.LAST_NAME,
                             UpdateDateOwner = sr.UPDATE_DATE_BY_OWNER,
                             DelegatorName = ((du.POSITION_CODE != null && du.POSITION_CODE.Length > 0)
                                            ? du.POSITION_CODE + "-" : "") + du.FIRST_NAME + " " + du.LAST_NAME,
                             UpdateDelegate = sr.UPDATE_DATE_BY_DELEGATE,
                             SRSubject = sr.SR_SUBJECT,
                             SRRemark = sr.SR_REMARK,
                             ContactName = ct.FIRST_NAME_TH,
                             ContactSurname = ct.LAST_NAME_TH,
                             Relationship = rp.RELATIONSHIP_NAME,

                             MediaSourceName = me.MEDIA_SOURCE_NAME,
                             AttachFile = sr.TB_T_SR_ATTACHMENT.Any() ? "Yes" : "No",
                             JobType = jc.CHANNEL_NAME,
                             ContactNo = (from p in ct.TB_M_CONTACT_PHONE
                                          where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE == Constants.PhoneTypeCode.Mobile
                                          select new
                                          {
                                              PhoneNo = p.PHONE_NO
                                          }).FirstOrDefault().PhoneNo,
                             UpdateDate = sr.UPDATE_DATE,
                             UpdaterName = (uu != null ? (((uu.POSITION_CODE != null && uu.POSITION_CODE.Length > 0)
                                                            ? uu.POSITION_CODE + "-" : "") + uu.FIRST_NAME + " " + uu.LAST_NAME) : sr.UPDATE_USERNAME),
                             CloseUserName = (clu != null ? (((clu.POSITION_CODE != null && clu.POSITION_CODE.Length > 0)
                                                            ? clu.POSITION_CODE + "-" : "") + clu.FIRST_NAME + " " + clu.LAST_NAME) : sr.CLOSE_USERNAME)
                         });
            return query.ToList();
        }

        public IList<ExportSREntity> GetExportSRMonthly(ExportSRSearchFilter searchFilter)
        {
            int? productGroupId = searchFilter.ProductGroup.ToNullable<int>();
            int? productId = searchFilter.Product.ToNullable<int>();
            int? campaignId = searchFilter.Campaign.ToNullable<int>();
            int? typeId = searchFilter.Type.ToNullable<int>();
            int? areaId = searchFilter.Area.ToNullable<int>();
            int? subareaId = searchFilter.SubArea.ToNullable<int>();
            int? slaId = searchFilter.Sla.ToNullable<int>();
            int? srstatusId = searchFilter.SRStatus.ToNullable<int>();
            int? channelId = searchFilter.SRChannel.ToNullable<int>();
            int? ownerId = searchFilter.OwnerSR.ToNullable<int>();
            int? ownerBranchId = searchFilter.OwnerBranch.ToNullable<int>();
            int? creatorId = searchFilter.CreatorSR.ToNullable<int>();
            int? creatorBranchId = searchFilter.CreatorBranch.ToNullable<int>();

            DateTime? srMinDate = null;
            DateTime? srMaxDate = null;

            if (searchFilter.SRDateTimeFromValue.HasValue && searchFilter.SRDateTimeToValue.HasValue)
            {
                srMinDate = searchFilter.SRDateTimeFromValue.Value;
                srMaxDate = searchFilter.SRDateTimeToValue.Value;
            }
            else if (!string.IsNullOrEmpty(searchFilter.SRDateFrom) && !string.IsNullOrEmpty(searchFilter.SRDateTo))
            {
                srMinDate = DateTime.Parse(searchFilter.SRDateFrom);
                srMaxDate = DateTime.Parse(searchFilter.SRDateTo);
            }
            else
            {
                int? monthOfReportExport =
                    _context.TB_C_PARAMETER.Where(x => x.PARAMETER_NAME == Constants.ParameterName.ReportExportDate)
                        .FirstOrDefault().PARAMETER_VALUE.ToNullable<int>();

                srMinDate = DateTime.Now.Date.AddMonths(monthOfReportExport.Value * -1);
                srMaxDate = DateTime.Now.Date.AddDays(1);
            }


            #region "Find Role CallCenter"

            List<int> lstRole = _context.TB_C_ROLE.Where
                (o => (o.ROLE_CODE == Constants.SrRoleCode.ContactCenterAgent || o.ROLE_CODE == Constants.SrRoleCode.ContactCenterFollowUp
                    || o.ROLE_CODE == Constants.SrRoleCode.ContactCenterManager || o.ROLE_CODE == Constants.SrRoleCode.ContactCenterSupervisor)
                ).Select(o => o.ROLE_ID).ToList();

            #endregion


            var query = (from sr in _context.TB_T_SR.AsNoTracking()
                         from cs in _context.TB_M_CUSTOMER.Where(c => c.CUSTOMER_ID == sr.CUSTOMER_ID).DefaultIfEmpty()
                         from ac in _context.TB_M_ACCOUNT.Where(c => c.ACCOUNT_ID == sr.ACCOUNT_ID).DefaultIfEmpty()
                         from ch in _context.TB_R_CHANNEL.Where(c => c.CHANNEL_ID == sr.CHANNEL_ID).DefaultIfEmpty()
                         from pg in _context.TB_R_PRODUCTGROUP.Where(c => c.PRODUCTGROUP_ID == sr.PRODUCTGROUP_ID).DefaultIfEmpty()
                         from pr in _context.TB_R_PRODUCT.Where(c => c.PRODUCT_ID == sr.PRODUCT_ID).DefaultIfEmpty()
                         from cp in _context.TB_R_CAMPAIGNSERVICE.Where(c => c.CAMPAIGNSERVICE_ID == sr.CAMPAIGNSERVICE_ID).DefaultIfEmpty()
                         from ty in _context.TB_M_TYPE.Where(c => c.TYPE_ID == sr.TYPE_ID).DefaultIfEmpty()
                         from ar in _context.TB_M_AREA.Where(c => c.AREA_ID == sr.AREA_ID).DefaultIfEmpty()
                         from sb in _context.TB_M_SUBAREA.Where(c => c.SUBAREA_ID == sr.SUBAREA_ID).DefaultIfEmpty()
                         from st in _context.TB_C_SR_STATUS.Where(c => c.SR_STATUS_ID == sr.SR_STATUS_ID).DefaultIfEmpty()
                         from ste in _context.TB_C_SR_STATE.Where(c => c.SR_STATE_ID == st.SR_STATE_ID).DefaultIfEmpty()
                         from cu in _context.TB_R_USER.Where(c => c.USER_ID == sr.CREATE_USER).DefaultIfEmpty()
                         from ou in _context.TB_R_USER.Where(c => c.USER_ID == sr.OWNER_USER_ID).DefaultIfEmpty()
                         from du in _context.TB_R_USER.Where(c => c.USER_ID == sr.DELEGATE_USER_ID).DefaultIfEmpty()
                         from uu in _context.TB_R_USER.Where(c => c.USER_ID == sr.UPDATE_USER).DefaultIfEmpty()
                         from clu in _context.TB_R_USER.Where(c => c.USER_ID == sr.CLOSE_USER).DefaultIfEmpty()
                         from ct in _context.TB_M_CONTACT.Where(c => c.CONTACT_ID == sr.CONTACT_ID).DefaultIfEmpty()
                         from me in _context.TB_M_MEDIA_SOURCE.Where(c => c.MEDIA_SOURCE_ID == sr.MEDIA_SOURCE_ID).DefaultIfEmpty()
                         from jb in _context.TB_T_JOB.Where(c => c.SR_ID == sr.SR_ID).DefaultIfEmpty()
                         from jc in _context.TB_R_CHANNEL.Where(c => c.CHANNEL_ID == jb.CHANNEL_ID).DefaultIfEmpty()
                         from cb in _context.TB_R_BRANCH.Where(c => c.BRANCH_ID == sr.CREATE_BRANCH_ID).DefaultIfEmpty()
                         from ob in _context.TB_R_BRANCH.Where(o => o.BRANCH_ID == sr.OWNER_BRANCH_ID).DefaultIfEmpty()
                         from rp in _context.TB_M_RELATIONSHIP.Where(c => c.RELATIONSHIP_ID == sr.CONTACT_RELATIONSHIP_ID).DefaultIfEmpty()

                         where
                            (sr.SR_PAGE_ID == Constants.SRPage.DefaultPageId || sr.SR_PAGE_ID == Constants.SRPage.AFSPageId || !sr.SR_PAGE_ID.HasValue ||
                            (cu.ROLE_ID.HasValue && lstRole.Contains(cu.ROLE_ID.Value))
)
                           && (ownerId == null || sr.OWNER_USER_ID == ownerId)
                           && (ownerBranchId == null || sr.OWNER_BRANCH_ID == ownerBranchId)
                           && (creatorId == null || sr.CREATE_USER == creatorId)
                           && (creatorBranchId == null || sr.CREATE_BRANCH_ID == creatorBranchId)
                         && (string.IsNullOrEmpty(searchFilter.FirstName) || (cs.FIRST_NAME_TH.Contains(searchFilter.FirstName) || cs.FIRST_NAME_EN.ToUpper().Contains(searchFilter.FirstName.ToUpper())))
                         && (string.IsNullOrEmpty(searchFilter.LastName) || (cs.LAST_NAME_TH.Contains(searchFilter.LastName) || cs.LAST_NAME_TH.ToUpper().Contains(searchFilter.LastName.ToUpper())))
                         && (string.IsNullOrEmpty(searchFilter.CardId) || cs.CARD_NO.Contains(searchFilter.CardId))
                         && (productGroupId == null || pg.PRODUCTGROUP_ID == productGroupId)
                         && (productId == null || pr.PRODUCT_ID == productId)
                         && (campaignId == null || cp.CAMPAIGNSERVICE_ID == campaignId)
                         && (typeId == null || ty.TYPE_ID == typeId)
                         && (areaId == null || ar.AREA_ID == areaId)
                         && (subareaId == null || sb.SUBAREA_ID == subareaId)
                         && (slaId == null || slaId == Constants.ApplicationStatus.All
                                    || (slaId == 1 && (sr.RULE_THIS_ALERT == null || sr.RULE_THIS_ALERT == 0))
                                    || (slaId == 2 && (sr.RULE_THIS_ALERT.HasValue && sr.RULE_THIS_ALERT > 0))
                               )
                         && (st.SR_STATUS_ID != 7)
                         && (channelId == null || channelId == -1 || ch.CHANNEL_ID == channelId)
                         && (sr.CREATE_DATE >= srMinDate)
                         && (sr.CREATE_DATE <= srMaxDate)
                         && (string.IsNullOrEmpty(searchFilter.Subject) || sr.SR_SUBJECT.Contains(searchFilter.Subject))
                         && (string.IsNullOrEmpty(searchFilter.Description) || sr.SR_REMARK.Contains(searchFilter.Description))
                         && (string.IsNullOrEmpty(searchFilter.SRId) || sr.SR_NO.Contains(searchFilter.SRId))
                         && (string.IsNullOrEmpty(searchFilter.AccountNo) || ac.ACCOUNT_NO == searchFilter.AccountNo)
                         select new ExportSREntity
                         {
                             TotalSla = sr.RULE_TOTAL_ALERT, //sr.RULE_TOTAL_WORK,
                             CurrentAlert = sr.RULE_THIS_ALERT,
                             CustomerFistname = !string.IsNullOrEmpty(cs.FIRST_NAME_TH) ? cs.FIRST_NAME_TH : cs.FIRST_NAME_EN,
                             CustomerLastname = !string.IsNullOrEmpty(cs.FIRST_NAME_TH) ? cs.LAST_NAME_TH : cs.LAST_NAME_EN,
                             CardNo = cs.CARD_NO,
                             AccountNo = ac.ACCOUNT_NO,
                             CarRegisNo = ac.CAR_NO,
                             SRNo = sr.SR_NO,
                             CreatorBranch = cb.BRANCH_NAME,
                             ChannelName = ch.CHANNEL_NAME,
                             CallId = sr.SR_CALL_ID,
                             ANo = sr.SR_ANO,
                             ProductGroupName = pg.PRODUCTGROUP_NAME,
                             ProductName = pr.PRODUCT_NAME,
                             CampaignServiceName = cp.CAMPAIGNSERVICE_NAME,
                             TypeName = ty.TYPE_NAME,
                             AreaName = ar.AREA_NAME,
                             SubAreaName = sb.SUBAREA_NAME,
                             SRStateName = ste != null ? ste.SR_STATE_NAME : null,
                             SRStatusName = st.SR_STATUS_NAME,
                             CloseDate = sr.CLOSE_DATE,
                             SRIsverifyPass = sr.SR_IS_VERIFY_PASS,
                             CreatorName = cu.POSITION_CODE + "-" + cu.FIRST_NAME + " " + cu.LAST_NAME,
                             CreateDate = sr.CREATE_DATE,
                             OwnerName = ou.POSITION_CODE + "-" + ou.FIRST_NAME + " " + ou.LAST_NAME,
                             UpdateDateOwner = sr.UPDATE_DATE_BY_OWNER,
                             DelegatorName = du.POSITION_CODE + "-" + du.FIRST_NAME + " " + du.LAST_NAME,
                             UpdateDelegate = sr.UPDATE_DATE_BY_DELEGATE,
                             SRSubject = sr.SR_SUBJECT,
                             SRRemark = sr.SR_REMARK,
                             ContactName = ct.FIRST_NAME_TH,
                             ContactSurname = ct.LAST_NAME_TH,
                             Relationship = rp.RELATIONSHIP_NAME,

                             MediaSourceName = me.MEDIA_SOURCE_NAME,
                             //AttachFile = sr.TB_T_SR_ATTACHMENT.Any() ? "Yes" : "No",
                             //JobType = jc.CHANNEL_NAME,
                             ContactNo = (from p in ct.TB_M_CONTACT_PHONE
                                          where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE == Constants.PhoneTypeCode.Mobile
                                          select new
                                          {
                                              PhoneNo = p.PHONE_NO
                                          }).FirstOrDefault().PhoneNo,
                             UpdateDate = sr.UPDATE_DATE,
                             UpdaterName = (uu != null ? (((uu.POSITION_CODE != null && uu.POSITION_CODE.Length > 0)
                                                            ? uu.POSITION_CODE + "-" : "") + uu.FIRST_NAME + " " + uu.LAST_NAME) : sr.UPDATE_USERNAME),
                             CloseUserName = (clu != null ? (((clu.POSITION_CODE != null && clu.POSITION_CODE.Length > 0)
                                                            ? clu.POSITION_CODE + "-" : "") + clu.FIRST_NAME + " " + clu.LAST_NAME) : sr.CLOSE_USERNAME)
                         });

            return query.ToList();
        }

        public IList<ExportSRComplaintEntity> GetExportComplaint(ExportSRSearchFilter searchFilter)
        {
            int? productGroupId = searchFilter.ProductGroup.ToNullable<int>();
            int? productId = searchFilter.Product.ToNullable<int>();
            int? campaignId = searchFilter.Campaign.ToNullable<int>();
            int? typeId = searchFilter.Type.ToNullable<int>();
            int? areaId = searchFilter.Area.ToNullable<int>();
            int? subareaId = searchFilter.SubArea.ToNullable<int>();
            int? slaId = searchFilter.Sla.ToNullable<int>();
            int? srstatusId = searchFilter.SRStatus.ToNullable<int>();
            int? channelId = searchFilter.SRChannel.ToNullable<int>();
            int? ownerId = searchFilter.OwnerSR.ToNullable<int>();
            int? ownerBranchId = searchFilter.OwnerBranch.ToNullable<int>();
            int? creatorId = searchFilter.CreatorSR.ToNullable<int>();
            int? creatorBranchId = searchFilter.CreatorBranch.ToNullable<int>();

            DateTime? srMinDate = null;
            DateTime? srMaxDate = null;

            if (searchFilter.SRDateTimeFromValue.HasValue && searchFilter.SRDateTimeToValue.HasValue)
            {
                srMinDate = searchFilter.SRDateTimeFromValue.Value;
                srMaxDate = searchFilter.SRDateTimeToValue.Value;
            }
            else
            {
                int? monthOfReportExport =
                    _context.TB_C_PARAMETER.Where(x => x.PARAMETER_NAME == Constants.ParameterName.ReportExportDate)
                        .FirstOrDefault().PARAMETER_VALUE.ToNullable<int>();

                srMinDate = DateTime.Now.Date.AddMonths(monthOfReportExport.Value * -1);
                srMaxDate = DateTime.Now.Date.AddDays(1);
            }


            #region "Find Role CallCenter"

            List<int> lstRole = _context.TB_C_ROLE.Where
                (o => (o.ROLE_CODE == Constants.SrRoleCode.ContactCenterAgent || o.ROLE_CODE == Constants.SrRoleCode.ContactCenterFollowUp
                    || o.ROLE_CODE == Constants.SrRoleCode.ContactCenterManager || o.ROLE_CODE == Constants.SrRoleCode.ContactCenterSupervisor)
                ).Select(o => o.ROLE_ID).ToList();

            #endregion

            Logger.Debug("Select Report");

            var query = (from sr in _context.TB_T_SR.AsNoTracking()
                         from cs in _context.TB_M_CUSTOMER.Where(c => c.CUSTOMER_ID == sr.CUSTOMER_ID).DefaultIfEmpty()
                         from ac in _context.TB_M_ACCOUNT.Where(c => c.ACCOUNT_ID == sr.ACCOUNT_ID).DefaultIfEmpty()
                         from ch in _context.TB_R_CHANNEL.Where(c => c.CHANNEL_ID == sr.CHANNEL_ID).DefaultIfEmpty()
                         from pg in _context.TB_R_PRODUCTGROUP.Where(c => c.PRODUCTGROUP_ID == sr.PRODUCTGROUP_ID).DefaultIfEmpty()
                         from pr in _context.TB_R_PRODUCT.Where(c => c.PRODUCT_ID == sr.PRODUCT_ID).DefaultIfEmpty()
                         from cp in _context.TB_R_CAMPAIGNSERVICE.Where(c => c.CAMPAIGNSERVICE_ID == sr.CAMPAIGNSERVICE_ID).DefaultIfEmpty()
                         from ty in _context.TB_M_TYPE.Where(c => c.TYPE_ID == sr.TYPE_ID).DefaultIfEmpty()
                         from ar in _context.TB_M_AREA.Where(c => c.AREA_ID == sr.AREA_ID).DefaultIfEmpty()
                         from sb in _context.TB_M_SUBAREA.Where(c => c.SUBAREA_ID == sr.SUBAREA_ID).DefaultIfEmpty()
                         from cu in _context.TB_R_USER.Where(c => c.USER_ID == sr.CREATE_USER).DefaultIfEmpty()
                         from ou in _context.TB_R_USER.Where(c => c.USER_ID == sr.OWNER_USER_ID).DefaultIfEmpty()
                         from du in _context.TB_R_USER.Where(c => c.USER_ID == sr.DELEGATE_USER_ID).DefaultIfEmpty()
                         from ct in _context.TB_M_CONTACT.Where(c => c.CONTACT_ID == sr.CONTACT_ID).DefaultIfEmpty()
                         from me in _context.TB_M_MEDIA_SOURCE.Where(c => c.MEDIA_SOURCE_ID == sr.MEDIA_SOURCE_ID).DefaultIfEmpty()
                         from jb in _context.TB_T_JOB.Where(c => c.SR_ID == sr.SR_ID).DefaultIfEmpty()
                         from jc in _context.TB_R_CHANNEL.Where(c => c.CHANNEL_ID == jb.CHANNEL_ID).DefaultIfEmpty()
                         from cb in _context.TB_R_BRANCH.Where(c => c.BRANCH_ID == sr.CREATE_BRANCH_ID).DefaultIfEmpty()
                         from ob in _context.TB_R_BRANCH.Where(o => o.BRANCH_ID == sr.OWNER_BRANCH_ID).DefaultIfEmpty()
                         from rp in _context.TB_M_RELATIONSHIP.Where(c => c.RELATIONSHIP_ID == sr.CONTACT_RELATIONSHIP_ID).DefaultIfEmpty()
                         from clu in _context.TB_R_USER.Where(r => r.USER_ID == sr.CLOSE_USER).DefaultIfEmpty()
                         from sra in _context.TB_T_SR_ACTIVITY.Where(r => r.SR_ID == sr.SR_ID)
                         from sra_create in _context.TB_R_USER.Where(r => r.USER_ID == sra.CREATE_USER).DefaultIfEmpty()
                         from upd_usr in _context.TB_R_USER.Where(r => r.USER_ID == sr.UPDATE_USER).DefaultIfEmpty()
                         from cpn_pdg in _context.TB_R_PRODUCTGROUP.Where(r => r.PRODUCTGROUP_ID == sr.CPN_PRODUCT_GROUP_ID).DefaultIfEmpty()
                         from cpn_pd in _context.TB_R_PRODUCT.Where(r => r.PRODUCT_ID == sr.CPN_PRODUCT_ID).DefaultIfEmpty()
                         from cpn_cam in _context.TB_R_CAMPAIGNSERVICE.Where(r => r.CAMPAIGNSERVICE_ID == sr.CPN_CAMPAIGNSERVICE_ID).DefaultIfEmpty()
                         from cpn_sub in _context.TB_M_COMPLAINT_SUBJECT.Where(r => r.COMPLAINT_SUBJECT_ID == sr.CPN_SUBJECT_ID).DefaultIfEmpty()
                         from cpn_typ in _context.TB_M_COMPLAINT_TYPE.Where(r => r.COMPLAINT_TYPE_ID == sr.CPN_TYPE_ID).DefaultIfEmpty()
                         from cpn_root in _context.TB_M_COMPLAINT_ROOT_CAUSE.Where(r => r.ROOT_CAUSE_ID == sr.CPN_ROOT_CAUSE_ID).DefaultIfEmpty()
                         from cpn_iss in _context.TB_M_COMPLAINT_ISSUES.Where(r => r.COMPLAINT_ISSUES_ID == sr.CPN_ISSUES_ID).DefaultIfEmpty()
                         from cpn_bu in _context.TB_M_COMPLAINT_BU_GROUP.Where(r => r.COMPLAINT_BU_GROUP_ID == sr.CPN_BU_GROUP_ID).DefaultIfEmpty()
                         from cpn_sum in _context.TB_M_COMPLAINT_SUMMARY.Where(r => r.COMPLAINT_SUMMARY_ID == sr.CPN_SUMMARY_ID).DefaultIfEmpty()
                         from cpn_ca_sum in _context.TB_M_COMPLAINT_CAUSE_SUMMARY.Where(r => r.FCAUSE_ID == sr.CPN_CAUSE_SUMMARY_ID).DefaultIfEmpty()
                         from st in _context.TB_C_SR_STATUS.Where(c => c.SR_STATUS_ID == sr.SR_STATUS_ID).DefaultIfEmpty()
                         from ste in _context.TB_C_SR_STATE.Where(c => c.SR_STATE_ID == st.SR_STATE_ID).DefaultIfEmpty()
                         from srl in _context.TB_L_SR_LOGGING.Where(r => r.SR_ACTIVITY_ID == sra.SR_ACTIVITY_ID && r.SR_LOGGING_ACTION == Constants.SrLogAction.ChangeStatus).DefaultIfEmpty()
                         from sta in _context.TB_C_SR_STATUS.Where(c => c.SR_STATUS_ID == sra.SR_STATUS_ID).DefaultIfEmpty()
                         from stea in _context.TB_C_SR_STATE.Where(c => c.SR_STATE_ID == sta.SR_STATE_ID).DefaultIfEmpty()
                         from osta in _context.TB_C_SR_STATUS.Where(c => c.SR_STATUS_ID == sra.OLD_SR_STATUS_ID).DefaultIfEmpty()
                         from ostea in _context.TB_C_SR_STATE.Where(c => c.SR_STATE_ID == osta.SR_STATE_ID).DefaultIfEmpty()
                         from oua in _context.TB_R_USER.Where(c => c.USER_ID == sra.OWNER_USER_ID).DefaultIfEmpty()
                         from dua in _context.TB_R_USER.Where(c => c.USER_ID == sra.DELEGATE_USER_ID).DefaultIfEmpty()
                         from ooua in _context.TB_R_USER.Where(c => c.USER_ID == sra.OLD_OWNER_USER_ID).DefaultIfEmpty()
                         from odua in _context.TB_R_USER.Where(c => c.USER_ID == sra.OLD_DELEGATE_USER_ID).DefaultIfEmpty()
                         where
                            (sr.SR_PAGE_ID == Constants.SRPage.CPNPageId)
                           && (ownerId == null || sr.OWNER_USER_ID == ownerId)
                           && (ownerBranchId == null || sr.OWNER_BRANCH_ID == ownerBranchId)
                           && (creatorId == null || sr.CREATE_USER == creatorId)
                           && (creatorBranchId == null || sr.CREATE_BRANCH_ID == creatorBranchId)
                         && (string.IsNullOrEmpty(searchFilter.FirstName) || (cs.FIRST_NAME_TH.Contains(searchFilter.FirstName) || cs.FIRST_NAME_EN.ToUpper().Contains(searchFilter.FirstName.ToUpper())))
                         && (string.IsNullOrEmpty(searchFilter.LastName) || (cs.LAST_NAME_TH.Contains(searchFilter.LastName) || cs.LAST_NAME_EN.ToUpper().Contains(searchFilter.LastName.ToUpper())))
                         && (string.IsNullOrEmpty(searchFilter.CardId) || cs.CARD_NO.Contains(searchFilter.CardId))
                         && (productGroupId == null || pg.PRODUCTGROUP_ID == productGroupId)
                         && (productId == null || pr.PRODUCT_ID == productId)
                         && (campaignId == null || cp.CAMPAIGNSERVICE_ID == campaignId)
                         && (typeId == null || ty.TYPE_ID == typeId)
                         && (areaId == null || ar.AREA_ID == areaId)
                         && (subareaId == null || sb.SUBAREA_ID == subareaId)
                         && (slaId == null || slaId == Constants.ApplicationStatus.All
                                    || (slaId == 1 && (sr.RULE_THIS_ALERT == null || sr.RULE_THIS_ALERT == 0))
                                    || (slaId == 2 && (sr.RULE_THIS_ALERT.HasValue && sr.RULE_THIS_ALERT > 0))
                               )
                         && (srstatusId == null || srstatusId == -1 || st.SR_STATUS_ID == srstatusId)
                         && (channelId == null || channelId == -1 || ch.CHANNEL_ID == channelId)
                         && (sr.CREATE_DATE >= srMinDate)
                         && (sr.CREATE_DATE <= srMaxDate)
                         && (string.IsNullOrEmpty(searchFilter.Subject) || sr.SR_SUBJECT.Contains(searchFilter.Subject))
                         && (string.IsNullOrEmpty(searchFilter.Description) || sr.SR_REMARK.Contains(searchFilter.Description))
                         && (string.IsNullOrEmpty(searchFilter.SRId) || sr.SR_NO.Contains(searchFilter.SRId))
                         && (string.IsNullOrEmpty(searchFilter.AccountNo) || ac.ACCOUNT_NO == searchFilter.AccountNo)
                         select new ExportSRComplaintEntity
                         {
                             TotalSla = sr.RULE_TOTAL_ALERT,
                             CurrentAlert = srl.OVER_SLA_TIMES,
                             TotalWorkHour = sr.TB_T_SR_ACTIVITY.Sum(x => x.WORKING_MINUTE ?? 0),
                             CurrentWorkHour = sra.WORKING_MINUTE,
                             CustomerFistname = !string.IsNullOrEmpty(cs.FIRST_NAME_TH) ? cs.FIRST_NAME_TH : cs.FIRST_NAME_EN,
                             CustomerLastname = !string.IsNullOrEmpty(cs.FIRST_NAME_TH) ? cs.LAST_NAME_TH : cs.LAST_NAME_EN,
                             CardNo = cs.CARD_NO,
                             AccountNo = ac.ACCOUNT_NO,
                             CarRegisNo = ac.CAR_NO,
                             SRNo = sr.SR_NO,
                             CreatorBranch = cb.BRANCH_NAME,
                             ChannelName = ch.CHANNEL_NAME,
                             CallId = sr.SR_CALL_ID,
                             ANo = sr.SR_ANO,
                             ProductGroupName = pg.PRODUCTGROUP_NAME,
                             ProductName = pr.PRODUCT_NAME,
                             CampaignServiceName = cp.CAMPAIGNSERVICE_NAME,
                             TypeName = ty.TYPE_NAME,
                             AreaName = ar.AREA_NAME,
                             SubAreaName = sb.SUBAREA_NAME,
                             //SRStateName = ste.SR_STATE_NAME,
                             //SRStatusName = st.SR_STATUS_NAME,
                             SRStateNameNew = stea != null ? stea.SR_STATE_NAME : null,
                             SRStatusNameNew = sta != null ? sta.SR_STATUS_NAME : null,
                             SRStateNameOld = ostea != null ? ostea.SR_STATE_NAME : null,
                             SRStatusNameOld = osta != null ? osta.SR_STATUS_NAME : null,
                             CloseDate = sr.CLOSE_DATE,
                             SRIsverifyPass = sr.SR_IS_VERIFY_PASS,
                             CreatorName = ((cu.POSITION_CODE ?? "") != "" ? cu.POSITION_CODE + "-" : "") + cu.FIRST_NAME + " " + cu.LAST_NAME,
                             CreateDate = sr.CREATE_DATE,
                             OwnerName = ((ou.POSITION_CODE ?? "") != "" ? ou.POSITION_CODE + "-" : "") + ou.FIRST_NAME + " " + ou.LAST_NAME,
                             UpdateDateOwner = sr.UPDATE_DATE_BY_OWNER,
                             DelegatorName = ((du.POSITION_CODE ?? "") != "" ? du.POSITION_CODE + "-" : "") + du.FIRST_NAME + " " + du.LAST_NAME,
                             UpdateDelegate = sr.UPDATE_DATE_BY_DELEGATE,
                             OwnerNameNew = ((oua.POSITION_CODE ?? "") != "" ? oua.POSITION_CODE + "-" : "") + oua.FIRST_NAME + " " + oua.LAST_NAME,
                             DelegatorNameNew = ((dua.POSITION_CODE ?? "") != "" ? dua.POSITION_CODE + "-" : "") + dua.FIRST_NAME + " " + dua.LAST_NAME,
                             OwnerNameOld = ((ooua.POSITION_CODE ?? "") != "" ? ooua.POSITION_CODE + "-" : "") + ooua.FIRST_NAME + " " + ooua.LAST_NAME,
                             DelegatorNameOld = ((odua.POSITION_CODE ?? "") != "" ? odua.POSITION_CODE + "-" : "") + odua.FIRST_NAME + " " + odua.LAST_NAME,
                             SRSubject = sr.SR_SUBJECT,
                             SRRemark = sr.SR_REMARK,
                             ContactName = ct.FIRST_NAME_TH,
                             ContactSurname = ct.LAST_NAME_TH,
                             Relationship = rp.RELATIONSHIP_NAME,

                             MediaSourceName = me.MEDIA_SOURCE_NAME,
                             AttachFile = sr.TB_T_SR_ATTACHMENT.Any() ? "Yes" : "No",
                             JobType = jc.CHANNEL_NAME,
                             ContactNo = (from p in ct.TB_M_CONTACT_PHONE
                                          where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE == Constants.PhoneTypeCode.Mobile
                                          select new
                                          {
                                              PhoneNo = p.PHONE_NO
                                          }).FirstOrDefault().PhoneNo,
                             SRUpdateDate = sr.UPDATE_DATE,
                             SRUpdateBy = (upd_usr == null ? sr.UPDATE_USERNAME :
                                                            ((upd_usr.POSITION_CODE ?? "") != "" ? upd_usr.POSITION_CODE + "-" : "")
                                                                + upd_usr.FIRST_NAME + " " + upd_usr.LAST_NAME),
                             CPNProductGroupName = cpn_pdg.PRODUCTGROUP_NAME,
                             CPNProductName = cpn_pd.PRODUCT_NAME,
                             CPNCampaignServiceName = cpn_cam.CAMPAIGNSERVICE_NAME,
                             CPNSubjectName = cpn_sub.COMPLAINT_SUBJECT_NAME,
                             CPNTypeName = cpn_typ.COMPLAINT_TYPE_NAME,
                             CPNRootCauseName = cpn_root.ROOT_CAUSE_NAME,
                             CPNIssuesName = cpn_iss.COMPLAINT_ISSUES_NAME,
                             CPNBUGroupName = cpn_bu.COMPLAINT_BU_GROUP_NAME,
                             CPNIsSummary = sr.CPN_SUMMARY.ToString(),
                             CPNSummaryName = cpn_sum.COMPLAINT_SUMMARY_NAME,
                             CPNCauseSummaryName = cpn_ca_sum.FCAUSE_NAME,
                             CPNFixedDetail = sr.CPN_FIXED_DETAIL,
                             CPNCauseCustomer = (sr.CPN_CAUSE_CUSTOMER ?? false ? "Y" : "N"),
                             CPNCauseCustomerDetail = sr.CPN_CAUSE_CUSTOMER_DETAIL,
                             CPNCauseStaff = (sr.CPN_CAUSE_STAFF ?? false ? "Y" : "N"),
                             CPNCauseStaffDetail = sr.CPN_CAUSE_STAFF_DETAIL,
                             CPNCauseSystem = (sr.CPN_CAUSE_SYSTEM ?? false ? "Y" : "N"),
                             CPNCauseSystemDetail = sr.CPN_CAUSE_SYSTEM_DETAIL,
                             CPNCauseProcess = (sr.CPN_CAUSE_PROCESS ?? false ? "Y" : "N"),
                             CPNCauseProcessDetail = sr.CPN_CAUSE_PROCESS_DETAIL,
                             ActivityTypeName = _context.TB_C_SR_ACTIVITY_TYPE.Where(x => x.SR_ACTIVITY_TYPE_ID == sra.SR_ACTIVITY_TYPE_ID).Select(x => x.SR_ACTIVITY_TYPE_NAME).FirstOrDefault(),
                             ActivityDescription = sra.SR_ACTIVITY_DESC,
                             CPNIsSecret = (sr.CPN_SECRET ?? false ? "Y" : "N"),
                             SendMailTo = sra.SR_ACTIVITY_EMAIL_TO,
                             SendMailCc = sra.SR_ACTIVITY_EMAIL_CC,
                             SendMailBcc = sra.SR_ACTIVITY_EMAIL_BCC,
                             SendMailSubject = sra.SR_ACTIVITY_EMAIL_SUBJECT,
                             SendMailBody = sra.SR_ACTIVITY_EMAIL_BODY,
                             ActivityCreateBy = sra_create.POSITION_CODE + "-" + sra_create.FIRST_NAME + " " + sra_create.LAST_NAME,
                             ActivityCreateDate = sra.CREATE_DATE,
                             ClosedBy = (clu == null ? sr.CLOSE_USERNAME :
                                            ((clu.POSITION_CODE ?? "") != "" ? clu.POSITION_CODE + "-" : "") + clu.FIRST_NAME + " " + clu.LAST_NAME)
                         });
            Logger.Debug("Select Report 2");
            return query.OrderBy(o => o.CreateDate).ThenBy(o => o.ActivityCreateDate).ToList();
        }

        public IList<ExportVerifyDetailEntity> GetExportVerifyDetail(ExportVerifyDetailSearchFilter searchFilter)
        {
            int? productGroupId = searchFilter.ProductGroup.ToNullable<int>();
            int? productId = searchFilter.Product.ToNullable<int>();
            int? campaignId = searchFilter.Campaign.ToNullable<int>();
            int? typeId = searchFilter.Type.ToNullable<int>();
            int? areaId = searchFilter.Area.ToNullable<int>();
            int? subareaId = searchFilter.SubArea.ToNullable<int>();
            int? ownerId = searchFilter.OwnerSR.ToNullable<int>();
            int? ownerbranchId = searchFilter.OwnerBranch.ToNullable<int>();
            string selectAllValue = Constants.ApplicationStatus.All.ConvertToString();

            DateTime? srMinDate = null;
            DateTime? srMaxDate = null;

            if (searchFilter.SRDateTimeFromValue.HasValue && searchFilter.SRDateTimeToValue.HasValue)
            {
                srMinDate = searchFilter.SRDateTimeFromValue.Value;
                srMaxDate = searchFilter.SRDateTimeToValue.Value;
            }
            else
            {
                int? monthOfReportExport =
                    _context.TB_C_PARAMETER.Where(x => x.PARAMETER_NAME == Constants.ParameterName.ReportExportDate)
                        .FirstOrDefault().PARAMETER_VALUE.ToNullable<int>();

                srMinDate = DateTime.Now.Date.AddMonths(monthOfReportExport.Value * -1);
                srMaxDate = DateTime.Now.Date.AddDays(1);
            }

            var query = (from sr in _context.TB_T_SR.AsNoTracking()
                         join cs in _context.TB_M_CUSTOMER on sr.CUSTOMER_ID equals cs.CUSTOMER_ID
                         join ac in _context.TB_M_ACCOUNT on sr.ACCOUNT_ID equals ac.ACCOUNT_ID
                         join pg in _context.TB_R_PRODUCTGROUP on sr.PRODUCTGROUP_ID equals pg.PRODUCTGROUP_ID
                         join pr in _context.TB_R_PRODUCT on sr.PRODUCT_ID equals pr.PRODUCT_ID
                         join cp in _context.TB_R_CAMPAIGNSERVICE on sr.CAMPAIGNSERVICE_ID equals cp.CAMPAIGNSERVICE_ID
                         join ty in _context.TB_M_TYPE on sr.TYPE_ID equals ty.TYPE_ID
                         join ar in _context.TB_M_AREA on sr.AREA_ID equals ar.AREA_ID
                         join sb in _context.TB_M_SUBAREA on sr.SUBAREA_ID equals sb.SUBAREA_ID
                         join ou in _context.TB_R_USER on sr.OWNER_USER_ID equals ou.USER_ID
                         from vr in _context.TB_T_SR_VERIFY_RESULT_QUESTION.Where(c => c.TB_T_SR_VERIFY_RESULT_GROUP.SR_ID == sr.SR_ID).DefaultIfEmpty()
                         where (productGroupId == null || pg.PRODUCTGROUP_ID == productGroupId)
                            && (productId == null || pr.PRODUCT_ID == productId)
                            && (campaignId == null || cp.CAMPAIGNSERVICE_ID == campaignId)
                            && (typeId == null || ty.TYPE_ID == typeId)
                            && (areaId == null || ar.AREA_ID == areaId)
                            && (subareaId == null || sb.SUBAREA_ID == subareaId)
                            && (ownerId == null || ou.USER_ID == ownerId)
                            && (ownerbranchId == null || sr.OWNER_BRANCH_ID == ownerbranchId)
                            && (string.IsNullOrEmpty(searchFilter.SRId) || sr.SR_NO == searchFilter.SRId)
                            && (string.IsNullOrEmpty(searchFilter.SRIsverify) || selectAllValue == searchFilter.SRIsverify || sr.SR_IS_VERIFY_PASS == searchFilter.SRIsverify)
                            && (sr.CREATE_DATE >= srMinDate)
                            && (sr.CREATE_DATE <= srMaxDate)
                         select new ExportVerifyDetailEntity
                         {
                             SRId = sr.SR_NO,
                             AccountNo = ac.ACCOUNT_NO,
                             CustomerFistname = !string.IsNullOrEmpty(cs.FIRST_NAME_TH) ? cs.FIRST_NAME_TH : cs.FIRST_NAME_EN,
                             CustomerLastname = !string.IsNullOrEmpty(cs.FIRST_NAME_TH) ? cs.LAST_NAME_TH : cs.LAST_NAME_EN,
                             ProductGroupName = pg.PRODUCTGROUP_NAME,
                             ProductName = pr.PRODUCT_NAME,
                             CampaignServiceName = cp.CAMPAIGNSERVICE_NAME,
                             TypeName = ty.TYPE_NAME,
                             AreaName = ar.AREA_NAME,
                             SubAreaName = sb.SUBAREA_NAME,
                             SROwnerName = ou.POSITION_CODE + "-" + ou.FIRST_NAME + " " + ou.LAST_NAME,
                             IsVerifyPass = sr.SR_IS_VERIFY_PASS,
                             QuestionGroupName = vr.TB_T_SR_VERIFY_RESULT_GROUP.QUESTIONGROUP_NAME,
                             QuestionName = vr.QUESTION_NAME,
                             VerifyResult = vr.RESULT
                         });

            return query.ToList();
        }

        public IList<ExportVerifyEntity> GetExportVerify(ExportVerifySearchFilter searchFilter)
        {
            int? productGroupId = searchFilter.ProductGroup.ToNullable<int>();
            int? productId = searchFilter.Product.ToNullable<int>();
            int? campaignId = searchFilter.Campaign.ToNullable<int>();
            int? typeId = searchFilter.Type.ToNullable<int>();
            int? areaId = searchFilter.Area.ToNullable<int>();
            int? subareaId = searchFilter.SubArea.ToNullable<int>();
            int? ownerId = searchFilter.OwnerSR.ToNullable<int>();
            int? ownerbranchId = searchFilter.OwnerBranch.ToNullable<int>();
            string selectAllValue = Constants.ApplicationStatus.All.ConvertToString();

            DateTime? srMinDate = null;
            DateTime? srMaxDate = null;

            if (searchFilter.SRDateTimeFromValue.HasValue && searchFilter.SRDateTimeToValue.HasValue)
            {
                srMinDate = searchFilter.SRDateTimeFromValue.Value;
                srMaxDate = searchFilter.SRDateTimeToValue.Value;
            }
            else
            {
                int? monthOfReportExport =
                    _context.TB_C_PARAMETER.Where(x => x.PARAMETER_NAME == Constants.ParameterName.ReportExportDate)
                        .FirstOrDefault().PARAMETER_VALUE.ToNullable<int>();

                srMinDate = DateTime.Now.Date.AddMonths(monthOfReportExport.Value * -1);
                srMaxDate = DateTime.Now.Date.AddDays(1);
            }

            var query = (from sr in _context.TB_T_SR.AsNoTracking()
                         join cs in _context.TB_M_CUSTOMER on sr.CUSTOMER_ID equals cs.CUSTOMER_ID
                         join ac in _context.TB_M_ACCOUNT on sr.ACCOUNT_ID equals ac.ACCOUNT_ID
                         join pg in _context.TB_R_PRODUCTGROUP on sr.PRODUCTGROUP_ID equals pg.PRODUCTGROUP_ID
                         join pr in _context.TB_R_PRODUCT on sr.PRODUCT_ID equals pr.PRODUCT_ID
                         join cp in _context.TB_R_CAMPAIGNSERVICE on sr.CAMPAIGNSERVICE_ID equals cp.CAMPAIGNSERVICE_ID
                         join ty in _context.TB_M_TYPE on sr.TYPE_ID equals ty.TYPE_ID
                         join ar in _context.TB_M_AREA on sr.AREA_ID equals ar.AREA_ID
                         join sb in _context.TB_M_SUBAREA on sr.SUBAREA_ID equals sb.SUBAREA_ID
                         join st in _context.TB_C_SR_STATUS on sr.SR_STATUS_ID equals st.SR_STATUS_ID
                         from ste in _context.TB_C_SR_STATE.Where(c => c.SR_STATE_ID == st.SR_STATE_ID).DefaultIfEmpty()

                         from ou in _context.TB_R_USER.Where(c => c.USER_ID == sr.OWNER_USER_ID).DefaultIfEmpty()
                         from ob in _context.TB_R_BRANCH.Where(c => c.BRANCH_ID == sr.OWNER_BRANCH_ID).DefaultIfEmpty()
                         from cb in _context.TB_R_BRANCH.Where(c => c.BRANCH_ID == sr.CREATE_BRANCH_ID).DefaultIfEmpty()


                         where (productGroupId == null || pg.PRODUCTGROUP_ID == productGroupId)
                            && (productId == null || pr.PRODUCT_ID == productId)
                            && (campaignId == null || cp.CAMPAIGNSERVICE_ID == campaignId)
                            && (typeId == null || ty.TYPE_ID == typeId)
                            && (areaId == null || ar.AREA_ID == areaId)
                            && (subareaId == null || sb.SUBAREA_ID == subareaId)
                            && (ownerId == null || ou.USER_ID == ownerId)
                            && (ownerbranchId == null || sr.OWNER_BRANCH_ID == ownerbranchId)
                            && (string.IsNullOrEmpty(searchFilter.SRId) || sr.SR_NO.Contains(searchFilter.SRId))
                            && (string.IsNullOrEmpty(searchFilter.SRIsverify) || selectAllValue == searchFilter.SRIsverify || sr.SR_IS_VERIFY_PASS == searchFilter.SRIsverify)
                            && (sr.CREATE_DATE >= srMinDate)
                            && (sr.CREATE_DATE <= srMaxDate)
                            && (sr.SR_IS_VERIFY == true)
                            && (string.IsNullOrEmpty(searchFilter.Description) || sr.SR_REMARK.Contains(searchFilter.Description))
                            && (string.IsNullOrEmpty(searchFilter.SRId) || sr.SR_NO == searchFilter.SRId)
                         select new ExportVerifyEntity
                         {
                             SRId = sr.SR_NO,
                             AccountNo = ac.ACCOUNT_NO,
                             CustomerFistname = !string.IsNullOrEmpty(cs.FIRST_NAME_TH) ? cs.FIRST_NAME_TH : cs.FIRST_NAME_EN,
                             CustomerLastname = !string.IsNullOrEmpty(cs.FIRST_NAME_TH) ? cs.LAST_NAME_TH : cs.LAST_NAME_EN,
                             ProductGroupName = pg.PRODUCTGROUP_NAME,
                             SROwnerName = ou.POSITION_CODE + "-" + ou.FIRST_NAME + " " + ou.LAST_NAME,
                             SRCreateDate = sr.CREATE_DATE,
                             SRCreatorBranch = cb.BRANCH_NAME,
                             ProductName = pr.PRODUCT_NAME,
                             CampaignServiceName = cp.CAMPAIGNSERVICE_NAME,
                             TypeName = ty.TYPE_NAME,
                             AreaName = ar.AREA_NAME,
                             SubAreaName = sb.SUBAREA_NAME,
                             SRSubject = sr.SR_SUBJECT,
                             SRDescription = sr.SR_REMARK,
                             SRState = ste.SR_STATE_NAME,
                             SRStatus = st.SR_STATUS_NAME,
                             IsVerifyResult = sr.SR_IS_VERIFY_PASS,
                             TotalQuestion = (from Question in _context.TB_T_SR_VERIFY_RESULT_QUESTION.Where(c => c.TB_T_SR_VERIFY_RESULT_GROUP.SR_ID == sr.SR_ID) select Question).Count(),
                             TotalPass = (from Pass in _context.TB_T_SR_VERIFY_RESULT_QUESTION.Where(c => c.TB_T_SR_VERIFY_RESULT_GROUP.SR_ID == sr.SR_ID && c.RESULT == Constants.VerifyResultStatus.Pass) select Pass).Count(),
                             TotalFailed = (from Failed in _context.TB_T_SR_VERIFY_RESULT_QUESTION.Where(c => c.TB_T_SR_VERIFY_RESULT_GROUP.SR_ID == sr.SR_ID && c.RESULT == Constants.VerifyResultStatus.Fail) select Failed).Count(),
                             TotalDisregard = (from Disregard in _context.TB_T_SR_VERIFY_RESULT_QUESTION.Where(c => c.TB_T_SR_VERIFY_RESULT_GROUP.SR_ID == sr.SR_ID && c.RESULT == Constants.VerifyResultStatus.Skip) select Disregard).Count()
                         });

            return query.ToList();
        }

        public IList<ExportNcbEntity> GetExportNcb(ExportNcbSearchFilter searchFilter)
        {
            int? productGroupId = searchFilter.ProductGroup.ToNullable<int>();
            int? productId = searchFilter.Product.ToNullable<int>();
            int? campaignId = searchFilter.Campaign.ToNullable<int>();
            int? typeId = searchFilter.Type.ToNullable<int>();
            int? areaId = searchFilter.Area.ToNullable<int>();
            int? subareaId = searchFilter.SubArea.ToNullable<int>();
            int? ownerId = searchFilter.OwnerSR.ToNullable<int>();
            int? ownerbranchId = searchFilter.OwnerBranch.ToNullable<int>();
            int? slaId = searchFilter.Sla.ToNullable<int>();
            int? srstatusId = searchFilter.SRStatus.ToNullable<int>();
            int? upperbranchId = searchFilter.UpperBranch.ToNullable<int>();
            int? creatorbranchId = searchFilter.CreatorBranch.ToNullable<int>();
            int? creatorsrId = searchFilter.CreatorSR.ToNullable<int>();
            int? delegatebranchId = searchFilter.DelegateBranch.ToNullable<int>();
            int? delegatesrId = searchFilter.DelegateSR.ToNullable<int>();

            DateTime? srMinDate = null;
            DateTime? srMaxDate = null;

            if (searchFilter.SRDateTimeFromValue.HasValue && searchFilter.SRDateTimeToValue.HasValue)
            {
                srMinDate = searchFilter.SRDateTimeFromValue.Value;
                srMaxDate = searchFilter.SRDateTimeToValue.Value;
            }
            else
            {
                int? monthOfReportExport =
                    _context.TB_C_PARAMETER.Where(x => x.PARAMETER_NAME == Constants.ParameterName.ReportExportDate)
                        .FirstOrDefault().PARAMETER_VALUE.ToNullable<int>();

                srMinDate = DateTime.Now.Date.AddMonths(monthOfReportExport.Value * -1);
                srMaxDate = DateTime.Now.Date.AddDays(1);
            }

            #region "Find Role NCB"

            List<int> lstRole = _context.TB_C_ROLE.Where
                (o => (o.ROLE_CODE == Constants.SrRoleCode.NCB)
                ).Select(o => o.ROLE_ID).ToList();

            #endregion

            var query = (from sr in _context.TB_T_SR.AsNoTracking()
                         from cs in _context.TB_M_CONTACT.Where(c => c.CONTACT_ID == sr.CONTACT_ID).DefaultIfEmpty()
                         from pg in _context.TB_R_PRODUCTGROUP.Where(c => c.PRODUCTGROUP_ID == sr.PRODUCTGROUP_ID).DefaultIfEmpty()
                         from pr in _context.TB_R_PRODUCT.Where(c => c.PRODUCT_ID == sr.PRODUCT_ID).DefaultIfEmpty()
                         from cp in _context.TB_R_CAMPAIGNSERVICE.Where(c => c.CAMPAIGNSERVICE_ID == sr.CAMPAIGNSERVICE_ID).DefaultIfEmpty()
                         from ty in _context.TB_M_TYPE.Where(c => c.TYPE_ID == sr.TYPE_ID).DefaultIfEmpty()
                         from ar in _context.TB_M_AREA.Where(c => c.AREA_ID == sr.AREA_ID).DefaultIfEmpty()
                         from sb in _context.TB_M_SUBAREA.Where(c => c.SUBAREA_ID == sr.SUBAREA_ID).DefaultIfEmpty()
                         from ou in _context.TB_R_USER.Where(c => c.USER_ID == sr.OWNER_USER_ID).DefaultIfEmpty()
                         from cu in _context.TB_R_USER.Where(c => c.USER_ID == sr.CREATE_USER).DefaultIfEmpty()
                         from du in _context.TB_R_USER.Where(c => c.USER_ID == sr.DELEGATE_USER_ID).DefaultIfEmpty()
                         from st in _context.TB_C_SR_STATUS.Where(c => c.SR_STATUS_ID == sr.SR_STATUS_ID).DefaultIfEmpty()
                         from ste in _context.TB_C_SR_STATE.Where(c => c.SR_STATE_ID == st.SR_STATE_ID).DefaultIfEmpty()
                         from mk in _context.TB_R_USER.Where(c => c.USER_ID == sr.SR_NCB_MARKETING_USER_ID).DefaultIfEmpty()
                         where (sr.SR_PAGE_ID == Constants.SRPage.NCBPageId || (cu.ROLE_ID.HasValue && lstRole.Contains(cu.ROLE_ID.Value)))
                            && (string.IsNullOrEmpty(searchFilter.FirstName) || (cs.FIRST_NAME_TH.Contains(searchFilter.FirstName) || cs.FIRST_NAME_EN.ToUpper().Contains(searchFilter.FirstName.ToUpper())))
                            && (string.IsNullOrEmpty(searchFilter.LastName) || (cs.LAST_NAME_TH.Contains(searchFilter.LastName) || cs.LAST_NAME_TH.ToUpper().Contains(searchFilter.LastName.ToUpper())))
                            && (string.IsNullOrEmpty(searchFilter.CardId) || cs.CARD_NO.Contains(searchFilter.CardId))
                            && (!searchFilter.BirthDateValue.HasValue || sr.SR_NCB_CUSTOMER_BIRTHDATE == searchFilter.BirthDateValue.Value)
                            && (productGroupId == null || pg.PRODUCTGROUP_ID == productGroupId)
                            && (productId == null || pr.PRODUCT_ID == productId)
                            && (campaignId == null || cp.CAMPAIGNSERVICE_ID == campaignId)
                            && (typeId == null || ty.TYPE_ID == typeId)
                            && (areaId == null || ar.AREA_ID == areaId)
                            && (subareaId == null || sb.SUBAREA_ID == subareaId)
                            && (ownerId == null || sr.OWNER_USER_ID == ownerId)
                            && (ownerbranchId == null || sr.OWNER_BRANCH_ID == ownerbranchId)
                            && (string.IsNullOrEmpty(searchFilter.SRId) || sr.SR_NO.Contains(searchFilter.SRId))
                            && (sr.CREATE_DATE >= srMinDate)
                            && (sr.CREATE_DATE <= srMaxDate)
                            //&& (customerTypeId == null || customerTypeId == Constants.ApplicationStatus.All || ct.TYPE == customerTypeId)
                            && (slaId == null || slaId == Constants.ApplicationStatus.All
                                    || (slaId == 1 && (sr.RULE_THIS_ALERT == null || sr.RULE_THIS_ALERT == 0))
                                    || (slaId == 2 && (sr.RULE_THIS_ALERT.HasValue && sr.RULE_THIS_ALERT > 0))
                               )
                            && (srstatusId == null || srstatusId == Constants.ApplicationStatus.All || sr.SR_STATUS_ID == srstatusId)
                            && (upperbranchId == null || sr.SR_NCB_MARKETING_BRANCH_UPPER_1_ID == upperbranchId || sr.SR_NCB_MARKETING_BRANCH_UPPER_2_ID == upperbranchId || sr.SR_NCB_MARKETING_BRANCH_ID == upperbranchId)
                            && (creatorsrId == null || sr.CREATE_USER == creatorsrId)
                            && (creatorbranchId == null || sr.CREATE_BRANCH_ID == creatorbranchId)
                            && (delegatesrId == null || sr.DELEGATE_USER_ID == delegatesrId)
                            && (delegatebranchId == null || sr.DELEGATE_BRANCH_ID == delegatebranchId)
                         select new ExportNcbEntity
                         {
                             Sla = sr.RULE_TOTAL_ALERT.HasValue ? sr.RULE_TOTAL_ALERT.ToString() : "",
                             CustomerFistname = !string.IsNullOrEmpty(cs.FIRST_NAME_TH) ? cs.FIRST_NAME_TH : cs.FIRST_NAME_EN,
                             CustomerLastname = !string.IsNullOrEmpty(cs.FIRST_NAME_TH) ? cs.LAST_NAME_TH : cs.LAST_NAME_EN,
                             CardNo = cs.CARD_NO,
                             CustomerBirthDate = sr.SR_NCB_CUSTOMER_BIRTHDATE,
                             NcbCheckStatus = sr.SR_NCB_CHECK_STATUS,
                             SRId = sr.SR_NO,
                             SRState = ste.SR_STATE_NAME,
                             SRStatus = st.SR_STATUS_NAME,
                             ProductGroupName = pg.PRODUCTGROUP_NAME,
                             ProductName = pr.PRODUCT_NAME,
                             CampaignName = cp.CAMPAIGNSERVICE_NAME,
                             TypeName = ty.TYPE_NAME,
                             AreaName = ar.AREA_NAME,
                             SubAreaName = sb.SUBAREA_NAME,
                             SRCreator = cu.POSITION_CODE + "-" + cu.FIRST_NAME + " " + cu.LAST_NAME,
                             SRCreateDate = sr.CREATE_DATE,
                             SROwner = ou.POSITION_CODE + "-" + ou.FIRST_NAME + " " + ou.LAST_NAME,
                             OwnerUpdate = sr.UPDATE_DATE_BY_OWNER,
                             SRDelegate = du.POSITION_CODE + "-" + du.FIRST_NAME + " " + du.LAST_NAME,
                             SRDelegateUpdate = sr.UPDATE_DATE_BY_DELEGATE,
                             MKTUpperBranch1 = sr.SR_NCB_MARKETING_BRANCH_UPPER_1_NAME,
                             MKTUpperBranch2 = sr.SR_NCB_MARKETING_BRANCH_UPPER_2_NAME,
                             MKTEmployeeBranch = sr.SR_NCB_MARKETING_BRANCH_NAME,
                             MKTEmployeeName = mk.POSITION_CODE + "-" + mk.FIRST_NAME + " " + mk.LAST_NAME
                         });

            return query.ToList();
        }

        public List<SubscriptTypeEntity> GetActiveSubscriptType()
        {
            var query = from st in _context.TB_M_SUBSCRIPT_TYPE
                        orderby st.SUBSCRIPT_TYPE_NAME ascending
                        select new SubscriptTypeEntity
                        {
                            SubscriptTypeId = st.SUBSCRIPT_TYPE_ID,
                            SubscriptTypeName = st.SUBSCRIPT_TYPE_NAME,
                            SubscriptTypeCode = st.SUBSCRIPT_TYPE_CODE
                        };

            return query.Any() ? query.ToList() : null;
        }

        public string GetReportPath()
        {
            return _context.TB_C_PARAMETER.FirstOrDefault(x => x.PARAMETER_NAME == Constants.ParameterName.ReportPath).PARAMETER_VALUE;
        }

        public string GetTriggerDays()
        {
            return _context.TB_C_PARAMETER.FirstOrDefault(x => x.PARAMETER_NAME == Constants.ParameterName.TriggerDays).PARAMETER_VALUE;
        }

        public string GetNumOfDaySRReport()
        {
            return _context.TB_C_PARAMETER.FirstOrDefault(x => x.PARAMETER_NAME == Constants.ParameterName.NumDaysSRReport).PARAMETER_VALUE;
        }

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
