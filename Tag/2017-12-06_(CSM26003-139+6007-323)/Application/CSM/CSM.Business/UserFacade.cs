using System;
using System.Collections.Generic;
using CSM.Common.Resources;
using CSM.Common.Securities;
using CSM.Common.Utilities;
using CSM.Data.DataAccess;
using CSM.Entity;
using System.Linq;
using log4net;
using System.Globalization;
using CSM.Service.Messages.Common;
using CSM.Service.Messages.User;

namespace CSM.Business
{
    public class UserFacade : IUserFacade
    {
        private ICommonFacade _commonFacade;
        private readonly CSMContext _context;
        private IUserDataAccess _userDataAccess;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserFacade));

        public UserFacade()
        {
            _context = new CSMContext();
        }

        public UserEntity Login(string username, string passwd)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Login").Add("UserName", username).ToInputLogString());
            UserEntity user = null;

            if (WebConfig.IsSkipAD())
            {
                if (CheckValidUser(username))
                {
                    user = GetUserByLogin(username);
                }
                else
                {
                    throw new CustomException(Resource.Msg_UserRoleNotFound);
                }
            }
            else
            {
                string result;
                using (var authen = new LdapAuthentication())
                {
                    result = authen.Login(username, passwd);
                }

                if ("SUCCESS".Equals(result))
                {
                    if (!CheckValidUser(username))
                    {
                        throw new CustomException(Resource.Msg_UserRoleNotFound);
                    }

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Login").ToSuccessLogString());
                    user = GetUserByLogin(username);
                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Login").ToFailLogString());
                }
            }

            return user;
        }

        public void CheckExceededMaxConcurrent(string username, System.Web.HttpSessionStateBase session)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                _userDataAccess = new UserDataAccess(_context);

                if (session["sessionid"] == null)
                {
                    session["sessionid"] = "empty";
                }

                // check to see if your ID in the Logins table has LoggedIn = true - if so, continue, otherwise, redirect to Login page.
                if (_userDataAccess.IsYourLoginStillTrue(username, (session["sessionid"] as string)))
                {
                    // check to see if your user ID is being used elsewhere under a different session ID
                    if (!_userDataAccess.IsUserLoggedOnElsewhere(username, (session["sessionid"] as string)))
                    {
                        // Do nothing
                    }
                    else
                    {
                        // if it is being used elsewhere, update all their Logins records to LoggedIn = false, except for your session ID
                        _userDataAccess.LogEveryoneElseOut(username, (session["sessionid"] as string));
                    }
                }
                else
                {
                    // Go to Login page
                    session["sessionid"] = null;
                }
            }
            else
            {
                // Go to Logout page
                session["sessionid"] = null;
                session.Clear();
                session.Abandon();
                session.RemoveAll();
            }
        }

        public void SaveLogin(string loginName, string sid)
        {
            _userDataAccess = new UserDataAccess(_context);
            _userDataAccess.SaveLogin(loginName, sid);
        }

        public List<UserEntity> GetEmployees(UserEntity manager)
        {
            _userDataAccess = new UserDataAccess(_context);
            return _userDataAccess.GetEmployees(manager);
        }

        public List<UserEntity> GetDummyUsers(UserEntity manager)
        {
            _userDataAccess = new UserDataAccess(_context);
            return _userDataAccess.GetDummyUsers(manager);
        }

        public UserEntity GetBranchUserByUserId(int uid)
        {
            return (new UserDataAccess(_context)).GetBranchUserByUserId(uid);
        }

        public UserEntity GetUserById(int userId)
        {
            _userDataAccess = new UserDataAccess(_context);
            return _userDataAccess.GetUserById(userId);
        }

        public List<UserEntity> GetUsersBySupervisorIds(List<int> supervisor)
        {
            var employeeIds = GetUserIdsBySupervisorIds(supervisor);

            _userDataAccess = new UserDataAccess(_context);
            return _userDataAccess.GetUserByUserIds(employeeIds);
        }

        public List<int> GetUserIdsBySupervisorIds(List<int> supervisor)
        {
            _userDataAccess = new UserDataAccess(_context);

            List<int> result = new List<int>();

            List<int> emps = _userDataAccess.GetUserIdsBySupervisorIds(supervisor);
            result.AddRange(emps);

            List<int> temp = new List<int>();
            temp.AddRange(emps);

            while (true)
            {
                var data = _userDataAccess.GetUserIdsBySupervisorIds(temp);

                result.AddRange(data);

                if (!data.Any())
                {
                    break;
                }

                temp = new List<int>();
                temp.AddRange(data);
            }

            return result.Distinct().ToList();
        }
        public List<UserEntity> AutoCompleteSearchUserByUserIds(string keyword, int branchId, List<int> userIds, int limit)
        {
            _userDataAccess = new UserDataAccess(_context);
            return _userDataAccess.AutoCompleteSearchUserByUserIds(keyword, branchId, userIds, limit);
        }

        public List<int> GetBranchIdsByUserIds(List<int> userIds)
        {
            _userDataAccess = new UserDataAccess(_context);
            return _userDataAccess.GetBranchIdsByUserIds(userIds);
        }

        public List<int> GetDummyUserIdsByUserIds(List<int> userIds)
        {
            if (_userDataAccess == null)
                _userDataAccess = new UserDataAccess(_context);

            return _userDataAccess.GetDummyUserIdsByUserIds(userIds);
        }

        public InsertOrUpdateUserResponse InsertOrUpdateUser(InsertOrUpdateUserRequest request)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            UserResponse response = new UserResponse();

            Logger.Info(_logMsg.Clear().SetPrefixMsg("Call UserService.InsertOrUpdateUser").ToInputLogString());
            Logger.Debug("I:--START--:--UserService.InsertOrUpdateUser--");

            try
            {
                if (request.ActionType != 1 && request.ActionType != 2)
                {
                    return new InsertOrUpdateUserResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "2",
                        ErrorMessage = "Action Type must be 1 or 2. (1=Insert,2=Update)"
                    };
                }

                bool valid = false;
                _commonFacade = new CommonFacade();

                if (request.Header != null)
                {
                    valid = _commonFacade.VerifyServiceRequest<Header>(request.Header);
                    response.Header = new Header
                    {
                        reference_no = request.Header.reference_no,
                        service_name = request.Header.service_name,
                        system_code = request.Header.system_code,
                        transaction_date = request.Header.transaction_date
                    };
                }

                Logger.DebugFormat("-- XMLRequest --\n{0}", request.SerializeObject());

                if (!valid)
                {
                    return new InsertOrUpdateUserResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "99",
                        ErrorMessage = "Bad Request, the header is not valid"
                    };
                }

                #region == Validate Require Field ==

                if (string.IsNullOrEmpty(request.WindowsUsername))
                    return GetReturnErrorRequireField("WindowUserName");

                if (string.IsNullOrEmpty(request.EmployeeCodeNew))
                    return GetReturnErrorRequireField("EmployeeCodeNew");

                if (request.ActionType == 2)
                {
                    if (string.IsNullOrEmpty(request.EmployeeCodeOld))
                        return GetReturnErrorRequireField("EmployeeCodeOld");
                }

                if (string.IsNullOrEmpty(request.FirstName))
                    return GetReturnErrorRequireField("FirstName");

                if (string.IsNullOrEmpty(request.Phone1) && !request.IsGroup)
                    return GetReturnErrorRequireField("Phone1", " (Phone1 เป็น Required Field เมื่อ IsGroup = FALSE)");

                if (string.IsNullOrEmpty(request.PositionCode))
                    return GetReturnErrorRequireField("PositionCode");

                if (string.IsNullOrEmpty(request.RoleSale))
                    return GetReturnErrorRequireField("RoleSale");

                if (string.IsNullOrEmpty(request.BranchCode))
                    return GetReturnErrorRequireField("BranchCode");

                if (string.IsNullOrEmpty(request.RoleCode))
                    return GetReturnErrorRequireField("RoleCode");

                #endregion

                #region == Validate Code ==

                _userDataAccess = new UserDataAccess(_context);

                int? supervisorUserId = null;

                if (!string.IsNullOrEmpty(request.SupervisorEmployeeCode))
                {
                    supervisorUserId = _userDataAccess.GetUserIdByEmployeeCode(request.SupervisorEmployeeCode);
                    if (!supervisorUserId.HasValue)
                    {
                        return new InsertOrUpdateUserResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "3",
                            ErrorMessage = "ไม่พบ Employee Code ของ Supervisor ในฐานข้อมูล CSM"
                        };
                    }
                }

                int? roleId = _userDataAccess.GetRoleIdByRoleCode(request.RoleCode);
                if (!roleId.HasValue)
                {
                    return new InsertOrUpdateUserResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "4",
                        ErrorMessage = "ไม่พบ Role Code ในฐานข้อมูล CSM",
                    };
                }

                var branchDataAccess = new BranchDataAccess(_context);
                int? branchId = branchDataAccess.GetBranchIdByBranchCode(request.BranchCode);
                if (!branchId.HasValue)
                {
                    Logger.DebugFormat("-- XMLResponse --\n{0}", response.SerializeObject());
                    stopwatch.Stop();
                    Logger.DebugFormat("O:--Finish--:ElapsedMilliseconds/{0}", stopwatch.ElapsedMilliseconds);

                    return new InsertOrUpdateUserResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "5",
                        ErrorMessage = "ไม่พบ Branch Code ในฐานข้อมูล CSM",
                    };
                }
                else
                {
                    response.StatusResponse = new StatusResponse
                    {
                        ErrorCode = string.Empty,
                        Status = Constants.StatusResponse.Success,
                        Description = "Save successful"
                    };

                    Logger.DebugFormat("-- XMLResponse --\n{0}", response.SerializeObject());
                    stopwatch.Stop();
                    Logger.DebugFormat("O:--Finish--:ElapsedMilliseconds/{0}", stopwatch.ElapsedMilliseconds);
                    var result = _userDataAccess.InsertOrUpdateUser(request, supervisorUserId, roleId.Value, branchId.Value);
                    if (!result.IsSuccess)
                    {
                        return new InsertOrUpdateUserResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "5",
                            ErrorMessage = result.ErrorMessage
                        };
                    }
                    return result;
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                return new InsertOrUpdateUserResponse()
                {
                    IsSuccess = false,
                    ErrorCode = "1",
                    ErrorMessage = ex.Message
                };
            }
        }

        private static InsertOrUpdateUserResponse GetReturnErrorRequireField(string fieldName, string remark = "")
        {
            return new InsertOrUpdateUserResponse
            {
                IsSuccess = false,
                ErrorCode = "2",
                ErrorMessage = string.Format(CultureInfo.InvariantCulture, "ข้อมูลที่ส่งมาไม่ครบถ้วน ไม่สามารถบันทึกรายการได้ (Field={0}){1}", fieldName, remark)
            };
        }

        public int GetRoleByUser(string username)
        {
            _userDataAccess = new UserDataAccess(_context);
            return _userDataAccess.GetRoleByUser(username);
        }

        public int? GetUserIdByLogin(string username)
        {
            _userDataAccess = new UserDataAccess(_context);
            return _userDataAccess.GetUserIdByLogin(username);
        }

        #region "Functions"

        private bool CheckValidUser(string login)
        {
            _userDataAccess = new UserDataAccess(_context);
            return _userDataAccess.CheckValidUser(login);
        }

        public UserEntity GetUserByLogin(string login)
        {
            _userDataAccess = new UserDataAccess(_context);
            return _userDataAccess.GetUserByLoginName(login);
        }

        #endregion

        #region "IDisposable"

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (_context != null) { _context.Dispose(); }
                    if (_commonFacade != null) { _commonFacade.Dispose(); }
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