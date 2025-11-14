namespace MilkingYield.API.Clients;

public sealed class CattleApiClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;
    public async Task<bool> CattleExistsAsync(Guid cattleId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/api/cattle/{cattleId}", cancellationToken);
        return response.IsSuccessStatusCode;
    }
    public async Task<Cattle?> GetCowByIdAsync(Guid cattleId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/api/cattle/{cattleId}", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var cattle = await response.Content.ReadFromJsonAsync<Cattle>(cancellationToken: cancellationToken);
            return cattle;
        }
        return null;
    }
}
