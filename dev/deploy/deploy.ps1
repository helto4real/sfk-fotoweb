docker build -t helto4real/fotowebbapi -f .\Dockerfile.Api . --no-cache 
docker build -t helto4real/fotowebb -f .\Dockerfile.Web . --no-cache 
docker push helto4real/fotowebbapi
docker push helto4real/fotowebb
