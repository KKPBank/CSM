using System;
using CSM.Common.Utilities;
using CSM.Data.DataAccess;
using CSM.Entity;
using log4net;
using System.Collections.Generic;
using CSM.Common.Resources;
using System.Globalization;

namespace CSM.Business
{
    public class AuditLogFacade : IAuditLogFacade
    {
        private readonly CSMContext _context;
        private IAuditLogDataAccess _auditLogDataAccess;
        //private LogMessageBuilder _logMsg = new LogMessageBuilder();
        //private static readonly ILog Logger = LogManager.GetLogger(typeof(AuditLogFacade));

        public AuditLogFacade()
        {
            _context = new CSMContext();
        }

        public void AddLog(AuditLogEntity auditLog)
        {
            _auditLogDataAccess = new AuditLogDataAccess(_context);
            _auditLogDataAccess.AddLog(auditLog);
        }

        public IEnumerable<AuditLogEntity> SearchAuditLogs(AuditLogSearchFilter searchFilter)
        {
            _auditLogDataAccess = new AuditLogDataAccess(_context);
            return _auditLogDataAccess.SearchAuditLogs(searchFilter);
        }

        public IDictionary<string, string> GetModule()
        {
            return this.GetModule(null);
        }

        public IDictionary<string, string> GetModule(string textName, int? textValue = null)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();
            _auditLogDataAccess = new AuditLogDataAccess(_context);
            var result = _auditLogDataAccess.GetModule();

            if (!string.IsNullOrWhiteSpace(textName))
            {
                dic.Add(textValue.ConvertToString(), textName);
            }

            foreach (var item in result)
            {
                dic.Add(item.ModuleId.ToString(), item.ModuleName);
            }

            return dic;
        }

        public IDictionary<string, string> GetAction()
        {
            return this.GetAction(null);
        }

        public IDictionary<string, string> GetAction(string textName, int? textValue = null)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();
            _auditLogDataAccess = new AuditLogDataAccess(_context);
            var result = _auditLogDataAccess.GetAction();

            if (!string.IsNullOrWhiteSpace(textName))
            {
                dic.Add(textValue.ConvertToString(), textName);
            }

            foreach (var item in result)
            {
                dic.Add(item.ActionId.ToString(), item.ActionName);
            }

            return dic;
        }

        public IDictionary<string, string> GetActionByModule(string module)
        {
            //return this.GetActionByModule(module);
            return null;
        }

        public IDictionary<string, string> GetActionByModule(string module,string textName, int? textValue = null)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();
            _auditLogDataAccess = new AuditLogDataAccess(_context);
            var result = _auditLogDataAccess.GetActionByModule(module);

            if (!string.IsNullOrWhiteSpace(textName))
            {
                dic.Add(textValue.ConvertToString(), textName);
            }

            foreach (var item in result)
            {
                dic.Add(item.ActionId.ToString(), item.ActionName);
            }

            return dic;
        }

        public IDictionary<string, string> GetStatusSelectList()
        {
            return this.GetStatusSelectList(null);
        }

        public IDictionary<string, string> GetStatusSelectList(string textName, int? textValue = null)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(textName))
            {
                dic.Add(textValue.ConvertToString(), textName);
            }

            dic.Add(Constants.AuditLogStatus.Success.ToString(CultureInfo.InvariantCulture), Resource.Ddl_Status_Success);
            dic.Add(Constants.AuditLogStatus.Fail.ToString(CultureInfo.InvariantCulture), Resource.Ddl_Status_Fail);
            return dic;
        }

        public IEnumerable<BatchProcessEntity> GetBatchProcess()
        {
            _auditLogDataAccess = new AuditLogDataAccess(_context);
            return _auditLogDataAccess.GetBatchProcess();
        }

        public BatchProcessEntity GetBatchProcessByCode(string processCode)
        {
            _auditLogDataAccess = new AuditLogDataAccess(_context);
            return _auditLogDataAccess.GetBatchProcessByCode(processCode);
        }

        public bool UpdateBatchProcess(BatchProcessEntity batchProcessEntity)
        {
            _auditLogDataAccess = new AuditLogDataAccess(_context);
            return _auditLogDataAccess.UpdateBatchProcess(batchProcessEntity);
        }

        public int GetBatchInterval()
        {
            _auditLogDataAccess = new AuditLogDataAccess(_context);
            return _auditLogDataAccess.GetBatchInterval();
        }

        public bool SaveBatchInterval(int intervalTime)
        {
            _auditLogDataAccess = new AuditLogDataAccess(_context);
            return _auditLogDataAccess.SaveBatchInterval(intervalTime);
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
