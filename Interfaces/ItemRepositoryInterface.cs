using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CatalogApp.Entities;

namespace CatalogApp.Interfaces{
    public interface ItemRepositoryInterface{
        Task<Item> GetItemAsync(Guid Id);
        Task<IEnumerable<Item>> GetItemsAsync();
        Task CreateItemAsync(Item item);
        Task UpdateItemAsync(Item item);
        Task DeleteItemAsync(Guid Id);
    }
}