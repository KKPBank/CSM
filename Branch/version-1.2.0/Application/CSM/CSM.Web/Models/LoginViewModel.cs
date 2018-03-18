using System;
using System.Security.Principal;
using CSM.Business;
using CSM.Common.Utilities;
using CSM.Entity;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CSM.Web.Models
{
    [Serializable]
    public class LoginViewModel
    {
        [Display(Name = "User name")]
        [LocalizedStringLengthAttribute(Constants.MaxLength.Username, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [LocalizedStringLengthAttribute(Constants.MaxLength.Password, ErrorMessageResourceName = "ValErr_StringLength", 
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public string Password { get; set; }

        [Display(Name = "Remember on this computer")]
        public bool RememberMe { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsValid(string username, string password)
        {
            IUserFacade userFacade = null;

            try
            {
                userFacade = new UserFacade();
                UserEntity user = userFacade.Login(username, password);

                if (user == null)
                {
                    return false;
                }
                else
                {
                    string sid = HttpContext.Current.Session.SessionID;
                    HttpContext.Current.Session["sessionid"] = sid;
                    userFacade.SaveLogin(user.Username, sid);
                    return true;
                }
            }
            finally
            {
                if (userFacade != null) { userFacade.Dispose(); }
            }
        }
    }
}