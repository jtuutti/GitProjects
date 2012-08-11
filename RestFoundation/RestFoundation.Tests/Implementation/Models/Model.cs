using System.ComponentModel.DataAnnotations;

namespace RestFoundation.Tests.Implementation.Models
{
    public class Model
    {
        [Required]
        public int? ID { get; set; }

        [Required, StringLength(25, MinimumLength = 1)]
        public string Name { get; set; }

        public string[] Items { get; set; }
    }
}
