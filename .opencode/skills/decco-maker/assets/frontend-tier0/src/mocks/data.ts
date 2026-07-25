import type {
  Anomalia, CamadaOntologica, ClasseObjeto, ForcaFundamental, Incidente,
  InstanciaDeviante, ManifestacaoEspecifica, MecanismoInteracao,
  PericiaAnomalia, User,
} from "./types";

/* Dados fictícios (SCP/MIB-style) — TIER 0 apenas. Sem dados reais/segredos. */

export const CLASSES: ClasseObjeto[] = [
  { codigo: "PACATO",  nome: "Pacato",  classeAcs: "SAFE",     corAlerta: "#4caf50", nivelAcessoMinimo: 1 },
  { codigo: "YAGUARA", nome: "Yaguara", classeAcs: "EUCLID",   corAlerta: "#ffc107", nivelAcessoMinimo: 2 },
  { codigo: "ABAPORU", nome: "Abaporu", classeAcs: "KETER",    corAlerta: "#f44336", nivelAcessoMinimo: 3 },
  { codigo: "UKAR",    nome: "Ukar",    classeAcs: "THAUMIEL", corAlerta: "#9c27b0", nivelAcessoMinimo: 4 },
];

export const CAMADAS: CamadaOntologica[] = [
  { simbolo: "THETA", nome: "Theta", descricao: "Física anômala — 95% dos casos." },
  { simbolo: "PSI",   nome: "Psi",   descricao: "Narrativo/informacional." },
  { simbolo: "PHI",   nome: "Phi",   descricao: "Consciência/digital." },
  { simbolo: "OMEGA", nome: "Omega", descricao: "Substrato não-bariônico." },
];

export const FORCAS_FUNDAMENTAIS: ForcaFundamental[] = [
  { id: 1, simbolo: "THETA", nome: "Theta",     descricao: "Força fundamental associada à matéria escura e à coerência de spin anômalo. Manifesta-se como distorções na estrutura espaço-temporal local.", particulaPortadora: "Theta-Bóson Hipotético" },
  { id: 2, simbolo: "PSI",   nome: "Psi",       descricao: "Força fundamental associada à mente coletiva e a fenômenos psíquicos. Responde a padrões de campo mnemônico ressonante.", particulaPortadora: "Psi-Fóton" },
  { id: 3, simbolo: "PHI",   nome: "Phi",       descricao: "Força fundamental ligada à matéria bariônica densa e a campos de distorção física. Interfere na inércia local.", particulaPortadora: "Phi-Gráviton" },
  { id: 4, simbolo: "OMEGA", nome: "Omega",     descricao: "Força fundamental relacionada à entropia negativa e à reversão parcial de processos termodinâmicos.", particulaPortadora: "Omega-Táquion" },
];

export const MECANISMOS: MecanismoInteracao[] = [
  { id: 1, codigo: "THETA-A", nome: "Theta-Ativo",      descricao: "Mecanismo de interação ativa na camada Theta.", camadaOntologicaId: 1, ehSubnatureza: false },
  { id: 2, codigo: "THETA-C", nome: "Theta-Condicional", descricao: "Interação condicional disparada por ressonância Theta.", camadaOntologicaId: 1, ehSubnatureza: true },
  { id: 3, codigo: "PSI-C",   nome: "Psi-Condicional",  descricao: "Mecanismo Psi ativado por padrões mnemônicos.", camadaOntologicaId: 2, ehSubnatureza: true },
  { id: 4, codigo: "PHI-B",   nome: "Phi-Passivo",      descricao: "Mecanismo de distorção física passiva na camada Phi.", camadaOntologicaId: 3, ehSubnatureza: false },
  { id: 5, codigo: "OMEGA-A", nome: "Omega-Ativo",      descricao: "Mecanismo de reversão entrópica ativa.", camadaOntologicaId: 4, ehSubnatureza: false },
];

export const PERICIAS_ANOMALIA: PericiaAnomalia[] = [
  { id: 1, codigo: "PER-001", nome: "Rastreamento Theta", descricao: "Capacidade de rastrear emissoes theta em um raio de 500m.", nivel: 2 },
  { id: 2, codigo: "PER-002", nome: "Leitura de Spin", descricao: "Leitura e interpretacao do fator de coerencia de spin anomalo.", nivel: 3 },
  { id: 3, codigo: "PER-003", nome: "Contencao Fisica", descricao: "Tecnicas de contencao fisica para anomalias biometricas.", nivel: 1 },
  { id: 4, codigo: "PER-004", nome: "Analise de Narrativa", descricao: "Analise de padroes narrativos em anomalias Psi.", nivel: 4 },
  { id: 5, codigo: "PER-005", nome: "Neutralizacao Omega", descricao: "Procedimentos de neutralizacao de entidades Omega.", nivel: 5 },
];

export const MANIFESTACOES: ManifestacaoEspecifica[] = [
  { id: 1, codigo: "MAN-001", nome: "Materializacao Espontanea", descricao: "A anomalia se materializa sem gatilho conhecido.", tipo: "Fisica" },
  { id: 2, codigo: "MAN-002", nome: "Distorcao Temporal", descricao: "A anomalia causa distorcoes no fluxo temporal local.", tipo: "Temporal" },
  { id: 3, codigo: "MAN-003", nome: "Manipulacao MneMonica", descricao: "A anomalia altera ou remove memorias recentes.", tipo: "Psiquica" },
  { id: 4, codigo: "MAN-004", nome: "Corrupcao Digital", descricao: "A anomalia se propaga por sistemas digitais.", tipo: "Digital" },
  { id: 5, codigo: "MAN-005", nome: "Dobragem Espacial", descricao: "A anomalia cria atalhos ou barreiras no espaco fisico.", tipo: "Fisica" },
];

export const INSTANCIAS_DEVIANTES: InstanciaDeviante[] = [
  { id: 1, codigo: "INST-001", nome: "Proteu-Alfa", descricao: "Instancia primaria do Proteu, forma humanoide estavel.", anomaliaCodigoSCP: "SCP-1001", status: "CONTIDA" },
  { id: 2, codigo: "INST-002", nome: "Proteu-Beta", descricao: "Instancia secundaria, forma felina instavel.", anomaliaCodigoSCP: "SCP-1001", status: "OBSERVACAO" },
  { id: 3, codigo: "INST-003", nome: "Codex-Folha-7", descricao: "Pagina destacada do Codex com leitura sigma ativa.", anomaliaCodigoSCP: "SCP-1002", status: "CONTIDA" },
  { id: 4, codigo: "INST-004", nome: "Nevoa-Sentinela", descricao: "Foco autonomo de nevoa que patrulha o perimetro.", anomaliaCodigoSCP: "SCP-1013", status: "ATIVA" },
  { id: 5, codigo: "INST-005", nome: "Pedra-Mestre", descricao: "Fragmento central da Pedra de Contencao com emissao amplificada.", anomaliaCodigoSCP: "SCP-1020", status: "CONTIDA" },
];

export const ANOMALIAS: Anomalia[] = [
  {
    id: 1, codigoSCP: "SCP-1001", nomeComum: "Proteu — O Metamorfo Complexo",
    descricao: "Entidade humanoide capaz de se transformar em múltiplas formas animais.",
    classeObjeto: "Yaguara", camadaOntologica: "Theta", tipoMateria: "Bariônica Anômala",
    mecanismoPrimario: "Theta-Condicional", mecanismoSecundario: null,
    status: "ATIVA", sitioContencao: "Sitio-19",
    ieiaDBase: 0.5, fatorCoerenciaSpin: "Alto", responsavelPesquisa: "Dra. Elara Vance",
    dataCriacao: "2026-02-11T14:03:00Z", dataAtualizacao: "2026-06-30T09:12:00Z",
  },
  {
    id: 2, codigoSCP: "SCP-1002", nomeComum: "Codex de Realidades — Grimório SIGMA",
    descricao: "Tomo antigo com padrões de spin coerente 'congelados' no pergaminho.",
    classeObjeto: "Abaporu", camadaOntologica: "Omega", tipoMateria: "Mista",
    mecanismoPrimario: "Omega-Ativo", mecanismoSecundario: null,
    status: "ATIVA", sitioContencao: "Sitio-64",
    ieiaDBase: 0.05, fatorCoerenciaSpin: "Crítico", responsavelPesquisa: "Dr. Aris Thoth",
    dataCriacao: "2026-03-02T10:00:00Z", dataAtualizacao: "2026-07-01T18:41:00Z",
  },
  {
    id: 3, codigoSCP: "SCP-1013", nomeComum: "Farol de Névoa",
    descricao: "Estrutura costeira que emite uma névoa que altera memórias recentes.",
    classeObjeto: "Yaguara", camadaOntologica: "Psi", tipoMateria: "Bariônica Anômala",
    mecanismoPrimario: "Psi-Condicional", mecanismoSecundario: null,
    status: "EM_PESQUISA", sitioContencao: "Sitio-19",
    ieiaDBase: 0.22, fatorCoerenciaSpin: "Médio", responsavelPesquisa: "Dr. N. Okafor",
    dataCriacao: "2026-04-18T08:30:00Z", dataAtualizacao: "2026-07-05T11:05:00Z",
  },
  {
    id: 4, codigoSCP: "SCP-1020", nomeComum: "Pedra de Contenção Padrão",
    descricao: "Mineral inerte usado para amortecer emissões theta. Baixo risco.",
    classeObjeto: "Pacato", camadaOntologica: "Theta", tipoMateria: "Bariônica Anômala",
    mecanismoPrimario: "Theta-Ativo", mecanismoSecundario: null,
    status: "ATIVA", sitioContencao: "Sitio-19",
    ieiaDBase: 0.01, fatorCoerenciaSpin: "Baixo", responsavelPesquisa: "Equipe de Logística",
    dataCriacao: "2026-01-09T07:00:00Z", dataAtualizacao: "2026-05-22T16:00:00Z",
  },
  {
    id: 5, codigoSCP: "SCP-1099", nomeComum: "O Segredo dos Segredos",
    descricao: "Anomalia usada para conter outras anomalias. Acesso O5 apenas.",
    classeObjeto: "Ukar", camadaOntologica: "Omega", tipoMateria: "Não-Bariônica",
    mecanismoPrimario: "Omega-Ativo", mecanismoSecundario: null,
    status: "ATIVA", sitioContencao: "Sitio-01",
    ieiaDBase: null, fatorCoerenciaSpin: "Crítico", responsavelPesquisa: "Conselho O5",
    dataCriacao: "2025-11-30T00:00:00Z", dataAtualizacao: "2026-07-10T22:00:00Z",
  },
];

export const INCIDENTES: Incidente[] = [
  { id: 1, anomaliaCodigoScp: "SCP-1002", dataHora: "2026-07-01T18:40:00Z", tipo: "Evento SIGMA", titulo: "Manifestação Não-Autorizada", nivelSeguranca: "Nível 4", isEventoSigma: true, mortes: 1, feridos: 0 },
  { id: 2, anomaliaCodigoScp: "SCP-1001", dataHora: "2026-06-30T09:10:00Z", tipo: "Teste de Pesquisa", titulo: "Teste de Limites de Transformação", nivelSeguranca: "Alto", isEventoSigma: false, mortes: 0, feridos: 0 },
  { id: 3, anomaliaCodigoScp: "SCP-1013", dataHora: "2026-07-05T11:00:00Z", tipo: "Falha de Contenção", titulo: "Vazamento de Névoa Setor B", nivelSeguranca: "Médio", isEventoSigma: false, mortes: 0, feridos: 3 },
];

/* Usuários mock — tipos/clearance diferentes (auth a nível de recurso). Senha: "decco". */
export const USERS: (User & { password: string })[] = [
  { id: 1, username: "vance",  password: "decco", nome: "Dra. Elara Vance", role: "PESQUISADOR",      clearance: 2, sites: ["Sitio-19"] },
  { id: 2, username: "okafor", password: "decco", nome: "Dr. N. Okafor",    role: "AGENTE_CONTENCAO", clearance: 2, sites: ["Sitio-19"] },
  { id: 3, username: "thoth",  password: "decco", nome: "Dr. Aris Thoth",   role: "DIRETOR_SITIO",    clearance: 3, sites: ["Sitio-64", "Sitio-19"] },
  { id: 4, username: "o5",     password: "decco", nome: "Conselho O5",      role: "O5",               clearance: 4, sites: ["*"] },
];
