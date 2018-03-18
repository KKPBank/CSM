using System;
using System.Collections.Generic;

namespace CSM.Entity
{
    [Serializable]
    public class MenuEntity
    {
        #region "Local Declaration"

        private bool m_IsSelected = false;

        #endregion

        public int? MenuId { get; set; }
        public string MenuCode { get; set; }
        public string MenuName { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public int AccessRoles { get; set; }
        public string CssClass { get; set; }
        public List<ConfigureUrlEntity> ConfigUrlList { get; set; }
        public string TabContent { get; set; }

        public bool IsSelected
        {
            get { return m_IsSelected; }
            set { m_IsSelected = value; }
        }
    }

    public static class MenuCode
    {
        public const string NA = "00";
        public const string Home = "01";
        public const string Customer = "02";
        public const string ServiceRequest = "03";
        public const string CommPool = "04";
        public const string Master = "05";
        public const string Report = "06";
        public const string UserMonitoring = "07";
    }

    #region "Customer Tab"

    public static class CustomerTabCode
    {
        public const string NA = "00";
        public const string CustomerNote = "01";
        public const string Contact = "02";
        public const string ExistingProduct = "03";
        public const string SR = "04";
        public const string Activity = "05";
        public const string ExistingLead = "06";
        public const string Campaign = "07";
        public const string Document = "08";
        public const string Logging = "09";
    }

    #endregion
}