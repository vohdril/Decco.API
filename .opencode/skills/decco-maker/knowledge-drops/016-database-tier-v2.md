# 016 — Database Tier e versionamento da skill v2

- **Data de registo:** 2026-07-22
- **Fonte:** Lore "Sistema Brasileiro para Classificação de Anomalias.txt" + "decco-docs.md" + pedido do operador de estruturar track DB
- **Tipo:** padrão novo (expansão de tracks)
- **Afeta:** SKILL.md, reference/04, reference/17, templates/DECCO-PROGRESS.md, decco.sql, reference/18 (nova)
- **Camada:** banco (DB)

## O padrão / a mudança

**Contexto:** O lore do Decco foi expandido com:
- Sistema de classificação brasileiro (Cognição Aparente SE/SA/IN/AA, Periculosidade, identificador OA)
- Sistema de perícias/herança/desvios detalhado em decco-docs.md
- Necessidade de novas entidades: Laboratórios, Protocolos de Contenção, Notificações

**Mudança estrutural na skill:**
1. A track de "Persistência" (transversal) é promovida a **track DB** própria, com 5 tiers (DB0-DB4)
2. SKILL.md recebe versionamento semântico (v2) no frontmatter e entradas para DB
3. decco.sql é atualizado com as novas tabelas do lore
4. Nova reference/18 documenta a track DB
5. reference/17 (tracking) e templates/DECCO-PROGRESS.md ganham a track DB

## Novo schema de tracks

```
Tracks:   BACK (A/B)   ·   FRONT (FE)   ·   AUTH   ·   DB (DB0-4)
```

**DB0 — SQL: Schema do lore + novas entidades.** Cat_CognicaoAparente, Cat_Periculosidade, Laboratorio, ProtocoloContencao, NotificacaoAnomalia. FKs adicionadas à Anomalia. SPs e views atualizadas.

**DB1 — CRUDs de catálogo + telas.** Endpoints e telas para todas as tabelas de catálogo e as novas entidades (Laboratorio, Protocolo, Notificacao).

**DB2 — NoSQL (MongoDB ou Couchbase).** Perfil completo da anomalia como documento agregado desnormalizado.

**DB3 — Elasticsearch.** Indexação para busca full-text + CDC.

**DB4 — Poliglota.** Redis/Vault/Kafka aplicados ao domínio (não à infra).

## Porquê

- A separação back/front/auth/persistência como tracks paralelas já existe; adicionar DB como track própria reconhece que modelagem de dados é uma disciplina independente com sua própria progressão (SQL → NoSQL → busca → poliglota).
- O lore brasileiro adiciona camadas de classificação (Cognição, Periculosidade, OA) que expandem significativamente o schema.
- As entidades de configuração (Laboratório, Protocolo, Notificação) são o "backoffice" do sistema — merecem track própria.

## Como aplicar a partir de agora

- A skill deve reconhecer 5 tracks: A (moderno), B (legado), C (front), Auth (vertical), DB.
- Ao diagnosticar, incluir a track DB se `assets/decco.sql` tiver as seções de lore.
- A rubrica de sinais para DB0 inclui: `cognicao-tabela`, `periculosidade-tabela`, `laboratorio-crud`, `protocolo-crud`, `notificacao-crud`.
- Versionamento da skill: frontmatter `version: 2` no SKILL.md.
