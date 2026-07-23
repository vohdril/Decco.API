using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class CatManifestacaoEspecificaConfiguration : IEntityTypeConfiguration<CatManifestacaoEspecifica>
{
    public void Configure(EntityTypeBuilder<CatManifestacaoEspecifica> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Cat_Mani__3214EC07411DA892");

        builder.ToTable("Cat_ManifestacaoEspecifica");

        builder.HasIndex(e => e.Codigo, "UQ__Cat_Mani__06370DAC6716953A").IsUnique();

        builder.Property(e => e.Codigo)
            .HasMaxLength(20)
            .IsUnicode(false);
        builder.Property(e => e.Nome).HasMaxLength(100);
    }
}
