terraform {
    required_providers {
        digitalocean = {
            source = "digitalocean/digitalocean"
            version = ">= 2.4.0"
        }
    }
}

data "digitalocean_kubernetes_versions" "version" {
  version_prefix = var.cluster_version
}

# π°βββββπΊβββββπ§βββββπͺβββββπ¨βββββπ±βββββπΊβββββπΈβββββπΉβββββπͺβββββπ·βββββ
resource "digitalocean_kubernetes_cluster" "bookvacation" {
    name = var.cluster_name
    region = var.cluster_region
    version = data.digitalocean_kubernetes_versions.version.latest_version
    auto_upgrade = true

    maintenance_policy {
      start_time  = "01:00"
      day         = "sunday"
    }

    node_pool {
      name = "default"
      size = "s-4vcpu-8gb"
      node_count = 3
    }
}