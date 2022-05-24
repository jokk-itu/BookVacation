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

# 🇸​​​​​🇪​​​​​🇷​​​​​🇻​​​​​🇮​​​​​🇨​​​​​🇪​​​​​
resource "kubernetes_service" "ravendb" {
  metadata {
    name = "ravendb"
    namespace = var.namespace
  }
  spec {
    port {
      port        = 8080
      target_port = 8080
    }
    type = "ClusterIP"
  }
}

# 🇨​​​​​🇴​​​​​🇳​​​​​🇫​​​​​🇮​​​​​🇬​​​​​🇲​​​​​🇦​​​​​🇵​​​​​
resource "kubernetes_config_map" "ravendb" {
  metadata {
    name = "ravendb-config"
  }
  data = {
    url = "http://ravendb.${var.namespace}.svc.cluster.local"
  }
}

# 🇸​​​​​🇹​​​​​🇦​​​​​🇹​​​​​🇪​​​​​🇫​​​​​🇺​​​​​🇱​​​​​🇸​​​​​🇪​​​​​🇹​​​​​
resource "kubernetes_stateful_set" "ravendb" {
  provisioner "local-exec" {
    command = <<-EOT
      kubectl config --kubeconfig "./kubeconfig" cp join_cluster.sh var.namespace/ravendb-0:/
      kubectl config --kubeconfig "./kubeconfig" exec ravendb-0 -- /bin/sh -c /join_cluster.sh var.replicas var.namespace
    EOT

    interpreter = ["/bin/sh", "-c"]
  }
  depends_on = [kubernetes_service.ravendb]
  metadata {
    name = "ravendb"
    namespace = var.namespace
    labels = {
      app = "ravendb"
    }
  }

  spec {
    service_name = "ravendb_service"
    replicas = var.replicas
    update_strategy {
      type = "RollingUpdate"
      rolling_update {
        partition = 1
      }
    }
    pod_management_policy = "OrderedReady"
    template {
      metadata {
        labels = {
          app = "ravendb"
        }
      }

      spec {
        container {
          image = "ravendb/ravendb:ubuntu-latest"
          imagePullPolicy = "Always"
          name = "ravendb"
          port {
            container_port = 8080
            name = "ravendb"
          }
          volume_mount {
            name       = "data"
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
          resources {
            limits = {
              cpu    = "10m"
              memory = "10Mi"
            }
            requests = {
              cpu    = "10m"
              memory = "10Mi"
            }
          }
        }
      }
    }
    volume_claim_template {
      metadata {
        name = "data"
      }

      spec {
        access_modes       = ["ReadWriteOnce"]
        storage_class_name = "standard"

        resources {
          requests = {
            storage = "10Gi"
          }
        }
      }
    }
  }
}