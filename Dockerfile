FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["CalendarManagementApi/CalendarManagementApi.csproj", "CalendarManagementApi/"]
RUN dotnet restore "CalendarManagementApi/CalendarManagementApi.csproj"
COPY . .
WORKDIR "/src/CalendarManagementApi"
RUN dotnet build "CalendarManagementApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CalendarManagementApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
HEALTHCHECK --interval=30s --timeout=10s --retries=3 --start-period=15s \
    CMD curl -f http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "CalendarManagementApi.dll"]
