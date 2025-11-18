# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia apenas os .csproj para aproveitar cache de camadas
COPY ["PaymentGateway.API/PaymentGateway.API.csproj", "PaymentGateway.API/"]
COPY ["PaymentGateway.Application/PaymentGateway.Application.csproj", "PaymentGateway.Application/"]
COPY ["PaymentGateway.Domain/PaymentGateway.Domain.csproj", "PaymentGateway.Domain/"]
COPY ["PaymentGateway.Infrastructure/PaymentGateway.Infrastructure.csproj", "PaymentGateway.Infrastructure/"]

# Restaura dependências com base nos .csproj copiados (cache-friendly)
RUN dotnet restore "PaymentGateway.API/PaymentGateway.API.csproj"

# Copia todo o código fonte
COPY . .

# Restaura novamente com todo o código presente (resolve referências relativas ou arquivos gerados)
RUN dotnet restore "PaymentGateway.API/PaymentGateway.API.csproj"

# Publica o projeto em Release
RUN dotnet publish "PaymentGateway.API/PaymentGateway.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=build /app/publish .
EXPOSE 80

ENTRYPOINT ["dotnet", "PaymentGateway.API.dll"]