@echo off

CALL ./stop-containers.bat

CALL docker volume create --name server_db_data -d local
CALL docker-compose -f ./docker-compose-database.yml up