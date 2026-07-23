using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class VwDashboardAnomaliasConfiguration : IEntityTypeConfiguration<VwDashboardAnomalias>
{
    public void Configure(EntityTypeBuilder<VwDashboardAnomalias> builder)
    {
        builder
            .HasNoKey()
            .ToView("vw_Dashboard_Anomalias");

        builder.Property(e => e.Camada)
            .HasMaxLength(10)
            .IsUnicode(false)
            .IsFixedLength();
        builder.Property(e => e.CamadaOntologica)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.ClasseObjeto)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.CodigoScp)
            .HasMaxLength(50)
            .IsUnicode(false)
            .HasColumnName("CodigoSCP");
        builder.Property(e => e.CorAlerta)
            .HasMaxLength(7)
            .IsUnicode(false);
        builder.Property(e => e.FatorCoerenciaSpin)
            .HasMaxLength(20)
            .IsUnicode(false);
        builder.Property(e => e.IeiaDBase)
            .HasColumnType("decimal(8, 4)")
            .HasColumnName("IEIA_D_Base");
        builder.Property(e => e.MecanismoPrimario)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.MecanismoSecundario)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.NomeComum).HasMaxLength(255);
        builder.Property(e => e.SitioContencao).HasMaxLength(100);
        builder.Property(e => e.Status)
            .HasMaxLength(20)
            .IsUnicode(false);
        builder.Property(e => e.StatusTheta)
            .HasMaxLength(12)
            .IsUnicode(false);
        builder.Property(e => e.TipoMateria)
            .HasMaxLength(50)
            .IsUnicode(false);
    }
}
