import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { AlertTriangle, Plus, Trash2 } from "lucide-react";
import { useState } from "react";
import { AppShell } from "../components/layout/AppShell";
import { Button, EmptyState, ErrorState, LoadingState, Panel } from "../components/ui/primitives";
import { Modal } from "../components/ui/Modal";
import { useToast } from "../components/ui/Toast";
import { api } from "../data";
import type { CatPericulosidade } from "../data/gateway";

export default function PericulosidadePage() {
  const { toast } = useToast();
  const qc = useQueryClient();
  const [open, setOpen] = useState(false);
  const [edit, setEdit] = useState<CatPericulosidade | null>(null);
  const [toDelete, setToDelete] = useState<CatPericulosidade | null>(null);
  const [form, setForm] = useState({ nivel: 1, nome: "", descricao: "", corAlerta: "" });

  const query = useQuery({
    queryKey: ["periculosidades"],
    queryFn: () => api.listPericulosidades(),
  });

  const saveMut = useMutation({
    mutationFn: async () => {
      const payload = { ...form, nivel: Number(form.nivel), corAlerta: form.corAlerta || null };
      if (edit) { await api.updatePericulosidade({ ...edit, ...payload }); }
      else { await api.createPericulosidade(payload as Omit<CatPericulosidade, "id">); }
    },
    onSuccess: () => {
      toast({ title: edit ? "Atualizado" : "Criado", msg: `Periculosidade ${edit ? "atualizada" : "cadastrada"}.`, tone: "success" });
      qc.invalidateQueries({ queryKey: ["periculosidades"] });
      setOpen(false);
    },
    onError: (e) => toast({ title: "Falha", msg: (e as Error).message, tone: "error" }),
  });

  const deleteMut = useMutation({
    mutationFn: (id: number) => api.deletePericulosidade(id),
    onSuccess: () => {
      toast({ title: "Removido", msg: "Periculosidade excluída.", tone: "info" });
      qc.invalidateQueries({ queryKey: ["periculosidades"] });
      setToDelete(null);
    },
    onError: (e) => toast({ title: "Falha", msg: (e as Error).message, tone: "error" }),
  });

  const openNew = () => { setEdit(null); setForm({ nivel: 1, nome: "", descricao: "", corAlerta: "" }); setOpen(true); };
  const openEdit = (item: CatPericulosidade) => {
    setEdit(item);
    setForm({ nivel: item.nivel, nome: item.nome, descricao: item.descricao, corAlerta: item.corAlerta ?? "" });
    setOpen(true);
  };

  return (
    <AppShell title="CONFIGURAÇÕES" subtitle="Periculosidade">
      <div className="toolbar">
        <div className="grow" />
        <Button variant="primary" onClick={openNew}>
          <Plus size={14} /> Novo
        </Button>
      </div>

      <Panel title={<><AlertTriangle size={14} /> Níveis de Periculosidade ({query.data?.length ?? "—"})</>}>
        {query.isLoading ? (
          <LoadingState label="Carregando níveis…" />
        ) : query.isError ? (
          <ErrorState message={(query.error as Error).message} onRetry={query.refetch} />
        ) : query.data!.length === 0 ? (
          <EmptyState
            icon={<AlertTriangle size={40} />}
            title="Nenhum nível cadastrado"
            description="Catálogo de periculosidade vazio."
            action={<Button variant="primary" onClick={openNew}><Plus size={14} /> Cadastrar</Button>}
          />
        ) : (
          <div className="anom-grid">
            {query.data!.map((item) => (
              <div key={item.id} className="anom-card" onClick={() => openEdit(item)}>
                <div className="row" style={{ justifyContent: "space-between" }}>
                  <span className="anom-card__code">
                    <span className="badge" style={{ color: item.corAlerta ?? "var(--text-2)", borderColor: item.corAlerta ?? "var(--border)", background: `${item.corAlerta ?? "transparent"}18` }}>
                      Nível {item.nivel}
                    </span>
                  </span>
                </div>
                <div className="anom-card__name">{item.nome}</div>
                <div className="muted" style={{ fontSize: 12, display: "-webkit-box", WebkitLineClamp: 2, WebkitBoxOrient: "vertical", overflow: "hidden" }}>
                  {item.descricao}
                </div>
              </div>
            ))}
          </div>
        )}
      </Panel>

      <Modal open={open} onOpenChange={setOpen}
        title={edit ? "Editar Periculosidade" : "Nova Periculosidade"}
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
              <label className="hud-label">Nível</label>
              <input className="input" type="number" min={1} value={form.nivel} onChange={e => setForm(p => ({ ...p, nivel: Number(e.target.value) }))} />
            </div>
            <div className="field">
              <label className="hud-label">Cor de Alerta</label>
              <div className="row" style={{ gap: 8 }}>
                <input className="input" style={{ flex: 1, fontFamily: "var(--font-mono)" }} placeholder="#ff0000" value={form.corAlerta} onChange={e => setForm(p => ({ ...p, corAlerta: e.target.value }))} />
                {form.corAlerta && <span style={{ width: 28, height: 28, borderRadius: 4, background: form.corAlerta, border: "1px solid var(--border)", flexShrink: 0 }} />}
              </div>
            </div>
          </div>
          <div className="field">
            <label className="hud-label">Nome</label>
            <input className="input" placeholder="Crítico" value={form.nome} onChange={e => setForm(p => ({ ...p, nome: e.target.value }))} />
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
        <p style={{ margin: 0 }}>Remover <b className="accent">{toDelete?.nome}</b> (Nível {toDelete?.nivel})?</p>
      </Modal>
    </AppShell>
  );
}
