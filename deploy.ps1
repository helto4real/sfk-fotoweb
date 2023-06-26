docker build -t helto4real/fotowebbapi -f .\Dockerfile.Api .
docker build -t helto4real/fotowebb -f .\Dockerfile.Web .
docker push helto4real/fotowebbapi
docker push helto4real/fotowebb
