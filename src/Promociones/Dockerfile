#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Promociones.csproj", ""]
RUN dotnet restore "./Promociones.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Promociones.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Promociones.csproj" -c Release -o /app/publish

FROM base AS final
ENV ASPNETCORE_URLS="https://+"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="./webserver.pfx"
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="password"
WORKDIR /app
COPY webserver.pfx .
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Promociones.dll"]