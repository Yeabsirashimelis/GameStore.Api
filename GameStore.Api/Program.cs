using GameStore.Api.Dtos;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

List <GameDto> games = [
    new(1, "E-Football", "Football", 19.99m, new DateOnly(2015, 4, 5)),
    new(2, "FIFA 23", "Football", 59.99m, new DateOnly(2022, 9, 30)),
    new(3, "NBA 2K24", "Basketball", 69.99m, new DateOnly(2023, 9, 8)),
    new(4, "Call of Duty: Modern Warfare", "Shooter", 49.99m, new DateOnly(2019, 10, 25)),
    new(5, "Assassin’s Creed Valhalla", "Action RPG", 59.99m, new DateOnly(2020, 11, 10))

];

//GET /games
app.MapGet("/games", () =>games);

//GET  /games/1
app.MapGet("/games/{id}", (int id) =>
    games.Find(game =>  game.Id == id)
);

//POST /games
app.MapPost("/games", ()=>{});


app.Run();
