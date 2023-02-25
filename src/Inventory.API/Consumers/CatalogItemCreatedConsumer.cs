using Catalog.Contracts;
using CommonLibrary.Interfaces;
using Inventory.Data.Entities;
using MassTransit;

namespace Inventory.API.Consumers;

public class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated>
{
    private readonly IRepository<CatalogItem> _catalogItemrepository;

    public CatalogItemCreatedConsumer(IRepository<CatalogItem> catalogItemrepository)
    {
        _catalogItemrepository = catalogItemrepository ?? throw new ArgumentNullException(nameof(catalogItemrepository));
    }

    public async Task Consume(ConsumeContext<CatalogItemCreated> context)
    {
        var message = context.Message;

        var item = await _catalogItemrepository.GetAsync(message.ItemId);

        if (item is not null)
        {
            return;
        }

        item = new CatalogItem
        {
            Id = message.ItemId,
            Name = message.Name,
            Description = message.Description
        };

        await _catalogItemrepository.CreateAsync(item);
    }
}