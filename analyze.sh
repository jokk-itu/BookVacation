#! /bin/sh
dotnet sonarscanner begin /k:"BookVacation" /d:sonar.host.url="http://localhost:9100"  /d:sonar.login="bed7ae7da24e90566ca1700ee533efeda61ecf2d"
dotnet build
dotnet sonarscanner end /d:sonar.login="bed7ae7da24e90566ca1700ee533efeda61ecf2d"
