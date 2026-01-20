using GameStore.Api.Entities;
using GameStore.Api.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameStore.Api.Repositories;

public class MongoDbGameRepository : IGameRepository
{
    private readonly IMongoCollection<Game> _gamesCollection;
    private readonly ILogger<MongoDbGameRepository> _logger;

    public MongoDbGameRepository(IOptions<MongoDbSettings> settings, ILogger<MongoDbGameRepository> logger)
    {
        _logger = logger;
        
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _gamesCollection = database.GetCollection<Game>(settings.Value.GamesCollectionName);

        _logger.LogInformation("Connected to MongoDB database: {DatabaseName}", settings.Value.DatabaseName);
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await _gamesCollection.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<Game>> GetAllAsync(
        string? genre, 
        decimal? minPrice, 
        decimal? maxPrice, 
        string? sortBy, 
        bool descending, 
        int page, 
        int pageSize)
    {
        var filterBuilder = Builders<Game>.Filter;
        var filter = filterBuilder.Empty;

        // Apply filters
        if (!string.IsNullOrWhiteSpace(genre))
        {
            filter &= filterBuilder.Regex(g => g.Genre, new BsonRegularExpression(genre, "i"));
        }

        if (minPrice.HasValue)
        {
            filter &= filterBuilder.Gte(g => g.Price, minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            filter &= filterBuilder.Lte(g => g.Price, maxPrice.Value);
        }

        // Build sort definition
        var sortBuilder = Builders<Game>.Sort;
        SortDefinition<Game> sort = sortBy?.ToLowerInvariant() switch
        {
            "name" => descending ? sortBuilder.Descending(g => g.Name) : sortBuilder.Ascending(g => g.Name),
            "price" => descending ? sortBuilder.Descending(g => g.Price) : sortBuilder.Ascending(g => g.Price),
            "releasedate" => descending ? sortBuilder.Descending(g => g.ReleaseDate) : sortBuilder.Ascending(g => g.ReleaseDate),
            "genre" => descending ? sortBuilder.Descending(g => g.Genre) : sortBuilder.Ascending(g => g.Genre),
            _ => sortBuilder.Ascending(g => g.CreatedAt)
        };

        // Apply pagination
        return await _gamesCollection
            .Find(filter)
            .Sort(sort)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<long> CountAsync(string? genre, decimal? minPrice, decimal? maxPrice)
    {
        var filterBuilder = Builders<Game>.Filter;
        var filter = filterBuilder.Empty;

        if (!string.IsNullOrWhiteSpace(genre))
        {
            filter &= filterBuilder.Regex(g => g.Genre, new BsonRegularExpression(genre, "i"));
        }

        if (minPrice.HasValue)
        {
            filter &= filterBuilder.Gte(g => g.Price, minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            filter &= filterBuilder.Lte(g => g.Price, maxPrice.Value);
        }

        return await _gamesCollection.CountDocumentsAsync(filter);
    }

    public async Task<Game?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return null;
        }

        return await _gamesCollection.Find(g => g.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Game> CreateAsync(Game game)
    {
        game.CreatedAt = DateTime.UtcNow;
        game.UpdatedAt = DateTime.UtcNow;
        
        await _gamesCollection.InsertOneAsync(game);
        _logger.LogInformation("Created game with ID: {GameId}", game.Id);
        
        return game;
    }

    public async Task<Game?> UpdateAsync(string id, Game game)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return null;
        }

        game.Id = id;
        game.UpdatedAt = DateTime.UtcNow;

        var updateDefinition = Builders<Game>.Update
            .Set(g => g.Name, game.Name)
            .Set(g => g.Genre, game.Genre)
            .Set(g => g.Price, game.Price)
            .Set(g => g.ReleaseDate, game.ReleaseDate)
            .Set(g => g.UpdatedAt, game.UpdatedAt);

        var result = await _gamesCollection.UpdateOneAsync(
            g => g.Id == id,
            updateDefinition
        );

        if (result.MatchedCount == 0)
        {
            return null;
        }

        _logger.LogInformation("Updated game with ID: {GameId}", id);
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return false;
        }

        var result = await _gamesCollection.DeleteOneAsync(g => g.Id == id);
        
        if (result.DeletedCount > 0)
        {
            _logger.LogInformation("Deleted game with ID: {GameId}", id);
            return true;
        }

        return false;
    }
}
