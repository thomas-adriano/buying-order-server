@echo off
set /p version="Enter version: "
dotnet publish --force --runtime linux-x64 --self-contained true --output ./dist
docker build --no-cache -t buying-order-agent:%version%