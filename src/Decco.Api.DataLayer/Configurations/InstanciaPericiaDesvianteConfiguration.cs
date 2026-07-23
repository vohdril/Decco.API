using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class InstanciaPericiaDesvianteConfiguration : IEntityTypeConfiguration<InstanciaPericiaDesviante>
{
    public void Configure(EntityTypeBuilder<InstanciaPericiaDesviante> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Instanci__3214EC07FCE1266A");

        builder.ToTable("Instancia_PericiaDesviante");

        builder.HasIndex(e => new { e.TipoInstancia, e.InstanciaId }, "IX_Instancia_PericiaDesviante_Instancia");

        builder.HasIndex(e => e.PericiaDesvianteId, "IX_Instancia_PericiaDesviante_Pericia");

        builder.Property(e => e.DataDescoberta).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.Intensidade)
            .HasMaxLength(20)
            .IsUnicode(false);
        builder.Property(e => e.TipoInstancia)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.HasOne(d => d.PericiaDesviante).WithMany(p => p.InstanciaPericiaDesviantes)
            .HasForeignKey(d => d.PericiaDesvianteId)
            .HasConstraintName("FK__Instancia__Peric__60A75C0F");
    }
}
