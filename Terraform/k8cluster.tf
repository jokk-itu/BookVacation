data "digitalocean_kubernetes_versions" "version" {
  version_prefix = "1.22."
}

resource "digitalocean_kubernetes_cluster" "bookvacation" {
    name = "bookvacation"
    region = "lon1"
    version = digitalocean_kubernetes_versions.version.latest_version
    auto_upgrade = true

    maintenance_policy {
        start_time  = "01:00"
        day         = "sunday"
    }

    node_pool {
        name = "default"
        size = "s-1vcpu-2gb"
        node_count = 1
    }
}