using Decco.Api.DataLayer;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.Operations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var corsPolicy = "DashboardOrigins";

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DeccoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DeccoDb")));

builder.Services.AddScoped<IAnomaliaRepository, AnomaliaRepository>();
builder.Services.AddScoped<IAnomaliaManager, AnomaliaManager>();

var app = builder.Build();

app.UseCors(corsPolicy);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
