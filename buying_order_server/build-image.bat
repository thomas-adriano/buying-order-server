@echo off
set /p version="Enter version: "
rm -rdf ./dist
dotnet publish --force --runtime linux-x64 --self-contained true --output ./dist
docker build . --no-cache -t buying-order-server:%version% -t buying-order-server:latest