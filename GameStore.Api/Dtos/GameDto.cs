namespace GameStore.Api.Dtos;

public record GameDto(
    string Id,
    string Name,
    string Genre,
    decimal Price,
    DateTime ReleaseDate,
    DateTime CreatedAt,
    DateTime UpdatedAt
);