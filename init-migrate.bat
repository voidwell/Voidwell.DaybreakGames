pushd %~dp0\src\Voidwell.DaybreakGames.Data
set ASPNETCORE_ENVIRONMENT=Development

set dt=%DATE:~10,4%_%DATE:~4,2%_%DATE:~7,2%_%TIME:~0,2%_%TIME:~3,2%_%TIME:~6,2%
set migration=ps2dbcontext.%dt%

dotnet ef migrations add %migration% -v ^
    -c Voidwell.DaybreakGames.Data.PS2DbContext ^
    -o ./Migrations ^
    --msbuildprojectextensionspath ./../../build/Voidwell.DaybreakGames.Data/Debug/obj
popd