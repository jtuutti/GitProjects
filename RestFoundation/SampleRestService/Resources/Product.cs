using System;
using System.ComponentModel.DataAnnotations;

namespace SampleRestService.Resources
{
    public class Product
    {
        public Product()
        {
            InStock = true;
        }

        [Required(ErrorMessage = "Product ID is required")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Product ID be a non-negative number")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Product name must be provided")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Product name must be between 1 and 255 characters")]
        public string Name { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "Product ID must be a non-negative number")]
        public decimal Price { get; set; }

        public bool InStock { get; set; }
        public DateTime? Added { get; set; }


        // a conditional serialization pattern supported by JSON and XML formatters
        public bool ShouldSerializeID()
        {
            return ID > 0;
        }

        public bool ShouldSerializeAdded()
        {
            return Added.HasValue;
        }
    }
}