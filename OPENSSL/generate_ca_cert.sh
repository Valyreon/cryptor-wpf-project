# generate certificate request and key for it
openssl req -new -newkey rsa:2048 -keyout private/ca.key -out careq.pem -config ./openssl.cnf

# generate and selfsign CA certificate with onfig from openssl.conf
openssl ca -out cacert.pem -days 365 -keyfile private/ca.key -selfsign -config ./openssl.cnf -infiles careq.pem

# print information from CA certificate
openssl x509 -in cacert.pem -noout -text