using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class CatMecanismoInteracaoConfiguration : IEntityTypeConfiguration<CatMecanismoInteracao>
{
    public void Configure(EntityTypeBuilder<CatMecanismoInteracao> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Cat_Meca__3214EC07305BCFDD");

        builder.ToTable("Cat_MecanismoInteracao");

        builder.HasIndex(e => e.Codigo, "UQ__Cat_Meca__06370DAC28701545").IsUnique();

        builder.Property(e => e.Codigo)
            .HasMaxLength(10)
            .IsUnicode(false);
        builder.Property(e => e.Nome)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.HasOne(d => d.CamadaOntologica).WithMany(p => p.CatMecanismoInteracaos)
            .HasForeignKey(d => d.CamadaOntologicaId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Cat_Mecan__Camad__38996AB5");
    }
}
