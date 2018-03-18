using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Entity
{
    [Serializable]
    public class WebServicePagingEntity
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        //public int TotalRecord { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
    }
}
