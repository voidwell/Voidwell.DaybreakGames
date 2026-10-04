# syntax=docker/dockerfile:1.7-labs

# --- Build (restore + publish) ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY Directory.Build.props Directory.Packages.props ./
COPY --parents ./src/**/*.csproj ./

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore --nologo ./src/Voidwell.DaybreakGames/Voidwell.DaybreakGames.csproj

COPY . .

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -c Release -o /app/publish --no-restore ./src/Voidwell.DaybreakGames/Voidwell.DaybreakGames.csproj

# --- Runtime ---
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app

COPY --from=build /app/publish ./

RUN mkdir -p /data
VOLUME ["/data"]

ENTRYPOINT ["sh", "-c", "exec dotnet Voidwell.DaybreakGames.dll"]
