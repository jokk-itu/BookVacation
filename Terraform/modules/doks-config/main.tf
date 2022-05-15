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


# 🇰​​​​​🇺​​​​​🇧​​​​​🇪​​​​​🇨​​​​​🇴​​​​​🇳​​​​​🇫​​​​​
resource "local_file" "kubeconfig" {
  depends_on = [var.cluster_id]
  count      = var.write_kubeconfig ? 1 : 0
  content    = var.kubeconfig
  filename   = "./kubeconfig"
}


# 🇳​​​​​🇦​​​​​🇲​​​​​🇪​​​​​🇸​​​​​🇵​​​​​🇦​​​​​🇨​​​​​🇪​​​​​🇸​​​​​
resource "kubernetes_namespace" "test" {
  metadata {
    name = "test"
  }
}

resource "kubernetes_namespace" "production" {
  metadata {
    name = "production"
  }
}