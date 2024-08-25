# simple PACS Server orthanc -> docker-compose.yml

version: '3.1'  # Secrets are only available since this version of Docker Compose
services:
  orthanc:
    image: jodogne/orthanc-plugins:1.12.4
    command: /run/secrets/  # Path to the configuration files (stored as secrets)
    ports:
      - 4242:4242
      - 8042:8042
    secrets:
      - orthanc.json
    environment:
      - ORTHANC_NAME=HelloWorld
secrets:
  orthanc.json:




orthanc config file: orthanc.json

{
  "Name" : "${ORTHANC_NAME} in Docker Compose",
  "RemoteAccessAllowed" : true
}
