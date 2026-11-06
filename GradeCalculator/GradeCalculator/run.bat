@echo off
title Student Grade Calculator
where dotnet >nul 2>nul
if errorlevel 1 (
  echo .NET SDK is not installed.
  echo Opening the download page. Install it, then double-click run.bat again.
  start https://dotnet.microsoft.com/download
  pause
  exit /b
)
echo Starting... your browser will open at http://localhost:5080
echo Close this window (or press Ctrl+C) to stop the program.
start "" http://localhost:5080
dotnet run
pause
