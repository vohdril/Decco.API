/* =========================================================================
   Tipos do Tier-0 (domínio DeccoDB). Só o necessário para o Tier-0:
   Anomalia (agregado) + catálogos + Incidente (histórico) + User (auth).
   No contrato público expõe-se CÓDIGO (não Id interno) — padrão IdToCode.
   ========================================================================= */

export type ClasseCodigo = "PACATO" | "YAGUARA" | "ABAPORU" | "UKAR";
export type CamadaSimbolo = "THETA" | "PSI" | "PHI" | "OMEGA";
export type AnomaliaStatus = "ATIVA" | "NEUTRALIZADA" | "EM_PESQUISA";

export interface ClasseObjeto {
  codigo: ClasseCodigo;
  nome: string;
  classeAcs: string;       // SAFE / EUCLID / KETER / THAUMIEL
  corAlerta: string;       // #hex — dirige a severidade na UI
  nivelAcessoMinimo: number; // clearance mínimo p/ ver (1..4)
}

export interface CamadaOntologica {
  simbolo: CamadaSimbolo;
  nome: string;
  descricao: string;
}

export interface ForcaFundamental {
  id: number;
  simbolo: string;
  nome: string;
  descricao: string;
  particulaPortadora: string | null;
}

export interface MecanismoInteracao {
  id: number;
  codigo: string;          // THETA-A, PSI-C, OMEGA-A...
  nome: string;
  descricao: string;
  camadaOntologicaId: number;
  ehSubnatureza: boolean;
}

export interface PericiaAnomalia {
  id: number;
  codigo: string;
  nome: string;
  descricao: string;
  nivel: number;
}

export interface ManifestacaoEspecifica {
  id: number;
  codigo: string;
  nome: string;
  descricao: string;
  tipo: string;
}

export interface InstanciaDeviante {
  id: number;
  codigo: string;
  nome: string;
  descricao: string;
  anomaliaCodigoSCP: string;
  status: string;
}

export interface Anomalia {
  id: number;
  codigoSCP: string;
  nomeComum: string;
  descricao: string;
  classeObjeto: string;
  camadaOntologica: string;
  tipoMateria: string;
  mecanismoPrimario: string;
  mecanismoSecundario: string | null;
  ieiaDBase: number | null;
  fatorCoerenciaSpin: string | null;
  status: string;
  sitioContencao: string | null;
  responsavelPesquisa: string | null;
  dataCriacao: string | null;
  dataAtualizacao: string | null;
}

export interface Incidente {
  id: number;
  anomaliaCodigoScp: string;
  dataHora: string;        // ISO
  tipo: string;
  titulo: string;
  nivelSeguranca: string;
  isEventoSigma: boolean;
  mortes: number;
  feridos: number;
}

export type UserRole = "PESQUISADOR" | "AGENTE_CONTENCAO" | "DIRETOR_SITIO" | "O5";

export interface User {
  id: number;
  username: string;
  nome: string;
  role: UserRole;
  clearance: number;       // 1..4 (mapeia a nivelAcessoMinimo das classes)
  sites: string[];         // sítios que pode ver ("*" = todos)
}

export interface DashboardStats {
  totalAnomalias: number;
  ativas: number;
  incidentesSigma: number;
  coreStability: number;   // %
  porClasse: { classe: ClasseCodigo; total: number; cor: string }[];
  ieiaSerie: { t: string; v: number }[];
}
