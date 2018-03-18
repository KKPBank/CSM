using System;

namespace CSM.Common.Utilities
{
    [Serializable]
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