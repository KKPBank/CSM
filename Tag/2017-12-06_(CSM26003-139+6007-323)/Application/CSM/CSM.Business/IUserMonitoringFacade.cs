using System;
using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Business
{
    public interface IUserMonitoringFacade : IDisposable
    {
        List<GroupAssignEntity> SearchGroupAssign(GroupAssignSearchFilter searchFilter);
        List<UserAssignEntity> SearchUserAssign(UserAssignSearchFilter searchFilter);
        List<ServiceRequestEntity> SearchServiceRequest(UserMonitoringSrSearchFilter searchFilter);

        UserEntity GetUserByLoginName(string login);
        List<UserEntity> GetDummyUserByBranchIds(List<int> branchIds);
        IEnumerable<ServiceRequestEntity> GetServiceRequestList(ServiceRequestSearchFilter searchFilter);
        TransferOwnerDelegateResult TransferServiceRequest(List<SrTransferEntity> transfers, int currentUser);
        List<BranchEntity> AutoCompleteSearchBranch(string keyword, int limit);
        List<ProductEntity> AutoCompleteSearchProduct(string keyword, int? productGroupId, int limit);

        List<CampaignServiceEntity> AutoCompleteSearchCampaignService(string keyword, int? productGroupId, int? productId, int limit);

        List<AreaItemEntity> AutoCompleteSearchArea(string keyword, int? subAreaId, int limit);

        List<AreaItemEntity> AutoCompleteSearchAreaOnMapping(string keyword, int? campaignServiceId, int? subAreaId, int? typeId, int limit);

        List<SubAreaItemEntity> AutoCompleteSearchSubArea(string keyword, int? areaId, int limit);

        List<SubAreaItemEntity> AutoCompleteSearchSubAreaOnMapping(string keyword, int? campaignServiceId, int? areaId, int? typeId, int limit);

        IEnumerable<ServiceRequestEntity> GetServiceRequestListByUserIds(List<int> userIds, string statusCode);

        List<UserEntity> AutoCompleteSearchUser(string keyword, List<int> userIds, int branchId, int limit);
    }
}
