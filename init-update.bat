pushd %~dp0\src\Voidwell.DaybreakGames.Data
dotnet ef database update -v ^
    -c Voidwell.DaybreakGames.Data.PS2DbContext ^
    -s ./../Voidwell.DaybreakGames/Voidwell.DaybreakGames.csproj ^
    --msbuildprojectextensionspath ./../../build/Voidwell.DaybreakGames/Debug/obj
popd