using GameStore.Api.Entities;

namespace GameStore.Api.Repositories;

public interface IGameRepository
{
    Task<IEnumerable<Game>> GetAllAsync();
    Task<IEnumerable<Game>> GetAllAsync(string? genre, decimal? minPrice, decimal? maxPrice, string? sortBy, bool descending, int page, int pageSize);
    Task<Game?> GetByIdAsync(string id);
    Task<Game> CreateAsync(Game game);
    Task<Game?> UpdateAsync(string id, Game game);
    Task<bool> DeleteAsync(string id);
    Task<long> CountAsync(string? genre, decimal? minPrice, decimal? maxPrice);
}
