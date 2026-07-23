using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decco.Api.DataLayer.Configurations;

public class CatClasseObjetoConfiguration : IEntityTypeConfiguration<CatClasseObjeto>
{
    public void Configure(EntityTypeBuilder<CatClasseObjeto> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Cat_Clas__3214EC07EB39AA3C");

        builder.ToTable("Cat_ClasseObjeto");

        builder.HasIndex(e => e.Codigo, "UQ__Cat_Clas__06370DAC826148C5").IsUnique();

        builder.Property(e => e.Ativo).HasDefaultValue(true);
        builder.Property(e => e.ClasseAcs)
            .HasMaxLength(40)
            .IsUnicode(false)
            .HasColumnName("ClasseACS");
        builder.Property(e => e.Codigo)
            .HasMaxLength(10)
            .IsUnicode(false);
        builder.Property(e => e.CorAlerta)
            .HasMaxLength(7)
            .IsUnicode(false)
            .HasDefaultValue("#FFFFFF");
        builder.Property(e => e.DataCriacao)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.Descricao).HasColumnType("text");
        builder.Property(e => e.NivelAcessoMinimo).HasDefaultValue(1);
        builder.Property(e => e.Nome)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.TipoClasse)
            .HasMaxLength(50)
            .IsUnicode(false);
    }
}
