using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class EntidadeVivaConfiguration : IEntityTypeConfiguration<EntidadeViva>
{
    public void Configure(EntityTypeBuilder<EntidadeViva> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Entidade__3214EC07F947E505");

        builder.ToTable("EntidadeViva");

        builder.HasIndex(e => e.AnomaliaId, "IX_EntidadeViva_AnomaliaId");

        builder.Property(e => e.Biologia).HasMaxLength(255);
        builder.Property(e => e.Dieta).HasMaxLength(100);
        builder.Property(e => e.Especie).HasMaxLength(150);
        builder.Property(e => e.Identificacao).HasMaxLength(100);
        builder.Property(e => e.IsConsciente).HasDefaultValue(true);
        builder.Property(e => e.OrigemPoder).HasMaxLength(100);

        builder.HasOne(d => d.Anomalia).WithMany(p => p.EntidadeVivas)
            .HasForeignKey(d => d.AnomaliaId)
            .HasConstraintName("FK__EntidadeV__Anoma__4BAC3F29");
    }
}
