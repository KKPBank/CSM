using CSM.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSM.Web.Models
{
    public class SearchSRStatusModel
    {
        public SRStatusSearchFilter SearchFilter { get; set; }
        public List<SRStatusEntity> SearchList { get; set; }
    }

    [Serializable]
    public class SRStatusViewModel : SRStatusEntity
    {
        public SRStatusSearchFilter SearchFilter { get; set; }
        //public string SRPageIdListRequireMsg
        //{
        //    get
        //    {
        //        return string.Format(Common.Resources.SRResource.ValidationRequiredField, Common.Resources.Resource.Lbl_SR_Page);
        //    }
        //}
    }
}