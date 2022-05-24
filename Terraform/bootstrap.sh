#! /bin/sh

terraform init
terraform validate

# Baseline
terraform apply \
  -target "certificate" \
  -target "doks" \
  -target "doks-config" \
  -target "ingress-controller"

# Services
terraform apply \
  -target "rabbitmq" \
  -target "ravendb" \
  -target "seq"

echo "Done!"