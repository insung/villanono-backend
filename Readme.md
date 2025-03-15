### run local Elasticsearch + Kibana

```bash
$ docker network create elastic
$ docker pull docker.elastic.co/elasticsearch/elasticsearch:8.17.3
$ docker run -d --name es01 --net elastic -p 9200:9200 -it -m 1GB docker.elastic.co/elasticsearch/elasticsearch:8.17.3
$ docker exec -it es01 /usr/share/elasticsearch/bin/elasticsearch-reset-password -u elastic
# $ export ELASTIC_PASSWORD="test000"
# $ docker cp es01:/usr/share/elasticsearch/config/certs/http_ca.crt .
# $ curl --cacert http_ca.crt -u elastic:$ELASTIC_PASSWORD https://localhost:9200
$ 
$ docker pull docker.elastic.co/kibana/kibana:8.17.3
$ docker run -d --name kib01 --net elastic -p 5601:5601 docker.elastic.co/kibana/kibana:8.17.3
$ docker exec -it es01 /usr/share/elasticsearch/bin/elasticsearch-create-enrollment-token -s kibana
$ docker exec -it es01 /usr/share/elasticsearch/bin/elasticsearch-reset-password -u elastic
$ docker exec -it kib01 kibana-verification-code
```

jcAFJWzwNtOaVSR+DZRi
eyJ2ZXIiOiI4LjE0LjAiLCJhZHIiOlsiMTcyLjE4LjAuMjo5MjAwIl0sImZnciI6ImNmZDdhZDgzMGMxZjQ2NWRjNWYxMzljNWQ2YzQwYTU3YzEzYjM2Mjc2NGM3NGZlN2NmNWMxYzgzN2JlYTk4MjQiLCJrZXkiOiJIR3F0bEpVQnFDMnlWRGNQVDdnZzp1Mzc5NmFMOVFTR1lkTWo4VUd0T0N3In0=