#! /bin/sh

terraform init
terraform validate

# Resources
terraform apply \
  -target module.rabbitmq \
  -target module.ravendb \
  -target module.logger \
  -target module.minio \
  -target module.carservice \
  -target module.hotelservice \
  -target module.main-record \
  -target module.logger-record

echo "Done!"