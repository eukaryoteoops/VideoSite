@echo off
set /p project="Enter Project: "
set /a port=0
if %project% equ api ( set /a port=9000 )
if %project% equ bo ( set /a port=9001 )
if %project% equ bom ( set /a port=9002 )
if %project% equ sche ( set /a port=9003 )
docker build . --force-rm -f Dockerfile.%project% -t phoenixnet.azurecr.io/comic_%project%:latest && docker push phoenixnet.azurecr.io/comic_%project%:latest
ssh -t root@104.155.200.99 -i C:\Users\User\.ssh\tony docker rm -f %project%; docker rmi $(docker images --filter 'dangling=true' -q --no-trunc); docker pull phoenixnet.azurecr.io/comic_%project%:latest; docker run -itd --restart on-failure --name %project% -p %port%:80 phoenixnet.azurecr.io/comic_%project%:latest;
pause