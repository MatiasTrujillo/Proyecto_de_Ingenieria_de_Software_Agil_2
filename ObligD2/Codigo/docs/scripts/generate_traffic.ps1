# PowerShell helper to generate traffic for PharmaGo API
# Usage examples:
#   .\generate_traffic.ps1 -SendSuccess 100 -DelayMs 100
#   .\generate_traffic.ps1 -SendForced500 10 -DelayMs 200
#   .\generate_traffic.ps1 -SendMixed 50 -DelayMs 100

param(
    [int]$SendSuccess = 0,
    [int]$SendForced500 = 0,
    [int]$SendMixed = 0,
    [int]$SendForced400 = 0,
    [int]$DelayMs = 100
)

function Send-Post([string]$nombre, [int]$cantidad) {
    $body = @{ 
        NombreMedicamento = $nombre
        CantidadMedicamento = $cantidad
        FechaExpiracion = (Get-Date).AddMonths(6).ToString('o')
        Usuario = 'load-test'
        RequierePrescripcion = $false
    } | ConvertTo-Json

    try {
        $r = Invoke-RestMethod -Uri 'http://localhost:5000/api/reserva' -Method Post -Body $body -ContentType 'application/json' -ErrorAction Stop
        Write-Output "OK: $($nombre) -> 201"
    } catch {
        # show status code if available
        if ($_.Exception.Response -ne $null) {
            $resp = $_.Exception.Response
            Write-Output "ERROR: $($nombre) -> $($resp.StatusCode)"
        } else {
            Write-Output "ERROR: $($nombre) -> $($_.Exception.Message)"
        }
    }
}

# Send a deliberately malformed JSON body to provoke a 400 Bad Request
function Send-BadJson([string]$payload) {
    try {
        # send as application/json but payload is not valid JSON
        $r = Invoke-RestMethod -Uri 'http://localhost:5000/api/reserva' -Method Post -Body $payload -ContentType 'application/json' -ErrorAction Stop
        Write-Output "OK (unexpected): malformed -> 201"
    } catch {
        if ($_.Exception.Response -ne $null) {
            $resp = $_.Exception.Response
            Write-Output "BADREQUEST: malformed -> $($resp.StatusCode)"
        } else {
            Write-Output "BADREQUEST: malformed -> $($_.Exception.Message)"
        }
    }
}

# Send n successful requests
for ($i=0; $i -lt $SendSuccess; $i++) {
    Send-Post -nombre "Medicamento-$i" -cantidad 1
    Start-Sleep -Milliseconds $DelayMs
}

# Send n forced 500 requests (NombreMedicamento = FORCE500 triggers exception in controller)
for ($i=0; $i -lt $SendForced500; $i++) {
    Send-Post -nombre "FORCE500" -cantidad 1
    Start-Sleep -Milliseconds $DelayMs
}

# Send n forced 400 requests (malformed JSON)
for ($i=0; $i -lt $SendForced400; $i++) {
    # malformed JSON: unquoted property name or single quotes will trigger a 400
    $bad = "{ nombreMedicamento: 'MALFORMED', CantidadMedicamento: 1 }"
    Send-BadJson $bad
    Start-Sleep -Milliseconds $DelayMs
}

# Send mixed: half success, half forced 500
for ($i=0; $i -lt $SendMixed; $i++) {
    if ($i % 2 -eq 0) { Send-Post -nombre "Medicamento-mix-$i" -cantidad 1 } else { Send-Post -nombre "FORCE500" -cantidad 1 }
    Start-Sleep -Milliseconds $DelayMs
}

Write-Output "Done"
