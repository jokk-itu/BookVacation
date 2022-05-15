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
  set {
    name = "service.beta.kubernetes.io/do-loadbalancer-certificate-id"
    value = var.certificate_id
  }
  set {
    name = "service.annotations.service.beta.kubernetes.io/do-loadbalancer-protocol"
    value = "https"
  }

  # Proxy rule
  set {
    name = "service.ports.0.name"
    value = "https"
  }
  set {
    name = "service.ports.0.protocol"
    value = "TCP"
  }
  set {
    name = "service.ports.0.port"
    value = "443"
  }
  set {
    name = "service.ports.0.targetPort"
    value = "80"
  }
}