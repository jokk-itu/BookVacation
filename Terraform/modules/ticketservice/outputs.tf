output "ingress-ip" {
  value = kubernetes_ingress_v1.ticketservice.status.0.load_balancer.0.ingress.0.ip
}