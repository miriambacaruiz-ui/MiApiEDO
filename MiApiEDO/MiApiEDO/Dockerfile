# Etapa 1: Compilar
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Copiamos el csproj (asegúrate de que el nombre sea exacto)
COPY ["MiApiEDO.csproj", "./"]
# Restauramos
RUN dotnet restore "MiApiEDO.csproj"
# Copiamos todo lo demás
COPY . .
# Publicamos
RUN dotnet publish "MiApiEDO.csproj" -c Release -o /app/publish

# Etapa 2: Ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:10000
ENTRYPOINT ["dotnet", "MiApiEDO.dll"]
