using CommonLibrary.Interfaces;
using Inventory.API.Exceptions;
using Inventory.Contracts;
using Inventory.Data.Entities;
using MassTransit;

namespace Inventory.API.Consumers;

public class SubtractItemsConsumer : IConsumer<SubtractItems>
{
    private readonly IRepository<InventoryItem> _inventoryItemsRepository;
    private readonly IRepository<CatalogItem> _catalogItemsRepository;

    public SubtractItemsConsumer(IRepository<InventoryItem> inventoryItemsRepository, IRepository<CatalogItem> catalogItemsRepository)
    {
        _inventoryItemsRepository = inventoryItemsRepository ?? throw new ArgumentNullException(nameof(inventoryItemsRepository));

        _catalogItemsRepository = catalogItemsRepository ?? throw new ArgumentNullException(nameof(catalogItemsRepository));
    }

    public async Task Consume(ConsumeContext<SubtractItems> context)
    {
        var message = context.Message;

        var item = await _catalogItemsRepository.GetAsync(message.CatalogItemId);

        if (item is null)
        {
            throw new UnknownItemException(message.CatalogItemId);
        }

        var inventoryItem = await _inventoryItemsRepository.GetAsync(
            item => item.UserId == message.UserId && item.CatalogItemId == message.CatalogItemId);

        if (inventoryItem is not null)
        {
            if (inventoryItem.MessageIds.Contains(context.MessageId!.Value))
            {
                await context.Publish(new InventoryItemsSubtracted(message.CorrelationId));

                return;
            }

            inventoryItem.Quantity -= message.Quantity;

            inventoryItem.MessageIds.Add(context.MessageId.Value);

            await _inventoryItemsRepository.UpdateAsync(inventoryItem);

            await context.Publish(new InventoryItemUpdated(inventoryItem.UserId, inventoryItem.CatalogItemId, inventoryItem.Quantity));
        }

        await context.Publish(new InventoryItemsSubtracted(message.CorrelationId));
    }
}
