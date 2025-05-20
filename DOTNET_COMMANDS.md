# All commands used to setup the project
dotnet new gitignore
dotnet new packagesprops
dotnet new editorconfig
dotnet new globaljson
dotnet new sln -n tabularius-dotnet-lib

# In vscode switch to Solution Explorer

## TabulariusLib
dotnet new classlib -n TabulariusLib -o src/TabulariusLib
dotnet solution add src/TabulariusLib

## Tests
dotnet new xunit -n UnitTests -o tests/UnitTests
dotnet solution add tests/UnitTests