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

## TicketService
To handle tickets for flights, hotels and cars.

## TrackingService
Subscribes to routing slip events and persists them.

## VacationService
To buy a complete vacation with flight ticket, hotel room and car.

## GatewayService
A reverse proxy to route a request to different services.

## Prometheus and Grafana
Masstransit and Api data to illustrate in grafana.

## Seq
Sends all logs to Seq as a sink in Serilog.

## RavenDB
The DBMS for all data. There is one database pr. service.

## RabbitMQ
The broker to handle messages between services.

## TestConsole
Uses the NBomber load testing package.
```
-v, --vacation        Runs the vacation load test
-r, --rentalcar       Runs the rentalcar load test
-h, --hotel           Runs the hotel load test
-f, --flight          Runs the flight load test
-a, --airplane        Runs the airplane load test
```

## How to run system

```
docker-compose build

docker-compose up -d
```
