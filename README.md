# BookVacation

![Architecture](architecture.png "Architecture")


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
-a, --vacationload        Runs the vacation load test
-b, --rentalcarload       Runs the rentalcar load test
-c, --hotelload           Runs the hotel load test
-d, --flightload          Runs the flight load test
-e, --airplaneload        Runs the airplane load test
-f, --vacationsingle      Runs the vacation single test
```

## Development
Use docker during development.

```
docker-compose build

docker-compose up -d
```

## Production
Use kubernetes during production.
Set up services by running the <b>start.sh</b> script in the Kubernetes directory.
