output "secretname" {
  value = kubernetes_secret.minio.metadata.0.name
}

output "configname" {
  value = kubernetes_config_map.minio.metadata.0.name
}