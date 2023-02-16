namespace Inventory.Data.Dtos;

public record GrantItemsDto(Guid UserId, Guid CatalogItemId, int Quantity);