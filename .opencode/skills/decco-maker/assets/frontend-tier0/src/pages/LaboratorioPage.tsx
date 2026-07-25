import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { FlaskConical, Plus, Trash2 } from "lucide-react";
import { useState } from "react";
import { AppShell } from "../components/layout/AppShell";
import { Button, EmptyState, ErrorState, LoadingState, Panel } from "../components/ui/primitives";
import { Modal } from "../components/ui/Modal";
import { useToast } from "../components/ui/Toast";
import { api } from "../data";
import type { Laboratorio } from "../data/gateway";

const STATUS_OPTIONS = ["ATIVO", "INATIVO"];

export default function LaboratorioPage() {
  const { toast } = useToast();
  const qc = useQueryClient();
  const [open, setOpen] = useState(false);
  const [edit, setEdit] = useState<Laboratorio | null>(null);
  const [toDelete, setToDelete] = useState<Laboratorio | null>(null);
  const [form, setForm] = useState({ codigo: "", nome: "", descricao: "", sitio: "", responsavel: "", especialidade: "", nivelAcessoMinimo: 0, status: "ATIVO" });

  const query = useQuery({
    queryKey: ["laboratorios"],
    queryFn: () => api.listLaboratorios(),
  });

  const saveMut = useMutation({
    mutationFn: async () => {
      const payload = { ...form, nivelAcessoMinimo: Number(form.nivelAcessoMinimo), responsavel: form.responsavel || null, especialidade: form.especialidade || null };
      if (edit) { await api.updateLaboratorio({ ...edit, ...payload }); }
      else { await api.createLaboratorio(payload as Omit<Laboratorio, "id">); }
    },
    onSuccess: () => {
      toast({ title: edit ? "Atualizado" : "Criado", msg: `Laboratório ${edit ? "atualizado" : "cadastrado"}.`, tone: "success" });
      qc.invalidateQueries({ queryKey: ["laboratorios"] });
      setOpen(false);
    },
    onError: (e) => toast({ title: "Falha", msg: (e as Error).message, tone: "error" }),
  });

  const deleteMut = useMutation({
    mutationFn: (id: number) => api.deleteLaboratorio(id),
    onSuccess: () => {
      toast({ title: "Removido", msg: "Laboratório excluído.", tone: "info" });
      qc.invalidateQueries({ queryKey: ["laboratorios"] });
      setToDelete(null);
    },
    onError: (e) => toast({ title: "Falha", msg: (e as Error).message, tone: "error" }),
  });

  const openNew = () => { setEdit(null); setForm({ codigo: "", nome: "", descricao: "", sitio: "", responsavel: "", especialidade: "", nivelAcessoMinimo: 0, status: "ATIVO" }); setOpen(true); };
  const openEdit = (item: Laboratorio) => {
    setEdit(item);
    setForm({ codigo: item.codigo, nome: item.nome, descricao: item.descricao, sitio: item.sitio, responsavel: item.responsavel ?? "", especialidade: item.especialidade ?? "", nivelAcessoMinimo: item.nivelAcessoMinimo, status: item.status });
    setOpen(true);
  };

  return (
    <AppShell title="CONFIGURAÇÕES" subtitle="Laboratórios">
      <div className="toolbar">
        <div className="grow" />
        <Button variant="primary" onClick={openNew}>
          <Plus size={14} /> Novo
        </Button>
      </div>

      <Panel title={<><FlaskConical size={14} /> Laboratórios ({query.data?.length ?? "—"})</>}>
        {query.isLoading ? (
          <LoadingState label="Carregando laboratórios…" />
        ) : query.isError ? (
          <ErrorState message={(query.error as Error).message} onRetry={query.refetch} />
        ) : query.data!.length === 0 ? (
          <EmptyState
            icon={<FlaskConical size={40} />}
            title="Nenhum laboratório cadastrado"
            description="Catálogo de laboratórios vazio."
            action={<Button variant="primary" onClick={openNew}><Plus size={14} /> Cadastrar</Button>}
          />
        ) : (
          <div className="anom-grid">
            {query.data!.map((item) => (
              <div key={item.id} className="anom-card" onClick={() => openEdit(item)}>
                <div className="row" style={{ justifyContent: "space-between" }}>
                  <span className="anom-card__code">{item.codigo}</span>
                  <span className="tag" style={{ color: item.status === "ATIVO" ? "var(--ok)" : "var(--text-3)" }}>{item.status}</span>
                </div>
                <div className="anom-card__name">{item.nome}</div>
                <div className="muted" style={{ fontSize: 12, display: "-webkit-box", WebkitLineClamp: 2, WebkitBoxOrient: "vertical", overflow: "hidden" }}>
                  {item.descricao}
                </div>
                <div className="anom-card__meta">
                  <span className="tag">{item.sitio}</span>
                  <span className="tag">Nível {item.nivelAcessoMinimo}</span>
                </div>
              </div>
            ))}
          </div>
        )}
      </Panel>

      <Modal open={open} onOpenChange={setOpen}
        title={edit ? "Editar Laboratório" : "Novo Laboratório"}
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
              <input className="input" placeholder="LAB-01" value={form.codigo} onChange={e => setForm(p => ({ ...p, codigo: e.target.value }))} />
            </div>
            <div className="field">
              <label className="hud-label">Status</label>
              <select className="select" value={form.status} onChange={e => setForm(p => ({ ...p, status: e.target.value }))}>
                {STATUS_OPTIONS.map(s => <option key={s} value={s}>{s}</option>)}
              </select>
            </div>
          </div>
          <div className="field">
            <label className="hud-label">Nome</label>
            <input className="input" placeholder="Laboratório de Contenção Alfa" value={form.nome} onChange={e => setForm(p => ({ ...p, nome: e.target.value }))} />
          </div>
          <div className="field">
            <label className="hud-label">Descrição</label>
            <textarea className="input" rows={2} value={form.descricao} onChange={e => setForm(p => ({ ...p, descricao: e.target.value }))} />
          </div>
          <div className="grid" style={{ gridTemplateColumns: "1fr 1fr" }}>
            <div className="field">
              <label className="hud-label">Sítio</label>
              <input className="input" placeholder="Sítio-19" value={form.sitio} onChange={e => setForm(p => ({ ...p, sitio: e.target.value }))} />
            </div>
            <div className="field">
              <label className="hud-label">Nível Acesso Mínimo</label>
              <input className="input" type="number" min={0} value={form.nivelAcessoMinimo} onChange={e => setForm(p => ({ ...p, nivelAcessoMinimo: Number(e.target.value) }))} />
            </div>
          </div>
          <div className="field">
            <label className="hud-label">Responsável</label>
            <input className="input" placeholder="Dr.º Nome" value={form.responsavel} onChange={e => setForm(p => ({ ...p, responsavel: e.target.value }))} />
          </div>
          <div className="field">
            <label className="hud-label">Especialidade</label>
            <input className="input" placeholder="Bioontologia" value={form.especialidade} onChange={e => setForm(p => ({ ...p, especialidade: e.target.value }))} />
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
