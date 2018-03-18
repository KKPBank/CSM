using CSM.Entity;
using System.Collections.Generic;

namespace CSM.Web.Models
{
    public class ExportSRViewModel
    {
        public ExportSRSearchFilter SearchFilter { get; set; }
        public IEnumerable<ExportSREntity> SrList { get; set; }
    }
}