@echo off
echo Last known tag name:
git describe --tags --abbrev=0
if %errorlevel% neq 0 goto noparam
echo.
set /p tag="Enter a tag name: "
if "%tag%"=="" goto noparam
git tag -d %tag%
git push https://github.com/dstarosta/GitProjects :%tag%
git push D:/GitRepo :%tag%
goto complete
:noparam
echo No git tag provided
:complete
echo.
pause
