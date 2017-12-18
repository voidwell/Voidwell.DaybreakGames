pushd %~dp0\src\Voidwell.DaybreakGames.Data
dotnet ef migrations add initialPS2DbContext -v ^
    -c Voidwell.DaybreakGames.Data.PS2DbContext ^
    -s ./../Voidwell.DaybreakGames/Voidwell.DaybreakGames.csproj ^
    -o ./Migrations ^
    --msbuildprojectextensionspath ./../../build/Voidwell.DaybreakGames/Debug/obj
popd