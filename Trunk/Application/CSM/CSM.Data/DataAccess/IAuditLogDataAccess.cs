using CSM.Entity;
using System.Collections.Generic;
using System.Linq;

namespace CSM.Data.DataAccess
{
    public interface IAuditLogDataAccess
    {
        void AddLog(AuditLogEntity auditlog);
        IEnumerable<AuditLogEntity> SearchAuditLogs(AuditLogSearchFilter searchFilter);
        IQueryable<ModuleEntity> GetModule();
        IQueryable<ActionEntity> GetAction();
        IQueryable<ActionEntity> GetActionByModule(string module);
        IQueryable<MessageEntity> GetMessages(string culture);
        IEnumerable<BatchProcessEntity> GetBatchProcess();
        BatchProcessEntity GetBatchProcessByCode(string processCode);
        bool UpdateBatchProcess(BatchProcessEntity batchProcessEntity);
        int GetBatchInterval();
        bool SaveBatchInterval(int intervalTime);
    }
}
