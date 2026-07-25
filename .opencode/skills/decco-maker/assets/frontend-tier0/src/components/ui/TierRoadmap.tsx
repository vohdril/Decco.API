/* Roadmap de progressão estilo jogo — mostra tiers e o que desbloqueiam */

export interface TierNode {
  id: string;
  label: string;
  description: string;
  unlocks: string[];
}

export const FE_TIERS: TierNode[] = [
  {
    id: "FE0",
    label: "FE0 — Console Mock",
    description: "Base operacional estabelecida",
    unlocks: ["Login com RBAC", "Catálogo de anomalias mock", "Modal Radix + 4 estados", "Auth na UI (clearance/sítio)"],
  },
  {
    id: "FE1",
    label: "FE1 — Conexão ao Back",
    description: "Comunicação com o núcleo Decco.API",
    unlocks: ["httpApi.ts com endpoints reais", "SDK NSwag / OpenAPI", "Toggle mock ↔ live funcional", "Autenticação JWT"],
  },
  {
    id: "FE2",
    label: "FE2 — Tabelas e Gráficos",
    description: "Visualização avançada de dados",
    unlocks: ["Tabelas @tanstack/react-table", "Gráficos recharts / @visx", "Dashboard com dados reais", "Ordenação e filtros"],
  },
  {
    id: "FE3",
    label: "FE3 — Esfera 3D (Three.js)",
    description: "Visualização imersiva do núcleo Decco",
    unlocks: ["Esfera com shaders (three+fiber)", "Dados do reator em tempo real", "Animações de contenção"],
  },
  {
    id: "FE4",
    label: "FE4 — Micro-frontends",
    description: "Escala e independência de squads",
    unlocks: ["Module Federation", "Storybook", "Mobile-first"],
  },
];

export const BE_TIERS: TierNode[] = [
  {
    id: "BE0",
    label: "BE0 — Scaffold & Contratos",
    description: "Estrutura inicial do backend",
    unlocks: ["Projeto .NET 8 com camadas", "Envelope Request/Response", "EF Core + Dapper híbrido", "Primeira rota (health)"],
  },
  {
    id: "BE1",
    label: "BE1 — CRUD de Catálogos",
    description: "Endpoints para entidades do DeccoDB",
    unlocks: ["Cognição Aparente CRUD", "Periculosidade CRUD", "Laboratório CRUD", "Protocolo CRUD", "Notificação CRUD"],
  },
  {
    id: "BE2",
    label: "BE2 — Auth & Anomalia",
    description: "Autenticação e entidade principal",
    unlocks: ["JWT + RBAC", "Anomalia CRUD completo", "Resource-based authorization", "Filtros por clearance/sítio"],
  },
  {
    id: "BE3",
    label: "BE3 — Cache & Performance",
    description: "Otimização de acesso a dados",
    unlocks: ["Redis / FusionCache", "Cache de catálogos", "Paginação com Criteria", "Stored procedures complexas"],
  },
  {
    id: "BE4",
    label: "BE4 — Eventos & Observabilidade",
    description: "Sistema distribuído e monitorado",
    unlocks: ["Kafka / message bus", "OpenTelemetry + tracing", "Health checks", "Logs estruturados (Serilog)"],
  },
];

export const DB_TIERS: TierNode[] = [
  {
    id: "DB0",
    label: "DB0 — Schema & Lore Brasileiro",
    description: "Base de dados inicial",
    unlocks: ["Tabelas base (Anomalia, Classe, Camada)", "Lore: Cognição, Periculosidade, OA", "Seed de dados fictícios SCP", "Índices e constraints"],
  },
  {
    id: "DB1",
    label: "DB1 — Stored Procedures",
    description: "Inserção e atualização via SP",
    unlocks: ["sp_*_Inserir para catálogos", "sp_*_Atualizar para catálogos", "Dapper + CommandType.StoredProcedure", "POCOs com sufixo QR"],
  },
  {
    id: "DB2",
    label: "DB2 — Relacionamentos N:N",
    description: "Associações complexas",
    unlocks: ["Tabelas associativas", "Joins de múltiplas entidades", "Views materializadas", "Índices covering"],
  },
  {
    id: "DB3",
    label: "DB3 — Full-Text & Busca",
    description: "Pesquisa textual e Elasticsearch",
    unlocks: ["Full-text indexes (SQL)", "Elasticsearch integrado", "Busca por Código SCP", "Suggester / autocomplete"],
  },
  {
    id: "DB4",
    label: "DB4 — Poliglota & Change Tracking",
    description: "Múltiplos bancos e auditoria",
    unlocks: ["Polyglot persistence (SQL+NoSQL)", "Change Data Capture (CDC)", "Event sourcing", "Audit trail completo"],
  },
];

export function TierRoadmap({ tiers, currentTier, onTierClick }: { tiers: TierNode[]; currentTier: string; onTierClick?: (tier: TierNode) => void }) {
  const currentIdx = tiers.findIndex((t) => t.id === currentTier);
  if (currentIdx < 0) return null;

  return (
    <div className="roadmap">
      {tiers.map((tier, i) => {
        const isUnlocked = i < currentIdx;
        const isCurrent = i === currentIdx;
        const isLocked = i > currentIdx;

        return (
          <div
            key={tier.id}
            className={`roadmap__node ${isUnlocked ? "roadmap__node--done" : ""} ${isCurrent ? "roadmap__node--current" : ""} ${isLocked ? "roadmap__node--locked" : ""} ${onTierClick ? "roadmap__node--clickable" : ""}`}
            onClick={() => onTierClick?.(tier)}
            onKeyDown={(e) => { if (e.key === "Enter" || e.key === " ") { e.preventDefault(); onTierClick?.(tier); } }}
            tabIndex={onTierClick ? 0 : undefined}
            role={onTierClick ? "button" : undefined}
          >
            {i > 0 && <div className="roadmap__line" />}

            <div className="roadmap__content">
              <div className="roadmap__header">
                <span className="roadmap__status">
                  {isUnlocked ? "✅" : isCurrent ? "▶" : "🔒"}
                </span>
                <span className="roadmap__label">{tier.label}</span>
              </div>
              <div className="roadmap__desc">{tier.description}</div>

              {isCurrent && (
                <div className="roadmap__progress-bar">
                  <div className="roadmap__progress-fill" />
                </div>
              )}

              <ul className="roadmap__unlocks">
                {tier.unlocks.map((item) => (
                  <li key={item} className="roadmap__unlock-item">
                    {isUnlocked ? (
                      <span className="roadmap__check">✓</span>
                    ) : (
                      <span className="roadmap__lock-dot" />
                    )}
                    <span className={isLocked ? "roadmap__muted" : ""}>{item}</span>
                    {isLocked && <span className="roadmap__badge">Tier {tier.id}</span>}
                  </li>
                ))}
              </ul>
            </div>
          </div>
        );
      })}
    </div>
  );
}
