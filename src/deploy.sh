#!/bin/bash

dotnet publish MockSite.Web/MockSite.Web.csproj -c Release -o out
cp -R MockSite.Web/out/ClientApp/build WebProxy/www
docker-compose build
docker-compose up -d
docker-compose ps 