using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class InstalacaoConfiguration : IEntityTypeConfiguration<Instalacao>
{
    public void Configure(EntityTypeBuilder<Instalacao> builder)
    {
        // Os triggers precisam ser DECLARADOS: desde o EF Core 7, o SaveChanges usa
        // OUTPUT para ler valores gerados, e o SQL Server recusa OUTPUT sem INTO
        // numa tabela com trigger. HasTrigger faz o EF trocar a estratégia.
        builder.ToTable("Instalacao", tb =>
            {
                tb.HasTrigger("TR_Instalacao_Update_Date");
                tb.HasTrigger("TR_Instalacao_Validar_Hierarquia");
            });

        builder.HasIndex(e => e.Codigo).IsUnique();
        builder.HasIndex(e => e.InstalacaoPaiId, "IX_Instalacao_Pai");
        builder.HasIndex(e => e.TipoInstalacaoId, "IX_Instalacao_Tipo");

        builder.Property(e => e.Codigo).HasMaxLength(20).IsUnicode(false);
        builder.Property(e => e.Nome).HasMaxLength(255);
        builder.Property(e => e.Responsavel).HasMaxLength(255);
        builder.Property(e => e.Especialidade).HasMaxLength(50).IsUnicode(false);
        builder.Property(e => e.NivelAcessoMinimo).HasDefaultValue(1);
        builder.Property(e => e.Status).HasMaxLength(20).IsUnicode(false).HasDefaultValue("ATIVA");
        builder.Property(e => e.DataCriacao).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
        builder.Property(e => e.DataAtualizacao).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
        builder.Property(e => e.UsuarioCriacao).HasMaxLength(128).HasDefaultValueSql("(suser_sname())");
        builder.Property(e => e.UsuarioAtualizacao).HasMaxLength(128).HasDefaultValueSql("(suser_sname())");

        builder.HasOne(d => d.TipoInstalacao).WithMany(p => p.Instalacoes)
            .HasForeignKey(d => d.TipoInstalacaoId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(d => d.InstalacaoPai).WithMany(p => p.Filhas)
            .HasForeignKey(d => d.InstalacaoPaiId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
