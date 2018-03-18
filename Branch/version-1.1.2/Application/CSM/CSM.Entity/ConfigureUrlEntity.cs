using System;
using System.Collections.Generic;
using System.Linq;
using CSM.Entity.Common;
using CSM.Common.Utilities;
using System.Globalization;

namespace CSM.Entity
{
    [Serializable]
    public class ConfigureUrlEntity
    {
        public int ConfigureUrlId { get; set; }
        public string SystemName { get; set; }
        public string Url { get; set; }

        public MenuEntity Menu { get; set; }
        public UserEntity CreateUser { get; set; }
        public UserEntity UpdateUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? Updatedate { get; set; }

        public short? Status { get; set; }
        public string StatusDisplay
        {
            get { return Constants.ApplicationStatus.GetMessage(this.Status); }
        }

        public List<RoleEntity> Roles { get; set; }
        public string RolesDisplay
        {
            get { return StringHelpers.ConvertListToString(Roles.Select(x => x.RoleName).ToList<object>(), "<br>"); }
        }
        
        public string ImageFile { get; set; }
        public string ImageUrl
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0}{1}", Constants.ConfigUrlPath, this.ImageFile); }
        }

        public string FontName { get; set; }

        public ConfigureUrlEntity()
        {
            this.Roles = new List<RoleEntity>();
        }
    }

    public class ConfigureUrlSearchFilter : Pager
    {
         [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SystemName { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Url { get; set; }
        public short? Status { get; set; }
    }

    public class FontEntity
    {
        public int? FontId { get; set; }
        public string FontName { get; set; }
    }
}
