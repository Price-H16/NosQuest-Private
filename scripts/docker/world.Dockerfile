###
## Build docker
### 
FROM microsoft/dotnet:2.2-sdk as firstbuilder

WORKDIR /noswings/

COPY ./src/ ./

RUN dotnet build ./WingsEmu.Login/WingsEmu.Login.csproj -c Release -o /noswings/dist

## final builder
FROM firstbuilder as builder

WORKDIR /noswings/

# Copy everything
COPY ./src/ ./
# build World as publishable application in Release mode
RUN dotnet publish ./WingsEmu.Login/WingsEmu.Login.csproj -c Release -o /noswings/dist/

###
## Runtime Docker
###

## Use alpine as basis
FROM microsoft/dotnet:2.2-runtime-alpine

# Server listening port
ENV SERVER_PORT=4000 \
    SERVER_IP=127.0.0.1 \
    MASTER_IP=master.wingsemu \
    MASTER_PORT=20500 \
    DATABASE_IP=database.wingsemu \
    DATABASE_USER=sa \
    DATABASE_PASSWORD=strong_pass2018 \
    DATABASE_NAME=noswings_game \
    DATABASE_PORT=1433

LABEL name="wingsemu-world" \
    author="BlowaXD" \
    maintainer="BlowaXD <blowaxd693@gmail.com>"

# Setup Application Directory
RUN mkdir /server && \
    mkdir /server/plugins && \
    mkdir /server/plugins/config && \
    chmod -R +r /server/ && \
    chmod -R +x /server

WORKDIR /server/

COPY --from=builder /noswings/dist/ .

VOLUME /server/plugins

# SETUP DOCKER HEALTHCHECK
# Every 5 seconds, it will try to curl
# HEALTHCHECK --interval=15s --timeout=5s --start-period=5s --retries=3 CMD [ "netstat -an | grep $SERVER_PORT > /dev/null; if [ 0 != $? ]; then exit 1; fi;" ]

# Expose port 7777 tcp
EXPOSE 4000

ENTRYPOINT [ "dotnet", "/server/WingsEmu.Login.dll" ]