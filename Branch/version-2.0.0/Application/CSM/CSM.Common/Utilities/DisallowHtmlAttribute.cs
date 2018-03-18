using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using CSM.Common.Resources;

namespace CSM.Common.Utilities
{
    /// <summary>
    ///     Validates if the input has any xml/html tags
    /// </summary>
    public sealed class DisallowHtmlAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var tagWithoutClosingRegex = new Regex(@"<[^!\s*>]+>");

            bool hasTags = tagWithoutClosingRegex.IsMatch(value.ToString());

            if (!hasTags)
                return ValidationResult.Success;

            //The field cannot contain html tags
            return new ValidationResult(Resource.ValErr_ContainHtmlTags);
        }
    }
}