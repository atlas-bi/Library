# to build
# docker build  --build-arg HOST=host PASSWORD=password USER=user --tag atlas_demo_search -f solr.Dockerfile .

# to run locally
# docker run -i -t -p 8983:8983 -e PORT=8983 -u 0 atlas_demo_search:latest

# to get in shell
# docker run --entrypoint sh -i -t -u 0 atlas_demo_search:latest

# to access webapp
# http://localhost:8983

FROM python:3.10-alpine as search
WORKDIR /app
ARG USER
ARG PASSWORD
ARG HOST
# copy site
COPY ["./web/solr", "./"]

# startup search and load data
RUN apk add --no-cache openjdk11 bash lsof python3-dev curl gcc git py3-pip gcc libc-dev g++ libffi-dev libxml2 unixodbc-dev && \
    pip3 install pyodbc pysolr pytz

# install sql server driver
RUN curl -O https://download.microsoft.com/download/e/4/e/e4e67866-dffd-428c-aac7-8d28ddafb39b/msodbcsql17_17.8.1.1-1_amd64.apk && \
    apk add --allow-untrusted msodbcsql17_17.8.1.1-1_amd64.apk

# pull solr etl
RUN mkdir etl && cd etl && git clone --depth 1 https://github.com/atlas-bi/Solr-Search-ETL.git .

# create settings
RUN cd etl && echo "SOLR_URL = \"http://localhost:8983/solr/atlas\"" > settings.py && \
    echo "SOLR_LOOKUP_URL = \"http://localhost:8983/solr/atlas_lookups\"" >> settings.py && \
    echo "SQL_CONN = \"SERVER=$HOST;DATABASE=atlas;UID=$USER;PWD=$PASSWORD\"" >> settings.py

# load search
RUN chmod -R 777 bin && bin/solr start -force -noprompt -v && sleep 20 && cd etl && python3 atlas_collections.py && python3 atlas_groups.py && python3 atlas_initiatives.py && \
    python3 atlas_lookups.py && python3 atlas_reports.py && python3 atlas_terms.py && \
    python3 atlas_users.py

FROM alpine:latest
WORKDIR /app
RUN apk add --no-cache openjdk11 bash lsof
COPY --from=search ["/app", "./"]

ARG USER
ARG PASSWORD
ARG HOST

# create config

RUN chmod -R 777 bin
# in release 2022.02.2 we need to change the name from atlas_dotnet to atlas_web
CMD bin/solr start -force -noprompt -f -p $PORT
