ARG ELASTIC_VERSION

# https://www.docker.elastic.co/
FROM docker.elastic.co/elasticsearch/elasticsearch:${ELASTIC_VERSION}

# Add your elasticsearch plugins setup here
# Example: RUN elasticsearch-plugin install analysis-icu

# ADD jvm.options /usr/share/elasticsearch/config/
# ADD elasticsearch.yml /usr/share/elasticsearch/config/
# USER root
# RUN chown elasticsearch:elasticsearch config/jvm.options config/elasticsearch.yml
# 
# USER elasticsearch
# EXPOSE 9200 9300