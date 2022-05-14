terraform {
  required_providers {
    digitalocean = {
      source  = "digitalocean/digitalocean"
      version = ">= 2.4.0"
    }
    helm = {
        source  = "hashicorp/helm"
        version = ">= 2.0.1"
    }
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
    name = "service.annotations.kubernetes\\.digitalocean\\.com/load-balancer-id"
    value = var.loadbalancer_id
  }
}