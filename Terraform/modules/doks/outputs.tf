output "id" {
  value = digitalocean_kubernetes_cluster.bookvacation.id
}

output "name" {
  value = digitalocean_kubernetes_cluster.bookvacation.name
}

output "host" {
  value = digitalocean_kubernetes_cluster.bookvacation.endpoint
}

output "token" {
  value = digitalocean_kubernetes_cluster.bookvacation.kube_config[0].token
}

output "certificate" {
  value = base64decode(digitalocean_kubernetes_cluster.bookvacation.kube_config[0].cluster_ca_certificate)
}

output "kubeconfig" {
  value = digitalocean_kubernetes_cluster.bookvacation.kube_config[0].raw_config
}