#! /bin/sh

replicas=$1
namespace=$2
for (( i=1; i < $replicas; i++ ))
do
	curl -X PUT http://127.0.0.1:8080/admin/cluster/node?url=http://ravendb-$i.ravendb.$namespace.svc.cluster.local:8080
done


: '
provisioner "local-exec" {
    command = <<-EOT
      kubectl cp ./modules/ravendb/join_cluster.sh $NAMESPACE/ravendb-0:/
      kubectl exec --namespace test ravendb-0 -- bash /join_cluster.sh $REPLICAS $NAMESPACE
    EOT

    interpreter = ["/bin/sh", "-c"]

    environment = {
      NAMESPACE = var.namespace
      REPLICAS = var.replicas
    }
  }'