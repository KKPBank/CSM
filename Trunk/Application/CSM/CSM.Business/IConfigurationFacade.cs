using System;
using CSM.Entity;
using System.Collections.Generic; 

namespace CSM.Business
{
    public interface IConfigurationFacade : IDisposable
    {
        List<FontEntity> GetFont();
        IEnumerable<ConfigureUrlEntity> GetConfigureURL(ConfigureUrlSearchFilter searchFilter);
        IDictionary<string, string> GetAllScreen();
        List<RoleEntity> GetAllRole();
        ConfigureUrlEntity GetConfigureURLById(int ConfigureUrlId);
        bool SaveConfigurationUrl(ConfigureUrlEntity configureUrlEntity);
        bool IsDuplicateConfigureUrl(ConfigureUrlEntity configUrlEntity);
        IDictionary<string, string> GetAllMenu();
    }
}
