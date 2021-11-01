using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogApp.DTOs;
using CatalogApp.Entities;
using CatalogApp.Interfaces;
using CatalogApp.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CatalogApp.Controllers{
    [ApiController]
    [Route("[controller]")]
    public class ItemController:ControllerBase{
        private readonly ItemRepositoryInterface repository;

        public ItemController(ItemRepositoryInterface itemRepository){
            this.repository = itemRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDTO>> GetItemsAsync(){
            return (await repository.GetItemsAsync()).Select(item => item.AsItemDTO());
        }

        [HttpGet("{Id}")]
        public async  Task<ActionResult<ItemDTO>> GetItemAsync(Guid Id){
            var item = await repository.GetItemAsync(Id);

            if(item is null){
                return NotFound();
            }

            return Ok(item.AsItemDTO());
        }

        [HttpPost]
        public async  Task<ActionResult<ItemDTO>> CreateItemAsync(ItemCreateDTO itemDTO){
            Item item = new(){
                Id  = Guid.NewGuid(),
                Name = itemDTO.Name,
                Price = itemDTO.Price,
                CreatedAt   = DateTimeOffset.UtcNow
            };

            await repository.CreateItemAsync(item);

            return CreatedAtAction(nameof(GetItemAsync), new {id = item.Id}, item.AsItemDTO());
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid Id, ItemUpdateDTO itemDTO){
            var exists  = await repository.GetItemAsync(Id);

            if(exists is null){
                return NotFound();
            }

            Item updatedItem = exists with {
                Name    = itemDTO.Name,
                Price   = itemDTO.Price
            };

            await repository.UpdateItemAsync(updatedItem);

            return NoContent();
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteItemAsync(Guid Id){
            var exists  = await repository.GetItemAsync(Id);

            if(exists is null){
                return NotFound();
            }
            
            await repository.DeleteItemAsync(Id);

            return NoContent();
        }
    }
}