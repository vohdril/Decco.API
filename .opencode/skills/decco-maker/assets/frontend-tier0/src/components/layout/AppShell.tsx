import { Activity, AlertTriangle, BookOpen, Boxes, BrainCircuit, Bug, FileText, FlaskConical, LayoutDashboard, LogOut, Radar } from "lucide-react";
import { type ReactNode, useState } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../../auth/auth";

const NAV = [
  { to: "/", label: "Dashboard", icon: LayoutDashboard, end: true },
  { to: "/anomalias", label: "Anomalias", icon: Boxes, end: false },
  { to: "/glossario", label: "Glossário", icon: BookOpen, end: false },
];

const CATALOGO_NAV = [
  { to: "/config/pericia-anomalia", label: "Perícias", icon: BrainCircuit },
  { to: "/config/manifestacao-especifica", label: "Manifestações", icon: Activity },
  { to: "/config/instancia-deviante", label: "Instâncias Desviantes", icon: Bug },
  { to: "/config/laboratorio", label: "Laboratórios", icon: FlaskConical },
  { to: "/config/protocolo", label: "Protocolos", icon: FileText },
];

const ROLE_LABEL: Record<string, string> = {
  PESQUISADOR: "Pesquisador", AGENTE_CONTENCAO: "Agente de Contenção",
  DIRETOR_SITIO: "Diretor de Sítio", O5: "Conselho O5",
};

export function AppShell({ title, subtitle, children }: {
  title: string; subtitle?: string; children: ReactNode;
}) {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [collapsed, setCollapsed] = useState(false);
  const w = collapsed ? 85 : 200; // USER: alterado de 68 para 85 (teste manual)

  return (
    <div className="shell" style={{ gridTemplateColumns: `${w}px 1fr`, transition: "grid-template-columns .2s ease" }}>
      <aside className={`shell__side ${collapsed ? "shell__side--collapsed" : "shell__side--expanded"}`} style={{ width: w, transition: "width .2s ease" }}>
        {collapsed ? (
          <div className="brand-orb" title="Clique para expandir" style={{ cursor: "pointer" }} onClick={() => setCollapsed(false)}>
            <Radar size={18} />
          </div>
        ) : (
          <div className="brand-orb--expanded" onClick={() => setCollapsed(true)} style={{ cursor: "pointer" }}>
            <Radar size={18} />
            <span>DeCCO</span>
          </div>
        )}

        {!collapsed && (
          <div className="nav-category">NAVEGAÇÃO</div>
        )}

        <div className="shell__nav">
          {NAV.map(({ to, label, icon: Icon, end }) => (
            <NavLink key={to} to={to} end={end} title={label}
              className={({ isActive }) => `nav-item${isActive ? " nav-item--active" : ""}`}>
              <Icon size={20} />
              {!collapsed && <span className="nav-label">{label}</span>}
            </NavLink>
          ))}

          <div className="nav-divider" />

          {!collapsed && (
            <div className="nav-category">CATÁLOGOS</div>
          )}

          {CATALOGO_NAV.map(({ to, label, icon: Icon }) => (
            <NavLink key={to} to={to} title={label}
              className={({ isActive }) => `nav-item${isActive ? " nav-item--active" : ""}`}>
              <Icon size={20} />
              {!collapsed && <span className="nav-label">{label}</span>}
            </NavLink>
          ))}
        </div>

        {collapsed ? (
          <button className="nav-item nav-item--logout" title="Sair" onClick={() => { logout(); }}>
            <LogOut size={20} />
          </button>
        ) : (
          <div className="shell__user">
            <div className="shell__user-info">
              <div className="shell__user-avatar">{user ? user.nome.charAt(0) : "?"}</div>
              <div className="col" style={{ gap: 0 }}>
                <span className="shell__user-name">{user?.nome || "—"}</span>
                <span className="hud-label" style={{ fontSize: 10 }}>
                  {user ? `${ROLE_LABEL[user.role] || user.role} · Clearance ${user.clearance}` : "—"}
                </span>
              </div>
            </div>
            <button className="shell__logout"
              onClick={() => { logout(); }}>
              <LogOut size={16} />
              Sair
            </button>
          </div>
        )}
      </aside>

      <header className="shell__top">
        <div className="topbar__title">
          <b>{title}</b>
          {subtitle && <span className="muted" style={{ fontSize: 11 }}>{subtitle}</span>}
        </div>
        <div className="topbar__right">
          {user && (
            <div className="user-chip">
              <div className="user-chip__avatar">{user.nome.charAt(0)}</div>
              <div className="col" style={{ gap: 0 }}>
                <span style={{ color: "var(--text-0)", fontSize: 13 }}>{user.nome}</span>
                <span className="hud-label" style={{ fontSize: 10 }}>
                  {ROLE_LABEL[user.role]} · Clearance {user.clearance}
                </span>
              </div>
            </div>
          )}
        </div>
      </header>

      <main className="shell__main">{children}</main>
    </div>
  );
}
