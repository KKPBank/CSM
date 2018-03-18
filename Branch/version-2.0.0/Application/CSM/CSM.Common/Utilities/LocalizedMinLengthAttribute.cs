using System.ComponentModel.DataAnnotations;

namespace CSM.Common.Utilities
{
    public sealed class LocalizedMinLengthAttribute : MinLengthAttribute
    {
        private string _displayName;

        public LocalizedMinLengthAttribute(int length) : base(length)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            _displayName = validationContext.DisplayName;
            return base.IsValid(value, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, Length, _displayName);
        }
    }
}
