using System.ComponentModel.DataAnnotations;

namespace CSM.Common.Utilities
{
    public sealed class LocalizedStringLengthAttribute : StringLengthAttribute
    {
        private string _displayName;

        public LocalizedStringLengthAttribute(int maximumLength) : base(maximumLength)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            _displayName = validationContext.DisplayName;
            return base.IsValid(value, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, _displayName, MaximumLength);
        }
    }
}