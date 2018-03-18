using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Data.DataAccess
{
    public interface ICommPoolDataAccess
    {
        bool AddMailContent(List<CommunicationPoolEntity> mailList, ref int refNumOfSR, ref int refNumOfFax,
            ref int refNumOfWeb, ref int refNumOfEmail);
        IEnumerable<CommunicationPoolEntity> SearchJobs(CommPoolSearchFilter searchFilter);
        CommunicationPoolEntity GetJob(int jobId);
        IEnumerable<PoolEntity> GetPoolList(PoolSearchFilter searchFilter);
        List<PoolBranchEntity> GetPoolBranchList(int poolId);
        List<PoolBranchEntity> GetPoolBranchList(List<PoolBranchEntity> poolBranches);
        bool IsDuplicateCommPool(PoolEntity poolEntity);
        bool SaveCommPool(PoolEntity poolEntity, List<PoolBranchEntity> poolBranches);
        PoolEntity GetPoolByID(int commPoolId);
        List<PoolEntity> GetActivePoolList();
        List<ChannelEntity> GetActiveChannels();
        List<StatusEntity> GetAllJobStatuses();
        List<StatusEntity> GetAllSRStatuses();
        bool UpdateJob(int? jobId, int userId, int? status, string remark);
        bool SaveNewSR(int jobId, int userId, ref int srId);
        AttachmentEntity GetAttachmentsById(int attachmentId);
        bool DeleteMailContent(CommunicationPoolEntity mail, dynamic taskResult);
    }
}