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

# 🇮​​​​​🇳​​​​​🇸​​​​​🇹​​​​​🇦​​​​​🇱​​​​​🇱​​​​​ 🇴​​​​​🇵​​​​​🇪​​​​​🇷​​​​​🇦​​​​​🇹​​​​​🇴​​​​​🇷​​​​​
resource "null_resource" "rabbitmq" {
    provisioner "local-exec" {
        command = <<-EOT
            kubectl config --kubeconfig "./kubeconfig" apply -f "https://github.com/rabbitmq/cluster-operator/releases/latest/download/cluster-operator.yml"
        EOT

        interpreter = ["/bin/sh", "-c"]
    }
}

# 🇷​​​​​🇦​​​​​🇧​​​​​🇧​​​​​🇮​​​​​🇹​​​​​🇲​​​​​🇶​​​​​🇨​​​​​🇱​​​​​🇺​​​​​🇸​​​​​🇹​​​​​🇪​​​​​🇷​​​​​
resource "kubernetes_manifest" "rabbitmq" {
    manifest = {
        apiVersion = "rabbitmq.com/v1beta1"
        kind = "RabbitmqCluster"
        metadata = {
            name = "eventbus"
        }
    }
}