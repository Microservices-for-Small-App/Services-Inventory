using CommonLibrary.Interfaces;
using Inventory.Contracts;
using Inventory.Data.Entities;
using MassTransit;

namespace Inventory.API.Consumers;

public class GrantItemsConsumer : IConsumer<GrantItems>
{
    private readonly IRepository<InventoryItem> _inventoryItemsRepository;
    private readonly IRepository<CatalogItem> _catalogItemsRepository;

    public GrantItemsConsumer(IRepository<InventoryItem> inventoryItemsRepository, IRepository<CatalogItem> catalogItemsRepository)
    {
        _inventoryItemsRepository = inventoryItemsRepository ?? throw new ArgumentNullException(nameof(inventoryItemsRepository));

        _catalogItemsRepository = catalogItemsRepository ?? throw new ArgumentNullException(nameof(catalogItemsRepository));
    }

    public async Task Consume(ConsumeContext<GrantItems> context)
    {
        var message = context.Message;

        var item = await _catalogItemsRepository.GetAsync(message.CatalogItemId);

        if (item == null)
        {
            throw new UnknownItemException(message.CatalogItemId);
        }

        var inventoryItem = await _inventoryItemsRepository.GetAsync(
            item => item.UserId == message.UserId && item.CatalogItemId == message.CatalogItemId);

        if (inventoryItem == null)
        {
            inventoryItem = new InventoryItem
            {
                CatalogItemId = message.CatalogItemId,
                UserId = message.UserId,
                Quantity = message.Quantity,
                AcquiredDate = DateTimeOffset.UtcNow
            };

            await _inventoryItemsRepository.CreateAsync(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity += message.Quantity;
            await _inventoryItemsRepository.UpdateAsync(inventoryItem);
        }

        await context.Publish(new InventoryItemsGranted(message.CorrelationId));
    }
}