# recipes/04 — Front-end Tier-0 (sai JUNTO do Tier-0 de back)

> **Regra de entrega:** quando o comando dispara o **Tier-0**, o **FE Tier-0 vai junto** (mesmo checkpoint). É um **console
> executável e autossuficiente** que roda **em modo mock, sem back-end** — e já traz o **seam mock↔Decco.API** para ligar depois
> sem reescrever a UI. Conceitos, tiers e decisões: `reference/16`. Decisões consolidadas: `knowledge-drops/012` e `013`.
>
> **Linha não-destrutiva (igual ao back):** o FE Tier-0 é **um** checkpoint (login + dashboard + anomalias sobre mocks Tier-0).
> Os FE tiers **1–4** (ligar no back, data-grid/gráficos, 3D, plataforma) ficam **documentados em `reference/16`, não gerados**.

## Como entregar (a partir da skill, autossuficiente)
A skill **carrega o projeto pronto** em `assets/frontend-tier0/` (o análogo de `assets/decco.sql`). Para pôr de pé:

1. **Copiar** `assets/frontend-tier0/` → `<projeto>/front` (ou a pasta que o operador indicar).
2. **Instalar e rodar em modo mock** (default — não precisa de back):
   ```bash
   cd front
   npm install          # se houver aviso de peer-deps do React 19: npm install --legacy-peer-deps
   npm run dev          # http://localhost:5173
   ```
3. **Login demo:** `vance` (Pesquisador/Sítio-19) · `thoth` (Diretor/Sítio-64) · `o5` (acesso total). Senha p/ todos: `decco`.
✅ **Verificação (checkpoint FE Tier-0):** *"logo, vejo o dashboard, filtro anomalias pelo meu clearance, abro um modal Radix e
forço os 4 estados (OK/Lento/Vazio/Erro) na página Anomalias"*.

## O seam mock ↔ Decco.API (obrigatório em toda implementação de front)
Já vem montado — **a UI depende só do contrato `DeccoApi`** (`src/data/gateway.ts`), com duas implementações e um ponto único:
- `src/mocks/api.ts` → impl **mock** (em memória, latência/cenários, authz `visibleTo`).
- `src/data/httpApi.ts` → impl **HTTP** (esqueleto p/ Foundation.API/Decco.API; `// >>>` no que falta ligar).
- `src/data/index.ts` → **factory** que escolhe mock vs live; `src/data/dataSource.ts` → resolve a fonte (**localStorage > `.env` > mock**).
- **Alternar:** seletor **DADOS** no topo da tela (runtime) ou `VITE_DATA_SOURCE=live` no `.env` (ver `.env.example`).

> **Por quê assim (didática):** desenvolver o front **sem o back de pé** (nem sempre há toda a estrutura rodando) e ligar depois
> **sem tocar em nenhuma página**. É *dependency inversion* — a UI conhece o contrato, não a origem dos dados. Alternativa de
> escala documentada em `reference/16`: **MSW** (intercepta a rede; o app usa sempre o cliente real).

## Checkpoint de ligação (já é FE Tier-1 — fazer quando o back Tier-0 existir)
Quando a **Decco.API/Foundation.API** subir com **OpenAPI + CORS**:
1. Apontar `VITE_API_BASE_URL` e passar a fonte para `live` (seletor ou `.env`).
2. Completar `src/data/httpApi.ts` nos `// >>>` (caminhos + desembrulhar o envelope — `reference/03`/`10`); idealmente **gerar o
   SDK por NSwag** e substituir o corpo. A UI **não muda**.
✅ **Verificação:** a lista de anomalias vem da API real, com o mesmo layout; trocar o seletor de volta para `mock` volta a rodar offline.

## Como estender o front (mesmo espírito do back)
- Componente novo → entrada no array `REGISTRY` de `src/pages/ComponentsSandboxPage.tsx` (documentação viva).
- Estado/variação → primeiro no primitivo (`components/ui/primitives.tsx`) e no token (`styles/tokens.css`), nunca inline.
- Marcar ganchos de tier futuro com `// >>> (FE Tier N: ...)`, igual ao back.
- **Tracking**: o repo do front carrega um `DECCO-PROGRESS.md` (de `templates/`) marcando o Tier (FE0 concluído em modo mock; FE1 = flip). O progresso vive no **projeto**, não na skill; rode `recipes/05` (*"rode um diagnóstico"*) para situar. Ver `reference/17`.

> **Nota de proveniência:** existe uma instância de validação em `Downloads/DeCCo/front` (o teste que provou o FE rodando). O
> asset carregado é a fonte autossuficiente; a instância é proveniência opcional, não dependência.
