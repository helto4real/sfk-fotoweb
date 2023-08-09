# Skapa self-signed certifikat

## Skapa nyckel

```bash

openssl req -x509 -newkey rsa:4096 -config fotoweb-openssl.cnf -keyout fotoweb-key.pem -out fotoweb.pem -sha256 -days 365

```

## openssl cofigurationsfil openssl.cnf

```
[ req ]
default_bits = 2048
prompt = no
default_md = sha256
distinguished_name = dn
x509_extensions = x509_ext

[ dn ]
C = SE
ST = Vasternorrland
L = Sundsvall
O = JoySoftware
OU = JoySoftware CA
CN = fotoweb.sundsvallsfotoklubb.se

[ x509_ext ]
subjectAltName = @alt_names

[ alt_names ]
DNS.1 = fotoweb.sundsvallsfotoklubb.se
DNS.2 = localhost
```