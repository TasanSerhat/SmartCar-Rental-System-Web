
$employees = @(
    @{ FirstName = "Ali"; LastName = "Yilmaz"; Email = "ali@smartcar.com"; Role = "Manager"; Salary = 35000; BranchId = 1 },
    @{ FirstName = "Ayse"; LastName = "Demir"; Email = "ayse@smartcar.com"; Role = "Salesperson"; Salary = 25000; BranchId = 1 },
    @{ FirstName = "Mehmet"; LastName = "Kaya"; Email = "mehmet@smartcar.com"; Role = "Technician"; Salary = 28000; BranchId = 2 }
)

foreach ($e in $employees) {
    # Add dummies
    $e.Status = $true
    $e.PasswordHash = "ZHVtbXloYXNo"
    $e.PasswordSalt = "ZHVtbXlzYWx0"
    
    $body = $e | ConvertTo-Json
    try {
        Invoke-RestMethod -Uri "http://localhost:5242/api/Employees/add" -Method Post -Body $body -ContentType "application/json"
        Write-Host "Added $($e.FirstName)"
    } catch {
        Write-Host "Failed to add $($e.FirstName): $_"
    }
}
