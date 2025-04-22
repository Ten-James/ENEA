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
    private readonly IHttpContextAccessor _contextAccessor;

    public MyApiClient(string baseUrl, System.Net.Http.HttpClient httpClient, IHttpContextAccessor context): this(baseUrl, httpClient)
    {
        _contextAccessor = context;
    }

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
    {
        var token = _contextAccessor.HttpContext?.User?.FindFirst("JwtToken")?.Value;
        request.Headers.Add("Authorization", "Bearer " + token);
    }
}