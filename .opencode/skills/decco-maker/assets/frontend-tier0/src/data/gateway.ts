import type {
  Anomalia, CamadaOntologica, ClasseObjeto, DashboardStats, ForcaFundamental,
  Incidente, InstanciaDeviante, ManifestacaoEspecifica, MecanismoInteracao,
  PericiaAnomalia, User,
} from "../mocks/types";

/* =========================================================================
   CONTRATO DE DADOS do front (o "gateway"). A UI depende SÓ desta interface —
   a implementação está em httpApi.ts.
   ========================================================================= */

export interface CatCognicaoAparente {
  id: number;
  codigo: string;
  nome: string;
  descricao: string;
}

export interface CatPericulosidade {
  id: number;
  nivel: number;
  nome: string;
  descricao: string;
  corAlerta: string | null;
}

export interface Laboratorio {
  id: number;
  codigo: string;
  nome: string;
  descricao: string;
  sitio: string;
  responsavel: string | null;
  especialidade: string | null;
  nivelAcessoMinimo: number;
  status: string;
  dataCriacao: string | null;
  dataAtualizacao: string | null;
}

export interface ProtocoloContencao {
  id: number;
  codigo: string;
  titulo: string;
  descricao: string;
  nivelUrgencia: number;
  classesAplicaveis: string | null;
  passos: string;
  recursosNecessarios: string | null;
  dataCriacao: string | null;
  dataAtualizacao: string | null;
}

export interface DeccoApi {
  login(username: string, password: string): Promise<User>;

  listClasses(): Promise<ClasseObjeto[]>;
  listCamadas(): Promise<CamadaOntologica[]>;
  listMecanismos(): Promise<MecanismoInteracao[]>;

  listAnomalias(user: User): Promise<Anomalia[]>;
  getAnomalia(codigoScp: string): Promise<Anomalia>;
  createAnomalia(input: Omit<Anomalia, "dataCriacao" | "dataAtualizacao">): Promise<Anomalia>;
  deleteAnomalia(codigoScp: string): Promise<void>;

  listIncidentes(): Promise<Incidente[]>;
  getDashboard(user: User): Promise<DashboardStats>;

  listCognicoes(): Promise<CatCognicaoAparente[]>;
  getCognicao(id: number): Promise<CatCognicaoAparente>;
  createCognicao(input: Omit<CatCognicaoAparente, "id">): Promise<CatCognicaoAparente>;
  updateCognicao(input: CatCognicaoAparente): Promise<boolean>;
  deleteCognicao(id: number): Promise<boolean>;

  listPericulosidades(): Promise<CatPericulosidade[]>;
  getPericulosidade(id: number): Promise<CatPericulosidade>;
  createPericulosidade(input: Omit<CatPericulosidade, "id">): Promise<CatPericulosidade>;
  updatePericulosidade(input: CatPericulosidade): Promise<boolean>;
  deletePericulosidade(id: number): Promise<boolean>;

  listLaboratorios(): Promise<Laboratorio[]>;
  getLaboratorio(id: number): Promise<Laboratorio>;
  createLaboratorio(input: Omit<Laboratorio, "id">): Promise<Laboratorio>;
  updateLaboratorio(input: Laboratorio): Promise<boolean>;
  deleteLaboratorio(id: number): Promise<boolean>;

  listProtocolos(): Promise<ProtocoloContencao[]>;
  getProtocolo(id: number): Promise<ProtocoloContencao>;
  createProtocolo(input: Omit<ProtocoloContencao, "id">): Promise<ProtocoloContencao>;
  updateProtocolo(input: ProtocoloContencao): Promise<boolean>;
  deleteProtocolo(id: number): Promise<boolean>;

  listForcasFundamentais(): Promise<ForcaFundamental[]>;
  getForcaFundamental(id: number): Promise<ForcaFundamental>;
  createForcaFundamental(input: Omit<ForcaFundamental, "id">): Promise<ForcaFundamental>;
  updateForcaFundamental(input: ForcaFundamental): Promise<boolean>;
  deleteForcaFundamental(id: number): Promise<boolean>;

  listMecanismosInteracao(): Promise<MecanismoInteracao[]>;
  getMecanismoInteracao(id: number): Promise<MecanismoInteracao>;
  createMecanismoInteracao(input: Omit<MecanismoInteracao, "id">): Promise<MecanismoInteracao>;
  updateMecanismoInteracao(input: MecanismoInteracao): Promise<boolean>;
  deleteMecanismoInteracao(id: number): Promise<boolean>;

  listPericiasAnomalia(): Promise<PericiaAnomalia[]>;
  getPericiaAnomalia(id: number): Promise<PericiaAnomalia>;
  createPericiaAnomalia(input: Omit<PericiaAnomalia, "id">): Promise<PericiaAnomalia>;
  updatePericiaAnomalia(input: PericiaAnomalia): Promise<boolean>;
  deletePericiaAnomalia(id: number): Promise<boolean>;

  listManifestacoesEspecificas(): Promise<ManifestacaoEspecifica[]>;
  getManifestacaoEspecifica(id: number): Promise<ManifestacaoEspecifica>;
  createManifestacaoEspecifica(input: Omit<ManifestacaoEspecifica, "id">): Promise<ManifestacaoEspecifica>;
  updateManifestacaoEspecifica(input: ManifestacaoEspecifica): Promise<boolean>;
  deleteManifestacaoEspecifica(id: number): Promise<boolean>;

  listInstanciasDeviantes(): Promise<InstanciaDeviante[]>;
  getInstanciaDeviante(id: number): Promise<InstanciaDeviante>;
  createInstanciaDeviante(input: Omit<InstanciaDeviante, "id">): Promise<InstanciaDeviante>;
  updateInstanciaDeviante(input: InstanciaDeviante): Promise<boolean>;
  deleteInstanciaDeviante(id: number): Promise<boolean>;
}
