import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { FileText, Plus, Trash2 } from "lucide-react";
import { useState } from "react";
import { AppShell } from "../components/layout/AppShell";
import { Button, EmptyState, ErrorState, LoadingState, Panel } from "../components/ui/primitives";
import { Modal } from "../components/ui/Modal";
import { useToast } from "../components/ui/Toast";
import { api } from "../data";
import type { ProtocoloContencao } from "../data/gateway";

export default function ProtocoloPage() {
  const { toast } = useToast();
  const qc = useQueryClient();
  const [open, setOpen] = useState(false);
  const [edit, setEdit] = useState<ProtocoloContencao | null>(null);
  const [toDelete, setToDelete] = useState<ProtocoloContencao | null>(null);
  const [form, setForm] = useState({ codigo: "", titulo: "", descricao: "", nivelUrgencia: 1, classesAplicaveis: "", passos: "", recursosNecessarios: "" });

  const query = useQuery({
    queryKey: ["protocolos"],
    queryFn: () => api.listProtocolos(),
  });

  const saveMut = useMutation({
    mutationFn: async () => {
      const payload = { ...form, nivelUrgencia: Number(form.nivelUrgencia), classesAplicaveis: form.classesAplicaveis || null, recursosNecessarios: form.recursosNecessarios || null };
      if (edit) { await api.updateProtocolo({ ...edit, ...payload }); }
      else { await api.createProtocolo(payload as Omit<ProtocoloContencao, "id">); }
    },
    onSuccess: () => {
      toast({ title: edit ? "Atualizado" : "Criado", msg: `Protocolo ${edit ? "atualizado" : "cadastrado"}.`, tone: "success" });
      qc.invalidateQueries({ queryKey: ["protocolos"] });
      setOpen(false);
    },
    onError: (e) => toast({ title: "Falha", msg: (e as Error).message, tone: "error" }),
  });

  const deleteMut = useMutation({
    mutationFn: (id: number) => api.deleteProtocolo(id),
    onSuccess: () => {
      toast({ title: "Removido", msg: "Protocolo excluído.", tone: "info" });
      qc.invalidateQueries({ queryKey: ["protocolos"] });
      setToDelete(null);
    },
    onError: (e) => toast({ title: "Falha", msg: (e as Error).message, tone: "error" }),
  });

  const openNew = () => { setEdit(null); setForm({ codigo: "", titulo: "", descricao: "", nivelUrgencia: 1, classesAplicaveis: "", passos: "", recursosNecessarios: "" }); setOpen(true); };
  const openEdit = (item: ProtocoloContencao) => {
    setEdit(item);
    setForm({ codigo: item.codigo, titulo: item.titulo, descricao: item.descricao, nivelUrgencia: item.nivelUrgencia, classesAplicaveis: item.classesAplicaveis ?? "", passos: item.passos, recursosNecessarios: item.recursosNecessarios ?? "" });
    setOpen(true);
  };

  return (
    <AppShell title="CONFIGURAÇÕES" subtitle="Protocolos de Contenção">
      <div className="toolbar">
        <div className="grow" />
        <Button variant="primary" onClick={openNew}>
          <Plus size={14} /> Novo
        </Button>
      </div>

      <Panel title={<><FileText size={14} /> Protocolos de Contenção ({query.data?.length ?? "—"})</>}>
        {query.isLoading ? (
          <LoadingState label="Carregando protocolos…" />
        ) : query.isError ? (
          <ErrorState message={(query.error as Error).message} onRetry={query.refetch} />
        ) : query.data!.length === 0 ? (
          <EmptyState
            icon={<FileText size={40} />}
            title="Nenhum protocolo cadastrado"
            description="Catálogo de protocolos de contenção vazio."
            action={<Button variant="primary" onClick={openNew}><Plus size={14} /> Cadastrar</Button>}
          />
        ) : (
          <div className="anom-grid">
            {query.data!.map((item) => (
              <div key={item.id} className="anom-card" onClick={() => openEdit(item)}>
                <div className="row" style={{ justifyContent: "space-between" }}>
                  <span className="anom-card__code">{item.codigo}</span>
                  <span className="tag" style={{ color: item.nivelUrgencia >= 4 ? "var(--danger)" : item.nivelUrgencia >= 2 ? "var(--warn)" : "var(--text-2)" }}>
                    Urgência {item.nivelUrgencia}
                  </span>
                </div>
                <div className="anom-card__name">{item.titulo}</div>
                <div className="muted" style={{ fontSize: 12, display: "-webkit-box", WebkitLineClamp: 2, WebkitBoxOrient: "vertical", overflow: "hidden" }}>
                  {item.descricao}
                </div>
                {item.classesAplicaveis && (
                  <div className="anom-card__meta">
                    <span className="tag">{item.classesAplicaveis}</span>
                  </div>
                )}
              </div>
            ))}
          </div>
        )}
      </Panel>

      <Modal open={open} onOpenChange={setOpen}
        title={edit ? "Editar Protocolo" : "Novo Protocolo"}
        footer={
          <>
            <Button onClick={() => setOpen(false)}>Cancelar</Button>
            <Button variant="primary" disabled={saveMut.isPending} onClick={() => saveMut.mutate()}>
              {saveMut.isPending ? "Salvando…" : "Salvar"}
            </Button>
          </>
        }>
        <div className="col" style={{ gap: 12 }}>
          <div className="grid" style={{ gridTemplateColumns: "1fr 1fr" }}>
            <div className="field">
              <label className="hud-label">Código</label>
              <input className="input" placeholder="PROT-001" value={form.codigo} onChange={e => setForm(p => ({ ...p, codigo: e.target.value }))} />
            </div>
            <div className="field">
              <label className="hud-label">Nível de Urgência (1-5)</label>
              <input className="input" type="number" min={1} max={5} value={form.nivelUrgencia} onChange={e => setForm(p => ({ ...p, nivelUrgencia: Number(e.target.value) }))} />
            </div>
          </div>
          <div className="field">
            <label className="hud-label">Título</label>
            <input className="input" placeholder="Protocolo de Contenção de Entidade Sentiente" value={form.titulo} onChange={e => setForm(p => ({ ...p, titulo: e.target.value }))} />
          </div>
          <div className="field">
            <label className="hud-label">Descrição</label>
            <textarea className="input" rows={2} value={form.descricao} onChange={e => setForm(p => ({ ...p, descricao: e.target.value }))} />
          </div>
          <div className="field">
            <label className="hud-label">Classes Aplicáveis</label>
            <input className="input" placeholder="PACATO, YAGUARA" value={form.classesAplicaveis} onChange={e => setForm(p => ({ ...p, classesAplicaveis: e.target.value }))} />
          </div>
          <div className="field">
            <label className="hud-label">Passos</label>
            <textarea className="input" rows={4} placeholder="1. Isolar o perímetro&#10;2. Estabelecer contenção visual&#10;3. …" value={form.passos} onChange={e => setForm(p => ({ ...p, passos: e.target.value }))} />
          </div>
          <div className="field">
            <label className="hud-label">Recursos Necessários</label>
            <textarea className="input" rows={2} placeholder="EPI nível 4, Unidade de contenção móvel, …" value={form.recursosNecessarios} onChange={e => setForm(p => ({ ...p, recursosNecessarios: e.target.value }))} />
          </div>
        </div>
      </Modal>

      <Modal open={!!toDelete} onOpenChange={(o) => !o && setToDelete(null)}
        title="Confirmar exclusão"
        footer={
          <>
            <Button onClick={() => setToDelete(null)}>Cancelar</Button>
            <Button variant="danger" disabled={deleteMut.isPending} onClick={() => toDelete && deleteMut.mutate(toDelete.id)}>
              {deleteMut.isPending ? "Removendo…" : "Confirmar"}
            </Button>
          </>
        }>
        <p style={{ margin: 0 }}>Remover <b className="accent">{toDelete?.codigo}</b> — {toDelete?.titulo}?</p>
      </Modal>
    </AppShell>
  );
}
