using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Web.Models
{
    public class AuditLogViewModel
    { 
        public IEnumerable<AuditLogEntity> AuditLogList { get; set; }
        public AuditLogSearchFilter SearchFilter { get; set; }   
    }
}