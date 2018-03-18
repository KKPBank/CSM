using System;
using System.Collections.Generic;
using CSM.Common.Utilities;
using CSM.Entity;
using log4net;
using System.Linq;
using CSM.Common.Resources;
using System.IO;
using System.Globalization;
using System.Data;

namespace CSM.Data.DataAccess
{
    public class CommPoolDataAccess : ICommPoolDataAccess
    {
        private readonly CSMContext _context;
        private ICommonDataAccess _commonDataAccess;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CommPoolDataAccess));

        public CommPoolDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        public List<StatusEntity> GetAllJobStatuses()
        {
            var query = from st in _context.TB_C_JOB_STATUS
                            //where st.STATUS_TYPE == Constants.StatusType.Job
                        select new StatusEntity
                        {
                            StatusValue = st.STATUS_VALUE,
                            StatusName = st.STATUS_NAME
                        };

            return query.ToList();
        }

        public IEnumerable<CommunicationPoolEntity> SearchJobs(CommPoolSearchFilter searchFilter)
        {
            int? branchId = searchFilter.User.BranchId;
            //string customerName = searchFilter.FirstName.NullSafeTrim();
            //string customerSurname = searchFilter.LastName.NullSafeTrim();
            //string ownerSR = searchFilter.OwnerSR.NullSafeTrim();
            string actionBy = searchFilter.ActionBy.NullSafeTrim();
            //string creatorSR = searchFilter.CreatorSR.NullSafeTrim();

            DateTime? minDate = null;
            DateTime? maxDate = null;
            DateTime? minJobDate = null;
            DateTime? maxJobDate = null;
            if (searchFilter.DateFromValue.HasValue)
            {
                minDate = searchFilter.DateFromValue.Value.Date;
            }
            if (searchFilter.DateToValue.HasValue)
            {
                maxDate = searchFilter.DateToValue.Value.Date.AddDays(1);
            }
            if (searchFilter.JobDateFromValue.HasValue)
            {
                minJobDate = searchFilter.JobDateFromValue.Value.Date;
            }
            if (searchFilter.JobDateToValue.HasValue)
            {
                maxJobDate = searchFilter.JobDateToValue.Value.Date.AddDays(1);
            }

            var query = from jb in _context.TB_T_JOB.AsNoTracking()
                        from sr in _context.TB_T_SR.Where(o => o.SR_ID == jb.SR_ID).DefaultIfEmpty()
                        from cs in _context.TB_M_CUSTOMER.Where(o => o.CUSTOMER_ID == sr.CUSTOMER_ID).DefaultIfEmpty()
                        from st in _context.TB_C_SR_STATUS.Where(o => o.SR_STATUS_ID == sr.SR_STATUS_ID).DefaultIfEmpty()
                        from ste in _context.TB_C_SR_STATE.Where(o => o.SR_STATE_ID == st.SR_STATE_ID).DefaultIfEmpty()
                        from us in _context.TB_R_USER.Where(o => o.USER_ID == jb.UPDATE_USER).DefaultIfEmpty()
                        from cr in _context.TB_R_USER.Where(o => o.USER_ID == sr.CREATE_USER).DefaultIfEmpty()
                        from ow in _context.TB_R_USER.Where(o => o.USER_ID == sr.OWNER_USER_ID).DefaultIfEmpty()
                        join pl in _context.TB_M_POOL on jb.POOL_ID equals pl.POOL_ID
                        join pb in _context.TB_M_POOL_BRANCH on pl.POOL_ID equals pb.POOL_ID
                        join ch in _context.TB_R_CHANNEL on jb.CHANNEL_ID equals ch.CHANNEL_ID
                        where (searchFilter.Channel == null || searchFilter.Channel == Constants.ApplicationStatus.All || jb.CHANNEL_ID == searchFilter.Channel)
                            && (string.IsNullOrEmpty(searchFilter.Subject) || jb.SUBJECT.ToUpper().Contains(searchFilter.Subject.ToUpper()))
                            && (string.IsNullOrEmpty(searchFilter.From) || jb.FROM.ToUpper().Contains(searchFilter.From.ToUpper()))
                            && (searchFilter.JobStatus == null || searchFilter.JobStatus == Constants.ApplicationStatus.All || jb.JOB_STATUS == searchFilter.JobStatus)
                            && (searchFilter.SRStatus == null || searchFilter.SRStatus == Constants.ApplicationStatus.All || st.SR_STATUS_ID == searchFilter.SRStatus)
                            && (searchFilter.SRId == null || sr.SR_NO == searchFilter.SRId)
                           //&& (string.IsNullOrEmpty(customerName) || cs.FIRST_NAME_TH.Contains(customerName) || cs.FIRST_NAME_EN.Contains(customerName)
                           //    || string.IsNullOrEmpty(customerSurname) || cs.LAST_NAME_TH.Contains(customerSurname) || cs.LAST_NAME_EN.Contains(customerSurname))
                           //&& (string.IsNullOrEmpty(creatorSR) || cr.FIRST_NAME.Contains(creatorSR) || cr.LAST_NAME.Contains(creatorSR))
                           //&& (string.IsNullOrEmpty(ownerSR) || ow.FIRST_NAME.Contains(ownerSR) || ow.LAST_NAME.Contains(ownerSR))
                           && (!searchFilter.OwnerBranchId.HasValue || sr.OWNER_BRANCH_ID == searchFilter.OwnerBranchId)
                           && (!searchFilter.OwnerUserId.HasValue || sr.OWNER_USER_ID == searchFilter.OwnerUserId)
                           && (!searchFilter.CreatorBranchId.HasValue || sr.CREATE_BRANCH_ID == searchFilter.CreatorBranchId)
                           && (!searchFilter.CreatorUserId.HasValue || sr.CREATE_USER == searchFilter.CreatorUserId)
                           && (string.IsNullOrEmpty(actionBy) || us.FIRST_NAME.Contains(actionBy) || us.LAST_NAME.Contains(actionBy))
                           && (!searchFilter.DateFromValue.HasValue || jb.UPDATE_DATE >= minDate)
                           && (!searchFilter.DateToValue.HasValue || jb.UPDATE_DATE <= maxDate)
                           && (!searchFilter.JobDateFromValue.HasValue || jb.JOB_DATE >= minJobDate)
                           && (!searchFilter.JobDateToValue.HasValue || jb.JOB_DATE <= maxJobDate)
                           && pb.BRANCH_ID == branchId
                           && jb.CHANNEL_ID != null
                           && (searchFilter.SRState == null || ste.SR_STATE_ID == searchFilter.SRState)
                        select new CommunicationPoolEntity
                        {
                            JobId = jb.JOB_ID,
                            SenderAddress = jb.FROM,
                            SenderName = jb.SENDER_NAME,
                            Subject = jb.SUBJECT,
                            Content = jb.CONTENT,
                            SequenceNo = 1,
                            Status = jb.JOB_STATUS,
                            Remark = jb.REMARK,
                            CreatedDate = jb.JOB_DATE,
                            UpdateDate = jb.UPDATE_DATE,
                            UpdateUser = (us != null ? new UserEntity
                            {
                                Firstname = us.FIRST_NAME,
                                Lastname = us.LAST_NAME,
                                PositionCode = us.POSITION_CODE
                            } : null),
                            ServiceRequest = (sr != null ? new ServiceRequestEntity
                            {
                                SrNo = sr.SR_NO,
                                SRStateName = ste.SR_STATE_NAME,
                                StatusDisplay = st.SR_STATUS_NAME,
                                Customer = (cs != null ? new CustomerEntity
                                {
                                    FirstNameThai = cs.FIRST_NAME_TH,
                                    LastNameThai = cs.LAST_NAME_TH,
                                    FirstNameEnglish = cs.FIRST_NAME_EN,
                                    LastNameEnglish = cs.LAST_NAME_EN,
                                    FirstNameThaiEng = !string.IsNullOrEmpty(cs.FIRST_NAME_TH) ? cs.FIRST_NAME_TH : cs.FIRST_NAME_EN,
                                    LastNameThaiEng = !string.IsNullOrEmpty(cs.FIRST_NAME_TH) ? cs.LAST_NAME_TH : cs.LAST_NAME_EN,
                                } : null),
                                CreateUser = (cr != null ? new UserEntity
                                {
                                    Firstname = cr.FIRST_NAME,
                                    Lastname = cr.LAST_NAME,
                                    PositionCode = cr.POSITION_CODE
                                } : null),
                                Owner = (ow != null ? new UserEntity
                                {
                                    Firstname = ow.FIRST_NAME,
                                    Lastname = ow.LAST_NAME,
                                    PositionCode = ow.POSITION_CODE
                                } : null),
                            } : null),
                            ChannelEntity = (ch != null ? new ChannelEntity
                            {
                                ChannelId = ch.CHANNEL_ID,
                                Name = ch.CHANNEL_NAME
                            } : null),
                            PoolEntity = (pl != null ? new PoolEntity
                            {
                                PoolName = pl.POOL_NAME
                            } : null),
                            //Attachments = (from jba in jb.TB_T_JOB_ATTACHMENT
                            //               where jba.JOB_ID == jb.JOB_ID
                            //               select new AttachmentEntity
                            //               {
                            //                   Filename = jba.FILE_NAME,
                            //                   ContentType = jba.CONTENT_TYPE
                            //               }).ToList()
                            AttachmentsDisplay = jb.TB_T_JOB_ATTACHMENT.Any() ? "Yes" : "No"
                        };

            // Wildcard filter by
            query = WildcardFilterBy(query, searchFilter);

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = query.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }
            query = SetJobListSort(query, searchFilter);
            return query.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<CommunicationPoolEntity>();
        }

        public CommunicationPoolEntity GetJob(int jobId)
        {
            var query = from jb in _context.TB_T_JOB
                        from sr in _context.TB_T_SR.Where(o => o.SR_ID == jb.SR_ID).DefaultIfEmpty()
                        from cs in _context.TB_M_CUSTOMER.Where(o => o.CUSTOMER_ID == sr.CUSTOMER_ID).DefaultIfEmpty()
                        from st in _context.TB_C_SR_STATUS.Where(o => o.SR_STATUS_ID == sr.SR_STATUS_ID).DefaultIfEmpty()
                        from ste in _context.TB_C_SR_STATE.Where(o => o.SR_STATE_ID == st.SR_STATE_ID).DefaultIfEmpty()
                        from us in _context.TB_R_USER.Where(o => o.USER_ID == jb.UPDATE_USER).DefaultIfEmpty()
                        from cr in _context.TB_R_USER.Where(o => o.USER_ID == sr.CREATE_USER).DefaultIfEmpty()
                        from ow in _context.TB_R_USER.Where(o => o.USER_ID == sr.OWNER_USER_ID).DefaultIfEmpty()
                        from dl in _context.TB_R_USER.Where(o => o.USER_ID == sr.DELEGATE_USER_ID).DefaultIfEmpty()
                        from pg in _context.TB_R_PRODUCTGROUP.Where(o => o.PRODUCTGROUP_ID == sr.PRODUCTGROUP_ID).DefaultIfEmpty()
                        from pd in _context.TB_R_PRODUCT.Where(o => o.PRODUCT_ID == sr.PRODUCT_ID).DefaultIfEmpty()
                        from ty in _context.TB_M_TYPE.Where(o => o.TYPE_ID == sr.TYPE_ID).DefaultIfEmpty()
                        from sb in _context.TB_M_SUBAREA.Where(o => o.SUBAREA_ID == sr.SUBAREA_ID).DefaultIfEmpty()
                        join pl in _context.TB_M_POOL on jb.POOL_ID equals pl.POOL_ID
                        join jc in _context.TB_R_CHANNEL on jb.CHANNEL_ID equals jc.CHANNEL_ID
                        from js in _context.TB_R_CHANNEL.Where(o => o.CHANNEL_ID == sr.CHANNEL_ID).DefaultIfEmpty()
                        from me in _context.TB_M_MEDIA_SOURCE.Where(o => o.MEDIA_SOURCE_ID == sr.MEDIA_SOURCE_ID).DefaultIfEmpty()

                        where jb.JOB_ID == jobId

                        select new CommunicationPoolEntity
                        {
                            JobId = jb.JOB_ID,
                            SenderAddress = jb.FROM,
                            SenderName = jb.SENDER_NAME,
                            Subject = jb.SUBJECT,
                            Content = jb.CONTENT,
                            SequenceNo = 1,
                            Status = jb.JOB_STATUS,
                            CreatedDate = jb.JOB_DATE,
                            UpdateDate = jb.UPDATE_DATE,
                            Remark = jb.REMARK,

                            UpdateUser = (us != null ? new UserEntity
                            {
                                Firstname = us.FIRST_NAME,
                                Lastname = us.LAST_NAME,
                                PositionCode = us.POSITION_CODE
                            } : null),
                            ServiceRequest = (sr != null ? new ServiceRequestEntity
                            {
                                SrNo = sr.SR_NO,
                                SRStateName = ste.SR_STATE_NAME,
                                StatusDisplay = st.SR_STATUS_NAME,
                                Subject = sr.SR_SUBJECT,
                                Remark = sr.SR_REMARK,
                                CreateDate = sr.CREATE_DATE,
                                ChannelName = js.CHANNEL_NAME,
                                MediaSourceName = me.MEDIA_SOURCE_NAME,

                                Customer = (cs != null ? new CustomerEntity
                                {
                                    FirstNameThai = cs.FIRST_NAME_TH,
                                    LastNameThai = cs.LAST_NAME_TH,
                                    FirstNameEnglish = cs.FIRST_NAME_EN,
                                    LastNameEnglish = cs.LAST_NAME_EN
                                } : null),

                                ProductMapping = (pg != null ? new ProductSREntity
                                {
                                    ProductGroupName = pg.PRODUCTGROUP_NAME,
                                    ProductName = pd.PRODUCT_NAME,
                                    TypeName = ty.TYPE_NAME,
                                    SubAreaName = sb.SUBAREA_NAME
                                } : null),
                                CreateUser = (cr != null ? new UserEntity
                                {
                                    Firstname = cr.FIRST_NAME,
                                    Lastname = cr.LAST_NAME,
                                    PositionCode = cr.POSITION_CODE
                                } : null),
                                Owner = (ow != null ? new UserEntity
                                {
                                    Firstname = ow.FIRST_NAME,
                                    Lastname = ow.LAST_NAME,
                                    PositionCode = ow.POSITION_CODE
                                } : null),
                                Delegator = (dl != null ? new UserEntity
                                {
                                    Firstname = dl.FIRST_NAME,
                                    Lastname = dl.LAST_NAME,
                                    PositionCode = dl.POSITION_CODE
                                } : null)
                            } : null),

                            PoolEntity = (pl != null ? new PoolEntity
                            {
                                PoolName = pl.POOL_NAME,
                            } : null),
                            ChannelEntity = (jc != null ? new ChannelEntity
                            {
                                ChannelId = jc.CHANNEL_ID,
                                Name = jc.CHANNEL_NAME
                            } : null),
                        };

            CommunicationPoolEntity commPoolEntity = new CommunicationPoolEntity();

            if (query.Any())
            {
                commPoolEntity = query.FirstOrDefault();
                var lstAtt = (from at in _context.TB_T_JOB_ATTACHMENT
                              where at.JOB_ID == jobId
                              orderby at.FILE_NAME ascending
                              select new AttachmentEntity
                              {
                                  AttachmentId = at.JOB_ATTACHMENT_ID,
                                  Filename = at.FILE_NAME,
                                  CreateDate = at.CREATE_DATE,
                                  ContentType = at.CONTENT_TYPE,
                                  Url = at.URL
                              }).ToList();

                commPoolEntity.Attachments = lstAtt;
            }
            return commPoolEntity;
        }

        public bool AddMailContent(List<CommunicationPoolEntity> mailMessages, ref int refNumOfSR, ref int refNumOfFax, ref int refNumOfWeb, ref int refNumOfEmail)
        {
            try
            {
                _context.Configuration.AutoDetectChangesEnabled = false;

                if (mailMessages != null && mailMessages.Any(x => x.IsDeleted))
                {
                    var docfolderEntity = _context.TB_C_PARAMETER.FirstOrDefault(x => x.PARAMETER_NAME == Constants.ParameterName.AttachmentPathJob);
                    string documentFolder = docfolderEntity != null ? docfolderEntity.PARAMETER_VALUE : string.Empty;

                    foreach (CommunicationPoolEntity mail in mailMessages.Where(x => x.IsDeleted))
                    {
                        using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                        {
                            try
                            {
                                TB_T_SR sr = _context.TB_T_SR.FirstOrDefault(x => !string.IsNullOrEmpty(mail.SRNo) && x.SR_NO == mail.SRNo);
                                TB_C_SR_STATUS srStatus = _context.TB_C_SR_STATUS.FirstOrDefault(x => !string.IsNullOrEmpty(mail.SRStatusCode)
                                    && x.SR_STATUS_CODE == mail.SRStatusCode);

                                #region "Find UserId by SenderAddress"

                                int? createUserId = null;
                                string createUserName = string.Empty;

                                if (!string.IsNullOrWhiteSpace(mail.SenderAddress))
                                {
                                    createUserName = ApplicationHelpers.GetSenderAddress(mail.SenderAddress);
                                    var usr = _context.TB_R_USER.FirstOrDefault(x => !string.IsNullOrEmpty(createUserName) && x.USERNAME.ToUpper() == createUserName.ToUpper());

                                    if (usr != null)
                                    {
                                        createUserId = usr.USER_ID;
                                        createUserName = null;
                                    }
                                }

                                #endregion

                                if (sr != null)
                                {
                                    #region "Save Job"

                                    TB_T_JOB commPool = new TB_T_JOB();
                                    commPool.SUBJECT = mail.Subject;
                                    commPool.CONTENT = mail.Content;
                                    commPool.JOB_DATE = DateTime.Now;
                                    //commPool.UPDATE_DATE = DateTime.Now;
                                    commPool.POOL_ID = mail.PoolEntity.PoolId;
                                    commPool.CREATE_USER = createUserId;
                                    commPool.CREATE_USERNAME = createUserName;
                                    commPool.CREATE_DATE = DateTime.Now;
                                    commPool.FROM = mail.SenderAddress;
                                    commPool.SENDER_NAME = mail.SenderName;
                                    _context.TB_T_JOB.Add(commPool);
                                    this.Save();

                                    // Save Mail Attachments
                                    if (mail.Attachments != null && mail.Attachments.Count > 0)
                                    {
                                        this.AddAttachment(commPool.JOB_ID, mail.Attachments, documentFolder);
                                    }

                                    #endregion

                                    TB_T_SR_REPLY_EMAIL srReplyEmail = new TB_T_SR_REPLY_EMAIL();
                                    srReplyEmail.SR_ID = sr.SR_ID;
                                    srReplyEmail.CREATE_DATE = DateTime.Now;
                                    srReplyEmail.JOB_ID = commPool.JOB_ID; // FK

                                    if (srStatus != null)
                                    {
                                        srReplyEmail.SR_STATUS_ID = srStatus.SR_STATUS_ID;
                                    }

                                    _context.TB_T_SR_REPLY_EMAIL.Add(srReplyEmail);

                                    // Updated jobId to mailList
                                    mail.JobId = commPool.JOB_ID;
                                    mail.SRId = srReplyEmail.SR_ID;

                                    if (this.Save() > 0)
                                    {
                                        refNumOfSR += 1;
                                    }
                                    else
                                    {
                                        Logger.ErrorFormat("Failed to save SR ID({0})", mail.SRNo);
                                    }
                                }
                                else
                                {
                                    TB_T_JOB commPool = new TB_T_JOB();
                                    commPool.SUBJECT = mail.Subject;
                                    commPool.FROM = Constants.ChannelCode.KKWebSite.Equals(mail.ChannelEntity.Code) ? mail.ContactFullName : mail.SenderAddress;
                                    commPool.SENDER_NAME = Constants.ChannelCode.KKWebSite.Equals(mail.ChannelEntity.Code) ? mail.ContactFullName : mail.SenderName;
                                    commPool.CONTENT = mail.Content;
                                    commPool.CHANNEL_ID = mail.ChannelEntity.ChannelId;
                                    commPool.JOB_DATE = DateTime.Now;
                                    //commPool.UPDATE_DATE = DateTime.Now;
                                    commPool.POOL_ID = mail.PoolEntity.PoolId;
                                    commPool.JOB_STATUS = Constants.JobStatus.Open;
                                    // Create User
                                    commPool.CREATE_USER = createUserId;
                                    commPool.CREATE_USERNAME = createUserName;
                                    commPool.CREATE_DATE = DateTime.Now;
                                    _context.TB_T_JOB.Add(commPool);

                                    if (this.Save() > 0)
                                    {
                                        switch (mail.ChannelEntity.Code)
                                        {
                                            case Constants.ChannelCode.Fax:
                                                refNumOfFax += 1;
                                                break;
                                            case Constants.ChannelCode.KKWebSite:
                                                refNumOfWeb += 1;
                                                break;
                                            case Constants.ChannelCode.Email:
                                                refNumOfEmail += 1;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        Logger.ErrorFormat("Failed to save Job({0})", mail.ChannelEntity.Code);
                                    }

                                    // Updated jobId to mailList
                                    mail.JobId = commPool.JOB_ID;

                                    // Save Mail Attachments
                                    if (mail.Attachments != null && mail.Attachments.Count > 0)
                                    {
                                        this.AddAttachment(commPool.JOB_ID, mail.Attachments, documentFolder);
                                    }
                                }

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                mail.IsDeleted = false;

                                switch (mail.ChannelEntity.Code)
                                {
                                    case Constants.ChannelCode.Fax:
                                        refNumOfFax -= 1;
                                        break;
                                    case Constants.ChannelCode.KKWebSite:
                                        refNumOfWeb -= 1;
                                        break;
                                    case Constants.ChannelCode.Email:
                                        refNumOfEmail -= 1;
                                        break;
                                    default:
                                        break;
                                }

                                transaction.Rollback();
                                Logger.Error("Exception occur:\n", ex);
                            }
                        }
                    }
                }

                return true;
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public bool DeleteMailContent(CommunicationPoolEntity mail, dynamic taskResult)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        string path;
                        var docfolderEntity = _context.TB_C_PARAMETER.FirstOrDefault(x => x.PARAMETER_NAME == Constants.ParameterName.AttachmentPathJob);
                        string documentFolder = docfolderEntity != null ? docfolderEntity.PARAMETER_VALUE : string.Empty;

                        var attachments = _context.TB_T_JOB_ATTACHMENT.Where(x => x.JOB_ID == mail.JobId);

                        if (attachments.Any())
                        {
                            foreach (var attach in attachments)
                            {
                                path = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", documentFolder, attach.FILE_NAME);

                                if (StreamDataHelpers.TryToDelete(path))
                                {
                                    _context.TB_T_JOB_ATTACHMENT.Remove(attach);
                                }
                            }

                            this.Save();
                        }

                        if (mail.SRId != null)
                        {
                            var srReplyEmail = _context.TB_T_SR_REPLY_EMAIL.FirstOrDefault(x => x.JOB_ID == mail.JobId && x.SR_ID == mail.SRId);
                            if (srReplyEmail != null)
                            {
                                _context.TB_T_SR_REPLY_EMAIL.Remove(srReplyEmail);

                                if (this.Save() > 0)
                                {
                                    taskResult.NumOfSR -= 1;
                                }
                            }
                        }

                        var job = _context.TB_T_JOB.FirstOrDefault(x => x.JOB_ID == mail.JobId);
                        if (job != null)
                        {
                            _context.TB_T_JOB.Remove(job);

                            if (this.Save() > 0)
                            {
                                switch (mail.ChannelEntity.Code)
                                {
                                    case Constants.ChannelCode.Fax:
                                        taskResult.NumOfFax -= 1;
                                        break;
                                    case Constants.ChannelCode.KKWebSite:
                                        taskResult.NumOfKKWebSite -= 1;
                                        break;
                                    case Constants.ChannelCode.Email:
                                        taskResult.NumOfEmail -= 1;
                                        break;
                                    default:
                                        break;
                                }

                                taskResult.NumFailedDelete += 1;
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        private void AddAttachment(int jobId, IEnumerable<AttachmentEntity> attachments, string documentFolder)
        {
            if (attachments != null && attachments.Any())
            {
                _commonDataAccess = new CommonDataAccess(_context);

                foreach (AttachmentEntity attachment in attachments)
                {
                    int pos = attachment.Filename.LastIndexOf('.');
                    string fileExtension = pos != -1 ? attachment.Filename.Substring(pos) : Constants.UnknownFileExt;
                    int seqNo = _commonDataAccess.GetNextAttachmentSeq();
                    string fileNameUrl = ApplicationHelpers.GenerateFileName(documentFolder, fileExtension, seqNo,
                        Constants.AttachmentPrefix.Job);
                    string path = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", documentFolder, fileNameUrl);
                    bool success = StreamDataHelpers.ByteArrayToFile(path, attachment.ByteArray);

                    Logger.DebugFormat("I:--START--:--Add Attachment--:FileExtension/{0}:SeqNo:/{1}:FileNameUrl/{2}:Path/{3}", fileExtension, seqNo, fileNameUrl, path);

                    if (success)
                    {
                        TB_T_JOB_ATTACHMENT dbAttachment = new TB_T_JOB_ATTACHMENT
                        {
                            JOB_ID = jobId,
                            FILE_NAME = attachment.Filename,
                            URL = fileNameUrl,
                            CONTENT_TYPE = attachment.ContentType,
                            CREATE_DATE = DateTime.Now
                        };

                        _context.TB_T_JOB_ATTACHMENT.Add(dbAttachment);
                    }
                    else
                    {
                        throw new CustomException(string.Format(CultureInfo.InvariantCulture, "Failed to save {0}", attachment.Filename));
                    }
                }

                this.Save();
            }
        }

        public IEnumerable<PoolEntity> GetPoolList(PoolSearchFilter searchFilter)
        {
            var poolList = from pl in _context.TB_M_POOL.AsNoTracking()
                           from pb in _context.TB_M_POOL_BRANCH.Where(x => x.POOL_ID == pl.POOL_ID).DefaultIfEmpty()
                           where (string.IsNullOrEmpty(searchFilter.PoolName) || pl.POOL_NAME.ToUpper().Contains(searchFilter.PoolName.ToUpper()))
                               && (string.IsNullOrEmpty(searchFilter.PoolDesc) || pl.POOL_DESC.ToUpper().Contains(searchFilter.PoolDesc.ToUpper()))
                               && (string.IsNullOrEmpty(searchFilter.Email) || pl.EMAIL.ToUpper().Contains(searchFilter.Email.ToUpper()))
                               && (searchFilter.BranchId == null || (pb != null && searchFilter.BranchId != null && pb.BRANCH_ID == searchFilter.BranchId))
                           group pl.POOL_ID by pl into g
                           select new PoolEntity
                           {
                               PoolId = g.Key.POOL_ID,
                               PoolName = g.Key.POOL_NAME,
                               PoolDesc = g.Key.POOL_DESC,
                               Email = g.Key.EMAIL,
                               NumOfJob = (from jb in _context.TB_T_JOB
                                           where jb.JOB_STATUS == Constants.JobStatus.Open && jb.POOL_ID == g.Key.POOL_ID
                                           select jb.JOB_ID).Count()
                           };

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = poolList.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            poolList = SetPoolListSort(poolList, searchFilter);
            return poolList.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<PoolEntity>();
        }

        public List<PoolBranchEntity> GetPoolBranchList(int poolId)
        {
            var query = from pb in _context.TB_M_POOL_BRANCH
                        join br in _context.TB_R_BRANCH on pb.BRANCH_ID equals br.BRANCH_ID
                        where pb.POOL_ID == poolId && pb.STATUS == Constants.ApplicationStatus.Active
                        select new PoolBranchEntity
                        {
                            BranchId = pb.BRANCH_ID,
                            BranchName = br.BRANCH_NAME,
                            PoolId = pb.POOL_ID,
                            IsDelete = pb.STATUS == Constants.ApplicationStatus.Active ? false : true
                        };

            return query.ToList();
        }

        public List<PoolBranchEntity> GetPoolBranchList(List<PoolBranchEntity> poolBranches)
        {
            var query = from tmp in poolBranches
                        join br in _context.TB_R_BRANCH on tmp.BranchId equals br.BRANCH_ID
                        from pb in _context.TB_M_POOL_BRANCH.Where(x => tmp.PoolId != null && x.POOL_ID == tmp.PoolId && x.BRANCH_ID == tmp.BranchId).DefaultIfEmpty()
                        select new PoolBranchEntity
                        {
                            BranchId = br.BRANCH_ID,
                            BranchName = br.BRANCH_NAME,
                            PoolId = pb != null ? pb.POOL_ID : null,
                            IsDelete = tmp.IsDelete
                        };

            return query.ToList();
        }

        public bool IsDuplicateCommPool(PoolEntity poolEntity)
        {

            var cnt = _context.TB_M_POOL.Where(
                            x => x.EMAIL.ToUpper() == poolEntity.Email.ToUpper()
                                 && x.POOL_ID != poolEntity.PoolId
                                 ).Count();

            return cnt > 0;

        }

        public bool SaveCommPool(PoolEntity poolEntity, List<PoolBranchEntity> poolBranches)
        {
            try
            {
                var today = DateTime.Now;

                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        TB_M_POOL pool = null;
                        if (poolEntity.PoolId == null || poolEntity.PoolId == 0)
                        {
                            pool = new TB_M_POOL();
                            pool.POOL_NAME = poolEntity.PoolName;
                            pool.POOL_DESC = poolEntity.PoolDesc;
                            pool.EMAIL = poolEntity.Email;
                            pool.STATUS = poolEntity.Status;
                            pool.PASSWORD = poolEntity.Password;
                            pool.CREATE_USER = poolEntity.CreateUser.UserId;
                            pool.CREATE_DATE = today;
                            _context.TB_M_POOL.Add(pool);
                            this.Save();
                        }
                        else
                        {
                            pool = _context.TB_M_POOL.FirstOrDefault(x => x.POOL_ID == poolEntity.PoolId);
                            if (pool != null)
                            {
                                pool.POOL_NAME = poolEntity.PoolName;
                                pool.POOL_DESC = poolEntity.PoolDesc;
                                pool.EMAIL = poolEntity.Email;
                                pool.STATUS = poolEntity.Status;
                                if (!string.IsNullOrWhiteSpace(poolEntity.Password))
                                {
                                    pool.PASSWORD = poolEntity.Password;
                                }
                                pool.UPDATE_USER = poolEntity.UpdateUser.UserId;
                                pool.UPDATE_DATE = today;
                                SetEntryStateModified(pool);
                            }
                            else
                            {
                                Logger.ErrorFormat("Pool ID: {0} does not exist", poolEntity.PoolId);
                            }
                        }

                        if (poolBranches != null && poolBranches.Count > 0 && pool != null)
                        {
                            this.SavePoolBranches(pool.POOL_ID, poolBranches);
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        public PoolEntity GetPoolByID(int commPoolId)
        {
            var query = from p in _context.TB_M_POOL
                        from cs in _context.TB_R_USER.Where(o => o.USER_ID == p.CREATE_USER).DefaultIfEmpty()
                        from us in _context.TB_R_USER.Where(o => o.USER_ID == p.UPDATE_USER).DefaultIfEmpty()
                        where p.POOL_ID == commPoolId
                        select new PoolEntity
                        {
                            PoolId = p.POOL_ID,
                            PoolName = p.POOL_NAME,
                            PoolDesc = p.POOL_DESC,
                            Email = p.EMAIL,
                            Status = p.STATUS,
                            CreatedDate = p.CREATE_DATE,
                            Updatedate = p.UPDATE_DATE,
                            CreateUser = cs != null ? new UserEntity
                            {
                                Firstname = cs.FIRST_NAME,
                                Lastname = cs.LAST_NAME,
                                PositionCode = cs.POSITION_CODE
                            } : null,
                            UpdateUser = us != null ? new UserEntity
                            {
                                Firstname = us.FIRST_NAME,
                                Lastname = us.LAST_NAME,
                                PositionCode = us.POSITION_CODE
                            } : null
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        public List<PoolEntity> GetActivePoolList()
        {
            var query = from pl in _context.TB_M_POOL
                        where pl.STATUS == Constants.ApplicationStatus.Active
                        select new PoolEntity
                        {
                            PoolId = pl.POOL_ID,
                            PoolName = pl.POOL_NAME,
                            PoolDesc = pl.POOL_DESC,
                            Email = pl.EMAIL,
                            Status = pl.STATUS,
                            Password = pl.PASSWORD
                        };

            return query.Any() ? query.ToList() : null;
        }

        public List<ChannelEntity> GetActiveChannels()
        {
            var query = from ch in _context.TB_R_CHANNEL
                        where ch.STATUS == Constants.ApplicationStatus.Active
                        orderby ch.CHANNEL_NAME ascending
                        select new ChannelEntity
                        {
                            ChannelId = ch.CHANNEL_ID,
                            Code = ch.CHANNEL_CODE,
                            Name = ch.CHANNEL_NAME,
                            Email = ch.EMAIL
                        };

            return query.Any() ? query.ToList() : null;
        }

        public List<StatusEntity> GetAllSRStatuses()
        {
            var query = from st in _context.TB_C_SR_STATUS
                        select new StatusEntity
                        {
                            StatusValue = st.SR_STATUS_ID,
                            StatusName = st.SR_STATUS_NAME
                        };

            return query.ToList();
        }

        public bool UpdateJob(int? jobId, int userId, int? status, string remark)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var job = _context.TB_T_JOB.FirstOrDefault(x => x.JOB_ID == jobId && x.JOB_STATUS == status);

                if (job != null)
                {
                    job.JOB_STATUS = Constants.JobStatus.Done;
                    job.UPDATE_DATE = DateTime.Now;
                    job.UPDATE_USER = userId;
                    job.REMARK = remark;

                    SetEntryStateModified(job);
                    this.Save();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }

            return false;
        }

        public bool SaveNewSR(int jobId, int userId, ref int srId)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _commonDataAccess = new CommonDataAccess(_context);
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    Logger.DebugFormat("I:--START--:--Save New SR--:Job Id/{0}:User Id/{1}", jobId, userId);

                    try
                    {
                        #region "Get Job and Attachment"

                        var jobFolderEntity = _context.TB_C_PARAMETER.FirstOrDefault(x => x.PARAMETER_NAME == Constants.ParameterName.AttachmentPathJob);
                        string jobFolder = jobFolderEntity != null ? jobFolderEntity.PARAMETER_VALUE : string.Empty;

                        var job = _context.TB_T_JOB.FirstOrDefault(x => x.JOB_ID == jobId);
                        var jobAttachments = _context.TB_T_JOB_ATTACHMENT.Where(x => x.JOB_ID == jobId);

                        #endregion

                        #region "Get User Branch"

                        var user = _context.TB_R_USER.FirstOrDefault(x => x.USER_ID == userId);

                        #endregion

                        #region "Get SR Attachment"

                        var srFolderEntity = _context.TB_C_PARAMETER.FirstOrDefault(x => x.PARAMETER_NAME == Constants.ParameterName.AttachmentPathSr);
                        string srFolder = srFolderEntity != null ? srFolderEntity.PARAMETER_VALUE : string.Empty;

                        #endregion

                        var today = DateTime.Now;
                        var status = _context.TB_C_SR_STATUS.FirstOrDefault(x => Constants.SRStatusCode.Draft.Equals(x.SR_STATUS_CODE));

                        TB_T_SR newSR = new TB_T_SR();
                        newSR.SR_NO = string.Empty;
                        newSR.CREATE_USER = userId;
                        newSR.CREATE_DATE = today;
                        newSR.UPDATE_DATE = today;

                        newSR.CREATE_BRANCH_ID = user.BRANCH_ID;
                        newSR.CHANNEL_ID = job.CHANNEL_ID;
                        newSR.SR_SUBJECT = job.SUBJECT;
                        newSR.SR_REMARK = job.CONTENT;

                        newSR.SR_STATUS_ID = status != null ? status.SR_STATUS_ID : new Nullable<int>();

                        #region "jobAttachments"


                        if (jobAttachments.Any())
                        {
                            List<SrAttachmentEntity> lstSrAttachment = new List<SrAttachmentEntity>();

                            foreach (var jobAttach in jobAttachments)
                            {
                                var jobFileName = jobAttach.FILE_NAME;
                                var jobFileNameUrl = jobAttach.URL;
                                string jobFilePath = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", jobFolder, jobFileNameUrl);
                                var fileInfo = new FileInfo(jobFilePath);

                                int seqNo = _commonDataAccess.GetNextAttachmentSeq();
                                string srAttachName = Path.GetFileNameWithoutExtension(jobFileName);
                                var srFileNameUrl = ApplicationHelpers.GenerateFileName(srFolder, Path.GetExtension(jobFileName), seqNo, Constants.AttachmentPrefix.Sr);
                                string srFilePath = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", srFolder, srFileNameUrl);

                                if (fileInfo != null && fileInfo.Length > 0)
                                {
                                    if (StreamDataHelpers.TryToCopy(jobFilePath, srFilePath))
                                    {
                                        #region "comment out"
                                        //var srAttachment = new TB_T_SR_ATTACHMENT
                                        //{
                                        //    SR_ID = newSR.SR_ID,
                                        //    FILE_SIZE = (int) fileInfo.Length,
                                        //    SR_ATTACHMENT_FILE_NAME = jobFileName,
                                        //    SR_ATTACHMENT_CONTENT_TYPE = jobAttach.CONTENT_TYPE,
                                        //    SR_ATTACHMENT_DESC = jobAttach.ATTACHMENT_DESC,
                                        //    SR_ATTACHMENT_URL = srFileNameUrl,
                                        //    SR_ATTACHMENT_NAME = srAttachName,
                                        //    CREATE_USER = userId,
                                        //    CREATE_DATE = today,
                                        //    UPDATE_DATE = today
                                        //};

                                        //_context.TB_T_SR_ATTACHMENT.Add(srAttachment);

                                        #endregion

                                        var defaDocType = _context.TB_M_DOCUMENT_TYPE
                                                            .Where(d => d.CATEGORY == 2 && d.STATUS == 1 && d.DOCUMENT_TYPE_NAME == "อื่นๆ")
                                                            .FirstOrDefault();

                                        List<AttachmentTypeEntity> attTypeList = new List<AttachmentTypeEntity>();
                                        if (defaDocType != null)
                                        {
                                            attTypeList.Add(new AttachmentTypeEntity()
                                            {
                                                DocTypeId = defaDocType.DOCUMENT_TYPE_ID,
                                                Name = defaDocType.DOCUMENT_TYPE_NAME,
                                                Code = defaDocType.DOCUMENT_TYPE_CODE,
                                                Status = defaDocType.STATUS
                                            });
                                        }

                                        var srAttachment = new SrAttachmentEntity
                                        {
                                            Url = srFileNameUrl,
                                            Filename = jobFileName,
                                            Name = srAttachName,
                                            DocDesc = jobAttach.ATTACHMENT_DESC,
                                            ContentType = jobAttach.CONTENT_TYPE,
                                            DocumentType = attTypeList,
                                            CreateDate = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime),
                                            IsEditable = true,
                                            FileSize = (int)fileInfo.Length,
                                            CreateUserId = userId.ToString(),
                                            Status = Constants.ApplicationStatus.Active,
                                            AttachToEmail = ""

                                        };

                                        lstSrAttachment.Add(srAttachment);
                                    }
                                    else
                                    {
                                        Logger.DebugFormat("I:--FAILED--:--Cannot copy file {0} to SR Attachment--", jobFileNameUrl);
                                    }
                                }
                            }

                            string jsonAtt = Newtonsoft.Json.JsonConvert.SerializeObject(lstSrAttachment);
                            newSR.DRAFT_ATTACHMENT_JSON = jsonAtt.JSEncode();

                            //this.Save();
                        }

                        #endregion

                        _context.TB_T_SR.Add(newSR);
                        this.Save();

                        srId = newSR.SR_ID;
                        this.UpdateJobBySRId(jobId, userId, newSR.SR_ID);

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        private void UpdateJobBySRId(int jobId, int userId, int SRId)
        {
            var job = _context.TB_T_JOB.FirstOrDefault(x => x.JOB_ID == jobId);
            if (job != null)
            {
                job.SR_ID = SRId;
                job.JOB_STATUS = Constants.JobStatus.Refer;
                job.UPDATE_DATE = DateTime.Now;
                job.UPDATE_USER = userId;

                SetEntryStateModified(job);
                this.Save();
            }
        }

        public AttachmentEntity GetAttachmentsById(int attachmentId)
        {
            var query = from at in _context.TB_T_JOB_ATTACHMENT
                        where at.JOB_ATTACHMENT_ID == attachmentId
                        select new AttachmentEntity
                        {
                            AttachmentId = at.JOB_ATTACHMENT_ID,
                            Filename = at.FILE_NAME,
                            CreateDate = at.CREATE_DATE,
                            ContentType = at.CONTENT_TYPE,
                            Url = at.URL
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        #region "Functions"

        private void SavePoolBranches(int commPoolId, IEnumerable<PoolBranchEntity> poolBranches)
        {
            foreach (PoolBranchEntity pbEntity in poolBranches)
            {
                TB_M_POOL_BRANCH pb = _context.TB_M_POOL_BRANCH.FirstOrDefault(x => x.POOL_ID == commPoolId && x.BRANCH_ID == pbEntity.BranchId);
                if (pb != null && pbEntity.IsDelete == true)
                {
                    _context.TB_M_POOL_BRANCH.Remove(pb);
                }
                else if (pb == null)
                {
                    pb = new TB_M_POOL_BRANCH
                    {
                        POOL_ID = commPoolId,
                        BRANCH_ID = pbEntity.BranchId,
                        STATUS = Constants.ApplicationStatus.Active
                    };
                    _context.TB_M_POOL_BRANCH.Add(pb);
                }
            }

            this.Save();
        }

        private static IQueryable<PoolEntity> SetPoolListSort(IQueryable<PoolEntity> poolList, PoolSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "POOLNAME":
                        return poolList.OrderBy(ord => ord.PoolName);
                    case "POOLDESC":
                        return poolList.OrderBy(ord => ord.PoolDesc);
                    case "EMAIL":
                        return poolList.OrderBy(ord => ord.Email);
                    default:
                        return poolList.OrderBy(ord => ord.PoolId);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "POOLNAME":
                        return poolList.OrderByDescending(ord => ord.PoolName);
                    case "POOLDESC":
                        return poolList.OrderByDescending(ord => ord.PoolDesc);
                    case "EMAIL":
                        return poolList.OrderByDescending(ord => ord.Email);
                    default:
                        return poolList.OrderByDescending(ord => ord.PoolId);
                }
            }
        }

        private static IQueryable<CommunicationPoolEntity> SetJobListSort(IQueryable<CommunicationPoolEntity> commPoolList, CommPoolSearchFilter searchFilter)
        {
            int statusOpen = Constants.JobStatus.Open;
            int statusRefer = Constants.JobStatus.Refer;
            int statusDone = Constants.JobStatus.Done;

            string strOpen = Resource.Lbl_JobStatusOpen;
            string strRefer = Resource.Lbl_JobStatusRefer;
            string strDone = Resource.Lbl_JobStatusDone;

            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "ATTACHFILE":
                        //return commPoolList.OrderBy(ord => ord.Status);
                        return commPoolList.OrderBy(ord => ord.AttachmentsDisplay);
                    case "CHANNEL":
                        return commPoolList.OrderBy(ord => ord.ChannelEntity.Name);
                    case "FROM":
                        return commPoolList.OrderBy(ord => ord.SenderAddress);
                    case "JOBSTATUS":
                        return commPoolList.OrderBy(ord => (ord.Status == statusOpen) ? strOpen : ((ord.Status == statusRefer) ? strRefer : ((ord.Status == statusDone) ? strDone : "")));
                    case "JOBDATE":
                        return commPoolList.OrderBy(ord => ord.CreatedDate);
                    case "SUBJECT":
                        return commPoolList.OrderBy(ord => ord.Subject);
                    case "FIRSTNAME":
                        return commPoolList.OrderBy(ord => ord.ServiceRequest.Customer.FirstNameThaiEng);
                    case "LASTNAME":
                        return commPoolList.OrderBy(ord => ord.ServiceRequest.Customer.LastNameThaiEng);
                    case "CREATORSR":
                        //return commPoolList.OrderBy(ord => ord.ServiceRequest.CreateUser.Firstname);
                        return commPoolList.OrderBy(ord => ord.ServiceRequest.CreateUser.PositionCode).ThenBy(ord => ord.ServiceRequest.CreateUser.Firstname).ThenBy(ord => ord.ServiceRequest.CreateUser.Lastname);
                    case "OWNERSR":
                        //return commPoolList.OrderBy(ord => ord.ServiceRequest.Owner.Firstname);
                        return commPoolList.OrderBy(ord => ord.ServiceRequest.Owner.PositionCode).ThenBy(ord => ord.ServiceRequest.Owner.Firstname).ThenBy(ord => ord.ServiceRequest.Owner.Lastname);
                    case "SRID":
                        return commPoolList.OrderBy(ord => ord.ServiceRequest.SrNo);
                    case "STATUS":
                        return commPoolList.OrderBy(ord => ord.ServiceRequest.StatusDisplay);
                    case "ACTIONBY":
                        //return commPoolList.OrderBy(ord => ord.UpdateUser.Firstname);
                        return commPoolList.OrderBy(ord => ord.UpdateUser.PositionCode).ThenBy(ord => ord.UpdateUser.Firstname).ThenBy(ord => ord.UpdateUser.Lastname);
                    case "ACTIONDATE":
                        return commPoolList.OrderBy(ord => ord.UpdateDate);
                    case "POOLNAME":
                        return commPoolList.OrderBy(ord => ord.PoolEntity.PoolName);
                    default:
                        return commPoolList.OrderBy(x => x.JobId);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "ATTACHFILE":
                        //return commPoolList.OrderByDescending(ord => ord.Status);
                        return commPoolList.OrderByDescending(ord => ord.AttachmentsDisplay);
                    case "CHANNEL":
                        return commPoolList.OrderByDescending(ord => ord.ChannelEntity.Name);
                    case "FROM":
                        return commPoolList.OrderByDescending(ord => ord.SenderAddress);
                    case "JOBSTATUS":
                        return commPoolList.OrderByDescending(ord => (ord.Status == statusOpen) ? strOpen : ((ord.Status == statusRefer) ? strRefer : ((ord.Status == statusDone) ? strDone : "")));
                    case "JOBDATE":
                        return commPoolList.OrderByDescending(ord => ord.CreatedDate);
                    case "SUBJECT":
                        return commPoolList.OrderByDescending(ord => ord.Subject);
                    case "FIRSTNAME":
                        return commPoolList.OrderByDescending(ord => ord.ServiceRequest.Customer.FirstNameThaiEng);
                    case "LASTNAME":
                        return commPoolList.OrderByDescending(ord => ord.ServiceRequest.Customer.LastNameThaiEng);
                    case "CREATORSR":
                        //return commPoolList.OrderByDescending(ord => ord.ServiceRequest.CreateUser.Firstname);
                        return commPoolList.OrderByDescending(ord => ord.ServiceRequest.CreateUser.PositionCode).ThenByDescending(ord => ord.ServiceRequest.CreateUser.Firstname).ThenByDescending(ord => ord.ServiceRequest.CreateUser.Lastname);
                    case "OWNERSR":
                        //return commPoolList.OrderByDescending(ord => ord.ServiceRequest.Owner.Firstname);
                        return commPoolList.OrderByDescending(ord => ord.ServiceRequest.Owner.PositionCode).ThenByDescending(ord => ord.ServiceRequest.Owner.Firstname).ThenByDescending(ord => ord.ServiceRequest.Owner.Lastname);
                    case "SRID":
                        return commPoolList.OrderByDescending(ord => ord.ServiceRequest.SrNo);
                    case "STATUS":
                        return commPoolList.OrderByDescending(ord => ord.ServiceRequest.StatusDisplay);
                    case "ACTIONBY":
                        //return commPoolList.OrderByDescending(ord => ord.UpdateUser.Firstname);
                        return commPoolList.OrderByDescending(ord => ord.UpdateUser.PositionCode).ThenByDescending(ord => ord.UpdateUser.Firstname).ThenByDescending(ord => ord.UpdateUser.Lastname);
                    case "ACTIONDATE":
                        return commPoolList.OrderByDescending(ord => ord.UpdateDate);
                    case "POOLNAME":
                        return commPoolList.OrderByDescending(ord => ord.PoolEntity.PoolName);
                    default:
                        return commPoolList.OrderByDescending(x => x.JobId);
                }
            }
        }

        private static IQueryable<CommunicationPoolEntity> WildcardFilterBy(IQueryable<CommunicationPoolEntity> query, CommPoolSearchFilter searchFilter)
        {
            int refSearchType = 0;

            #region "Filter by FirstName"

            if (!string.IsNullOrWhiteSpace(searchFilter.FirstName))
            {
                string firstName = searchFilter.FirstName.ExtractString(ref refSearchType);
                switch (refSearchType)
                {
                    case 1:
                        query = query.Where(x => x.ServiceRequest != null && (x.ServiceRequest.Customer.FirstNameThai.StartsWith(firstName)
                            || x.ServiceRequest.Customer.FirstNameEnglish.ToUpper().StartsWith(firstName.ToUpper())));
                        break;
                    case 2:
                        query = query.Where(x => x.ServiceRequest != null && (x.ServiceRequest.Customer.FirstNameThai.EndsWith(firstName)
                            || x.ServiceRequest.Customer.FirstNameEnglish.ToUpper().EndsWith(firstName.ToUpper())));
                        break;
                    case 3:
                        query = query.Where(x => x.ServiceRequest != null && (x.ServiceRequest.Customer.FirstNameThai.Contains(firstName)
                            || x.ServiceRequest.Customer.FirstNameEnglish.ToUpper().Contains(firstName.ToUpper())));
                        break;
                    default:
                        query = query.Where(x => x.ServiceRequest != null && (x.ServiceRequest.Customer.FirstNameThai.Equals(firstName)
                            || x.ServiceRequest.Customer.FirstNameEnglish.ToUpper().Equals(firstName.ToUpper())));
                        break;
                }
            }

            #endregion

            #region "Filter by LastName"

            refSearchType = 0;

            if (!string.IsNullOrWhiteSpace(searchFilter.LastName))
            {
                string lastName = searchFilter.LastName.ExtractString(ref refSearchType);
                switch (refSearchType)
                {
                    case 1:
                        query = query.Where(x => x.ServiceRequest != null && (x.ServiceRequest.Customer.LastNameThai.StartsWith(lastName)
                             || x.ServiceRequest.Customer.LastNameEnglish.ToUpper().StartsWith(lastName.ToUpper())));
                        break;
                    case 2:
                        query = query.Where(x => x.ServiceRequest != null && (x.ServiceRequest.Customer.LastNameThai.EndsWith(lastName)
                            || x.ServiceRequest.Customer.LastNameEnglish.ToUpper().EndsWith(lastName.ToUpper())));
                        break;
                    case 3:
                        query = query.Where(x => x.ServiceRequest != null && (x.ServiceRequest.Customer.LastNameThai.Contains(lastName)
                            || x.ServiceRequest.Customer.LastNameEnglish.ToUpper().Contains(lastName.ToUpper())));
                        break;
                    default:
                        query = query.Where(x => x.ServiceRequest != null && (x.ServiceRequest.Customer.LastNameThai.Equals(lastName)
                            || x.ServiceRequest.Customer.LastNameEnglish.ToUpper().Equals(lastName.ToUpper())));
                        break;
                }
            }

            #endregion

            return query;
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