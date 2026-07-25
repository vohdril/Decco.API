# 013 — Front-end sai junto do Tier-0 + seam de dados mock↔Decco.API

- **Data de registo:** 2026-07-18
- **Fonte:** refinamento pedido pelo Paulo — consolidar o front na skill (implementar **junto do Tier-0**) e mandar que **toda** implementação de front permita **alternar entre mock e Decco.API**, com boas práticas para a transição (nem sempre o back estará de pé ao desenvolver o front). Pesquisa de opções validada por workflow.
- **Tipo:** complemento (consolida a trilha de FE aberta em `knowledge-drops/012`)
- **Afeta:** `SKILL.md` (nova Regra de ouro 11), `reference/16` (secção do seam + entrega junto do Tier-0), `recipes/04-frontend-tier0.md` (novo), `assets/frontend-tier0/` (refatorado com o seam), `open-questions/Q-006`
- **Camada:** front-end

## O padrão / a mudança
Duas consolidações:

**(1) Entrega:** quando o comando dispara o **Tier-0**, o **FE Tier-0 sai JUNTO** do Tier-0 de back (mesmo checkpoint) — não é uma
fase separada e opcional. Recipe: `recipes/04`. A skill continua carregando o projeto em `assets/frontend-tier0/` (autossuficiência).

**(2) Seam de dados obrigatório (mock↔Decco.API):** a UI depende só do **contrato** `DeccoApi` (`data/gateway.ts`); há duas
implementações — **mock** (`mocks/api.ts`, em memória) e **HTTP** (`data/httpApi.ts`, esqueleto p/ a Decco.API/Foundation.API) — e
um **ponto único** (`data/index.ts`) que escolhe qual usar. A fonte resolve por **`localStorage` > `.env` (`VITE_DATA_SOURCE`) >
`mock`** (`data/dataSource.ts`); há um **seletor DADOS** no topo da tela para trocar em runtime. **Trocar mock↔live nunca toca em
nenhuma página.** `.env.example` documenta `VITE_DATA_SOURCE`/`VITE_API_BASE_URL`.

## Porquê (obrigatório nesta skill)
- **Desenvolver o front sem o back de pé** era o requisito explícito. O seam (gateway + factory) resolve com **dependency
  inversion**: a UI conhece o *contrato*, não a *origem* dos dados. Ligar depois é **local** (só o corpo de `data/httpApi.ts`).
- **Por que este padrão e não outro (pesquisa):** avaliadas 4 opções — (1) **gateway/adapter + factory por env/localStorage**,
  (2) **MSW**, (3) if de env dentro de cada `queryFn`, (4) json-server. Vencedora do **Tier-0**: **opção 1** — *zero dependências
  novas* (só TypeScript, casa com a meta "Tier-0 leve"), a mais **didática** (torna explícita a fronteira de dados) e reaproveita
  o `mocks/api.ts` que já tinha "a assinatura de um cliente". **Rejeitadas:** a (3) espalha condicionais pelos hooks (dívida); a (4)
  reintroduz um processo a subir (contradiz o objetivo) e casa mal com o envelope. **MSW fica como upgrade** (FE Tier 1),
  **complementar** ao gateway: intercepta a rede, então o app usa sempre o cliente real e reusa handlers em testes — paga-se o
  Service Worker quando houver suíte de testes e mais rotas.
- **Entregar junto do Tier-0** evita o front virar "fase que nunca chega": o operador vê o sistema de ponta (tela) desde o primeiro
  checkpoint, e o seam garante que o back real entra sem retrabalho de UI.

## Como aplicar a partir de agora
- Disparou Tier-0 → montar back (`recipes/01`) **e** front (`recipes/04`) no mesmo checkpoint; front roda em **mock** primeiro.
- **Nunca** gerar um front que fale HTTP direto de dentro das páginas/componentes. Sempre atrás do contrato `DeccoApi`, com o
  ponto único de troca. Marcar no `httpApi.ts` o que falta ligar com `// >>>`.
- Manter a linha não-destrutiva: FE tiers 1–4 (ligar no back/flip, data-grid/gráficos, 3D, plataforma) **documentados em
  `reference/16`, não gerados**.
- Ao ligar no back (FE Tier 1): garantir **OpenAPI + CORS** no back; preferir **SDK por NSwag** substituindo `httpApi.ts`.

## Fio a puxar
Fazer o **flip para live** de **um** endpoint (ex.: `listAnomalias`) com a Decco.API rodando e medir: a UI deve mudar **zero**.
Depois, introduzir **MSW** e comparar a experiência (DevTools > Network, erros 4xx/5xx, reuso em teste) com o mock em memória.
