variable "cluster_certificate" {
  type = string
}

variable "cluster_token" {
  type = string
}

variable "cluster_host" {
  type = string
}

variable "namespace" {
  type = string
}

variable "replicas" {
  type = number
  default = 1
}