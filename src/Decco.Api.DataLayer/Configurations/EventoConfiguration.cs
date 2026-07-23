using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class EventoConfiguration : IEntityTypeConfiguration<Evento>
{
    public void Configure(EntityTypeBuilder<Evento> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Evento__3214EC07A8AA20C5");

        builder.ToTable("Evento");

        builder.HasIndex(e => e.AnomaliaId, "IX_Evento_AnomaliaId");

        builder.Property(e => e.DataHoraFim).HasColumnType("datetime");
        builder.Property(e => e.DataHoraInicio).HasColumnType("datetime");
        builder.Property(e => e.Nome).HasMaxLength(255);
        builder.Property(e => e.Periodicidade).HasMaxLength(100);
        builder.Property(e => e.ZonaAfetada).HasMaxLength(255);

        builder.HasOne(d => d.Anomalia).WithMany(p => p.Eventos)
            .HasForeignKey(d => d.AnomaliaId)
            .HasConstraintName("FK__Evento__Anomalia__5629CD9C");
    }
}
