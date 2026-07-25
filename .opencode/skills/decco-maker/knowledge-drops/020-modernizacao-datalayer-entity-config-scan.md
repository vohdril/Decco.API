# 020 — Modernização do DataLayer: `IEntity` + `Set<T>` + `ApplyConfigurationsFromAssembly`

- **Data de registo:** 2026-07-23
- **Fonte:** Análise profunda do DeccoDbContext (627 linhas monolíticas) + decisão do operador de implementar Abordagem 2
- **Tipo:** padrão novo
- **Afeta:** `reference/02-conexao-banco-e-config.md` (indiretamente — o padrão de DbContext mudou)
- **Camada:** Decco.API (core)

## O padrão / a mudança

O `DeccoDbContext` foi refatorado de **627 linhas monolíticas** para **~15 linhas**. As três peças da Abordagem 2:

### 1. Interface `IEntity` (marcador)

`src/Decco.Api.DataLayer/Models/IEntity.cs`:
```csharp
namespace Decco.Api.DataLayer.Models;
public interface IEntity { }
```

Toda entidade de tabela implementa `IEntity`:
```csharp
public partial class Anomalium : IEntity { ... }
```

**Não implementam:** `Vw*` (views keyless).

### 2. `Set<T>()` no lugar de `DbSet<T>` properties

Repositórios antes:
```csharp
_ctx.Anomalia                          // DbSet property
_context.CatCamadaOntologicas
```

Repositórios depois:
```csharp
_ctx.Set<Anomalium>()                  // runtime-resolved
_context.Set<CatCamadaOntologica>()
```

O DbContext perdeu **todas** as 24 propriedades `DbSet<T>`.

### 3. `IEntityTypeConfiguration<T>` + `ApplyConfigurationsFromAssembly`

Cada entidade ganhou uma classe de configuração separada em `Configurations/`:

```csharp
public class AnomaliumConfiguration : IEntityTypeConfiguration<Anomalium>
{
    public void Configure(EntityTypeBuilder<Anomalium> builder) { ... }
}
```

O `OnModelCreating` encolheu para:
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeccoDbContext).Assembly);
    OnModelCreatingPartial(modelBuilder);
}
```

As 24 classes de configuração (21 tabelas + 3 views) vivem em `src/Decco.Api.DataLayer/Configurations/`.

## Porquê

**Problema original:** a cada nova entidade, o dev precisava abrir o `DeccoDbContext.cs` (627 linhas) e mexer em **2 lugares**: adicionar `DbSet<T>` + escrever `modelBuilder.Entity<T>(...)`. Isso não escala, viola SRP e é fonte garantida de conflitos de merge.

**Alternativas rejeitadas:**
- **Abordagem 1 (só extrair configs):** mantém `DbSet<T>` props — ainda exige 1 toque no DbContext por entidade nova.
- **Abordagem 3 (repositório genérico + IRepository\<T\>):** mais DRY, mas exigiria substituir todos os repositórios manuais (incluindo chamadas a SPs via Dapper) — overkill para o Tier 0/1.

**Abordagem 2 escolhida:** elimina **toda** dependência do DbContext para adicionar entidades novas. Basta:
1. Criar a classe model implementando `IEntity`
2. Criar a classe `IEntityTypeConfiguration<T>` em `Configurations/`
3. (opcional: criar repositório, serviço, controller — que já são registrados por convenção via `CompositionRoot`)

**O que quebraria se fosse diferente:** sem `ApplyConfigurationsFromAssembly`, toda entidade nova exigiria editar o `OnModelCreating` — exatamente o problema que queremos resolver.

## Como aplicar a partir de agora

1. **Entidades de tabela** (com chave primária): implementar `IEntity`, criar `IConfiguration` em `Configurations/`.
2. **Views:** NÃO implementam `IEntity`; ainda precisam de configuração (`.HasNoKey().ToView(...)`) mas a classe de config vai em `Configurations/` igual.
3. **Repositórios:** usar `_ctx.Set<T>()` em vez de `_ctx.PropriedadeDbSet`.
4. **DbContext:** não mexer — `OnModelCreating` já descobre tudo via assembly scan.

## Fio a puxar

O `CompositionRoot` já registra `IService` e `IRepository` por convenção (assembly scan). Dá para criar um `IRepository<T>` genérico que cubra CRUD básico, e deixar os repositórios manuais só para operações com SPs? Isso reduziria ainda mais a cerimônia de uma entidade nova: model → config → (nada) → controller usa service genérico.
