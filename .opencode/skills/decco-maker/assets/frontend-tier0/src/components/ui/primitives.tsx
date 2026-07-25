import clsx from "clsx";
import type { CSSProperties, ReactNode } from "react";
import { CLASSES } from "../../mocks/data";

/* Primitivos do design system Decco (HUD). Estilos em styles/components.css. */

/* ---- Panel: painel HUD com cabeçalho e cantos em colchete ---- */
export function Panel(props: {
  title?: ReactNode; actions?: ReactNode; bracket?: boolean;
  className?: string; children: ReactNode; style?: CSSProperties;
}) {
  const { title, actions, bracket = true, className, children, style } = props;
  return (
    <section className={clsx("panel", bracket && "panel--bracket", className)} style={style}>
      {(title || actions) && (
        <header className="panel__head">
          <div className="panel__title hud-label">{title}</div>
          {actions && <div className="row">{actions}</div>}
        </header>
      )}
      {children}
    </section>
  );
}

/* ---- StatTile ---- */
export function StatTile({ label, value, sub, accent }: {
  label: string; value: ReactNode; sub?: ReactNode; accent?: boolean;
}) {
  return (
    <div className={clsx("stat-tile", accent && "stat-tile--accent")}>
      <div className="hud-label">{label}</div>
      <div className="stat-tile__value mono">{value}</div>
      {sub && <div className="stat-tile__sub muted">{sub}</div>}
    </div>
  );
}

/* ---- ProgressBar ---- */
export function ProgressBar({ value, max = 100, label, tone = "accent" }: {
  value: number; max?: number; label?: string; tone?: "accent" | "ok" | "warn" | "danger";
}) {
  const pct = Math.max(0, Math.min(100, (value / max) * 100));
  return (
    <div className="progress">
      {label && (
        <div className="progress__top">
          <span className="hud-label">{label}</span>
          <span className="mono muted">{Math.round(pct)}%</span>
        </div>
      )}
      <div className="progress__track">
        <div className={clsx("progress__fill", `progress__fill--${tone}`)} style={{ width: `${pct}%` }} />
      </div>
    </div>
  );
}

/* ---- SeverityBadge: cor pela CorAlerta da classe ---- */
const CLASSE_COR: Record<string, string> =
  Object.fromEntries(CLASSES.map((c) => [c.nome, c.corAlerta]));

export function SeverityBadge({ classe }: { classe: string }) {
  const cor = CLASSE_COR[classe] ?? "var(--accent)";
  return (
    <span className="badge" style={{ color: cor, borderColor: cor, background: `${cor}18` }}>
      <span className="badge__dot" style={{ background: cor }} />
      {classe}
    </span>
  );
}

/* ---- Button ---- */
export function Button({ variant = "ghost", className, children, ...rest }: {
  variant?: "primary" | "ghost" | "danger";
} & React.ButtonHTMLAttributes<HTMLButtonElement>) {
  return (
    <button className={clsx("btn", `btn--${variant}`, className)} {...rest}>
      {children}
    </button>
  );
}

/* ---- Skeleton ---- */
export function Skeleton({ w, h = 14, radius = 4, style }: {
  w?: number | string; h?: number | string; radius?: number; style?: CSSProperties;
}) {
  return <span className="skeleton" style={{ width: w ?? "100%", height: h, borderRadius: radius, ...style }} />;
}
export function SkeletonText({ lines = 3 }: { lines?: number }) {
  return (
    <div className="col" style={{ gap: 8 }}>
      {Array.from({ length: lines }).map((_, i) => (
        <Skeleton key={i} w={i === lines - 1 ? "60%" : "100%"} />
      ))}
    </div>
  );
}

/* ---- Spinner / LoadingState ---- */
export function Spinner({ size = 20 }: { size?: number }) {
  return <span className="spinner" style={{ width: size, height: size }} aria-label="carregando" />;
}
export function LoadingState({ label = "Sincronizando com o núcleo…" }: { label?: string }) {
  return (
    <div className="state">
      <Spinner size={26} />
      <div className="hud-label" style={{ marginTop: 12 }}>{label}</div>
    </div>
  );
}

/* ---- EmptyState ---- */
export function EmptyState({ icon, title, description, action }: {
  icon?: ReactNode; title: string; description?: string; action?: ReactNode;
}) {
  return (
    <div className="state">
      {icon && <div className="state__icon">{icon}</div>}
      <h3 style={{ marginTop: 12 }}>{title}</h3>
      {description && <p className="muted" style={{ maxWidth: 380, textAlign: "center" }}>{description}</p>}
      {action && <div style={{ marginTop: 14 }}>{action}</div>}
    </div>
  );
}

/* ---- ErrorState ---- */
export function ErrorState({ message, onRetry }: { message: string; onRetry?: () => void }) {
  return (
    <div className="state">
      <div className="state__icon" style={{ color: "var(--danger)" }}>⚠</div>
      <h3 style={{ marginTop: 12, color: "var(--danger)" }}>Falha de sincronização</h3>
      <p className="muted mono" style={{ maxWidth: 420, textAlign: "center", fontSize: 12 }}>{message}</p>
      {onRetry && <div style={{ marginTop: 14 }}><Button variant="primary" onClick={onRetry}>Tentar novamente</Button></div>}
    </div>
  );
}
