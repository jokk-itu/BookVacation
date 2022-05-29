output "ingress-ip" {
  value = kubernetes_ingress.carservice.status.0.load_balancer.0.ingress.0.ip
}