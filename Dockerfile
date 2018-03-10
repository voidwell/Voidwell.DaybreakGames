FROM microsoft/dotnet:2.0.0-sdk AS build-env
WORKDIR /app

# Copy and restore as distinct layers
COPY *.sln ./
COPY ./src/Voidwell.Cache/*.csproj ./src/Voidwell.Cache/
COPY ./src/Voidwell.DaybreakGames/*.csproj ./src/Voidwell.DaybreakGames/
COPY ./src/Voidwell.DaybreakGames.Census/*.csproj ./src/Voidwell.DaybreakGames.Census/
COPY ./src/Voidwell.DaybreakGames.Data/*.csproj ./src/Voidwell.DaybreakGames.Data/

RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Build runtime image
FROM microsoft/aspnetcore:2.0.0

# Copy the app
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000

# Start the app
ENTRYPOINT dotnet Voidwell.DaybreakGames.dll