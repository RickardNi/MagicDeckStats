@echo off
set PORT=5263

netstat -ano | findstr :%PORT% > nul
if %errorlevel% equ 0 (
    echo Port %PORT% in use, killing process...
    for /f "tokens=5" %%a in ('netstat -ano ^| findstr :%PORT%') do taskkill /F /PID %%a 2>nul
)

dotnet watch run --non-interactive