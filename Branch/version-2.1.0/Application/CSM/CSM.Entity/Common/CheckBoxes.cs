using System;

namespace CSM.Entity.Common
{
    [Serializable]
    public class CheckBoxes
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }
    }
}
