FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine3.11 AS builder
WORKDIR /src
ADD . .
RUN dotnet restore
RUN dotnet publish -r linux-musl-x64

FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1-alpine3.11
WORKDIR /app
COPY --from=builder /src/Squirrel.Ingest/bin/Debug/netcoreapp3.1/linux-musl-x64/publish/ .
CMD ["/app/Squirrel.Ingest"]
