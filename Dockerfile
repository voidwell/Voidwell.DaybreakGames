FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy and restore as distinct layers
COPY *.sln ./
COPY ./src/Voidwell.DaybreakGames.App/*.csproj ./src/Voidwell.DaybreakGames.App/
COPY ./src/Voidwell.DaybreakGames.Api/*.csproj ./src/Voidwell.DaybreakGames.Api/
COPY ./src/Voidwell.DaybreakGames.Data/*.csproj ./src/Voidwell.DaybreakGames.Data/
COPY ./src/Voidwell.DaybreakGames.CensusStore/*.csproj ./src/Voidwell.DaybreakGames.CensusStore/
COPY ./src/Voidwell.DaybreakGames.CensusServices/*.csproj ./src/Voidwell.DaybreakGames.CensusServices/
COPY ./src/Voidwell.DaybreakGames.Utils/*.csproj ./src/Voidwell.DaybreakGames.Utils/
COPY ./src/Voidwell.Cache/*.csproj ./src/Voidwell.Cache/
COPY ./test/Voidwell.DaybreakGames.Test/*.csproj ./test/Voidwell.DaybreakGames.Test/

RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN find -type d -name bin -prune -exec rm -rf {} \; && find -type d -name obj -prune -exec rm -rf {} \;
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