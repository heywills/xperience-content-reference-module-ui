del *.nupkg
dotnet build .\ContentReferenceUi.csproj -c Release
nuget pack ContentReferenceUi.csproj -Prop Configuration=Release
copy .\KenticoCommunity.ContentReferenceUi.*.nupkg C:\_OfflineNugetSource\
@echo off
pause
