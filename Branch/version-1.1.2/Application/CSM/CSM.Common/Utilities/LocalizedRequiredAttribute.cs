using System.ComponentModel.DataAnnotations;

namespace CSM.Common.Utilities
{
    public sealed class LocalizedRequiredAttribute : RequiredAttribute
    {
        private string _displayName;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            _displayName = validationContext.DisplayName;
            return base.IsValid(value, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ApplicationHelpers.GetMessage(ErrorMessageString), _displayName);
        }
    }
}