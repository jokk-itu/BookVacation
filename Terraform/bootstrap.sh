#! /bin/sh

terraform init
terraform validate
terraform apply -var write_kubeconfig=true

kubectl config --kubeconfig
kubectl apply -f "https://github.com/rabbitmq/cluster-operator/releases/latest/download/cluster-operator.yml"
