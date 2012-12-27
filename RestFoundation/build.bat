@echo off
del .\RestFoundation\bin\Release\*.* /f /q
del .\RestFoundation\obj\Release\*.* /f /q
mkdir .\build > nul
del .\build\Rest.Foundation.* /f /q > nul

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild RestFoundation\RestFoundation.csproj /t:Rebuild /p:Configuration=Release
cd .\RestFoundation\bin\Release

echo.
echo.
echo Performing IL Merge
ilmerge /out:Rest.Foundation.dll RestFoundation.dll Newtonsoft.Json.dll /ver:1.2.0.0 /t:library /internalize /targetplatform="v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0" /keyfile:..\..\..\RestFoundation.snk /xmldocs
echo.
echo.
copy Rest.Foundation.* ..\..\..\build\ /y
echo.
pause
