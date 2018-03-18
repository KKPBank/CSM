using System;
using System.Collections.Generic;
using CSM.Entity;
using CSM.Service.Messages.Activity;

namespace CSM.Business
{
    public interface IActivityFacade : IDisposable
    {
        IEnumerable<ActivityDataItem> GetActivityLogList(AuditLogEntity auditLog, ActivitySearchFilter searchFilter);
        //IEnumerable<ActivityDataItem> GetActivityLogList(ActivitySearchFilter searchFilter);
        IEnumerable<ServiceRequestActivityResult> GetSRActivityList(ActivitySearchFilter searchFilter);
        ServiceRequestSaveCarResult InsertActivityLogToCAR(AuditLogEntity auditLog, ServiceRequestForInsertLogEntity entity);
        ServiceRequestSaveCbsHpResult InsertActivityLogToLog100(AuditLogEntity auditLog, ServiceRequestForInsertLogEntity entity);
        IEnumerable<ServiceRequestActivityResult> GetServiceRequestActivityResults(AuditLogEntity auditLog, ActivitySearchFilter searchFilter);
    }
}
