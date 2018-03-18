using System.Collections.Generic;
using System.Web.Mvc;
using CSM.Business;
using CSM.Entity;
using CSM.Web.Controllers.Common;

namespace CSM.Web.Controllers
{
    public class MenuNavigatorController : BaseController
    {
        private ICommonFacade _commonFacade;

        public ActionResult MainMenu(string selectedMenu)
        {
            _commonFacade = new CommonFacade();
            int roleValue = this.UserInfo == null ? 0 : this.UserInfo.RoleValue;
            List<MenuEntity> mainMenu = _commonFacade.GetCacheMainMenu(selectedMenu, roleValue);
            return PartialView("~/Views/MenuNavigator/MainMenu.cshtml", mainMenu);
        }

        public ActionResult CustomerTab(string selectedTab, int? customerId = null)
        {
            _commonFacade = new CommonFacade();
            List<MenuEntity> customerTab = _commonFacade.GetCacheCustomerTab(selectedTab, customerId);
            return PartialView("~/Views/MenuNavigator/CustomerTab.cshtml", customerTab);
        }
    }
}
