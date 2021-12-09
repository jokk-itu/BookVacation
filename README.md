# BookVacation

[![Build, Test, Publish](https://github.com/jokk-itu/BookVacation/actions/workflows/ci.yml/badge.svg)](https://github.com/jokk-itu/BookVacation/actions/workflows/ci.yml)

## Choreography

Project to illustrate a Choreography architecture using Routing slips in Masstransit.
It is illustrated by booking a vacation with a Flight, Hotel and Car.

This flow is activated by the api on: http://localhost:20001/api/v1/vacation.


## Orchestrator

The project also illustrates an Orchestrator architecture.
By using a Saga Statemachine in Masstransit.
There are three flows which can be triggered.


### Complete Flight Booking

api: http://localhost:20001/api/v1/flight/complete


### Cancel Flight Booking

api: http://localhost:20001/api/v1/flight/cancel


### Expire Flight Booking

api: http://localhost:20001/api/v1/flight/expire
