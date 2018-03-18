using System;

namespace CSM.Entity
{
    [Serializable]
    public class ParameterEntity
    {
        public int? ParamId { get; set; }
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
        public string ParamDesc { get; set; }
        public string ParamType { get; set; }
    }
}
