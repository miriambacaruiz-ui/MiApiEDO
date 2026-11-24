# Etapa 1: Compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MiApiEDO.csproj", "./"]
RUN dotnet restore "MiApiEDO.csproj"
COPY . .
# IMPORTANTE: Todo el comando en una sola línea
RUN dotnet publish "MiApiEDO.csproj" -c Release -o /app/publish

# Etapa 2: Ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# Forzamos a que escuche en el puerto 10000 de Render
ENV ASPNETCORE_URLS=http://+:10000
ENTRYPOINT ["dotnet", "MiApiEDO.dll"]
