using CSM.Entity;
using System.Collections.Generic;

namespace CSM.Web.Models
{
    public class ExportVerifyViewModel
    {
        public ExportVerifySearchFilter SearchFilter { get; set; }
        public IEnumerable<ExportVerifyEntity> VerifyList { get; set; }
    }
}