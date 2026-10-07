using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class CatTipoInstalacaoConfiguration : IEntityTypeConfiguration<CatTipoInstalacao>
{
    public void Configure(EntityTypeBuilder<CatTipoInstalacao> builder)
    {
        builder.ToTable("Cat_TipoInstalacao");
        builder.HasIndex(e => e.Codigo).IsUnique();
        builder.Property(e => e.Codigo).HasMaxLength(20).IsUnicode(false);
        builder.Property(e => e.Nome).HasMaxLength(50);
    }
}
