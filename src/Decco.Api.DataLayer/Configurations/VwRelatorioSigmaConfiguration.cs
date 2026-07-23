using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class VwRelatorioSigmaConfiguration : IEntityTypeConfiguration<VwRelatorioSigma>
{
    public void Configure(EntityTypeBuilder<VwRelatorioSigma> builder)
    {
        builder
            .HasNoKey()
            .ToView("vw_Relatorio_Sigma");

        builder.Property(e => e.Camada)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.Classe)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.CodigoScp)
            .HasMaxLength(50)
            .IsUnicode(false)
            .HasColumnName("CodigoSCP");
        builder.Property(e => e.NomeComum).HasMaxLength(255);
        builder.Property(e => e.ResponsavelPesquisa).HasMaxLength(255);
        builder.Property(e => e.SitioContencao).HasMaxLength(100);
        builder.Property(e => e.UltimoIncidente).HasColumnType("datetime");
    }
}
