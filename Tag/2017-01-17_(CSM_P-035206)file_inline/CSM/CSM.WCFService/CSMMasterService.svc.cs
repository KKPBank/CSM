using System;
using CSM.Business;
using CSM.Common.Utilities;
using CSM.Service.Messages.Master;
using log4net;

namespace CSM.WCFService
{
    public class CSMMasterService : ICSMMasterService
    {
        private readonly ILog _logger;
        private IProductFacade _productFacade;

        #region "Constructor"
        public CSMMasterService()
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

        public CreateProductMasterResponse CreateProductMaster(CreateProductMasterRequest request)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

            if (request != null && request.Header != null)
            {
                ThreadContext.Properties["UserID"] = request.Header.user_name;
            }

            _productFacade = new ProductFacade();
            return _productFacade.CreateProductMaster(request);
        }

        #region "IDisposable"

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (_productFacade != null) { _productFacade.Dispose(); }
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
