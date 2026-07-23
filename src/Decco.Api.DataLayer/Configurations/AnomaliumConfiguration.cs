using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class AnomaliumConfiguration : IEntityTypeConfiguration<Anomalium>
{
    public void Configure(EntityTypeBuilder<Anomalium> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Anomalia__3214EC07CC25B98D");

        builder.ToTable(tb =>
            {
                tb.HasTrigger("TR_Anomalia_Update_Date");
                tb.HasTrigger("TR_Anomalia_Validar_Mecanismos");
            });

        builder.HasIndex(e => e.CodigoScp, "IX_Anomalia_CodigoSCP");

        builder.HasIndex(e => e.Status, "IX_Anomalia_Status");

        builder.HasIndex(e => e.CodigoScp, "UQ__Anomalia__F02FBC8831139E8E").IsUnique();

        builder.Property(e => e.CodigoScp)
            .HasMaxLength(50)
            .IsUnicode(false)
            .HasColumnName("CodigoSCP");
        builder.Property(e => e.DataAtualizacao)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.DataCriacao)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.FatorCoerenciaSpin)
            .HasMaxLength(20)
            .IsUnicode(false);
        builder.Property(e => e.IeiaDBase)
            .HasColumnType("decimal(8, 4)")
            .HasColumnName("IEIA_D_Base");
        builder.Property(e => e.NomeComum).HasMaxLength(255);
        builder.Property(e => e.ResponsavelPesquisa).HasMaxLength(255);
        builder.Property(e => e.SitioContencao).HasMaxLength(100);
        builder.Property(e => e.Status)
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasDefaultValue("ATIVA");
        builder.Property(e => e.UsuarioAtualizacao)
            .HasMaxLength(128)
            .HasDefaultValueSql("(suser_sname())");
        builder.Property(e => e.UsuarioCriacao)
            .HasMaxLength(128)
            .HasDefaultValueSql("(suser_sname())");

        builder.HasOne(d => d.CamadaOntologica).WithMany(p => p.Anomalia)
            .HasForeignKey(d => d.CamadaOntologicaId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Anomalia__Camada__412EB0B6");

        builder.HasOne(d => d.ClasseObjeto).WithMany(p => p.Anomalia)
            .HasForeignKey(d => d.ClasseObjetoId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Anomalia__Classe__403A8C7D");

        builder.HasOne(d => d.MecanismoPrimario).WithMany(p => p.AnomaliumMecanismoPrimarios)
            .HasForeignKey(d => d.MecanismoPrimarioId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Anomalia__Mecani__4316F928");

        builder.HasOne(d => d.MecanismoSecundario).WithMany(p => p.AnomaliumMecanismoSecundarios)
            .HasForeignKey(d => d.MecanismoSecundarioId)
            .HasConstraintName("FK__Anomalia__Mecani__440B1D61");

        builder.HasOne(d => d.TipoMateria).WithMany(p => p.Anomalia)
            .HasForeignKey(d => d.TipoMateriaId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Anomalia__TipoMa__4222D4EF");

        builder.HasOne(d => d.CognicaoAparente).WithMany(p => p.Anomalia)
            .HasForeignKey(d => d.CognicaoAparenteId)
            .HasConstraintName("FK_Anomalia_CognicaoAparente");

        builder.HasOne(d => d.Periculosidade).WithMany(p => p.Anomalia)
            .HasForeignKey(d => d.PericulosidadeId)
            .HasConstraintName("FK_Anomalia_Periculosidade");
    }
}
