using System;
using CSM.Common.Utilities;
using CSM.Business;
using CSM.Service.Messages.User;
using log4net;

namespace CSM.WCFService
{
    public class CSMUserService : ICSMUserService
    {
        private readonly ILog _logger;
        private IUserFacade _userFacade;

        #region "Constructor"
        public CSMUserService()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();

                // Set logfile name and application name variables
                GlobalContext.Properties["ApplicationCode"] = "CSMWS";
                GlobalContext.Properties["ServerName"] = Environment.MachineName;
                _logger = LogManager.GetLogger(typeof(CSMMasterService));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
            }
        }
        #endregion

        public InsertOrUpdateUserResponse InsertOrUpdateUser(InsertOrUpdateUserRequest request)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

            _logger.DebugFormat("-- XMLRequest --\n{0}", request.SerializeObject());
            _userFacade = new UserFacade();
            var response = _userFacade.InsertOrUpdateUser(request);
            if (response != null)
            {
                _logger.DebugFormat("-- XMLResponse --\n{0}", response.SerializeObject());
            }
            return response;
        }

        #region "IDisposable"

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (_userFacade != null) { _userFacade.Dispose(); }
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
