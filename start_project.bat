@echo off
echo SmartCar Projesi Baslatiliyor...

:: 1. Backend'i Baslat (Yeni pencerede açar)
echo Backend (WebAPI) baslatiliyor...
start "SmartCar Backend" cmd /k "cd SmartCar\WebAPI && dotnet run --urls=http://localhost:5242"

:: 2. Frontend'i Baslat (Python sunucusu ile)
echo Frontend baslatiliyor...
start "SmartCar Frontend" cmd /k "cd Frontend && python -m http.server 8000"

:: 3. Biraz bekle ve tarayiciyi aç
echo Tarayici aciliyor (Lutfen bekleyin)...
timeout /t 5 >nul
start http://localhost:8000

echo Islem tamam! Bu pencereyi kapatabilirsiniz (Diger siyah pencereler acik kalmali).
pause
