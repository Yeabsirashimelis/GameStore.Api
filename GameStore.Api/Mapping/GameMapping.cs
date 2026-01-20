using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping;

public static class GameMapping
{
    public static GameDto ToDto(this Game game)
    {
        return new GameDto(
            game.Id!,
            game.Name,
            game.Genre,
            game.Price,
            game.ReleaseDate,
            game.CreatedAt,
            game.UpdatedAt
        );
    }

    public static Game ToEntity(this CreateGameDto dto)
    {
        return new Game
        {
            Name = dto.Name,
            Genre = dto.Genre,
            Price = dto.Price,
            ReleaseDate = dto.ReleaseDate
        };
    }

    public static Game ToEntity(this UpdateGameDto dto, string id)
    {
        return new Game
        {
            Id = id,
            Name = dto.Name,
            Genre = dto.Genre,
            Price = dto.Price,
            ReleaseDate = dto.ReleaseDate
        };
    }
}
