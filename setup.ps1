# Setup script for AnyCompany Banking Product
Write-Host "Setting up AnyCompany Banking Product..." -ForegroundColor Green

# Prompt for database connection information
$DB_SERVER = Read-Host -Prompt "Enter database server address"
$DB_USER = Read-Host -Prompt "Enter database username"
$DB_PASSWORD = Read-Host -Prompt "Enter database password" -AsSecureString
$BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($DB_PASSWORD)
$DB_PASSWORD_PLAIN = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)

# Set environment variables
[Environment]::SetEnvironmentVariable("DB_SERVER", $DB_SERVER, "Machine")
[Environment]::SetEnvironmentVariable("DB_USER", $DB_USER, "Machine")
[Environment]::SetEnvironmentVariable("DB_PASSWORD", $DB_PASSWORD_PLAIN, "Machine")

# Build the application
Write-Host "Building the application..." -ForegroundColor Green
$msbuildPath = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
if (-not (Test-Path $msbuildPath)) {
    Write-Host "MSBuild not found at expected path. Searching for MSBuild..." -ForegroundColor Yellow
    $msbuildPath = (Get-Command MSBuild -ErrorAction SilentlyContinue).Source
    if (-not $msbuildPath) {
        # Try additional common locations including Visual Studio 2022
        $possiblePaths = @(
            # Visual Studio 2022 paths
            "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
            "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
            "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
            # Visual Studio 2019 paths (for backward compatibility)
            "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
            "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe",
            "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
        )
        
        foreach ($path in $possiblePaths) {
            if (Test-Path $path) {
                $msbuildPath = $path
                break
            }
        }
        
        if (-not $msbuildPath) {
            Write-Host "MSBuild not found. Please install .NET Framework 4.8 SDK." -ForegroundColor Red
            exit 1
        }
    }
}

Write-Host "Using MSBuild at: $msbuildPath" -ForegroundColor Green
& $msbuildPath AnyCompanyBankingProduct.sln /p:Configuration=Release

# Check if build was successful
if (-not $?) {
    Write-Host "Build failed. Please check the error messages above." -ForegroundColor Red
    exit 1
}

# Update connection string in config file
Write-Host "Updating connection string..." -ForegroundColor Green
$configPath = ".\AnyCompanyBankingProduct\bin\Release\AnyCompanyBankingProduct.exe.config"
if (Test-Path $configPath) {
    # Use more robust pattern matching for connection string updates
    $content = Get-Content $configPath -Raw
    $content = $content -replace 'Server=localhost', "Server=$DB_SERVER"
    $content = $content -replace 'Uid=root', "Uid=$DB_USER"
    $content = $content -replace 'Pwd=;', "Pwd=$DB_PASSWORD_PLAIN;"
    Set-Content $configPath $content
}

# Install MySQL client if not already installed
Write-Host "Checking for MySQL client..." -ForegroundColor Green
if (-not (Get-Command mysql -ErrorAction SilentlyContinue)) {
    Write-Host "MySQL client not found. Installing MySQL client..." -ForegroundColor Yellow
    
    # Create temp directory
    $tempDir = ".\mysql-temp"
    New-Item -ItemType Directory -Force -Path $tempDir | Out-Null
    
    # Download MySQL installer
    $mysqlUrl = "https://dev.mysql.com/get/Downloads/MySQL-9.2/mysql-9.2.0-winx64.zip"
    $mysqlZip = "$tempDir\mysql.zip"
    Write-Host "Downloading MySQL client..." -ForegroundColor Green
    Invoke-WebRequest -Uri $mysqlUrl -OutFile $mysqlZip
    
    # Extract MySQL
    Write-Host "Extracting MySQL client..." -ForegroundColor Green
    Expand-Archive -Path $mysqlZip -DestinationPath "C:\mysql" -Force
    
    # Add MySQL to PATH
    Write-Host "Adding MySQL to PATH..." -ForegroundColor Green
    $env:Path += ";C:\mysql\bin"
    [Environment]::SetEnvironmentVariable("Path", $env:Path, "Machine")
    
    # Clean up
    Remove-Item -Path $tempDir -Recurse -Force
}

# Run database setup script if requested
$setupDB = Read-Host -Prompt "Do you want to set up the database? (y/n)"
if ($setupDB -eq "y") {
    Write-Host "Setting up database..." -ForegroundColor Green
    Get-Content database_setup.sql | mysql -h $DB_SERVER -u $DB_USER -p"$DB_PASSWORD_PLAIN"
}

Write-Host "Setup complete! You can now run the application from AnyCompanyBankingProduct\bin\Release\AnyCompanyBankingProduct.exe" -ForegroundColor Green
