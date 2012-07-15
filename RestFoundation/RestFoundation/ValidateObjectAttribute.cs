using System.Collections.Generic;
using System.Globalization;
using RestFoundation.Runtime;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Marks an inner property of a object to be validated as a part of the object
    /// validation. The property type must be a class or a struct that can be validated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateObjectAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult"/> class. 
        /// </returns>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null) throw new ArgumentNullException("validationContext");

            var results = new List<ValidationResult>();
            var context = new ValidationContext(value, null, null);
          
            Validator.TryValidateObject(value, context, results, true);
 
            if (results.Count != 0)
            {
                var compositeResults = new CompositeValidationResult(String.Format(CultureInfo.InvariantCulture, "Validation for '{0}' failed", validationContext.DisplayName));
                results.ForEach(compositeResults.AddResult);
 
                return compositeResults;
            }
 
            return ValidationResult.Success;
        }
    }
}
