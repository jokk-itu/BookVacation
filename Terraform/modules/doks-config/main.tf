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

# ๐ตโโโโโ๐ทโโโโโ๐ดโโโโโ๐ปโโโโโ๐ฎโโโโโ๐ฉโโโโโ๐ชโโโโโ๐ทโโโโโ๐ธโโโโโ
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

# ๐ฐโโโโโ๐บโโโโโ๐งโโโโโ๐ชโโโโโ๐จโโโโโ๐ดโโโโโ๐ณโโโโโ๐ซโโโโโ
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

# ๐ณโโโโโ๐ฆโโโโโ๐ฒโโโโโ๐ชโโโโโ๐ธโโโโโ๐ตโโโโโ๐ฆโโโโโ๐จโโโโโ๐ชโโโโโ๐ธโโโโโ
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

# ๐ฒโโโโโ๐ชโโโโโ๐นโโโโโ๐ทโโโโโ๐ฎโโโโโ๐จโโโโโ๐ธโโโโโ ๐ธโโโโโ๐ชโโโโโ๐ทโโโโโ๐ปโโโโโ๐ชโโโโโ๐ทโโโโโ
resource "helm_release" "metric-server" {
  name       = "metric-server"
  namespace  = "test"
  repository = "https://kubernetes-sigs.github.io/metrics-server/"
  chart      = "metrics-server"
}