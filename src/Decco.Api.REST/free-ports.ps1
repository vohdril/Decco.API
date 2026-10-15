<#
  free-ports.ps1 - frees this API's ports before it builds and starts.

  Called by the FreePorts target in the .csproj before the build (dotnet run, dotnet build and
  every Visual Studio start). It stops:
    1. any process LISTENING on a port of a "Project" profile in Properties\launchSettings.json;
    2. any instance of this app still running from this project's bin folder - it locks the
       DLLs the build is about to overwrite, even when it listens on another port.

  It never stops system, Docker, Visual Studio, IIS Express or database processes: those are
  reported as an MSBuild warning, and the port is left for you to free.
#>
param(
  [Parameter(Mandatory)][string]$ProjectDir,
  [Parameter(Mandatory)][string]$AppName
)

$protectedNames = @(
  'idle', 'system', 'svchost', 'services', 'lsass', 'wininit', 'csrss', 'smss',   # Windows
  'com.docker.backend', 'com.docker.proxy', 'com.docker.service', 'vpnkit',       # Docker: the port
  'wslrelay', 'docker desktop', 'dockerd',                                         # is a container's
  'devenv', 'iisexpress',                                                          # Visual Studio, IIS Express
  'sqlservr', 'msmdsrv'                                                            # SQL Server, Analysis Services
)

function Get-ProfilePorts {
  $path = Join-Path $ProjectDir 'Properties\launchSettings.json'
  if (-not (Test-Path $path)) { return @() }
  $settings = Get-Content $path -Raw | ConvertFrom-Json
  $ports = foreach ($launchProfile in $settings.profiles.PSObject.Properties.Value) {
    # IIS Express profiles are left out on purpose: Visual Studio owns that process.
    if ($launchProfile.commandName -ne 'Project' -or -not $launchProfile.applicationUrl) { continue }
    foreach ($url in $launchProfile.applicationUrl -split ';') { ([Uri]$url.Trim()).Port }
  }
  return @($ports | Sort-Object -Unique)
}

function Get-Listeners([int[]]$Ports) {
  if (-not $Ports) { return @() }
  # One row per (port, process): Kestrel and Vite listen on IPv4 and IPv6 loopback at once.
  return @(Get-NetTCPConnection -State Listen -LocalPort $Ports -ErrorAction SilentlyContinue |
    Sort-Object LocalPort, OwningProcess -Unique)
}

try {
  $ports = Get-ProfilePorts
  $reasons = @{}   # PID -> why it is a target

  foreach ($listener in Get-Listeners $ports) {
    $reasons[[int]$listener.OwningProcess] = "listening on port $($listener.LocalPort)"
  }

  $binDir = (Join-Path $ProjectDir 'bin') + '\'
  Get-Process -Name $AppName -ErrorAction SilentlyContinue |
    Where-Object { $_.Path -and $_.Path.StartsWith($binDir, [StringComparison]::OrdinalIgnoreCase) } |
    ForEach-Object { if (-not $reasons.ContainsKey($_.Id)) { $reasons[$_.Id] = 'running from this project''s bin folder' } }

  $stopped = @()
  foreach ($id in @($reasons.Keys)) {
    $process = Get-Process -Id $id -ErrorAction SilentlyContinue
    if (-not $process -or $id -eq $PID) { continue }

    $name = $process.ProcessName
    if ($id -le 4 -or $protectedNames -contains $name.ToLowerInvariant()) {
      $hint = if ($name -match 'docker|vpnkit|wslrelay') { ' Stop the container that publishes it (docker ps).' } else { '' }
      Write-Host "free-ports : warning FREEPORT001 : $name (PID $id) is $($reasons[$id]) and was not stopped.$hint"
      continue
    }

    try {
      Stop-Process -Id $id -Force -ErrorAction Stop
      $stopped += $id
      Write-Host "free-ports: stopped $name (PID $id), $($reasons[$id])"
    } catch {
      Write-Host "free-ports : warning FREEPORT002 : could not stop $name (PID $id): $($_.Exception.Message)"
    }
  }

  # The port is released when the process is gone; wait for it before the build goes on.
  if ($stopped) { Wait-Process -Id $stopped -Timeout 5 -ErrorAction SilentlyContinue }
} catch {
  Write-Host "free-ports : warning FREEPORT003 : $($_.Exception.Message)"
}

exit 0
