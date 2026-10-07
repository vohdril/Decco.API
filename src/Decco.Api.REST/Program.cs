using Decco.Api.DataLayer;
using Decco.Api.Root;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

/* PID handshake: writes the PID so the pre-build step can kill the previous process */
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

/* static files: MF2 remote (decco-demo) */
var mfRemotePath = Path.Combine(app.Environment.ContentRootPath, "..", "..", "mf-remote", "dist");
if (Directory.Exists(mfRemotePath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(mfRemotePath),
        RequestPath = "/mf-remote",
    });
}

app.Run();

/* Removes the PID file on shutdown */
try { if (File.Exists(pidFile)) File.Delete(pidFile); } catch { }
