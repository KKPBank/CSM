using System;
using System.Collections.Generic;
using CSM.Entity;
using CSM.Service.Messages.Sr;

namespace CSM.Business
{
    public interface IServiceRequestFacade : IDisposable
    {
        CreateSRResponse CreateSRWebService(CreateSRRequest request);
        UpdateSRResponse UpdateSRWebService(UpdateSRRequest request);
        SearchSRResponse SearchSRWebService(SearchSRRequest request);
        GetSRResponse GetSRWebService(GetSRRequest request);
        ServiceRequestSaveResult CreateServiceRequestActivity(AuditLogEntity auditLog,  ServiceRequestForSaveEntity newValue, bool disableSendToHp = false);
        void ReSubmitActivityToCBSHPSystem(ref int countSuccess, ref int countError);
        void ReSubmitActivityToCARSystem(ref int countSuccess, ref int countError);
        List<BranchEntity> AutoCompleteSearchBranch(string keyword, int limit);
        List<UserEntity> AutoCompleteSearchUser(string keyword, int? branchId, int limit);
        List<UserEntity> AutoCompleteSearchUserWithJobOnHand(string keyword, int branchId, int limit);
        List<ProductGroupEntity> AutoCompleteSearchProductGroup(string keyword, int limit, int? productId, bool? isAllStatus);
        List<ProductEntity> AutoCompleteSearchProduct(string keyword, int? productGroupId, int limit, int? campaignServiceId, bool? isAllStatus);
        List<ProductEntity> AutoCompleteSearchProductForQuestionGroup(string keyword, int? productGroupId, int limit, int? campaignServiceId);
        List<CampaignServiceEntity> AutoCompleteSearchCampaignService(string keyword, int? productGroupId, int? productId, int limit, bool? isAllStatus);
        List<CampaignServiceEntity> AutoCompleteSearchCampaignServiceOnMapping(string keyword, int? areaId, int? subAreaId, int? typeId);
        List<AreaItemEntity> AutoCompleteSearchArea(string keyword, int? subAreaId, int limit, bool? isAllStatus);
        List<AreaItemEntity> AutoCompleteSearchAreaOnMapping(string keyword, int? campaignServiceId, int? subAreaId, int? typeId, int limit);
        List<SubAreaItemEntity> AutoCompleteSearchSubArea(string keyword, int? areaId, int limit, bool? isAllStatus);
        List<SubAreaItemEntity> AutoCompleteSearchSubAreaOnMapping(string keyword, int? campaignServiceId, int? areaId, int? typeId, int limit);
        List<TypeItemEntity> AutoCompleteSearchType(string keyword, int limit, bool? isAllStatus);
        List<TypeItemEntity> AutoCompleteSearchTypeOnMapping(string keyword, int? campaignServiceId, int? areaId, int? subAreaId, int limit);
        List<ChannelEntity> AutoCompleteSearchChannel();
        List<SRStatusEntity> AutoCompleteSearchSrStatus();
        List<AfsAssetEntity> AutoCompleteSearchAfsAsset(string keyword, int limit);
        ServiceRequestCustomerContactEntity GetCustomerContact(int id);
        ServiceRequestCustomerAccount GetCustomerAccount(int? accountId, int? customerId = null);
        CustomerEntity GetCustomerByID(int id);
        IEnumerable<ChannelEntity> GetChannels();
        IEnumerable<MediaSourceEntity> GetMediaSources();
        CampaignServiceEntity GetCampaignService(int campaignserviceId);
        UserEntity GetUserById(int id);
        void CreateSRActivityFromReplyEmail(ref int countSuccess, ref int countError);
        IEnumerable<ServiceRequestActivityResult> GetTabActivityList(AuditLogEntity auditLog, ActivityTabSearchFilter searchFilter);
    }
}
