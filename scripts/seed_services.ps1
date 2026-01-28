
$services = @(
    @{ ServiceName = "Baby Seat"; Cost = 15; Description = "Safety seat for infants." },
    @{ ServiceName = "GPS Navigation"; Cost = 10; Description = "Up-to-date offline maps." },
    @{ ServiceName = "Insurance Plus"; Cost = 50; Description = "Full coverage zero excess." },
    @{ ServiceName = "Additional Driver"; Cost = 20; Description = "Register a second driver." }
)

foreach ($s in $services) {
    $body = $s | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "http://localhost:5242/api/AdditionalServices/add" -Method Post -Body $body -ContentType "application/json"
        Write-Host "Added $($s.ServiceName)"
    } catch {
        Write-Host "Failed to add $($s.ServiceName): $_"
    }
}
