import {
  ANOMALIAS, CAMADAS, CLASSES, FORCAS_FUNDAMENTAIS, INCIDENTES,
  INSTANCIAS_DEVIANTES, MANIFESTACOES, MECANISMOS, PERICIAS_ANOMALIA, USERS,
} from "./data";
import type {
  Anomalia, CamadaOntologica, ClasseObjeto, DashboardStats, ForcaFundamental,
  Incidente, InstanciaDeviante, ManifestacaoEspecifica, MecanismoInteracao,
  PericiaAnomalia, User,
} from "./types";
import type { DeccoApi } from "../data/gateway";

/* =========================================================================
   IMPLEMENTAÇÃO MOCK do gateway DeccoApi (mantida para referência/teste).
   ========================================================================= */

const clone = <T,>(v: T): T => JSON.parse(JSON.stringify(v));
const delay = (ms: number) => new Promise<void>((r) => setTimeout(r, ms));

let anomalias: Anomalia[] = clone(ANOMALIAS);
const incidentes: Incidente[] = clone(INCIDENTES);

async function withDefault<T>(value: T): Promise<T> {
  await delay(650);
  return value;
}

/** Autorização a nível de recurso: clearance >= nível da classe E sítio no escopo. */
function visibleTo(user: User, a: Anomalia): boolean {
  const classe = CLASSES.find((c) => c.nome === a.classeObjeto);
  const okClearance = user.clearance >= (classe?.nivelAcessoMinimo ?? 99);
  const okSite = user.sites.includes("*") || user.sites.includes(a.sitioContencao ?? "");
  return okClearance && okSite;
}

export const mockApi: DeccoApi = {
  async login(username: string, password: string): Promise<User> {
    await delay(700);
    const found = USERS.find((u) => u.username === username.trim().toLowerCase());
    if (!found || found.password !== password) {
      throw new Error("Credenciais inválidas. Verifique o operador e a senha.");
    }
    const { password: _pw, ...user } = found;
    return user;
  },

  async listClasses(): Promise<ClasseObjeto[]> { await delay(300); return clone(CLASSES); },
  async listCamadas(): Promise<CamadaOntologica[]> { await delay(300); return clone(CAMADAS); },
  async listMecanismos(): Promise<MecanismoInteracao[]> { await delay(300); return clone(MECANISMOS); },

  async listAnomalias(user: User): Promise<Anomalia[]> {
    const visiveis = anomalias.filter((a) => visibleTo(user, a));
    return withDefault(clone(visiveis));
  },

  async getAnomalia(codigoScp: string): Promise<Anomalia> {
    await delay(400);
    const a = anomalias.find((x) => x.codigoSCP === codigoScp);
    if (!a) throw new Error(`Anomalia ${codigoScp} não encontrada.`);
    return clone(a);
  },

  async createAnomalia(input: Omit<Anomalia, "dataCriacao" | "dataAtualizacao">): Promise<Anomalia> {
    await delay(800);
    if (anomalias.some((a) => a.codigoSCP === input.codigoSCP)) {
      throw new Error(`Já existe uma anomalia com o código ${input.codigoSCP}.`);
    }
    const now = new Date().toISOString();
    const id = Math.max(...anomalias.map((a) => a.id), 0) + 1;
    const nova: Anomalia = { ...input, id, dataCriacao: now, dataAtualizacao: now };
    anomalias = [nova, ...anomalias];
    return clone(nova);
  },

  async deleteAnomalia(codigoScp: string): Promise<void> {
    await delay(600);
    anomalias = anomalias.filter((a) => a.codigoSCP !== codigoScp);
  },

  async listIncidentes(): Promise<Incidente[]> { await delay(500); return clone(incidentes); },

  async getDashboard(user: User): Promise<DashboardStats> {
    await delay(700);
    const visiveis = anomalias.filter((a) => visibleTo(user, a));
    const porClasse = CLASSES.map((c) => ({
      classe: c.codigo,
      total: visiveis.filter((a) => a.classeObjeto === c.nome).length,
      cor: c.corAlerta,
    }));
    return {
      totalAnomalias: visiveis.length,
      ativas: visiveis.filter((a) => a.status === "ATIVA").length,
      incidentesSigma: incidentes.filter((i) => i.isEventoSigma).length,
      coreStability: 99.9,
      porClasse,
      ieiaSerie: [0.42, 0.55, 0.48, 0.61, 0.5, 0.67, 0.58, 0.72, 0.63, 0.7, 0.66, 0.74]
        .map((v, i) => ({ t: `T-${12 - i}`, v })),
    };
  },

  // — Configurações do Sistema (mock stub — retorna dados vazios) —
  async listCognicoes() { await delay(300); return []; },
  async getCognicao(_id: number) { throw new Error("Mock: use HTTP API"); },
  async createCognicao(input) { await delay(300); return { id: 1, ...input }; },
  async updateCognicao(_input) { await delay(300); return true; },
  async deleteCognicao(_id: number) { await delay(300); return true; },

  async listPericulosidades() { await delay(300); return []; },
  async getPericulosidade(_id: number) { throw new Error("Mock: use HTTP API"); },
  async createPericulosidade(input) { await delay(300); return { id: 1, ...input }; },
  async updatePericulosidade(_input) { await delay(300); return true; },
  async deletePericulosidade(_id: number) { await delay(300); return true; },

  async listLaboratorios() { await delay(300); return []; },
  async getLaboratorio(_id: number) { throw new Error("Mock: use HTTP API"); },
  async createLaboratorio(input) { await delay(300); return { id: 1, ...input }; },
  async updateLaboratorio(_input) { await delay(300); return true; },
  async deleteLaboratorio(_id: number) { await delay(300); return true; },

  async listProtocolos() { await delay(300); return []; },
  async getProtocolo(_id: number) { throw new Error("Mock: use HTTP API"); },
  async createProtocolo(input) { await delay(300); return { id: 1, ...input }; },
  async updateProtocolo(_input) { await delay(300); return true; },
  async deleteProtocolo(_id: number) { await delay(300); return true; },

  async listForcasFundamentais() { await delay(300); return clone(FORCAS_FUNDAMENTAIS); },
  async getForcaFundamental(id: number) { await delay(200); const f = FORCAS_FUNDAMENTAIS.find(x => x.id === id); if (!f) throw new Error("Força não encontrada"); return clone(f); },
  async createForcaFundamental(input) { await delay(300); const id = Math.max(...FORCAS_FUNDAMENTAIS.map(f => f.id), 0) + 1; const novo = { id, ...input }; FORCAS_FUNDAMENTAIS.push(novo); return clone(novo); },
  async updateForcaFundamental(input) { await delay(300); const idx = FORCAS_FUNDAMENTAIS.findIndex(f => f.id === input.id); if (idx < 0) return false; FORCAS_FUNDAMENTAIS[idx] = input; return true; },
  async deleteForcaFundamental(id: number) { await delay(300); const idx = FORCAS_FUNDAMENTAIS.findIndex(f => f.id === id); if (idx < 0) return false; FORCAS_FUNDAMENTAIS.splice(idx, 1); return true; },

  async listMecanismosInteracao() { await delay(300); return clone(MECANISMOS); },
  async getMecanismoInteracao(id: number) { await delay(200); const m = MECANISMOS.find(x => x.id === id); if (!m) throw new Error("Mecanismo não encontrado"); return clone(m); },
  async createMecanismoInteracao(input) { await delay(300); const id = Math.max(...MECANISMOS.map(m => m.id), 0) + 1; const novo = { id, ...input }; MECANISMOS.push(novo); return clone(novo); },
  async updateMecanismoInteracao(input) { await delay(300); const idx = MECANISMOS.findIndex(m => m.id === input.id); if (idx < 0) return false; MECANISMOS[idx] = input; return true; },
  async deleteMecanismoInteracao(id: number) { await delay(300); const idx = MECANISMOS.findIndex(m => m.id === id); if (idx < 0) return false; MECANISMOS.splice(idx, 1); return true; },

  async listPericiasAnomalia() { await delay(300); return clone(PERICIAS_ANOMALIA); },
  async getPericiaAnomalia(id: number) { await delay(200); const m = PERICIAS_ANOMALIA.find(x => x.id === id); if (!m) throw new Error("Perícia não encontrada"); return clone(m); },
  async createPericiaAnomalia(input) { await delay(300); const id = Math.max(...PERICIAS_ANOMALIA.map(p => p.id), 0) + 1; const novo = { id, ...input }; PERICIAS_ANOMALIA.push(novo); return clone(novo); },
  async updatePericiaAnomalia(input) { await delay(300); const idx = PERICIAS_ANOMALIA.findIndex(p => p.id === input.id); if (idx < 0) return false; PERICIAS_ANOMALIA[idx] = input; return true; },
  async deletePericiaAnomalia(id: number) { await delay(300); const idx = PERICIAS_ANOMALIA.findIndex(p => p.id === id); if (idx < 0) return false; PERICIAS_ANOMALIA.splice(idx, 1); return true; },

  async listManifestacoesEspecificas() { await delay(300); return clone(MANIFESTACOES); },
  async getManifestacaoEspecifica(id: number) { await delay(200); const m = MANIFESTACOES.find(x => x.id === id); if (!m) throw new Error("Manifestação não encontrada"); return clone(m); },
  async createManifestacaoEspecifica(input) { await delay(300); const id = Math.max(...MANIFESTACOES.map(m => m.id), 0) + 1; const novo = { id, ...input }; MANIFESTACOES.push(novo); return clone(novo); },
  async updateManifestacaoEspecifica(input) { await delay(300); const idx = MANIFESTACOES.findIndex(m => m.id === input.id); if (idx < 0) return false; MANIFESTACOES[idx] = input; return true; },
  async deleteManifestacaoEspecifica(id: number) { await delay(300); const idx = MANIFESTACOES.findIndex(m => m.id === id); if (idx < 0) return false; MANIFESTACOES.splice(idx, 1); return true; },

  async listInstanciasDeviantes() { await delay(300); return clone(INSTANCIAS_DEVIANTES); },
  async getInstanciaDeviante(id: number) { await delay(200); const m = INSTANCIAS_DEVIANTES.find(x => x.id === id); if (!m) throw new Error("Instância não encontrada"); return clone(m); },
  async createInstanciaDeviante(input) { await delay(300); const id = Math.max(...INSTANCIAS_DEVIANTES.map(i => i.id), 0) + 1; const novo = { id, ...input }; INSTANCIAS_DEVIANTES.push(novo); return clone(novo); },
  async updateInstanciaDeviante(input) { await delay(300); const idx = INSTANCIAS_DEVIANTES.findIndex(i => i.id === input.id); if (idx < 0) return false; INSTANCIAS_DEVIANTES[idx] = input; return true; },
  async deleteInstanciaDeviante(id: number) { await delay(300); const idx = INSTANCIAS_DEVIANTES.findIndex(i => i.id === id); if (idx < 0) return false; INSTANCIAS_DEVIANTES.splice(idx, 1); return true; },
};
