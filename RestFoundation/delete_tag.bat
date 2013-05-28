@echo off
set /p tag="Enter a tag name: "
if "%tag%"=="" goto noparam
git push https://github.com/dstarosta/GitProjects :%tag%
git push D:/GitRepo :%tag%
goto complete
:noparam
echo No git tag provided
:complete
pause
