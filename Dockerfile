FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy and restore as distinct layers
COPY *.sln ./
COPY ./src/Voidwell.DaybreakGames.Api/*.csproj ./src/Voidwell.DaybreakGames.Api/
COPY ./src/Voidwell.DaybreakGames.Census/*.csproj ./src/Voidwell.DaybreakGames.Census/
COPY ./src/Voidwell.DaybreakGames.CensusStore/*.csproj ./src/Voidwell.DaybreakGames.CensusStore/
COPY ./src/Voidwell.DaybreakGames.Data/*.csproj ./src/Voidwell.DaybreakGames.Data/
COPY ./src/Voidwell.DaybreakGames.Domain/*.csproj ./src/Voidwell.DaybreakGames.Domain/
COPY ./src/Voidwell.DaybreakGames.Live/*.csproj ./src/Voidwell.DaybreakGames.Live/
COPY ./src/Voidwell.DaybreakGames.Services/*.csproj ./src/Voidwell.DaybreakGames.Services/
COPY ./src/Voidwell.DaybreakGames.Utils/*.csproj ./src/Voidwell.DaybreakGames.Utils/

COPY ./test/Voidwell.DaybreakGames.Test/*.csproj ./test/Voidwell.DaybreakGames.Test/

RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN find -type d -name bin -prune -exec rm -rf {} \; && find -type d -name obj -prune -exec rm -rf {} \;
RUN dotnet publish -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Copy the app
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000

# Start the app
ENTRYPOINT dotnet Voidwell.DaybreakGames.Api.dll