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
resource "kubernetes_deployment" "vacationservice" {
  metadata {
    name = "vacationservice"
    namespace = var.namespace
    labels = {
      app = "vacationservice"
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
        app = "vacationservice"
      }
    }

    template {
      metadata {
        labels = {
          app = "vacationservice"
        }
      }

      spec {
        container {
          image = "jokk/vacationservice:latest"
          name = "vacationservice"
          
          port {
            container_port = 80
            name = "vacationservice"
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
        }
      }
    }
  }
}

# 🇭​​​​​🇵​​​​​🇦​​​​​
resource "kubernetes_horizontal_pod_autoscaler" "vacationservice" {
  metadata {
    name = "vacationservice"
    namespace = var.namespace
  }
  depends_on = [kubernetes_deployment.vacationservice]

  spec {
    min_replicas = 1
    max_replicas = 10

    scale_target_ref {
      kind = "Deployment"
      name = "vacationservice"
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
resource "kubernetes_service" "vacationservice" {
  metadata {
    name = "vacationservice"
    namespace = var.namespace
  }
  depends_on = [kubernetes_deployment.vacationservice]

  spec {

    selector = {
      app = "vacationservice"
    }

    session_affinity = "ClientIP"

    port {
      port        = 80
      target_port = "vacationservice"
    }

    type = "ClusterIP"
  }
}

# 🇮​​​​​🇳​​​​​🇬​​​​​🇷​​​​​🇪​​​​​🇸​​​​​🇸​​​​​
resource "kubernetes_ingress_v1" "vacationservice" {
  wait_for_load_balancer = true
  depends_on = [kubernetes_service.vacationservice]
  metadata {
    name = "vacationservice"
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
              name = "vacationservice"
              port {
                number = 80
              }
            }
          }
          path = "/vacation/(.*)"
        }
      }
    }
  }
}

# 🇨​​​​​🇴​​​​​🇳​​​​​🇫​​​​​🇮​​​​​🇬​​​​​🇲​​​​​🇦​​​​​🇵​​​​​
resource "kubernetes_config_map" "vacationservice" {
  metadata {
    name = "vacationservice"
    namespace = var.namespace
  }
  
  data = {
    url = "http://vacationservice.${var.namespace}.svc:80"
  }
}