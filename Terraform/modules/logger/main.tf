terraform {
  required_providers {
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

# 🇸​​​​​🇪​​​​​🇶​​​​​ 🇱​​​​​🇴​​​​​🇬​​​​​🇬​​​​​🇪​​​​​🇷​​​​​
resource "helm_release" "logger" {
  name       = "logger"
  namespace  = var.namespace
  repository = "https://helm.datalust.co"
  chart      = "seq"

  set {
    name = "ingress.annotations.kubernetes\\.io/ingress\\.class"
    value = "nginx"
    type = "string"
  }
  set {
    name = "ui.ingress.enabled"
    value = "true"
  }
  set {
    name = "ui.ingress.path"
    value = "/"
  }
  set {
    name = "ui.ingress.hosts.0"
    value = "seq.${var.domain_name}"
  }
}

# 🇨​​​​​🇴​​​​​🇳​​​​​🇫​​​​​🇮​​​​​🇬​​​​​🇲​​​​​🇦​​​​​🇵​​​​​
resource "kubernetes_config_map" "logger" {
  metadata {
    name = "logger-config"
    namespace = var.namespace
  }
  data = {
    url = "http://logger-seq.${var.namespace}.svc:80"
  }
}