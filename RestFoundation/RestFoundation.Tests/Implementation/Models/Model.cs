using System.ComponentModel.DataAnnotations;

namespace RestFoundation.Tests.Implementation.Models
{
    public class Model
    {
        [Required]
        public int? Id { get; set; }

        [Required, StringLength(25, MinimumLength = 1)]
        public string Name { get; set; }

        public string[] Items { get; set; }
    }
}
