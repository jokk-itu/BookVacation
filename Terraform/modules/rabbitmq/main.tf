terraform {
  required_providers {
    kubernetes = {
      source = "hashicorp/kubernetes"
      version = ">= 2.0.0"
    }
  }
}

# ๐ตโโโโโ๐ทโโโโโ๐ดโโโโโ๐ปโโโโโ๐ฎโโโโโ๐ฉโโโโโ๐ชโโโโโ๐ทโโโโโ๐ธโโโโโ
provider "kubernetes" {
  host = var.cluster_host
  token = var.cluster_token
  cluster_ca_certificate = var.cluster_certificate
}

# ๐ทโโโโโ๐ฆโโโโโ๐งโโโโโ๐งโโโโโ๐ฎโโโโโ๐นโโโโโ๐ฒโโโโโ๐ถโโโโโ๐จโโโโโ๐ฑโโโโโ๐บโโโโโ๐ธโโโโโ๐นโโโโโ๐ชโโโโโ๐ทโโโโโ
resource "kubernetes_manifest" "rabbitmq" {
    manifest = {
        apiVersion = "rabbitmq.com/v1beta1"
        kind = "RabbitmqCluster"
        metadata = {
            name = "eventbus"
            namespace = var.namespace
        }
    }
}