FROM mcr.microsoft.com/dotnet/core/sdk microsoft/dotnet:3.1 AS build-env
WORKDIR /app

# Copy and restore as distinct layers
COPY *.sln ./
COPY ./src/Voidwell.Cache/*.csproj ./src/Voidwell.Cache/
COPY ./src/Voidwell.DaybreakGames.Api/*.csproj ./src/Voidwell.DaybreakGames.Api/
COPY ./src/Voidwell.DaybreakGames.App/*.csproj ./src/Voidwell.DaybreakGames.App/
COPY ./src/Voidwell.DaybreakGames.CensusServices/*.csproj ./src/Voidwell.DaybreakGames.CensusServices/
COPY ./src/Voidwell.DaybreakGames.CensusStream/*.csproj ./src/Voidwell.DaybreakGames.CensusStream/
COPY ./src/Voidwell.DaybreakGames.Data/*.csproj ./src/Voidwell.DaybreakGames.Data/
COPY ./src/Voidwell.DaybreakGames.GameState/*.csproj ./src/Voidwell.DaybreakGames.GameState/
COPY ./src/Voidwell.DaybreakGames.Models/*.csproj ./src/Voidwell.DaybreakGames.Models/
COPY ./src/Voidwell.DaybreakGames.Services/*.csproj ./src/Voidwell.DaybreakGames.Services/
COPY ./src/Voidwell.DaybreakGames.Utils/*.csproj ./src/Voidwell.DaybreakGames.Utils/
COPY ./test/Voidwell.DaybreakGames.Test/*.csproj ./test/Voidwell.DaybreakGames.Test/

RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

# Copy the app
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000

# Start the app
ENTRYPOINT dotnet Voidwell.DaybreakGames.Api.dll