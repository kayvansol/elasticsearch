# Deploy Elasticsearch, Kibana &amp; Logstash with Docker Compose

![alt text](https://raw.githubusercontent.com/kayvansol/elasticsearch/main/img/0.png?raw=true)

Pull the related docker images :
```bash
docker pull docker.elastic.co/elasticsearch/elasticsearch:8.13.4
docker pull docker.elastic.co/kibana/kibana:8.13.4
docker pull docker.elastic.co/logstash/logstash:8.13.4
```
docker compose file :
```yaml
version: '3.7'

services:

  # The 'setup' service runs a one-off script which initializes users inside
  # Elasticsearch — such as 'logstash_internal' and 'kibana_system' — with the
  # values of the passwords defined in the '.env' file. It also creates the
  # roles required by some of these users.
  #
  # This task only needs to be performed once, during the *initial* startup of
  # the stack. Any subsequent run will reset the passwords of existing users to
  # the values defined inside the '.env' file, and the built-in roles to their
  # default permissions.
  #
  # By default, it is excluded from the services started by 'docker compose up'
  # due to the non-default profile it belongs to. To run it, either provide the
  # '--profile=setup' CLI flag to Compose commands, or "up" the service by name
  # such as 'docker compose up setup'.

  setup:
    profiles:
      - setup
    build:
      context: setup/
      args:
        ELASTIC_VERSION: ${ELASTIC_VERSION}
    init: true
    volumes:
      - ./setup/entrypoint.sh:/entrypoint.sh:ro,Z
      - ./setup/lib.sh:/lib.sh:ro,Z
      - ./setup/roles:/roles:ro,Z
    environment:
      ELASTIC_PASSWORD: ${ELASTIC_PASSWORD:-}
      LOGSTASH_INTERNAL_PASSWORD: ${LOGSTASH_INTERNAL_PASSWORD:-}
      KIBANA_SYSTEM_PASSWORD: ${KIBANA_SYSTEM_PASSWORD:-}
      METRICBEAT_INTERNAL_PASSWORD: ${METRICBEAT_INTERNAL_PASSWORD:-}
      FILEBEAT_INTERNAL_PASSWORD: ${FILEBEAT_INTERNAL_PASSWORD:-}
      HEARTBEAT_INTERNAL_PASSWORD: ${HEARTBEAT_INTERNAL_PASSWORD:-}
      MONITORING_INTERNAL_PASSWORD: ${MONITORING_INTERNAL_PASSWORD:-}
      BEATS_SYSTEM_PASSWORD: ${BEATS_SYSTEM_PASSWORD:-}
    networks:
      - elk
    depends_on:
      - elasticsearch

  elasticsearch:
    build:
      context: elasticsearch/
      args:
        ELASTIC_VERSION: ${ELASTIC_VERSION}
    volumes:
      #- ./elasticsearch/config/elasticsearch.yml:/usr/share/elasticsearch/config/elasticsearch.yml:ro,Z
      - elasticsearch:/usr/share/elasticsearch/data:Z
    ports:
      - 9200:9200
      - 9300:9300
    environment:
      node.name: elasticsearch
      ES_JAVA_OPTS: -Xms512m -Xmx512m
      # Bootstrap password.
      # Used to initialize the keystore during the initial startup of
      # Elasticsearch. Ignored on subsequent runs.
      ELASTIC_PASSWORD: ${ELASTIC_PASSWORD:-}
      # Use single node discovery in order to disable production mode and avoid bootstrap checks.
      # see: https://www.elastic.co/guide/en/elasticsearch/reference/current/bootstrap-checks.html
      discovery.type: single-node
    networks:
      - elk
    restart: unless-stopped

  logstash:
    build:
      context: logstash/
      args:
        ELASTIC_VERSION: ${ELASTIC_VERSION}
    volumes:
      #- ./logstash/config/logstash.yml:/usr/share/logstash/config/logstash.yml:ro,Z
      - ./logstash/pipeline:/usr/share/logstash/pipeline:ro,Z
    ports:
      - 5044:5044
      - 50000:50000/tcp
      - 50000:50000/udp
      - 9600:9600
    environment:
      LS_JAVA_OPTS: -Xms256m -Xmx256m
      LOGSTASH_INTERNAL_PASSWORD: ${LOGSTASH_INTERNAL_PASSWORD:-}
    networks:
      - elk
    depends_on:
      - elasticsearch
    restart: unless-stopped

  kibana:
    build:
      context: kibana/
      args:
        ELASTIC_VERSION: ${ELASTIC_VERSION}
    #volumes:
    #  - ./kibana/config/kibana.yml:/usr/share/kibana/config/kibana.yml:ro,Z
    ports:
      - 5601:5601
    environment:
      KIBANA_SYSTEM_PASSWORD: ${KIBANA_SYSTEM_PASSWORD:-}
    networks:
      - elk
    depends_on:
      - elasticsearch
    restart: unless-stopped

networks:
  elk:
    driver: bridge

volumes:
  elasticsearch:

```

Start your deployment :
```bash
docker compose up
```

test the elastic node on browser :

![alt text](https://raw.githubusercontent.com/kayvansol/elasticsearch/main/img/1.png?raw=true)

set elasticsearch on etc/hosts (dns record) :

![alt text](https://raw.githubusercontent.com/kayvansol/elasticsearch/main/img/2.png?raw=true)

on the elasticsearch node, write the below command :
```bash
bin/elasticsearch-reset-password -u kibana_system --auto
```

![alt text](https://raw.githubusercontent.com/kayvansol/elasticsearch/main/img/3.png?raw=true)

and copy the result to the kibana node and kibana.yaml :
```
elasticsearch.username: "kibana_system"
elasticsearch.password: "q7pgSbw_JYm9ehk-=kc+"
```

![alt text](https://raw.githubusercontent.com/kayvansol/elasticsearch/main/img/4.png?raw=true)

and go to kibana portal :

![alt text](https://raw.githubusercontent.com/kayvansol/elasticsearch/main/img/5.png?raw=true)

and get the elastic node healthy info from kibana :
```
GET /_cluster/health?pretty
```

![alt text](https://raw.githubusercontent.com/kayvansol/elasticsearch/main/img/6.png?raw=true)


on the elasticsearch node, write the below command :
```bash
bin/elasticsearch-reset-password -u logstash_system --auto
```

![alt text](https://raw.githubusercontent.com/kayvansol/elasticsearch/main/img/7.png?raw=true)

and copy the result to the logstash node and logstash.yaml :
```
xpack.monitoring.elasticsearch.username: "logstash_system"
xpack.monitoring.elasticsearch.password: "nMv9pFyreKMIkzQh-GEy"
```

![alt text](https://raw.githubusercontent.com/kayvansol/elasticsearch/main/img/8.png?raw=true)

then change the elastoc node auth info in the logstash.conf :
```conf
output {
	elasticsearch {
		hosts => "elasticsearch:9200"
		#user => "logstash_internal"
		#password => "${LOGSTASH_INTERNAL_PASSWORD}"
		user => "elastic"
		password => "changeme"
	}
}
```

![alt text](https://raw.githubusercontent.com/kayvansol/elasticsearch/main/img/9.png?raw=true)

check the logstash log :

![alt text](https://raw.githubusercontent.com/kayvansol/elasticsearch/main/img/10.png?raw=true)

and logstash info :

![alt text](https://raw.githubusercontent.com/kayvansol/elasticsearch/main/img/11.png?raw=true)

and the docker desktop stack shows the stack healthy :

![alt text](https://raw.githubusercontent.com/kayvansol/elasticsearch/main/img/12.png?raw=true)

