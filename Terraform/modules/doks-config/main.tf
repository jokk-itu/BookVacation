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


# 🇰​​​​​🇺​​​​​🇧​​​​​🇪​​​​​🇨​​​​​🇴​​​​​🇳​​​​​🇫​​​​​
resource "local_file" "kubeconfig" {
  depends_on = [var.cluster_id]
  content    = var.kubeconfig
  filename   = "./kubeconfig"

  provisioner "local-exec" {
    command = <<-EOT
      doctl kubernetes cluster kubeconfig save $CLUSTERID
      kubectl apply -f 'https://github.com/rabbitmq/cluster-operator/releases/latest/download/cluster-operator.yml'
    EOT
      
    interpreter = ["/bin/sh", "-c"]
    environment = {
      CLUSTERID = var.cluster_id
    }
  }
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

# 🇲​​​​​🇪​​​​​🇹​​​​​🇷​​​​​🇮​​​​​🇨​​​​​🇸​​​​​ 🇸​​​​​🇪​​​​​🇷​​​​​🇻​​​​​🇪​​​​​🇷​​​​​
resource "helm_release" "metric-server" {
  name       = "metric-server"
  namespace  = var.namespace
  repository = "https://kubernetes-sigs.github.io/metrics-server/"
  chart      = "metrics-server"
}