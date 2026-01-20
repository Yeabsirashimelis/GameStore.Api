using GameStore.Api.Dtos;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

List<GameDto> games = [
    new(1, "E-Football", "Football", 19.99m, new DateOnly(2015, 4, 5)),
    new(2, "FIFA 23", "Football", 59.99m, new DateOnly(2022, 9, 30)),
    new(3, "NBA 2K24", "Basketball", 69.99m, new DateOnly(2023, 9, 8)),
    new(4, "Call of Duty: Modern Warfare", "Shooter", 49.99m, new DateOnly(2019, 10, 25)),
    new(5, "Assassin's Creed Valhalla", "Action RPG", 59.99m, new DateOnly(2020, 11, 10))
];

// Track next ID for proper ID generation
int nextId = games.Max(g => g.Id) + 1;

// GET /games
app.MapGet("/games", () => games);

// GET /games/{id}
app.MapGet("/games/{id}", (int id) =>
{
    GameDto? game = games.Find(game => game.Id == id);
    return game is null ? Results.NotFound(new { message = $"Game with ID {id} not found" }) : Results.Ok(game);
}).WithName("GetGame");

// POST /games
app.MapPost("/games", (CreateGameDto newGame) =>
{
    GameDto game = new(
        nextId++,
        newGame.Name,
        newGame.Genre,
        newGame.Price,
        newGame.ReleaseDate
    );

    games.Add(game);
    return Results.CreatedAtRoute("GetGame", new { id = game.Id }, game);
});

// PUT /games/{id}
app.MapPut("/games/{id}", (int id, UpdateGameDto updatedGame) =>
{
    var index = games.FindIndex(game => game.Id == id);
    
    if (index == -1)
    {
        return Results.NotFound(new { message = $"Game with ID {id} not found" });
    }

    games[index] = new GameDto(
        id,
        updatedGame.Name,
        updatedGame.Genre,
        updatedGame.Price,
        updatedGame.ReleaseDate
    );

    return Results.NoContent();
});

// DELETE /games/{id}
app.MapDelete("/games/{id}", (int id) =>
{
    var index = games.FindIndex(game => game.Id == id);
    
    if (index == -1)
    {
        return Results.NotFound(new { message = $"Game with ID {id} not found" });
    }

    games.RemoveAt(index);
    return Results.NoContent();
});

app.Run();
