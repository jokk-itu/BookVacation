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

data "digitalocean_kubernetes_cluster" "bookvacation" {
  name = var.cluster_name
}


# 🇰​​​​​🇺​​​​​🇧​​​​​🇪​​​​​🇨​​​​​🇴​​​​​🇳​​​​​🇫​​​​​
resource "local_file" "kubeconfig" {
  depends_on = [var.cluster_id]
  count      = var.write_kubeconfig ? 1 : 0
  content    = data.digitalocean_kubernetes_cluster.bookvacation.kube_config[0].raw_config
  filename   = "${path.root}/kubeconfig"
}


# 🇵​​​​​🇷​​​​​🇴​​​​​🇻​​​​​🇮​​​​​🇩​​​​​🇪​​​​​🇷​​​​​🇸​​​​​
provider "kubernetes" {
  host = data.digitalocean_kubernetes_cluster.bookvacation.endpoint
  token = data.digitalocean_kubernetes_cluster.bookvacation.kube_config[0].token
  cluster_ca_certificate = base64decode(
    data.digitalocean_kubernetes_cluster.bookvacation.kube_config[0].cluster_ca_certificate
  )
}

provider "helm" {
  kubernetes {
    host  = data.digitalocean_kubernetes_cluster.bookvacation.endpoint
    token = data.digitalocean_kubernetes_cluster.bookvacation.kube_config[0].token
    cluster_ca_certificate = base64decode(
      data.digitalocean_kubernetes_cluster.bookvacation.kube_config[0].cluster_ca_certificate
    )
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

# 🇩​​​​​🇴​​​​​🇲​​​​​🇦​​​​​🇮​​​​​🇳​​​​​
resource "digitalocean_domain" "default" {
  name = "occupancydetection.software"
}

# 🇷​​​​​🇪​​​​​🇨​​​​​🇴​​​​​🇷​​​​​🇩​​​​​
resource "digitalocean_record" "static" {
  domain = digitalocean_domain.default.name
  type = "A"
  name = "static"
  value = "ip address of ingress"
}

# 🇨​​​​​🇪​​​​​🇷​​​​​🇹​​​​​🇮​​​​​🇫​​​​​🇮​​​​​🇨​​​​​🇦​​​​​🇹​​​​​🇪​​​​​
resource "digitalocean_certificate" "cert" {
  name = "default"
  type = "lets_encrypt"
  domains = [digitalocean_domain.default.name]

  lifecycle {
    create_before_destroy = true
  }
}

#🇮​​​​​🇳​​​​​🇬​​​​​🇷​​​​​🇪​​​​​🇸​​​​​🇸​​​​​ 🇨​​​​​🇴​​​​​🇳​​​​​🇹​​​​​🇷​​​​​🇴​​​​​🇱​​​​​🇱​​​​​🇪​​​​​🇷​​​​​
resource "helm_release" "nginx_ingress" {
  name       = "nginx-ingress-controller"
  namespace  = kubernetes_namespace.test.metadata.0.name

  repository = "https://kubernetes.github.io/ingress-nginx"
  chart      = "nginx-ingress-controller"

  set {
    name  = "service.type"
    value = "LoadBalancer"
  }
  set {
    name = "controller.publishService.enabled"
    value = true
  }
  set {
    name  = "service.annotations.service.beta.kubernetes.io/do-loadbalancer-name"
    value = format("%s-nginx-ingress", var.cluster_name)
  }

  set {
    name = "service.annotations.service.beta.kubernetes.io/do-loadbalancer-algorithm"
    value = "round_robin"
  }
  set {
    name = "service.annotations.service.beta.kubernetes.io/do-loadbalancer-protocol"
    value = "https"
  }
  set {
    name = "service.annotations.service.beta.kubernetes.io/do-loadbalancer-redirect-http-to-https"
    value = "true"
  }
  set {
    name = "service.annotations.service.beta.kubernetes.io/do-loadbalancer-certificate-id"
    value = digitalocean_certificate.cert.id
  }

  set {
    name = "service.ports.0.name"
    value = "https"
  }
  set {
    name = "service.ports.0.protocol"
    value = "TCP"
  }
  set {
    name = "service.ports.0.port"
    value = "443"
  }
  set {
    name = "service.ports.0.targetPort"
    value = "80"
  }
}