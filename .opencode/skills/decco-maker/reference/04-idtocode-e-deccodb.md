# reference/04 — IdToCode/CodeToId e o domínio do DeccoDB

> Fonte: padrão `LocalizationHelper`/`EsMappingType` do Partners + conversores + `decco.sql`.

## O DeccoDB em uma tabela

| Grupo | Tabelas | Papel no padrão |
|---|---|---|---|
| Catálogos (Id + Código/Símbolo) | `Cat_ClasseObjeto` (`Codigo`/`ClasseACS`), `Cat_ForcaFundamental` (`Simbolo`), `Cat_CamadaOntologica` (`Simbolo`), `Cat_TipoMateria`, `Cat_MecanismoInteracao` (`Codigo`), `Cat_ManifestacaoEspecifica` (`Codigo`), `Cat_CognicaoAparente` (`Codigo`), `Cat_Periculosidade` | read-only + **IdToCode/CodeToId** |
| Agregado | `Anomalia` (`Id` 1000+ ↔ `CodigoSCP`, + `CognicaoAparenteId`, `PericulosidadeId`) | entidade-raiz do CRUD |
| 1:N | `EntidadeViva`, `Artefato`, `Localidade`, `Evento` | coleções filhas (`ON DELETE CASCADE`) |
| N:N | `Pericia_Manifestacao` (PericiaAnomalia × ManifestacaoEspecifica), `Protocolo_AplicadoEm` (Protocolo × Anomalia) | **delta-merge** |
| 1:N rica | `PericiaAnomalia` | entidade com FKs a mecanismos |
| Polimórfica | `Instancia_PericiaDesviante` (`TipoInstancia`+`InstanciaId`) | referência polimórfica (avançado) |
| Histórico | `Incidente` | log de eventos |
| Configuração | `Laboratorio`, `ProtocoloContencao`, `NotificacaoAnomalia` | backoffice |

**SPs** = operações do core (Decco.API as chama). **Views** (`vw_Dashboard_Anomalias`, `vw_Relatorio_Sigma`, `vw_Estatisticas_Anomalias`) = read-models. **Triggers** (`TR_Anomalia_Validar_Mecanismos`, `TR_Anomalia_Update_Date`) = regra de negócio + auditoria no banco.

## O padrão IdToCode / CodeToId (unificado no Decco)

**Problema:** no banco, `Anomalia` referencia catálogos por **Id** (`ClasseObjetoId`, `CamadaOntologicaId`, `MecanismoPrimarioId`…);
no contrato público a gente quer **código** (`classeCodigo:"YAGUARA"`, `camadaSimbolo:"THETA"`, `mecanismoPrimarioCodigo:"THETA-A"`).
E a própria `Anomalia` tem `Id` (interno) ↔ `CodigoSCP` (público).

**Solução (mora na FACHADA):**
- Um **`CatalogHelper`** (análogo do `LocalizationHelper` do Partners) monta, a partir dos endpoints de catálogo da Decco.API,
  os dicionários diretos (`ClasseIdToCodigo`, `CamadaIdToSimbolo`, `MecanismoIdToCodigo`…) **e inversos** (`CodigoToClasseId`…).
- **Leitura (IdToCode):** conversor `List` faz `anomalia.ClasseObjetoId → "YAGUARA"` via `ClasseIdToCodigo.GetValueOrDefault(id)`.
- **Escrita (CodeToId):** conversor `Receive` faz `"YAGUARA" → Id` via `CodigoToClasseId`; **código desconhecido → Id 0**
  (o core rejeita e o erro flui de volta — mesma semântica `?? 0` do Partners).
- **Regra de ouro:** a tradução vive **no serviço** (monta os dicionários); o conversor **só consome** (nunca faz I/O).

**Porquê a tradução vive no serviço, não no conversor:** o conversor precisa ser **puro** (sem dependência de repositório/rede)
para ser testável e componível. *Alternativa rejeitada:* conversor que busca o catálogo — vira I/O escondido, difícil de testar
e sujeito a N+1. **Porquê expor código e não Id:** o Id interno é detalhe de implementação do core; se vazar, o cliente fica
acoplado à numeração do banco. `CodigoSCP`/`Simbolo` são identidade **estável de negócio**.

**Unificação (o pedido):** o Partners tem 3 sistemas de "código" (Id↔parceiro via `EsMappingType`; `LocalizationHelper` id→ISO/nome;
ponte OTA). No Decco, **sem multi-tenant**, tudo colapsa em **um só**: `Id de catálogo ↔ Código/Símbolo`. *Progressão:* depois
adicione um pseudo-tenant (`SitioContencao`) para reencontrar o caso `EsMappingType`.

## Delta-merge (N:N `Pericia_Manifestacao`)
No Update, a lista enviada é o **estado desejado**: itens novos entram; itens que existiam (`OldObject`) e não vieram são
re-emitidos com `IsDeleted=true` e o core apaga. Padrão `RelatedEntity{Key, IsDeleted}`.

## Fio a puxar 🔎
`Instancia_PericiaDesviante` usa discriminador `TipoInstancia` + `InstanciaId` (referência polimórfica). Como você exporia
isso num contrato público sem vazar Id interno? Pesquise "polymorphic association" e compare com como o EF Core modela TPH/TPT.
(Registre a decisão em `open-questions/`.)
