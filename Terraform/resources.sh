#! /bin/sh

terraform init
terraform validate

# Resources
terraform apply \
  -target module.rabbitmq \
  -target module.ravendb \
  -target module.logger \
  -target module.minio \
  -target module.carservice

echo "Done!"