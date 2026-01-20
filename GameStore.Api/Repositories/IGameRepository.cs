using GameStore.Api.Entities;

namespace GameStore.Api.Repositories;

public interface IGameRepository
{
    IEnumerable<Game> GetAll();
    IEnumerable<Game> GetAll(string? genre, decimal? minPrice, decimal? maxPrice, string? sortBy, bool descending, int page, int pageSize);
    Game? GetById(int id);
    Game Create(Game game);
    Game? Update(int id, Game game);
    bool Delete(int id);
    int Count(string? genre, decimal? minPrice, decimal? maxPrice);
}
