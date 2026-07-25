import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Plus, Radio, Trash2 } from "lucide-react";
import { useState } from "react";
import { AppShell } from "../components/layout/AppShell";
import { Button, EmptyState, ErrorState, LoadingState, Panel } from "../components/ui/primitives";
import { Modal } from "../components/ui/Modal";
import { useToast } from "../components/ui/Toast";
import { api } from "../data";
import type { MecanismoInteracao } from "../mocks/types";

export default function MecanismoInteracaoPage() {
  const { toast } = useToast();
  const qc = useQueryClient();
  const [open, setOpen] = useState(false);
  const [edit, setEdit] = useState<MecanismoInteracao | null>(null);
  const [toDelete, setToDelete] = useState<MecanismoInteracao | null>(null);
  const [form, setForm] = useState({ codigo: "", nome: "", descricao: "", camadaOntologicaId: 1, ehSubnatureza: false });

  const query = useQuery({
    queryKey: ["mecanismos-interacao"],
    queryFn: () => api.listMecanismosInteracao(),
  });

  const saveMut = useMutation({
    mutationFn: async () => {
      if (edit) { await api.updateMecanismoInteracao({ ...edit, ...form } as unknown as MecanismoInteracao); }
      else { await api.createMecanismoInteracao(form as unknown as Omit<MecanismoInteracao, "id">); }
    },
    onSuccess: () => {
      toast({ title: edit ? "Atualizado" : "Criado", msg: `Mecanismo ${edit ? "atualizado" : "cadastrado"}.`, tone: "success" });
      qc.invalidateQueries({ queryKey: ["mecanismos-interacao"] });
      setOpen(false);
    },
    onError: (e) => toast({ title: "Falha", msg: (e as Error).message, tone: "error" }),
  });

  const deleteMut = useMutation({
    mutationFn: (id: number) => api.deleteMecanismoInteracao(id),
    onSuccess: () => {
      toast({ title: "Removido", msg: "Mecanismo excluído.", tone: "info" });
      qc.invalidateQueries({ queryKey: ["mecanismos-interacao"] });
      setToDelete(null);
    },
    onError: (e) => toast({ title: "Falha", msg: (e as Error).message, tone: "error" }),
  });

  const openNew = () => { setEdit(null); setForm({ codigo: "", nome: "", descricao: "", camadaOntologicaId: 1, ehSubnatureza: false }); setOpen(true); };
  const openEdit = (item: MecanismoInteracao) => { setEdit(item); setForm({ codigo: item.codigo, nome: item.nome, descricao: item.descricao, camadaOntologicaId: item.camadaOntologicaId, ehSubnatureza: item.ehSubnatureza }); setOpen(true); };

  return (
    <AppShell title="CONFIGURAÇÕES" subtitle="Mecanismos de Interação">
      <div className="toolbar">
        <div className="grow" />
        <Button variant="primary" onClick={openNew}>
          <Plus size={14} /> Novo
        </Button>
      </div>

      <Panel title={<><Radio size={14} /> Mecanismos de Interação ({query.data?.length ?? "—"})</>}>
        {query.isLoading ? (
          <LoadingState label="Carregando mecanismos…" />
        ) : query.isError ? (
          <ErrorState message={(query.error as Error).message} onRetry={query.refetch} />
        ) : query.data!.length === 0 ? (
          <EmptyState
            icon={<Radio size={40} />}
            title="Nenhum mecanismo cadastrado"
            action={<Button variant="primary" onClick={openNew}><Plus size={14} /> Cadastrar</Button>}
          />
        ) : (
          <div className="anom-grid">
            {query.data!.map((item) => (
              <div key={item.id} className="anom-card" onClick={() => openEdit(item)}>
                <div className="row" style={{ justifyContent: "space-between" }}>
                  <span className="anom-card__code">{item.codigo}</span>
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
        title={edit ? "Editar Mecanismo" : "Novo Mecanismo"}
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
            <input className="input" placeholder="THETA-A" value={form.codigo} onChange={e => setForm(p => ({ ...p, codigo: e.target.value }))} />
          </div>
          <div className="field">
            <label className="hud-label">Nome</label>
            <input className="input" placeholder="Theta-Ativo" value={form.nome} onChange={e => setForm(p => ({ ...p, nome: e.target.value }))} />
          </div>
          <div className="field">
            <label className="hud-label">Descrição</label>
            <textarea className="input" rows={3} value={form.descricao} onChange={e => setForm(p => ({ ...p, descricao: e.target.value }))} />
          </div>
          <div className="field">
            <label className="hud-label">ID Camada Ontológica</label>
            <input className="input" type="number" min={1} value={form.camadaOntologicaId} onChange={e => setForm(p => ({ ...p, camadaOntologicaId: Number(e.target.value) }))} />
          </div>
          <div className="field">
            <label className="hud-label">Subnatureza</label>
            <input type="checkbox" checked={form.ehSubnatureza} onChange={e => setForm(p => ({ ...p, ehSubnatureza: e.target.checked }))} />
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
