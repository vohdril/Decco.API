using Decco.Api.DataLayer;
using Decco.Api.Root;
using Microsoft.EntityFrameworkCore;

/* PID handshake: grava o PID para o pré-build matar o processo anterior */
var pidFile = Path.Combine(Path.GetTempPath(), ".decco-api-rest.pid");
try { File.WriteAllText(pidFile, Environment.ProcessId.ToString()); } catch { }

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

builder.Services.RegisterDependencies();

var app = builder.Build();

app.UseCors(corsPolicy);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

/* Limpa o PID ao encerrar */
try { if (File.Exists(pidFile)) File.Delete(pidFile); } catch { }
