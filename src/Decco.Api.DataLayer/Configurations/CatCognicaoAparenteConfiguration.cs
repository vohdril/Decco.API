using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class CatCognicaoAparenteConfiguration : IEntityTypeConfiguration<CatCognicaoAparente>
{
    public void Configure(EntityTypeBuilder<CatCognicaoAparente> builder)
    {
        builder.ToTable("Cat_CognicaoAparente");
        builder.HasIndex(e => e.Codigo).IsUnique();
        builder.Property(e => e.Codigo).HasMaxLength(5).IsUnicode(false);
        builder.Property(e => e.Nome).HasMaxLength(50).IsUnicode(false);
    }
}
