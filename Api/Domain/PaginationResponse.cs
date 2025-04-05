namespace Domain;

public record PaginationResponse<T>(
    int TotalCount,
    int PageNumber,
    int PageSize,
    IEnumerable<T> Items
);