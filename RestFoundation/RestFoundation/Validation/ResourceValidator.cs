﻿// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace RestFoundation.Validation
{
    /// <summary>
    /// Represents a default resource validator that is based on the <see cref="System.ComponentModel.DataAnnotations"/>
    /// namespace attributes.
    /// </summary>
    public class ResourceValidator : IResourceValidator
    {
        internal const string ValidationErrorKey = "REST_ValidationErrors";

        /// <summary>
        /// Returns a <see cref="bool"/> value indicating whether the resource is valid along with the associated
        /// collection of errors.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="errors">A read-only collection of the validation errors.</param>
        /// <returns>true if the resource is valid; otherwise, false.</returns>
        public virtual bool IsValid(object resource, out IReadOnlyCollection<ValidationError> errors)
        {
            if (resource == null || resource is DynamicObject || resource is JObject)
            {
                errors = new ValidationError[0];
                return true;
            }

            var context = new ValidationContext(resource, null, null);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(resource, context, results, true);

            errors = PopulateValidationErrors(results);
            return isValid;
        }

        private static List<ValidationError> PopulateValidationErrors(IEnumerable<ValidationResult> results)
        {
            var errorCollection = new List<ValidationError>();

            foreach (ValidationResult result in results)
            {
                if (result.MemberNames != null && result.MemberNames.Any())
                {
                    foreach (string memberName in result.MemberNames)
                    {
                        errorCollection.Add(new ValidationError(memberName, result.ErrorMessage));
                    }
                }
                else
                {
                    errorCollection.Add(new ValidationError(null, result.ErrorMessage));
                }
            }

            return errorCollection;
        }
    }
}
