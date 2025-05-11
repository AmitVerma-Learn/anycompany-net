# AnyCompany .NET Framework 4.8 Modernization - Deployment Package

This package contains the AnyCompany Banking Product Catalogue application for deployment on a Windows EC2 instance.

## Prerequisites

- Windows Server 2019 or later
- .NET Framework 4.8 SDK
- Visual Studio 2022 (any edition: Community, Professional, or Enterprise)
- MySQL Server 9.2 (can be on the same instance or a separate RDS instance)

## Deployment Instructions

### 1. Launch a Windows EC2 Instance

1. Log in to the AWS Management Console
2. Navigate to EC2 and click "Launch Instance"
3. Choose a Windows Server 2019 or 2022 AMI
4. Select an instance type (t3.medium recommended minimum)
5. Configure security groups to allow:
   - RDP (port 3389) from your IP
   - MySQL (port 3306) if using a separate database server
6. Launch the instance and connect via RDP

### 2. Set Up the Environment

1. Connect to your EC2 instance using RDP
2. Install .NET Framework 4.8 SDK if not already installed
   - Download from: https://dotnet.microsoft.com/download/dotnet-framework/net48
3. Install MySQL Server 9.2 if you plan to host the database on the same instance
   - Download from: https://dev.mysql.com/downloads/mysql/9.2.html

### 3. Deploy the Application

1. Copy this deployment package to the EC2 instance
2. Extract the package to a folder (e.g., C:\AnyCompanyApp)
3. Open PowerShell as Administrator
4. Navigate to the extracted folder: `cd C:\AnyCompanyApp`
5. Run the setup script: `.\setup.ps1`
6. Follow the prompts to:
   - Enter database connection information
   - Build the application
   - Set up the database (if needed)

### 4. Run the Application

After successful setup, you can run the application from:
`.\AnyCompanyBankingProduct\bin\Release\AnyCompanyBankingProduct.exe`

## Configuration

The application uses the following environment variables for database connection:
- `DB_SERVER`: Database server address
- `DB_USER`: Database username
- `DB_PASSWORD`: Database password

These are set automatically by the setup script.

## Troubleshooting

If you encounter issues:

1. Check that .NET Framework 4.8 is properly installed
2. Verify database connectivity
3. Check Windows Event Logs for application errors
4. Ensure security groups allow necessary connections

## Support

For additional assistance, please refer to the documentation or contact the development team.
