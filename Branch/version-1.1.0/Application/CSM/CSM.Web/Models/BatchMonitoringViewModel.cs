using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CSM.Entity;
using CSM.Common.Utilities;
using System.ComponentModel.DataAnnotations;

namespace CSM.Web.Models
{
    public class BatchMonitoringViewModel
    {
        public string MonitorDateTime { get; set; }
        public int? IntervalTime { get; set; }
        [Display(Name = "Interval")]
        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public string IntervalTimeInput { get; set; }

        public IEnumerable<BatchProcessEntity> BatchProcessList { get; set; }
    }
}