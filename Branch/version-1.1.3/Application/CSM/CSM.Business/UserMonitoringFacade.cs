using System;
using System.Collections.Generic;
using CSM.Business.Common;
using CSM.Data.DataAccess;
using CSM.Entity;

namespace CSM.Business
{
    public class UserMonitoringFacade : BaseFacade, IUserMonitoringFacade
    {
        private readonly CSMContext _context;
        private IUserDataAccess _userDataAccess;
        private ServiceRequestDataAccess _serviceRequestDataAccess; 

        public UserMonitoringFacade()
        {
            _context = new CSMContext();
            _context.Configuration.ValidateOnSaveEnabled = false;
        }

        public List<GroupAssignEntity> SearchGroupAssign(GroupAssignSearchFilter searchFilter)
        {
            var dataAccess = new UserMonitoringDataAccess(_context);
            return dataAccess.SearchGroupAssign(searchFilter);
        }

        public List<UserAssignEntity> SearchUserAssign(UserAssignSearchFilter searchFilter)
        {
            var dataAccess = new UserMonitoringDataAccess(_context);
            return dataAccess.SearchUserAssign(searchFilter);
        }

        public List<ServiceRequestEntity> SearchServiceRequest(UserMonitoringSrSearchFilter searchFilter)
        {
            var dataAccess = new UserMonitoringDataAccess(_context);
            return dataAccess.SearchServiceRequest(searchFilter);
        }

        public UserEntity GetUserByLoginName(string username)
        {
            _userDataAccess = new UserDataAccess(_context);
            return _userDataAccess.GetUserByLoginName(username);
        }

        public List<UserEntity> GetDummyUserByBranchIds(List<int> branchIds)
        {
            _userDataAccess = new UserDataAccess(_context);
            return _userDataAccess.GetDummyUserByBranchIds(branchIds);
        }

        public IEnumerable<ServiceRequestEntity> GetServiceRequestList(ServiceRequestSearchFilter searchFilter)
        {
            _serviceRequestDataAccess = new ServiceRequestDataAccess(_context);
            return _serviceRequestDataAccess.GetServiceRequestById(searchFilter);
        }

        public IEnumerable<ServiceRequestEntity> GetServiceRequestListByUserIds(List<int> userIds, string statusCode)
        {
            _serviceRequestDataAccess = new ServiceRequestDataAccess(_context);
            return _serviceRequestDataAccess.GetServiceRequestById(userIds, statusCode);
        }

        public List<BranchEntity> AutoCompleteSearchBranch(string keyword, int limit)
        {
            return new BranchDataAccess(_context).AutoCompleteSearchBranch(keyword, limit);
        }

        public List<ProductEntity> AutoCompleteSearchProduct(string keyword, int? productGroupId, int limit)
        {
            return new ProductDataAccess(_context).AutoCompleteSearchProduct(keyword, productGroupId, limit, null, false);
        }

        public List<CampaignServiceEntity> AutoCompleteSearchCampaignService(string keyword, int? productGroupId, int? productId, int limit)
        {
            return new CampaignServiceDataAccess(_context).AutoCompleteSearchCampaignService(keyword, productGroupId, productId, limit, false);
        }

        public List<AreaItemEntity> AutoCompleteSearchArea(string keyword, int? subAreaId, int limit)
        {
            return new AreaDataAccess(_context).AutoCompleteSearchArea(keyword, subAreaId, limit, false);
        }

        public List<AreaItemEntity> AutoCompleteSearchAreaOnMapping(string keyword, int? campaignServiceId, int? subAreaId, int? typeId, int limit)
        {
            return new AreaDataAccess(_context).AutoCompleteSearchAreaOnMapping(keyword, campaignServiceId, subAreaId, typeId, limit);
        }

        public List<SubAreaItemEntity> AutoCompleteSearchSubArea(string keyword, int? areaId, int limit)
        {
            return new SubAreaDataAccess(_context).AutoCompleteSearchSubArea(keyword, areaId, limit, false);
        }

        public List<SubAreaItemEntity> AutoCompleteSearchSubAreaOnMapping(string keyword, int? campaignServiceId, int? areaId, int? typeId, int limit)
        {
            return new SubAreaDataAccess(_context).AutoCompleteSearchSubAreaOnMapping(keyword, campaignServiceId, areaId, typeId, limit);
        }

        public TransferOwnerDelegateResult TransferServiceRequest(List<SrTransferEntity> transfers, int currentUser)
        {
            _serviceRequestDataAccess = new ServiceRequestDataAccess(_context);
            return _serviceRequestDataAccess.TransferOwnerDelegate(transfers, currentUser);
        }

        public List<UserEntity> AutoCompleteSearchUser(string keyword, List<int> userIds, int branchId, int limit)
        {
            _serviceRequestDataAccess = new ServiceRequestDataAccess(_context);
            return _serviceRequestDataAccess.AutoCompleteSearchUser(keyword, userIds, branchId, limit);
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
