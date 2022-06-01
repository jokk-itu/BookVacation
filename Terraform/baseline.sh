#! /bin/sh

terraform init
terraform validate

# Baseline
terraform apply \
  -target module.doks \
  -target module.doks-config \
  -target module.ingress-controller \
  -target module.certificate

echo "Done!"