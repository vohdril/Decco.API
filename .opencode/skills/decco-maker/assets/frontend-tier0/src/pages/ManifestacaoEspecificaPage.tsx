import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Activity, Plus, Trash2 } from "lucide-react";
import { useState } from "react";
import { AppShell } from "../components/layout/AppShell";
import { Button, EmptyState, ErrorState, LoadingState, Panel } from "../components/ui/primitives";
import { Modal } from "../components/ui/Modal";
import { useToast } from "../components/ui/Toast";
import { api } from "../data";
import type { ManifestacaoEspecifica } from "../mocks/types";

export default function ManifestacaoEspecificaPage() {
  const { toast } = useToast();
  const qc = useQueryClient();
  const [open, setOpen] = useState(false);
  const [edit, setEdit] = useState<ManifestacaoEspecifica | null>(null);
  const [toDelete, setToDelete] = useState<ManifestacaoEspecifica | null>(null);
  const [form, setForm] = useState({ codigo: "", nome: "", descricao: "", tipo: "" });

  const query = useQuery({
    queryKey: ["manifestacoes-especificas"],
    queryFn: () => api.listManifestacoesEspecificas(),
  });

  const saveMut = useMutation({
    mutationFn: async () => {
      if (edit) { await api.updateManifestacaoEspecifica({ ...edit, ...form } as ManifestacaoEspecifica); }
      else { await api.createManifestacaoEspecifica(form as Omit<ManifestacaoEspecifica, "id">); }
    },
    onSuccess: () => {
      toast({ title: edit ? "Atualizado" : "Criado", msg: `Manifestação ${edit ? "atualizada" : "cadastrada"}.`, tone: "success" });
      qc.invalidateQueries({ queryKey: ["manifestacoes-especificas"] });
      setOpen(false);
    },
    onError: (e) => toast({ title: "Falha", msg: (e as Error).message, tone: "error" }),
  });

  const deleteMut = useMutation({
    mutationFn: (id: number) => api.deleteManifestacaoEspecifica(id),
    onSuccess: () => {
      toast({ title: "Removido", msg: "Manifestação excluída.", tone: "info" });
      qc.invalidateQueries({ queryKey: ["manifestacoes-especificas"] });
      setToDelete(null);
    },
    onError: (e) => toast({ title: "Falha", msg: (e as Error).message, tone: "error" }),
  });

  const openNew = () => { setEdit(null); setForm({ codigo: "", nome: "", descricao: "", tipo: "" }); setOpen(true); };
  const openEdit = (item: ManifestacaoEspecifica) => { setEdit(item); setForm({ codigo: item.codigo, nome: item.nome, descricao: item.descricao, tipo: item.tipo }); setOpen(true); };

  return (
    <AppShell title="CONFIGURAÇÕES" subtitle="Manifestações Específicas">
      <div className="toolbar">
        <div className="grow" />
        <Button variant="primary" onClick={openNew}>
          <Plus size={14} /> Novo
        </Button>
      </div>

      <Panel title={<><Activity size={14} /> Manifestações Específicas ({query.data?.length ?? "—"})</>}>
        {query.isLoading ? (
          <LoadingState label="Carregando manifestações…" />
        ) : query.isError ? (
          <ErrorState message={(query.error as Error).message} onRetry={query.refetch} />
        ) : query.data!.length === 0 ? (
          <EmptyState
            icon={<Activity size={40} />}
            title="Nenhuma manifestação cadastrada"
            action={<Button variant="primary" onClick={openNew}><Plus size={14} /> Cadastrar</Button>}
          />
        ) : (
          <div className="anom-grid">
            {query.data!.map((item) => (
              <div key={item.id} className="anom-card" onClick={() => openEdit(item)}>
                <div className="row" style={{ justifyContent: "space-between" }}>
                  <span className="anom-card__code">{item.codigo}</span>
                  <span className="muted" style={{ fontSize: 11 }}>{item.tipo}</span>
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
        title={edit ? "Editar Manifestação" : "Nova Manifestação"}
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
            <input className="input" placeholder="MAN-001" value={form.codigo} onChange={e => setForm(p => ({ ...p, codigo: e.target.value }))} />
          </div>
          <div className="field">
            <label className="hud-label">Nome</label>
            <input className="input" placeholder="Materialização Espontânea" value={form.nome} onChange={e => setForm(p => ({ ...p, nome: e.target.value }))} />
          </div>
          <div className="field">
            <label className="hud-label">Tipo</label>
            <input className="input" placeholder="Física" value={form.tipo} onChange={e => setForm(p => ({ ...p, tipo: e.target.value }))} />
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
