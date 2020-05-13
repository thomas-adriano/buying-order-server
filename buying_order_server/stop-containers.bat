@echo off

CALL docker-compose -f ./docker-compose-database.yml down --remove-orphans
CALL docker system prune --volumes --force