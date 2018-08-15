FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /app

# Copy and restore as distinct layers
COPY *.sln ./
COPY ./src/Voidwell.Cache/*.csproj ./src/Voidwell.Cache/
COPY ./src/Voidwell.DaybreakGames/*.csproj ./src/Voidwell.DaybreakGames/
COPY ./src/Voidwell.DaybreakGames.Census/*.csproj ./src/Voidwell.DaybreakGames.Census/
COPY ./src/Voidwell.DaybreakGames.Data/*.csproj ./src/Voidwell.DaybreakGames.Data/
COPY ./test/Voidwell.DaybreakGames.Census.Test/*.csproj ./test/Voidwell.DaybreakGames.Census.Test/
COPY ./test/Voidwell.DaybreakGames.Test/*.csproj ./test/Voidwell.DaybreakGames.Test/

RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Build runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime

# Copy the app
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000

# Start the app
ENTRYPOINT dotnet Voidwell.DaybreakGames.dll