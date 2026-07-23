using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class ProtocoloContencaoConfiguration : IEntityTypeConfiguration<ProtocoloContencao>
{
    public void Configure(EntityTypeBuilder<ProtocoloContencao> builder)
    {
        builder.ToTable("ProtocoloContencao");
        builder.HasIndex(e => e.Codigo).IsUnique();
        builder.Property(e => e.Codigo).HasMaxLength(20).IsUnicode(false);
        builder.Property(e => e.Titulo).HasMaxLength(255);
        builder.Property(e => e.ClassesAplicaveis).HasMaxLength(100).IsUnicode(false);
        builder.Property(e => e.DataCriacao).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
        builder.Property(e => e.DataAtualizacao).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
    }
}
