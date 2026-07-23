using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class PericiaManifestacaoConfiguration : IEntityTypeConfiguration<PericiaManifestacao>
{
    public void Configure(EntityTypeBuilder<PericiaManifestacao> builder)
    {
        builder.HasKey(e => new { e.PericiaAnomaliaId, e.ManifestacaoEspecificaId }).HasName("PK__Pericia___F733A3D702044810");

        builder.ToTable("Pericia_Manifestacao");

        builder.Property(e => e.Intensidade)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.HasOne(d => d.ManifestacaoEspecifica).WithMany(p => p.PericiaManifestacaos)
            .HasForeignKey(d => d.ManifestacaoEspecificaId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Pericia_M__Manif__656C112C");

        builder.HasOne(d => d.PericiaAnomalia).WithMany(p => p.PericiaManifestacaos)
            .HasForeignKey(d => d.PericiaAnomaliaId)
            .HasConstraintName("FK__Pericia_M__Peric__6477ECF3");
    }
}
