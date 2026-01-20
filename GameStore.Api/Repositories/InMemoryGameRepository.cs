using GameStore.Api.Entities;

namespace GameStore.Api.Repositories;

public class InMemoryGameRepository : IGameRepository
{
    private readonly List<Game> _games = [
        new() { Id = 1, Name = "E-Football", Genre = "Football", Price = 19.99m, ReleaseDate = new DateOnly(2015, 4, 5) },
        new() { Id = 2, Name = "FIFA 23", Genre = "Football", Price = 59.99m, ReleaseDate = new DateOnly(2022, 9, 30) },
        new() { Id = 3, Name = "NBA 2K24", Genre = "Basketball", Price = 69.99m, ReleaseDate = new DateOnly(2023, 9, 8) },
        new() { Id = 4, Name = "Call of Duty: Modern Warfare", Genre = "Shooter", Price = 49.99m, ReleaseDate = new DateOnly(2019, 10, 25) },
        new() { Id = 5, Name = "Assassin's Creed Valhalla", Genre = "Action RPG", Price = 59.99m, ReleaseDate = new DateOnly(2020, 11, 10) }
    ];

    private int _nextId = 6;

    public IEnumerable<Game> GetAll() => _games;

    public IEnumerable<Game> GetAll(string? genre, decimal? minPrice, decimal? maxPrice, string? sortBy, bool descending, int page, int pageSize)
    {
        var query = _games.AsEnumerable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(genre))
            query = query.Where(g => g.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));
        
        if (minPrice.HasValue)
            query = query.Where(g => g.Price >= minPrice.Value);
        
        if (maxPrice.HasValue)
            query = query.Where(g => g.Price <= maxPrice.Value);

        // Sorting
        query = sortBy?.ToLowerInvariant() switch
        {
            "name" => descending ? query.OrderByDescending(g => g.Name) : query.OrderBy(g => g.Name),
            "price" => descending ? query.OrderByDescending(g => g.Price) : query.OrderBy(g => g.Price),
            "releasedate" => descending ? query.OrderByDescending(g => g.ReleaseDate) : query.OrderBy(g => g.ReleaseDate),
            "genre" => descending ? query.OrderByDescending(g => g.Genre) : query.OrderBy(g => g.Genre),
            _ => query.OrderBy(g => g.Id)
        };

        // Pagination
        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }

    public int Count(string? genre, decimal? minPrice, decimal? maxPrice)
    {
        var query = _games.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(genre))
            query = query.Where(g => g.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));
        
        if (minPrice.HasValue)
            query = query.Where(g => g.Price >= minPrice.Value);
        
        if (maxPrice.HasValue)
            query = query.Where(g => g.Price <= maxPrice.Value);

        return query.Count();
    }

    public Game? GetById(int id) => _games.Find(g => g.Id == id);

    public Game Create(Game game)
    {
        game.Id = _nextId++;
        _games.Add(game);
        return game;
    }

    public Game? Update(int id, Game game)
    {
        var index = _games.FindIndex(g => g.Id == id);
        if (index == -1) return null;

        game.Id = id;
        _games[index] = game;
        return game;
    }

    public bool Delete(int id)
    {
        var index = _games.FindIndex(g => g.Id == id);
        if (index == -1) return false;

        _games.RemoveAt(index);
        return true;
    }
}
