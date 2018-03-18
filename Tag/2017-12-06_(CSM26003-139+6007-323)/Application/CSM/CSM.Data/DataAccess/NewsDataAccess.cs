using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using CSM.Entity;
using CSM.Common.Utilities;
using System.Globalization;
using System.Data;

namespace CSM.Data.DataAccess
{
    public class NewsDataAccess : INewsDataAccess
    {
        private readonly CSMContext _context;
        private ICommonDataAccess _commonDataAccess;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(NewsDataAccess));

        public NewsDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        public IEnumerable<NewsEntity> GetNewsList(NewsSearchFilter searchFilter)
        {
            int? status = searchFilter.Status.ToNullable<int>();
            var newsList = (from nw in _context.TB_T_NEWS
                            from usr in _context.TB_R_USER.Where(x => x.USER_ID == nw.CREATE_USER).DefaultIfEmpty()
                            where (string.IsNullOrEmpty(searchFilter.Topic) || nw.TOPIC.ToUpper().Contains(searchFilter.Topic.ToUpper()))
                                && (status == null || status == Constants.ApplicationStatus.All || nw.STATUS == status)
                                && (!searchFilter.AnnounceDate.HasValue || nw.ANNOUNCE_DATE >= searchFilter.AnnounceDate.Value)
                                && (!searchFilter.ExpiryDate.HasValue || nw.EXPIRY_DATE <= searchFilter.ExpiryDate.Value)
                            select new NewsEntity
                            {
                                NewsId = nw.NEWS_ID,
                                Topic = nw.TOPIC,
                                AnnounceDate = nw.ANNOUNCE_DATE,
                                ExpiryDate = nw.EXPIRY_DATE,
                                Status = nw.STATUS,
                                CreateUser = new UserEntity
                                {
                                    Firstname = usr.FIRST_NAME,
                                    Lastname = usr.LAST_NAME,
                                    PositionCode = usr.POSITION_CODE
                                },
                            });

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = newsList.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            newsList = SetNewsListSort(newsList, searchFilter);
            return newsList.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<NewsEntity>();
        }

        public NewsEntity GetNewsByID(int newsId)
        {
            var query = from nw in _context.TB_T_NEWS
                        from cs in _context.TB_R_USER.Where(x => x.USER_ID == nw.CREATE_USER).DefaultIfEmpty()
                        from us in _context.TB_R_USER.Where(x => x.USER_ID == nw.UPDATE_USER).DefaultIfEmpty()
                        where nw.NEWS_ID == newsId
                        select new NewsEntity
                        {
                            NewsId = nw.NEWS_ID,
                            Topic = nw.TOPIC,
                            AnnounceDate = nw.ANNOUNCE_DATE,
                            ExpiryDate = nw.EXPIRY_DATE,
                            Content = nw.CONTENT,
                            Status = nw.STATUS,
                            CreatedDate = nw.CREATE_DATE,
                            UpdateDate = nw.UPDATE_DATE,
                            CreateUser = new UserEntity
                            {
                                Firstname = cs.FIRST_NAME,
                                Lastname = cs.LAST_NAME,
                                PositionCode = cs.POSITION_CODE
                            },
                            UpdateUser = us != null
                            ? new UserEntity
                            {
                                Firstname = us.FIRST_NAME,
                                Lastname = us.LAST_NAME,
                                PositionCode = us.POSITION_CODE
                            }
                            : null
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        public List<NewsBranchEntity> GetNewsBranchList(int newsId)
        {
            var query = from nb in _context.TB_T_NEWS_BRANCH
                        join br in _context.TB_R_BRANCH on nb.BRANCH_ID equals br.BRANCH_ID
                        where nb.NEWS_ID == newsId
                        select new NewsBranchEntity
                        {
                            BranchId = nb.BRANCH_ID,
                            BranchName = br.BRANCH_NAME,
                            NewsId = nb.NEWS_ID,
                            IsDelete = false
                        };

            return query.ToList();
        }

        public List<NewsBranchEntity> GetNewsBranchList(List<NewsBranchEntity> newsBranches)
        {
            var query = from tmp in newsBranches
                        join br in _context.TB_R_BRANCH on tmp.BranchId equals br.BRANCH_ID
                        from nb in _context.TB_T_NEWS_BRANCH.Where(x => tmp.NewsId != null && x.NEWS_ID == tmp.NewsId
                            && x.BRANCH_ID == tmp.BranchId).DefaultIfEmpty()
                        select new NewsBranchEntity
                        {
                            BranchId = br.BRANCH_ID,
                            BranchName = br.BRANCH_NAME,
                            NewsId = nb != null ? nb.NEWS_ID : null,
                            IsDelete = tmp.IsDelete
                        };

            return query.ToList();
        }

        public bool SaveNews(NewsEntity newsEntity, List<NewsBranchEntity> newsBranches, List<AttachmentEntity> attachments)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        TB_T_NEWS news = null;
                        if (newsEntity.NewsId == null || newsEntity.NewsId == 0)
                        {
                            news = new TB_T_NEWS();
                            news.TOPIC = newsEntity.Topic;
                            news.ANNOUNCE_DATE = newsEntity.AnnounceDate;
                            news.EXPIRY_DATE = newsEntity.ExpiryDate;
                            news.CONTENT = newsEntity.Content;
                            news.STATUS = newsEntity.Status;
                            news.CREATE_USER = newsEntity.CreateUserId;
                            news.CREATE_DATE = DateTime.Now;
                            news.UPDATE_USER = newsEntity.CreateUserId;
                            news.UPDATE_DATE = DateTime.Now;
                            _context.TB_T_NEWS.Add(news);
                            this.Save();
                        }
                        else
                        {
                            news = _context.TB_T_NEWS.FirstOrDefault(x => x.NEWS_ID == newsEntity.NewsId);
                            if (news != null)
                            {
                                news.TOPIC = newsEntity.Topic;
                                news.ANNOUNCE_DATE = newsEntity.AnnounceDate;
                                news.EXPIRY_DATE = newsEntity.ExpiryDate;
                                news.CONTENT = newsEntity.Content;
                                news.STATUS = newsEntity.Status;
                                news.UPDATE_USER = newsEntity.UpdateUserId;
                                news.UPDATE_DATE = DateTime.Now;
                                SetEntryStateModified(news);
                            }
                            else
                            {
                                Logger.ErrorFormat("NEWS ID: {0} does not exist", newsEntity.NewsId);
                            }
                        }

                        if (newsBranches != null && newsBranches.Count > 0 && news != null)
                        {
                            this.SaveNewsBranches(news.NEWS_ID, newsBranches);
                        }

                        if (attachments != null && attachments.Count > 0 && news != null)
                        {
                            this.SaveAttachments(news.NEWS_ID, newsEntity, attachments);
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

        public IEnumerable<NewsEntity> GetNewsUnreadList(NewsSearchFilter searchFilter)
        {
            int? status = searchFilter.Status.ToNullable<int>();
            DateTime today = DateTime.Now.Date;

            var newsList = from nw in _context.TB_T_NEWS
                           from usr in _context.TB_R_USER.Where(x => x.USER_ID == nw.CREATE_USER) // ผู้ประกาศ
                           from ur in _context.TB_R_USER.Where(x => x.USER_ID == searchFilter.UserId) // UserLogin                         
                           from nb in _context.TB_T_NEWS_BRANCH.Where(x => x.NEWS_ID == nw.NEWS_ID && x.BRANCH_ID == ur.BRANCH_ID)
                           from rn in _context.TB_T_READ_NEWS.Where(x => x.NEWS_ID == nw.NEWS_ID && x.CREATE_USER == searchFilter.UserId).DefaultIfEmpty()
                           where nw.STATUS == status && rn.CREATE_USER == null
                           && (nw.ANNOUNCE_DATE <= today)
                           && (nw.EXPIRY_DATE == null || nw.EXPIRY_DATE >= today)
                           group new { nw.NEWS_ID, ur.BRANCH_ID } by new { nw.NEWS_ID, nw.TOPIC, nw.ANNOUNCE_DATE, nw.EXPIRY_DATE, nw.STATUS, usr } into g
                           select new NewsEntity
                           {
                               NewsId = g.Key.NEWS_ID,
                               Topic = g.Key.TOPIC,
                               AnnounceDate = g.Key.ANNOUNCE_DATE,
                               ExpiryDate = g.Key.EXPIRY_DATE,
                               CreateUser = new UserEntity
                               {
                                   Firstname = g.Key.usr.FIRST_NAME,
                                   Lastname = g.Key.usr.LAST_NAME,
                                   PositionCode = g.Key.usr.POSITION_CODE
                               },
                               Status = g.Key.STATUS
                           };

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = newsList.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            newsList = SetNewsListSort(newsList, searchFilter);
            return newsList.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<NewsEntity>();
        }

        public IEnumerable<NewsEntity> GetNewsReadList(NewsSearchFilter searchFilter)
        {
            int? status = searchFilter.Status.ToNullable<int>();
            DateTime today = DateTime.Now.Date;

            var newsList = from nw in _context.TB_T_NEWS
                           join rn in _context.TB_T_READ_NEWS on nw.NEWS_ID equals rn.NEWS_ID
                           join usr in _context.TB_R_USER on nw.CREATE_USER equals usr.USER_ID  // ผู้ประกาศ
                           from ur in _context.TB_R_USER.Where(x => x.USER_ID == searchFilter.UserId) // UserLogin 
                           from nb in _context.TB_T_NEWS_BRANCH.Where(x => x.NEWS_ID == nw.NEWS_ID && x.BRANCH_ID == ur.BRANCH_ID)
                           where nw.STATUS == status && rn.CREATE_USER == searchFilter.UserId
                           && (nw.EXPIRY_DATE == null || nw.EXPIRY_DATE >= today)
                           group new { nw.NEWS_ID, ur.BRANCH_ID } by new { nw.NEWS_ID, nw.TOPIC, nw.ANNOUNCE_DATE, nw.EXPIRY_DATE, nw.STATUS, usr } into g
                           select new NewsEntity
                           {
                               NewsId = g.Key.NEWS_ID,
                               Topic = g.Key.TOPIC,
                               AnnounceDate = g.Key.ANNOUNCE_DATE,
                               ExpiryDate = g.Key.EXPIRY_DATE,
                               CreateUser = new UserEntity
                               {
                                   Firstname = g.Key.usr.FIRST_NAME,
                                   Lastname = g.Key.usr.LAST_NAME,
                                   PositionCode = g.Key.usr.POSITION_CODE
                               },
                               Status = g.Key.STATUS
                           };

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = newsList.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            newsList = SetNewsListSort(newsList, searchFilter);
            return newsList.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<NewsEntity>();
        }

        public void SaveReadNews(ReadNewsEntity readNewsEntity)
        {
            TB_T_READ_NEWS rd = null;
            rd = new TB_T_READ_NEWS();
            rd.NEWS_ID = readNewsEntity.NewsId;
            rd.CREATE_USER = readNewsEntity.CreateUserId;
            rd.CREATE_DATE = DateTime.Now;
            _context.TB_T_READ_NEWS.Add(rd);
            this.Save();
        }

        public List<AttachmentEntity> GetNewsAttachmentList(int newsId)
        {
            var query = from at in _context.TB_T_NEWS_ATTACHMENT
                        join nw in _context.TB_T_NEWS on at.NEWS_ID equals nw.NEWS_ID
                        where nw.NEWS_ID == newsId
                        select new AttachmentEntity
                        {
                            AttachmentId = at.NEWS_ATTACHMENT_ID,
                            NewsId = at.NEWS_ID,
                            Filename = at.FILE_NAME,
                            ContentType = at.CONTENT_TYPE,
                            Url = at.URL,
                            Name = at.ATTACHMENT_NAME,
                            Description = at.ATTACHMENT_DESC,
                            ExpiryDate = at.EXPIRY_DATE,
                            IsDelete = false,
                            FileSize = at.FILE_SIZE,
                            AttachTypeList = _context.TB_T_ATTACHMENT_TYPE.Where(x => x.NEWS_ATTACHMENT_ID == at.NEWS_ATTACHMENT_ID)
                                                    .Select(x => new AttachmentTypeEntity
                                                    {
                                                        AttachmentId = x.NEWS_ATTACHMENT_ID,
                                                        DocTypeId = x.DOCUMENT_TYPE_ID
                                                    }).ToList()
                        };

            return query.ToList();
        }

        #region "Functions"

        private void SaveNewsBranches(int newsId, IEnumerable<NewsBranchEntity> newsBranches)
        {
            foreach (NewsBranchEntity nbEntity in newsBranches)
            {
                TB_T_NEWS_BRANCH nb = _context.TB_T_NEWS_BRANCH.FirstOrDefault(x => x.NEWS_ID == newsId && x.BRANCH_ID == nbEntity.BranchId);
                if (nb != null && nbEntity.IsDelete == true)
                {
                    _context.TB_T_NEWS_BRANCH.Remove(nb);
                }
                else if (nb == null)
                {
                    nb = new TB_T_NEWS_BRANCH();
                    nb.NEWS_ID = newsId;
                    nb.BRANCH_ID = nbEntity.BranchId;
                    _context.TB_T_NEWS_BRANCH.Add(nb);
                }
            }

            this.Save();
        }

        private void SaveAttachments(int newsId, NewsEntity newsEnity, IEnumerable<AttachmentEntity> attachments)
        {
            foreach (AttachmentEntity attachment in attachments)
            {
                TB_T_NEWS_ATTACHMENT dbAttach = null;
                var isIDNull = attachment.AttachmentId == null || attachment.AttachmentId == 0 ? true : false;

                if (isIDNull)
                {
                    dbAttach = new TB_T_NEWS_ATTACHMENT();
                    dbAttach.NEWS_ID = newsId;
                    dbAttach.ATTACHMENT_NAME = attachment.Name;
                    dbAttach.ATTACHMENT_DESC = attachment.Description;
                    dbAttach.CREATE_DATE = DateTime.Now;
                    dbAttach.EXPIRY_DATE = attachment.ExpiryDate;

                    // New file
                    var tempFile = attachment.TempPath;
                    _commonDataAccess = new CommonDataAccess(_context);
                    int nextSeq = _commonDataAccess.GetNextAttachmentSeq();
                    string fileNameUrl = ApplicationHelpers.GenerateFileName(newsEnity.DocumentFolder, attachment.FileExtension, nextSeq, Constants.AttachmentPrefix.News);

                    var targetFile = string.Format(CultureInfo.InvariantCulture,"{0}\\{1}", newsEnity.DocumentFolder, fileNameUrl);

                    if (StreamDataHelpers.TryToCopy(tempFile, targetFile))
                    {
                        // Save new file
                        dbAttach.FILE_NAME = attachment.Filename;
                        dbAttach.URL = fileNameUrl;
                        dbAttach.CONTENT_TYPE = attachment.ContentType;
                        dbAttach.FILE_SIZE = attachment.FileSize;

                        _context.TB_T_NEWS_ATTACHMENT.Add(dbAttach);
                    }
                }

                if (!isIDNull && attachment.IsDelete == true)
                {
                    // Delete AttachmentType
                    var listType = _context.TB_T_ATTACHMENT_TYPE.Where(x => x.NEWS_ATTACHMENT_ID == attachment.AttachmentId);
                    _context.TB_T_ATTACHMENT_TYPE.RemoveRange(listType);

                    // Delete News Attachment
                    dbAttach = _context.TB_T_NEWS_ATTACHMENT.FirstOrDefault(x => x.NEWS_ATTACHMENT_ID == attachment.AttachmentId);
                    var prevFile = dbAttach.URL; // for delete file
                    _context.TB_T_NEWS_ATTACHMENT.Remove(dbAttach);

                    StreamDataHelpers.TryToDelete(string.Format(CultureInfo.InvariantCulture,"{0}\\{1}", newsEnity.DocumentFolder, prevFile));

                }

                if (!isIDNull && attachment.IsDelete == false)
                {
                    // Get previous path file
                    dbAttach = _context.TB_T_NEWS_ATTACHMENT.FirstOrDefault(x => x.NEWS_ATTACHMENT_ID == attachment.AttachmentId);
                    dbAttach.ATTACHMENT_NAME = attachment.Name;
                    dbAttach.ATTACHMENT_DESC = attachment.Description;
                    //dbAttach.CREATE_DATE = DateTime.Now;
                    dbAttach.EXPIRY_DATE = attachment.ExpiryDate;

                    if (!string.IsNullOrWhiteSpace(attachment.TempPath))
                    {
                        var prevFile = dbAttach.URL;

                        // New file
                        var tempFile = attachment.TempPath;
                        _commonDataAccess = new CommonDataAccess(_context);
                        int nextSeq = _commonDataAccess.GetNextAttachmentSeq();
                        string fileNameUrl = ApplicationHelpers.GenerateFileName(newsEnity.DocumentFolder, attachment.FileExtension, nextSeq, Constants.AttachmentPrefix.News);

                        var targetFile = string.Format(CultureInfo.InvariantCulture,"{0}\\{1}", newsEnity.DocumentFolder, fileNameUrl);

                        if (StreamDataHelpers.TryToCopy(tempFile, targetFile))
                        {
                            if (StreamDataHelpers.TryToDelete(string.Format(CultureInfo.InvariantCulture,"{0}\\{1}", newsEnity.DocumentFolder, prevFile)))
                            {
                                // Save new file
                                dbAttach.FILE_NAME = attachment.Filename;
                                dbAttach.URL = fileNameUrl;
                                dbAttach.CONTENT_TYPE = attachment.ContentType;
                                dbAttach.FILE_SIZE = attachment.FileSize;
                            }
                        }
                    }

                    SetEntryStateModified(dbAttach);
                }

                this.Save();

                if (attachment.IsDelete == false && attachment.AttachTypeList != null && attachment.AttachTypeList.Count > 0
                    && dbAttach != null)
                {
                    this.SaveAttachTypes(dbAttach.NEWS_ATTACHMENT_ID, attachment.AttachTypeList);
                }
            }
        }

        private void SaveAttachTypes(int attachmentId, IEnumerable<AttachmentTypeEntity> attachTypes)
        {
            foreach (AttachmentTypeEntity attachType in attachTypes)
            {
                TB_T_ATTACHMENT_TYPE dbAttachType = _context.TB_T_ATTACHMENT_TYPE.FirstOrDefault(x => x.NEWS_ATTACHMENT_ID == attachmentId
                        && x.DOCUMENT_TYPE_ID == attachType.DocTypeId);

                if (dbAttachType == null && attachType.IsDelete == false)
                {
                    dbAttachType = new TB_T_ATTACHMENT_TYPE();
                    dbAttachType.NEWS_ATTACHMENT_ID = attachmentId;
                    dbAttachType.DOCUMENT_TYPE_ID = attachType.DocTypeId;
                    dbAttachType.CREATE_USER = attachType.CreateUserId;
                    dbAttachType.CREATE_DATE = DateTime.Now;
                    _context.TB_T_ATTACHMENT_TYPE.Add(dbAttachType);
                }

                if (dbAttachType != null && attachType.IsDelete == true)
                {
                    _context.TB_T_ATTACHMENT_TYPE.Remove(dbAttachType);
                }
            }

            this.Save();
        }

        private static IQueryable<NewsEntity> SetNewsListSort(IQueryable<NewsEntity> newsList, NewsSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField)
                {
                    case "Topic":
                        return newsList.OrderBy(ord => ord.Topic);
                    case "AnnounceDate":
                        return newsList.OrderBy(ord => ord.AnnounceDate);
                    case "ExpiryDate":
                        return newsList.OrderBy(ord => ord.ExpiryDate);
                    case "Status":
                        return newsList.OrderBy(ord => (ord.Status == 1) ? "A" : "I");
                    case "FullName":
                        return newsList.OrderBy(ord => ord.CreateUser.PositionCode).ThenBy(ord => ord.CreateUser.Firstname).ThenBy(ord => ord.CreateUser.Lastname);
                    default:
                        return newsList.OrderBy(ord => ord.NewsId);
                }
            }
            else
            {
                switch (searchFilter.SortField)
                {
                    case "Topic":
                        return newsList.OrderByDescending(ord => ord.Topic);
                    case "AnnounceDate":
                        return newsList.OrderByDescending(ord => ord.AnnounceDate);
                    case "ExpiryDate":
                        return newsList.OrderByDescending(ord => ord.ExpiryDate);
                    case "Status":
                        return newsList.OrderByDescending(ord => (ord.Status == 1) ? "A" : "I");
                    case "FullName":
                        return newsList.OrderByDescending(ord => ord.CreateUser.PositionCode).ThenByDescending(ord => ord.CreateUser.Firstname).ThenByDescending(ord => ord.CreateUser.Lastname);
                    default:
                        return newsList.OrderByDescending(ord => ord.NewsId);
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
