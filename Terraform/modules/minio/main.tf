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

# ๐ธโโโโโ๐ชโโโโโ๐ทโโโโโ๐ปโโโโโ๐ฎโโโโโ๐จโโโโโ๐ชโโโโโ
resource "kubernetes_service" "minio" {
  metadata {
    name = "minio"
    namespace = var.namespace
    labels = {
      app = "minio"
    }
  }
  spec {
    selector = {
      app = "minio"
    }
    port {
      port        = 9001
      target_port = 9001
    }
    port {
      port        = 9000
      target_port = 9000
    }
    cluster_ip = "None"
  }
}

# ๐จโโโโโ๐ดโโโโโ๐ณโโโโโ๐ซโโโโโ๐ฎโโโโโ๐ฌโโโโโ๐ฒโโโโโ๐ฆโโโโโ๐ตโโโโโ
resource "kubernetes_config_map" "minio" {
  metadata {
    name = "minio-config"
    namespace = var.namespace
  }
  data = {
    url = "http://minio.${var.namespace}.svc:9000"
  }
}

# ๐ธโโโโโ๐นโโโโโ๐ฆโโโโโ๐นโโโโโ๐ชโโโโโ๐ซโโโโโ๐บโโโโโ๐ฑโโโโโ๐ธโโโโโ๐ชโโโโโ๐นโโโโโ
resource "kubernetes_stateful_set" "minio" {
  depends_on = [kubernetes_service.minio]
  metadata {
    name = "minio"
    namespace = var.namespace
    labels = {
      app = "minio"
    }
  }

  spec {
    service_name = "minio"
    replicas = var.replicas
    pod_management_policy = "OrderedReady"
    selector {
      match_labels = {
        app = "minio"
      }
    }
    template {
      metadata {
        labels = {
          app = "minio"
        }
      }

      spec {
        container {
          image = "minio/minio"
          name = "minio"
          command = ["server"]
          args = ["/data", "--console-address", ":9001"]
          port {
            container_port = 9001
            name = "ui"
          }
          port {
            container_port = 9000
            name = "minio"
          }
          volume_mount {
            name       = "minio"
            mount_path = "/data"
          }
          env {
            name = "MINIO_ROOT_USER"
            value = "access"
          }
          env {
            name = "MINIO_ROOT_PASSWORD"
            value = "secret1234"
          }
        }
      }
    }
    volume_claim_template {
      metadata {
        name = "minio"
      }

      spec {
        access_modes = ["ReadWriteOnce"]
        storage_class_name = "do-block-storage"

        resources {
          requests = {
            storage = "10Gi"
          }
        }
      }
    }
  }
}

# ๐ธโโโโโ๐ชโโโโโ๐จโโโโโ๐ทโโโโโ๐ชโโโโโ๐นโโโโโ
resource "kubernetes_secret" "minio" {
  metadata {
    name = "minio"
  }

  data = {
    username = "access"
    password = "secret1234"
  }
}