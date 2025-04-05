namespace Domain;

public record PaginationRequest(
    int PageNumber,
    int PageSize
);