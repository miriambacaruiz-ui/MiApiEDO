# Etapa 1: Compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MiApiEDO.csproj", "./"]
RUN dotnet restore "MiApiEDO.csproj"
COPY . .
RUN dotnet publish "MiApiEDO.csproj" -c Release -o /app/publish

# Etapa 2: Ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# Render usa el puerto 10000 por defecto, le decimos a .NET que escuche ahí
ENV ASPNETCORE_URLS=http://+:10000
ENTRYPOINT ["dotnet", "MiApiEDO.dll"]