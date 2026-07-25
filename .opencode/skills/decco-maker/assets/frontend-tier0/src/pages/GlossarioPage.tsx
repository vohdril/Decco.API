import { useQuery } from "@tanstack/react-query";
import { AlertTriangle, Atom, BookOpen, BrainCircuit, Cpu, GitBranch, Layers, Radio, Sparkles, Square } from "lucide-react";
import { useCallback, useState } from "react";
import { AppShell } from "../components/layout/AppShell";
import { Modal } from "../components/ui/Modal";
import { Button, EmptyState, ErrorState, LoadingState, Panel, ProgressBar, Skeleton, Spinner, StatTile } from "../components/ui/primitives";
import { RobotDashCompleto } from "../components/robot/RobotAnatomy";
import { ParticleSphere } from "../components/robot/ParticleSphere";
import { api } from "../data";
import { CLASSES, CAMADAS } from "../mocks/data";
import type { CamadaSimbolo } from "../mocks/types";
import s from "./GlossarioPage.module.css";

const CAMADA_CORES: Record<CamadaSimbolo, string> = {
  THETA: "#34d3e6",
  PSI: "#ab47bc",
  PHI: "#66bb6a",
  OMEGA: "#ffa726",
};

export default function GlossarioPage() {
  const cogn = useQuery({ queryKey: ["cognicoes"], queryFn: () => api.listCognicoes() });
  const per = useQuery({ queryKey: ["periculosidades"], queryFn: () => api.listPericulosidades() });
  const mec = useQuery({ queryKey: ["mecanismos-interacao"], queryFn: () => api.listMecanismosInteracao() });
  const forc = useQuery({ queryKey: ["forcas-fundamentais"], queryFn: () => api.listForcasFundamentais() });

  const [coreGlow, setCoreGlow] = useState(false);
  const [showSecretModal, setShowSecretModal] = useState(false);

  const handleCoreClick = useCallback(() => {
    setCoreGlow(true);
    setTimeout(() => {
      setCoreGlow(false);
      setShowSecretModal(true);
    }, 2000);
  }, []);

  return (
    <AppShell title="GLOSSÁRIO" subtitle="Catálogos do DeccoDB">
      <div className={s.glossario}>

        <Panel title={<><Cpu size={14} /> Anatomia do Robô</>}>
          <div className={s["widget-center"]}>
            <RobotDashCompleto onCoreClick={handleCoreClick} glowBoost={coreGlow} />
            <p className={s["robot-legend"]}>
              Partes nomeadas: cabeça, testa, ponte nasal, órbitas, boca, grade, abertura do tronco,
              peitoral, braços, pernas, pés — cada uma editável independentemente.
            </p>
          </div>
        </Panel>

        <Panel title={<><Sparkles size={14} /> Orbe de Partículas</>}>
          <div className={s["widget-center"]}>
            <ParticleSphere size={280} />
            <p className={s["sphere-legend"]}>
              Esfera de pontos ciano com rotação e ondulação (canvas 2D, zero dependências).
              Upgrade FE Tier 3: <code>react-three-fiber</code> + shaders.
            </p>
          </div>
        </Panel>

        <Panel title={<><BookOpen size={14} /> Classes de Objeto</>}>
          <div className={s.grid}>
            {CLASSES.map((c) => (
              <div key={c.codigo} className={s.card} style={{ borderColor: c.corAlerta, background: `${c.corAlerta}0d` }}>
                <div className={s["card-header"]}>
                  <span className={s["card-code"]} style={{ color: c.corAlerta }}>{c.codigo}</span>
                  <span className={s.badge} style={{ color: c.corAlerta, borderColor: c.corAlerta }}>{c.classeAcs}</span>
                </div>
                <div className={s["card-name"]}>{c.nome}</div>
                <div className={s["card-meta"]}>
                  <span className={s.badge} style={{ color: "var(--text-2)", borderColor: "var(--border)" }}>
                    Nível {c.nivelAcessoMinimo}
                  </span>
                </div>
              </div>
            ))}
          </div>
        </Panel>

        <Panel title={<><Layers size={14} /> Camadas Ontológicas</>}>
          <div className={s.grid}>
            {CAMADAS.map((c) => {
              const cor = CAMADA_CORES[c.simbolo];
              return (
                <div key={c.simbolo} className={s.card} style={{ borderColor: cor, background: `${cor}0d` }}>
                  <div className={s["card-header"]}>
                    <span className={s["card-code"]} style={{ color: cor }}>{c.simbolo}</span>
                  </div>
                  <div className={s["card-name"]}>{c.nome}</div>
                  <div className={s["card-desc"]}>{c.descricao}</div>
                </div>
              );
            })}
          </div>
        </Panel>

        <Panel title={<><BrainCircuit size={14} /> Cognição Aparente</>}>
          {cogn.isLoading ? (
            <LoadingState label="Carregando cognicoes..." />
          ) : cogn.isError ? (
            <ErrorState message={(cogn.error as Error).message} onRetry={cogn.refetch} />
          ) : (
          <table className={s.tabela}>
            <thead>
              <tr>
                <th>Código</th>
                <th>Nome</th>
                <th>Descrição</th>
              </tr>
            </thead>
            <tbody>
              {(cogn.data ?? []).map((c) => (
                <tr key={c.id}>
                  <td style={{ fontFamily: "var(--font-mono)", fontWeight: 600 }}>{c.codigo}</td>
                  <td>{c.nome}</td>
                  <td className="muted" style={{ fontSize: 13 }}>{c.descricao}</td>
                </tr>
              ))}
            </tbody>
          </table>
          )}
        </Panel>

        <Panel title={<><AlertTriangle size={14} /> Periculosidade</>}>
          {per.isLoading ? (
            <LoadingState label="Carregando niveis de periculosidade..." />
          ) : per.isError ? (
            <ErrorState message={(per.error as Error).message} onRetry={per.refetch} />
          ) : (
          <table className={s.tabela}>
            <thead>
              <tr>
                <th>Nível</th>
                <th>Nome</th>
                <th>Descrição</th>
              </tr>
            </thead>
            <tbody>
              {(per.data ?? []).map((p) => (
                <tr key={p.id}>
                  <td><span className={s.badge} style={{ borderColor: "var(--border)" }}>{p.nivel}</span></td>
                  <td>{p.nome}</td>
                  <td className="muted" style={{ fontSize: 13 }}>{p.descricao}</td>
                </tr>
              ))}
            </tbody>
          </table>
          )}
        </Panel>

        <Panel title={<><Atom size={14} /> Forças Fundamentais</>}>
          {forc.isLoading ? (
            <LoadingState label="Carregando forcas fundamentais..." />
          ) : forc.isError ? (
            <ErrorState message={(forc.error as Error).message} onRetry={forc.refetch} />
          ) : (
          <table className={s.tabela}>
            <thead>
              <tr>
                <th>Símbolo</th>
                <th>Nome</th>
                <th>Descrição</th>
              </tr>
            </thead>
            <tbody>
              {(forc.data ?? []).map((f) => (
                <tr key={f.id}>
                  <td style={{ fontFamily: "var(--font-mono)", fontWeight: 600 }}>{f.simbolo}</td>
                  <td>{f.nome}</td>
                  <td className="muted" style={{ fontSize: 13 }}>{f.descricao}</td>
                </tr>
              ))}
            </tbody>
          </table>
          )}
        </Panel>

        <Panel title={<><Radio size={14} /> Mecanismos de Interação</>}>
          {mec.isLoading ? (
            <LoadingState label="Carregando mecanismos de interacao..." />
          ) : mec.isError ? (
            <ErrorState message={(mec.error as Error).message} onRetry={mec.refetch} />
          ) : (
          <table className={s.tabela}>
            <thead>
              <tr>
                <th>Código</th>
                <th>Nome</th>
                <th>Descrição</th>
              </tr>
            </thead>
            <tbody>
              {(mec.data ?? []).map((m) => (
                <tr key={m.id}>
                  <td style={{ fontFamily: "var(--font-mono)", fontWeight: 600 }}>{m.codigo}</td>
                  <td>{m.nome}</td>
                  <td className="muted" style={{ fontSize: 13 }}>{m.descricao}</td>
                </tr>
              ))}
            </tbody>
          </table>
          )}
        </Panel>

      </div>

      <Modal open={showSecretModal} onOpenChange={setShowSecretModal} className="modal__content--wide"
        title={<><Sparkles size={14} /> CATÁLOGO DE COMPONENTES</>}
        description="Todos os componentes do design system Decco com mini live demos. Toque nos controles para alternar cores e estados.">
        <div className={s.catalog}>

          <details className={s.catSection}>
            <summary className={s.catHead}><Cpu size={14} /> Inputs <span className={s.catBadge}>6</span></summary>
            <div className={s.catGrid}>
              <DemoCard title="Button">
                <ButtonDemo />
              </DemoCard>
              <DemoCard title="Input">
                <InputDemo />
              </DemoCard>
              <DemoCard title="Select">
                <SelectDemo />
              </DemoCard>
              <DemoCard title="Checkbox / Switch / Radio">
                <ToggleDemo />
              </DemoCard>
              <DemoCard title="Slider">
                <SliderDemo />
              </DemoCard>
              <DemoCard title="Rating">
                <RatingDemo />
              </DemoCard>
            </div>
          </details>

          <details className={s.catSection}>
            <summary className={s.catHead}><BookOpen size={14} /> Data Display <span className={s.catBadge}>6</span></summary>
            <div className={s.catGrid}>
              <DemoCard title="Badge">
                <BadgeDemo />
              </DemoCard>
              <DemoCard title="Chip">
                <ChipDemo />
              </DemoCard>
              <DemoCard title="Avatar">
                <AvatarDemo />
              </DemoCard>
              <DemoCard title="Tooltip">
                <TooltipDemo />
              </DemoCard>
              <DemoCard title="Divider">
                <DividerDemo />
              </DemoCard>
              <DemoCard title="Typography">
                <TypeDemo />
              </DemoCard>
            </div>
          </details>

          <details className={s.catSection}>
            <summary className={s.catHead}><AlertTriangle size={14} /> Feedback <span className={s.catBadge}>4</span></summary>
            <div className={s.catGrid}>
              <DemoCard title="Alert">
                <AlertDemo />
              </DemoCard>
              <DemoCard title="Progress">
                <ProgressDemo />
              </DemoCard>
              <DemoCard title="Skeleton">
                <SkeletonDemo />
              </DemoCard>
              <DemoCard title="Toast / Snackbar">
                <ToastDemo />
              </DemoCard>
            </div>
          </details>

          <details className={s.catSection}>
            <summary className={s.catHead}><Layers size={14} /> Navigation <span className={s.catBadge}>3</span></summary>
            <div className={s.catGrid}>
              <DemoCard title="Tabs">
                <TabsDemo />
              </DemoCard>
              <DemoCard title="Pagination">
                <PaginationDemo />
              </DemoCard>
              <DemoCard title="Breadcrumbs">
                <BreadcrumbDemo />
              </DemoCard>
            </div>
          </details>

          <details className={s.catSection}>
            <summary className={s.catHead}><Square size={14} /> Surface <span className={s.catBadge}>2</span></summary>
            <div className={s.catGrid}>
              <DemoCard title="Card">
                <CardDemo />
              </DemoCard>
              <DemoCard title="Accordion">
                <AccordionDemo />
              </DemoCard>
            </div>
          </details>

          <details className={s.catSection}>
            <summary className={s.catHead}><Sparkles size={14} /> Data Viz <span className={s.catBadge}>3</span></summary>
            <div className={s.catGrid}>
              <DemoCard title="StatTile">
                <StatTileDemo />
              </DemoCard>
              <DemoCard title="Chart">
                <ChartDemo />
              </DemoCard>
              <DemoCard title="Sphere">
                <SphereDemo />
              </DemoCard>
            </div>
          </details>

          <details className={s.catSection}>
            <summary className={s.catHead}><Radio size={14} /> Utils <span className={s.catBadge}>4</span></summary>
            <div className={s.catGrid}>
              <DemoCard title="LockedFeature">
                <LockedDemo />
              </DemoCard>
              <DemoCard title="DeviationWarning">
                <DeviationDemo />
              </DemoCard>
              <DemoCard title="Skeleton (texto)">
                <SkeletonTextDemo />
              </DemoCard>
              <DemoCard title="Spinner">
                <SpinnerDemo />
              </DemoCard>
            </div>
          </details>

        </div>
      </Modal>

    </AppShell>
  );
}

function DemoCard({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div className={s.demoCard}>
      <div className={s.demoCardTitle}>{title}</div>
      <div className={s.demoCardBody}>{children}</div>
    </div>
  );
}

function ButtonDemo() {
  const [color, setColor] = useState("accent");
  const [loading, setLoading] = useState(false);
  return (
    <>
      <div className={s.demoRow}>
        <button className={`btn btn--${color}`} disabled={loading}>
          {loading ? <Spinner size={12} /> : null}{loading ? "Carregando" : "Ação"}
        </button>
        <button className={`btn btn--${color}`} disabled>
          Desabilitado
        </button>
      </div>
      <div className={s.demoCtrls}>
        {["accent", "core", "danger"].map(c => (
          <button key={c} className={`btn ${color===c?"btn--primary":""}`} style={{fontSize:10,padding:"3px 8px"}} onClick={()=>setColor(c)}>{c}</button>
        ))}
        <label className={s.demoLabel}><input type="checkbox" checked={loading} onChange={e=>setLoading(e.target.checked)} /> loading</label>
      </div>
    </>
  );
}

function InputDemo() {
  const [state, setState] = useState("normal");
  return (
    <>
      <div className={s.demoRow} style={{flexDirection:"column",alignItems:"stretch"}}>
        <input className="input" placeholder="Digite algo…"
          {...(state==="error"?{style:{borderColor:"var(--danger)"}}:{})}
          {...(state==="disabled"?{disabled:true}:{})} />
      </div>
      <div className={s.demoCtrls}>
        {["normal","focus","error","disabled"].map(s => (
          <button key={s} className={`btn ${state===s?"btn--primary":""}`} style={{fontSize:10,padding:"3px 8px"}} onClick={()=>setState(s)}>{s}</button>
        ))}
      </div>
    </>
  );
}

function SelectDemo() {
  return (
    <div className={s.demoRow}>
      <select className="select" style={{maxWidth:180}}>
        <option>Pacato (SAFE)</option>
        <option>Yaguara (EUCLID)</option>
        <option>Abaporu (KETER)</option>
        <option>Ukar (THAUMIEL)</option>
      </select>
    </div>
  );
}

function ToggleDemo() {
  const [checked, setChecked] = useState(false);
  return (
    <div className={s.demoCol}>
      <label className={s.demoLabel}>
        <input type="checkbox" checked={checked} onChange={e=>setChecked(e.target.checked)} />
        <span style={{color:"var(--text-0)"}}> Checkbox</span>
      </label>
      <label className={s.demoLabel}>
        <input type="radio" name="rd" defaultChecked />
        <span style={{color:"var(--text-0)"}}> Radio 1</span>
      </label>
      <label className={s.demoLabel}>
        <input type="radio" name="rd" />
        <span style={{color:"var(--text-0)"}}> Radio 2</span>
      </label>
      <label className={s.demoLabel}>
        <span className={s.switch} data-on={checked} onClick={()=>setChecked(!checked)}>
          <span className={s.switchThumb} />
        </span>
        <span style={{color:"var(--text-0)"}}> Switch {checked?"ON":"OFF"}</span>
      </label>
    </div>
  );
}

function SliderDemo() {
  const [val, setVal] = useState(40);
  return (
    <div className={s.demoCol}>
      <div style={{display:"flex",alignItems:"center",gap:10}}>
        <span className="muted" style={{fontSize:11}}>0</span>
        <div style={{flex:1,height:6,background:"var(--bg-3)",borderRadius:6,position:"relative"}}>
          <div style={{width:`${val}%`,height:"100%",background:"linear-gradient(90deg,var(--accent-dim),var(--accent))",borderRadius:6,boxShadow:"0 0 8px var(--accent-glow)"}} />
        </div>
        <span className="muted" style={{fontSize:11}}>100</span>
      </div>
      <input type="range" min="0" max="100" value={val} onChange={e=>setVal(Number(e.target.value))}
        style={{width:"100%",accentColor:"var(--accent)"}} />
      <span className="mono" style={{fontSize:12,color:"var(--accent)"}}>{val}%</span>
    </div>
  );
}

function RatingDemo() {
  const [rate, setRate] = useState(3);
  return (
    <div className={s.demoRow}>
      {[1,2,3,4,5].map(i => (
        <span key={i} onClick={()=>setRate(i)}
          style={{cursor:"pointer",fontSize:22,color:i<=rate?"var(--accent-strong)":"var(--text-3)",transition:"color .15s ease",textShadow:i<=rate?"0 0 8px var(--accent-glow)":"none"}}>
          ★
        </span>
      ))}
    </div>
  );
}

function BadgeDemo() {
  const [color, setColor] = useState("accent");
  const colors: Record<string,string> = {accent:"var(--accent)",core:"var(--core)",ok:"var(--ok)",warn:"var(--warn)",danger:"var(--danger)"};
  return (
    <>
      <div className={s.demoRow}>
        <span className="badge" style={{borderColor:colors[color],color:colors[color],background:`${colors[color]}18`}}>
          <span className="badge__dot" style={{background:colors[color]}} />{color}
        </span>
      </div>
      <div className={s.demoCtrls}>
        {Object.keys(colors).map(c => (
          <button key={c} className={`btn ${color===c?"btn--primary":""}`} style={{fontSize:10,padding:"3px 8px"}} onClick={()=>setColor(c)}>{c}</button>
        ))}
      </div>
    </>
  );
}

function ChipDemo() {
  const [chips, setChips] = useState(["KETER","EUCLID","SAFE"]);
  const remove = (i:number) => setChips(chips.filter((_,idx)=>idx!==i));
  return (
    <div className={s.demoRow}>
      {chips.map((c,i) => (
        <span key={c} className="badge" style={{borderColor:"var(--accent-dim)",color:"var(--accent)"}}>
          {c}
          <span onClick={()=>remove(i)} style={{cursor:"pointer",marginLeft:4,opacity:.6}}>×</span>
        </span>
      ))}
      {chips.length===0 && <span className="muted" style={{fontSize:11}}>Todas removidas</span>}
    </div>
  );
}

function AvatarDemo() {
  return (
    <div className={s.demoRow}>
      {["DC","MI","KA","??"].map((init,i) => (
        <span key={i} className={s.avatar} style={{background:`radial-gradient(circle at 30% 30%, var(--accent), #0b3a44)`}}>
          {init}
        </span>
      ))}
    </div>
  );
}

function TooltipDemo() {
  return (
    <div className={s.demoRow} style={{gap:30}}>
      <div className={s.tooltipWrap}>
        <span className="btn">Hover me</span>
        <span className={s.tooltip}>Tooltip top</span>
      </div>
      <div className={s.tooltipWrap} style={{flexDirection:"column-reverse"}}>
        <span className="btn">Hover</span>
        <span className={s.tooltip}>Tooltip bottom</span>
      </div>
    </div>
  );
}

function DividerDemo() {
  return (
    <div className={s.demoCol}>
      <span className="muted" style={{fontSize:12}}>Acima da linha</span>
      <hr className={s.divider} />
      <span className="muted" style={{fontSize:12}}>Abaixo da linha</span>
      <hr className={s.divider} data-label="OU" />
    </div>
  );
}

function TypeDemo() {
  return (
    <div className={s.demoCol}>
      <div style={{fontFamily:"var(--font-mono)",fontSize:22,color:"var(--text-0)",letterSpacing:".15em"}}>TÍTULO</div>
      <div style={{fontFamily:"var(--font-mono)",fontSize:14,color:"var(--text-0)"}}>subtitulo <span className="muted">(label)</span></div>
      <div style={{fontSize:13,color:"var(--text-1)"}}>Corpo de texto com a fonte sans-serif do sistema. <span className="muted">Muted</span></div>
      <div className="mono" style={{fontSize:12,color:"var(--text-2)"}}>código / mono: font-family var(--font-mono)</div>
    </div>
  );
}

function AlertDemo() {
  const [sev, setSev] = useState("accent");
  const sevs: Record<string,string> = {accent:"var(--accent)",core:"var(--core)",ok:"var(--ok)",warn:"var(--warn)",danger:"var(--danger)"};
  return (
    <>
      <div className={s.demoCol}>
        <div style={{background:`${sevs[sev]}0d`,border:`1px solid ${sevs[sev]}`,borderRadius:"var(--r-sm)",padding:"10px 12px",display:"flex",alignItems:"center",gap:10}}>
          <span style={{color:sevs[sev],fontSize:16}}>●</span>
          <span style={{fontSize:12,color:"var(--text-1)"}}>Alerta com severidade <b style={{color:sevs[sev]}}>{sev}</b></span>
        </div>
      </div>
      <div className={s.demoCtrls}>
        {Object.keys(sevs).map(c => (
          <button key={c} className={`btn ${sev===c?"btn--primary":""}`} style={{fontSize:10,padding:"3px 8px"}} onClick={()=>setSev(c)}>{c}</button>
        ))}
      </div>
    </>
  );
}

function ProgressDemo() {
  const [val, setVal] = useState(60);
  const [tone, setTone] = useState<"accent"|"ok"|"warn"|"danger">("accent");
  return (
    <>
      <div className={s.demoCol}>
        <ProgressBar value={val} label="Progresso" tone={tone} />
        <div className={s.demoRow}>
          <Spinner size={16} />
          <span className="muted" style={{fontSize:12}}>Carregando…</span>
        </div>
      </div>
      <div className={s.demoCtrls}>
        <input type="range" min="0" max="100" value={val} onChange={e=>setVal(Number(e.target.value))} style={{width:80,accentColor:"var(--accent)"}} />
        {(["accent","ok","warn","danger"] as const).map(t => (
          <button key={t} className={`btn ${tone===t?"btn--primary":""}`} style={{fontSize:10,padding:"3px 8px"}} onClick={()=>setTone(t)}>{t}</button>
        ))}
      </div>
    </>
  );
}

function SkeletonDemo() {
  return (
    <div className={s.demoCol}>
      <div className={s.demoRow}>
        <div className="skeleton" style={{width:40,height:40,borderRadius:"var(--r-sm)"}} />
        <div style={{flex:1,display:"flex",flexDirection:"column",gap:6}}>
          <div className="skeleton" style={{width:"70%",height:12}} />
          <div className="skeleton" style={{width:"90%",height:12}} />
        </div>
      </div>
      <div className="skeleton" style={{width:"100%",height:60,borderRadius:"var(--r-sm)",marginTop:6}} />
    </div>
  );
}

function SkeletonTextDemo() {
  return (
    <div style={{display:"flex",flexDirection:"column",gap:8,width:"100%"}}>
      <div className="skeleton" style={{width:"60%",height:12}} />
      <div className="skeleton" style={{width:"100%",height:12}} />
      <div className="skeleton" style={{width:"80%",height:12}} />
    </div>
  );
}

function SpinnerDemo() {
  return (
    <div className={s.demoRow}>
      <Spinner size={14} />
      <Spinner size={20} />
      <Spinner size={28} />
    </div>
  );
}

function ToastDemo() {
  const [show, setShow] = useState(false);
  return (
    <div className={s.demoCol}>
      <button className="btn" onClick={()=>{setShow(true);setTimeout(()=>setShow(false),2000)}}>
        Mostrar Toast
      </button>
      {show && <div style={{background:"var(--bg-2)",border:"1px solid var(--border-strong)",borderLeft:"3px solid var(--ok)",borderRadius:"var(--r-sm)",padding:"8px 12px",fontSize:12,color:"var(--text-0)",animation:"fade-in .2s ease"}}>
        ✅ Operação concluída
      </div>}
    </div>
  );
}

function TabsDemo() {
  const [tab, setTab] = useState("desc");
  return (
    <div className={s.demoCol}>
      <div style={{display:"flex",borderBottom:"1px solid var(--border)"}}>
        {["desc","meta","hist"].map(t => (
          <button key={t} onClick={()=>setTab(t)}
            style={{fontFamily:"var(--font-mono)",fontSize:11,letterSpacing:".08em",textTransform:"uppercase",padding:"8px 14px",background:"transparent",border:"none",borderBottom:`2px solid ${tab===t?"var(--accent)":"transparent"}`,color:tab===t?"var(--accent)":"var(--text-2)",cursor:"pointer"}}>
            {t}
          </button>
        ))}
      </div>
      <div style={{fontSize:12,color:"var(--text-1)",padding:"8px 0"}}>Conteúdo da aba: <b>{tab}</b></div>
    </div>
  );
}

function PaginationDemo() {
  const [page, setPage] = useState(3);
  return (
    <div className={s.demoRow}>
      {[1,2,3,4,5].map(p => (
        <button key={p} onClick={()=>setPage(p)}
          style={{width:30,height:30,borderRadius:"var(--r-sm)",border:`1px solid ${page===p?"var(--accent)":"var(--border)"}`,background:page===p?"var(--accent-glow)":"transparent",color:page===p?"var(--accent)":"var(--text-1)",fontFamily:"var(--font-mono)",fontSize:12,cursor:"pointer"}}>
          {p}
        </button>
      ))}
    </div>
  );
}

function BreadcrumbDemo() {
  return (
    <div className={s.demoRow} style={{fontFamily:"var(--font-mono)",fontSize:12,color:"var(--text-2)"}}>
      <span style={{color:"var(--accent)",cursor:"pointer"}}>Home</span>
      <span style={{color:"var(--text-3)"}}>/</span>
      <span style={{color:"var(--accent)",cursor:"pointer"}}>Catálogo</span>
      <span style={{color:"var(--text-3)"}}>/</span>
      <span style={{color:"var(--text-1)"}}>Anomalias</span>
    </div>
  );
}

function CardDemo() {
  return (
    <div style={{background:"var(--bg-2)",border:"1px solid var(--border)",borderRadius:"var(--r-md)",padding:14,display:"flex",flexDirection:"column",gap:8,width:"100%",transition:"all .15s ease"}}
      className={s.cardHover}>
      <div style={{fontFamily:"var(--font-mono)",fontSize:12,color:"var(--accent)",letterSpacing:".1em"}}>SCP-1001</div>
      <div style={{fontSize:13,color:"var(--text-0)"}}>Nome da Anomalia</div>
      <div className="muted" style={{fontSize:11}}>Descrição resumida do card no grid.</div>
      <span className="badge" style={{borderColor:"var(--accent-dim)",color:"var(--accent)",alignSelf:"flex-start"}}>
        <span className="badge__dot" style={{background:"var(--accent)"}} />KETER
      </span>
    </div>
  );
}

function AccordionDemo() {
  const [open, setOpen] = useState(0);
  return (
    <div className={s.demoCol}>
      {["Item 1","Item 2","Item 3"].map((label,i) => (
        <div key={i} style={{border:"1px solid var(--border)",borderRadius:"var(--r-sm)",overflow:"hidden"}}>
          <button onClick={()=>setOpen(open===i?-1:i)}
            style={{width:"100%",padding:"8px 12px",background:"var(--bg-2)",border:"none",borderBottom:open===i?"1px solid var(--border)":"none",color:"var(--text-0)",fontFamily:"var(--font-mono)",fontSize:12,cursor:"pointer",textAlign:"left",display:"flex",justifyContent:"space-between"}}>
            {label} <span style={{transition:"transform .2s",transform:open===i?"rotate(180deg)":"none"}}>▾</span>
          </button>
          {open===i && <div style={{padding:"8px 12px",fontSize:12,color:"var(--text-1)"}}>Conteúdo expansível do {label}.</div>}
        </div>
      ))}
    </div>
  );
}

function StatTileDemo() {
  return (
    <div className={s.demoRow}>
      <StatTile label="Anomalias" value={42} sub="Ativas" accent />
      <StatTile label="Segurança" value="SAFE" sub="Nível 1" />
    </div>
  );
}

function ChartDemo() {
  return (
    <svg viewBox="0 0 200 60" style={{width:"100%",height:60}}>
      <defs><linearGradient id="chartGrad" x1="0" y1="0" x2="0" y2="1"><stop offset="0%" stopColor="var(--accent)" stopOpacity=".4" /><stop offset="100%" stopColor="var(--accent)" stopOpacity="0" /></linearGradient></defs>
      <path d="M0,50 L10,45 L20,48 L30,30 L40,35 L50,20 L60,25 L70,10 L80,15 L90,5 L100,8 L110,12 L120,22 L130,18 L140,28 L150,15 L160,20 L170,10 L180,12 L190,8 L200,10" fill="none" stroke="var(--accent)" strokeWidth="2" filter="drop-shadow(0 0 4px var(--accent-glow))" />
      <path d="M0,50 L10,45 L20,48 L30,30 L40,35 L50,20 L60,25 L70,10 L80,15 L90,5 L100,8 L110,12 L120,22 L130,18 L140,28 L150,15 L160,20 L170,10 L180,12 L190,8 L200,10 L200,60 L0,60 Z" fill="url(#chartGrad)" opacity=".4" />
    </svg>
  );
}

function SphereDemo() {
  const canvasRef = useCallback((canvas: HTMLCanvasElement | null) => {
    if (!canvas) return;
    const ctx = canvas.getContext("2d"); if (!ctx) return;
    const cx=40, cy=40, r=28;
    let angle = 0;
    const pts: {x:number;y:number;z:number}[] = [];
    for (let i = 0; i < 60; i++) {
      const theta = Math.random() * Math.PI * 2;
      const phi = Math.acos(2 * Math.random() - 1);
      pts.push({x: Math.sin(phi)*Math.cos(theta), y: Math.sin(phi)*Math.sin(theta), z: Math.cos(phi)});
    }
    function draw() {
      if (!ctx) return;
      angle += 0.01;
      ctx.clearRect(0,0,80,80);
      const proj = pts.map(p => {
        const x2 = p.x * Math.cos(angle) - p.z * Math.sin(angle);
        const z2 = p.x * Math.sin(angle) + p.z * Math.cos(angle);
        return {px: cx + x2 * r, py: cy + p.y * r, depth: z2};
      }).sort((a,b) => a.depth - b.depth);
      for (const p of proj) {
        const alpha = 0.3 + (p.depth + 1) * 0.35;
        ctx.beginPath(); ctx.arc(p.px, p.py, 2, 0, Math.PI*2);
        ctx.fillStyle = `rgba(111, 240, 255, ${alpha})`;
        ctx.fill();
      }
      requestAnimationFrame(draw);
    }
    draw();
  }, []);
  return (
    <div style={{display:"flex",justifyContent:"center"}}>
      <canvas ref={canvasRef} width={80} height={80} style={{borderRadius:"var(--r-sm)",background:"var(--bg-0)"}} />
    </div>
  );
}

function LockedDemo() {
  return (
    <div style={{background:"var(--bg-2)",border:"1px solid var(--danger)",borderRadius:"var(--r-md)",padding:"10px 12px",display:"flex",alignItems:"center",gap:10}}>
      <span style={{color:"var(--danger)",fontSize:16}}>🔒</span>
      <div>
        <div style={{fontFamily:"var(--font-mono)",fontSize:11,letterSpacing:".08em",color:"var(--danger)",textTransform:"upperCase"}}>Funcionalidade Bloqueada</div>
        <div className="muted" style={{fontSize:11}}>Requer FE Tier 2 (Auth) e BE Tier 2 (Auth)</div>
      </div>
    </div>
  );
}

function DeviationDemo() {
  return (
    <div style={{background:"linear-gradient(135deg, rgba(255, 179, 71, 0.08), rgba(244, 67, 54, 0.06))",border:"1px solid var(--core)",borderRadius:"var(--r-md)",padding:"10px 12px"}}>
      <div style={{fontFamily:"var(--font-mono)",fontSize:10,letterSpacing:".08em",color:"var(--core)",marginBottom:6,textTransform:"upperCase"}}>⚠ Desvio de Sequência</div>
      <div className="muted" style={{fontSize:11}}>Está no DB2 mas falta o FE Tier-1. Considere voltar.</div>
    </div>
  );
}
