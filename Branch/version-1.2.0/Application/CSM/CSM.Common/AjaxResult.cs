using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Common
{
    public class AjaxResult
    {
        public AjaxResult()
        {
            IsSuccess = false;
            Data = null;
            ErrorCode = string.Empty;
            ErrorMessage = string.Empty;
        }

        public bool IsSuccess { get; set; }
        public object Data { get; set; }

        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
