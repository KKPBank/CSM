using System.Collections.Generic;
using CSM.Entity;
using CSM.Service.Messages.User;

namespace CSM.Data.DataAccess
{
    public interface IUserDataAccess
    {
        UserEntity GetUserByLoginName(string login);
        bool CheckValidUser(string login);
        bool IsYourLoginStillTrue(string loginName, string sid);
        bool IsUserLoggedOnElsewhere(string loginName, string sid);
        void LogEveryoneElseOut(string loginName, string sid);
        void SaveLogin(string loginName, string sid);
        List<UserEntity> GetEmployees(UserEntity manager);
        List<UserEntity> GetDummyUsers(UserEntity manager);
        List<int> GetUserIdsBySupervisorIds(List<int> supervisorIds);
        List<UserEntity> GetUserByUserIds(List<int> ids);
        List<UserEntity> GetDummyUserByBranchIds(List<int> branchIds);
        UserEntity GetUserById(int id);
        List<int> GetDummyUserIdsByUserIds(List<int> id);
        List<int> GetBranchIdsByUserIds(List<int> userIds);
        List<UserEntity> AutoCompleteSearchUserByUserIds(string keyword, int branchId, List<int> userIds, int limit);
        int? GetUserIdByEmployeeCode(string employeeCode);
        int? GetRoleIdByRoleCode(string p);
        InsertOrUpdateUserResponse InsertOrUpdateUser(InsertOrUpdateUserRequest request, int? supervisorUserId, int roleId, int branchId);
        int GetRoleByUser(string username);
        int? GetUserIdByLogin(string username);
    }
}