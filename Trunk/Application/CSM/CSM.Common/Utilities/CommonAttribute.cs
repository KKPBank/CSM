using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Common.Utilities
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExportAttribute : Attribute
    {
        string _displayName { get; set; }
        public string DisplayName { get { return _displayName; } }
        public bool IsNumberAsString { get; set; }

        public ExportAttribute(string displayName)
        {
            _displayName = displayName;
        }
    }
}
