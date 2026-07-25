import type {
  Anomalia, CamadaOntologica, ClasseObjeto, DashboardStats, ForcaFundamental,
  Incidente, InstanciaDeviante, ManifestacaoEspecifica, MecanismoInteracao,
  PericiaAnomalia, User,
} from "../mocks/types";
import { API_PATHS } from "./apiPaths";
import type {
  CatCognicaoAparente, CatPericulosidade, DeccoApi, Laboratorio, ProtocoloContencao,
} from "./gateway";

/* =========================================================================
   IMPLEMENTAÇÃO HTTP (modo "live") — fala com a Decco.API via envelope
   RequestBase/ResponseBase definido em Decco.Contracts.

   Toda chamada:
     1. Monta um RequestBase<TRequest> com { data, metadata, culture }
     2. POST para a rota mapeada em API_PATHS
     3. Desembrulha o envelope ResponseBase, extrai o .data

   Rotas que ainda não existem na Decco.API (auth, catálogos, dashboard)
   estão marcadas com `// >>>` — lançam erro até serem implementadas.
   ========================================================================= */

/* Envelope ResponseBase devolvido pela Decco.API */
interface Envelope<T> {
  data: T;
  status: "Success" | "PartialSuccess" | "Fail";
  error?: { code: string; message: string };
  pageIndex?: number;
  pageSize?: number;
  totalRecords?: number;
  hasNextPage?: boolean;
}

/* RequestBase enviado para a Decco.API */
function makeRequest<T>(data: T) {
  return { data, metadata: null, culture: "pt-BR" };
}

const BASE = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5000";

async function httpPost<TReq, TRes>(path: string, body: TReq): Promise<TRes> {
  const res = await fetch(`${BASE}${path}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(makeRequest(body)),
  });
  if (!res.ok) throw new Error(`HTTP ${res.status} em ${path}`);

  const text = await res.text();
  if (!text) return undefined as TRes;

  const envelope = JSON.parse(text) as Envelope<TRes>;
  if (envelope.status === "Fail") {
    throw new Error(envelope.error?.message ?? `Erro desconhecido em ${path}`);
  }
  return envelope.data;
}

export const httpApi: DeccoApi = {
  async login(username, password) {
    // >>> Auth não implementada na Decco.API (Tier 1+)
    throw new Error("AUTH_NOT_IMPLEMENTED: login via Decco.API não disponível. Use modo mock.");
  },

  // >>> Catálogos não implementados na Decco.API (Tier 1+)
  listClasses() { throw new Error("CATALOG_NOT_IMPLEMENTED: listClasses via Decco.API não disponível. Use modo mock."); },
  listCamadas() { throw new Error("CATALOG_NOT_IMPLEMENTED: listCamadas via Decco.API não disponível. Use modo mock."); },
  listMecanismos() { throw new Error("CATALOG_NOT_IMPLEMENTED: listMecanismos via Decco.API não disponível. Use modo mock."); },

  async listAnomalias(_user: User) {
    return httpPost<Record<string, never>, Anomalia[]>(API_PATHS.ANOMALIA_LIST, {});
  },

  async getAnomalia(codigoScp: string) {
    // >>> o GetAnomalia espera um int (Id), não o código SCP. É necessário
    //     um endpoint de resolução IdToCode ou buscar por código.
    //     Por enquanto, mapeia via Id=-1 para forçar erro.
    return httpPost<number, Anomalia>(API_PATHS.ANOMALIA_GET, -1);
  },

  async createAnomalia(input) {
    // >>> o InsertAnomalia recebe AnomaliaDto (com Id=0) e devolve o novo Id.
    //     O retorno Anomalia completo requer uma busca posterior.
    const id = await httpPost<typeof input, number>(API_PATHS.ANOMALIA_INSERT, input);
    return { ...input, dataCriacao: new Date().toISOString(), dataAtualizacao: new Date().toISOString() };
  },

  async deleteAnomalia(codigoScp: string) {
    // >>> mesmo problema do getAnomalia: precisa resolver código→Id
    await httpPost<number, boolean>(API_PATHS.ANOMALIA_DELETE, -1);
  },

  // >>> Incidentes e Dashboard não existem na Decco.API atual (Tier 1+)
  listIncidentes() { throw new Error("INCIDENT_NOT_IMPLEMENTED: listIncidentes via Decco.API não disponível. Use modo mock."); },
  getDashboard(_user: User) { throw new Error("DASHBOARD_NOT_IMPLEMENTED: getDashboard via Decco.API não disponível. Use modo mock."); },

  async listCognicoes(): Promise<CatCognicaoAparente[]> {
    return httpPost<object, CatCognicaoAparente[]>(API_PATHS.COGNICAO_LIST, {});
  },
  async getCognicao(id: number): Promise<CatCognicaoAparente> {
    return httpPost<number, CatCognicaoAparente>(API_PATHS.COGNICAO_GET, id);
  },
  async createCognicao(input: Omit<CatCognicaoAparente, "id">): Promise<CatCognicaoAparente> {
    return httpPost(API_PATHS.COGNICAO_INSERT, input);
  },
  async updateCognicao(input: CatCognicaoAparente): Promise<boolean> {
    return httpPost(API_PATHS.COGNICAO_UPDATE, input);
  },
  async deleteCognicao(id: number): Promise<boolean> {
    return httpPost<number, boolean>(API_PATHS.COGNICAO_DELETE, id);
  },

  async listPericulosidades(): Promise<CatPericulosidade[]> {
    return httpPost<object, CatPericulosidade[]>(API_PATHS.PERICULOSIDADE_LIST, {});
  },
  async getPericulosidade(id: number): Promise<CatPericulosidade> {
    return httpPost<number, CatPericulosidade>(API_PATHS.PERICULOSIDADE_GET, id);
  },
  async createPericulosidade(input: Omit<CatPericulosidade, "id">): Promise<CatPericulosidade> {
    return httpPost(API_PATHS.PERICULOSIDADE_INSERT, input);
  },
  async updatePericulosidade(input: CatPericulosidade): Promise<boolean> {
    return httpPost(API_PATHS.PERICULOSIDADE_UPDATE, input);
  },
  async deletePericulosidade(id: number): Promise<boolean> {
    return httpPost<number, boolean>(API_PATHS.PERICULOSIDADE_DELETE, id);
  },

  async listLaboratorios(): Promise<Laboratorio[]> {
    return httpPost<object, Laboratorio[]>(API_PATHS.LABORATORIO_LIST, {});
  },
  async getLaboratorio(id: number): Promise<Laboratorio> {
    return httpPost<number, Laboratorio>(API_PATHS.LABORATORIO_GET, id);
  },
  async createLaboratorio(input: Omit<Laboratorio, "id">): Promise<Laboratorio> {
    return httpPost(API_PATHS.LABORATORIO_INSERT, input);
  },
  async updateLaboratorio(input: Laboratorio): Promise<boolean> {
    return httpPost(API_PATHS.LABORATORIO_UPDATE, input);
  },
  async deleteLaboratorio(id: number): Promise<boolean> {
    return httpPost<number, boolean>(API_PATHS.LABORATORIO_DELETE, id);
  },

  async listProtocolos(): Promise<ProtocoloContencao[]> {
    return httpPost<object, ProtocoloContencao[]>(API_PATHS.PROTOCOLO_LIST, {});
  },
  async getProtocolo(id: number): Promise<ProtocoloContencao> {
    return httpPost<number, ProtocoloContencao>(API_PATHS.PROTOCOLO_GET, id);
  },
  async createProtocolo(input: Omit<ProtocoloContencao, "id">): Promise<ProtocoloContencao> {
    return httpPost(API_PATHS.PROTOCOLO_INSERT, input);
  },
  async updateProtocolo(input: ProtocoloContencao): Promise<boolean> {
    return httpPost(API_PATHS.PROTOCOLO_UPDATE, input);
  },
  async deleteProtocolo(id: number): Promise<boolean> {
    return httpPost<number, boolean>(API_PATHS.PROTOCOLO_DELETE, id);
  },

  listForcasFundamentais(): Promise<ForcaFundamental[]> {
    return httpPost<object, ForcaFundamental[]>(API_PATHS.FORCA_LIST, {});
  },
  getForcaFundamental(id: number): Promise<ForcaFundamental> {
    return httpPost<number, ForcaFundamental>(API_PATHS.FORCA_GET, id);
  },
  createForcaFundamental(input: Omit<ForcaFundamental, "id">): Promise<ForcaFundamental> {
    return httpPost(API_PATHS.FORCA_INSERT, input);
  },
  updateForcaFundamental(input: ForcaFundamental): Promise<boolean> {
    return httpPost(API_PATHS.FORCA_UPDATE, input);
  },
  deleteForcaFundamental(id: number): Promise<boolean> {
    return httpPost<number, boolean>(API_PATHS.FORCA_DELETE, id);
  },

  listMecanismosInteracao(): Promise<MecanismoInteracao[]> {
    return httpPost<object, MecanismoInteracao[]>(API_PATHS.MECANISMO_LIST, {});
  },
  getMecanismoInteracao(id: number): Promise<MecanismoInteracao> {
    return httpPost<number, MecanismoInteracao>(API_PATHS.MECANISMO_GET, id);
  },
  createMecanismoInteracao(input: Omit<MecanismoInteracao, "id">): Promise<MecanismoInteracao> {
    return httpPost(API_PATHS.MECANISMO_INSERT, input);
  },
  updateMecanismoInteracao(input: MecanismoInteracao): Promise<boolean> {
    return httpPost(API_PATHS.MECANISMO_UPDATE, input);
  },
  deleteMecanismoInteracao(id: number): Promise<boolean> {
    return httpPost<number, boolean>(API_PATHS.MECANISMO_DELETE, id);
  },

  listPericiasAnomalia() { throw new Error("NOT_IMPLEMENTED"); },
  getPericiaAnomalia(_id: number): Promise<PericiaAnomalia> { throw new Error("NOT_IMPLEMENTED"); },
  createPericiaAnomalia(_input: Omit<PericiaAnomalia, "id">): Promise<PericiaAnomalia> { throw new Error("NOT_IMPLEMENTED"); },
  updatePericiaAnomalia(_input: PericiaAnomalia): Promise<boolean> { throw new Error("NOT_IMPLEMENTED"); },
  deletePericiaAnomalia(_id: number): Promise<boolean> { throw new Error("NOT_IMPLEMENTED"); },

  listManifestacoesEspecificas() { throw new Error("NOT_IMPLEMENTED"); },
  getManifestacaoEspecifica(_id: number): Promise<ManifestacaoEspecifica> { throw new Error("NOT_IMPLEMENTED"); },
  createManifestacaoEspecifica(_input: Omit<ManifestacaoEspecifica, "id">): Promise<ManifestacaoEspecifica> { throw new Error("NOT_IMPLEMENTED"); },
  updateManifestacaoEspecifica(_input: ManifestacaoEspecifica): Promise<boolean> { throw new Error("NOT_IMPLEMENTED"); },
  deleteManifestacaoEspecifica(_id: number): Promise<boolean> { throw new Error("NOT_IMPLEMENTED"); },

  listInstanciasDeviantes() { throw new Error("NOT_IMPLEMENTED"); },
  getInstanciaDeviante(_id: number): Promise<InstanciaDeviante> { throw new Error("NOT_IMPLEMENTED"); },
  createInstanciaDeviante(_input: Omit<InstanciaDeviante, "id">): Promise<InstanciaDeviante> { throw new Error("NOT_IMPLEMENTED"); },
  updateInstanciaDeviante(_input: InstanciaDeviante): Promise<boolean> { throw new Error("NOT_IMPLEMENTED"); },
  deleteInstanciaDeviante(_id: number): Promise<boolean> { throw new Error("NOT_IMPLEMENTED"); },
};
