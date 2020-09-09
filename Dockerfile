FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

# Copy and restore as distinct layers
COPY *.sln ./
COPY ./src/Voidwell.DaybreakGames.App/*.csproj ./src/Voidwell.DaybreakGames.App/
COPY ./src/Voidwell.DaybreakGames.Api/*.csproj ./src/Voidwell.DaybreakGames.Api/
COPY ./src/Voidwell.DaybreakGames.Data/*.csproj ./src/Voidwell.DaybreakGames.Data/
COPY ./src/Voidwell.DaybreakGames.CensusServices/*.csproj ./src/Voidwell.DaybreakGames.CensusServices/
COPY ./src/Voidwell.Cache/*.csproj ./src/Voidwell.Cache/
COPY ./test/Voidwell.DaybreakGames.Test/*.csproj ./test/Voidwell.DaybreakGames.Test/

RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime

# Copy the app
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000

# Start the app
ENTRYPOINT dotnet Voidwell.DaybreakGames.Api.dll