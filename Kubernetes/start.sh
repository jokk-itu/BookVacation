kubectl apply -f RavenDB/raven-config.yml
kubectl apply -f RavenDB/raven-secret.yml
kubectl apply -f RavenDB/raven.yml

kubectl apply -f Logger/logger-config.yml
kubectl apply -f Logger/logger.yml

kubectl apply -f EventBus/eventbus-config.yml
kubectl apply -f EventBus/eventbus-secret.yml
kubectl apply -f EventBus/eventbus.yml

kubectl apply -f CarService/carservice-config.yml
kubectl apply -f CarService/carservice.yml
