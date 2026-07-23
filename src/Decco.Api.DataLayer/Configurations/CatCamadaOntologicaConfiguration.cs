using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class CatCamadaOntologicaConfiguration : IEntityTypeConfiguration<CatCamadaOntologica>
{
    public void Configure(EntityTypeBuilder<CatCamadaOntologica> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Cat_Cama__3214EC07EAF5D8B4");

        builder.ToTable("Cat_CamadaOntologica");

        builder.HasIndex(e => e.Simbolo, "UQ__Cat_Cama__090D9EAC04A50054").IsUnique();

        builder.Property(e => e.Nome)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.Prioridade).HasDefaultValue(1);
        builder.Property(e => e.Simbolo)
            .HasMaxLength(10)
            .IsUnicode(false)
            .IsFixedLength();

        builder.HasOne(d => d.ForcaFundamental).WithMany(p => p.CatCamadaOntologicas)
            .HasForeignKey(d => d.ForcaFundamentalId)
            .HasConstraintName("FK__Cat_Camad__Forca__300424B4");
    }
}
