using GameStore.Api.Endpoints;
using GameStore.Api.Middleware;
using GameStore.Api.Repositories;
using GameStore.Api.Settings;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Register repositories
builder.Services.AddSingleton<IGameRepository, MongoDbGameRepository>();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() 
    { 
        Title = "GameStore API", 
        Version = "v1",
        Description = "A modern API for managing video games in a game store with MongoDB Atlas",
        Contact = new() { Name = "GameStore Team" }
    });
});

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure middleware
app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GameStore API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

// Map endpoints
app.MapGamesEndpoints();
app.MapHealthChecks("/health");

app.Run();
