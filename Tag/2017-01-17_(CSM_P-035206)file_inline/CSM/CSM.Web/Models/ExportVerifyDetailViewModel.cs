using CSM.Entity;
using System.Collections.Generic;

namespace CSM.Web.Models
{
    public class ExportVerifyDetailViewModel
    {
        public ExportVerifyDetailSearchFilter SearchFilter { get; set; }
        public IEnumerable<ExportVerifyDetailEntity> VerifyDetailList { get; set; }
    }
}