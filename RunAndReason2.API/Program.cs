using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<RunAndReason2.API.Data.EFContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddScoped<RunAndReason2.API.Repositories.Interfaces.ILocationRepository, RunAndReason2.API.Repositories.Implementations.LocationRepository>();
builder.Services.AddScoped<RunAndReason2.API.Repositories.Interfaces.IVehicleRepository, RunAndReason2.API.Repositories.Implementations.VehicleRepository>();
builder.Services.AddScoped<RunAndReason2.API.Services.Interfaces.ILocationService, RunAndReason2.API.Services.Implementations.LocationService>();
builder.Services.AddScoped<RunAndReason2.API.Services.Interfaces.IVehicleService, RunAndReason2.API.Services.Implementations.VehicleService>();
builder.Services.Configure<RunAndReason2.API.Configuration.MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));
builder.Services.AddSingleton<RunAndReason2.API.Data.MongoDbContext>();
builder.Services.AddSingleton<RunAndReason2.API.Algorithms.AStar.IHeuristic, RunAndReason2.API.Algorithms.AStar.EuclideanHeuristic>();
builder.Services.AddSingleton<RunAndReason2.API.Algorithms.Dijkstra.DijkstraAlgorithm>();
builder.Services.AddSingleton<RunAndReason2.API.Algorithms.AStar.AStarAlgorithm>();
builder.Services.AddSingleton<RunAndReason2.API.Algorithms.Common.IOptimizationAlgorithm, RunAndReason2.API.Algorithms.Dijkstra.DijkstraAlgorithm>();
builder.Services.AddSingleton<RunAndReason2.API.Algorithms.Common.IOptimizationAlgorithm>(sp =>
    sp.GetRequiredService<RunAndReason2.API.Algorithms.AStar.AStarAlgorithm>());
builder.Services.AddSingleton<RunAndReason2.API.Algorithms.Common.IOptimizationAlgorithm, RunAndReason2.API.Algorithms.NearestNeighbor.NearestNeighborAlgorithm>();
builder.Services.AddSingleton<RunAndReason2.API.Algorithms.Common.AlgorithmFactory>();
builder.Services.AddSingleton<RunAndReason2.API.Algorithms.Common.IOptimizationAlgorithm, RunAndReason2.API.Algorithms.GeneticAlgorithm.GeneticAlgorithm>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
