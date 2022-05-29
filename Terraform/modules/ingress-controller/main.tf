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

# 🇵​​​​​🇷​​​​​🇴​​​​​🇻​​​​​🇮​​​​​🇩​​​​​🇪​​​​​🇷​​​​​🇸​​​​​
provider "kubernetes" {
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

#🇮​​​​​🇳​​​​​🇬​​​​​🇷​​​​​🇪​​​​​🇸​​​​​🇸​​​​​ 🇨​​​​​🇴​​​​​🇳​​​​​🇹​​​​​🇷​​​​​🇴​​​​​🇱​​​​​🇱​​​​​🇪​​​​​🇷​​​​​
resource "helm_release" "nginx_ingress" {
  name       = "nginx-ingress-controller"
  namespace  = var.namespace
  repository = "https://kubernetes.github.io/ingress-nginx"
  chart      = "ingress-nginx"

  set {
    name  = "service.type"
    value = "LoadBalancer"
  }
  set {
    name = "controller.publishService.enabled"
    value = "true"
  }

  # Loadbalancer configuration
  set {
    name  = "service.annotations.service.beta.kubernetes.io/do-loadbalancer-name"
    value = format("%s-nginx-ingress", var.cluster_name)
  }
  set {
    name = "service.beta.kubernetes.io/do-loadbalancer-algorithm"
    value = "round_robin"
  }
  set {
    name = "service.beta.kubernetes.io/do-loadbalancer-redirect-http-to-https"
    value = "true"
  }
}