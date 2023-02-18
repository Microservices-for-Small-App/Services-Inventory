using Inventory.Data.Dtos;
using Inventory.Data.Entities;

namespace Inventory.Data.Extensions;

public static class InventoryItemExtensions
{
    public static InventoryItemDto AsDto(this InventoryItem item, string name, string description)
    {
        return new InventoryItemDto(item.CatalogItemId, name, description, item.Quantity, item.AcquiredDate);
    }
}
