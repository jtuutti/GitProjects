@echo off
echo Last known tag:
git describe --tags --abbrev=0
echo.
set /p tag="Enter a tag name: "
if "%tag%"=="" goto noparam
git push https://github.com/dstarosta/GitProjects :%tag%
git push D:/GitRepo :%tag%
goto complete
:noparam
echo No git tag provided
:complete
echo.
pause
