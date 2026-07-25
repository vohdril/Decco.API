/* =========================================================================
   MAPEAMENTO DAS ROTAS DA Decco.API — espelha os endpoints documentados
   no Swagger (swagger/index.html). Cada constante segue o padrão
   {Controller}/{Action} conforme [Route("api/[controller]")] + [HttpPost("{action}")].

   Sempre que um endpoint não existir na Decco.API, fica marcado com
   `// >>>` apontando para o tier em que será implementado.
   ========================================================================= */

export const API_PATHS = {
  /** POST api/Anomalia/ListAnomalias — RequestBase<object> → PagedResponse<AnomaliaDto> */
  ANOMALIA_LIST: "/api/Anomalia/ListAnomalias",

  /** POST api/Anomalia/GetAnomalia — RequestBase<int> → SingleResponse<AnomaliaDto> */
  ANOMALIA_GET: "/api/Anomalia/GetAnomalia",

  /** POST api/Anomalia/InsertAnomalia — RequestBase<AnomaliaDto> → SingleResponse<int> */
  ANOMALIA_INSERT: "/api/Anomalia/InsertAnomalia",

  /** POST api/Anomalia/UpdateAnomalia — RequestBase<AnomaliaDto> → SingleResponse<bool> */
  ANOMALIA_UPDATE: "/api/Anomalia/UpdateAnomalia",

  /** POST api/Anomalia/DeleteAnomalia — RequestBase<int> → SingleResponse<bool> */
  ANOMALIA_DELETE: "/api/Anomalia/DeleteAnomalia",

  COGNICAO_LIST: "/api/CatCognicaoAparente/List",
  COGNICAO_GET: "/api/CatCognicaoAparente/Get",
  COGNICAO_INSERT: "/api/CatCognicaoAparente/Insert",
  COGNICAO_UPDATE: "/api/CatCognicaoAparente/Update",
  COGNICAO_DELETE: "/api/CatCognicaoAparente/Delete",

  PERICULOSIDADE_LIST: "/api/CatPericulosidade/List",
  PERICULOSIDADE_GET: "/api/CatPericulosidade/Get",
  PERICULOSIDADE_INSERT: "/api/CatPericulosidade/Insert",
  PERICULOSIDADE_UPDATE: "/api/CatPericulosidade/Update",
  PERICULOSIDADE_DELETE: "/api/CatPericulosidade/Delete",

  LABORATORIO_LIST: "/api/Laboratorio/List",
  LABORATORIO_GET: "/api/Laboratorio/Get",
  LABORATORIO_INSERT: "/api/Laboratorio/Insert",
  LABORATORIO_UPDATE: "/api/Laboratorio/Update",
  LABORATORIO_DELETE: "/api/Laboratorio/Delete",

  PROTOCOLO_LIST: "/api/ProtocoloContencao/List",
  PROTOCOLO_GET: "/api/ProtocoloContencao/Get",
  PROTOCOLO_INSERT: "/api/ProtocoloContencao/Insert",
  PROTOCOLO_UPDATE: "/api/ProtocoloContencao/Update",
  PROTOCOLO_DELETE: "/api/ProtocoloContencao/Delete",

  FORCA_LIST: "/api/CatForcaFundamental/List",
  FORCA_GET: "/api/CatForcaFundamental/Get",
  FORCA_INSERT: "/api/CatForcaFundamental/Insert",
  FORCA_UPDATE: "/api/CatForcaFundamental/Update",
  FORCA_DELETE: "/api/CatForcaFundamental/Delete",

  MECANISMO_LIST: "/api/CatMecanismoInteracao/List",
  MECANISMO_GET: "/api/CatMecanismoInteracao/Get",
  MECANISMO_INSERT: "/api/CatMecanismoInteracao/Insert",
  MECANISMO_UPDATE: "/api/CatMecanismoInteracao/Update",
  MECANISMO_DELETE: "/api/CatMecanismoInteracao/Delete",

  PERICIA_LIST: "/api/PericiaAnomalia/List",
  PERICIA_GET: "/api/PericiaAnomalia/Get",
  PERICIA_INSERT: "/api/PericiaAnomalia/Insert",
  PERICIA_UPDATE: "/api/PericiaAnomalia/Update",
  PERICIA_DELETE: "/api/PericiaAnomalia/Delete",

  MANIFESTACAO_LIST: "/api/ManifestacaoEspecifica/List",
  MANIFESTACAO_GET: "/api/ManifestacaoEspecifica/Get",
  MANIFESTACAO_INSERT: "/api/ManifestacaoEspecifica/Insert",
  MANIFESTACAO_UPDATE: "/api/ManifestacaoEspecifica/Update",
  MANIFESTACAO_DELETE: "/api/ManifestacaoEspecifica/Delete",

  INSTANCIA_LIST: "/api/InstanciaDeviante/List",
  INSTANCIA_GET: "/api/InstanciaDeviante/Get",
  INSTANCIA_INSERT: "/api/InstanciaDeviante/Insert",
  INSTANCIA_UPDATE: "/api/InstanciaDeviante/Update",
  INSTANCIA_DELETE: "/api/InstanciaDeviante/Delete",
} as const;
