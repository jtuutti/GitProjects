@echo off
del .\RestFoundation\bin\Debug\*.* /f /q
del .\RestFoundation\obj\Debug\*.* /f /q
mkdir .\build > nul
del .\build\Rest.Foundation.* /f /q > nul

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild RestFoundation\RestFoundation.csproj /t:Rebuild /p:Configuration=Debug
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild RestFoundation.Tests\RestFoundation.Tests.csproj /t:Rebuild /p:Configuration=Debug

echo.
echo Performing IL Merge
cd .\RestFoundation\bin\Debug
ilmerge /out:Rest.Foundation.dll RestFoundation.dll Newtonsoft.Json.dll Linq2Rest.dll /ver:2.6.0.0 /t:library /internalize /targetplatform="v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5" /keyfile:..\..\..\RestFoundation.snk /xmldocs
cd ..\..\..
echo.
copy .\RestFoundation\bin\Debug\Rest.Foundation.* .\build\ /y

:end
if %errorlevel% neq 0 echo ##teamcity[buildStatus status='FAILURE' text='{build.status.text} in build']
echo.
if "%1"=="" pause

:exit
