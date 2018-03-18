using CSM.Common.Utilities;
using log4net;
using System;

namespace CSM.Data.DataAccess
{
    public class BaseDA<TDataAccess> where TDataAccess : class
    {
        protected CSMContext _context;
        protected static ILog Logger = LogManager.GetLogger(typeof(TDataAccess));
        //protected static TDataAccess instance = null;
        //public static TDataAccess Instance { get { return instance; } }

        public BaseDA(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
            //instance = Activator.CreateInstance(typeof(instance), context);
        }
    }
}
