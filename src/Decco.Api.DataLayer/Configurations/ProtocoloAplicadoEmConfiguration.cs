using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class ProtocoloAplicadoEmConfiguration : IEntityTypeConfiguration<ProtocoloAplicadoEm>
{
    public void Configure(EntityTypeBuilder<ProtocoloAplicadoEm> builder)
    {
        builder.HasKey(e => new { e.ProtocoloId, e.AnomaliaId });
        builder.ToTable("Protocolo_AplicadoEm");
        builder.Property(e => e.Status).HasMaxLength(20).IsUnicode(false).HasDefaultValue("ATIVO");
        builder.Property(e => e.DataInicio).HasColumnType("datetime");
        builder.Property(e => e.DataFim).HasColumnType("datetime");

        builder.HasOne(d => d.Anomalium).WithMany(p => p.ProtocoloAplicadoEms)
            .HasForeignKey(d => d.AnomaliaId)
            .HasConstraintName("FK_Protocolo_AplicadoEm_Anomalia");

        builder.HasOne(d => d.ProtocoloContencao).WithMany(p => p.ProtocoloAplicadoEms)
            .HasForeignKey(d => d.ProtocoloId)
            .HasConstraintName("FK_Protocolo_AplicadoEm_Protocolo");
    }
}
