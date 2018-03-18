using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSM.Common.Utilities
{
    public class BootstrapTableResult
    {
        public BootstrapTableResult()
        {
            total = 0;
            rows = null;
            is_error = false;
            error_message = string.Empty;
        }

        public int total { get; set; }
        public object rows { get; set; }
        public bool is_error { get; set; }
        public string error_message { get; set; }
    }
}