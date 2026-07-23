using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class IncidenteConfiguration : IEntityTypeConfiguration<Incidente>
{
    public void Configure(EntityTypeBuilder<Incidente> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Incident__3214EC077D3331FB");

        builder.ToTable("Incidente");

        builder.HasIndex(e => e.AnomaliaId, "IX_Incidente_Anomalia");

        builder.Property(e => e.DanoMaterial).HasMaxLength(255);
        builder.Property(e => e.DataHora)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.Feridos).HasDefaultValue(0);
        builder.Property(e => e.IsEventoSigma).HasDefaultValue(false);
        builder.Property(e => e.Mortes).HasDefaultValue(0);
        builder.Property(e => e.NivelSeguranca)
            .HasMaxLength(20)
            .IsUnicode(false);
        builder.Property(e => e.Tipo)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.Titulo).HasMaxLength(255);

        builder.HasOne(d => d.Anomalia).WithMany(p => p.Incidentes)
            .HasForeignKey(d => d.AnomaliaId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Incidente__Anoma__68487DD7");
    }
}
