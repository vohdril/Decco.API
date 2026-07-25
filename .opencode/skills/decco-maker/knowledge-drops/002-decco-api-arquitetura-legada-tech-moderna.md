# 002 — Decco.API: arquitetura do OB.API, tecnologia da Partners

- **Data de registo:** 2026-07-12
- **Fonte:** decisão de design do utilizador (quer os dois projetos com tech atual/corporativa) + cruzamento OB.API×Partners
- **Tipo:** padrão novo (decisão de rumo)
- **Afeta:** reference/01, reference/02, reference/03, recipes/01, templates/ (moldes do core)
- **Camada:** Decco.API (core)

## O padrão / a mudança
A **Decco.API** conserva o **papel e os padrões arquiteturais** do OB.API — core dono do DeccoDB, **envelope** Request/Response
(Single/Bulk/Paged, `Status`/`Errors`/`RequestId`), controller→manager→repositório+UnitOfWork, acesso a dados **centrado em
stored procedures** — mas é implementada em **tecnologia moderna** (a mesma da Partners): **.NET 8, ASP.NET Core, EF Core 8 +
Dapper, DI nativa, System.Text.Json, StackExchange.Redis/FusionCache, Serilog+OpenTelemetry, FluentValidation, Docker**.

Mapa de substituição (legado → moderno, sem mudar a arquitetura): EF6/EDMX→EF Core; Unity+AOP→DI nativa (+Scrutor p/ decoração);
Web API 2/OWIN→ASP.NET Core; ServiceStack.Redis→StackExchange.Redis atrás de `ICacheProvider`; Web.config→appsettings+env vars;
WCF/SOAP→HttpClient; .NET Framework 4.8→.NET 8; packages.config→PackageReference.

## Porquê (obrigatório nesta skill)
A Omnibees **constrói serviços novos** no stack moderno (Partners é o `api-template`/boilerplate; OB.API legado é mantido, não
estendido). O `bhi-ob-api` é 100% .NET Framework (0 projetos SDK-style) — replicá-lo ensinaria *manter legado*, não *construir
moderno*, que é o objetivo do Paulo. **Alternativa rejeitada:** clonar o stack legado (EF6/EDMX, Unity, .NET Framework) — alto
atrito, tech em fim de vida (ServiceStack.Redis é EOL), baixo ROI de aprendizado. **O que quebraria se não fizesse:** o sandbox
divergiria da realidade corporativa de *novos* serviços e prenderia o estudo a ferramentas datadas.

## Como aplicar a partir de agora
Ao gerar/analisar a Decco.API: usar **sempre** o stack moderno da tabela acima, **mas** manter a forma arquitetural do core
(envelope, managers, repositório híbrido EF+Dapper, SP como cidadão de 1ª classe). A diferença entre Decco.API e Foundation.API
é **arquitetural** (dono-do-dado vs consumidor-fachada), não tecnológica — ambas em .NET 8. Alinhar a versão de runtime com a
Foundation.API (.NET 8 LTS) por consistência.

## Fio a puxar
Vale um exercício-comparação pontual "como esta operação seria no legado (EF6/EDMX/Unity)"? Registrar como open-question se o
Paulo quiser tocar no legado sem construir o sandbox nele.
