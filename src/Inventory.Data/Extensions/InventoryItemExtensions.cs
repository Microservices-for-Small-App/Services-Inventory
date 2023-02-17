using Inventory.Data.Dtos;
using Inventory.Data.Entities;

namespace Inventory.Data.Extensions;

public static class InventoryItemExtensions
{
    public static InventoryItemDto AsDto(this InventoryItem item)
    {
        return new InventoryItemDto(item.CatalogItemId, item.Quantity, item.AcquiredDate);
    }
}
