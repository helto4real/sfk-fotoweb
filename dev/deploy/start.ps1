Start-Process -FilePath "powershell.exe" -ArgumentList "-NoExit", "-Command", "dev/deploy/RunApi.ps1"
Start-Process -FilePath "powershell.exe" -ArgumentList "-NoExit", "-Command", "dev/deploy/WatchWebserver.ps1"