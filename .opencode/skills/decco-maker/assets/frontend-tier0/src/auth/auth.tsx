import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from "react";
import { Navigate, useLocation } from "react-router-dom";
import { api } from "../data";
import { USERS } from "../mocks/data";
import type { User, UserRole } from "../mocks/types";

/* Auth mock com persistência em localStorage. Tipos de usuário: PESQUISADOR,
   AGENTE_CONTENCAO, DIRETOR_SITIO, O5 — cada um com clearance e sítios. */

interface AuthCtx {
  user: User | null;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
  hasClearance: (min: number) => boolean;
  hasRole: (...roles: UserRole[]) => boolean;
}

const Ctx = createContext<AuthCtx | null>(null);
const STORAGE_KEY = "decco.auth.user";

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(() => {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (raw) return JSON.parse(raw) as User;
    } catch { /* ignore */ }
    const { password: _, ...vance } = USERS.find((u) => u.username === "vance")!;
    localStorage.setItem(STORAGE_KEY, JSON.stringify(vance));
    return vance;
  });

  useEffect(() => {
    if (user) localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
    else localStorage.removeItem(STORAGE_KEY);
  }, [user]);

  const value = useMemo<AuthCtx>(() => ({
    user,
    login: async (username, password) => { setUser(await api.login(username, password)); },
    logout: () => setUser(null),
    hasClearance: (min) => (user?.clearance ?? 0) >= min,
    hasRole: (...roles) => (user ? roles.includes(user.role) : false),
  }), [user]);

  return <Ctx.Provider value={value}>{children}</Ctx.Provider>;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth(): AuthCtx {
  const ctx = useContext(Ctx);
  if (!ctx) throw new Error("useAuth deve ser usado dentro de <AuthProvider>.");
  return ctx;
}

export function ProtectedRoute({ children }: { children: ReactNode }) {
  const { user } = useAuth();
  const location = useLocation();
  if (!user) return <Navigate to="/login" replace state={{ from: location.pathname }} />;
  return <>{children}</>;
}
