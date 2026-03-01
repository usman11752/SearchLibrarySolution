@echo off
echo ============================================
echo  Search Library - Run Tests + Coverage
echo ============================================

set DOTNET="C:\Program Files\dotnet\dotnet.exe"
set PROJ=SearchLibrary.Tests\SearchLibrary.Tests.csproj
set RESULTS=TestResults
set REPORT=CoverageReport

echo.
echo [1/4] Cleaning old results...
if exist %RESULTS% rmdir /s /q %RESULTS%
if exist %REPORT% rmdir /s /q %REPORT%

echo.
echo [2/4] Running tests with coverage collection...
%DOTNET% test %PROJ% --configuration Release --collect:"XPlat Code Coverage" --results-directory %RESULTS% --logger "console;verbosity=normal"

if %errorlevel% neq 0 (
    echo.
    echo !! TESTS FAILED - check output above !!
    pause
    exit /b 1
)

echo.
echo [3/4] Installing ReportGenerator (if needed)...
%DOTNET% tool install -g dotnet-reportgenerator-globaltool 2>nul
if %errorlevel% neq 0 (
    echo    ReportGenerator already installed, continuing...
)

echo.
echo [4/4] Generating HTML coverage report...
set RGEN=%USERPROFILE%\.dotnet\tools\reportgenerator.exe
%RGEN% -reports:"%RESULTS%\**\coverage.cobertura.xml" -targetdir:%REPORT% -reporttypes:Html

echo.
echo ============================================
echo  DONE! Opening coverage report...
echo ============================================
start %REPORT%\index.html

pause
