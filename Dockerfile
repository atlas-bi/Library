# to build
# docker build  --tag atlas_demo . --build-arg HOST=host PASSWORD=password USER=user

# to run locally
# docker run -i -t -p 1234:1234 -p 8983:8983 -e PORT=1234  -u 0 atlas_demo:latest

# to get in shell
# docker run --entrypoint sh -i -t -u 0 atlas_demo:latest

# to access webapp
# http://localhost:1234 

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /app
COPY web.sln .
COPY ["./web/web.csproj", "./web/"]
RUN dotnet restore

COPY ["./web/.", "./web/"]
WORKDIR "/app/web/"
RUN dotnet publish -c Release -o out

FROM python:3.10-alpine as search
WORKDIR /app
ARG USER
ARG PASSWORD
ARG HOST
# copy site
COPY --from=build ["/app/web/out", "./"]

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
RUN solr/bin/solr start -force -noprompt -v && sleep 20 && cd etl && python3 atlas_collections.py && python3 atlas_groups.py && python3 atlas_initiatives.py && \
    python3 atlas_lookups.py && python3 atlas_reports.py && python3 atlas_terms.py && \
    python3 atlas_users.py

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine
WORKDIR /app
RUN apk add --no-cache openjdk11 bash lsof
COPY --from=search ["/app", "./"]

ARG USER
ARG PASSWORD
ARG HOST

# create config
RUN echo "{\"solr\": {\"atlas_address\": \"http://localhost:8983/solr/atlas\"},\"ConnectionStrings\": {\"AtlasDatabase\": \"Server=$HOST;Database=atlas;User Id=$USER; Password=$PASSWORD; MultipleActiveResultSets=true\"}}" > appsettings.cust.json

# in release 2022.02.2 we need to change the name from atlas_dotnet to atlas_web
CMD solr/bin/solr start -force -noprompt && ASPNETCORE_URLS=http://*:$PORT dotnet "Atlas_Dotnet.dll"
