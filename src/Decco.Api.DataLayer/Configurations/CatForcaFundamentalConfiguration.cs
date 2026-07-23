using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class CatForcaFundamentalConfiguration : IEntityTypeConfiguration<CatForcaFundamental>
{
    public void Configure(EntityTypeBuilder<CatForcaFundamental> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Cat_Forc__3214EC073A7D3E0F");

        builder.ToTable("Cat_ForcaFundamental");

        builder.HasIndex(e => e.Simbolo, "UQ__Cat_Forc__090D9EAC639F135B").IsUnique();

        builder.HasIndex(e => e.Nome, "UQ__Cat_Forc__7D8FE3B2A558EFCE").IsUnique();

        builder.Property(e => e.Nome)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.ParticulaPortadora)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.Simbolo)
            .HasMaxLength(10)
            .IsUnicode(false)
            .IsFixedLength();
    }
}
