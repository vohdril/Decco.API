param()

# Mata o processo anterior da Decco.API.REST antes de rebuildar
$pidFile = Join-Path ([System.IO.Path]::GetTempPath()) '.decco-api-rest.pid'
if (Test-Path $pidFile) {
  $oldPid = [int](Get-Content $pidFile -Raw).Trim()
  try { Stop-Process -Id $oldPid -Force -ErrorAction Stop; Write-Host "Matou PID $oldPid (handshake)" } catch {}
  try { Remove-Item $pidFile -Force } catch {}
}

# Fallback: mata qualquer processo Decco.Api.REST
$procs = Get-CimInstance Win32_Process -Filter "Name='dotnet.exe'" | Where-Object { $_.CommandLine -match 'Decco.Api.REST' }
foreach ($p in $procs) {
  Write-Host ("Matou PID " + $p.ProcessId + " (fallback)")
  Stop-Process -Id $p.ProcessId -Force -ErrorAction SilentlyContinue
}
