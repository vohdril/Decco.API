# templates/ — moldes de código (solidificam no primeiro slice)

Ao contrário do `partners-maker` (cujos templates derivam de um vertical slice **já existente** — a entidade Extra), o sandbox
Decco **ainda não tem código gerado**. Portanto os moldes aqui nascem **derivados por convenção** (das `reference/` + do padrão
Omnibees) e **solidificam** quando o **primeiro slice vertical** (Anomalia, `recipes/01`) for gerado e validado.

## Regra
- Enquanto um molde não existir, a skill **gera a estrutura padrão por convenção** (a partir das `reference/`), marcando a
  lógica específica com `// >>> <o quê> (investigar: <pista>)`.
- Quando o Paulo validar o primeiro exemplar de um artefato, **promover** esse exemplar a `templates/<artefato>.md`
  (parametrizado com `{Entidade}`/`{entidade}`/`{Entidades}`), como fez o `partners-maker`, e registar um `knowledge-drop`.

## Artefatos previstos (a criar sob demanda)
**Core (Decco.API):** `envelope.md` · `manager.md` · `repositorio-hibrido.md` (EF + Dapper/SP) · `controller-envelope.md`.
**Fachada (Foundation.API):** `contratos-dtos.md` (13 classes) · `validadores.md` · `conversores.md` (Receive/List) ·
`repositorio-remake.md` (FusionCache + HTTP) · `servico.md` (envelope) · `controller-v2.md`.

> Cada template, ao ser criado, deve trazer também um bloco **Porquê** (razão de desenho) — coerente com o princípio didático (`knowledge-drops/001`).
