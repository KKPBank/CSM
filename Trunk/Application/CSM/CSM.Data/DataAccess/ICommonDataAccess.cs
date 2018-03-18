using System.Collections.Generic;
using CSM.Entity;
using System.Linq;

namespace CSM.Data.DataAccess
{
    public interface ICommonDataAccess
    {
        IEnumerable<BranchEntity> GetBranchList(BranchSearchFilter searchFilter);
        List<BranchEntity> GetBranchesByName(string searchTerm, int pageSize, int pageNum);
        int GetBranchCountByName(string searchTerm, int pageSize, int pageNum);
        List<DocumentTypeEntity> GetActiveDocumentTypes(int documentCategory);
        List<DocumentTypeEntity> GetDocumentTypeList(List<AttachmentTypeEntity> attachTypes, int documentCategory);
        List<ParameterEntity> GetAllParameters();
        List<FontEntity> GetFont();
        IEnumerable<ConfigureUrlEntity> GetConfigureUrl(ConfigureUrlSearchFilter searchFilter);
        IQueryable<ScreenEntity> GetAllScreen();
        List<RoleEntity> GetAllRole();
        ConfigureUrlEntity GetConfigureUrlById(int configUrlId);
        bool SaveConfigureUrl(ConfigureUrlEntity configUrlEntity);
        List<TitleEntity> GetActiveTitle();
        List<SubscriptTypeEntity> GetActiveSubscriptType();
        List<PhoneTypeEntity> GetActivePhoneType();
        SubscriptTypeEntity GetSubscriptTypeByCode(string subscriptTypeCode);
        PhoneTypeEntity GetPhoneTypeByCode(string phoneTypeCode);
        IQueryable<MenuEntity> GetAllMenu();
        List<MenuEntity> GetMenuList();
        List<ScreenEntity> GetScreenList();
        List<UserEntity> GetActionByName(string searchTerm, int pageSize, int pageNum, int? branchId);
        int GetActionCountByName(string searchTerm, int pageSize, int pageNum, int? branchId);
        List<BranchEntity> GetBranchesByName(string searchTerm, int pageSize, int pageNum, int? userId);
        int GetBranchCountByName(string searchTerm, int pageSize, int pageNum, int? userId);
        List<RelationshipEntity> GetActiveRelationship();
        bool SaveShowhidePanel(int expand, int userId, string currentPage);
        DefaultSearchEntity GetShowhidePanelByUserId(int userId, string currentPage);
        int GetNextAttachmentSeq();
        List<string> GetExceptionErrorCodes(string errorSystem, string errorService);
        BranchEntity GetBranchById(int branchId);
        bool IsDuplicateConfigureUrl(ConfigureUrlEntity configUrlEntity);
        ParameterEntity GetParamByName(string paramName);
    }
}