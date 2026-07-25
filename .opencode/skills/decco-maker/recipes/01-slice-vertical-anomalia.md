# recipes/01 — Slice vertical (trilha de aprendizagem)

> Percurso progressivo para construir o sandbox **entendendo**, não só copiando. Cada etapa tem um **ponto de verificação**.
> A entidade canônica é a **Anomalia** (como o Extra é para o Partners).

> **Alinhamento com os tiers (`reference/11`) e o molde:** Etapa 0-2 = **🟢 Tier 0** (envelope + Anomalia ponta a ponta),
> Etapa 1/2 encostam no **🟡 Tier 1** (catálogos/IdToCode/cache), Etapa 5 = **🔵 Tier 2** (escala). O **molde de estrutura** é o
> **`OB.API.PAR`** (`reference/10`): solução em 5 pastas, envelope enxuto, controller fino → manager → UoW → repo. Faça a Anomalia
> primeiro no track **moderno** (Decco.API, EF Core — `reference/09`) e, para comparar, no **legado** (Decco.Legacy.API, EF6 —
> `reference/07`/`08`). **Um tier por vez**; não gerar tudo de uma vez.

> **Front-end junto:** quando este Tier-0 é disparado, o **FE Tier-0 sai junto** — um console executável em **modo mock** (sem
> back), já com o **seam mock↔Decco.API**. Ver `recipes/04-frontend-tier0.md` e `reference/16`.

### Setup (Etapa −1) — Banco database-first (a skill NÃO gera o schema do domínio)
Antes das etapas: **criar o DeccoDB rodando `assets/decco.sql`** (embutido na skill) — o banco **já existe** (database-first).
Depois, obter as entidades por **`dotnet ef dbcontext scaffold`** (gera as classes C# a partir do banco → **OOP preservada**) ou mapear à mão a `Anomalia`+`Cat_*`. As **SPs** (`sp_Anomalia_*`) vão por **Dapper** (ver `reference/14`). Auth (mais tarde) é **code-first** (`recipes/02`). Ver `knowledge-drops/009`.
✅ **Verificação:** app conecta e faz um `SELECT` numa tabela via EF.

## A trilha (do mais simples ao mais rico)

### Etapa 0 — O envelope (`Decco.Contracts`)
Criar `RequestBase`/`ResponseBase` + `SingleRequest<T>`/`SingleResponse<T>` + `ListRequest`/`ListResponse<T>` + `Status`/`Error`.
✅ **Verificação:** um teste que serializa/desserializa um `ListResponse<Anomalia>` com `TotalRecords` e `Errors`.
🔎 *Porquê primeiro:* tudo referencia o envelope; é o "protocolo" comum.

### Etapa 1 — Catálogo read-only (exercita IdToCode de leitura)
- **Core:** `Decco.API` expõe `POST /api/Catalogos/ListClasses` etc. — repositório EF Core sobre `Cat_*` + `ICacheProvider` (cache-aside).
- **Fachada:** repositório **global read-only** consumindo esses endpoints; `CatalogHelper` monta `ClasseIdToCodigo`…
✅ **Verificação:** `GET /api/anomalias` (ainda vazio) já resolve nomes de catálogo; o cache é usado no 2º request.
🔎 *Fio:* meça o hit-ratio do cache; o que acontece se o core cair (fail-safe)?

### Etapa 2 — Anomalia, slice completo (a espinha)
Ordem: **contratos DTO V2 (13)** → **validadores** → **conversores Receive/List** → **repositório Remake (fachada)** +
**manager + repositório híbrido EF/Dapper (core, chamando `sp_Anomalia_*`)** → **serviço-envelope** → **controller V2**.
Com **IdToCode/CodeToId** nos dois sentidos (`reference/04`).
> **Rampa didática do core (EF antes de Dapper):** **2a)** faça o CRUD da Anomalia primeiro **só com EF Core/LINQ** (conforto OOP);
> **2b)** só então troque a List por `sp_Anomalia_Buscar` e adicione `sp_Anomalia_ObterPerfilCompleto` **via Dapper** (`reference/14`) —
> é o estudo de stored procedures. Não misture os dois no primeiro passo.
✅ **Verificação:** criar uma Anomalia via `POST /api/anomalias` com `classeCodigo:"YAGUARA"` e ler de volta com o código certo;
conferir no banco que gravou o `Id` correto.
🔎 *Fio:* onde colocar a regra do trigger `TR_Anomalia_Validar_Mecanismos` — banco, manager ou validador?

### Etapa 3 — Coleção N:N (`Pericia_Manifestacao`) → delta-merge
✅ **Verificação:** um Update que remove uma manifestação re-emite `IsDeleted=true` e o core apaga.

### Etapa 4 — Sub-agregados 1:N (`EntidadeViva`/`Artefato`/…)
✅ **Verificação:** `sp_Anomalia_ObterPerfilCompleto` (multi-result set via Dapper `QueryMultiple`) monta o perfil.

### Etapa 5 — Avançado
Referência polimórfica (`Instancia_PericiaDesviante`); pseudo-tenant por `SitioContencao` (reencontra `EsMappingType`);
views como read-models; **Docker/compose** (Decco.API + Foundation.API + SQL Server + Redis).

## Regra de emissão de código (quando gerar)
Emitir a **estrutura padrão completa** (heranças, envelope, ganchos do repositório, actions do controller). A **lógica
específica de campo** só quando os campos forem fornecidos; senão, marcar `// >>> <o quê> (investigar: <onde/análogo/busca>)`.
Ao fim, **listar** os `// >>>` deixados e **um fio a puxar**.
