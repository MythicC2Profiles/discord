#!/bin/bash

cd /Mythic/c2_code/src/discord
dotnet restore
dotnet publish -c Release -o /Mythic/c2_code
mv /Mythic/c2_code/discord /Mythic/c2_code/server

cd /Mythic/mythic

export PYTHONPATH=/Mythic:/Mythic/mythic

python3.8 mythic_service.py