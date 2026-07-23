using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class LocalidadeConfiguration : IEntityTypeConfiguration<Localidade>
{
    public void Configure(EntityTypeBuilder<Localidade> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Localida__3214EC0721A940BF");

        builder.ToTable("Localidade");

        builder.HasIndex(e => e.AnomaliaId, "IX_Localidade_AnomaliaId");

        builder.Property(e => e.ClimaAnomalo)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.IeiaDAmbiente)
            .HasColumnType("decimal(8, 4)")
            .HasColumnName("IEIA_D_Ambiente");
        builder.Property(e => e.IsGeograficamenteLimitada).HasDefaultValue(true);
        builder.Property(e => e.Nome).HasMaxLength(255);
        builder.Property(e => e.TipoTerreno)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.HasOne(d => d.Anomalia).WithMany(p => p.Localidades)
            .HasForeignKey(d => d.AnomaliaId)
            .HasConstraintName("FK__Localidad__Anoma__52593CB8");
    }
}
