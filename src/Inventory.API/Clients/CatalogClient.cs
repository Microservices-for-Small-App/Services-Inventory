using Inventory.Data.Dtos;

namespace Inventory.API.Clients;

public class CatalogClient
{
    private readonly HttpClient _httpClient;

    public CatalogClient(HttpClient httpClient)
    {
        this._httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<IReadOnlyCollection<CatalogItemDto>?> GetCatalogItemsAsync()
    {
        var items = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/items");

        return items;
    }
}
