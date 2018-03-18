using System;
using CSM.Data.DataAccess;
using CSM.Entity;
using System.Collections.Generic;
using System.Globalization;

namespace CSM.Business
{
    public class ConfigurationFacade : IConfigurationFacade
    {
        private readonly CSMContext _context;
        private ICommonDataAccess _commonDataAccess;

        public ConfigurationFacade()
        {
            _context = new CSMContext();
        }

        public List<FontEntity> GetFont()
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetFont();
        }

        public IEnumerable<ConfigureUrlEntity> GetConfigureURL(ConfigureUrlSearchFilter searchFilter)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetConfigureUrl(searchFilter);
        }

        public IDictionary<string, string> GetAllScreen()
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();

            _commonDataAccess = new CommonDataAccess(_context);

            var result = _commonDataAccess.GetAllScreen();
            foreach (var item in result)
            {
                dic.Add(item.ScreenId.ToString(CultureInfo.InvariantCulture), item.ScreenName);
            }

            return dic;
        }

        public IDictionary<string, string> GetAllMenu()
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();

            _commonDataAccess = new CommonDataAccess(_context);

            var result = _commonDataAccess.GetAllMenu();
            foreach (var item in result)
            {
                dic.Add(item.MenuId.ToString(), item.MenuName);
            }

            return dic;
        }

        public List<RoleEntity> GetAllRole()
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetAllRole();
        }

        public ConfigureUrlEntity GetConfigureURLById(int ConfigureUrlId)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.GetConfigureUrlById(ConfigureUrlId);
        }

        public bool SaveConfigurationUrl(ConfigureUrlEntity configureUrlEntity)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.SaveConfigureUrl(configureUrlEntity);
        }

        public bool IsDuplicateConfigureUrl(ConfigureUrlEntity configUrlEntity)
        {
            _commonDataAccess = new CommonDataAccess(_context);
            return _commonDataAccess.IsDuplicateConfigureUrl(configUrlEntity);
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
