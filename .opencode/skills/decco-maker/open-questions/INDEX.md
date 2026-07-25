# Open Questions — investigações pendentes (decco-maker)

> A inovação didática desta skill. Aqui ficam as **dúvidas em aberto** — o que ainda **não** se sabe/decidiu. É o oposto do
> `knowledge-drops/` (que guarda conhecimento **resolvido**). Instiga pesquisa em vez de fingir certeza.

## Como funciona
- Cada questão é uma entrada `Q-NNN` com: **pergunta**, **porquê importa**, **como investigar** (pista concreta), **estado**.
- Quando uma questão é **resolvida**, escrever a resposta, mudar o estado para `RESOLVIDA`, e **promover** o achado para um
  `knowledge-drop` (ou `reference/`). A entrada fica como histórico (não se apaga).
- A skill deve, proativamente, **abrir** uma `Q-NNN` quando topar com uma zona não coberta — e convidar o Paulo a investigar.

## Template
```markdown
### Q-NNN — <pergunta em uma linha>
- **Porquê importa:** <impacto na decisão de desenho>
- **Como investigar:** <ficheiro/comando/experimento/fonte concretos>
- **Estado:** ABERTA | EM INVESTIGAÇÃO | RESOLVIDA (→ ver knowledge-drops/NNN)
```

## Backlog inicial (semeado a partir das análises)
### Q-001 — `Decco.Contracts` deve ser projeto referenciado ou pacote NuGet?
- **Porquê importa:** define o acoplamento fachada↔core e reflete (ou não) a realidade Omnibees (onde `OB.BL.Contracts`/`Partners.Dtos` são pacotes).
- **Como investigar:** comparar DX de `ProjectReference` local vs empacotar/consumir um `.nupkg` local (feed de pasta). Ler como o `partners-api` referencia `Partners.Dtos` (versão fixada no `.csproj`).
- **Estado:** ABERTA

### Q-002 — Database-first (scaffold do DeccoDB) vs code-first (migrations) para a Decco.API?
- **Porquê importa:** o OB.API é database-first (EDMX); o Partners quase não usa banco. Qual ensina mais e serve melhor um banco que já existe (`decco.sql`)?
- **Como investigar:** scaffoldar o DeccoDB com `dotnet ef dbcontext scaffold` e, em paralelo, tentar code-first + `EnsureCreated`. Comparar fidelidade (SPs/triggers/views não vêm no scaffold).
- **Estado:** RESOLVIDA (→ `knowledge-drops/009`): decisão **HÍBRIDA** — **domínio (Decco.API) database-first** (rodar `assets/decco.sql`; scaffold gera as entidades e **preserva OOP**; SPs via Dapper — `reference/14`) + **auth code-first** (`recipes/02`). Fiel ao ecossistema (que é database-first e por isso evita Identity). A comparação **didática** EF/LINQ vs SP/Dapper fica como **exercício** dentro do slice (não como decisão em aberto).

### Q-003 — Onde aplicar a regra do trigger `TR_Anomalia_Validar_Mecanismos`?
- **Porquê importa:** a mesma regra pode viver no banco (trigger), no manager do core, ou no validador da fachada — cada um com trade-offs de acoplamento/testabilidade.
- **Como investigar:** desativar o trigger e reproduzir a regra em C#; comparar mensagens de erro que chegam ao cliente pelo envelope.
- **Estado:** ABERTA

### Q-004 — Como expor `Instancia_PericiaDesviante` (referência polimórfica) sem vazar Id?
- **Porquê importa:** discriminador `TipoInstancia`+`InstanciaId` não tem análogo direto no Partners.
- **Como investigar:** pesquisar "polymorphic association" e TPH/TPT do EF Core; esboçar 2 shapes de DTO e comparar.
- **Estado:** ABERTA

### Q-005 — Decco.Legacy.API: como obter o EDMX sem o designer do Visual Studio, e o que fazer com o ServiceStack.Redis EOL?
- **Porquê importa:** fidelidade ao legado (EF6 database-first via `.edmx`) esbarra no ferramental (o designer é do VS); e `ServiceStack.Redis 3.9.71` é EOL — replicar tal-e-qual vs manter só a *forma*.
- **Como investigar:** (a) testar gerar o EDMX no VS vs `EdmGen`/T4 manual vs aproximar com EF6 **code-first** (perde `.edmx`, mantém EF6); (b) decidir entre fixar `ServiceStack.Redis 3.9.71` (fiel, EOL) ou manter o `ICacheProvider`+pool+failover com **StackExchange.Redis** por baixo (forma fiel, backend vivo). Ver `reference/07` §"Nota EOL/ferramental".
- **Estado:** ABERTA

### Q-006 — Trilha de front-end: qual framework e (se) micro-frontends?
- **Porquê importa:** o estudo de FE entra num horizonte mais longo; a decisão define como o front consome a API e o que se aprende. A Omnibees é **API-only** com SDK-cliente **Angular** gerado (nswag) — mas os apps reais não estão no disco e **não há sinal de micro-frontend** (ver `knowledge-drops/006`).
- **Como investigar:** (1) gerar um SDK-cliente TypeScript da Decco.API/Foundation via **NSwag/OpenAPI Generator** e montar um front mínimo que liste anomalias; (2) decidir **Angular vs React** (o Paulo faz os dois) — fazer o mesmo mini-front nos dois e comparar; (3) só depois avaliar **micro-frontends** (module federation) — sem análogo confirmado na Omnibees, é estudo exploratório. Pré-requisito: API com **OpenAPI + CORS** ligados (garantir no Tier 0).
- **Estado:** **PARCIALMENTE RESOLVIDA** (→ `knowledge-drops/012`, `reference/16`).
  - **Framework — decidido: React** (Vite + React 19 + TS). Existe **FE Tier-0 executável** carregado em `assets/frontend-tier0/` (stack Radix Dialog + TanStack Query + rhf/zod; design system JARVIS por tokens; estados Skeleton/Loading/Empty/Error; authz na UI por clearance+sítio; sandbox editável; esfera em canvas 2D). *Angular fica como comparação opcional (o Paulo faz os dois) — não bloqueia.*
  - **Seam mock↔live já montado** (→ `knowledge-drops/013`): a alternância entre mock e Decco.API está **implementada** na referência
    (contrato `DeccoApi` + factory + toggle por env/localStorage + esqueleto `data/httpApi.ts`). Decisão: **gateway+factory** no Tier-0
    (zero deps), **MSW** como upgrade.
  - **Ainda ABERTO:** (a) **completar `data/httpApi.ts`** contra um back rodando e/ou gerar o **SDK por NSwag** (é o *flip* do FE Tier 1
    — `reference/16`/`recipes/04`); pré-requisito no back = **OpenAPI + CORS**. (b) **Micro-frontends** (Module Federation) — segue
    **exploratório**, sem análogo Omnibees confirmado (FE Tier 4).

### Q-007 — As tabelas de auth ficam no `DeccoDB` ou num banco/contexto separado?
- **Porquê importa:** o Partners guarda a `ApiPermission` no banco DELE (conector), separado do banco de domínio; misturar auth com o domínio no `DeccoDB` é mais simples mas acopla ciclos de vida distintos.
- **Como investigar:** montar a `ApiPermissions` do conector como contexto próprio (EF code-first) e as tabelas de user/role/clearance no core (`DeccoDB` ou `DeccoAuthDB`); comparar clareza de migração/seed e o acoplamento. Ver `reference/12` §2 e `reference/13` §4.
- **Estado:** ABERTA (decidir ao construir o vertical de auth — `recipes/02`)

### Q-008 — Emissor de token: JWT de dev vs IdentityServer/Keycloak local?
- **Porquê importa:** o Decco constrói o pedaço que a Omnibees terceiriza (AuthServer); a escolha afeta fidelidade vs simplicidade e o que se aprende de OIDC.
- **Como investigar:** começar com JWT de dev assinado (HS256, chave em user-secrets — `recipes/02` Etapa B); depois trocar por **Duende IdentityServer** ou **Keycloak** local e ver o que muda no cliente (fluxo OIDC, `Authority`, validação). ⚠️ **Não** confundir com ASP.NET Identity (user-store) — é abordagem diferente (`reference/12`).
- **Estado:** ABERTA (trilha do vertical de auth)

> Próximo número livre: **Q-009**.
