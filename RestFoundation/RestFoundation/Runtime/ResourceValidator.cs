using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RestFoundation.Runtime
{
    public class ResourceValidator : IResourceValidator
    {
        public bool IsValid(object resource, out ICollection<ValidationError> errors)
        {
            if (resource == null)
            {
                errors = new ReadOnlyCollection<ValidationError>(new ValidationError[0]);
                return true;
            }

            var context = new ValidationContext(resource, null, null);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(resource, context, results, true);
            List<ValidationError> errorCollection = PopulateValidationErrors(results);

            errors = new ReadOnlyCollection<ValidationError>(errorCollection);
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
