terraform {
  required_providers {
    digitalocean = {
      source  = "digitalocean/digitalocean"
      version = ">= 2.4.0"
    }
  }
}

# 🇨​​​​​🇪​​​​​🇷​​​​​🇹​​​​​🇮​​​​​🇫​​​​​🇮​​​​​🇨​​​​​🇦​​​​​🇹​​​​​🇪​​​​​
resource "digitalocean_certificate" "certificate" {
  name = "default"
  type = "lets_encrypt"
  domains = [var.domain_name]
  lifecycle {
    create_before_destroy = true
  }
}

# 🇸​​​​​🇪​​​​​🇨​​​​​🇷​​​​​🇪​​​​​🇹​​​​​
resource "kubernetes_secret" "certificate" {
  metadata {
    name = "certificate"
  }

  data = {
    tls.crt = ""
    tls.key = ""
  }

  type = "kubernetes.io/tls"
}