using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class NotificacaoAnomaliaConfiguration : IEntityTypeConfiguration<NotificacaoAnomalia>
{
    public void Configure(EntityTypeBuilder<NotificacaoAnomalia> builder)
    {
        builder.ToTable("NotificacaoAnomalia");
        builder.HasIndex(e => e.Status, "IX_NotificacaoAnomalia_Status");
        builder.HasIndex(e => e.DataHora, "IX_NotificacaoAnomalia_Data");
        builder.Property(e => e.Titulo).HasMaxLength(255);
        builder.Property(e => e.LocalIdentificado).HasMaxLength(255);
        builder.Property(e => e.DataHora).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
        builder.Property(e => e.Status).HasMaxLength(20).IsUnicode(false).HasDefaultValue("PENDENTE");
        builder.Property(e => e.Relator).HasMaxLength(255);
        builder.Property(e => e.DataResolucao).HasColumnType("datetime");

        builder.HasOne(d => d.Anomalium).WithMany(p => p.NotificacaoAnomalias)
            .HasForeignKey(d => d.AnomaliaId)
            .HasConstraintName("FK_NotificacaoAnomalia_Anomalia");
    }
}
