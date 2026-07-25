import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Bug, Plus, Trash2 } from "lucide-react";
import { useState } from "react";
import { AppShell } from "../components/layout/AppShell";
import { Button, EmptyState, ErrorState, LoadingState, Panel } from "../components/ui/primitives";
import { Modal } from "../components/ui/Modal";
import { useToast } from "../components/ui/Toast";
import { api } from "../data";
import type { InstanciaDeviante } from "../mocks/types";

export default function InstanciaDeviantePage() {
  const { toast } = useToast();
  const qc = useQueryClient();
  const [open, setOpen] = useState(false);
  const [edit, setEdit] = useState<InstanciaDeviante | null>(null);
  const [toDelete, setToDelete] = useState<InstanciaDeviante | null>(null);
  const [form, setForm] = useState({ codigo: "", nome: "", descricao: "", anomaliaCodigoSCP: "", status: "" });

  const query = useQuery({
    queryKey: ["instancias-deviantes"],
    queryFn: () => api.listInstanciasDeviantes(),
  });

  const saveMut = useMutation({
    mutationFn: async () => {
      if (edit) { await api.updateInstanciaDeviante({ ...edit, ...form } as InstanciaDeviante); }
      else { await api.createInstanciaDeviante(form as Omit<InstanciaDeviante, "id">); }
    },
    onSuccess: () => {
      toast({ title: edit ? "Atualizado" : "Criado", msg: `Instância ${edit ? "atualizada" : "cadastrada"}.`, tone: "success" });
      qc.invalidateQueries({ queryKey: ["instancias-deviantes"] });
      setOpen(false);
    },
    onError: (e) => toast({ title: "Falha", msg: (e as Error).message, tone: "error" }),
  });

  const deleteMut = useMutation({
    mutationFn: (id: number) => api.deleteInstanciaDeviante(id),
    onSuccess: () => {
      toast({ title: "Removido", msg: "Instância excluída.", tone: "info" });
      qc.invalidateQueries({ queryKey: ["instancias-deviantes"] });
      setToDelete(null);
    },
    onError: (e) => toast({ title: "Falha", msg: (e as Error).message, tone: "error" }),
  });

  const openNew = () => { setEdit(null); setForm({ codigo: "", nome: "", descricao: "", anomaliaCodigoSCP: "", status: "" }); setOpen(true); };
  const openEdit = (item: InstanciaDeviante) => { setEdit(item); setForm({ codigo: item.codigo, nome: item.nome, descricao: item.descricao, anomaliaCodigoSCP: item.anomaliaCodigoSCP, status: item.status }); setOpen(true); };

  return (
    <AppShell title="CONFIGURAÇÕES" subtitle="Instâncias Deviantes">
      <div className="toolbar">
        <div className="grow" />
        <Button variant="primary" onClick={openNew}>
          <Plus size={14} /> Novo
        </Button>
      </div>

      <Panel title={<><Bug size={14} /> Instâncias Deviantes ({query.data?.length ?? "—"})</>}>
        {query.isLoading ? (
          <LoadingState label="Carregando instâncias…" />
        ) : query.isError ? (
          <ErrorState message={(query.error as Error).message} onRetry={query.refetch} />
        ) : query.data!.length === 0 ? (
          <EmptyState
            icon={<Bug size={40} />}
            title="Nenhuma instância cadastrada"
            action={<Button variant="primary" onClick={openNew}><Plus size={14} /> Cadastrar</Button>}
          />
        ) : (
          <div className="anom-grid">
            {query.data!.map((item) => (
              <div key={item.id} className="anom-card" onClick={() => openEdit(item)}>
                <div className="row" style={{ justifyContent: "space-between" }}>
                  <span className="anom-card__code">{item.codigo}</span>
                  <span className="muted" style={{ fontSize: 11 }}>{item.status}</span>
                </div>
                <div className="anom-card__name">{item.nome}</div>
                <div className="muted" style={{ fontSize: 12 }}>{item.anomaliaCodigoSCP}</div>
                <div className="muted" style={{ fontSize: 12, display: "-webkit-box", WebkitLineClamp: 1, WebkitBoxOrient: "vertical", overflow: "hidden" }}>
                  {item.descricao}
                </div>
              </div>
            ))}
          </div>
        )}
      </Panel>

      <Modal open={open} onOpenChange={setOpen}
        title={edit ? "Editar Instância" : "Nova Instância"}
        footer={
          <>
            <Button onClick={() => setOpen(false)}>Cancelar</Button>
            <Button variant="primary" disabled={saveMut.isPending} onClick={() => saveMut.mutate()}>
              {saveMut.isPending ? "Salvando…" : "Salvar"}
            </Button>
          </>
        }>
        <div className="col" style={{ gap: 12 }}>
          <div className="field">
            <label className="hud-label">Código</label>
            <input className="input" placeholder="INST-001" value={form.codigo} onChange={e => setForm(p => ({ ...p, codigo: e.target.value }))} />
          </div>
          <div className="field">
            <label className="hud-label">Nome</label>
            <input className="input" placeholder="Proteu-Alfa" value={form.nome} onChange={e => setForm(p => ({ ...p, nome: e.target.value }))} />
          </div>
          <div className="field">
            <label className="hud-label">Anomalia</label>
            <input className="input" placeholder="SCP-1001" value={form.anomaliaCodigoSCP} onChange={e => setForm(p => ({ ...p, anomaliaCodigoSCP: e.target.value }))} />
          </div>
          <div className="field">
            <label className="hud-label">Status</label>
            <input className="input" placeholder="CONTIDA" value={form.status} onChange={e => setForm(p => ({ ...p, status: e.target.value }))} />
          </div>
          <div className="field">
            <label className="hud-label">Descrição</label>
            <textarea className="input" rows={3} value={form.descricao} onChange={e => setForm(p => ({ ...p, descricao: e.target.value }))} />
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
        <p style={{ margin: 0 }}>Remover <b className="accent">{toDelete?.codigo}</b> — {toDelete?.nome}?</p>
      </Modal>
    </AppShell>
  );
}
