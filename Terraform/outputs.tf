output "cluster_name" {
  value = module.doks.cluster_name
}

output "kubeconfig_path" {
  value = var.write_kubeconfig ? abspath("${path.root}/kubeconfig") : "none"
}