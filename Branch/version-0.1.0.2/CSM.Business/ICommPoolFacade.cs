using System;
using CSM.Entity;
using System.Collections.Generic;
using CSM.Service.Messages.SchedTask;

namespace CSM.Business
{
    public interface ICommPoolFacade : IDisposable
    {
        JobTaskResult AddMailContent(string hostname, int port, bool useSsl, PoolEntity pool);
        IEnumerable<PoolEntity> GetPoolList(PoolSearchFilter searchFilter);
        List<PoolBranchEntity> GetPoolBranchList(int poolId);
        List<PoolBranchEntity> GetPoolBranchList(List<PoolBranchEntity> poolBranches);
        bool IsDuplicateCommPool(PoolEntity poolEntity);
        bool SaveCommPool(PoolEntity poolEntity, List<PoolBranchEntity> poolBranches);
        PoolEntity GetPoolByID(int commPoolId);
        IEnumerable<CommunicationPoolEntity> SearchJobs(CommPoolSearchFilter searchFilter);
        CommunicationPoolEntity GetJob(int jobId);
        List<PoolEntity> GetActivePoolList();
        IDictionary<string, string> GetJobStatusSelectList();
        IDictionary<string, string> GetJobStatusSelectList(string textName, int? textValue = null);
        IDictionary<string, string> GetSRStatusSelectList();
        IDictionary<string, string> GetSRStatusSelectList(string textName, int? textValue = null);
        IDictionary<string, string> GetChannelSelectList();
        IDictionary<string, string> GetChannelSelectList(string textName, int textValue = 0);
        IDictionary<string, string> GetChannelWithEmailSelectList(string textName, int textValue = 0);
        bool UpdateJob(int? jobId, int userId, int? status, string remark);
        bool SaveNewSR(int jobId, int userId, ref int srId);
        AttachmentEntity GetAttachmentsById(int AttachmentId);
    }
}