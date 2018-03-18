using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Entity
{
    public class CbsCustTypeEntity
    {
        public int CustomerCategoryId { get; set; }
        public string CustomerCategoryValue { get; set; }
        public string ShortDescEng { get; set; }
        public string LongDescEng { get; set; }
        public string LongDescThai { get; set; }
        public string CustomerType { get; set; }
    }
}
