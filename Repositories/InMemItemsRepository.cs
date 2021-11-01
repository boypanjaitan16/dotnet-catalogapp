using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogApp.Entities;
using CatalogApp.Interfaces;

namespace CatalogApp.Repositories {
    public class InMemItemsRepository:ItemRepositoryInterface{
        private readonly List<Item> items = new(){
            new Item{ Id= Guid.NewGuid(), Name= "Positon", Price = 9, CreatedAt= DateTimeOffset.UtcNow},
            new Item{ Id= Guid.NewGuid(), Name= "Flying Scicors", Price = 16, CreatedAt= DateTimeOffset.UtcNow},
            new Item{ Id= Guid.NewGuid(), Name= "Pantex", Price = 6, CreatedAt= DateTimeOffset.UtcNow},
            new Item{ Id= Guid.NewGuid(), Name= "Good Chair", Price = 41, CreatedAt= DateTimeOffset.UtcNow},
        };

        public async Task<IEnumerable<Item>> GetItemsAsync(){
            return await Task.FromResult(this.items);
        }

        public async Task<Item> GetItemAsync(Guid Id){
            return await Task.FromResult(this.items.Where(item => item.Id == Id).SingleOrDefault());
        }

        public async Task CreateItemAsync(Item item)
        {
            this.items.Add(item);

            await Task.CompletedTask;
        }

        public async Task UpdateItemAsync(Item item)
        {
            var index   = this.items.FindIndex(them => them.Id == item.Id);
            this.items[index]   = item;

            await Task.CompletedTask;
        }

        public async Task DeleteItemAsync(Guid Id)
        {
            var index   = this.items.FindIndex(them => them.Id == Id);
            this.items.RemoveAt(index);

            await Task.CompletedTask;
        }
    }
}