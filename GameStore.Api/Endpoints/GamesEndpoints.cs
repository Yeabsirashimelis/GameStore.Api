using GameStore.Api.Dtos;
using GameStore.Api.Mapping;
using GameStore.Api.Repositories;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games")
            .WithTags("Games");

        // GET /games - Get all games with filtering, sorting, and pagination
        group.MapGet("/", (
            IGameRepository repository,
            string? genre = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortBy = null,
            bool descending = false,
            int page = 1,
            int pageSize = 10) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var games = repository.GetAll(genre, minPrice, maxPrice, sortBy, descending, page, pageSize);
            var totalCount = repository.Count(genre, minPrice, maxPrice);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var response = new PaginatedResponse<GameDto>(
                games.Select(g => g.ToDto()),
                page,
                pageSize,
                totalCount,
                totalPages
            );

            return Results.Ok(response);
        })
        .WithName("GetGames")
        .WithSummary("Get all games")
        .WithDescription("Retrieve all games with optional filtering, sorting, and pagination.");

        // GET /games/{id} - Get a single game by ID
        group.MapGet("/{id:int}", (int id, IGameRepository repository) =>
        {
            var game = repository.GetById(id);
            return game is null
                ? Results.NotFound(new { message = $"Game with ID {id} not found" })
                : Results.Ok(game.ToDto());
        })
        .WithName("GetGame")
        .WithSummary("Get a game by ID")
        .WithDescription("Retrieve a specific game by its unique identifier.");

        // POST /games - Create a new game
        group.MapPost("/", (CreateGameDto dto, IGameRepository repository) =>
        {
            var game = repository.Create(dto.ToEntity());
            return Results.CreatedAtRoute("GetGame", new { id = game.Id }, game.ToDto());
        })
        .WithName("CreateGame")
        .WithSummary("Create a new game")
        .WithDescription("Add a new game to the store.");

        // PUT /games/{id} - Update an existing game
        group.MapPut("/{id:int}", (int id, UpdateGameDto dto, IGameRepository repository) =>
        {
            var updatedGame = repository.Update(id, dto.ToEntity(id));
            return updatedGame is null
                ? Results.NotFound(new { message = $"Game with ID {id} not found" })
                : Results.NoContent();
        })
        .WithName("UpdateGame")
        .WithSummary("Update a game")
        .WithDescription("Update an existing game by its ID.");

        // DELETE /games/{id} - Delete a game
        group.MapDelete("/{id:int}", (int id, IGameRepository repository) =>
        {
            var deleted = repository.Delete(id);
            return deleted
                ? Results.NoContent()
                : Results.NotFound(new { message = $"Game with ID {id} not found" });
        })
        .WithName("DeleteGame")
        .WithSummary("Delete a game")
        .WithDescription("Remove a game from the store by its ID.");

        return group;
    }
}
