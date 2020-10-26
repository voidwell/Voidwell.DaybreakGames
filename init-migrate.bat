pushd %~dp0\src\Voidwell.DaybreakGames.Data
set ASPNETCORE_ENVIRONMENT=Development
dotnet ef migrations add ps2dbcontext.release.33 -v ^
    -c Voidwell.DaybreakGames.Data.PS2DbContext ^
    -o ./Migrations ^
    --msbuildprojectextensionspath ./../../build/Voidwell.DaybreakGames.Data/Debug/obj
popd