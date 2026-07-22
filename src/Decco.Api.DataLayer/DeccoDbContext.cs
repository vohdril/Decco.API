using System;
using System.Collections.Generic;
using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Decco.Api.DataLayer;

public partial class DeccoDbContext : DbContext
{
    public DeccoDbContext(DbContextOptions<DeccoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Anomalium> Anomalia { get; set; }

    public virtual DbSet<Artefato> Artefatos { get; set; }

    public virtual DbSet<CatCamadaOntologica> CatCamadaOntologicas { get; set; }

    public virtual DbSet<CatClasseObjeto> CatClasseObjetos { get; set; }

    public virtual DbSet<CatForcaFundamental> CatForcaFundamentals { get; set; }

    public virtual DbSet<CatManifestacaoEspecifica> CatManifestacaoEspecificas { get; set; }

    public virtual DbSet<CatMecanismoInteracao> CatMecanismoInteracaos { get; set; }

    public virtual DbSet<CatTipoMaterium> CatTipoMateria { get; set; }

    public virtual DbSet<EntidadeViva> EntidadeVivas { get; set; }

    public virtual DbSet<Evento> Eventos { get; set; }

    public virtual DbSet<Incidente> Incidentes { get; set; }

    public virtual DbSet<InstanciaPericiaDesviante> InstanciaPericiaDesviantes { get; set; }

    public virtual DbSet<Localidade> Localidades { get; set; }

    public virtual DbSet<PericiaAnomalium> PericiaAnomalia { get; set; }

    public virtual DbSet<PericiaManifestacao> PericiaManifestacaos { get; set; }

    public virtual DbSet<VwDashboardAnomalias> VwDashboardAnomaliases { get; set; }

    public virtual DbSet<VwEstatisticasAnomalias> VwEstatisticasAnomaliases { get; set; }

    public virtual DbSet<VwRelatorioSigma> VwRelatorioSigmas { get; set; }

    public virtual DbSet<CatCognicaoAparente> CatCognicaoAparentes { get; set; }

    public virtual DbSet<CatPericulosidade> CatPericulosidades { get; set; }

    public virtual DbSet<Laboratorio> Laboratorios { get; set; }

    public virtual DbSet<ProtocoloContencao> ProtocoloContencaos { get; set; }

    public virtual DbSet<ProtocoloAplicadoEm> ProtocoloAplicadoEms { get; set; }

    public virtual DbSet<NotificacaoAnomalia> NotificacaoAnomalias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anomalium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Anomalia__3214EC07CC25B98D");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("TR_Anomalia_Update_Date");
                    tb.HasTrigger("TR_Anomalia_Validar_Mecanismos");
                });

            entity.HasIndex(e => e.CodigoScp, "IX_Anomalia_CodigoSCP");

            entity.HasIndex(e => e.Status, "IX_Anomalia_Status");

            entity.HasIndex(e => e.CodigoScp, "UQ__Anomalia__F02FBC8831139E8E").IsUnique();

            entity.Property(e => e.CodigoScp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CodigoSCP");
            entity.Property(e => e.DataAtualizacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DataCriacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FatorCoerenciaSpin)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IeiaDBase)
                .HasColumnType("decimal(8, 4)")
                .HasColumnName("IEIA_D_Base");
            entity.Property(e => e.NomeComum).HasMaxLength(255);
            entity.Property(e => e.ResponsavelPesquisa).HasMaxLength(255);
            entity.Property(e => e.SitioContencao).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("ATIVA");
            entity.Property(e => e.UsuarioAtualizacao)
                .HasMaxLength(128)
                .HasDefaultValueSql("(suser_sname())");
            entity.Property(e => e.UsuarioCriacao)
                .HasMaxLength(128)
                .HasDefaultValueSql("(suser_sname())");

            entity.HasOne(d => d.CamadaOntologica).WithMany(p => p.Anomalia)
                .HasForeignKey(d => d.CamadaOntologicaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Anomalia__Camada__412EB0B6");

            entity.HasOne(d => d.ClasseObjeto).WithMany(p => p.Anomalia)
                .HasForeignKey(d => d.ClasseObjetoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Anomalia__Classe__403A8C7D");

            entity.HasOne(d => d.MecanismoPrimario).WithMany(p => p.AnomaliumMecanismoPrimarios)
                .HasForeignKey(d => d.MecanismoPrimarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Anomalia__Mecani__4316F928");

            entity.HasOne(d => d.MecanismoSecundario).WithMany(p => p.AnomaliumMecanismoSecundarios)
                .HasForeignKey(d => d.MecanismoSecundarioId)
                .HasConstraintName("FK__Anomalia__Mecani__440B1D61");

            entity.HasOne(d => d.TipoMateria).WithMany(p => p.Anomalia)
                .HasForeignKey(d => d.TipoMateriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Anomalia__TipoMa__4222D4EF");

            entity.HasOne(d => d.CognicaoAparente).WithMany(p => p.Anomalia)
                .HasForeignKey(d => d.CognicaoAparenteId)
                .HasConstraintName("FK_Anomalia_CognicaoAparente");

            entity.HasOne(d => d.Periculosidade).WithMany(p => p.Anomalia)
                .HasForeignKey(d => d.PericulosidadeId)
                .HasConstraintName("FK_Anomalia_Periculosidade");
        });

        modelBuilder.Entity<Artefato>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Artefato__3214EC078DC74F28");

            entity.ToTable("Artefato");

            entity.HasIndex(e => e.AnomaliaId, "IX_Artefato_AnomaliaId");

            entity.Property(e => e.Dimensoes)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Identificacao).HasMaxLength(100);
            entity.Property(e => e.LocalOrigem).HasMaxLength(255);
            entity.Property(e => e.Material).HasMaxLength(255);
            entity.Property(e => e.PesoKg)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Peso_Kg");
            entity.Property(e => e.PropriedadeSpin)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Anomalia).WithMany(p => p.Artefatos)
                .HasForeignKey(d => d.AnomaliaId)
                .HasConstraintName("FK__Artefato__Anomal__4F7CD00D");
        });

        modelBuilder.Entity<CatCamadaOntologica>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cat_Cama__3214EC07EAF5D8B4");

            entity.ToTable("Cat_CamadaOntologica");

            entity.HasIndex(e => e.Simbolo, "UQ__Cat_Cama__090D9EAC04A50054").IsUnique();

            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Prioridade).HasDefaultValue(1);
            entity.Property(e => e.Simbolo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.ForcaFundamental).WithMany(p => p.CatCamadaOntologicas)
                .HasForeignKey(d => d.ForcaFundamentalId)
                .HasConstraintName("FK__Cat_Camad__Forca__300424B4");
        });

        modelBuilder.Entity<CatClasseObjeto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cat_Clas__3214EC07EB39AA3C");

            entity.ToTable("Cat_ClasseObjeto");

            entity.HasIndex(e => e.Codigo, "UQ__Cat_Clas__06370DAC826148C5").IsUnique();

            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.ClasseAcs)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("ClasseACS");
            entity.Property(e => e.Codigo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CorAlerta)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasDefaultValue("#FFFFFF");
            entity.Property(e => e.DataCriacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Descricao).HasColumnType("text");
            entity.Property(e => e.NivelAcessoMinimo).HasDefaultValue(1);
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TipoClasse)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatForcaFundamental>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cat_Forc__3214EC073A7D3E0F");

            entity.ToTable("Cat_ForcaFundamental");

            entity.HasIndex(e => e.Simbolo, "UQ__Cat_Forc__090D9EAC639F135B").IsUnique();

            entity.HasIndex(e => e.Nome, "UQ__Cat_Forc__7D8FE3B2A558EFCE").IsUnique();

            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ParticulaPortadora)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Simbolo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<CatManifestacaoEspecifica>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cat_Mani__3214EC07411DA892");

            entity.ToTable("Cat_ManifestacaoEspecifica");

            entity.HasIndex(e => e.Codigo, "UQ__Cat_Mani__06370DAC6716953A").IsUnique();

            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Nome).HasMaxLength(100);
        });

        modelBuilder.Entity<CatMecanismoInteracao>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cat_Meca__3214EC07305BCFDD");

            entity.ToTable("Cat_MecanismoInteracao");

            entity.HasIndex(e => e.Codigo, "UQ__Cat_Meca__06370DAC28701545").IsUnique();

            entity.Property(e => e.Codigo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.CamadaOntologica).WithMany(p => p.CatMecanismoInteracaos)
                .HasForeignKey(d => d.CamadaOntologicaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cat_Mecan__Camad__38996AB5");
        });

        modelBuilder.Entity<CatTipoMaterium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cat_Tipo__3214EC07CDADACA6");

            entity.ToTable("Cat_TipoMateria");

            entity.HasIndex(e => e.Nome, "UQ__Cat_Tipo__7D8FE3B2C88FCF2D").IsUnique();

            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EntidadeViva>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Entidade__3214EC07F947E505");

            entity.ToTable("EntidadeViva");

            entity.HasIndex(e => e.AnomaliaId, "IX_EntidadeViva_AnomaliaId");

            entity.Property(e => e.Biologia).HasMaxLength(255);
            entity.Property(e => e.Dieta).HasMaxLength(100);
            entity.Property(e => e.Especie).HasMaxLength(150);
            entity.Property(e => e.Identificacao).HasMaxLength(100);
            entity.Property(e => e.IsConsciente).HasDefaultValue(true);
            entity.Property(e => e.OrigemPoder).HasMaxLength(100);

            entity.HasOne(d => d.Anomalia).WithMany(p => p.EntidadeVivas)
                .HasForeignKey(d => d.AnomaliaId)
                .HasConstraintName("FK__EntidadeV__Anoma__4BAC3F29");
        });

        modelBuilder.Entity<Evento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Evento__3214EC07A8AA20C5");

            entity.ToTable("Evento");

            entity.HasIndex(e => e.AnomaliaId, "IX_Evento_AnomaliaId");

            entity.Property(e => e.DataHoraFim).HasColumnType("datetime");
            entity.Property(e => e.DataHoraInicio).HasColumnType("datetime");
            entity.Property(e => e.Nome).HasMaxLength(255);
            entity.Property(e => e.Periodicidade).HasMaxLength(100);
            entity.Property(e => e.ZonaAfetada).HasMaxLength(255);

            entity.HasOne(d => d.Anomalia).WithMany(p => p.Eventos)
                .HasForeignKey(d => d.AnomaliaId)
                .HasConstraintName("FK__Evento__Anomalia__5629CD9C");
        });

        modelBuilder.Entity<Incidente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Incident__3214EC077D3331FB");

            entity.ToTable("Incidente");

            entity.HasIndex(e => e.AnomaliaId, "IX_Incidente_Anomalia");

            entity.Property(e => e.DanoMaterial).HasMaxLength(255);
            entity.Property(e => e.DataHora)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Feridos).HasDefaultValue(0);
            entity.Property(e => e.IsEventoSigma).HasDefaultValue(false);
            entity.Property(e => e.Mortes).HasDefaultValue(0);
            entity.Property(e => e.NivelSeguranca)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Titulo).HasMaxLength(255);

            entity.HasOne(d => d.Anomalia).WithMany(p => p.Incidentes)
                .HasForeignKey(d => d.AnomaliaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Incidente__Anoma__68487DD7");
        });

        modelBuilder.Entity<InstanciaPericiaDesviante>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Instanci__3214EC07FCE1266A");

            entity.ToTable("Instancia_PericiaDesviante");

            entity.HasIndex(e => new { e.TipoInstancia, e.InstanciaId }, "IX_Instancia_PericiaDesviante_Instancia");

            entity.HasIndex(e => e.PericiaDesvianteId, "IX_Instancia_PericiaDesviante_Pericia");

            entity.Property(e => e.DataDescoberta).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Intensidade)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TipoInstancia)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.PericiaDesviante).WithMany(p => p.InstanciaPericiaDesviantes)
                .HasForeignKey(d => d.PericiaDesvianteId)
                .HasConstraintName("FK__Instancia__Peric__60A75C0F");
        });

        modelBuilder.Entity<Localidade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Localida__3214EC0721A940BF");

            entity.ToTable("Localidade");

            entity.HasIndex(e => e.AnomaliaId, "IX_Localidade_AnomaliaId");

            entity.Property(e => e.ClimaAnomalo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IeiaDAmbiente)
                .HasColumnType("decimal(8, 4)")
                .HasColumnName("IEIA_D_Ambiente");
            entity.Property(e => e.IsGeograficamenteLimitada).HasDefaultValue(true);
            entity.Property(e => e.Nome).HasMaxLength(255);
            entity.Property(e => e.TipoTerreno)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Anomalia).WithMany(p => p.Localidades)
                .HasForeignKey(d => d.AnomaliaId)
                .HasConstraintName("FK__Localidad__Anoma__52593CB8");
        });

        modelBuilder.Entity<PericiaAnomalium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PericiaA__3214EC073B1D400F");

            entity.HasIndex(e => new { e.AnomaliaId, e.Nome }, "UQ_Pericia_Anomalia_Nome").IsUnique();

            entity.Property(e => e.Custo).HasMaxLength(100);
            entity.Property(e => e.Nivel).HasDefaultValue(1);
            entity.Property(e => e.Nome).HasMaxLength(100);

            entity.HasOne(d => d.Anomalia).WithMany(p => p.PericiaAnomalia)
                .HasForeignKey(d => d.AnomaliaId)
                .HasConstraintName("FK__PericiaAn__Anoma__59FA5E80");

            entity.HasOne(d => d.MecanismoPrimario).WithMany(p => p.PericiaAnomaliumMecanismoPrimarios)
                .HasForeignKey(d => d.MecanismoPrimarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PericiaAn__Mecan__5AEE82B9");

            entity.HasOne(d => d.MecanismoSecundario).WithMany(p => p.PericiaAnomaliumMecanismoSecundarios)
                .HasForeignKey(d => d.MecanismoSecundarioId)
                .HasConstraintName("FK__PericiaAn__Mecan__5BE2A6F2");
        });

        modelBuilder.Entity<PericiaManifestacao>(entity =>
        {
            entity.HasKey(e => new { e.PericiaAnomaliaId, e.ManifestacaoEspecificaId }).HasName("PK__Pericia___F733A3D702044810");

            entity.ToTable("Pericia_Manifestacao");

            entity.Property(e => e.Intensidade)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.ManifestacaoEspecifica).WithMany(p => p.PericiaManifestacaos)
                .HasForeignKey(d => d.ManifestacaoEspecificaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pericia_M__Manif__656C112C");

            entity.HasOne(d => d.PericiaAnomalia).WithMany(p => p.PericiaManifestacaos)
                .HasForeignKey(d => d.PericiaAnomaliaId)
                .HasConstraintName("FK__Pericia_M__Peric__6477ECF3");
        });

        modelBuilder.Entity<VwDashboardAnomalias>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Dashboard_Anomalias");

            entity.Property(e => e.Camada)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CamadaOntologica)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ClasseObjeto)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CodigoScp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CodigoSCP");
            entity.Property(e => e.CorAlerta)
                .HasMaxLength(7)
                .IsUnicode(false);
            entity.Property(e => e.FatorCoerenciaSpin)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IeiaDBase)
                .HasColumnType("decimal(8, 4)")
                .HasColumnName("IEIA_D_Base");
            entity.Property(e => e.MecanismoPrimario)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MecanismoSecundario)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NomeComum).HasMaxLength(255);
            entity.Property(e => e.SitioContencao).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StatusTheta)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.TipoMateria)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwEstatisticasAnomalias>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Estatisticas_Anomalias");

            entity.Property(e => e.CamadaOmega).HasColumnName("Camada_Omega");
            entity.Property(e => e.CamadaPhi).HasColumnName("Camada_Phi");
            entity.Property(e => e.CamadaPsi).HasColumnName("Camada_Psi");
            entity.Property(e => e.CamadaTheta).HasColumnName("Camada_Theta");
            entity.Property(e => e.ClasseApotheosis).HasColumnName("Classe_Apotheosis");
            entity.Property(e => e.ClasseEuclid).HasColumnName("Classe_Euclid");
            entity.Property(e => e.ClasseKeter).HasColumnName("Classe_Keter");
            entity.Property(e => e.ClasseSafe).HasColumnName("Classe_Safe");
            entity.Property(e => e.ClasseThaumiel).HasColumnName("Classe_Thaumiel");
            entity.Property(e => e.IeiaDMedio)
                .HasColumnType("decimal(38, 6)")
                .HasColumnName("IEIA_D_Medio");
            entity.Property(e => e.MateriaBarionica).HasColumnName("Materia_Barionica");
            entity.Property(e => e.MateriaMista).HasColumnName("Materia_Mista");
            entity.Property(e => e.MateriaNaoBarionica).HasColumnName("Materia_NaoBarionica");
        });

        modelBuilder.Entity<VwRelatorioSigma>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Relatorio_Sigma");

            entity.Property(e => e.Camada)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Classe)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CodigoScp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CodigoSCP");
            entity.Property(e => e.NomeComum).HasMaxLength(255);
            entity.Property(e => e.ResponsavelPesquisa).HasMaxLength(255);
            entity.Property(e => e.SitioContencao).HasMaxLength(100);
            entity.Property(e => e.UltimoIncidente).HasColumnType("datetime");
        });

        modelBuilder.Entity<CatCognicaoAparente>(entity =>
        {
            entity.ToTable("Cat_CognicaoAparente");
            entity.HasIndex(e => e.Codigo).IsUnique();
            entity.Property(e => e.Codigo).HasMaxLength(5).IsUnicode(false);
            entity.Property(e => e.Nome).HasMaxLength(50).IsUnicode(false);
        });

        modelBuilder.Entity<CatPericulosidade>(entity =>
        {
            entity.ToTable("Cat_Periculosidade");
            entity.HasIndex(e => e.Nivel).IsUnique();
            entity.Property(e => e.Nome).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.CorAlerta).HasMaxLength(7).IsUnicode(false);
        });

        modelBuilder.Entity<Laboratorio>(entity =>
        {
            entity.ToTable("Laboratorio");
            entity.HasIndex(e => e.Codigo).IsUnique();
            entity.Property(e => e.Codigo).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.Nome).HasMaxLength(255);
            entity.Property(e => e.Sitio).HasMaxLength(100);
            entity.Property(e => e.Responsavel).HasMaxLength(255);
            entity.Property(e => e.Especialidade).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.Status).HasMaxLength(20).IsUnicode(false).HasDefaultValue("ATIVO");
            entity.Property(e => e.DataCriacao).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.DataAtualizacao).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
        });

        modelBuilder.Entity<ProtocoloContencao>(entity =>
        {
            entity.ToTable("ProtocoloContencao");
            entity.HasIndex(e => e.Codigo).IsUnique();
            entity.Property(e => e.Codigo).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.Titulo).HasMaxLength(255);
            entity.Property(e => e.ClassesAplicaveis).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.DataCriacao).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.DataAtualizacao).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
        });

        modelBuilder.Entity<ProtocoloAplicadoEm>(entity =>
        {
            entity.HasKey(e => new { e.ProtocoloId, e.AnomaliaId });
            entity.ToTable("Protocolo_AplicadoEm");
            entity.Property(e => e.Status).HasMaxLength(20).IsUnicode(false).HasDefaultValue("ATIVO");
            entity.Property(e => e.DataInicio).HasColumnType("datetime");
            entity.Property(e => e.DataFim).HasColumnType("datetime");

            entity.HasOne(d => d.Anomalium).WithMany(p => p.ProtocoloAplicadoEms)
                .HasForeignKey(d => d.AnomaliaId)
                .HasConstraintName("FK_Protocolo_AplicadoEm_Anomalia");

            entity.HasOne(d => d.ProtocoloContencao).WithMany(p => p.ProtocoloAplicadoEms)
                .HasForeignKey(d => d.ProtocoloId)
                .HasConstraintName("FK_Protocolo_AplicadoEm_Protocolo");
        });

        modelBuilder.Entity<NotificacaoAnomalia>(entity =>
        {
            entity.ToTable("NotificacaoAnomalia");
            entity.HasIndex(e => e.Status, "IX_NotificacaoAnomalia_Status");
            entity.HasIndex(e => e.DataHora, "IX_NotificacaoAnomalia_Data");
            entity.Property(e => e.Titulo).HasMaxLength(255);
            entity.Property(e => e.LocalIdentificado).HasMaxLength(255);
            entity.Property(e => e.DataHora).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(20).IsUnicode(false).HasDefaultValue("PENDENTE");
            entity.Property(e => e.Relator).HasMaxLength(255);
            entity.Property(e => e.DataResolucao).HasColumnType("datetime");

            entity.HasOne(d => d.Anomalium).WithMany(p => p.NotificacaoAnomalias)
                .HasForeignKey(d => d.AnomaliaId)
                .HasConstraintName("FK_NotificacaoAnomalia_Anomalia");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
