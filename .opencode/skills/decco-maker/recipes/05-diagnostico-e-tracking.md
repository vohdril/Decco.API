# recipes/05 — Diagnóstico de progresso + tracking (a frio, read-only)

> Procedimento para responder **"em que Tier o projeto está e o que falta"** cruzando **código × rubrica de sinais**
> (`reference/17`). Projetado para rodar numa **conversa sem contexto** (primeira vez vendo o repositório). **Read-only por
> padrão**: só escreve no `DECCO-PROGRESS.md` do projeto se o operador **pedir explicitamente**. **Nunca** edita a skill.
>
> Gatilhos típicos: *"rode um diagnóstico do progresso atual"*, *"em que tier estou?"*, *"o que falta pro próximo checkpoint?"*,
> *"analise as implementações X e Y"*, *"validação de progresso"*.

## A. Diagnóstico completo — passo a passo
**1. Localizar o alvo.** Usar o diretório atual do repositório (ou o caminho que o operador indicar). Não presumir contexto de
conversas anteriores — tudo vem do disco.

**2. Descobrir os tracks** (tabela de âncoras, `reference/17` §1): procurar `*.csproj` (net8.0 → moderno; v4.8+`.edmx` → legado),
`package.json`+`src/` (front), sinais de auth, connection strings/serviços de persistência, `DECCO-PROGRESS.md`. Listar o que existe.

**3. Ler o DECLARADO.** Se houver `DECCO-PROGRESS.md` na raiz, ler (é a intenção do operador). **Não confiar cego** — validar contra
o código no passo 4. Se não houver, seguir só com o observado (e, ao final, **oferecer** criá-lo).

**4. Checar os SINAIS (o observado).** Para cada track detectado, percorrer os conceitos-checkpoint da rubrica (`reference/17` §2) e
marcar **presente / ausente / parcial** procurando o **sinal decisivo** de cada um. Ferramentas: `Grep` (símbolos: `RequestBase`,
`useQuery`, `CommandType.StoredProcedure`, `visibleTo`, `AddJwtBearer`…), leitura de `package.json`/`*.csproj`/`packages.config`
(dependências), `Glob` (arquivos/pastas: `src/data/gateway.ts`, `*.edmx`, `Receive/`), e o `decco.sql`/banco (objetos `sp_*`,
tabelas). Registrar a **evidência** (arquivo:símbolo) de cada presente — o relatório cita de onde tirou.

**5. Derivar o Tier observado** por track (`reference/17` §3): maior tier com todos os obrigatórios presentes (cumulativo); senão
`Tier N em andamento (x/y)` com presentes e faltantes. Opcionais fora do denominador. Tracks independentes.

**6. Detectar fora-de-sequência** (`reference/17` §4): conceito de tier superior com o tier corrente ainda aberto → **aviso
didático** (não-bloqueante), sempre com o caminho para fechar o tier corrente. Opt-in explícito não gera aviso.

**7. Reconciliar** declarado × observado (`reference/17` §5): o **observado vence**; divergências viram avisos (matriz).

**8. Emitir o relatório** no formato de `reference/17` §6: lidera pelo **Tier observado por track**, depois avisos, encerra com **um
fio a puxar** (a maior lacuna do tier corrente). Para cada faltante do tier corrente, dar a **ação objetiva** (o que implementar, com
ponteiro à `reference/`/`recipe` — ex.: *"falta `repo-dapper-sp` → adicionar `sp_Anomalia_Buscar` via Dapper, ver `reference/14`"*).

**9. (Só sob pedido explícito) Atualizar o tracking.** Se o operador disser *"atualize o progresso"* / *"marque o checkpoint X"*,
então **escrever no `DECCO-PROGRESS.md` do PROJETO** (nunca na skill): marcar checkboxes conforme o observado, ajustar o tier
declarado, atualizar a data. Se o arquivo não existir, criá-lo a partir de `templates/DECCO-PROGRESS.md`. Confirmar antes de gravar.

## B. Diagnóstico focado — "analise X e Y"
Quando o operador nomeia implementações/conceitos específicos (ex.: *"analise o seam de dados e a autorização na UI"*):
1. Mapear cada X/Y para o(s) **conceito-checkpoint** da rubrica (`reference/17` §2) e seu **sinal decisivo**.
2. Checar só esses sinais no código; reportar **presente/parcial/ausente** com evidência (arquivo:símbolo) e, se parcial, **o que
   falta** para o conceito ficar completo.
3. Situar cada um no seu **Tier** e dizer se está **na ordem** ou **adiantado** (fora-de-sequência).
4. Encerrar com o **próximo passo** coerente e um fio a puxar. Não precisa varrer o projeto todo — é um recorte assertivo.

## C. Invariantes (o que NÃO fazer)
- **Não editar a skill** (nenhum arquivo em `decco-maker/`) — nem knowledge-drops, nem open-questions — salvo pedido explícito
  (Regra de ouro 12). O diagnóstico é análise, não enriquecimento.
- **Não escrever no projeto** sem pedido explícito — o diagnóstico padrão é **relatório**; o `DECCO-PROGRESS.md` só muda no passo 9.
- **Não reprovar/bloquear.** Tiers são guia; o tom é de estudo. O observado vence o declarado, sempre como aviso.
- **Não inventar sinais.** Se um sinal for ambíguo, reportar como **parcial/indício fraco** e dizer o que confirmaria.

## D. Exemplo (estado atual do projeto de referência)
Rodando no `front` de referência (só Track C): saída esperada ≈
```
▸ Track C — Front .......... FE0 completo (7/7)
   Próximo: FE1 — flip do seam para live (completar data/httpApi.ts; VITE_DATA_SOURCE=live; ou SDK NSwag)
▸ Track A/B — Back ......... não iniciado (sem *.csproj)
🔎 Fio a puxar: subir o back Tier-0 e virar o seam do front para `live` num endpoint (listAnomalias).
```

## E. Verificação
✅ O diagnóstico roda **sem** contexto de conversa, cita **evidência** por conceito, deriva **Tier observado por track**, lista
**faltantes com ação**, sinaliza **fora-de-sequência/divergências** de forma não-bloqueante e **não escreveu** em nada (salvo pedido).
