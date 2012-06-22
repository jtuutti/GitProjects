﻿using System.Collections.Generic;
using RestFoundation.Runtime;

namespace System.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateObjectAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(value, null, null);
          
            Validator.TryValidateObject(value, context, results, true);
 
            if (results.Count != 0)
            {
                var compositeResults = new CompositeValidationResult(String.Format("Validation for '{0}' failed", validationContext.DisplayName));
                results.ForEach(compositeResults.AddResult);
 
                return compositeResults;
            }
 
            return ValidationResult.Success;
        }
    }
}
