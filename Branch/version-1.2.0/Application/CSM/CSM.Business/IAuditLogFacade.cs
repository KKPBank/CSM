using System;
using CSM.Entity;
using System.Collections.Generic;

namespace CSM.Business
{
    public interface IAuditLogFacade : IDisposable
    {
        void AddLog(AuditLogEntity auditLog);
        IEnumerable<AuditLogEntity> SearchAuditLogs(AuditLogSearchFilter searchFilter); 
        IDictionary<string, string> GetModule();
        IDictionary<string, string> GetAction();
        IDictionary<string, string> GetActionByModule(string module);
        IDictionary<string, string> GetStatusSelectList();
        IDictionary<string, string> GetModule(string textName, int? textValue = null);
        IDictionary<string, string> GetAction(string textName, int? textValue = null);
        IDictionary<string, string> GetActionByModule(string module, string textName, int? textValue = null);
        IDictionary<string, string> GetStatusSelectList(string textName, int? textValue = null);
        IEnumerable<BatchProcessEntity> GetBatchProcess();
        BatchProcessEntity GetBatchProcessByCode(string processCode);
        bool UpdateBatchProcess(BatchProcessEntity batchProcessEntity);
        int GetBatchInterval();
        bool SaveBatchInterval(int intervalTime);
    }
}
