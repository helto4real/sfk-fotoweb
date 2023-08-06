$env:ASPNETCORE_URLS='https://localhost:8001'
cd src/Foto.WebServer
$host.ui.RawUI.WindowTitle = 'Webserver'
dotnet watch run --no-hot-reload