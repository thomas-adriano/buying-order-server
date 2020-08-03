@echo off
set /p version="Enter version: "
docker build --no-cache -t buying-order-agent:%version%