kubectl apply -f RavenDB/raven-config.yml
kubectl apply -f RavenDB/raven-secret.yml
kubectl apply -f RavenDB/raven.yml

kubectl apply -f Logger/logger-config.yml
kubectl apply -f Logger/logger.yml

kubectl apply -f EventBus/eventbus-config.yml
kubectl apply -f EventBus/eventbus-secret.yml
kubectl apply -f EventBus/eventbus.yml

kubectl apply -f Blobstore/blobstorage-config.yml
kubectl apply -f Blobstore/blobstorage-secret.yml
kubectl apply -f Blobstore/blobstorage.yml

kubectl apply -f VacationService/vacationservice-config.yml
kubectl apply -f FlightService/flightservice-config.yml
kubectl apply -f HotelService/hotelservice-config.yml
kubectl apply -f CarService/carservice-config.yml
kubectl apply -f TrackingService/trackingservice-config.yml
kubectl apply -f TicketService/ticketservice-config.yml

kubectl apply -f VacationService/vacationservice.yml
kubectl apply -f FlightService/flightservice.yml
kubectl apply -f HotelService/hotelservice.yml
kubectl apply -f CarService/carservice.yml
kubectl apply -f TrackingService/trackingservice.yml
kubectl apply -f TicketService/ticketservice.yml