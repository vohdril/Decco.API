using Microsoft.AspNetCore.Mvc;

namespace Decco.Api.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MfEndpointController : ControllerBase
{
    [HttpGet("html")]
    public ContentResult GetHtml()
    {
        var html = /*lang=html*/ """
        <!DOCTYPE html>
        <html lang="pt-BR">
        <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Micro-Frontend Demo — Decco</title>
        <style>
          * { margin:0; padding:0; box-sizing:border-box; }
          body {
            font-family: 'Segoe UI', system-ui, -apple-system, sans-serif;
            background: #0f1117;
            color: #e4e6ef;
            padding: 24px;
            min-height: 100%;
          }
          h1 { font-size:1.1rem; font-weight:600; margin-bottom:4px; display:flex; align-items:center; gap:8px; }
          h1 small { font-size:0.7rem; color:#888baa; font-weight:400; }
          .badge { display:inline-block; padding:2px 8px; border-radius:999px; font-size:0.65rem; font-weight:600; letter-spacing:0.3px; text-transform:uppercase; }
          .badge-mf { background:#6366f140; color:#a5b4fc; }
          .badge-api { background:#22c55e20; color:#4ade80; }
          .grid { display:grid; grid-template-columns:repeat(3,1fr); gap:12px; margin-top:16px; }
          .card { background:#1a1d29; border-radius:8px; padding:16px; border:1px solid #2a2d3a; transition:border-color .2s, box-shadow .2s; }
          .card:hover { border-color:#6366f166; box-shadow:0 0 16px #6366f120; }
          .card .num { font-size:1.8rem; font-weight:700; }
          .card .label { font-size:0.7rem; color:#888baa; margin-top:2px; text-transform:uppercase; letter-spacing:0.5px; }
          .severity-row { display:flex; gap:6px; margin-top:16px; }
          .sev { flex:1; padding:8px 6px; border-radius:6px; text-align:center; font-size:0.65rem; font-weight:600; }
          .sev span { display:block; font-size:1rem; font-weight:700; margin-top:2px; }
          .sev-0 { background:#e6394620; color:#e63946; border:1px solid #e6394630; }
          .sev-1 { background:#f77f0020; color:#f77f00; border:1px solid #f77f0030; }
          .sev-2 { background:#fcbf4920; color:#fcbf49; border:1px solid #fcbf4930; }
          .sev-3 { background:#a8dadc20; color:#a8dadc; border:1px solid #a8dadc30; }
          .sev-4 { background:#457b9d20; color:#457b9d; border:1px solid #457b9d30; }
          .sev-5 { background:#1d355720; color:#1d3557; border:1px solid #1d355730; }
          .sev-6 { background:#6a4c9320; color:#c084fc; border:1px solid #6a4c9330; }
          .sev-7 { background:#10b98120; color:#34d399; border:1px solid #10b98130; }
          .footer { margin-top:20px; font-size:0.65rem; color:#555770; display:flex; justify-content:space-between; }
          .footer code { background:#1a1d29; padding:1px 6px; border-radius:4px; color:#888baa; }
          .glow { width:100%; height:3px; background:linear-gradient(90deg,#6366f1,#a78bfa,#6366f1); background-size:200% 100%; animation:shimmer 2s ease infinite; border-radius:8px 8px 0 0; margin-bottom:16px; }
          @keyframes shimmer { 0%{background-position:200% 0} 100%{background-position:-200% 0} }
          .env { display:inline-flex; align-items:center; gap:6px; font-size:0.6rem; color:#555770; }
          .env svg { width:8px; height:8px; border-radius:50%; background:#22c55e; }
        </style>
        </head>
        <body>
          <div class="glow"></div>
          <h1>
            🧩 Painel de Anomalias
            <small>Micro-frontend via API</small>
          </h1>
          <div style="display:flex; gap:8px; margin-top:6px; flex-wrap:wrap;">
            <span class="badge badge-mf">Module Federation 2.0</span>
            <span class="badge badge-api">iframe Legacy</span>
            <span class="env"><svg></svg> Online</span>
          </div>

          <div class="grid">
            <div class="card">
              <div class="num" style="color:#e63946;">12</div>
              <div class="label">Críticas</div>
            </div>
            <div class="card">
              <div class="num" style="color:#f77f00;">7</div>
              <div class="label">Altas</div>
            </div>
            <div class="card">
              <div class="num" style="color:#fcbf49;">23</div>
              <div class="label">Médias</div>
            </div>
            <div class="card">
              <div class="num" style="color:#a8dadc;">45</div>
              <div class="label">Baixas</div>
            </div>
            <div class="card">
              <div class="num" style="color:#c084fc;">8</div>
              <div class="label">Desconhecidas</div>
            </div>
            <div class="card">
              <div class="num" style="color:#34d399;">92</div>
              <div class="label">Total</div>
            </div>
          </div>

          <div class="severity-row">
            <div class="sev sev-0">Crítico <span>12</span></div>
            <div class="sev sev-1">Alto <span>7</span></div>
            <div class="sev sev-2">Médio <span>23</span></div>
            <div class="sev sev-3">Baixo <span>45</span></div>
            <div class="sev sev-6">N/A <span>5</span></div>
            <div class="sev sev-7">OK <span>3</span></div>
          </div>

          <div class="footer">
            <span>© Decco · Demonstração de Micro-Frontend</span>
            <span><code>GET /api/mf-endpoint/html</code> · <span id="ts"></span></span>
          </div>

          <script>
            document.getElementById('ts').textContent = new Date().toLocaleTimeString('pt-BR') + ' · ' + new Date().toLocaleDateString('pt-BR');
          </script>
        </body>
        </html>
        """;

        return Content(html, "text/html; charset=utf-8");
    }

    [HttpGet("demo2")]
    public ContentResult GetHtml2()
    {
        var html = /*lang=html*/ """
        <!DOCTYPE html>
        <html lang="pt-BR">
        <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Decco · Segundo Endpoint</title>
        <style>
          * { margin:0; padding:0; box-sizing:border-box; }
          body {
            font-family: 'Courier New', monospace;
            background: #0a0806;
            color: #b8953a;
            padding: 32px;
          }
          .terminal { border:1px solid #3a2a0a; border-radius:6px; padding:24px; background:#0d0a05; }
          h1 { font-size:1rem; color:#f0c060; letter-spacing:.12em; text-transform:uppercase; }
          .line { margin-top:12px; font-size:0.85rem; display:flex; gap:12px; }
          .label { color:#6a5a2a; min-width:100px; }
          .val { color:#b8953a; }
          blink { animation:blink 1s step-end infinite; }
          @keyframes blink { 50%{opacity:0} }
        </style>
        </head>
        <body>
          <div class="terminal">
            <h1>🔌 Endpoint de Teste #2</h1>
            <div class="line"><span class="label">STATUS:</span><span class="val">ONLINE</span></div>
            <div class="line"><span class="label">TIPO:</span><span class="val">HTML PURO (iframe)</span></div>
            <div class="line"><span class="label">ORIGEM:</span><span class="val">Decco.API / MfEndpointController</span></div>
            <div class="line"><span class="label">TIMESTAMP:</span><span class="val"><span id="ts"></span></span></div>
            <div class="line" style="margin-top:20px; color:#5a4a2a; font-size:0.7rem;">
              Altere o <code>src</code> da MicroFrontendLegacyModal para este endpoint e veja o resultado imediato.
            </div>
          </div>
          <script>
            document.getElementById('ts').textContent = new Date().toLocaleString('pt-BR');
          </script>
        </body>
        </html>
        """;

        return Content(html, "text/html; charset=utf-8");
    }

    [HttpGet("user-card")]
    public ContentResult GetUserCard()
    {
        var html = /*lang=html*/ """
        <!DOCTYPE html>
        <html lang="pt-BR">
        <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>DeCCO · Credenciais do Operador</title>
        <style>
          * { margin:0; padding:0; box-sizing:border-box; }
          body {
            font-family: 'Segoe UI', system-ui, -apple-system, sans-serif;
            background: #0f1117;
            color: #e4e6ef;
            padding: 24px;
            display:flex; justify-content:center; align-items:center; min-height:100%;
          }
          .card {
            background:#1a1d29; border-radius:10px; padding:24px;
            border:1px solid #2a2d3a; max-width:420px; width:100%;
          }
          .card h1 {
            font-size:0.95rem; font-weight:600; margin-bottom:4px;
            display:flex; align-items:center; gap:8px; color:#f0c060;
          }
          .card .sub {
            font-size:0.65rem; color:#888baa; margin-bottom:18px; line-height:1.5;
          }
          .field {
            display:flex; flex-direction:column; gap:3px;
            padding:10px 0; border-bottom:1px solid #2a2d3a;
          }
          .field:last-child { border-bottom:none; }
          .field .label {
            font-size:0.65rem; color:#6a6a8a; text-transform:uppercase;
            letter-spacing:0.5px; display:flex; align-items:center; gap:6px;
          }
          .field .value {
            font-size:0.9rem; color:#e4e6ef; font-weight:500;
            font-family:'Courier New', monospace;
          }
          .field .value--redacted { color:#cc4433; letter-spacing:2px; }
          .glow {
            width:100%; height:3px;
            background:linear-gradient(90deg,#6366f1,#a78bfa,#6366f1);
            background-size:200% 100%; animation:shimmer 2s ease infinite;
            border-radius:8px 8px 0 0; margin-bottom:16px;
          }
          @keyframes shimmer { 0%{background-position:200% 0} 100%{background-position:-200% 0} }
          .footer {
            margin-top:16px; font-size:0.6rem; color:#555770; display:flex; justify-content:space-between;
          }
          .footer code { background:#1a1d29; padding:1px 6px; border-radius:4px; color:#888baa; }
          .badge {
            display:inline-flex; align-items:center; gap:5px;
            background:#6366f120; color:#a5b4fc;
            padding:2px 8px; border-radius:999px; font-size:0.6rem; font-weight:600;
            text-transform:uppercase; margin-bottom:16px;
          }
        </style>
        </head>
        <body>
        <div class="card">
          <div class="glow"></div>
          <div class="badge">🛡️ Operador</div>
          <h1>🔐 Credenciais do Operador</h1>
          <div class="sub">Informações do usuário atualmente autenticado no sistema DeCCO.</div>
          <div class="field">
            <div class="label">👤 Nome Completo</div>
            <div class="value">Dr. Helena Vasquez</div>
          </div>
          <div class="field">
            <div class="label">🔑 Username</div>
            <div class="value value--redacted">████████</div>
          </div>
          <div class="field">
            <div class="label">🛡️ Nível de Clearance</div>
            <div class="value value--redacted">███</div>
          </div>
          <div class="field">
            <div class="label">🏢 Sítios Autorizados</div>
            <div class="value value--redacted">██████</div>
          </div>
          <div class="field">
            <div class="label">📧 Matrícula</div>
            <div class="value value--redacted">██████████</div>
          </div>
          <div class="footer">
            <span>© Decco · DeCCO Clearance</span>
            <span><code>GET /api/mf-endpoint/user-card</code> · <span id="ts"></span></span>
          </div>
        </div>
        <script>document.getElementById('ts').textContent = new Date().toLocaleTimeString('pt-BR');</script>
        </body>
        </html>
        """;

        return Content(html, "text/html; charset=utf-8");
    }
}
