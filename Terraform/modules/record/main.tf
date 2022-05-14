terraform {
  required_providers {
    digitalocean = {
      source  = "digitalocean/digitalocean"
      version = ">= 2.4.0"
    }
  }
}

# 🇷​​​​​🇪​​​​​🇨​​​​​🇴​​​​​🇷​​​​​🇩​​​​​
resource "digitalocean_record" "static" {
  domain = var.domain_name
  type = "A"
  ttl = "60"
  name = "@"
  value = var.loadbalancer_ip
}