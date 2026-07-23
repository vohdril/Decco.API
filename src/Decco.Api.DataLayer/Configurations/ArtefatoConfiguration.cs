using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class ArtefatoConfiguration : IEntityTypeConfiguration<Artefato>
{
    public void Configure(EntityTypeBuilder<Artefato> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Artefato__3214EC078DC74F28");

        builder.ToTable("Artefato");

        builder.HasIndex(e => e.AnomaliaId, "IX_Artefato_AnomaliaId");

        builder.Property(e => e.Dimensoes)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.Identificacao).HasMaxLength(100);
        builder.Property(e => e.LocalOrigem).HasMaxLength(255);
        builder.Property(e => e.Material).HasMaxLength(255);
        builder.Property(e => e.PesoKg)
            .HasColumnType("decimal(10, 2)")
            .HasColumnName("Peso_Kg");
        builder.Property(e => e.PropriedadeSpin)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.HasOne(d => d.Anomalia).WithMany(p => p.Artefatos)
            .HasForeignKey(d => d.AnomaliaId)
            .HasConstraintName("FK__Artefato__Anomal__4F7CD00D");
    }
}
