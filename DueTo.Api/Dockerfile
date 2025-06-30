FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Dueto.Api/Dueto.Api.csproj", "Dueto.Api/"]
COPY ["DueTo.Domain/DueTo.Domain.csproj", "DueTo.Domain/"]
COPY ["DueTo.Service/DueTo.Service.csproj", "DueTo.Service/"]
COPY ["DueTo.Repository/DueTo.Repository.csproj", "DueTo.Repository/"]
RUN dotnet restore "Dueto.Api/Dueto.Api.csproj"
COPY . .
WORKDIR "/src/Dueto.Api"
RUN dotnet build "Dueto.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Dueto.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dueto.Api.dll"]
