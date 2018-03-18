using System;
using System.Globalization;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using CSM.Common.Utilities;

namespace CSM.Business.Securities
{
    public class CustomRoleProvider : RoleProvider
    {
        private int _cacheTimeoutInMinute = 20;

        public override bool IsUserInRole(string username, string screenCode)
        {
            IUserFacade userFacade = null;
            ICommonFacade commonFacade = null;

            try
            {
                userFacade = new UserFacade();
                commonFacade = new CommonFacade();

                int userRole = userFacade.GetRoleByUser(username);
                int accessRoles = commonFacade.GetRoleValueByScreenCode(screenCode);
                if (accessRoles != 0 && (accessRoles & userRole) == 0)
                {
                    return false;
                }

                return true;
            }
            finally
            {
                if (userFacade != null) { userFacade.Dispose(); }
                if (commonFacade != null) { commonFacade.Dispose(); }
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            var cacheKey = string.Format(CultureInfo.InvariantCulture, "{0}_role", username);
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (string[])HttpRuntime.Cache[cacheKey];
            }

            string[] roles = null;
            using (IUserFacade userFacade = new UserFacade())
            {
                int? roleValue = userFacade.GetRoleByUser(username);
                roles = new string[] { roleValue.ConvertToString() };
                HttpRuntime.Cache.Insert(cacheKey, roles, null, DateTime.Now.AddMinutes(_cacheTimeoutInMinute), Cache.NoSlidingExpiration);
            }

            return roles;
        }

        public override void CreateRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new System.NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new System.NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new System.NotImplementedException();
        }

        public override string ApplicationName { get; set; }
    }
}
