using CommonLibrary.Interfaces;
using Inventory.Data.Dtos;
using Inventory.Data.Entities;
using Inventory.Data.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[Route("api/items")]
[ApiController]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly IRepository<InventoryItem> _inventoryItemsRepository;
    private readonly IRepository<CatalogItem> _catalogItemsRepository;

    public ItemsController(IRepository<InventoryItem> inventoryItemsRepository, IRepository<CatalogItem> catalogItemsRepository)
    {
        _inventoryItemsRepository = inventoryItemsRepository ?? throw new ArgumentNullException(nameof(inventoryItemsRepository));

        _catalogItemsRepository = catalogItemsRepository ?? throw new ArgumentNullException(nameof(catalogItemsRepository));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<InventoryItemDto>>> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest();
        }

        var inventoryItemEntities = await _inventoryItemsRepository.GetAllAsync(item => item.UserId == userId);

        var itemIds = inventoryItemEntities.Select(item => item.CatalogItemId);

        var catalogItemEntities = await _catalogItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));

        var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
        {
            var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);

            return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
        });

        return Ok(inventoryItemDtos);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
    {
        var inventoryItem = await _inventoryItemsRepository.GetAsync(
                item => item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogItemId);

        if (inventoryItem is null)
        {
            inventoryItem = new InventoryItem
            {
                CatalogItemId = grantItemsDto.CatalogItemId,
                UserId = grantItemsDto.UserId,
                Quantity = grantItemsDto.Quantity,
                AcquiredDate = DateTimeOffset.UtcNow
            };

            await _inventoryItemsRepository.CreateAsync(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity += grantItemsDto.Quantity;
            await _inventoryItemsRepository.UpdateAsync(inventoryItem);
        }

        return Ok();
    }

}
