using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class PericiaAnomaliumConfiguration : IEntityTypeConfiguration<PericiaAnomalium>
{
    public void Configure(EntityTypeBuilder<PericiaAnomalium> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__PericiaA__3214EC073B1D400F");

        builder.HasIndex(e => new { e.AnomaliaId, e.Nome }, "UQ_Pericia_Anomalia_Nome").IsUnique();

        builder.Property(e => e.Custo).HasMaxLength(100);
        builder.Property(e => e.Nivel).HasDefaultValue(1);
        builder.Property(e => e.Nome).HasMaxLength(100);

        builder.HasOne(d => d.Anomalia).WithMany(p => p.PericiaAnomalia)
            .HasForeignKey(d => d.AnomaliaId)
            .HasConstraintName("FK__PericiaAn__Anoma__59FA5E80");

        builder.HasOne(d => d.MecanismoPrimario).WithMany(p => p.PericiaAnomaliumMecanismoPrimarios)
            .HasForeignKey(d => d.MecanismoPrimarioId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__PericiaAn__Mecan__5AEE82B9");

        builder.HasOne(d => d.MecanismoSecundario).WithMany(p => p.PericiaAnomaliumMecanismoSecundarios)
            .HasForeignKey(d => d.MecanismoSecundarioId)
            .HasConstraintName("FK__PericiaAn__Mecan__5BE2A6F2");
    }
}
