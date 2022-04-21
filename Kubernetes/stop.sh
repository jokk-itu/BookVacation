#! /bin/sh

kubectl delete -f RavenDB/raven-config.yml
kubectl delete -f RavenDB/raven-secret.yml
kubectl delete -f RavenDB/raven.yml

kubectl delete -f Logger/logger-config.yml
kubectl delete -f Logger/logger.yml

kubectl delete -f EventBus/eventbus-config.yml
kubectl delete -f EventBus/eventbus-secret.yml
kubectl delete -f EventBus/eventbus.yml

kubectl delete -f Blobstorage/blobstorage-config.yml
kubectl delete -f Blobstorage/blobstorage-secret.yml
kubectl delete -f Blobstorage/blobstorage.yml

kubectl delete -f VacationService/vacationservice-config.yml
kubectl delete -f FlightService/flightservice-config.yml
kubectl delete -f HotelService/hotelservice-config.yml
kubectl delete -f CarService/carservice-config.yml
kubectl delete -f TrackingService/trackingservice-config.yml
kubectl delete -f TicketService/ticketservice-config.yml

kubectl delete -f VacationService/vacationservice.yml
kubectl delete -f FlightService/flightservice.yml
kubectl delete -f HotelService/hotelservice.yml
kubectl delete -f CarService/carservice.yml
kubectl delete -f TrackingService/trackingservice.yml
kubectl delete -f TicketService/ticketservice.yml

kubectl delete -f Blobstorage/blobstorage-ingress.yml
kubectl delete -f Logger/logger-ingress.yml
kubectl delete -f EventBus/eventbus-ingress.yml
kubectl delete -f RavenDB/raven-ingress.yml