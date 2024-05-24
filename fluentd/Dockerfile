FROM fluentd:latest
USER root
RUN ["gem", "install", "elasticsearch", "--no-document", "--version", "8.13.4"]
RUN ["gem", "install", "fluent-plugin-elasticsearch", "--no-document", "--version", "5.4.3"]
USER fluent