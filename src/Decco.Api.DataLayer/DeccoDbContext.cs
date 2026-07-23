using Decco.Api.DataLayer.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Decco.Api.DataLayer;

public partial class DeccoDbContext : DbContext
{
    public DeccoDbContext(DbContextOptions<DeccoDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeccoDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
