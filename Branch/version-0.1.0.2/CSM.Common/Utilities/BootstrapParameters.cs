using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSM.Common.Utilities
{
    public class BootstrapParameters
    {
        public BootstrapParameters()
        {
            order = "asc";
            limit = 15;
            offset = 0;
        }

        public string sort { get; set; }
        public string order { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
    }
}