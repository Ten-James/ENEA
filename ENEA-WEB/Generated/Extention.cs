using System.Net.Http.Headers;

namespace Generated.Client;

public interface IPaginationResponse<T>
{
    int TotalCount { get; }
    int PageNumber { get; }
    int PageSize { get; }
    IEnumerable<T> Items { get; }
}

public class ChargerGroupReadDtoPaginationResponseWrapper(
    ChargerGroupReadDtoPaginationResponse response) : IPaginationResponse<ChargerGroupReadDto>
{
    public int TotalCount => response.TotalCount;
    public int PageNumber => response.PageNumber;
    public int PageSize => response.PageSize;
    public IEnumerable<ChargerGroupReadDto> Items => response.Items;
}

public class UserReadDtoPaginationResponseWrapper(
    UserReadDtoPaginationResponse response) : IPaginationResponse<UserReadDto>
{
    public int TotalCount => response.TotalCount;
    public int PageNumber => response.PageNumber;
    public int PageSize => response.PageSize;
    public IEnumerable<UserReadDto> Items => response.Items;
}

public partial class MyApiClient
{
    public void SetBearerToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}