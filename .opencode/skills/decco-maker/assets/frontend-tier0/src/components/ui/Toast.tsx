import { createContext, useCallback, useContext, useState, type ReactNode } from "react";

/* Sistema de toast leve e próprio (Tier-0). Em escala, trocar por Sonner ou
   @radix-ui/react-toast (documentado no sandbox). Assinatura estável: useToast(). */

type ToastTone = "info" | "success" | "error";
interface ToastItem { id: number; title: string; msg?: string; tone: ToastTone; }
interface ToastCtx { toast: (t: { title: string; msg?: string; tone?: ToastTone }) => void; }

const Ctx = createContext<ToastCtx | null>(null);
let seq = 1;

export function ToastProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<ToastItem[]>([]);

  const toast = useCallback((t: { title: string; msg?: string; tone?: ToastTone }) => {
    const id = seq++;
    setItems((prev) => [...prev, { id, title: t.title, msg: t.msg, tone: t.tone ?? "info" }]);
    setTimeout(() => setItems((prev) => prev.filter((x) => x.id !== id)), 4000);
  }, []);

  return (
    <Ctx.Provider value={{ toast }}>
      {children}
      <div className="toaster" role="region" aria-label="Notificações">
        {items.map((t) => (
          <div key={t.id} className={`toast toast--${t.tone}`} role="status">
            <div className="toast__title">{t.title}</div>
            {t.msg && <div className="toast__msg">{t.msg}</div>}
          </div>
        ))}
      </div>
    </Ctx.Provider>
  );
}

// eslint-disable-next-line react-refresh/only-export-components
export function useToast(): ToastCtx {
  const ctx = useContext(Ctx);
  if (!ctx) throw new Error("useToast deve ser usado dentro de <ToastProvider>.");
  return ctx;
}
