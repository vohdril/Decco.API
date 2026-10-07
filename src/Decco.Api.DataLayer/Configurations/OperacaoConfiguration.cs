using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class OperacaoConfiguration : IEntityTypeConfiguration<Operacao>
{
    public void Configure(EntityTypeBuilder<Operacao> builder)
    {
        builder.ToTable("Operacao", tb =>
            {
                tb.HasTrigger("TR_Operacao_Update_Date");
                tb.HasTrigger("TR_Operacao_Validar");
            });

        builder.HasIndex(e => e.Codigo).IsUnique();
        builder.HasIndex(e => e.InstalacaoId, "IX_Operacao_Instalacao");
        builder.HasIndex(e => e.Status, "IX_Operacao_Status");
        builder.HasIndex(e => e.AnomaliaId, "IX_Operacao_Anomalia");
        builder.HasIndex(e => e.TipoOperacaoId, "IX_Operacao_Tipo");

        builder.Property(e => e.Codigo).HasMaxLength(20).IsUnicode(false);
        builder.Property(e => e.Codinome).HasMaxLength(100);
        builder.Property(e => e.Objetivo).HasMaxLength(500);
        builder.Property(e => e.Status).HasMaxLength(20).IsUnicode(false).HasDefaultValue("PLANEJADA");
        builder.Property(e => e.Prioridade).HasDefaultValue(3);
        builder.Property(e => e.NivelAcessoMinimo).HasDefaultValue(1);
        builder.Property(e => e.Responsavel).HasMaxLength(255);
        builder.Property(e => e.DataAbertura).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
        builder.Property(e => e.DataPrevisaoTermino).HasColumnType("datetime");
        builder.Property(e => e.DataEncerramento).HasColumnType("datetime");
        builder.Property(e => e.DataCriacao).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
        builder.Property(e => e.DataAtualizacao).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
        builder.Property(e => e.UsuarioCriacao).HasMaxLength(128).HasDefaultValueSql("(suser_sname())");
        builder.Property(e => e.UsuarioAtualizacao).HasMaxLength(128).HasDefaultValueSql("(suser_sname())");

        builder.HasOne(d => d.TipoOperacao).WithMany(p => p.Operacoes)
            .HasForeignKey(d => d.TipoOperacaoId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(d => d.Instalacao).WithMany()
            .HasForeignKey(d => d.InstalacaoId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(d => d.Anomalia).WithMany()
            .HasForeignKey(d => d.AnomaliaId);

        builder.HasOne(d => d.Notificacao).WithMany()
            .HasForeignKey(d => d.NotificacaoId);

        builder.HasOne(d => d.Protocolo).WithMany()
            .HasForeignKey(d => d.ProtocoloId);
    }
}
