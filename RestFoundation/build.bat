@echo off
del .\RestFoundation\bin\Release\*.* /f /q
del .\RestFoundation\obj\Release\*.* /f /q
mkdir .\build > nul
del .\build\Rest.Foundation.* /f /q > nul

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild RestFoundation\RestFoundation.csproj /t:Rebuild /p:Configuration=Release
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild RestFoundation.Tests\RestFoundation.Tests.csproj /t:Rebuild /p:Configuration=Release

echo.
echo Running Unit Tests
"C:\Program Files (x86)\NUnit 2.5.8\bin\net-2.0\nunit-console" D:\Development\GitProjects\RestFoundation\RestFoundation.Tests\bin\Release\RestFoundation.Tests.dll /labels

if %errorlevel% neq 0 goto error
if %errorlevel% equ 0 goto merge

:error
echo Error # %errorlevel% !!! Unit Tests Failed !!!
goto end

:merge
echo.
echo Performing IL Merge
cd .\RestFoundation\bin\Release
ilmerge /out:Rest.Foundation.dll RestFoundation.dll Newtonsoft.Json.dll /ver:2.3.0.0 /t:library /internalize /targetplatform="v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5" /keyfile:..\..\..\RestFoundation.snk /xmldocs
cd ..\..\..
echo.
copy .\RestFoundation\bin\Release\Rest.Foundation.* .\build\ /y

:end
if %errorlevel% neq 0 echo ##teamcity[buildStatus status='FAILURE' text='{build.status.text} in build']
echo.
if "%1"=="" pause

:exit
