using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestTestContracts.Resources
{
    public class Person : IValidatableObject
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Name cannot be shorter than 1 or longer than 255 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "No age provided")]
        [Range(16, 125, ErrorMessage = "Age must be between 16 and 125")]
        public int Age { get; set; }

        [Required(ErrorMessage = "No value collection provided")]
        public string[] Values { get; set; }

        /// <summary>
        /// Determines whether the specified object is valid.
        /// </summary>
        /// <returns>
        /// A collection that holds failed-validation information.
        /// </returns>
        /// <param name="validationContext">The validation context.</param>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name == "Joe Blo")
            {
                yield return new ValidationResult("Give me a real name", new[] { "Name" });
            }
        }
    }
}
