### run local Elasticsearch + Kibana

```bash
# $ docker network create elastic
$ docker pull docker.elastic.co/elasticsearch/elasticsearch:8.17.3
# $ --net elastic
$ docker run -d --name es01 -p 9200:9200 -it -m 1GB -e http.host=0.0.0.0 -e network.host=0.0.0.0 -e xpack.security.enrollment.enabled=true -e xpack.security.authc.api_key.enabled=true -e discovery.type=single-node docker.elastic.co/elasticsearch/elasticsearch:8.17.3
$ docker exec -it es01 /usr/share/elasticsearch/bin/elasticsearch-reset-password -u elastic
# $ export ELASTIC_PASSWORD="test000"
# $ docker cp es01:/usr/share/elasticsearch/config/certs/http_ca.crt .
# $ curl --cacert http_ca.crt -u elastic:$ELASTIC_PASSWORD https://localhost:9200
$ 
$ docker pull docker.elastic.co/kibana/kibana:8.17.3
$ docker run -d --name kib01 -p 5601:5601 docker.elastic.co/kibana/kibana:8.17.3
$ docker exec -it es01 /usr/share/elasticsearch/bin/elasticsearch-create-enrollment-token -s kibana
$ docker exec -it es01 /usr/share/elasticsearch/bin/elasticsearch-reset-password -u elastic
$ docker exec -it kib01 kibana-verification-code
```