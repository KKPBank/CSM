using CSM.Entity;
using System.Collections.Generic;

namespace CSM.Data.DataAccess
{
    public interface INewsDataAccess
    {
        IEnumerable<NewsEntity> GetNewsList(NewsSearchFilter searchFilter);
        NewsEntity GetNewsByID(int newsId);
        List<NewsBranchEntity> GetNewsBranchList(int newsId);
        List<NewsBranchEntity> GetNewsBranchList(List<NewsBranchEntity> newsBranches);
        bool SaveNews(NewsEntity newsEntity, List<NewsBranchEntity> newsBranches, List<AttachmentEntity> attachments);
        IEnumerable<NewsEntity> GetNewsUnreadList(NewsSearchFilter searchFilter);
        IEnumerable<NewsEntity> GetNewsReadList(NewsSearchFilter searchFilter);
        void SaveReadNews(ReadNewsEntity readNewsEntity);
        List<AttachmentEntity> GetNewsAttachmentList(int newsId);
    }
}
