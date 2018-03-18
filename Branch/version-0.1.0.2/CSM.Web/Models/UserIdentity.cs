using System.Security.Principal;
using System.Web.Security;
using CSM.Entity;

namespace CSM.Web.Models
{
    public class UserIdentity : IIdentity, IPrincipal
    {
        private readonly FormsAuthenticationTicket _ticket;

        public UserIdentity(FormsAuthenticationTicket ticket)
        {
            _ticket = ticket;
        }

        public string AuthenticationType
        {
            get { return "User"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name
        {
            get { return _ticket.Name; }
        }

        public string UserId
        {
            get { return _ticket.UserData; }
        }

        public bool IsInRole(string role)
        {
            return Roles.IsUserInRole(role);
        }

        public UserEntity UserInfo { get; set; }

        public IIdentity Identity
        {
            get { return this; }
        }
    }
}