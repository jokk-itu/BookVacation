terraform {
  required_providers {
    kubernetes = {
      source = "hashicorp/kubernetes"
      version = ">= 2.0.0"
    }
  }
}

# 🇵​​​​​🇷​​​​​🇴​​​​​🇻​​​​​🇮​​​​​🇩​​​​​🇪​​​​​🇷​​​​​🇸​​​​​
provider "kubernetes" {
  host = var.cluster_host
  token = var.cluster_token
  cluster_ca_certificate = var.cluster_certificate
}

# 🇩​​​​​🇪​​​​​🇵​​​​​🇱​​​​​🇴​​​​​🇾​​​​​🇲​​​​​🇪​​​​​🇳​​​​​🇹​​​​​
resource "kubernetes_deployment" "ravendb" {
  metadata {
    name = "ravendb"
    namespace = "test"
    labels = {
      app = "ravendb"
    }
  }

  spec {
    replicas = 1
            
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
          resources {
          limits = {
            cpu    = "0.5"
            memory = "1024Mi"
          }
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
        port {
          container_port = 8080
          name = "ravendb-pod-port"
        }
      }
    }
  }
}

# 🇸​​​​​🇪​​​​​🇷​​​​​🇻​​​​​🇮​​​​​🇨​​​​​🇪​​​​​
resource "kubernetes_service" "ravendb" {
  metadata {
    name = "ravendb-service"
  }
  spec {
    selector {
      app = "${kubernetes_deployment.ravendb.metadata.0.labels.app}"
    }
    port {
      name = "ravendb-service-port"
      port = 8080
      target_port = "${kubernetes_deployment.ravendb.spec.template.spec.port.name}"
    }
  }
}

# 🇨​​​​​🇴​​​​​🇳​​​​​🇫​​​​​🇮​​​​​🇬​​​​​🇲​​​​​🇦​​​​​🇵​​​​​
resource "kubernetes_config_map" "ravendb" {
  metadata {
    name = "ravendb-config"
  }
  data = {
    url = "http://${kubernetes_service.ravendb.spec.port.name}:${kubernetes_service.ravendb.spec.port.port}"
  }
}