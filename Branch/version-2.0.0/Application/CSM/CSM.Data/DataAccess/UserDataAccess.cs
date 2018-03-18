using System;
using System.Collections.Generic;
using System.Linq;
using CSM.Common.Resources;
using CSM.Common.Utilities;
using CSM.Entity;
using log4net;
using CSM.Service.Messages.User;

namespace CSM.Data.DataAccess
{
    public class UserDataAccess : IUserDataAccess
    {
        private readonly CSMContext _context;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserDataAccess));

        public UserDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        public UserEntity GetUserByLoginName(string login)
        {
            try
            {
                IQueryable<UserEntity> query = from u in _context.TB_R_USER.AsNoTracking()
                                               join b in _context.TB_R_BRANCH on u.BRANCH_ID equals b.BRANCH_ID
                                               join c in _context.TB_R_CHANNEL on b.CHANNEL_ID equals c.CHANNEL_ID
                                               join r in _context.TB_C_ROLE on u.ROLE_ID equals r.ROLE_ID
                                               where u.STATUS == Constants.ApplicationStatus.Active &&
                                                     u.USERNAME.ToUpper() == login.ToUpper()
                                               select new UserEntity
                                               {
                                                   UserId = u.USER_ID,
                                                   Username = u.USERNAME,
                                                   Firstname = u.FIRST_NAME,
                                                   Lastname = u.LAST_NAME,
                                                   Status = u.STATUS,
                                                   Email = u.EMAIL,
                                                   BranchId = u.BRANCH_ID,
                                                   BranchCode = b.BRANCH_CODE,
                                                   BranchName = b.BRANCH_NAME,
                                                   RoleCode = r.ROLE_CODE,
                                                   RoleValue = r.ROLE_VALUE ?? 0,
                                                   PositionCode = u.POSITION_CODE,
                                                   ChannelId = c.CHANNEL_ID,
                                                   ChannelName = c.CHANNEL_NAME,
                                               };

                var user = query.Any() ? query.FirstOrDefault() : null;
                return user;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException(Resource.Msg_CannotConnectToDB);
            }
        }

        public bool CheckValidUser(string login)
        {
            try
            {
                return _context.TB_R_USER.Any(x => x.USERNAME.ToUpper() == login.ToUpper() && x.STATUS == Constants.ApplicationStatus.Active);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException(Resource.Msg_CannotConnectToDB);
            }
        }

        /// <summary>
        /// Check to see if your ID in the Logins table has LoggedIn = true
        /// If so, continue, otherwise, redirect to Login page.
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public bool IsYourLoginStillTrue(string loginName, string sid)
        {
            var query = from l in _context.TB_L_LOGIN
                        where l.LOGGED_IN == 1 && l.LOGIN_NAME == loginName && l.SESSION_ID == sid
                        select l;

            return query.Any();
        }

        /// <summary>
        /// Check to see if your login name is being used elsewhere under a different session ID
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public bool IsUserLoggedOnElsewhere(string loginName, string sid)
        {
            var query = from x in _context.TB_L_LOGIN
                        where x.LOGGED_IN == 1 && x.LOGIN_NAME == loginName && x.SESSION_ID != sid
                        select x;

            return query.Any();
        }

        /// <summary>
        /// If it is being used elsewhere, update all their Logins records to LoggedIn = false, except for your session ID
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="sid"></param>
        public void LogEveryoneElseOut(string loginName, string sid)
        {
            var query = from x in _context.TB_L_LOGIN
                        where x.LOGGED_IN == 1 && x.LOGIN_NAME == loginName && x.SESSION_ID != sid // Need to filter by login name
                        select x;

            foreach (TB_L_LOGIN entity in query)
            {
                entity.LOGGED_IN = 0;
            }

            this.Save();
        }

        /// <summary>
        /// Save user login session in database 
        /// </summary>
        /// <param name="loginName">Login Name</param>
        /// <param name="sid">Session ID</param>
        public void SaveLogin(string loginName, string sid)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var query = from x in _context.TB_L_LOGIN
                            where x.LOGGED_IN == 1 && x.LOGIN_NAME == loginName && x.SESSION_ID == sid
                            // Need to filter by login name
                            select x;

                if (query.Any())
                {
                    TB_L_LOGIN entity = query.FirstOrDefault();
                    entity.LOGGED_IN = 0;
                    _context.Entry(entity).Property("LOGGED_IN").IsModified = true;
                }

                TB_L_LOGIN newLogin = new TB_L_LOGIN();
                newLogin.LOGGED_IN = 1;
                newLogin.LOGIN_NAME = loginName;
                newLogin.SESSION_ID = sid;
                newLogin.CREATE_DATE = DateTime.Now;
                _context.TB_L_LOGIN.Add(newLogin);
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = false;
            }
        }

        public List<UserEntity> GetEmployees(UserEntity manager)
        {
            var result = new List<UserEntity>();
            var employees = _context.TB_R_USER.Where(x => x.SUPERVISOR_ID == manager.UserId && x.IS_GROUP == false)
                            .Select(x => new UserEntity
                            {
                                UserId = x.USER_ID,
                                BranchId = x.BRANCH_ID
                            });

            if (employees.Any())
            {
                foreach (var employee in employees)
                {
                    result.Add(employee);
                    result.AddRange(GetEmployees(employee));
                }
            }

            result.Add(manager);

            return result;
        }

        public List<UserEntity> GetDummyUsers(UserEntity manager)
        {
            var dummyUsers = new List<UserEntity>();
            var employees = this.GetEmployees(manager);

            if (employees != null && employees.Count > 0)
            {
                var branchIds = (from em in employees group em by em.BranchId into g select g.Key).ToArray();
                if (branchIds != null && branchIds.Length > 0)
                {
                    dummyUsers = (from ur in _context.TB_R_USER
                                  where branchIds.Contains(ur.BRANCH_ID) && ur.IS_GROUP == true
                                  select new UserEntity
                                  {
                                      UserId = ur.USER_ID,
                                      BranchId = ur.BRANCH_ID
                                  }).ToList();
                }
            }

            return dummyUsers;
        }

        public List<UserEntity> AutoCompleteSearchUser(string keyword, int? branchId, int limit)
        {
            var query = _context.TB_R_USER.AsQueryable();

            query = query.Where(q => q.STATUS == 1 || q.STATUS == null);

            if (branchId.HasValue)
            {
                query = query.Where(q => q.BRANCH_ID == branchId);
            }


            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q =>
                    q.FIRST_NAME.Contains(keyword)
                    || q.LAST_NAME.Contains(keyword));
            }

            query = query.OrderBy(q => q.FIRST_NAME);

            return query.Take(limit).Select(item => new UserEntity
            {
                UserId = item.USER_ID,
                Firstname = item.FIRST_NAME,
                Lastname = item.LAST_NAME,
                PositionCode = item.POSITION_CODE,
            }).ToList();
        }

        public List<UserEntity> AutoCompleteSearchUserWithJobOnHand(string keyword, int branchId, int limit)
        {
            return (from user in _context.TB_R_USER
                    where user.STATUS == 1 && user.BRANCH_ID == branchId
                        && (string.IsNullOrEmpty(keyword) || user.FIRST_NAME.Contains(keyword) || user.LAST_NAME.Contains(keyword))
                    orderby user.FIRST_NAME
                    select new UserEntity()
                    {
                        UserId = user.USER_ID,
                        Firstname = user.FIRST_NAME,
                        Lastname = user.LAST_NAME,
                        PositionCode = user.POSITION_CODE,
                        JobOnHand = _context.TB_T_SR.Count(x =>
                            Constants.SRStatusId.JobOnHandStatuses.Contains(x.SR_STATUS_ID.Value)
                            &&
                            (
                                ((x.OWNER_USER_ID == user.USER_ID) && (!x.DELEGATE_USER_ID.HasValue))
                                || (x.DELEGATE_USER_ID == user.USER_ID)
                            )
                        ),
                    }).Take(limit).ToList();
        }

        public UserEntity GetUserById(int id)
        {
            IQueryable<UserEntity> query = from u in _context.TB_R_USER
                                           join b in _context.TB_R_BRANCH on u.BRANCH_ID equals b.BRANCH_ID
                                           join r in _context.TB_C_ROLE on u.ROLE_ID equals r.ROLE_ID
                                           where u.STATUS == Constants.ApplicationStatus.Active &&
                                                 u.USER_ID == id
                                           select new UserEntity
                                           {
                                               UserId = u.USER_ID,
                                               Username = u.USERNAME,
                                               Firstname = u.FIRST_NAME,
                                               Lastname = u.LAST_NAME,
                                               Status = u.STATUS,
                                               Email = u.EMAIL,
                                               BranchId = u.BRANCH_ID,
                                               BranchCode = b.BRANCH_CODE,
                                               BranchName = b.BRANCH_NAME,
                                               RoleCode = r.ROLE_CODE,
                                               RoleValue = r.ROLE_VALUE ?? 0,
                                               PositionCode = u.POSITION_CODE,
                                               EmployeeCode = u.EMPLOYEE_CODE,
                                           };

            var user = query.Any() ? query.FirstOrDefault() : null;
            return user;
        }

        public List<int> GetUserIdsBySupervisorId(int supervisorId)
        {
            return (from u in _context.TB_R_USER
                    where u.SUPERVISOR_ID.HasValue && u.SUPERVISOR_ID.Value == supervisorId && u.IS_GROUP == false && u.STATUS == 1
                    select u.USER_ID).ToList();
        }

        public List<int> GetUserIdsBySupervisorIds(List<int> supervisorIds)
        {
            return (from u in _context.TB_R_USER
                    where u.SUPERVISOR_ID.HasValue && supervisorIds.Contains(u.SUPERVISOR_ID.Value) && u.IS_GROUP == false && u.STATUS == 1
                    select u.USER_ID).ToList();
        }

        public List<UserEntity> GetUserByUserIds(List<int> ids)
        {
            var q = from u in _context.TB_R_USER
                    join b in _context.TB_R_BRANCH on u.BRANCH_ID equals b.BRANCH_ID
                    join r in _context.TB_C_ROLE on u.ROLE_ID equals r.ROLE_ID
                    where ids.Contains(u.USER_ID) && u.STATUS == 1
                    select new UserEntity
                    {
                        UserId = u.USER_ID,
                        Username = u.USERNAME,
                        Firstname = u.FIRST_NAME,
                        Lastname = u.LAST_NAME,
                        Status = u.STATUS,
                        Email = u.EMAIL,
                        BranchId = u.BRANCH_ID,
                        BranchCode = b.BRANCH_CODE,
                        BranchName = b.BRANCH_NAME,
                        RoleCode = r.ROLE_CODE,
                        RoleValue = r.ROLE_VALUE ?? 0,
                        PositionCode = u.POSITION_CODE,
                        IsGroup = u.IS_GROUP.HasValue && u.IS_GROUP.Value
                    };

            return q.ToList();
        }

        public List<UserEntity> GetDummyUserByBranchIds(List<int> branchIds)
        {
            var employees = (from u in _context.TB_R_USER
                             join b in _context.TB_R_BRANCH on u.BRANCH_ID equals b.BRANCH_ID
                             join r in _context.TB_C_ROLE on u.ROLE_ID equals r.ROLE_ID
                             where u.BRANCH_ID.HasValue && branchIds.Contains(u.BRANCH_ID.Value) && u.IS_GROUP.HasValue && u.IS_GROUP.Value && u.STATUS == 1
                             select new UserEntity
                             {
                                 UserId = u.USER_ID,
                                 Username = u.USERNAME,
                                 Firstname = u.FIRST_NAME,
                                 Lastname = u.LAST_NAME,
                                 Status = u.STATUS,
                                 Email = u.EMAIL,
                                 BranchId = u.BRANCH_ID,
                                 BranchCode = b.BRANCH_CODE,
                                 BranchName = b.BRANCH_NAME,
                                 RoleCode = r.ROLE_CODE,
                                 RoleValue = r.ROLE_VALUE ?? 0,
                                 PositionCode = u.POSITION_CODE
                             }).ToList();

            return employees;
        }

        public List<int> GetDummyUserIdsByUserIds(List<int> ids)
        {
            return
                _context.TB_R_USER
                    .Where(
                        dummy => (dummy.IS_GROUP ?? false)
                        && _context.TB_R_USER
                            .Where(user => ids.Contains(user.USER_ID) && user.STATUS == 1)
                            .Select(user => user.BRANCH_ID).Contains(dummy.BRANCH_ID)
                    )
                    .Select(u => u.USER_ID)
                    .ToList();
        }

        public UserEntity GetBranchUserByUserId(int uid)
        {
            int? branchId = (from us in _context.TB_R_USER.AsNoTracking()
                            where us.USER_ID == uid
                            select us.BRANCH_ID).FirstOrDefault();
            if (branchId.HasValue)
            {
                return (from ub in _context.TB_R_USER.AsNoTracking()
                        where ub.BRANCH_ID == branchId.Value && (ub.IS_GROUP ?? false) && ub.STATUS == 1
                        select new UserEntity()
                        {
                            UserId = ub.USER_ID,
                            BranchId = ub.BRANCH_ID,
                            Email = ub.EMAIL
                        }).FirstOrDefault();
            }
            return null;
        }

        public List<int> GetBranchIdsByUserIds(List<int> userIds)
        {
            return (from user in _context.TB_R_USER
                    from branch in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == user.BRANCH_ID).DefaultIfEmpty()
                    where userIds.Contains(user.USER_ID) && user.BRANCH_ID.HasValue && branch.STATUS == 1
                    select user.BRANCH_ID.Value).Distinct().ToList();
        }

        public List<UserEntity> AutoCompleteSearchUserByUserIds(string keyword, int branchId, List<int> userIds, int limit)
        {
            return (from u in _context.TB_R_USER
                    join b in _context.TB_R_BRANCH on u.BRANCH_ID equals b.BRANCH_ID
                    join r in _context.TB_C_ROLE on u.ROLE_ID equals r.ROLE_ID
                    where userIds.Contains(u.USER_ID)
                          && u.STATUS == 1
                          && u.BRANCH_ID.HasValue
                          && u.BRANCH_ID.Value == branchId
                          && (string.IsNullOrEmpty(keyword) || (!string.IsNullOrEmpty(keyword) && (u.FIRST_NAME.Contains(keyword) || u.LAST_NAME.Contains(keyword))))
                    orderby u.FIRST_NAME, u.LAST_NAME
                    select new UserEntity
                    {
                        UserId = u.USER_ID,
                        Username = u.USERNAME,
                        Firstname = u.FIRST_NAME,
                        Lastname = u.LAST_NAME,
                        Status = u.STATUS,
                        Email = u.EMAIL,
                        BranchId = u.BRANCH_ID,
                        BranchCode = b.BRANCH_CODE,
                        BranchName = b.BRANCH_NAME,
                        RoleCode = r.ROLE_CODE,
                        RoleValue = r.ROLE_VALUE ?? 0,
                        PositionCode = u.POSITION_CODE,
                        IsGroup = u.IS_GROUP.HasValue && u.IS_GROUP.Value
                    }).Take(limit).ToList();
        }

        public int? GetUserIdByEmployeeCode(string employeeCode)
        {
            var item = _context.TB_R_USER.Where(x => x.EMPLOYEE_CODE.Trim().ToUpper() == employeeCode.Trim().ToUpper()).Select(x => new { x.USER_ID, x.USERNAME }).FirstOrDefault();
            if (item == null)
                return null;
            else
                return item.USER_ID;
        }

        public int? GetRoleIdByRoleCode(string roleCode)
        {
            var item = _context.TB_C_ROLE.Where(x => x.ROLE_CODE.Trim().ToUpper() == roleCode.Trim().ToUpper()).Select(x => new { x.ROLE_ID, x.ROLE_CODE }).FirstOrDefault();
            if (item == null)
                return null;
            else
                return item.ROLE_ID;
        }

        public InsertOrUpdateUserResponse InsertOrUpdateUser(InsertOrUpdateUserRequest request, int? supervisorUserId, int roleId, int branchId)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var result = new InsertOrUpdateUserResponse();

                TB_R_USER dbUser = null;

                var transactionDateTime = DateTime.Now;

                //if (request.ActionType == 1)
                //{
                //    // Insert
                //    dbUser = null;
                //}
                //else
                //{
                //    // Update
                //    dbUser = _context.TB_R_USER.Where(user => user.EMPLOYEE_CODE.Trim().ToUpper() == request.EmployeeCodeOld.Trim().ToUpper()).FirstOrDefault();

                //    if (dbUser == null)
                //    {
                //        result.IsSuccess = false;
                //        result.ErrorCode = "8";
                //        result.ErrorMessage = "ไม่สามารถอัพเดตข้อมูล User เนื่องจากไม่พบข้อมูล User (Employee Code Old='" + request.EmployeeCodeOld + "')";
                //        return result;
                //    }
                //}

                if (!string.IsNullOrEmpty(request.EmployeeCodeOld))
                {
                    dbUser = _context.TB_R_USER.Where(user => user.EMPLOYEE_CODE.Trim().ToUpper() == request.EmployeeCodeOld.Trim().ToUpper()).FirstOrDefault();
                }

                if (dbUser == null)
                {
                    dbUser = new TB_R_USER();
                    result.IsNewUser = true;
                }
                else
                {
                    result.IsNewUser = false;

                    // IF (Change Branch) Or (Change Status 1 to 0)
                    if (dbUser.BRANCH_ID != branchId || (dbUser.STATUS == 1 && request.Status == 0))
                    {
                        #region == Validate Job On Hand ==

                        var userId = dbUser.USER_ID;

                        var activeStatuses = new int[]
                        {
                            Constants.SRStatusId.Open,
                            Constants.SRStatusId.InProgress,
                            Constants.SRStatusId.WaitingCustomer,
                            Constants.SRStatusId.RouteBack,
                        };

                        var countJobOnHand = _context.TB_T_SR.Count(sr =>
                                (
                                    (sr.OWNER_USER_ID == userId || sr.DELEGATE_USER_ID == userId)
                                    && activeStatuses.Contains(sr.SR_STATUS_ID.Value)
                                )
                                ||
                                (
                                    sr.CREATE_USER == userId
                                    && sr.SR_STATUS_ID == Constants.SRStatusId.Draft
                                )
                            );

                        if (countJobOnHand > 0)
                        {
                            result.IsSuccess = false;
                            result.ErrorCode = "7";
                            result.ErrorMessage = "ไม่สามารถอัพเดตข้อมูล User ได้เนื่องจากมีงานค้างในมือ (ServiceRequest)";
                            return result;
                        }

                        #endregion
                    }
                }

                var actionUser = _context.TB_R_USER.Where(user => user.USERNAME.Trim().ToUpper() == request.ActionUsername.Trim().ToUpper()).FirstOrDefault();
                if (actionUser == null)
                {
                    //if (dbUser == null)
                    //{
                    //    result.IsSuccess = false;
                    //    result.ErrorCode = "9";
                    //    result.ErrorMessage = "ไม่สามารถอัพเดตข้อมูล User เนื่องจากไม่พบข้อมูล Action User (Action Username='" + request.ActionUsername + "')";
                    //    return result;
                    //}

                    actionUser = _context.TB_R_USER.Where(user => user.USERNAME.Trim().ToUpper() == Constants.SystemUserName).FirstOrDefault();
                }

                dbUser.BRANCH_ID = branchId;
                dbUser.SUPERVISOR_ID = supervisorUserId;
                dbUser.FIRST_NAME = ValueOrDefault(request.FirstName);
                dbUser.LAST_NAME = ValueOrDefault(request.LastName);
                dbUser.USERNAME = ValueOrDefault(request.WindowsUsername);
                dbUser.STATUS = Convert.ToInt16(request.Status);
                dbUser.ROLE_ID = roleId;
                dbUser.EMPLOYEE_CODE = ValueOrDefault(request.EmployeeCodeNew);
                dbUser.UPDATE_DATE = transactionDateTime;
                dbUser.IS_GROUP = request.IsGroup;
                dbUser.POSITION_CODE = ValueOrDefault(request.PositionCode);
                dbUser.MARKETING_CODE = ValueOrDefault(request.MarketingCode);
                dbUser.EMAIL = ValueOrDefault(request.Email);
                dbUser.ROLE_SALE = ValueOrDefault(request.RoleSale);
                dbUser.MARKETING_TEAM = ValueOrDefault(request.MarketingTeam);
                dbUser.LINE = ValueOrDefault(request.Line);
                dbUser.RANK = ValueOrDefault(request.Rank);
                dbUser.EMPLOYEE_TYPE = ValueOrDefault(request.EmployeeType);
                dbUser.COMPANY_NAME = ValueOrDefault(request.CompanyName);
                dbUser.TELESALE_TEAM = ValueOrDefault(request.TelesaleTeam);
                dbUser.MARKETING_FLAG = request.MarketingFlag;
                dbUser.EXPORT_DATE = null;

                if (result.IsNewUser)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        var phone = new TB_R_USER_PHONE();
                        phone.TB_R_USER = dbUser;
                        phone.PHONE_NO = i == 0 ? request.Phone1 : i == 1 ? request.Phone2 : request.Phone3;
                        phone.ORDER = Convert.ToInt16(i + 1);
                        phone.CREATE_DATE = transactionDateTime;
                        phone.UPDATE_DATE = transactionDateTime;
                        _context.TB_R_USER_PHONE.Add(phone);
                    }
                }
                else
                {
                    var dbPhoneList = dbUser.TB_R_USER_PHONE.OrderBy(p => p.ORDER).Take(10).ToList();
                    InsertOrUpdateUserPhone(dbPhoneList, request.Phone1, 1, dbUser, transactionDateTime);
                    InsertOrUpdateUserPhone(dbPhoneList, request.Phone2, 2, dbUser, transactionDateTime);
                    InsertOrUpdateUserPhone(dbPhoneList, request.Phone3, 3, dbUser, transactionDateTime);
                }

                if (result.IsNewUser)
                {
                    dbUser.CREATE_USERNAME = ValueOrDefault(request.ActionUsername);
                    dbUser.UPDATE_USERNAME = ValueOrDefault(request.ActionUsername);
                    dbUser.CREATE_DATE = transactionDateTime;
                    dbUser.UPDATE_DATE = transactionDateTime;

                    _context.TB_R_USER.Add(dbUser);
                }
                else
                {
                    dbUser.UPDATE_USERNAME = ValueOrDefault(request.ActionUsername);
                    dbUser.UPDATE_DATE = transactionDateTime;
                    SetEntryStateModified(dbUser);
                }

                this.Save();

                if (!request.IsGroup)
                {
                    var _customerDataAccess = new CustomerDataAccess(_context);

                    CustomerEntity customerEntity;

                    bool isCreateCustomer;
                    if (result.IsNewUser)
                    {
                        isCreateCustomer = true;
                        customerEntity = new CustomerEntity();
                    }
                    else
                    {
                        customerEntity = _customerDataAccess.GetCustomerByEmployeeID(dbUser.USER_ID);

                        // If Update but no have Customer for this user >> It will create
                        if (customerEntity == null)
                        {
                            customerEntity = new CustomerEntity();
                            isCreateCustomer = true;
                        }
                        else
                        {
                            isCreateCustomer = false;
                        }
                    }

                    // Create/Update Customer for this user

                    if (isCreateCustomer)
                    {
                        // Create Customer
                        var _commonDataAccess = new CommonDataAccess(_context);
                        var subscriptionType = _commonDataAccess.GetSubscriptTypeByCode(Constants.DefaultSubscriptionTypeForUser);

                        customerEntity.CustomerType = Constants.CustomerType.Employee;
                        customerEntity.FirstNameThai = request.FirstName;
                        customerEntity.LastNameThai = request.LastName;
                        customerEntity.SubscriptType = subscriptionType;
                        customerEntity.CardNo = request.EmployeeCodeNew;
                        customerEntity.Email = request.Email;
                        customerEntity.EmployeeId = dbUser.USER_ID;

                        customerEntity.TitleThai = new TitleEntity();
                        customerEntity.TitleEnglish = new TitleEntity();

                        customerEntity.CreateUser = new UserEntity() { UserId = actionUser.USER_ID };
                        customerEntity.UpdateUser = new UserEntity() { UserId = actionUser.USER_ID };

                        customerEntity.PhoneList = new List<PhoneEntity>();

                        if (!string.IsNullOrEmpty(request.Phone1))
                            customerEntity.PhoneList.Add(new PhoneEntity() { PhoneNo = request.Phone1 });

                        if (!string.IsNullOrEmpty(request.Phone2))
                            customerEntity.PhoneList.Add(new PhoneEntity() { PhoneNo = request.Phone2 });

                        if (!string.IsNullOrEmpty(request.Phone3))
                            customerEntity.PhoneList.Add(new PhoneEntity() { PhoneNo = request.Phone3 });

                        var isSaveCustomerSuccess = _customerDataAccess.SaveCustomer(customerEntity);

                        if (!isSaveCustomerSuccess)
                        {
                            return new InsertOrUpdateUserResponse()
                            {
                                IsSuccess = false,
                                ErrorCode = "6",
                                ErrorMessage = "Cannot create customer for this user.",
                            };
                        }
                    }
                    else
                    {
                        customerEntity.FirstNameThai = request.FirstName;
                        customerEntity.LastNameThai = request.LastName;
                        customerEntity.CardNo = request.EmployeeCodeNew;
                        customerEntity.Email = request.Email;

                        customerEntity.TitleThai = new TitleEntity();
                        customerEntity.TitleEnglish = new TitleEntity();

                        customerEntity.CreateUser = new UserEntity() { UserId = actionUser.USER_ID };
                        customerEntity.UpdateUser = new UserEntity() { UserId = actionUser.USER_ID };

                        if (customerEntity.PhoneList == null)
                            customerEntity.PhoneList = new List<PhoneEntity>();

                        var phones = new List<string>();

                        if (!string.IsNullOrEmpty(request.Phone1))
                            phones.Add(request.Phone1);

                        if (!string.IsNullOrEmpty(request.Phone2))
                            phones.Add(request.Phone2);

                        if (!string.IsNullOrEmpty(request.Phone3))
                            phones.Add(request.Phone3);

                        if (phones.Count > customerEntity.PhoneList.Count)
                        {
                            var diff = (phones.Count - customerEntity.PhoneList.Count);
                            for (int i = 0; i < diff; i++)
                            {
                                customerEntity.PhoneList.Add(new PhoneEntity());
                            }
                        }
                        else if (phones.Count < customerEntity.PhoneList.Count)
                        {
                            var diff = (customerEntity.PhoneList.Count - phones.Count);
                            for (int i = 0; i < diff; i++)
                            {
                                customerEntity.PhoneList.RemoveAt(customerEntity.PhoneList.Count - 1);
                            }
                        }

                        for (int i = 0; i < phones.Count; i++)
                        {
                            customerEntity.PhoneList[i].PhoneNo = phones[i];
                        }

                        var isSaveCustomerSuccess = _customerDataAccess.SaveCustomer(customerEntity);

                        if (!isSaveCustomerSuccess)
                        {
                            return new InsertOrUpdateUserResponse()
                            {
                                IsSuccess = false,
                                ErrorCode = "6",
                                ErrorMessage = "Cannot update customer for this user.",
                            };
                        }
                    }
                }

                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);

                return new InsertOrUpdateUserResponse()
                {
                    IsSuccess = false,
                    ErrorCode = "1",
                    ErrorMessage = ex.Message,
                };
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = false;
            }
        }

        public int GetRoleByUser(string username)
        {
            var query = from u in _context.TB_R_USER.AsNoTracking()
                        join b in _context.TB_R_BRANCH on u.BRANCH_ID equals b.BRANCH_ID
                        join c in _context.TB_R_CHANNEL on b.CHANNEL_ID equals c.CHANNEL_ID
                        join r in _context.TB_C_ROLE on u.ROLE_ID equals r.ROLE_ID
                        where u.STATUS == Constants.ApplicationStatus.Active &&
                              u.USERNAME.ToUpper() == username.ToUpper()
                        select r.ROLE_VALUE ?? 0;

            return query.Any() ? query.FirstOrDefault() : 0;
        }

        public int? GetUserIdByLogin(string username)
        {
            var query = from u in _context.TB_R_USER.AsNoTracking()
                        join b in _context.TB_R_BRANCH on u.BRANCH_ID equals b.BRANCH_ID
                        join c in _context.TB_R_CHANNEL on b.CHANNEL_ID equals c.CHANNEL_ID
                        where u.STATUS == Constants.ApplicationStatus.Active &&
                              u.USERNAME.ToUpper() == username.ToUpper()
                        select u.USER_ID;

            return query.Any() ? query.FirstOrDefault() : new Nullable<int>();
        }

        #region "Functions"

        private static string ValueOrDefault(string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : str.Trim();
        }

        private void InsertOrUpdateUserPhone(List<TB_R_USER_PHONE> dbPhoneList, string phoneNo, int order, TB_R_USER user, DateTime transactionDateTime)
        {
            var dbPhone = dbPhoneList.FirstOrDefault(x => x.ORDER == order);
            if (dbPhone == null)
            {
                // Insert
                dbPhone = new TB_R_USER_PHONE();
                dbPhone.TB_R_USER = user;
                dbPhone.PHONE_NO = phoneNo;
                dbPhone.ORDER = Convert.ToInt16(order);
                dbPhone.CREATE_DATE = transactionDateTime;
                dbPhone.UPDATE_DATE = transactionDateTime;
                _context.TB_R_USER_PHONE.Add(dbPhone);
            }
            else
            {
                // Update
                if (phoneNo != dbPhone.PHONE_NO)
                {
                    dbPhone.PHONE_NO = phoneNo;
                    dbPhone.UPDATE_DATE = transactionDateTime;
                }

                SetEntryStateModified(dbPhone);
            }
        }

        #endregion

        #region "Persistence"

        private int Save()
        {
            return _context.SaveChanges();
        }

        private void SetEntryCurrentValues(object entityTo, object entityFrom)
        {
            _context.Entry(entityTo).CurrentValues.SetValues(entityFrom);
            // Set state to Modified
            _context.Entry(entityTo).State = System.Data.Entity.EntityState.Modified;
        }

        private void SetEntryStateModified(object entity)
        {
            if (_context.Configuration.AutoDetectChangesEnabled == false)
            {
                // Set state to Modified
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
        }

        #endregion
    }
}