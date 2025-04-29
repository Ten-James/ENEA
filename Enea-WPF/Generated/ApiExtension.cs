using System.Net.Http.Headers;

namespace Generated.Client;

public partial class MyApiClient
{
    public void SetToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}