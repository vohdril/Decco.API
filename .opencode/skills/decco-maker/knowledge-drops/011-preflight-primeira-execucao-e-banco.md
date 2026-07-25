# 011 — Preflight, primeira execução guiada, provisionamento do banco e dúvidas didáticas

- **Data de registo:** 2026-07-17
- **Fonte:** pedido do Paulo (a skill deve criar o banco, verificar pré-requisitos, guiar a 1ª execução, config de banco trocável, e tratar dúvidas fora de ordem de forma didática)
- **Tipo:** decisão de comportamento da skill
- **Afeta:** SKILL.md (Regra 10 + Diretrizes 9/10), `recipes/03` (novo), `reference/02`

## Decisões
1. **A skill PROVISIONA o DeccoDB (cria se não existir).** Database-first: Etapa 0 checa `DB_ID('DeccoDB')`; ausente → roda `assets/decco.sql` (a skill **carrega** o script); presente → não re-roda (não é idempotente); recriar só com **confirmação explícita** (DROP → é destrutivo). Isto garante que **em qualquer sistema ou por outro agente** a skill gera o banco a partir do Tier 0. Auth = code-first, à parte (não pelo `decco.sql`).
2. **Preflight de pré-requisitos** antes de implementar, por tier (`recipes/03` §1): Tier 0 = .NET 8 SDK + motor SQL (LocalDB/instância/Docker) + executor de `.sql` (sqlcmd/Invoke-Sqlcmd/mini-runner) + `dotnet-ef`; tiers seguintes = Docker p/ Redis/Kafka/Elasticsearch/Couchbase/Vault. Reportar checklist "✔/✗ + como instalar"; não avançar num tier com pré-requisito em falta sem avisar.
3. **Primeira execução = fluxo guiado** (`recipes/03` §2): preflight → confirmar alvo de banco → criar DeccoDB se não existir → esqueleto + scaffold → seguir `recipes/01` **um checkpoint por vez, narrando e esperando o operador**. Acompanhar quem opera; não despejar o projeto pronto.
4. **Banco local trocável; LocalDB primeiro, Docker como 1ª etapa de aprendizagem.** Default do Tier 0 = **LocalDB** (o operador domina). A connection string é **um único ponto** (`ConnectionStrings:DeccoDb`, com **MARS**); trocar LocalDB⇄Docker⇄remoto = mudar essa linha. A **primeira etapa guiada** é migrar o mesmo banco para **Docker** trocando só a config (prova a trocabilidade).
5. **Dúvidas de implementação sempre didáticas — mesmo "fora de ordem"/tier.** Responder a dúvida primeiro (conceito + como o Decco aplica + porquê), depois situar no mapa de tiers. **Nunca** recusar com "isso é de uma etapa posterior"; os tiers são guia, não cerca.

## Porquê
- **Autossuficiência real** exige que a skill saiba **subir o ambiente**, não só gerar código — daí o preflight + provisionamento do banco.
- **LocalDB primeiro** respeita o conforto do operador e reduz atrito no Tier 0; **Docker como etapa** transforma a portabilidade num exercício (e valida que a config é trocável).
- **Dúvidas fora de ordem** são o modo natural de aprender; recusá-las mataria o princípio didático da skill.

## Como aplicar
- No arranque/quando o operador for começar: **rodar `recipes/03`** (preflight → criar banco → guiar). 
- Nunca hardcodar connection string/segredo; usar user-secrets/env var.
- Ao receber uma dúvida que "pula" tier: responder didaticamente e situar; oferecer `open-questions/` se abrir zona nova.

## Fio a puxar
Depois do Tier 0 em LocalDB, migrar para Docker (`mcr.microsoft.com/mssql/server`) trocando só `ConnectionStrings:DeccoDb` e re-rodando `assets/decco.sql` — comparar o setup e discutir portabilidade/reprodutibilidade.
