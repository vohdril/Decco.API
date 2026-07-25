import { Lock } from "lucide-react";
import type { ReactNode } from "react";
import { Panel } from "./primitives";
import { FE_TIERS, TierRoadmap } from "./TierRoadmap";

interface LockedFeatureProps {
  title: string;
  description: string;
  unlocksAt: string;
  unlocksAtLabel: string;
  requirements: string[];
  icon?: ReactNode;
}

function RobotSVG() {
  return (
    <svg viewBox="0 0 120 140" className="robot-icon" aria-label="Robô de contenção">
      {/* Antena */}
      <line x1="60" y1="10" x2="60" y2="28" stroke="var(--accent-dim)" strokeWidth="2" />
      <circle cx="60" cy="8" r="4" fill="var(--accent)" opacity={0.6}>
        <animate attributeName="opacity" values="0.3;1;0.3" dur="2s" repeatCount="indefinite" />
      </circle>
      {/* Cabeça */}
      <rect x="28" y="28" width="64" height="48" rx="10" fill="var(--bg-2)" stroke="var(--accent-dim)" strokeWidth="2" />
      {/* Olhos */}
      <circle cx="44" cy="50" r="6" fill="var(--accent)" opacity={0.9} />
      <circle cx="44" cy="50" r="3" fill="var(--accent-strong)" />
      <circle cx="76" cy="50" r="6" fill="var(--accent)" opacity={0.9} />
      <circle cx="76" cy="50" r="3" fill="var(--accent-strong)" />
      {/* Boca (grade) */}
      <rect x="40" y="62" width="40" height="6" rx="2" fill="var(--bg-3)" stroke="var(--border)" strokeWidth="1" />
      <line x1="50" y1="62" x2="50" y2="68" stroke="var(--border)" strokeWidth="1" />
      <line x1="60" y1="62" x2="60" y2="68" stroke="var(--border)" strokeWidth="1" />
      <line x1="70" y1="62" x2="70" y2="68" stroke="var(--border)" strokeWidth="1" />
      {/* Pescoço */}
      <rect x="52" y="76" width="16" height="8" fill="var(--bg-3)" />
      {/* Corpo */}
      <rect x="32" y="84" width="56" height="40" rx="6" fill="var(--bg-2)" stroke="var(--border)" strokeWidth="1.5" />
      {/* Núcleo no peito */}
      <circle cx="60" cy="104" r="8" fill="none" stroke="var(--core)" strokeWidth="1.5" opacity={0.5} />
      <circle cx="60" cy="104" r="4" fill="var(--core)" opacity={0.4}>
        <animate attributeName="opacity" values="0.2;0.6;0.2" dur="3s" repeatCount="indefinite" />
      </circle>
      {/* Braços */}
      <rect x="18" y="90" width="10" height="24" rx="4" fill="var(--bg-3)" stroke="var(--border)" strokeWidth="1" />
      <rect x="92" y="90" width="10" height="24" rx="4" fill="var(--bg-3)" stroke="var(--border)" strokeWidth="1" />
      {/* Correntes nos braços (trancado) */}
      <rect x="18" y="108" width="10" height="4" rx="2" fill="var(--danger)" opacity={0.6} />
      <rect x="92" y="108" width="10" height="4" rx="2" fill="var(--danger)" opacity={0.6} />
    </svg>
  );
}

export function LockedFeature({ title, description, unlocksAt, unlocksAtLabel, requirements, icon }: LockedFeatureProps) {
  return (
    <div className="locked">
      <div className="locked__hero">
        <div className="locked__robot">{icon ?? <RobotSVG />}</div>
        <div className="locked__hero-text">
          <h2 className="locked__title">{title}</h2>
          <p className="locked__desc">{description}</p>
        </div>
      </div>

      <div className="locked__status">
        <Lock size={16} />
        <span className="locked__status-label">
          BLOQUEADO · Dispositivo de contenção inoperante
        </span>
      </div>

      <Panel title="Requisitos de desbloqueio" bracket={false}>
        <div className="locked__requirements">
          <div className="locked__tier">
            <span className="hud-label">Disponível em</span>
            <span className="locked__tier-badge">{unlocksAtLabel}</span>
          </div>
          <ul className="locked__list">
            {requirements.map((r, i) => (
              <li key={i} className="locked__req">
                <span className="locked__req-dot" />
                {r}
              </li>
            ))}
          </ul>
        </div>
      </Panel>

      <details className="locked__roadmap-toggle">
        <summary className="hud-label" style={{ cursor: "pointer", color: "var(--accent)" }}>
          🗺️ Ver roadmap completo de tiers
        </summary>
        <TierRoadmap tiers={FE_TIERS} currentTier={unlocksAt} />
      </details>
    </div>
  );
}
