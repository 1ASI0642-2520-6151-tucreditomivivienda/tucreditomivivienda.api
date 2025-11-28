# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el .csproj y restaura dependencias
COPY ["TuCreditoMiVivienda.Api/TuCreditoMiVivienda.Api.csproj", "TuCreditoMiVivienda.Api/"]
RUN dotnet restore "TuCreditoMiVivienda.Api/TuCreditoMiVivienda.Api.csproj"

# Copia todo el código y publica
COPY . .
WORKDIR "/src/TuCreditoMiVivienda.Api"
RUN dotnet publish -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

# Render pone la variable PORT; la usamos para escuchar en ese puerto
CMD ["sh", "-c", "dotnet TuCreditoMiVivienda.Api.dll --urls http://0.0.0.0:${PORT}"]
