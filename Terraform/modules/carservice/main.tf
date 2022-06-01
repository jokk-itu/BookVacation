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
resource "kubernetes_deployment" "carservice" {
  metadata {
    name = "carservice"
    namespace = var.namespace
    labels = {
      app = "carservice"
    }
  }

  lifecycle {
    ignore_changes = [
      spec[0].replicas
    ]
  }

  spec {
    selector {
      match_labels = {
        app = "carservice"
      }
    }

    template {
      metadata {
        labels = {
          app = "carservice"
        }
      }

      spec {
        container {
          image = "jokk/carservice:latest"
          name = "carservice"
          
          port {
            container_port = 80
            name = "carservice"
          }

          resources {
            limits = {
              cpu = "500m"
              memory = "128Mi"
            }
            requests = {
              cpu = "250m"
              memory = "64Mi"
            }
          }

          liveness_probe {
            http_get {
              path = "/health/live"
            }

            initial_delay_seconds = 5
            period_seconds = 15
            success_threshold = 1
            timeout_seconds = 5
          }

          readiness_probe {
            http_get {
              path = "/health/ready"
              port = 80
            }

            initial_delay_seconds = 5
            period_seconds = 5
            success_threshold = 1
            timeout_seconds = 5
          }

          env {
            name = "ASPNETCORE_ENVIRONMENT"
            value = "Production"
          }

          env {
            name = "Node__Name"
            value_from {
              field_ref {
                field_path = "spec.nodeName"
              }
            }
          }

          env {
            name = "Pod__Name"
            value_from {
              field_ref {
                field_path = "status.podIP"
              }
            }
          }

          env {
            name = "Logging__SeqUri"
            value_from {
              config_map_key_ref {
                key = "url"
                name = var.logger-configname
                optional = false
              }
            }
          }

          env {
            name = "Logging__LogToSeq"
            value = true
          }

          env {
            name = "Logging__LogToConsole"
            value = true
          }

          env {
            name = "EventBus__Hostname"
            value_from {
              secret_key_ref {
                key = "host"
                name = var.eventbus-secretname
                optional = false
              }
            }
          }

          env {
            name = "EventBus__Port"
            value_from {
              secret_key_ref {
                key = "port"
                name = var.eventbus-secretname
                optional = false
              }
            }
          }

          env {
            name = "EventBus__Username"
            value_from {
              secret_key_ref {
                key = "username"
                name = var.eventbus-secretname
                optional = false
              }
            }
          }

          env {
            name = "EventBus__Password"
            value_from {
              secret_key_ref {
                key = "password"
                name = var.eventbus-secretname
                optional = false
              }
            }
          }

          env {
            name = "RavenSettings__Urls__0"
            value_from {
              config_map_key_ref {
                key = "url"
                name = var.ravendb-configname
                optional = false
              }
            }
          }
        }
      }
    }
  }
}

# 🇭​​​​​🇵​​​​​🇦​​​​​
resource "kubernetes_horizontal_pod_autoscaler" "carservice" {
  metadata {
    name = "carservice"
    namespace = var.namespace
  }
  depends_on = [kubernetes_deployment.carservice]

  spec {
    min_replicas = 1
    max_replicas = 10

    scale_target_ref {
      kind = "Deployment"
      name = "carservice"
    }

    metric {
      type = "Resource"
      resource {
        name = "CPU"
        target {
          type = "Utilization"
          average_utilization = "75"
        }
      }
    }

    metric {
      type = "Resource"
      resource {
        name = "Memory"
        target {
          type = "Utilization"
          average_utilization = "75"
        }
      }
    }
  }
}

# 🇸​​​​​🇪​​​​​🇷​​​​​🇻​​​​​🇮​​​​​🇨​​​​​🇪​​​​​
resource "kubernetes_service" "carservice" {
  metadata {
    name = "carservice"
    namespace = var.namespace
  }
  depends_on = [kubernetes_deployment.carservice]

  spec {

    selector = {
      app = "carservice"
    }

    session_affinity = "ClientIP"

    port {
      port        = 80
      target_port = "carservice"
    }

    type = "ClusterIP"
  }
}

# 🇮​​​​​🇳​​​​​🇬​​​​​🇷​​​​​🇪​​​​​🇸​​​​​🇸​​​​​
resource "kubernetes_ingress_v1" "carservice" {
  wait_for_load_balancer = true
  depends_on = [kubernetes_service.carservice]
  metadata {
    name = "carservice"
    namespace = var.namespace

    annotations = {
      "nginx.ingress.kubernetes.io/use-regex" = "true"
      "nginx.ingress.kubernetes.io/rewrite-target" = "/$1"
      "cert-manager.io/cluster-issuer" = var.cert-issuername
    }
  }

  spec {
    ingress_class_name = "nginx"

    tls {
      hosts = [var.domain_name]
      secret_name = var.tls-secretname
    }

    rule {
      host = var.domain_name

      http {
        path {
          backend {
            service {
              name = "carservice"
              port {
                number = 80
              }
            }
          }
          path = "/car/(.*)"
        }
      }
    }
  }
}

# 🇨​​​​​🇴​​​​​🇳​​​​​🇫​​​​​🇮​​​​​🇬​​​​​🇲​​​​​🇦​​​​​🇵​​​​​
resource "kubernetes_config_map" "carservice" {
  metadata {
    name = "carservice"
    namespace = var.namespace
  }
  
  data = {
    url = "http://carservice.${var.namespace}.svc:80"
  }
}