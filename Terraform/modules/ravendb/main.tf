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
resource "kubernetes_service" "ravendb" {
  metadata {
    name = "ravendb"
    namespace = var.namespace
    labels = {
      app = "ravendb"
    }
  }
  spec {
    selector = {
      app = "ravendb"
    }
    port {
      port        = 8080
      target_port = 8080
    }
    cluster_ip = "None"
  }
}

# ๐จโโโโโ๐ดโโโโโ๐ณโโโโโ๐ซโโโโโ๐ฎโโโโโ๐ฌโโโโโ๐ฒโโโโโ๐ฆโโโโโ๐ตโโโโโ
resource "kubernetes_config_map" "ravendb" {
  metadata {
    name = "ravendb-config"
    namespace = var.namespace
  }
  data = {
    url = "http://ravendb.${var.namespace}.svc:8080"
  }
}

# ๐ธโโโโโ๐นโโโโโ๐ฆโโโโโ๐นโโโโโ๐ชโโโโโ๐ซโโโโโ๐บโโโโโ๐ฑโโโโโ๐ธโโโโโ๐ชโโโโโ๐นโโโโโ
resource "kubernetes_stateful_set" "ravendb" {
  depends_on = [kubernetes_service.ravendb]
  metadata {
    name = "ravendb"
    namespace = var.namespace
    labels = {
      app = "ravendb"
    }
  }

  spec {
    service_name = "ravendb"
    replicas = var.replicas
    pod_management_policy = "OrderedReady"
    selector {
      match_labels = {
        app = "ravendb"
      }
    }
    template {
      metadata {
        labels = {
          app = "ravendb"
        }
      }

      spec {
        container {
          image = "ravendb/ravendb:ubuntu-latest"
          name = "ravendb"
          port {
            container_port = 8080
            name = "ravendb"
          }
          volume_mount {
            name       = "ravendb"
            mount_path = "/opt/RavenDB/Server/RavenData"
          }
          env {
            name = "RAVEN_License_Eula_Accepted"
            value = "true"
          }
          env {
            name = "RAVEN_Setup_Mode"
            value = "None"
          }
          env {
            name = "RAVEN_Security_UnsecuredAccessAllowed"
            value = "PrivateNetwork"
          }
        }
      }
    }
    volume_claim_template {
      metadata {
        name = "ravendb"
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