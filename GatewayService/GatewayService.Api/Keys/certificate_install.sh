#!/bin/bash

#Create keypair
openssl genrsa -des3 -passout pass:$1 -out keypair.key 2048

#Create private.key
sudo openssl rsa -passin pass:$1 -in keypair.key -out private.key

#Remove keypair
sudo rm keypair.key

#Create request for certificate
sudo openssl req -new -key private.key -out request.csr

#Create certificate
sudo openssl x509 -req -days 365 -in request.csr -signkey private.key -out public.crt
