using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class LaboratorioConfiguration : IEntityTypeConfiguration<Laboratorio>
{
    public void Configure(EntityTypeBuilder<Laboratorio> builder)
    {
        builder.ToTable("Laboratorio");
        builder.HasIndex(e => e.Codigo).IsUnique();
        builder.Property(e => e.Codigo).HasMaxLength(20).IsUnicode(false);
        builder.Property(e => e.Nome).HasMaxLength(255);
        builder.Property(e => e.Sitio).HasMaxLength(100);
        builder.Property(e => e.Responsavel).HasMaxLength(255);
        builder.Property(e => e.Especialidade).HasMaxLength(50).IsUnicode(false);
        builder.Property(e => e.Status).HasMaxLength(20).IsUnicode(false).HasDefaultValue("ATIVO");
        builder.Property(e => e.DataCriacao).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
        builder.Property(e => e.DataAtualizacao).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
    }
}
