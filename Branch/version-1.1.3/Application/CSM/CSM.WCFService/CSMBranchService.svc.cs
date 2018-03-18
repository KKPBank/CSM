using System;
using CSM.Business;
using CSM.Common.Utilities;
using CSM.Service.Messages.Branch;
using CSM.Service.Messages.Calendar;
using log4net;

namespace CSM.WCFService
{
    public class CSMBranchService : ICSMBranchService
    {
        private readonly ILog _logger;
        private IBranchFacade _branchFacade;

        #region "Constructor"

        public CSMBranchService()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();

                // Set logfile name and application name variables
                GlobalContext.Properties["ApplicationCode"] = "CSMWS";
                GlobalContext.Properties["ServerName"] = Environment.MachineName;
                _logger = LogManager.GetLogger(typeof(CSMSRService));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
            }
        }

        #endregion

        public CreateBranchResponse InsertOrUpdateBranch(InsertOrUpdateBranchRequest request)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

            _branchFacade = new BranchFacade();
            var response = _branchFacade.InsertOrUpdateBranch(request);
            return response;
        }

        public UpdateCalendarResponse UpdateBranchCalendar(UpdateBranchCalendarRequest request)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

            _branchFacade = new BranchFacade();
            return _branchFacade.UpdateBranchCalendar(request);
        }

        #region "IDisposable"

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (_branchFacade != null) { _branchFacade.Dispose(); }
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
