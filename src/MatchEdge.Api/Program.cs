using MatchEdge.Application.Clients;
using MatchEdge.Application.Services;
using MatchEdge.Application.UseCases.Statistics;
using MatchEdge.Application.UseCases.Teams;
using MatchEdge.Infrastructure.Clients;
using MatchEdge.Infrastructure.Configuration;
using MatchEdge.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ISeasonService, SofaScoreSeasonService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISofaScoreClient, SofaScoreClient>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IHttpRequestExecutor, HttpRequestExecutor>();

builder.Services.Configure<SofaScoreOptions>(
    builder.Configuration.GetSection("SofaScore"));

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
