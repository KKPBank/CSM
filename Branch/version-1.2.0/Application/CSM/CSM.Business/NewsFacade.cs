using System;
using CSM.Data.DataAccess;
using CSM.Entity;
using System.Collections.Generic;

namespace CSM.Business
{
    public class NewsFacade : INewsFacade
    {
        private readonly CSMContext _context;
        private INewsDataAccess _newsDataAccess;

        public NewsFacade()
        {
            _context = new CSMContext();
        }

        public IEnumerable<NewsEntity> GetNewsList(NewsSearchFilter searchFilter)
        {
            _newsDataAccess = new NewsDataAccess(_context);
            return _newsDataAccess.GetNewsList(searchFilter);
        }

        public NewsEntity GetNewsByID(int newsId)
        {
            _newsDataAccess = new NewsDataAccess(_context);
            return _newsDataAccess.GetNewsByID(newsId);
        }

        public List<NewsBranchEntity> GetNewsBranchList(int newsId)
        {
            _newsDataAccess = new NewsDataAccess(_context);
            return _newsDataAccess.GetNewsBranchList(newsId);
        }

        public List<NewsBranchEntity> GetNewsBranchList(List<NewsBranchEntity> newsBranches)
        {
            _newsDataAccess = new NewsDataAccess(_context);
            return _newsDataAccess.GetNewsBranchList(newsBranches);
        }

        public bool SaveNews(NewsEntity newsEntity, List<NewsBranchEntity> newsBranches, List<AttachmentEntity> attachments)
        {
            _newsDataAccess = new NewsDataAccess(_context);
            return _newsDataAccess.SaveNews(newsEntity, newsBranches, attachments);
        }

        public IEnumerable<NewsEntity> GetNewsUnreadList(NewsSearchFilter searchFilter)
        {
            _newsDataAccess = new NewsDataAccess(_context);
            return _newsDataAccess.GetNewsUnreadList(searchFilter);
        }

        public IEnumerable<NewsEntity> GetNewsReadList(NewsSearchFilter searchFilter)
        {
            _newsDataAccess = new NewsDataAccess(_context);
            return _newsDataAccess.GetNewsReadList(searchFilter);
        }

        public void SaveReadNews(ReadNewsEntity readNewsEntity)
        {
            _newsDataAccess = new NewsDataAccess(_context);
            _newsDataAccess.SaveReadNews(readNewsEntity);
        }

        public List<AttachmentEntity> GetNewsAttachmentList(int newsId)
        {
            _newsDataAccess = new NewsDataAccess(_context);
            return _newsDataAccess.GetNewsAttachmentList(newsId);
        }

        #region "IDisposable"

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (_context != null) { _context.Dispose(); }
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
