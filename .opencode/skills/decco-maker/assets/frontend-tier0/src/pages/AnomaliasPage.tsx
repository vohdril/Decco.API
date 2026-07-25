import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import * as Tabs from "@radix-ui/react-tabs";
import { BarChart3, Boxes, ImageOff, Plus, Radar, Trash2 } from "lucide-react";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { useAuth } from "../auth/auth";
import { AppShell } from "../components/layout/AppShell";
import { Modal } from "../components/ui/Modal";
import {
  Button, EmptyState, ErrorState, Panel, SeverityBadge, Skeleton, Spinner,
} from "../components/ui/primitives";
import { useToast } from "../components/ui/Toast";
import { api } from "../data";
import { CLASSES, MECANISMOS } from "../mocks/data";
import type { Anomalia } from "../mocks/types";

const createSchema = z.object({
  codigoSCP: z.string().regex(/^SCP-\d{3,}$/i, "Formato: SCP-#### (ex.: SCP-2001)."),
  nomeComum: z.string().min(3, "Mínimo 3 caracteres."),
  descricao: z.string().min(10, "Descreva a anomalia (≥10 caracteres)."),
  classeObjeto: z.enum(["PACATO", "YAGUARA", "ABAPORU", "UKAR"]),
  camadaOntologica: z.enum(["THETA", "PSI", "PHI", "OMEGA"]),
  mecanismoPrimario: z.string().min(1, "Selecione um mecanismo."),
  sitioContencao: z.string().min(1, "Informe o sítio."),
});
type CreateForm = z.infer<typeof createSchema>;

const CLASSE_NOME: Record<string, string> = Object.fromEntries(CLASSES.map((c) => [c.codigo, c.nome]));
const CAMADA_NOME: Record<string, string> = { THETA: "Theta", PSI: "Psi", PHI: "Phi", OMEGA: "Omega" };
const MECANISMO_NOME: Record<string, string> = Object.fromEntries(MECANISMOS.map((m) => [m.codigo, m.nome]));

export function AnomaliasPage() {
  const { user } = useAuth();
  const { toast } = useToast();
  const qc = useQueryClient();
  const [createOpen, setCreateOpen] = useState(false);
  const [detail, setDetail] = useState<Anomalia | null>(null);
  const [toDelete, setToDelete] = useState<Anomalia | null>(null);

  const query = useQuery({
    queryKey: ["anomalias", user?.id],
    queryFn: () => api.listAnomalias(user!),
    enabled: !!user,
  });

  const createMut = useMutation({
    mutationFn: (input: CreateForm) => api.createAnomalia({
      id: 0, codigoSCP: input.codigoSCP, nomeComum: input.nomeComum,
      descricao: input.descricao,
      classeObjeto: CLASSE_NOME[input.classeObjeto] ?? input.classeObjeto,
      camadaOntologica: CAMADA_NOME[input.camadaOntologica] ?? input.camadaOntologica,
      tipoMateria: "Bariônica Anômala", mecanismoPrimario: MECANISMO_NOME[input.mecanismoPrimario] ?? input.mecanismoPrimario, mecanismoSecundario: null,
      ieiaDBase: null, fatorCoerenciaSpin: null, status: "ATIVA",
      sitioContencao: input.sitioContencao, responsavelPesquisa: user!.nome,
    }),
    onSuccess: (a) => {
      toast({ title: "Anomalia registrada", msg: `${a.codigoSCP} adicionada ao catálogo.`, tone: "success" });
      qc.invalidateQueries({ queryKey: ["anomalias"] });
      qc.invalidateQueries({ queryKey: ["dashboard"] });
      setCreateOpen(false);
    },
    onError: (e) => toast({ title: "Falha ao registrar", msg: (e as Error).message, tone: "error" }),
  });

  const deleteMut = useMutation({
    mutationFn: (code: string) => api.deleteAnomalia(code),
    onSuccess: (_v, code) => {
      toast({ title: "Anomalia removida", msg: `${code} descatalogada.`, tone: "info" });
      qc.invalidateQueries({ queryKey: ["anomalias"] });
      qc.invalidateQueries({ queryKey: ["dashboard"] });
      setToDelete(null);
    },
  });

  const form = useForm<CreateForm>({
    resolver: zodResolver(createSchema),
    defaultValues: { codigoSCP: "", nomeComum: "", descricao: "", classeObjeto: "PACATO", camadaOntologica: "THETA", mecanismoPrimario: "", sitioContencao: user?.sites.find((s) => s !== "*") ?? "Sitio-19" },
  });

  return (
    <AppShell title="ANOMALIAS" subtitle="Catálogo ontológico · Tier-0">
      <div className="toolbar">
        <div className="grow" />
        <Button variant="primary" onClick={() => { form.reset(); setCreateOpen(true); }}>
          <Plus size={14} /> Nova Anomalia
        </Button>
      </div>

      <Panel title={<><Boxes size={14} /> Registros visíveis ({query.data?.length ?? "—"})</>}>
        {query.isLoading ? (
          <div className="anom-grid">
            {Array.from({ length: 4 }).map((_, i) => (
              <div key={i} className="anom-card" style={{ cursor: "default" }}>
                <Skeleton w={90} h={12} />
                <div style={{ height: 8 }} />
                <Skeleton w="80%" h={16} />
                <div style={{ height: 12 }} />
                <Skeleton w="100%" h={10} /><div style={{ height: 6 }} /><Skeleton w="60%" h={10} />
              </div>
            ))}
          </div>
        ) : query.isError ? (
          <ErrorState message={(query.error as Error).message} onRetry={query.refetch} />
        ) : query.data!.length === 0 ? (
          <EmptyState
            icon={<Boxes size={40} />}
            title="Nenhuma anomalia no seu escopo"
            description="Não há registros para o seu clearance/sítio neste cenário. Registre uma nova anomalia ou ajuste o cenário."
            action={<Button variant="primary" onClick={() => { form.reset(); setCreateOpen(true); }}><Plus size={14} /> Registrar</Button>}
          />
        ) : (
          <div className="anom-grid">
            {query.data!.map((a) => (
              <div key={a.codigoSCP} className="anom-card" onClick={() => setDetail(a)}>
                <div className="row" style={{ justifyContent: "space-between" }}>
                  <span className="anom-card__code">{a.codigoSCP}</span>
                  <SeverityBadge classe={a.classeObjeto} />
                </div>
                <div className="anom-card__name">{a.nomeComum}</div>
                <div className="muted" style={{ fontSize: 12, display: "-webkit-box", WebkitLineClamp: 2, WebkitBoxOrient: "vertical", overflow: "hidden" }}>
                  {a.descricao}
                </div>
                <div className="anom-card__meta">
                  <span className="tag">{a.camadaOntologica}</span>
                  <span className="tag">{a.mecanismoPrimario}</span>
                  <span className="tag">{a.sitioContencao}</span>
                </div>
              </div>
            ))}
          </div>
        )}
      </Panel>

      {/* DETALHE */}
      <Modal open={!!detail} onOpenChange={(o) => !o && setDetail(null)}
        title={detail ? `${detail.codigoSCP} · ${detail.nomeComum}` : ""}
        footer={detail && (
          <>
            <Button variant="danger" onClick={() => { setToDelete(detail); setDetail(null); }}>
              <Trash2 size={14} /> Descatalogar
            </Button>
            <Button onClick={() => setDetail(null)}>Fechar</Button>
          </>
        )}>
        {detail && (
          <Tabs.Root defaultValue="info" className="tabs">
            <Tabs.List className="tabs__list">
              <Tabs.Trigger value="info" className="tabs__trigger">Informações</Tabs.Trigger>
              <Tabs.Trigger value="manifestacoes" className="tabs__trigger">Manifestações</Tabs.Trigger>
              <Tabs.Trigger value="estatisticas" className="tabs__trigger">Estatísticas</Tabs.Trigger>
            </Tabs.List>

            <Tabs.Content value="info" className="tabs__content">
              <div style={{ display: "grid", gridTemplateColumns: "1fr 160px", gap: 16 }}>
                <div className="col" style={{ gap: 14 }}>
                  <div className="row" style={{ gap: 8, flexWrap: "wrap" }}>
                    <SeverityBadge classe={detail.classeObjeto} />
                    <span className="tag">{detail.status}</span>
                    <span className="tag">{detail.camadaOntologica}</span>
                  </div>
                  <p style={{ margin: 0, color: "var(--text-1)", fontSize: 13 }}>{detail.descricao}</p>
                  <dl className="dl" style={{ gridTemplateColumns: "140px 1fr" }}>
                    <dt>Código SCP</dt><dd className="mono">{detail.codigoSCP}</dd>
                    <dt>Nome Comum</dt><dd>{detail.nomeComum}</dd>
                    <dt>Classe</dt><dd>{detail.classeObjeto}</dd>
                    <dt>Camada Ontológica</dt><dd>{detail.camadaOntologica}</dd>
                    <dt>Tipo de Matéria</dt><dd>{detail.tipoMateria}</dd>
                    <dt>Mecanismo Primário</dt><dd className="mono">{detail.mecanismoPrimario}</dd>
                    <dt>Mecanismo Secundário</dt><dd className="mono">{detail.mecanismoSecundario ?? "—"}</dd>
                    <dt>IEIA-D Base</dt><dd className="mono">{detail.ieiaDBase ?? "—"}</dd>
                    <dt>Fator Coerência Spin</dt><dd className="mono">{detail.fatorCoerenciaSpin ?? "—"}</dd>
                    <dt>Status</dt><dd>{detail.status}</dd>
                    <dt>Sítio de Contenção</dt><dd>{detail.sitioContencao ?? "—"}</dd>
                    <dt>Responsável</dt><dd>{detail.responsavelPesquisa ?? "—"}</dd>
                  </dl>
                </div>
                <div style={{
                  background: "var(--bg-3)", border: "1px solid var(--border)", borderRadius: "var(--r-md)",
                  display: "flex", flexDirection: "column", alignItems: "center", justifyContent: "center",
                  gap: 8, padding: 16, aspectRatio: "3/4", color: "var(--text-3)", fontSize: 11,
                  fontFamily: "var(--font-mono)", textAlign: "center"
                }}>
                  <ImageOff size={28} />
                  <span>Nenhuma imagem disponível</span>
                </div>
              </div>
            </Tabs.Content>

            <Tabs.Content value="manifestacoes" className="tabs__content">
              <EmptyState
                icon={<Radar size={40} />}
                title="Nenhuma manifestação registrada"
                description="Manifestações de anomalias serão listadas aqui quando implementado no Tier 2."
              />
            </Tabs.Content>

            <Tabs.Content value="estatisticas" className="tabs__content">
              <EmptyState
                icon={<BarChart3 size={40} />}
                title="Estatísticas não disponíveis"
                description="Os dados estatísticos serão implementados em tiers futuros."
              />
            </Tabs.Content>
          </Tabs.Root>
        )}
      </Modal>

      {/* CONFIRM DELETE */}
      <Modal open={!!toDelete} onOpenChange={(o) => !o && setToDelete(null)}
        title="Confirmar descatalogação"
        description="Esta ação remove a anomalia do catálogo (mock, apenas na sessão)."
        footer={
          <>
            <Button onClick={() => setToDelete(null)}>Cancelar</Button>
            <Button variant="danger" disabled={deleteMut.isPending} onClick={() => toDelete && deleteMut.mutate(toDelete.codigoSCP)}>
              {deleteMut.isPending ? <><Spinner size={14} /> Removendo…</> : "Confirmar"}
            </Button>
          </>
        }>
        {toDelete && <p style={{ margin: 0 }}>Remover <b className="accent">{toDelete.codigoSCP}</b> — {toDelete.nomeComum}?</p>}
      </Modal>

      {/* CREATE */}
      <Modal open={createOpen} onOpenChange={setCreateOpen}
        title="Registrar nova anomalia"
        description="Preenchimento mínimo do Tier-0. Validação via react-hook-form + zod."
        footer={
          <>
            <Button onClick={() => setCreateOpen(false)}>Cancelar</Button>
            <Button variant="primary" form="create-anom" type="submit" disabled={createMut.isPending}>
              {createMut.isPending ? <><Spinner size={14} /> Registrando…</> : "Registrar"}
            </Button>
          </>
        }>
        <form id="create-anom" className="col" style={{ gap: 12 }}
          onSubmit={form.handleSubmit((d) => createMut.mutate(d))} noValidate>
          <div className="grid" style={{ gridTemplateColumns: "1fr 1fr" }}>
            <div className="field">
              <label className="hud-label">Código SCP</label>
              <input className="input" placeholder="SCP-2001" {...form.register("codigoSCP")} />
              {form.formState.errors.codigoSCP && <span className="field__error">{form.formState.errors.codigoSCP.message}</span>}
            </div>
            <div className="field">
              <label className="hud-label">Sítio</label>
              <input className="input" {...form.register("sitioContencao")} />
              {form.formState.errors.sitioContencao && <span className="field__error">{form.formState.errors.sitioContencao.message}</span>}
            </div>
          </div>
          <div className="field">
            <label className="hud-label">Nome comum</label>
            <input className="input" placeholder="ex.: O Relógio que Sussurra" {...form.register("nomeComum")} />
            {form.formState.errors.nomeComum && <span className="field__error">{form.formState.errors.nomeComum.message}</span>}
          </div>
          <div className="field">
            <label className="hud-label">Descrição</label>
            <textarea className="input" rows={3} {...form.register("descricao")} />
            {form.formState.errors.descricao && <span className="field__error">{form.formState.errors.descricao.message}</span>}
          </div>
          <div className="grid" style={{ gridTemplateColumns: "1fr 1fr 1fr" }}>
            <div className="field">
              <label className="hud-label">Classe</label>
              <select className="select" {...form.register("classeObjeto")}>
                {CLASSES.map((c) => <option key={c.codigo} value={c.codigo}>{c.codigo}</option>)}
              </select>
            </div>
            <div className="field">
              <label className="hud-label">Camada</label>
              <select className="select" {...form.register("camadaOntologica")}>
                <option value="THETA">THETA</option>
                <option value="PSI">PSI</option>
                <option value="PHI">PHI</option>
                <option value="OMEGA">OMEGA</option>
              </select>
            </div>
            <div className="field">
              <label className="hud-label">Mecanismo</label>
              <select className="select" {...form.register("mecanismoPrimario")}>
                <option value="">—</option>
                {MECANISMOS.map((m) => <option key={m.codigo} value={m.codigo}>{m.codigo}</option>)}
              </select>
              {form.formState.errors.mecanismoPrimario && <span className="field__error">{form.formState.errors.mecanismoPrimario.message}</span>}
            </div>
          </div>
        </form>
      </Modal>
    </AppShell>
  );
}
