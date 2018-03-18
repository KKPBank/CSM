using System;
using CSM.Entity;
using log4net;

namespace CSM.Business
{
    public class AppLog
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AppLog));

        public static void AuditLog(AuditLogEntity auditLog)
        {
            IAuditLogFacade auditLogFacade = null;

            try
            {
                auditLogFacade = new AuditLogFacade();
                auditLogFacade.AddLog(auditLog);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                if (auditLogFacade != null) { auditLogFacade.Dispose(); }
            }
        }

        public static void AuditLog(AuditLogEntity auditLog, LogStatus status, string detail)
        {
            IAuditLogFacade auditLogFacade = null;

            try
            {
                auditLogFacade = new AuditLogFacade();
                auditLog.Status = status;
                auditLog.Detail = detail;
                auditLogFacade.AddLog(auditLog);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                if (auditLogFacade != null) { auditLogFacade.Dispose(); }
            }
        }
    }
}
