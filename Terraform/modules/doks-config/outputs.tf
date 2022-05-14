output "test_namespace" {
    value = kubernetes_namespace.test.metadata.0.name
}

output "prod_namespace" {
    value = kubernetes_namespace.production.metadata.0.name
}