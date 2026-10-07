param()

# Kills the previous Decco.Api.REST process before rebuilding
$pidFile = Join-Path ([System.IO.Path]::GetTempPath()) '.decco-api-rest.pid'
if (Test-Path $pidFile) {
  $oldPid = [int](Get-Content $pidFile -Raw).Trim()
  try { Stop-Process -Id $oldPid -Force -ErrorAction Stop; Write-Host "Killed PID $oldPid (handshake)" } catch {}
  try { Remove-Item $pidFile -Force } catch {}
}

# Fallback: kills any Decco.Api.REST process
$procs = Get-CimInstance Win32_Process -Filter "Name='dotnet.exe'" | Where-Object { $_.CommandLine -match 'Decco.Api.REST' }
foreach ($p in $procs) {
  Write-Host ("Killed PID " + $p.ProcessId + " (fallback)")
  Stop-Process -Id $p.ProcessId -Force -ErrorAction SilentlyContinue
}
