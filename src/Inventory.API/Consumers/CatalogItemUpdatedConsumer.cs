using Catalog.Contracts;
using CommonLibrary.Interfaces;
using Inventory.Data.Entities;
using MassTransit;

namespace Inventory.API.Consumers;

public class CatalogItemUpdatedConsumer : IConsumer<CatalogItemUpdated>
{
    private readonly IRepository<CatalogItem> _catalogItemrepository;

    public CatalogItemUpdatedConsumer(IRepository<CatalogItem> catalogItemrepository)
    {
        _catalogItemrepository = catalogItemrepository ?? throw new ArgumentNullException(nameof(catalogItemrepository));
    }

    public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
    {
        var message = context.Message;

        var item = await _catalogItemrepository.GetAsync(message.ItemId);

        if (item is null)
        {
            item = new CatalogItem
            {
                Id = message.ItemId,
                Name = message.Name,
                Description = message.Description
            };

            await _catalogItemrepository.CreateAsync(item);
        }
        else
        {
            item.Name = message.Name;
            item.Description = message.Description;

            await _catalogItemrepository.UpdateAsync(item);
        }
    }
}