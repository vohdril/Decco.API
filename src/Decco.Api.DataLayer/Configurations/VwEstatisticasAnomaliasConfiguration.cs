using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class VwEstatisticasAnomaliasConfiguration : IEntityTypeConfiguration<VwEstatisticasAnomalias>
{
    public void Configure(EntityTypeBuilder<VwEstatisticasAnomalias> builder)
    {
        builder
            .HasNoKey()
            .ToView("vw_Estatisticas_Anomalias");

        builder.Property(e => e.CamadaOmega).HasColumnName("Camada_Omega");
        builder.Property(e => e.CamadaPhi).HasColumnName("Camada_Phi");
        builder.Property(e => e.CamadaPsi).HasColumnName("Camada_Psi");
        builder.Property(e => e.CamadaTheta).HasColumnName("Camada_Theta");
        builder.Property(e => e.ClasseApotheosis).HasColumnName("Classe_Apotheosis");
        builder.Property(e => e.ClasseEuclid).HasColumnName("Classe_Euclid");
        builder.Property(e => e.ClasseKeter).HasColumnName("Classe_Keter");
        builder.Property(e => e.ClasseSafe).HasColumnName("Classe_Safe");
        builder.Property(e => e.ClasseThaumiel).HasColumnName("Classe_Thaumiel");
        builder.Property(e => e.IeiaDMedio)
            .HasColumnType("decimal(38, 6)")
            .HasColumnName("IEIA_D_Medio");
        builder.Property(e => e.MateriaBarionica).HasColumnName("Materia_Barionica");
        builder.Property(e => e.MateriaMista).HasColumnName("Materia_Mista");
        builder.Property(e => e.MateriaNaoBarionica).HasColumnName("Materia_NaoBarionica");
    }
}
