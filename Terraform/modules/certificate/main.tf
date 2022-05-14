terraform {
  required_providers {
    digitalocean = {
      source  = "digitalocean/digitalocean"
      version = ">= 2.4.0"
    }
  }
}

# 🇨​​​​​🇪​​​​​🇷​​​​​🇹​​​​​🇮​​​​​🇫​​​​​🇮​​​​​🇨​​​​​🇦​​​​​🇹​​​​​🇪​​​​​
resource "digitalocean_certificate" "cert" {
  name = "default"
  type = "lets_encrypt"
  domains = [var.domain_name]
  lifecycle {
    create_before_destroy = true
  }
}