# 18 — Database Tier (DB0-4): modelagem de dados como track independente

> ⚠️ Atualização (2026-07-25): DB1 redefinido como **"Docker + tabelas de autenticação"** (o passo real após o modelo-template).
> DB0 agora é o schema SQL que já existe nos repositórios taggeados. Ver `knowledge-drops/024`.

> Expansão da antiga track "Persistência" (transversal) para uma track **DB** própria, com progressão do SQL relacional ao
> ecossistema poliglota. A modelagem de dados — tabelas, índices, SPs, migrations, e depois NoSQL, busca, cache — merece
> progressão própria porque **o schema é a fundação do sistema inteiro** e cada tecnologia resolve um problema diferente.

## Mapa de tiers

| Tier | Foco | Tecnologia | O que se aprende |
|------|------|-----------|------------------|
| **DB0** | Schema SQL + lore brasileiro | SQL Server (LocalDB) | Modelagem relacional, FKs, índices, SPs, triggers, views, database-first |
| **DB1** | Docker + Auth tables | Docker Desktop + SQL Server container + EF Migrations | Containerizaçao, tabelas de usuário/role/permissão, code-first |
| **DB2** | CRUDs de catálogo + telas | ASP.NET Core + React/Vite | CRUD completo de tabelas de referência, telas de configuração |
| **DB3** | NoSQL + Busca | MongoDB ou Couchbase + Elasticsearch | Desnormalização, agregados, CDC, busca textual |
| **DB4** | Poliglota aplicado ao domínio | Redis/Vault/Kafka | Cache de catálogos, eventos de domínio, segredos |

## DB0 — SQL Schema + Lore Brasileiro (modelo-template)

### O que o lore adicionou ao schema

| Conceito | Origem | Tabela |
|----------|--------|--------|
| Cognição Aparente | `Sistema Brasileiro.txt` | `Cat_CognicaoAparente` (SE/SA/IN/AA) |
| Periculosidade | `Sistema Brasileiro.txt` | `Cat_Periculosidade` (9 níveis) |
| Identificador OA | `Sistema Brasileiro.txt` | Gerado na app: `OA-[####][CF\|CN\|CI\|CMF]-[SE\|SA\|IN\|AA]` |
| Laboratórios | Operador | `Laboratorio` |
| Protocolos de Contenção | Operador | `ProtocoloContencao` + `Protocolo_AplicadoEm` (N:N) |
| Notificações | Operador | `NotificacaoAnomalia` |

### Mapa completo do DeccoDB (pós DB0)

| Grupo | Tabelas | Tipo |
|-------|---------|------|
| Catálogos | `Cat_ClasseObjeto`, `Cat_ForcaFundamental`, `Cat_CamadaOntologica`, `Cat_TipoMateria`, `Cat_MecanismoInteracao`, `Cat_ManifestacaoEspecifica`, `Cat_CognicaoAparente`, `Cat_Periculosidade` | read-only + IdToCode |
| Agregado principal | `Anomalia` | entidade-raiz |
| 1:N | `EntidadeViva`, `Artefato`, `Localidade`, `Evento` | coleções filhas |
| N:N | `Pericia_Manifestacao`, `Protocolo_AplicadoEm` | relacionamentos |
| Perícias | `PericiaAnomalia`, `Instancia_PericiaDesviante` | sistema de herança |
| Histórico | `Incidente` | log de eventos |
| Configuração | `Laboratorio`, `ProtocoloContencao`, `NotificacaoAnomalia` | backoffice |

### Sinais de diagnóstico (rubrica DB0)

| id | Sinal decisivo | Descrição |
|----|---------------|-----------|
| `cognicao-tabela` | `Cat_CognicaoAparente` existe com SE/SA/IN/AA | Tabela de cognição aparente criada e populada |
| `periculosidade-tabela` | `Cat_Periculosidade` existe com 9 níveis | Tabela de periculosidade criada e populada |
| `anomalia-oa` | `Anomalia` tem colunas `CognicaoAparenteId` e `PericulosidadeId` | FKs das novas classificações |
| `sps-novas` | SPs `sp_Laboratorio_Inserir`, `sp_ProtocoloContencao_Inserir`, `sp_NotificacaoAnomalia_Inserir` | Stored procedures das novas entidades |

## DB1 — Docker + Auth Tables (v0.2.0, o passo real após modelo-template)

O Tier-0 usa **LocalDB** (SQL Server local). O Tier-1 migra para **Docker** e adiciona **tabelas de autenticação**.

### Docker Compose

```yaml
# docker-compose.yml na raiz do Decco.API
services:
  sql:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      ACCEPT_EULA: Y
      SA_PASSWORD: "Decc0@Dev2024!"
    ports:
      - "1433:1433"
    volumes:
      - decco-data:/var/opt/mssql

volumes:
  decco-data:
```

### O que Docker ensina
- Container vs imagem vs volume
- Port mapping (`1433:1433`)
- Connection string apontando para `localhost,1433` (SQL Server no container)
- `docker compose up -d` / `docker compose down`
- Ver logs: `docker compose logs sql`
- Resetar banco: `docker compose down -v && docker compose up -d`

### Tabelas de Autenticação (code-first, EF Core)

```csharp
public class User {
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";  // PBKDF2
    public int Clearance { get; set; }   // nível de acesso 1-5
    public int SiteId { get; set; }      // sítio de lotação
    public bool IsActive { get; set; } = true;
}

public class Role {
    public int Id { get; set; }
    public string Name { get; set; } = "";  // "Researcher", "Director", "O5"
}

public class UserSite {
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SiteId { get; set; }
    // Usuário pode ter acesso a múltiplos sítios
}
```

### Sinais de diagnóstico (rubrica DB1)

| id | Sinal decisivo | Descrição |
|----|---------------|-----------|
| `docker-sql` | `docker-compose.yml` com serviço SQL Server + container rodando | Docker configurado e operacional |
| `user-table` | Entidade `User` com `Username`, `PasswordHash`, `Clearance`, `SiteId` | Tabela de usuários criada |
| `role-table` | Entidade `Role` com `Name` | Tabela de papéis criada |
| `usersite-table` | Entidade `UserSite` com FK para User e Site | Tabela de vínculo usuário-sítio criada |
| `auth-seed` | Seed data com `HasData` no DbContext para User/Role/UserSite | Dados iniciais de autenticação |

## DB2 — CRUDs de Catálogo + Telas

### Endpoints necessários
- `Cat_CognicaoAparente` — CRUD completo (GetAll, GetById, Insert, Update, Delete)
- `Cat_Periculosidade` — CRUD completo
- `Laboratorio` — CRUD + listagem por sítio
- `ProtocoloContencao` — CRUD + vincular/desvincular anomalias
- `NotificacaoAnomalia` — listar, atualizar status, resolver

### Telas necessárias
- **Catálogos**: tela de administração de tabelas de referência (padrão CRUD com grid + modal)
- **Laboratórios**: listagem + formulário de cadastro
- **Protocolos**: editor de passos (textarea com markdown ou steps numerados)
- **Notificações**: listagem com badge de prioridade, botão "resolver"

## DB3 — NoSQL + Busca

### Documento Agregado (MongoDB ou Couchbase)
O perfil completo de uma anomalia é um **documento agregado** que reúne:
- Dados básicos da Anomalia
- Instâncias (Entidades, Artefatos, Localidades, Eventos)
- Perícias + Manifestações
- Protocolos aplicados
- Incidentes

### Elasticsearch
Indexar anomalias, laboratórios, protocolos e notificações para busca full-text.
Usar **CDC** (Change Data Capture) ou **SaveChangesInterceptor** para manter o Elasticsearch sincronizado.

## DB4 — Poliglota Aplicado ao Domínio

### Cache de Catálogos (Redis + FusionCache)
Os catálogos (`Cat_*`) são **read-heavy**, mudam raramente. Cachear com FusionCache:
- L1 (MemoryCache) + L2 (Redis) + backplane
- Invalidação por push quando um catálogo é alterado

### Eventos de Domínio (Kafka)
Publicar eventos quando:
- Uma anomalia é criada/atualizada → `AnomaliaRegistrada`, `AnomaliaReclassificada`
- Um protocolo é aplicado → `ProtocoloAtivado`
- Uma notificação é gerada → `NotificacaoCriada`
- Um incidente Sigma ocorre → `IncidenteSigmaRegistrado`

### Segredos (Vault)
Connection strings, tokens de API e senhas saem do `appsettings.json` para o Vault.

## Fio a puxar

**DB0 → DB1 (Docker):** o projeto hoje usa LocalDB. Suba um container SQL Server com `docker compose up -d`, mude a
connection string para o container, e veja o dashboard continuar funcionando. Esse é o primeiro contato prático com
containerização — e o pré-requisito para o login real (as tabelas de auth serão criadas via migration no mesmo container).
Depois compare: o que muda na inicialização? No desempenho? Na portabilidade?
