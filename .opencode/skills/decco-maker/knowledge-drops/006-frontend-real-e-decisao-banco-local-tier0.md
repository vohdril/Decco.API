# 006 — Front-end real da Omnibees + decisão de banco local (Tier 0) + trilha futura de front-end

- **Data de registo:** 2026-07-13
- **Fonte:** varredura read-only de C:\Git + os fontes do OneDrive (`omni-src`) + `ob-skills`; pergunta do Paulo antes do Tier 0
- **Tipo:** conhecimento resolvido (front-end) + decisão de rumo (banco local / trilha FE)
- **Afeta:** reference/02 (banco), reference/11 (tiers — nova trilha FE), escopo do Tier 0
- **Complementa:** `knowledge-drops/004` (que já concluíra "backend-only")

## A — Front-end: OB.API funciona como o Partners (API-only; front em OUTRO projeto)
Verificado no código disponível:
- **OB.API é API-only** — não hospeda SPA/MVC. **Igual ao Partners.** O front-end de produto vive em **repositórios separados que NÃO estão no disco** → daqui **não** dá para afirmar quais frameworks os apps atuais usam.
- **Único artefato "front" presente:** `Main\Clients\OB.API.Clients.Angular` = **SDK-cliente gerado por nswag** a partir do Swagger da API (`ob-api-clients`, `@angular/core ^4.4.3`, `rxjs ^5`, TypeScript 2.5, build via rollup), publicado num **npm privado** (`dev-env.visualforma.pt/npm/`). É **biblioteca de chamadas à API**, não uma aplicação; está **fora de qualquer `.sln`**; idêntico no `bhi-ob-api` e no snapshot `OB.API`.
- **Micro-frontends:** **nenhum sinal** em todo o material (sem module-federation, single-spa, Nx, qiankun, piral, import-maps). → **não confirmável** que a Omnibees use micro-frontends; sem evidência no código disponível.
- **Pista de framework (só pista):** o SDK gerado é **Angular** (v4, antigo/2017). O Paulo declara domínio de **Angular e React**. Os apps atuais podem ser mais novos — **não verificável** aqui.

**Padrão importante (esse sim, replicável):** o contrato front↔back na Omnibees é **API expõe Swagger/OpenAPI → gera-se um SDK-cliente (nswag) → o front consome**. É assim que um futuro front do Decco se conectaria.

> **Porquê isso importa:** confirma que o Decco pode nascer **backend-only** sem perder nada — o front é sempre um projeto à parte que consome a API. E dá o caminho de integração (OpenAPI + client gerado) para quando o estudo de front entrar.

## B — DECISÃO: banco local prático para o Tier 0 da Decco.API
Requisito do Paulo: rodar a Decco.API com **banco local, prático de alimentar**.
- **Motor:** **SQL Server** (não SQLite) — o `decco.sql` usa recursos T-SQL que só o SQL Server tem: **stored procedures** (centrais no Tier 0), **triggers**, **views**, `GEOGRAPHY`, `FOR XML PATH`, `OFFSET/FETCH`, `IDENTITY`. Opções locais: **Docker** (`mcr.microsoft.com/mssql/server`) — recomendado, isolado e reproduzível — **ou LocalDB** (Windows, sem Docker).
- **Criação/seed:** **database-first** — rodar o `decco.sql`, que já cria `DeccoDB` **com schema + SPs + views + triggers + 2 anomalias de exemplo**. **Não** usar `EnsureCreated`/migrations para o core: EF **não** recria SPs/triggers/views (resolve parte da `open-questions/Q-002` — para o **core**, database-first vence porque o valor está nas SPs).
- **Alimentar de forma prática (além do seed):** (1) os próprios endpoints da Decco.API assim que o Tier 0 rodar (`InsertAnomalia`); (2) um `seed.sql` extra com mais anomalias fictícias (SCP-style); (3) SSMS/Azure Data Studio para inserts rápidos. Para reprodutibilidade: `docker-compose` (SQL Server + volume + passo de init que roda `decco.sql`) — deixa "subir e alimentar" num comando (isto é 🟡/🔵 tier; no Tier 0 basta rodar o `.sql` uma vez).
- **EF Core sobre um banco que já existe:** usar **`dotnet ef dbcontext scaffold`** (gera entidades a partir do `DeccoDB`) **ou** mapear à mão só a `Anomalia`+`Cat_*` no Tier 0; as SPs vão por **Dapper** (não pelo scaffold).

## C — Trilha FUTURA de front-end (documentada, fora do Tier 0)
Manter a API **front-end-ready** a custo ~zero desde já, sem construir front agora:
- **OpenAPI/Swagger ligado** (já previsto no `Decco.Api.REST`/`Foundation.Api.REST`) + **CORS** configurável → um front futuro anexa trivialmente.
- **Superfície natural para o front = a `Foundation.API`** (REST público V2, DTOs, validação), não o envelope do core. Um estudo de front consome a Foundation (ou, para simplificar, a Decco.API direto).
- Quando o estudo de FE entrar (🔵 tier futuro): decidir **framework** (Angular vs React — Paulo faz os dois) e **se** explorar **micro-frontends** (module federation) — hoje **sem análogo confirmado** na Omnibees. Registado como `open-questions/Q-006`.

## Como aplicar
- **Tier 0 não inclui front-end** (backend-only, como o OB.API). Ao gerar a Decco.API, **ligar Swagger + CORS** para não fechar a porta do FE futuro, mas **não** criar projeto de front.
- Banco: assumir **SQL Server local (Docker/LocalDB) + `decco.sql`** por defeito; SPs via Dapper.
- Se o Paulo perguntar de front-end de produto Omnibees: responder o que é **verificável** (API-only, SDK Angular gerado, sem micro-frontend à vista) e **sinalizar** que os apps reais estão fora do disco (oferecer analisar se ele trouxer os repos), **sem inventar** frameworks.

## Fio a puxar
Quando quiser abrir a trilha FE: gerar um SDK-cliente TypeScript da Decco.API via **NSwag/OpenAPI Generator** (o mesmo padrão do `OB.API.Clients.Angular`) e montar um front mínimo (Angular **ou** React) que liste anomalias — decidir framework em `open-questions/Q-006`.
