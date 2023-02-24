using Catalog.Contracts;
using CommonLibrary.Interfaces;
using Inventory.Data.Entities;
using MassTransit;

namespace Inventory.API.Consumers;

public class CatalogItemDeletedConsumer : IConsumer<CatalogItemDeleted>
{
    private readonly IRepository<CatalogItem> _catalogItemrepository;

    public CatalogItemDeletedConsumer(IRepository<CatalogItem> catalogItemrepository)
    {
        _catalogItemrepository = catalogItemrepository;
    }

    public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
    {
        var message = context.Message;

        var item = await _catalogItemrepository.GetAsync(message.ItemId);

        if (item == null)
        {
            return;
        }

        await _catalogItemrepository.RemoveAsync(message.ItemId);
    }
}