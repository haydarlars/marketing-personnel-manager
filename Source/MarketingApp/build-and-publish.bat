@echo off
REM ============================================================
REM Build and Publish Script
REM Run this from the Source\MarketingApp\MarketingApp.API folder
REM Output goes to ..\..\..\..\Project\Deploy
REM ============================================================

echo Building and publishing MarketingApp...

dotnet publish MarketingApp.API.csproj ^
  --configuration Release ^
  --output ..\..\..\Deploy ^
  --runtime win-x64 ^
  --self-contained false

echo.
echo Done! Publish output is in the Deploy folder.
echo Next steps:
echo   1. Update Deploy\appsettings.json with your SQL Server connection string.
echo   2. Run SQL\01_CreateDatabase.sql on your SQL Server.
echo   3. Follow Deploy\DeploymentGuide.md to configure IIS.
pause
