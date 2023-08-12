# Docker kommandon

## Kör API
docker run --rm -p 5001:8001 -e ASPNETCORE_ENVIRONMENT="Development" -v C:\git\FotoApp\FotoApi\.db:/app/.db  -v C:\git\FotoApp\.crt:/app/.crt -e Kestrel__Endpoints__Https__Certificate__Path="/app/.crt/fotoweb.pem" -e Kestrel__Endpoints__Https__Certificate__KeyPath="/app/.crt/fotoweb-key.pem" -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=8001 foto-api:1.0.0-c7e37886e51dd89184b80658da02233a5da0413f