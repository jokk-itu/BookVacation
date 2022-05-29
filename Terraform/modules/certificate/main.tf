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
    kubectl = {
      source = "gavinbunney/kubectl"
      version = ">= 1.13.0"
    }
    helm = {
      source  = "hashicorp/helm"
      version = ">= 2.0.1"
    }
  }
}

# 🇵​​​​​🇷​​​​​🇴​​​​​🇻​​​​​🇮​​​​​🇩​​​​​🇪​​​​​🇷​​​​​🇸​​​​​
provider "kubernetes" {
  host = var.cluster_host
  token = var.cluster_token
  cluster_ca_certificate = var.cluster_certificate
}
provider "kubectl" {
  host = var.cluster_host
  token = var.cluster_token
  cluster_ca_certificate = var.cluster_certificate
}
provider "helm" {
  kubernetes {
    host = var.cluster_host
  token = var.cluster_token
  cluster_ca_certificate = var.cluster_certificate
  }
}

# Certificate
module "cert_manager" {
  source = "terraform-iaac/cert-manager/kubernetes"
  cluster_issuer_email = "joachim@kelsen.nu"
  certificates = {
    "main" = {
      dns_names = var.dns_names
    }
  }
}