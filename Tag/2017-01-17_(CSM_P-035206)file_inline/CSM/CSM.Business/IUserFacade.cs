using System;
using System.Collections.Generic;
using CSM.Entity;
using CSM.Service.Messages.User;

namespace CSM.Business
{
    public interface IUserFacade : IDisposable
    {
        UserEntity Login(string username, string passwd);
        void SaveLogin(string loginName, string sid);
        void CheckExceededMaxConcurrent(string username, System.Web.HttpSessionStateBase session);
        List<UserEntity> GetEmployees(UserEntity manager);
        List<UserEntity> GetDummyUsers(UserEntity manager);
        UserEntity GetUserById(int userId);
        List<UserEntity> GetUsersBySupervisorIds(List<int> supervisorIds);
        List<int> GetUserIdsBySupervisorIds(List<int> supervisorIds);
        List<int> GetBranchIdsByUserIds(List<int> userIds);
        List<int> GetDummyUserIdsByUserIds(List<int> userIds);
        List<UserEntity> AutoCompleteSearchUserByUserIds(string keyword, int branchId, List<int> userIds, int limit);
        InsertOrUpdateUserResponse InsertOrUpdateUser(InsertOrUpdateUserRequest request);
        UserEntity GetUserByLogin(string login);
        int GetRoleByUser(string username);
        int? GetUserIdByLogin(string username);
    }
}