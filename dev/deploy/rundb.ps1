pwd
docker-compose --file "dev/deploy/docker-compose.yaml" up -d
#Start-Process 'docker-compose' `
#    -ArgumentList ".", "up" `
#    -NoNewWindow `
#    -Wait