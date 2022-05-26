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

module "certificate" {
  source = "./modules/certificate"
  name = "default"
  domain_name = local.domain_name
}

module "doks" {
  source = "./modules/doks"
  cluster_name = local.cluster_name
  cluster_region = "lon1"
  cluster_version = var.cluster_version
}

module "doks-config" {
  source = "./modules/doks-config"
  cluster_name = module.doks.name
  cluster_id = module.doks.id
  write_kubeconfig = var.write_kubeconfig
  cluster_host = module.doks.host
  cluster_token = module.doks.token
  cluster_certificate = module.doks.certificate
  kubeconfig = module.doks.kubeconfig
}

module "ingress-controller" {
  source = "./modules/ingress-controller"
  certificate_id = module.certificate.id
  namespace = module.doks-config.test_namespace
  cluster_name = module.doks.name
  cluster_host = module.doks.host
  cluster_token = module.doks.token
  cluster_certificate = module.doks.certificate
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
}

module "minio" {
  source = "./modules/minio"
  cluster_host = module.doks.host
  cluster_token = module.doks.token
  cluster_certificate = module.doks.certificate
}

module "main-record" {
  source = "./modules/record"
  loadbalancer_ip = "get ip from any ingressrule resource"
  domain_name = local.domain_name
}

module "logger-record" {
  source = "./modules/record"
  loadbalancer_ip = "get ip from any ingressrule resource"
  domain_name = "seq.${local.domain_name}"
}
