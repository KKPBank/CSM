using System;
using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;

namespace CSM.Common.Utilities
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class LocalizedRegexAttribute : RegularExpressionAttribute
    {
        private string _displayName;

        static LocalizedRegexAttribute()
        {
            // necessary to enable client side validation
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof (LocalizedRegexAttribute),
                typeof (RegularExpressionAttributeAdapter));
        }

        public LocalizedRegexAttribute(string _regularExpression, string _errorMessage)
            : base(_regularExpression)
        {
            ErrorMessageResourceName = _errorMessage;
        }

        protected override ValidationResult IsValid
            (object value, ValidationContext validationContext)
        {
            _displayName = validationContext.DisplayName;
            return base.IsValid(value, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ApplicationHelpers.GetMessage(ErrorMessageResourceName), _displayName);
        }
    }
}