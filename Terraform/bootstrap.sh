#! /bin/sh

terraform init
terraform validate
terraform apply -var write_kubeconfig=true

echo "Done!"