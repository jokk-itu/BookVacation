#! /bin/sh

replicas=$1
namespace=$2

do forloop, from 1 to N
for i in 1 $replicas
do
	curl -X PUT http://127.0.0.1:8080/admin/cluster/node?url=ravendb-$i.$namespace.svc.cluster.local
done