using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class CatPericulosidadeConfiguration : IEntityTypeConfiguration<CatPericulosidade>
{
    public void Configure(EntityTypeBuilder<CatPericulosidade> builder)
    {
        builder.ToTable("Cat_Periculosidade");
        builder.HasIndex(e => e.Nivel).IsUnique();
        builder.Property(e => e.Nome).HasMaxLength(50).IsUnicode(false);
        builder.Property(e => e.CorAlerta).HasMaxLength(7).IsUnicode(false);
    }
}
