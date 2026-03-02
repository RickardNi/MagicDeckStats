@echo off
netstat -ano | findstr :5263 > nul
if %errorlevel% equ 0 (
    echo Port 5263 in use, killing process...
    for /f "tokens=5" %%a in ('netstat -ano ^| findstr :5263') do taskkill /F /PID %%a 2>nul
)

dotnet watch run --non-interactive
