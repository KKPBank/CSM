using System;
using CSM.Common.Utilities;
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

        public static bool BatchProcessStart(string processCode, DateTime startTime)
        {
            IAuditLogFacade auditLogFacade = null;

            try
            {
                auditLogFacade = new AuditLogFacade();
                var proc = auditLogFacade.GetBatchProcessByCode(processCode);
                if (proc != null && proc.Status != Constants.BatchProcessStatus.Processing)
                {
                    proc.Status = Constants.BatchProcessStatus.Processing;
                    proc.StartTime = startTime;
                    proc.EndTime = null;
                    proc.ProcessTime = null;
                    proc.Detail = null;
                    
                    return auditLogFacade.UpdateBatchProcess(proc);
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                if (auditLogFacade != null) { auditLogFacade.Dispose(); }
            }

            return false;
        }

        public static bool BatchProcessEnd(string processCode, int batchStatus, DateTime endTime, TimeSpan processTime, string detail)
        {
            IAuditLogFacade auditLogFacade = null;

            try
            {
                auditLogFacade = new AuditLogFacade();
                var proc = auditLogFacade.GetBatchProcessByCode(processCode);
                if (proc != null)
                {
                    proc.Status = batchStatus;
                    proc.EndTime = endTime;
                    proc.ProcessTime = processTime;
                    proc.Detail = detail;

                    return auditLogFacade.UpdateBatchProcess(proc);
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                if (auditLogFacade != null) { auditLogFacade.Dispose(); }
            }

            return false;
        }
    }
}
