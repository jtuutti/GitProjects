@echo off
del .\RestFoundation\bin\Release\*.* /f /q
del .\RestFoundation\obj\Release\*.* /f /q
del .\Dependencies\Rest.Foundation.* /f /q
pause
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild RestFoundation\RestFoundation.csproj /t:Rebuild /p:Configuration=Release
cd .\RestFoundation\bin\Release
echo.
echo.
echo Performing IL Merge
ilmerge /out:Rest.Foundation.dll RestFoundation.dll Newtonsoft.Json.dll /ver:1.0.0.0 /v4 /keyfile:..\..\..\RestFoundation.snk /xmldocs
echo.
echo.
copy Rest.Foundation.* ..\..\..\Dependencies\ /y
echo.
pause
