FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
COPY . .
RUN ulimit -n 8192
RUN dotnet restore "buying_order_server.csproj"
RUN dotnet publish --force --runtime linux-x64 --self-contained true --output ./dist

FROM microsoft/aspnetcore
ENTRYPOINT ["dotnet", "buying_order_server.dll"]