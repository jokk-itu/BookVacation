#! /bin/sh
while ! nc -z localhost 7474; do sleep 1; done
docker exec -it neo4j sh -c "cypher-shell -u neo4j -p test -f /initialize.cypher"
