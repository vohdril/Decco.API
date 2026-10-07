using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class CatOperacaoConfiguration : IEntityTypeConfiguration<CatOperacao>
{
    public void Configure(EntityTypeBuilder<CatOperacao> builder)
    {
        builder.ToTable("Cat_Operacao");
        builder.HasIndex(e => e.Codigo).IsUnique();
        builder.Property(e => e.Codigo).HasMaxLength(20).IsUnicode(false);
        builder.Property(e => e.Nome).HasMaxLength(50);
        builder.Property(e => e.NivelAcessoMinimo).HasDefaultValue(1);
        builder.Property(e => e.CorAlerta).HasMaxLength(7).IsUnicode(false).HasDefaultValue("#FFFFFF");
    }
}
