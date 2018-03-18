using CSM.Common.Utilities;
using CSM.Data.DataAccess;
using log4net;
using System;

namespace CSM.Business.Common
{
    public class BaseFacade2<TFacade> : BaseFacade, IDisposable where TFacade : class, new()
    {
        protected static ILog Logger = LogManager.GetLogger(typeof(TFacade));
        protected LogMessageBuilder _logMsg = new LogMessageBuilder();
        protected CSMContext _context;

        public BaseFacade2()
        {
            _context = new CSMContext();
        }

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't   
        // own unmanaged resources itself, but leave the other methods  
        // exactly as they are.   
        ~BaseFacade2()
        {
            // Finalizer calls Dispose(false)  
            Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)  
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources  
                if (_context != null)
                {
                    _context = null;
                }
            }
            // free native resources if there are any.  
            // ...
        }
    }
}
