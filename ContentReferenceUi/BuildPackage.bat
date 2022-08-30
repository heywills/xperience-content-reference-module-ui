dotnet build .\ContentReferenceUi.csproj -c Release
nuget pack ContentReferenceUi.csproj -Prop Configuration=Release
copy .\ContentReferenceUi.1.0.0.nupkg C:\_OfflineNugetSource\
@echo off
pause
