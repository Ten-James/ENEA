namespace Domain;

public record LoginResponse(
    string Token,
    DateTime Expiration
);