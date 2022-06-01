terraform {
  required_providers {
    digitalocean = {
      source  = "digitalocean/digitalocean"
      version = ">= 2.4.0"
    }
    kubernetes = {
      source = "hashicorp/kubernetes"
      version = ">= 2.0.0"
    }
    helm = {
      source  = "hashicorp/helm"
      version = ">= 2.0.1"
    }
  }
}

resource "random_id" "cluster_name" {
  byte_length = 5
}

locals {
  cluster_name = "tf-k8s-${random_id.cluster_name.hex}"
  domain_name = "occupancydetection.software"
}

module "doks" {
  source = "./modules/doks"
  cluster_name = local.cluster_name
  cluster_region = "lon1"
  cluster_version = "1.22"
}

module "doks-config" {
  source = "./modules/doks-config"
  cluster_name = module.doks.name
  cluster_id = module.doks.id
  write_kubeconfig = true
  cluster_host = module.doks.host
  cluster_token = module.doks.token
  cluster_certificate = module.doks.certificate
  kubeconfig = module.doks.kubeconfig
}

module "ingress-controller" {
  source = "./modules/ingress-controller"
  namespace = module.doks-config.test_namespace
  cluster_name = module.doks.name
  cluster_host = module.doks.host
  cluster_token = module.doks.token
  cluster_certificate = module.doks.certificate
}

module "certificate" {
  source = "./modules/certificate"
  cluster_host = module.doks.host
  cluster_token = module.doks.token
  cluster_certificate = module.doks.certificate
  dns_names = [
    "seq.${local.domain_name}",
    local.domain_name
  ]
}

module "rabbitmq" {
  source = "./modules/rabbitmq"
  cluster_host = module.doks.host
  cluster_token = module.doks.token
  cluster_certificate = module.doks.certificate
  namespace = module.doks-config.test_namespace
}

module "ravendb" {
  source = "./modules/ravendb"
  cluster_host = module.doks.host
  cluster_token = module.doks.token
  cluster_certificate = module.doks.certificate
  namespace = module.doks-config.test_namespace
}

module "logger" {
  source = "./modules/logger"
  cluster_host = module.doks.host
  cluster_token = module.doks.token
  cluster_certificate = module.doks.certificate
  namespace = module.doks-config.test_namespace
  domain_name = local.domain_name
  tls-secretname = module.certificate.private-key-ref
}

module "minio" {
  source = "./modules/minio"
  cluster_host = module.doks.host
  cluster_token = module.doks.token
  cluster_certificate = module.doks.certificate
  namespace = module.doks-config.test_namespace
  replicas = 1
}

module "carservice" {
  source = "./modules/carservice"
  cluster_host = module.doks.host
  cluster_token = module.doks.token
  cluster_certificate = module.doks.certificate
  namespace = module.doks-config.test_namespace
  domain_name = local.domain_name
  tls-secretname = module.certificate.private-key-ref
  logger-configname = module.logger.configname
  eventbus-secretname = "eventbus-default-user"
  ravendb-configname = module.ravendb.configname
  cert-issuername = module.certificate.issuername
}

module "hotelservice" {
  source = "./modules/hotelservice"
  cluster_host = module.doks.host
  cluster_token = module.doks.token
  cluster_certificate = module.doks.certificate
  namespace = module.doks-config.test_namespace
  domain_name = local.domain_name
  tls-secretname = module.certificate.private-key-ref
  logger-configname = module.logger.configname
  eventbus-secretname = "eventbus-default-user"
  ravendb-configname = module.ravendb.configname
  cert-issuername = module.certificate.issuername
}

module "flightservice" {
  source = "./modules/flightservice"
  cluster_host = module.doks.host
  cluster_token = module.doks.token
  cluster_certificate = module.doks.certificate
  namespace = module.doks-config.test_namespace
  domain_name = local.domain_name
  tls-secretname = module.certificate.private-key-ref
  logger-configname = module.logger.configname
  eventbus-secretname = "eventbus-default-user"
  ravendb-configname = module.ravendb.configname
  cert-issuername = module.certificate.issuername
}

module "trackingservice" {
  source = "./modules/trackingservice"
  cluster_host = module.doks.host
  cluster_token = module.doks.token
  cluster_certificate = module.doks.certificate
  namespace = module.doks-config.test_namespace
  domain_name = local.domain_name
  tls-secretname = module.certificate.private-key-ref
  logger-configname = module.logger.configname
  eventbus-secretname = "eventbus-default-user"
  ravendb-configname = module.ravendb.configname
  cert-issuername = module.certificate.issuername
}

module "ticketservice" {
  source = "./modules/ticketservice"
  cluster_host = module.doks.host
  cluster_token = module.doks.token
  cluster_certificate = module.doks.certificate
  namespace = module.doks-config.test_namespace
  domain_name = local.domain_name
  tls-secretname = module.certificate.private-key-ref
  logger-configname = module.logger.configname
  eventbus-secretname = "eventbus-default-user"
  minio-secretname = module.minio.secretname
  minio-configname = module.minio.configname
  cert-issuername = module.certificate.issuername
}

module "vacationservice" {
  source = "./modules/vacationservice"
  cluster_host = module.doks.host
  cluster_token = module.doks.token
  cluster_certificate = module.doks.certificate
  namespace = module.doks-config.test_namespace
  domain_name = local.domain_name
  tls-secretname = module.certificate.private-key-ref
  logger-configname = module.logger.configname
  eventbus-secretname = "eventbus-default-user"
  cert-issuername = module.certificate.issuername
}

module "main-record" {
  source = "./modules/record"
  loadbalancer_ip = module.carservice.ingress-ip
  domain_name = local.domain_name
}

module "logger-record" {
  source = "./modules/record"
  loadbalancer_ip = module.carservice.ingress-ip
  domain_name = "seq.${local.domain_name}"
}