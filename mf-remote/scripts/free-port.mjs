/*
  free-port.mjs — frees the port of a Vite server before it starts.

  Wired as `predev` / `prepreview` in package.json, so `npm run dev` and `npm run preview` first
  stop whatever process is LISTENING on the port. The port comes from vite.config.ts
  (server.port / preview.port) through Vite's own loadConfigFromFile — the same value Vite is
  about to bind, with no second copy of it. Without a fixed port there is nothing to free: Vite
  picks the next free one by itself.

  It never stops system, Docker, Visual Studio, IIS Express or database processes: it names them
  and exits with an error, so the dev server does not start against a port you must free yourself.
  Windows only (the Decco environment); elsewhere it does nothing.
*/
import { execFileSync } from "node:child_process";
import { setTimeout as sleep } from "node:timers/promises";
import { loadConfigFromFile } from "vite";

const PROTECTED = new Set([
  "idle", "system", "svchost", "services", "lsass", "wininit", "csrss", "smss", // Windows
  "com.docker.backend", "com.docker.proxy", "com.docker.service", "vpnkit",     // Docker: the port
  "wslrelay", "docker desktop", "dockerd",                                       // is a container's
  "devenv", "iisexpress",                                                        // Visual Studio, IIS Express
  "sqlservr", "msmdsrv",                                                         // SQL Server, Analysis Services
]);

const log = (message) => console.log(`[free-port] ${message}`);

if (process.platform !== "win32") {
  log("skipped: implemented for Windows only");
  process.exit(0);
}

const isPreview = process.argv[2] === "preview";
const loaded = await loadConfigFromFile({ command: "serve", mode: "development", isPreview });
const port = isPreview ? loaded?.config.preview?.port : loaded?.config.server?.port;

if (!port) {
  log(`no fixed ${isPreview ? "preview" : "server"} port in vite.config; nothing to free`);
  process.exit(0);
}

/** Processes LISTENING on the port (one entry per process, IPv4 and IPv6 together). */
function listeners() {
  const script =
    `Get-NetTCPConnection -State Listen -LocalPort ${port} -ErrorAction SilentlyContinue | ` +
    `Select-Object -ExpandProperty OwningProcess -Unique | ` +
    `ForEach-Object { [pscustomobject]@{ pid = [int]$_; name = "$((Get-Process -Id $_ -ErrorAction SilentlyContinue).ProcessName)" } } | ` +
    // A free port makes Get-NetTCPConnection "fail" quietly, and PowerShell would exit with 1.
    `ConvertTo-Json -Compress; exit 0`;
  const out = execFileSync("powershell", ["-NoProfile", "-NonInteractive", "-Command", script], { encoding: "utf8" }).trim();
  if (!out) return [];
  const parsed = JSON.parse(out);
  return Array.isArray(parsed) ? parsed : [parsed];
}

const current = listeners();
if (current.length === 0) {
  log(`port ${port} is free`);
  process.exit(0);
}

let blocked = false;
const stopped = [];
for (const { pid, name } of current) {
  if (pid === process.pid || pid === process.ppid) continue;
  if (pid <= 4 || PROTECTED.has(name.toLowerCase())) {
    const hint = /docker|vpnkit|wslrelay/i.test(name) ? " Stop the container that publishes it (docker ps)." : "";
    log(`port ${port} is held by ${name} (PID ${pid}); not stopping it.${hint}`);
    blocked = true;
    continue;
  }
  try {
    // /T also ends the children (Vite's esbuild service, for instance).
    execFileSync("taskkill", ["/PID", String(pid), "/T", "/F"], { stdio: "ignore" });
    stopped.push(pid);
    log(`stopped ${name} (PID ${pid}), listening on port ${port}`);
  } catch {
    log(`could not stop ${name} (PID ${pid})`);
    blocked = true;
  }
}

// The port is released when the process is gone; wait for it before the server starts.
for (let i = 0; i < 25 && listeners().some((l) => stopped.includes(l.pid)); i++) await sleep(200);

if (blocked) process.exit(1);
