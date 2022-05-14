terraform {
  required_providers {
    digitalocean = {
      source  = "digitalocean/digitalocean"
      version = ">= 2.4.0"
    }
  }
}

# 🇮​​​​​🇳​​​​​🇬​​​​​🇷​​​​​🇪​​​​​🇸​​​​​🇸​​​​​ 🇱​​​​​🇴​​​​​🇦​​​​​🇩​​​​​🇧​​​​​🇦​​​​​🇱​​​​​🇦​​​​​🇳​​​​​🇨​​​​​🇪​​​​​🇷​​​​​
resource "digitalocean_loadbalancer" "ingress_loadbalancer" {
  name = "ingress-loadbalancer"
  region = "lon1"
  size = "lb-small"
  algorithm = "round_robin"
  redirect_http_to_https = true

  forwarding_rule {
    entry_port = 443
    entry_protocol = "https"

    target_port = 80
    target_protocol = "http"

    certificate_name = var.certificate_name
  }

  lifecycle {
    ignore_changes = [
      forwarding_rule
    ]
  }
}