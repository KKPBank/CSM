using CSM.Entity;
using System.Collections.Generic;

namespace CSM.Web.Models
{
    public class ExportNcbViewModel
    {
        public ExportNcbSearchFilter SearchFilter { get; set; }
        public IEnumerable<ExportNcbEntity> NcbList { get; set; }
    }
}