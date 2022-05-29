output "issuername" {
  value = module.cert_manager.cluster_issuer_name
}

output "private-key-ref" {
  value = module.cert_manager.cluster_issuer_private_key_name
}