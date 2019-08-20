openssl genrsa -out private/luka.key 2048
openssl req -new -config openssl.cnf -key private/luka.key -out certreqs/luka.csr
#openssl x509 -req -in certreqs/userOne.csr -CA ca.crt -CAkey ca.key -CAcreateserial -out userOne.crt
openssl ca -config openssl.cnf -infiles certreqs/luka.csr