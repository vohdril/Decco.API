# 005 — Fontes dissecados (Core/Base), PAR como molde, e foco em camadas

- **Data de registo:** 2026-07-13
- **Fonte:** o Paulo disponibilizou os fontes das DLLs em `OneDrive\...\Documentos\Omnibees\*.zip`; dissecação read-only + decisão de foco
- **Tipo:** conhecimento resolvido + decisão de rumo (⚠️ atualiza o mapa de fontes de `004` e o método de trabalho)
- **Afeta:** reference/07, reference/08, reference/09, reference/10, reference/11; método de geração (tiers)

## A — Fontes agora DISPONÍVEIS (atualiza o mapa de `004`)
Extraídos em `scratchpad/omni-src/` (cópia de trabalho; zips originais intactos):
- ⭐ **`OB.Api.Core`** (`bhi-core-api.zip`) — o "buraco" de `reference/07` **agora tem fonte real**: `IObjectContext`/`ISessionFactory`/`IUnitOfWork`+`UnitOfWorkBase`/`DomainScope`, padrão nested-scope `AsyncLocal`+shell, `DataLayerException`+`SqlToDataLayerException`, e (escala) `DatabaseDiscovery/RoundRobin` (réplicas RO). Dossiê em `reference/08`. Correção: `BusinessLayerException`/`BusinessRulesInterceptionBehavior` **não** estão aqui (são da BL/AOP do `bhi-ob-api`, `reference/05`).
- ⭐ **`OB.Api.Base.Abstractions/DataLayer/DataLayer.EF`** — base **moderna** (EF Core 8): Criteria, UoW/SessionFactory, `SqlRepositoryBase<T>`, auto-registo. Dossiê em `reference/09`. **Só-DLL ainda:** DI.AspNetCore(.Caching), BusinessLayer, Contracts(.Validation), Helpers concreto, `IObHttpClient` concreto, motor de conversores concreto.
- **`OB.API`** (monólito) e ⭐ **`OB.API.PAR`** (serviço de domínio único). Envelope real + molde PAR em `reference/10`.
- Dependência dura descoberta: o motor de query moderno usa **`OB.DynamicLinqCore`** (`DataSourceRequest`/`ToDataSourceResult`).

**Regra:** o Decco **reimplementa** o mínimo (não depende dos pacotes `OB.*` do feed corporativo); usa os fontes como **referência fiel**. Onde há fonte (`OB.BL.Contracts` in-tree, `OB.PAR.Contracts`, Core, Base parcial), espelhar; onde não há, reimplementar.

## B — PAR é o MOLDE escolhido (não o monólito)
`OB.API.PAR` é o mesmo ADN do OB.API destilado a **1 domínio**: envelope próprio mínimo (`OB.PAR.Contracts`, 35 ficheiros vs 2.059), **1 `DomainScope`**, 6 repositórios, 2 controllers, ponta-a-ponta completo. → **O Decco replica PAR, não o monólito.** Resolve o medo de "projeto espelhado sem estudo". Detalhe/checklist em `reference/10`.

## C — DECISÃO DE MÉTODO: foco em 3 tiers (linha não-destrutiva)
Para não perder o foco, o trabalho segue **tiers** (detalhe + tabela em `reference/11`):
- 🟢 **Tier 0 (agora):** 1 entidade (`Anomalia`) ponta-a-ponta, molde PAR — envelope + 1 DbContext/scope + manager + repo (CRUD + 1 SP) + host + DI. Sem cache/auth/observabilidade/eventos.
- 🟡 **Tier 1:** IdToCode + conversores + Criteria + cache de catálogos + validação (+ fachada quando quiser).
- 🔵 **Tier 2 (escala global — visão MIB/SCP):** réplicas RO/RoundRobin, cache distribuído, observabilidade/Docker, eventos, multi-scope, auth/Vault/K8s.

**Regras de foco:** um tier por vez, checkpoint funcional antes de avançar; o que é de tier futuro fica **documentado, não gerado**; marcar ganchos de escala com `// >>> (escala, Tier 2: ver reference/NN)`; a **visão lúdica global** entra como **motivação**, não como escopo.

## Como aplicar
- Ao gerar: **default = Tier 0 na Anomalia, molde PAR**, salvo pedido explícito de subir de tier. Nunca gerar o projeto inteiro de uma vez.
- Ao explicar uma tecnologia de Tier 2, enquadrar pela visão ("por que isto importaria num Decco global") e apontar o dossiê, **sem** construir.

## Fio a puxar
`OB.DynamicLinqCore` (motor de query da base moderna) — copiar/substituir? Registar como open-question se formos usar Criteria completo no Tier 1.
