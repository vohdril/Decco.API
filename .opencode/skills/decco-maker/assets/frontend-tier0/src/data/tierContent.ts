export interface TierContent {
  intro: string;
  code: string;
  lang: string;
}

export const TIER_CONTENT: Record<string, TierContent> = {
  FE0: {
    lang: "tsx",
    intro:
      "O Tier 0 estabelece o scaffold do frontend com Vite, React 19 e TypeScript. " +
      "A aplicação renderiza um componente raiz que serve como console mock do DeCCO, " +
      "sem dependência de backend — toda a navegação e dados são simulados localmente.",
    code: `import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <h1>DeCCO Console Mock</h1>
  </StrictMode>
)`,
  },
  FE1: {
    lang: "ts",
    intro:
      "Conexão real com a Decco.API via httpApi.ts. Usa fetch tipado para comunicar " +
      "com os endpoints do backend, com toggle entre dados mock e live. Prepara o " +
      "terreno para autenticação JWT e SDK NSwag.",
    code: `const BASE = "http://localhost:5000/api"

export async function healthCheck(): Promise<{ status: string }> {
  const res = await fetch(\`\${BASE}/health\`)
  if (!res.ok) throw new Error("API indisponível")
  return res.json()
}`,
  },
  FE2: {
    lang: "tsx",
    intro:
      "Visualização avançada de dados com @tanstack/react-table para tabelas " +
      "e Recharts para gráficos. O dashboard ganha ordenação, filtros, paginação " +
      "e gráficos em tempo real com dados servidos pela API.",
    code: `import {
  useReactTable,
  createColumnHelper,
  getCoreRowModel,
} from "@tanstack/react-table"

const columnHelper = createColumnHelper<Anomalia>()
const columns = [
  columnHelper.accessor("codigoSCP", { header: "SCP" }),
  columnHelper.accessor("nome", { header: "Anomalia" }),
]

const table = useReactTable({
  data,
  columns,
  getCoreRowModel: getCoreRowModel(),
})`,
  },
  FE3: {
    lang: "tsx",
    intro:
      "Visualização imersiva do núcleo DeCCO com Three.js e React Three Fiber. " +
      "Uma esfera com shaders animados representa o reator de contenção, exibindo " +
      "dados em tempo real com efeitos de glow e partículas.",
    code: `import { Canvas } from "@react-three/fiber"

function Esfera() {
  return (
    <mesh>
      <sphereGeometry args={[1, 32, 32]} />
      <meshStandardMaterial color="#34d3e6" />
    </mesh>
  )
}

;<Canvas>
  <ambientLight />
  <Esfera />
</Canvas>`,
  },
  FE4: {
    lang: "js",
    intro:
      "Arquitetura de micro-frontends com Module Federation do Webpack 5. " +
      "Permite que squads independentes desenvolvam e deployem módulos separados " +
      "(Glossário, Dashboard, Laboratório) sem acoplamento.",
    code: `new ModuleFederationPlugin({
  name: "decco_container",
  remotes: {
    glossario:
      "glossario@http://localhost:3001/remoteEntry.js",
    laboratorio:
      "laboratorio@http://localhost:3002/remoteEntry.js",
  },
  shared: { react: { singleton: true } },
})`,
  },
  BE0: {
    lang: "csharp",
    intro:
      "O backend começa com um scaffold .NET 8 em camadas (API, Domain, Infrastructure), " +
      "envelope Request/Response padronizado, EF Core + Dapper híbrido e a primeira " +
      "rota de health check que valida a conexão com o banco.",
    code: `var builder = WebApplication.CreateBuilder(args)
var app = builder.Build()

app.MapGet("/api/health", () =>
  Results.Ok(new { status = "operacional", timestamp = DateTime.UtcNow })
)

app.Run()`,
  },
  BE1: {
    lang: "csharp",
    intro:
      "Endpoints REST para entidades do DeccoDB (Cognição Aparente, Periculosidade, " +
      "Laboratório, Protocolo) seguindo o padrão envelope com request/response tipados " +
      "e validação via FluentValidation.",
    code: `app.MapGet("/api/cognicoes", async (DeccoDbContext db) =>
  await db.Cognicoes.ToListAsync()
)

app.MapPost("/api/cognicoes", async (
  CognicaoRequest req,
  DeccoDbContext db
) => {
  var entity = new Cognicao { Nome = req.Nome }
  db.Cognicoes.Add(entity)
  await db.SaveChangesAsync()
  return Results.Created($"/api/cognicoes/{entity.Id}", entity)
})`,
  },
  BE2: {
    lang: "csharp",
    intro:
      "Autenticação JWT com RBAC baseado em clearance e sítio. A entidade principal " +
      "Anomalia ganha CRUD completo com resource-based authorization — cada agente " +
      "só vê anomalias compatíveis com seu nível de acesso.",
    code: `builder.Services.AddAuthentication(
  JwtBearerDefaults.AuthenticationScheme
).AddJwtBearer(o =>
  o.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(
      Encoding.UTF8.GetBytes("chave-secreta")
    ),
    ValidateIssuer = true,
    ValidIssuer = "Decco.API",
  }
)`,
  },
  BE3: {
    lang: "csharp",
    intro:
      "Camada de cache com Redis / FusionCache para catálogos de alta consulta, " +
      "paginação com Criteria, stored procedures complexas e otimização de queries " +
      "para reduzir latência no painel de monitoramento.",
    code: `builder.Services.AddFusionCache()
  .WithDefaultEntryOptions(new FusionCacheEntryOptions
  {
    Duration = TimeSpan.FromMinutes(5),
  })

app.MapGet("/api/catalogo", async (IFusionCache cache) =>
  await cache.GetOrSetAsync(
    "catalogo",
    async ct => await db.Catalogo.ToListAsync()
  )
)`,
  },
  BE4: {
    lang: "csharp",
    intro:
      "Sistema distribuído com mensageria Kafka, tracing distribuído via OpenTelemetry, " +
      "health checks agregados e logs estruturados com Serilog. Observabilidade total " +
      "para diagnóstico de anomalias em produção.",
    code: `builder.Services.AddOpenTelemetry()
  .WithTracing(t => t
    .AddAspNetCoreInstrumentation()
    .AddConsoleExporter()
  )

builder.Services.AddCap(x =>
{
  x.UseRabbitMQ("localhost")
  x.UseSqlServer(connectionString)
})`,
  },
  DB0: {
    lang: "sql",
    intro:
      "O banco de dados inicia com o schema do DeccoDB: tabelas base (Anomalia, Classe, " +
      "Camada) com lore brasileiro (Cognição Aparente, Periculosidade, Protocolo OA), " +
      "seeds de dados fictícios estilo SCP e índices/constraints.",
    code: `CREATE TABLE Anomalia (
    Id          INT IDENTITY PRIMARY KEY,
    CodigoSCP   VARCHAR(20)  NOT NULL UNIQUE,
    Nome        VARCHAR(100) NOT NULL,
    Descricao   TEXT,
    CognicaoId  INT          NOT NULL,
    PericulosidadeId INT    NOT NULL,
    CONSTRAINT FK_Anomalia_Cognicao
        FOREIGN KEY (CognicaoId) REFERENCES Cognicao(Id)
)`,
  },
  DB1: {
    lang: "sql",
    intro:
      "Stored procedures para inserção e atualização de catálogos. Chamadas via Dapper " +
      "com CommandType.StoredProcedure e POCOs com sufixo QR que mapeiam diretamente " +
      "os parâmetros das procedures.",
    code: `CREATE PROCEDURE sp_Cognicao_Inserir
    @Nome       VARCHAR(100),
    @Descricao  VARCHAR(255)
AS
    INSERT INTO Cognicao (Nome, Descricao)
    VALUES (@Nome, @Descricao);

    SELECT SCOPE_IDENTITY() AS Id;`,
  },
  DB2: {
    lang: "sql",
    intro:
      "Tabelas associativas para relacionamentos muitos-para-muitos entre Anomalia e Classe, " +
      "laboratórios e protocolos. Joins de múltiplas entidades, views materializadas e " +
      "índices covering para performance.",
    code: `CREATE VIEW vw_AnomaliaClasses AS
SELECT a.CodigoSCP,
       a.Nome,
       c.Nome AS Classe
FROM Anomalia a
JOIN AnomaliaClasse ac ON a.Id = ac.AnomaliaId
JOIN Classe c          ON c.Id = ac.ClasseId;`,
  },
  DB3: {
    lang: "sql",
    intro:
      "Full-text indexes no SQL Server para busca textual em descrições de anomalias, " +
      "integração com Elasticsearch para busca avançada com pontuação, sugestões de " +
      "autocomplete e busca por Código SCP.",
    code: `CREATE FULLTEXT CATALOG DeccoCatalog AS DEFAULT;

CREATE FULLTEXT INDEX ON Anomalia(
    Nome,
    Descricao TYPE COLUMN DescricaoTipo
)
KEY INDEX PK_Anomalia
ON DeccoCatalog
WITH CHANGE_TRACKING AUTO;`,
  },
  DB4: {
    lang: "sql",
    intro:
      "Persistência poliglota combinando SQL Server e NoSQL (MongoDB / Redis). " +
      "Change Data Capture (CDC) para auditoria de todas as alterações, event sourcing " +
      "e audit trail completo de entidades críticas.",
    code: `EXEC sys.sp_cdc_enable_table
    @source_schema = 'dbo',
    @source_name   = 'Anomalia',
    @role_name     = NULL;

-- As alterações ficam em cdc.dbo_Anomalia_CT
-- consumidas por workers ou Debezium + Kafka`,
  },
};
