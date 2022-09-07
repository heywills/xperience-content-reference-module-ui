@echo off
del *.nupkg
msbuild /t:restore .\ContentReferenceUi.csproj
msbuild /p:Configuration=Release .\ContentReferenceUi.csproj
IF %ERRORLEVEL% NEQ 0 (Echo An error was found &Exit /b 1)
nuget pack ContentReferenceUi.csproj -Prop Configuration=Release 
IF %ERRORLEVEL% NEQ 0 (Echo An error was found &Exit /b 1)
copy .\KenticoCommunity.ContentReferenceUi.*.nupkg C:\_OfflineNugetSource\
pause
