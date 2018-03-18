using System;
using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Business
{
    public interface ICommonFacade : IDisposable
    {
        IDictionary<string, string> GetStatusSelectList();
        IDictionary<string, string> GetStatusSelectList(string textName, int? textValue = null);
        IEnumerable<BranchEntity> GetBranchList(BranchSearchFilter searchFilter);
        List<BranchEntity> GetBranchesByName(string searchTerm, int pageSize, int pageNum);
        int GetBranchCountByName(string searchTerm, int pageSize, int pageNum);
        List<BranchEntity> GetBranchesByName(string searchTerm, int pageSize, int pageNum, int? userId);
        int GetBranchCountByName(string searchTerm, int pageSize, int pageNum, int? userId);
        List<DocumentTypeEntity> GetActiveDocumentTypes(int documentCategory);
        List<DocumentTypeEntity> GetDocumentTypeList(List<AttachmentTypeEntity> attachTypes, int documentCategory);
        ParameterEntity GetCacheParamByName(string paramName);
        ParameterEntity GetParamByName(string paramName);
        string GetValueParamByName(string paramName);
        string GetDescParamByName(string paramName);
        IDictionary<string, string> GetCustomerTypeSelectList();
        IDictionary<string, string> GetTitleThaiSelectList();
        IDictionary<string, string> GetTitleEnglishSelectList();
        TitleEntity GetTitleByCode(string titleCode, string Language);
        IDictionary<string, string> GetSubscriptTypeSelectList();
        IDictionary<string, string> GetPhoneTypeSelectList();
        SubscriptTypeEntity GetSubscriptTypeByCode(string subscriptTypeCode);
        SubscriptTypeEntity GetSubscriptTypeByTypeCode(string IDTypeCode);
        PhoneTypeEntity GetPhoneTypeByCode(string phoneTypeCode);
        List<MenuEntity> GetCacheMainMenu(string selectedMenu, int roleValue);
        int GetRoleValueByScreenCode(string screenCode);
        dynamic GetHeaderByServiceName<T>(string serviceName);
        List<MenuEntity> GetCacheCustomerTab(string selectedTab, int? customerId = 0, decimal? customerNumber = 0);
        List<UserEntity> GetActionByName(string searchTerm, int pageSize, int pageNum, int? branchId);
        int GetActionCountByName(string searchTerm, int pageSize, int pageNum, int? branchId);
        IDictionary<string, string> GetRelationshipSelectList();
        bool SaveShowhidePanel(int expand, int userId, string currentPage);
        DefaultSearchEntity GetShowhidePanelByUserId(UserEntity user, string currentPage);
        string GetCSMDocumentFolder();
        string GetNewsDocumentFolder();
        string GetJobDocumentFolder();
        string GetSrDocumentFolder();
        string GetOfficePhoneNo();
        string GetOfficeHour();
        string GetProductGroupSubmitCBSHP();
        string GetTextDummyAccountNo();
        int GetNextAttachmentSeq();
        IDictionary<string, int> GetPageSizeList();
        int GetPageSizeStart();
        IDictionary<string, string> GetCustomerProductSelectList(string textName, int? textValue = null);
        IDictionary<string, string> GetCustomerTypeSelectList(string textName, int? textValue = null);
        List<MenuEntity> GetReportList(int roleValue);
        bool VerifyServiceRequest<T>(dynamic header);
        List<string> GetExceptionErrorCodes(string errorSystem, string errorService);
        List<MenuEntity> GetMasterList(int roleValue);
        BranchEntity GetBranchById(int branchId);
        int GetNumMonthsActivity();
        int GetMaxMinuteBatchCreateSRActivityFromReplyEmail();
        int GetMaxMinuteBatchReSubmitActivityToCARSystem();
        int GetMaxMinuteBatchReSubmitActivityToCBSHPSystem();
        string GetSLMUrlNewLead();
        string GetSLMUrlViewLead();
        string GetSLMEncryptPassword();
        CasaAccountStatusEntity GetConfigCasaAccountStatus(long casaAccountStatusValue);
        CbsCustomerCategoryEntity GetCbsCustomerCategory(string customerCatetoryValue);
        int GetCASAStatementHisMonth();
    }
}
