# BookVacation

![Architecture](architecture.png "Architecture")


|Build|Dockerhub|Nuget|
|-----|---------|-----|
| [![Build](https://github.com/jokk-itu/BookVacation/actions/workflows/build.yml/badge.svg)](https://github.com/jokk-itu/BookVacation/actions/workflows/build.yml) | [![Publish](https://github.com/jokk-itu/BookVacation/actions/workflows/publish.yml/badge.svg)](https://github.com/jokk-itu/BookVacation/actions/workflows/publish.yml) |[![Push to Github](https://github.com/jokk-itu/BookVacation/actions/workflows/push.yml/badge.svg)](https://github.com/jokk-itu/BookVacation/actions/workflows/push.yml)|


## Choreography

Project to illustrate a Choreography architecture using Routing slips in Masstransit.
It is illustrated by booking a vacation with a Flight, Hotel and Car.

This flow is activated by the api on: http://localhost:5000/api/v1/vacation.

## CarService
To rent cars.

## HotelService
To book a hotel room.

## FlightService
To buy a flight ticket.

## VacationService
To buy a complete vacation with flight ticket, hotel room and car.

## GatewayService
A reverse proxy to route a request to different services.

## TrackingService
Subscribes to routing slip events and logs them.

## Prometheus and Grafana
Masstransit and Api data to illustrate in grafana.

## Seq
Sends all logs to Seq as a sink in Serilog.

## Neo4j
The DBMS for all data. There is currently only one database, which is the limit for the community edition.

## RabbitMQ
The broker to handle messages between services.


## How to run

```
##The env file could also be development, depending on your scenario
docker-compose build --env-file production.env

docker-compose up -d
```
