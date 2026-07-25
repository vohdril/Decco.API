import { Inbox } from "lucide-react";
import { useState, type ReactNode } from "react";
import { AppShell } from "../components/layout/AppShell";
import { Modal } from "../components/ui/Modal";
import {
  Button, EmptyState, LoadingState, Panel, ProgressBar, SeverityBadge,
  Skeleton, SkeletonText, Spinner, StatTile,
} from "../components/ui/primitives";
import { useToast } from "../components/ui/Toast";

/* =========================================================================
   SANDBOX INTERNO / documentação viva de componentes.
   >>> COMO ADICIONAR UM COMPONENTE NOVO: acrescente uma entrada em REGISTRY
       com { id, name, desc, note, Demo }. O índice e a seção aparecem sozinhos.
   Serve como catálogo de tudo que é implementável no front do Decco.
   ========================================================================= */

function ModalDemo() {
  const [open, setOpen] = useState(false);
  return (
    <>
      <Button variant="primary" onClick={() => setOpen(true)}>Abrir modal</Button>
      <Modal open={open} onOpenChange={setOpen} title="Modal de exemplo"
        description="Radix Dialog: focus-trap, ESC, scroll-lock, ARIA e portal."
        footer={<Button variant="primary" onClick={() => setOpen(false)}>Ok</Button>}>
        <p style={{ margin: 0 }}>Conteúdo do modal. Tente <kbd>Tab</kbd> e <kbd>Esc</kbd>.</p>
      </Modal>
    </>
  );
}

function ToastDemo() {
  const { toast } = useToast();
  return (
    <div className="demo__row">
      <Button onClick={() => toast({ title: "Info", msg: "Mensagem informativa.", tone: "info" })}>Info</Button>
      <Button onClick={() => toast({ title: "Sucesso", msg: "Operação concluída.", tone: "success" })}>Success</Button>
      <Button variant="danger" onClick={() => toast({ title: "Erro", msg: "Algo falhou.", tone: "error" })}>Error</Button>
    </div>
  );
}

interface Entry { id: string; name: string; desc: string; note: ReactNode; Demo: () => ReactNode; }

const REGISTRY: Entry[] = [
  {
    id: "button", name: "Button", desc: "Ação primária/ghost/perigo.",
    note: "Escala: virar um <Button> acessível sobre Radix Slot (asChild) para compor com links/menus.",
    Demo: () => <div className="demo__row"><Button variant="primary">Primary</Button><Button>Ghost</Button><Button variant="danger">Danger</Button><Button disabled>Disabled</Button></div>,
  },
  {
    id: "severity", name: "SeverityBadge", desc: "Classe da anomalia pela CorAlerta do DeccoDB.",
    note: "A cor sai de um token por classe — mudar a paleta re-tematiza sem tocar no componente.",
    Demo: () => <div className="demo__row"><SeverityBadge classe="PACATO" /><SeverityBadge classe="YAGUARA" /><SeverityBadge classe="ABAPORU" /><SeverityBadge classe="UKAR" /></div>,
  },
  {
    id: "stat", name: "StatTile", desc: "KPI de dashboard.",
    note: "Escala: adicionar tendência (delta % ↑/↓) e sparkline embutido.",
    Demo: () => <div className="demo__row"><StatTile label="Anomalias" value={42} sub="visíveis" accent /><StatTile label="Sigma" value={3} sub="alertas" /></div>,
  },
  {
    id: "progress", name: "ProgressBar", desc: "Medidor com tom semântico.",
    note: "Tons: accent/ok/warn/danger via token.",
    Demo: () => <div className="col" style={{ gap: 10, width: 280 }}><ProgressBar label="Processing" value={49} /><ProgressBar label="Neural Load" value={78} tone="warn" /><ProgressBar label="Stability" value={99} tone="ok" /></div>,
  },
  {
    id: "skeleton", name: "Skeleton", desc: "Placeholder de carregamento (percebido).",
    note: "Use o skeleton com a MESMA forma do conteúdo real (evita layout shift).",
    Demo: () => <div style={{ width: 280 }}><SkeletonText lines={3} /><div style={{ height: 10 }} /><Skeleton w={120} h={30} /></div>,
  },
  {
    id: "loading", name: "LoadingState / Spinner", desc: "Estado de carregamento bloqueante.",
    note: "Prefira Skeleton para listas; Spinner para ações pontuais.",
    Demo: () => <div className="demo__row"><Spinner /><LoadingState label="Sincronizando…" /></div>,
  },
  {
    id: "empty", name: "EmptyState", desc: "Ausência de dados (não é erro).",
    note: "Sempre ofereça a próxima ação (CTA) e explique o porquê do vazio.",
    Demo: () => <EmptyState icon={<Inbox size={36} />} title="Sem registros" description="Nenhuma anomalia no escopo atual." action={<Button variant="primary">Registrar</Button>} />,
  },
  {
    id: "modal", name: "Modal (Radix Dialog)", desc: "Diálogo acessível.",
    note: "Escolhido na pesquisa: Radix > <dialog> nativo por a11y/composição. Escala: shadcn/ui (Radix+Tailwind).",
    Demo: () => <ModalDemo />,
  },
  {
    id: "toast", name: "Toast", desc: "Notificação transitória.",
    note: "Tier-0 é próprio; em escala trocar por Sonner ou @radix-ui/react-toast (fila, swipe, a11y).",
    Demo: () => <ToastDemo />,
  },
];

export function ComponentsSandboxPage() {
  return (
    <AppShell title="COMPONENTES" subtitle="Sandbox interno · documentação viva do design system">
      <Panel title="Catálogo de componentes implementáveis" bracket>
        <p className="muted" style={{ marginTop: 0 }}>
          Cada componente do Decco documentado com exemplo vivo e nota de escala. Para adicionar um novo,
          edite <code className="mono accent">src/pages/ComponentsSandboxPage.tsx</code> → array <code className="mono accent">REGISTRY</code>.
        </p>
        <nav className="sandbox__toc">
          {REGISTRY.map((e) => <a key={e.id} href={`#c-${e.id}`}>{e.name}</a>)}
        </nav>
      </Panel>

      <div className="col" style={{ gap: 16, marginTop: 16 }}>
        {REGISTRY.map((e) => (
          <Panel key={e.id} title={e.name}
            actions={<span className="muted" style={{ fontSize: 12 }}>{e.desc}</span>}>
            <div id={`c-${e.id}`} className="demo">
              <div className="demo__row">{e.Demo()}</div>
              <div className="demo__note">💡 {e.note}</div>
            </div>
          </Panel>
        ))}
      </div>
    </AppShell>
  );
}
