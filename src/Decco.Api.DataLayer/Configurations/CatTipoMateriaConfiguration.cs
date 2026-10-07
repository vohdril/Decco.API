using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class CatTipoMateriaConfiguration : IEntityTypeConfiguration<CatTipoMateria>
{
    public void Configure(EntityTypeBuilder<CatTipoMateria> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Cat_Tipo__3214EC07CDADACA6");

        builder.ToTable("Cat_TipoMateria");

        builder.HasIndex(e => e.Nome, "UQ__Cat_Tipo__7D8FE3B2C88FCF2D").IsUnique();

        builder.Property(e => e.Nome)
            .HasMaxLength(50)
            .IsUnicode(false);
    }
}
