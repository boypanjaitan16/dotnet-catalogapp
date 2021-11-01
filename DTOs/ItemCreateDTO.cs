using System.ComponentModel.DataAnnotations;

namespace CatalogApp.DTOs{
    public record ItemCreateDTO{
        [Required]
        public string Name {get;init;}
        [Required]
        [Range(1,1000)]
        public decimal Price {get;init;}
    }
}