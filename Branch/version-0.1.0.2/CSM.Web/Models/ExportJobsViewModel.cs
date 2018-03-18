using CSM.Entity;
using System.Collections.Generic;

namespace CSM.Web.Models
{
    public class ExportJobsViewModel
    {
        public ExportJobsSearchFilter SearchFilter { get; set; }
        public IEnumerable<ExportJobsEntity> JobsList { get; set; }
    }
}