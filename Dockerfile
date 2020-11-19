FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine3.11 AS builder

WORKDIR /src

ADD  Squirrel.Database/Squirrel.Database.csproj Squirrel.Database/
ADD  Squirrel.Ingest/Squirrel.Ingest.csproj Squirrel.Ingest/
ADD Squirrel.sln .

RUN dotnet restore 

ADD . .

RUN dotnet publish -c Release -r linux-musl-x64


FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1-alpine3.11

WORKDIR /app

COPY --from=builder /src/Squirrel.Ingest/bin/Release/netcoreapp3.1/linux-musl-x64/publish/ .

CMD ["/app/Squirrel.Ingest"]
