pushd %~dp0\src\Voidwell.DaybreakGames.Data
set ASPNETCORE_ENVIRONMENT=Development
dotnet ef database update -v ^
    -c Voidwell.DaybreakGames.Data.PS2DbContext ^
    --msbuildprojectextensionspath ./../../build/Voidwell.DaybreakGames.Data/Debug/obj
popd