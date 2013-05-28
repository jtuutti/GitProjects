@echo off
if "%1"=="" goto noparam
git push https://github.com/dstarosta/GitProjects :%1
git push D:/GitRepo :%1
goto complete
:noparam
echo No git tag provided
:complete
pause
