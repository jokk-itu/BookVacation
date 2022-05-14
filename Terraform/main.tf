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
  cluster_version = var.cluster_version
}

module "doks-config" {
  source = "./modules/doks-config"
  cluster_name = module.doks.cluster_name
  cluster_id = module.doks.cluster_id
  write_kubeconfig = var.write_kubeconfig
}

module "certificate" {
  source = "./modules/certificate"
  name = "default"
  domain_name = local.domain_name
}

module "ingress-loadbalancer" {
  source = "./modules/ingress-loadbalancer"
  name = "ingress-loadbalancer"
  region = "lon1"
  size = "lb-small"
  certificate_name = module.certificate.name
}

module "record" {
  source = "./modules/record"
  loadbalancer_ip = module.ingress-loadbalancer.ip
  domain_name = local.domain_name
}

module "ingress-controller" {
  source = "./modules/ingress-controller"
  loadbalancer_id = module.ingress-loadbalancer.id
  namespace = module.doks-config.test_namespace
}