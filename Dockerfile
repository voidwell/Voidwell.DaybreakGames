FROM microsoft/dotnet:1.1.2-runtime

# Copy the app
COPY /publish /app
WORKDIR /app

# Configure the listening port to 80
ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000

# Start the app
ENTRYPOINT dotnet Voidwell.DaybreakGames.dll
