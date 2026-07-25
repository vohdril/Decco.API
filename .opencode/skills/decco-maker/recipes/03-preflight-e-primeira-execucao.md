# recipes/03 — Preflight (pré-requisitos) + Primeira execução guiada

> Esta recipe roda **no arranque** (primeira vez que a skill é usada num ambiente, ou quando o operador diz "vamos começar").
> Garante que a skill, em **qualquer sistema ou por outro agente**, (1) **verifica os pré-requisitos**, (2) **cria o banco se não
> existir** e (3) **conduz o operador pela sequência didática**, um checkpoint de cada vez. **Nunca** gerar tudo de uma vez.
> Idioma: PT-BR. Segredos só por env var/user-secrets. **Sem cartão.**

## 1. Preflight — verificar tecnologias disponíveis (antes de implementar)
Rodar os checks do **tier atual** e **reportar** um checklist "✔ presente / ✗ ausente + como instalar". **Não** avançar num tier cujos pré-requisitos faltam sem avisar o operador.

### Tier 0 (obrigatório para começar)
| Requisito | Como verificar | Se faltar |
|---|---|---|
| **.NET 8 SDK** | `dotnet --list-sdks` (procurar `8.x`) | instalar o .NET 8 SDK |
| **Motor SQL Server** (escolher **1**) | **LocalDB** (Win): `sqllocaldb info` → instância `MSSQLLocalDB` · **ou** instância existente · **ou** **Docker**: `docker --version` + `docker info` (daemon up) | ver `reference/02`; default recomendado = **LocalDB** (o operador domina) |
| **Executor de `.sql`** | `sqlcmd -?` · **ou** PowerShell `Get-Command Invoke-Sqlcmd` (módulo `SqlServer`) · **ou** um mini-runner .NET (`Microsoft.Data.SqlClient`) | usar o que existir; a skill pode rodar o `assets/decco.sql` por qualquer um deles |
| **dotnet-ef** (scaffold database-first) | `dotnet ef --version` | `dotnet tool install --global dotnet-ef` |

### Tiers seguintes (verificar só quando chegar lá — não bloquear o Tier 0)
- **Tier 1 — Redis:** `docker --version` (imagem `redis`) — para cache/FusionCache.
- **Tier 2 — Kafka / Elasticsearch / Couchbase:** Docker (`confluentinc/cp-kafka`, `docker.elastic.co/...`, `couchbase`).
- **Tier 3 — Vault:** Docker (`hashicorp/vault`) — segredos.
> Estas são as tecnologias do mapa poliglota (`reference/15`); cada uma entra no seu tier, nunca antes.

## 2. Primeira execução — fluxo guiado (acompanhar o operador)
Sequência **conversacional**, um passo por vez, confirmando antes de avançar:

1. **Preflight** (§1 do Tier 0) → reportar o checklist.
2. **Escolher o alvo de banco** — perguntar ao operador; **default = Local SQL Server / LocalDB** (o que ele sabe configurar). Registrar a **connection string** num único ponto (user-secrets/env var/`appsettings.Development.json`).
3. **Etapa 0 — criar o DeccoDB SE não existir** (ver §3): checar existência; ausente → rodar `assets/decco.sql` (cria schema + SPs + views + triggers + 2 anomalias de exemplo); presente → **não** re-rodar (o script não é idempotente); recriar só com **confirmação explícita** do operador (é destrutivo).
   - ✅ **Verificação:** `SELECT COUNT(*) FROM Anomalia` retorna ≥ 2.
4. **Esqueleto + entidades** — criar a solução `Decco.API` (mínima) e obter as entidades por **`dotnet ef dbcontext scaffold`** (OOP preservada) ou mapear à mão `Anomalia`+`Cat_*`.
   - ✅ **Verificação:** app conecta e faz um `SELECT` numa tabela via EF.
5. **Seguir `recipes/01`** — Etapas 1 → 2a (EF) → 2b (Dapper/SP) → … **pausando em cada checkpoint** e perguntando se avança. É a trilha didática (`reference/11`).

> A skill deve **narrar o progresso** e **esperar o operador** entre etapas — o objetivo é acompanhar o desenvolvimento de quem opera, não despejar o projeto pronto.

## 3. Criar o banco SE não existir (a skill CRIA o DeccoDB)
Sim — a skill **provisiona o banco** a partir do `assets/decco.sql` (database-first: o schema é do banco, não gerado por código).
Lógica idempotente-segura:
```
1. Testar existência:  IF DB_ID('DeccoDB') IS NULL  → 'MISSING'  senão 'EXISTS'
   (sqlcmd:  sqlcmd -S <servidor> -Q "IF DB_ID('DeccoDB') IS NULL PRINT 'MISSING' ELSE PRINT 'EXISTS'")
2. MISSING → rodar o script:
   sqlcmd -S <servidor> -i "<skill>/assets/decco.sql"      (ou Invoke-Sqlcmd -InputFile ...)
3. EXISTS  → NÃO re-rodar (o decco.sql faz CREATE DATABASE e falharia).
   Recriar? só com confirmação explícita (DROP DATABASE DeccoDB;  depois rodar o script) — é DESTRUTIVO.
```
> `<servidor>` = `(localdb)\MSSQLLocalDB` (LocalDB), `localhost,1433` (Docker/instância), etc. — **um único parâmetro** (§4).
> As tabelas de **auth** (que o DeccoDB não tem) são **code-first** e criadas à parte (`recipes/02` / `reference/12`), não pelo `decco.sql`.

## 4. Config de banco = **um único ponto, trocável** (LocalDB ⇄ Docker ⇄ remoto)
A connection string é **uma** configuração (`ConnectionStrings:DeccoDb` em `appsettings.Development.json`, sobreposta por env var/user-secrets). Trocar de ambiente = trocar **essa linha**, nada mais:
- **LocalDB (default do Tier 0):** `Server=(localdb)\MSSQLLocalDB;Database=DeccoDB;Trusted_Connection=True;MultipleActiveResultSets=True`
- **Docker (1ª etapa de aprendizagem, guiada):** `Server=localhost,1433;Database=DeccoDB;User Id=sa;Password=<via user-secrets>;TrustServerCertificate=True;MultipleActiveResultSets=True`
> **MARS (`MultipleActiveResultSets=True`) é obrigatório** para o híbrido EF+Dapper coexistir na mesma conexão (`reference/14`).
> **Primeira etapa de aprendizagem sugerida:** começar em **LocalDB** (conforto) e depois **migrar o mesmo banco para Docker** trocando só a connection string — a skill guia o `docker run` do `mcr.microsoft.com/mssql/server` e a re-execução do `assets/decco.sql`. É o exercício que prova a **trocabilidade** da config.

## 5. Dúvidas de implementação — sempre didáticas, mesmo "fora de ordem"
Se o operador perguntar algo que **parece pular um tier** ou **fugir do assunto**, **responder a dúvida** (conceito + como o Decco
aplica + porquê), e só então **situar no mapa** ("isto normalmente entra no Tier N; posso adiantar um mini-exemplo agora"). **Nunca**
recusar com "isso é de uma etapa posterior". A ordem dos tiers é um **guia**, não uma cerca. Ver diretriz de comportamento no `SKILL.md`.

## Fecho
Terminar cada passo com o checkpoint e **um fio a puxar**. Se um pré-requisito faltar, dizer o comando de instalação e oferecer a
alternativa (ex.: sem Docker → LocalDB). Registrar qualquer decisão nova em `knowledge-drops/` e dúvidas em `open-questions/`.
