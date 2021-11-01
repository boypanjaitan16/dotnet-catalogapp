using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CatalogApp.Entities;
using CatalogApp.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CatalogApp.Repositories{
    public class MongoDBItemsRepository : ItemRepositoryInterface
    {
        private const string databaseName   = "catalog";
        private const string collectionName = "items";
        private readonly IMongoCollection<Item> itemsCollection;
        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

        public MongoDBItemsRepository(IMongoClient mongoClient){
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            itemsCollection = database.GetCollection<Item>(collectionName);
        }

        public async Task CreateItemAsync(Item item)
        {
            await this.itemsCollection.InsertOneAsync(item);
        }

        public async Task DeleteItemAsync(Guid Id)
        {
            var filter  = filterBuilder.Eq(all => all.Id, Id);

            await this.itemsCollection.DeleteOneAsync(filter);
        }

        public async Task<Item> GetItemAsync(Guid Id)
        {
            var filter = filterBuilder.Eq(item => item.Id, Id);
            return await this.itemsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await this.itemsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpdateItemAsync(Item item)
        {
            var filter  = filterBuilder.Eq(all => all.Id, item.Id);

            await this.itemsCollection.ReplaceOneAsync(filter, item);
        }
    }
}