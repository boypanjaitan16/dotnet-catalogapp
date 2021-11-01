using CatalogApp.DTOs;
using CatalogApp.Entities;

namespace CatalogApp{
    public static class Extension{
        public static ItemDTO AsItemDTO(this Item item){
            return new ItemDTO {
                Id  = item.Id,
                Name    = item.Name,
                Price   = item.Price,
                CreatedAt   = item.CreatedAt
            };
        }
    }
}