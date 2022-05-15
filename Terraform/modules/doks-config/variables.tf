variable "cluster_name" {
  type = string
}

variable "cluster_id" {
  type = string
}

variable "write_kubeconfig" {
  type        = bool
  default     = false
}

variable "cluster_certificate" {
  type = string
}

variable "cluster_token" {
  type = string
}

variable "cluster_host" {
  type = string
}

variable "kubeconfig" {
  type = string
}